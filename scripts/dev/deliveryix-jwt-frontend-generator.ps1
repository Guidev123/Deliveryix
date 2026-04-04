param(
    [Parameter(Mandatory)][string]$TenantSubdomain,
    [Parameter(Mandatory)][string]$TenantId,
    [Parameter(Mandatory)][string]$ClientId,
    [Parameter(Mandatory)][string]$ApiScope,
    [Parameter(Mandatory)][string]$RedirectPort,
    [Parameter(Mandatory)][string]$IamApiBase,
    [Parameter(Mandatory)][string]$IamProfileRoute
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Web

function New-PkceChallenge {
    $bytes = New-Object byte[] 32
    [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
    $verifier  = [Convert]::ToBase64String($bytes) -replace '\+','-' -replace '/','_' -replace '=',''
    $sha256    = [System.Security.Cryptography.SHA256]::Create()
    $hash      = $sha256.ComputeHash([System.Text.Encoding]::ASCII.GetBytes($verifier))
    $challenge = [Convert]::ToBase64String($hash) -replace '\+','-' -replace '/','_' -replace '=',''
    return @{ Verifier = $verifier; Challenge = $challenge }
}

function New-RandomState {
    $bytes = New-Object byte[] 16
    [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
    return [Convert]::ToBase64String($bytes) -replace '[^a-zA-Z0-9]',''
}

function Read-OidFromToken([string]$AccessToken) {
    $payload = $AccessToken.Split('.')[1]
    $mod = $payload.Length % 4
    if ($mod -eq 2) { $payload += "==" }
    elseif ($mod -eq 3) { $payload += "=" }
    $payload = $payload -replace '-','+' -replace '_','/'
    $json = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($payload))
    return ($json | ConvertFrom-Json).oid
}

function Write-Banner([string]$Text) {
    $line = "=" * ($Text.Length + 4)
    Write-Host ""
    Write-Host $line -ForegroundColor Cyan
    Write-Host "  $Text" -ForegroundColor Cyan
    Write-Host $line -ForegroundColor Cyan
    Write-Host ""
}

function Write-Step([string]$Text) {
    Write-Host "  >> $Text" -ForegroundColor Yellow
}

function Write-Ok([string]$Text) {
    Write-Host "  OK  $Text" -ForegroundColor Green
}

function Write-Fail([string]$Text) {
    Write-Host "  ERROR  $Text" -ForegroundColor Red
}

# ------------------------------------------------------------------------------
# STEP 1 - Build authorization URL and open browser
# ------------------------------------------------------------------------------

Write-Banner "Deliveryix Dev Token Generator"

$pkce        = New-PkceChallenge
$state       = New-RandomState
$redirectUri = "http://localhost:$RedirectPort"
$authority   = "https://$TenantSubdomain.ciamlogin.com/$TenantId/oauth2/v2.0"
$scopeParam  = [Uri]::EscapeDataString("openid profile $ApiScope")
$redirectEnc = [Uri]::EscapeDataString($redirectUri)

$authUrl = "$authority/authorize" +
           "?client_id=$ClientId" +
           "&response_type=code" +
           "&redirect_uri=$redirectEnc" +
           "&scope=$scopeParam" +
           "&code_challenge=$($pkce.Challenge)" +
           "&code_challenge_method=S256" +
           "&state=$state" +
           "&response_mode=query"

Write-Step "Opening browser for authentication on Entra External ID..."
Start-Process $authUrl

# ------------------------------------------------------------------------------
# STEP 2 - Local HTTP listener to capture the callback
# ------------------------------------------------------------------------------

Write-Step "Waiting for callback at $redirectUri ..."

$listener = New-Object System.Net.HttpListener
$listener.Prefixes.Add("$redirectUri/")
$listener.Start()

$context  = $listener.GetContext()
$rawQuery = $context.Request.Url.Query

$html   = "<html><body style='font-family:sans-serif;text-align:center;margin-top:80px'><h2 style='color:#0f6e56'>Authenticated!</h2><p>You can close this tab and return to the terminal.</p></body></html>"
$buffer = [System.Text.Encoding]::UTF8.GetBytes($html)
$context.Response.ContentType      = "text/html"
$context.Response.ContentLength64  = $buffer.Length
$context.Response.OutputStream.Write($buffer, 0, $buffer.Length)
$context.Response.OutputStream.Close()
$listener.Stop()

$queryParams   = [System.Web.HttpUtility]::ParseQueryString($rawQuery)
$returnedState = $queryParams["state"]
$code          = $queryParams["code"]

if ($returnedState -ne $state) {
    Write-Fail "State mismatch - possible CSRF attack. Aborting."
    exit 1
}

if (-not $code) {
    Write-Fail "Authorization code not received. Check the redirect URI in the App Registration."
    exit 1
}

Write-Ok "Authorization code received."

# ------------------------------------------------------------------------------
# STEP 3 - Exchange the code for an access token
# ------------------------------------------------------------------------------

Write-Step "Exchanging authorization code for access token..."

$tokenBody = @{
    client_id     = $ClientId
    grant_type    = "authorization_code"
    code          = $code
    redirect_uri  = $redirectUri
    code_verifier = $pkce.Verifier
    scope         = "openid profile $ApiScope"
}

try {
    $tokenResponse = Invoke-RestMethod `
        -Method Post `
        -Uri "$authority/token" `
        -ContentType "application/x-www-form-urlencoded" `
        -Headers @{ Origin = $redirectUri } `
        -Body $tokenBody
} catch {
    Write-Fail "Failed to obtain token: $_"
    exit 1
}

$accessToken = $tokenResponse.access_token
Write-Ok "Access token obtained."

# ------------------------------------------------------------------------------
# STEP 4 - Extract oid from the token
# ------------------------------------------------------------------------------

Write-Step "Extracting oid from token..."

try {
    $oid = Read-OidFromToken $accessToken
    Write-Ok "oid: $oid"
} catch {
    Write-Fail "Could not extract oid from token: $_"
    exit 1
}

# ------------------------------------------------------------------------------
# STEP 5 - Bypass self-signed certificate and call local IAM API
# ------------------------------------------------------------------------------

Write-Step "Querying local IAM API at $IamApiBase$IamProfileRoute..."

if (-not ("TrustAllCertsPolicy" -as [type])) {
    Add-Type @"
using System.Net;
using System.Security.Cryptography.X509Certificates;
public class TrustAllCertsPolicy : ICertificatePolicy {
    public bool CheckValidationResult(
        ServicePoint srvPoint, X509Certificate certificate,
        WebRequest request, int certificateProblem) { return true; }
}
"@
}
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy

try {
    $iamResponse = Invoke-RestMethod `
        -Method Get `
        -Uri "$IamApiBase$IamProfileRoute" `
        -Headers @{ Authorization = "Bearer $accessToken" }
} catch {
    Write-Fail "Failed to call IAM API: $_"
    Write-Host "  Make sure the IAM API is running at $IamApiBase" -ForegroundColor DarkYellow
    exit 1
}

Write-Ok "Profile returned for identityId: $($iamResponse.identityId)"

# ------------------------------------------------------------------------------
# STEP 6 - Build enriched claims
# ------------------------------------------------------------------------------

Write-Step "Building enriched claims..."

$roles       = [string[]]@($iamResponse.roles | ForEach-Object { $_.roleName })
$permissions = [string[]]@($iamResponse.roles | ForEach-Object { $_.permissions } | ForEach-Object { $_ } | Sort-Object -Unique)

Write-Host ""
Write-Host "  Roles:" -ForegroundColor White
$roles | ForEach-Object { Write-Host "    - $_" -ForegroundColor Gray }

Write-Host ""
Write-Host "  Permissions:" -ForegroundColor White
$permissions | ForEach-Object { Write-Host "    - $_" -ForegroundColor Gray }

# ------------------------------------------------------------------------------
# STEP 7 - Display result
# ------------------------------------------------------------------------------

Write-Banner "Token ready for use"

$rolesJson       = ConvertTo-Json -InputObject $roles       -Compress
$permissionsJson = ConvertTo-Json -InputObject $permissions -Compress

Write-Host "  Authorization Header:" -ForegroundColor White
Write-Host "  Bearer $accessToken" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  Additional headers (simulating APIM enrichment):" -ForegroundColor White
Write-Host "  X-Identity-Id:  $($iamResponse.identityId)" -ForegroundColor DarkGray
Write-Host "  X-Roles:        $rolesJson" -ForegroundColor DarkGray
Write-Host "  X-Permissions:  $permissionsJson" -ForegroundColor DarkGray

# ------------------------------------------------------------------------------
# STEP 8 - Copy to clipboard
# ------------------------------------------------------------------------------

"Bearer $accessToken" | Set-Clipboard

Write-Host ""
Write-Ok "Bearer token copied to clipboard."
Write-Host ""
Write-Host "  Paste in Postman at: Authorization > Bearer Token > Token" -ForegroundColor DarkYellow

# ------------------------------------------------------------------------------
# BONUS - Export Postman environment
# ------------------------------------------------------------------------------

$envFile = Join-Path $PSScriptRoot "deliveryix-dev.postman_environment.json"

$envObject = @{
    id     = [guid]::NewGuid().ToString()
    name   = "Deliveryix Dev"
    values = @(
        @{ key = "base_url";     value = $IamApiBase;             enabled = $true; type = "default" }
        @{ key = "bearer_token"; value = $accessToken;            enabled = $true; type = "secret"  }
        @{ key = "identity_id";  value = $iamResponse.identityId; enabled = $true; type = "default" }
        @{ key = "roles";        value = $rolesJson;              enabled = $true; type = "default" }
        @{ key = "permissions";  value = $permissionsJson;        enabled = $true; type = "default" }
    )
    _postman_variable_scope = "environment"
}

$envObject | ConvertTo-Json -Depth 5 | Out-File -FilePath $envFile -Encoding UTF8

Write-Ok "Postman environment exported to: $envFile"
Write-Host "  Import at: Environments > Import in Postman" -ForegroundColor DarkYellow
Write-Host ""