#define MyAppName "MAGE 2 Server"
#define MyAppVersion GetFileVersion("..\MAGE2_Server\Build\Windows_x86\MAGE 2 Server_Data\Managed\Assembly-CSharp.dll")
#define MyAppPublisher "OSU ECE Department"
#define MyAppURL "https://github.com/Jacob-Dixon/MAGE2/wiki/Server"
#define MyAppExeName "MAGE 2 Server.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{EE309DF4-221E-4941-9A4A-64D40566DD68}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DisableDirPage=yes
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
LicenseFile={#SourcePath}..\LICENSE
OutputDir={#SourcePath}..\Release
OutputBaseFilename=MAGE 2 Setup-{#MyAppVersion}
SetupIconFile={#SourcePath}Installer.ico
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
;x64
Source: "{#SourcePath}..\MAGE2_Server\Build\Windows_x86_64\MAGE 2 Server.exe"; DestDir: "{app}"; Check: Is64BitInstallMode; Flags: ignoreversion
Source: "{#SourcePath}..\MAGE2_Server\Build\Windows_x86_64\MAGE 2 Server_Data\Mono\*"; DestDir: "{app}\MAGE 2 Server_Data\Mono"; Check: Is64BitInstallMode; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#SourcePath}..\MAGE2_Server\Build\Windows_x86_64\MAGE 2 Server_Data\Plugins\*"; DestDir: "{app}\MAGE 2 Server_Data\Plugins"; Check: Is64BitInstallMode; Flags: ignoreversion recursesubdirs createallsubdirs
;x86
Source: "{#SourcePath}..\MAGE2_Server\Build\Windows_x86\MAGE 2 Server.exe"; DestDir: "{app}"; Check: not Is64BitInstallMode; Flags: ignoreversion
Source: "{#SourcePath}..\MAGE2_Server\Build\Windows_x86\MAGE 2 Server_Data\Mono\*"; DestDir: "{app}\MAGE 2 Server_Data\Mono"; Check: not Is64BitInstallMode; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#SourcePath}..\MAGE2_Server\Build\Windows_x86\MAGE 2 Server_Data\Plugins\*"; DestDir: "{app}\MAGE 2 Server_Data\Plugins"; Check: not Is64BitInstallMode; Flags: ignoreversion recursesubdirs createallsubdirs
;Independent
Source: "{#SourcePath}..\MAGE2_Server\Build\Windows_x86\MAGE 2 Server_Data\*"; DestDir: "{app}\MAGE 2 Server_Data"; Flags: ignoreversion
Source: "{#SourcePath}..\MAGE2_Server\Build\Windows_x86\MAGE 2 Server_Data\Managed\*"; DestDir: "{app}\MAGE 2 Server_Data\Managed"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#SourcePath}..\MAGE2_Server\Build\Windows_x86\MAGE 2 Server_Data\Resources\*"; DestDir: "{app}\MAGE 2 Server_Data\Resources"; Flags: ignoreversion recursesubdirs createallsubdirs

[Dirs]
Name: "{app}\Logs"
Name: "{app}\PIU Firmware"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

