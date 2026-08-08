# Winstaller Examples – Usage and Configuration Reference

This repository contains a comprehensive suite of example projects and JSON configurations for the **Winstaller** installation engine and tooling. The examples range from minimal setups to advanced custom C#/WPF UI steps, script plugins, multi-language localization, and a production-grade application installer featuring a Windows Service and Code Signing.

---

## 📁 Repository Structure

```text
Winstaller.Examples/
├── 01-Minimal/
│   └── minimal.json
├── 02-CustomSteps/
│   ├── custom_steps.json
│   └── Winstaller.CustomSteps/
│       └── Winstaller.CustomSteps.csproj
├── 03-Plugins/
│   ├── plugin.json
│   ├── plugin_error.json
│   └── Winstaller.Plugins/
│       └── Winstaller.Plugins.csproj
├── 04-Translations/
│   └── translations.json
└── 05-FullApplication/
    ├── fullapp.json
    ├── app.ico
    ├── license.rtf
    ├── winstaller-test.pfx
    ├── install-test-cert.ps1
    └── Solution/
        ├── WpfApp/
        │   └── WpfApp.csproj
        └── DemoService/
            └── DemoService.csproj
```

---

## 🚀 Examples Overview

### 1. `01-Minimal` – Minimal Setup Configuration
* **Directory:** `01-Minimal`
* **Configuration File:** `01-Minimal/minimal.json`
* **Description:** Demonstrates the simplest possible installer setup. Uses built-in default steps (`welcome`, `license`, `installtype`, `install`, `success`) and uninstallation steps (`confirmuninstall`, `uninstall`). Perfect starting point for simple applications.

---

### 2. `02-CustomSteps` – Custom Installer Steps (C# / WPF)
* **Directory:** `02-CustomSteps`
* **C# Project:** `02-CustomSteps/Winstaller.CustomSteps/Winstaller.CustomSteps.csproj`
* **Configuration File:** `02-CustomSteps/custom_steps.json`
* **Description:** Shows how to create custom installer UI steps using C# and WPF. The project contains classes and XAML views implementing `IStep` and inheriting from `StepViewBase`:
  * `MyWelcome` – Welcome screen with custom layout.
  * `MyLicense` – License screen with dynamic acceptance checking.
  * `MyInstallType` – Installation scope selection (per-user vs. per-machine).
  * `MyInstall` – Progress bar with error retry loop handling.
  * `MySuccess` – Completion screen.
  * `MyConfirmUninstall` & `MyUninstall` – Customized uninstallation workflow.

---

### 3. `03-Plugins` – Script Plugins (Pre-Install / Post-Install)
* **Directory:** `03-Plugins`
* **C# Project:** `03-Plugins/Winstaller.Plugins/Winstaller.Plugins.csproj`
* **Configuration Files:**
  * `03-Plugins/plugin.json` – Standard script plugin execution
  * `03-Plugins/plugin_error.json` – Error handling and rollback verification
* **Description:** Demonstrates creating standalone script plugins implementing the `IScript` interface:
  * `DemoPreInstallScript` – Executed before file copying starts.
  * `DemoPostInstallScript` – Executed after file copying finishes.
  * `DemoPreInstallErrorScript` & `DemoPostInstallErrorScript` – Test scripts that throw exceptions to verify Winstaller's automatic rollback procedure.

---

### 4. `04-Translations` – Localization and Translation Overrides
* **Directory:** `04-Translations`
* **Configuration File:** `04-Translations/translations.json`
* **Description:** Demonstrates Winstaller's multi-language localization features:
  * Restricting available installer languages using `allowedLanguages` (`["en", "es"]`).
  * Overriding all built-in English (`en`) translation keys in the JSON configuration with custom prefixed strings (e.g. `[EN]Next`).
  * Defining custom Spanish (`es`) translations.

---

### 5. `05-FullApplication` – Production Application with Windows Service & Code Signing
* **Directory:** `05-FullApplication`
* **C# Solution:** `05-FullApplication/Solution/Winstaller.FullApp.slnx`
  * `05-FullApplication/Solution/WpfApp/WpfApp.csproj` – WPF Desktop Application (.NET Framework 4.7.2) with embedded `app.ico`.
  * `05-FullApplication/Solution/DemoService/DemoService.csproj` – Background Windows Service (.NET Framework 4.7.2) logging heartbeat activity every second to disk (`ProgramData\DemoService\service_activity.log`).
* **Configuration File:** `05-FullApplication/fullapp.json`
* **Additional Files:**
  * `05-FullApplication/app.ico` – Icon for installer UI, desktop shortcut, and compiled EXE.
  * `05-FullApplication/license.rtf` – EULA document in RTF format.
  * `05-FullApplication/winstaller-test.pfx` – Self-signed Code Signing certificate.
  * `05-FullApplication/install-test-cert.ps1` – PowerShell script to import the test certificate into Windows Certificate Store.
