# Script registering Winstaller test certificate into Windows Trusted Root Certification Authorities store.
# Requires running in PowerShell with Administrator privileges.

$pfxPath = "$PSScriptRoot\winstaller-test.pfx"
$password = ConvertTo-SecureString 'pA$$w0rd' -AsPlainText -Force

if (-not (Test-Path $pfxPath)) {
    Write-Error "Certificate file $pfxPath does not exist."
    exit 1
}

Write-Host "Importing Winstaller test certificate into Trusted Root Certification Authorities..." -ForegroundColor Cyan

try {
    Import-PfxCertificate -FilePath $pfxPath -CertStoreLocation Cert:\LocalMachine\Root -Password $password | Out-Null
    Write-Host "Success! The certificate was imported successfully and is now trusted on this computer." -ForegroundColor Green
}
catch {
    Write-Error "Failed to import certificate. Make sure you ran PowerShell as Administrator. Error: $_"
}
