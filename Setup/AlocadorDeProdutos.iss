; ============================================================================
;  Alocador de Produtos — Inno Setup script
; ----------------------------------------------------------------------------
;  Empacota a pasta de publish self-contained do .NET 10 num instalador .exe
;  unico para distribuicao em Windows.
;
;  Pre-requisitos:
;   - Pasta `..\bin\Release\net10.0-windows\publish\win-x64\` com saida do
;     `dotnet publish -p:PublishProfile=win-x64`. Use a task do VSCode
;     "publish-for-inno-setup" ou rode manualmente antes de compilar este .iss.
;
;  Compilacao:
;     "C:\Users\<user>\AppData\Local\Programs\Inno Setup 6\ISCC.exe" \
;        "Setup\AlocadorDeProdutos.iss"
;
;  Saida: Setup\Output\AlocadorDeProdutos-Setup-11.0.0.0.exe
;
;  Politica de privilegios (intencional):
;   - Instalacao em Program Files (x64), exige UAC.
;   - Aplicacao roda sem UAC mas a engrenagem (FormConfig) precisa elevar
;     para gravar senha em `Alocador de Produtos.exe.config`. Isso isola
;     mudancas de configuracao a administradores da maquina, conforme
;     decidido por design.
; ============================================================================

#define MyAppName        "Alocador de Produtos"
#define MyAppVersion     "11.0.0.0"
#define MyAppPublisher   "Bernardo Graunke"
#define MyAppExeName     "Alocador de Produtos.exe"
#define MyAppDescription "Alocacao de produtos no estoque do ERP Integra"
; AppId estavel — NAO MUDAR entre versoes (controla upgrade in-place).
; Sintaxe Inno Setup: para AppId aceitar "{GUID}" como string literal,
; o #define usa "{{" duplo no inicio (escape para "{" literal) e "}" simples no fim.
#define MyAppId          "{{F7CBB36B-C8BB-455C-ADB0-21AB353C384D}"
; Pasta de publish (relativa ao .iss).
#define PublishDir       "..\bin\Release\net10.0-windows\publish\win-x64"

[Setup]
AppId={#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppCopyright=Copyright (c) 2024-2026 Bernardo Graunke - MIT License
AppComments={#MyAppDescription}
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription={#MyAppName} Setup
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}

; Instalacao em Program Files (x64) — exige UAC.
DefaultDirName={autopf}\Alocador de Produtos
DefaultGroupName=Alocador de Produtos
DisableProgramGroupPage=yes
DisableDirPage=auto
AllowNoIcons=yes

; Permissoes: sempre admin. Operador comum NAO consegue instalar nem mexer
; em config — isso e intencional para isolar credencial do banco.
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=

; Arquitetura: 64-bit somente (x64 + ARM64 via x64compatible).
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

; Saida do .exe gerado (ignorado pelo .gitignore).
OutputDir=Output
OutputBaseFilename=AlocadorDeProdutos-Setup-{#MyAppVersion}

; Branding e licenca.
LicenseFile=..\LICENSE
SetupIconFile=..\Resources\Icon.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName} {#MyAppVersion}

; Compactacao maxima (lzma2 ultra64 reduz ~119MB do publish para ~50-55MB).
Compression=lzma2/ultra64
LZMAUseSeparateProcess=yes
SolidCompression=yes
WizardStyle=modern

; Comportamento de upgrade.
; CloseApplications=yes — fecha o app aberto antes de substituir os arquivos.
CloseApplications=yes
RestartApplications=no

; Versao minima Windows: 10 (1809 / build 17763), alinhado com .NET 10.
MinVersion=10.0.17763

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Empacota a pasta inteira de publish — runtime .NET 10 self-contained + binarios.
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Comment: "{#MyAppDescription}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Comment: "{#MyAppDescription}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; Arquivos de state criados pelo operador em runtime (NAO foram instalados,
; entao o uninstaller padrao nao os remove). Listamos aqui para limpeza completa.
Type: files;       Name: "{app}\db_config.txt"
Type: files;       Name: "{app}\filial.txt"
Type: files;       Name: "{app}\fila_pendente.txt"
Type: files;       Name: "{app}\fila_pendente.txt.tmp"
Type: dirifempty;  Name: "{app}"