* **Description:** A complete, production-grade installer package that installs a WPF desktop application, automatically registers a Windows Service (`service`), creates a desktop shortcut with an icon, configures environment path variables (`envPath`), and automatically signs the generated installer binary via Code Signing.

---

## 🔑 Deep Dive: Code Signing & Self-Signed Certificates (Example 05)

### Why Code Signing Matters
Code signing attaches a digital signature to the built installer `.exe` file. This signature accomplishes two critical goals:
1. **Authenticity & Integrity**: Proves the installer originated from a verified publisher and has not been altered or tampered with by third parties.
2. **User Experience (SmartScreen & UAC)**: Prevents Windows SmartScreen from blocking the installer with a red/yellow warning banner ("Unknown Publisher / Windows protected your PC").

### How Code Signing Works in Winstaller
Winstaller natively supports automated code signing during the build process. In `05-FullApplication/fullapp.json`, the `"signing"` configuration section is declared:

```json
"signing": {
    "certificatePath": "winstaller-test.pfx",
    "certificatePassword": "pA$$w0rd",
    "timestampUrl": "http://timestamp.digicert.com"
}
```

### Understanding Self-Signed Certificates & The Certificate Store

#### Why Windows Displays Warnings for Self-Signed Certificates
The certificate included in Example 05 (`winstaller-test.pfx`) is a **self-signed certificate** created for testing purposes. 

By default, Windows trusts digital signatures **only** if the signing certificate was issued by a Certificate Authority (CA) listed in the Windows **Trusted Root Certification Authorities** store. Because a self-signed certificate is not issued by a public CA (like DigiCert or Sectigo), Windows treats it as untrusted out-of-the-box and displays a warning banner ("Publisher: Unknown").

#### Making the Self-Signed Certificate Trusted on Test Machines
To make Windows trust installers signed with `winstaller-test.pfx` on a development or test machine:

1. Open PowerShell as **Administrator**.
2. Run the provided helper script `install-test-cert.ps1`:
   ```powershell
   powershell -ExecutionPolicy Bypass -File 05-FullApplication/install-test-cert.ps1
   ```
   *Alternatively, run the equivalent PowerShell command directly:*
   ```powershell
   Import-PfxCertificate -FilePath "05-FullApplication/winstaller-test.pfx" -CertStoreLocation Cert:\LocalMachine\Root -Password (ConvertTo-SecureString "pA$$w0rd" -AsPlainText -Force)
   ```

**What this does:** This command installs the root public key of `winstaller-test.pfx` into the machine's `Cert:\LocalMachine\Root` (Trusted Root Certification Authorities) store. Once imported, Windows recognizes installers signed with this certificate as trusted on that computer, displaying a clean blue UAC prompt with publisher "Winstaller Test Publisher".

#### Production vs. Development Code Signing
* **Development / Testing**: Use self-signed `.pfx` certificates along with `install-test-cert.ps1` to test signed installer builds without incurring certificate costs.
* **Production Releases**: Purchase a valid EV (Extended Validation) or Standard Code Signing Certificate from a publicly trusted CA (e.g., DigiCert, Sectigo, GlobalSign). Publicly trusted certificates are automatically trusted by all Windows machines without requiring users to run any certificate import scripts.

---

## 🛠️ How to Build and Run the Examples

### 1. Building the C# Projects
All C# projects target `.NET Framework 4.7.2` using SDK-style project files.

To compile the projects via .NET CLI:

```cmd
# 02-CustomSteps project:
dotnet build 02-CustomSteps/Winstaller.CustomSteps/Winstaller.CustomSteps.csproj

# 03-Plugins project:
dotnet build 03-Plugins/Winstaller.Plugins/Winstaller.Plugins.csproj

# 05-FullApplication projects:
dotnet build 05-FullApplication/Solution/WpfApp/WpfApp.csproj
dotnet build 05-FullApplication/Solution/DemoService/DemoService.csproj
```

---

### 2. Trusting the Test Certificate (Example 05)
Before running signed installers generated from Example 05 on a test machine, run the certificate import script as Administrator:

```powershell
powershell -ExecutionPolicy Bypass -File 05-FullApplication/install-test-cert.ps1
```

---

### 3. Building Installers via Winstaller CLI
Once the C# projects are compiled, generate installer executables using the Winstaller CLI:

```cmd
# Example 1: Minimal Setup
winstaller build 01-Minimal/minimal.json

# Example 2: Custom Steps
winstaller build 02-CustomSteps/custom_steps.json

# Example 3: Script Plugins
winstaller build 03-Plugins/plugin.json

# Example 4: Localization & Translations
winstaller build 04-Translations/translations.json

# Example 5: Production App with Service & Code Signing
winstaller build 05-FullApplication/fullapp.json
```
