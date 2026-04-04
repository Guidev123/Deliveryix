# deliveryix-jwt-frontend-generator

A PowerShell script that automates the full OAuth 2.0 Authorization Code + PKCE flow against **Azure Entra External ID**, retrieves an enriched identity profile from the local IAM API, and hands you a ready-to-use Bearer token for local development and API testing.

---

## What it does

1. Builds an authorization URL and opens your default browser.
2. Spins up a temporary local HTTP listener to capture the OAuth callback.
3. Exchanges the authorization code for an access token (PKCE, no client secret).
4. Extracts the `oid` claim from the JWT payload.
5. Calls the local IAM API to fetch the user's roles and permissions.
6. Prints the Bearer token and the enriched headers that APIM would inject in a real environment (`X-Identity-Id`, `X-Roles`, `X-Permissions`).
7. Copies `Bearer <token>` to your clipboard automatically.
8. Exports a ready-to-import **Postman environment** (`deliveryix-dev.postman_environment.json`) alongside the script.

---

## Prerequisites

- PowerShell 7+ (Windows, macOS, or Linux)
- An **App Registration** configured in your Entra External ID tenant:
  - Platform: **Single-page application (SPA)**
  - Redirect URI: `http://localhost:<RedirectPort>` (must match the `-RedirectPort` parameter)
  - API permission: the scope passed in `-ApiScope` granted and consented
- The **IAM API** running locally at the address passed in `-IamApiBase`

---

## Parameters

| Parameter | Description |
|---|---|
| `TenantSubdomain` | The subdomain of your Entra External ID tenant (e.g. `contoso` for `contoso.ciamlogin.com`) |
| `TenantId` | The tenant GUID from the Azure portal |
| `ClientId` | The Application (client) ID of the SPA App Registration |
| `ApiScope` | The API scope to request (e.g. `api://deliveryix-iam/access_as_user`) |
| `RedirectPort` | The local port for the OAuth callback listener (e.g. `3000`) |
| `IamApiBase` | Base URL of the local IAM API (e.g. `https://localhost:7020`) |
| `IamProfileRoute` | Route of the identity profile endpoint (e.g. `/api/v1/identity/profile`) |

All parameters are **mandatory**. The script will error immediately if any are missing.

---

## Usage

```powershell
powershell -ExecutionPolicy Bypass -File .\deliveryix-jwt-frontend-generator.ps1 `
  -TenantSubdomain "contoso" `
  -TenantId        "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" `
  -ClientId        "yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy" `
  -ApiScope        "api://deliveryix-iam/access_as_user" `
  -RedirectPort    "3000" `
  -IamApiBase      "https://localhost:7020" `
  -IamProfileRoute "/api/v1/identity/profile"
```

After authenticating in the browser, the terminal will display:

```
  Authorization Header:
  Bearer eyJ0eXAiOiJKV1QiLCJhbGci...

  Additional headers (simulating APIM enrichment):
  X-Identity-Id:  xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
  X-Roles:        ["admin","operator"]
  X-Permissions:  ["orders:read","orders:write"]
```

The Bearer token is also copied to your clipboard automatically.

---

## Postman

After each run, a `deliveryix-dev.postman_environment.json` file is generated in the same directory as the script. It contains the following variables:

| Variable | Description |
|---|---|
| `base_url` | The IAM API base URL |
| `bearer_token` | The access token (secret) |
| `identity_id` | The user's identity ID returned by the IAM API |
| `roles` | JSON array of the user's roles |
| `permissions` | JSON array of the user's permissions |

To import it: **Postman → Environments → Import**.

---

## Security notes

- This script is intended **for local development only**. Never use it in CI/CD pipelines or shared environments.
- The script bypasses SSL certificate validation to support self-signed certificates on local APIs. This behavior is scoped to the current PowerShell session.
- No credentials are stored or logged. The token exists only in memory and in your clipboard for the duration of the session.