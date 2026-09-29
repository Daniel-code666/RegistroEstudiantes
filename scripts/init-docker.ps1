$ErrorActionPreference = 'Stop'
$projectPath = Split-Path -Parent $PSScriptRoot
$envPath = Join-Path $projectPath '.env'

if (Test-Path -LiteralPath $envPath) {
    Write-Host 'Se conserva el archivo .env existente.'
    exit 0
}

function New-RandomHex([int] $length) {
    $bytes = New-Object byte[] $length
    $random = [System.Security.Cryptography.RandomNumberGenerator]::Create()
    try { $random.GetBytes($bytes) }
    finally { $random.Dispose() }
    return [BitConverter]::ToString($bytes).Replace('-', '').ToLowerInvariant()
}

$sqlPassword = 'Re1!' + (New-RandomHex 24)
$jwtKey = New-RandomHex 48
$adminPassword = 'Admin1!' + (New-RandomHex 20)
$content = @(
    "SQL_SERVER_PASSWORD=$sqlPassword"
    "JWT_SIGNING_KEY=$jwtKey"
    ('ConnectionStrings__DefaultConnection=''Server=localhost,14334;Database=RegistroEstudiantes;User Id=sa;Password="{0}";Encrypt=True;TrustServerCertificate=True''' -f $sqlPassword)
    ('SQL_CONNECTION_STRING_DOCKER=''Server=db,1433;Database=RegistroEstudiantes;User Id=sa;Password="{0}";Encrypt=True;TrustServerCertificate=True''' -f $sqlPassword)
    'API_PORT=5088'
    'SQL_SERVER_PORT=14334'
    'BOOTSTRAP_ADMIN_ENABLED=true'
    'BOOTSTRAP_ADMIN_EMAIL=admin@registroestudiantes.local'
    "BOOTSTRAP_ADMIN_PASSWORD=$adminPassword"
    'BOOTSTRAP_ADMIN_NAME=Administrador'
    'BOOTSTRAP_ADMIN_LAST_NAME=Inicial'
    'BOOTSTRAP_ADMIN_IDENTIFICATION_TYPE=Pasaporte'
    'BOOTSTRAP_ADMIN_IDENTIFICATION_NUMBER=BOOTSTRAP-ADMIN'
) -join "`n"
[System.IO.File]::WriteAllText($envPath, $content + "`n", (New-Object System.Text.UTF8Encoding $false))
Write-Host 'Archivo .env creado con credenciales aleatorias. No lo incluya en control de versiones.'
