# Deliveryix — Entra External ID Setup Guide

This guide covers the setup of Microsoft Entra External ID for the Deliveryix project, scoped to the signup flow with email/document uniqueness validation.

---

## Prerequisites

- Azure account with at least the **Tenant Creator** role
- Azure Functions project running locally (use a dev tunnel to expose it publicly)
  ```bash
  devtunnel host -p 7071 --allow-anonymous
  ```

---

## Phase 1 — Create the External Tenant

1. Go to [entra.microsoft.com](https://entra.microsoft.com)
2. Navigate to **Entra ID → Overview → Manage tenants → Create**
3. Select **External** and click **Continue**
4. Fill in:
   - **Tenant Name**: `Deliveryix`
   - **Domain name**: `deliveryix` → becomes `deliveryix.onmicrosoft.com`
   - **Country/Region**: Brazil
5. Click **Review + Create → Create**
6. Wait up to 30 minutes for provisioning

> ⚠️ All subsequent steps must be performed **inside the external tenant**. Switch via the ⚙️ icon → **Directories + subscriptions**.

---

## Phase 2 — Register the Frontend App (SPA)

1. Go to **Entra ID → App registrations → New registration**
2. Fill in:
   - **Name**: `DeliveryixWebApp`
   - **Supported account types**: *Accounts in this organizational directory only*
   - **Redirect URI**: platform = **Single-page application (SPA)**, value = `http://localhost:3000`
3. Click **Register**
4. Note the **Application (client) ID** and **Directory (tenant) ID** — you will need these in your frontend MSAL config

---

## Phase 3 — Create Custom User Attributes

These attributes are collected during signup and validated by the custom extension.

1. Go to **Entra ID → External Identities → Custom user attributes → Add**
2. Create each attribute below:

| Name | Data type | Description |
|---|---|---|
| `DocumentNumber` | String | CPF or CNPJ |
| `DocumentType` | String | CPF or CNPJ |
| `AccountType` | String | Individual or Business |
| `PhoneNumber` | String | Mobile phone with country code |

> Internally, each attribute is stored as `extension_<AppId>_<AttributeName>`.

---

## Phase 4 — Create the Custom Authentication Extension

### 4a — Create the extension

1. Go to **Entra ID → External Identities → Custom authentication extensions → Create a custom extension**
2. On the **Basics** tab, select **AttributeCollectionSubmit** → **Next**
3. On **Endpoint Configuration**, fill in:
   - **Name**: `ValidateIdentityBeforeCreation`
   - **Target URL**: `https://<your-tunnel-or-function-url>/api/ValidateIdentityBeforeCreation`
   - **Description**: Validates email and document uniqueness before account creation
4. Click **Next**
5. On **API Authentication**, select **Create new app registration**:
   - **Name**: `DeliveryixValidateIdentityBeforeCreationFunction`
6. Click **Next → Create**

### 4b — Grant admin consent

1. On the extension's **Overview** page, click **Grant permission**
2. A consent dialog appears for the **Receive custom authentication extension HTTP requests** permission
3. Click **Accept**


---

## Phase 5 — Create the User Flow

1. Go to **Entra ID → External Identities → User flows → New user flow**
2. Fill in:
   - **Name**: `signup_signin` → the portal prefixes it as `deliveryix_signup_signin`
3. Under **Identity providers**, select:
   - ✅ **Email with password**
4. Under **User attributes**, click **Show more** and select:
   - ✅ **Display Name**
   - ✅ **AccountType**
   - ✅ **DocumentNumber**
   - ✅ **DocumentType**
   - ✅ **PhoneNumber**
5. Click **Create**

### Associate the frontend app with the user flow

6. Inside the created user flow, go to **Applications → Add application**
7. Select `DeliveryixWebApp`
8. Save

---

## Phase 6 — Associate the Extension with the User Flow

1. Inside the user flow (`deliveryix_signup_signin`), go to **Custom authentication extensions**
2. Next to **"When a user submits their information"**, click the pencil ✏️
3. Select `ValidateIdentityBeforeCreation`
4. Click **Select → Save**

---

## Signup Flow

```
User fills in attributes on the Entra signup page
  → Entra calls ValidateIdentityBeforeCreation (OnAttributeCollectionSubmit)
    → Function reads email from identities[] and DocumentNumber from attributes{}
    → Queries the Identity module database to check uniqueness
    → Duplicate found: returns showBlockPage
    → Unique: returns continueWithDefaultBehavior
  → Entra creates the account in the directory
  → Token is issued to the user
```
