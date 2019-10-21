#define MyAppName "Fork.Gui"
#define MyAppVersion "1.0.1"
#define MyAppPublisher "НВП Модем"
#define MyAppURL "https://www.modem.by/"


#ifndef srcPath
  #define srcPath
#endif

#ifndef outPath
  #define outPath
#endif

[Setup]
AppId={{A2478EBB-D480-49AC-ACA7-70422560F7CB}
AppName={#MyAppName}
AppVersion={#MyAppVersion}

AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName="Modem"
OutputDir={#outPath}
OutputBaseFilename=ForkGui
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
AlwaysRestart=yes
UninstallDisplayName=ForkGUI
DisableDirPage=auto

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

;[Tasks]
;Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
;Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; 

[Files]
Source:{#srcPath}\*.*; DestDir: "{app}"; Flags: ignoreversion recursesubdirs;    

[Icons]
Name: "{group}\Fork"; Filename: "{app}\Fork.Gui.exe"
Name: "{group}\Удаление Fork"; Filename: "{uninstallexe}"
Name: "{group}\Конфигурация Fork"; Filename: "{app}\Fork.Gui.exe.config"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\Fork.Gui.exe"; 
;Name: "{commonstartup}\{#MyAppName}"; Filename: "{app}\Fork.Gui.exe"      

[Run]
Filename: "{app}\Fork.Gui.exe"; Flags: shellexec skipifsilent nowait; 

[Registry]
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "Fork.Gui.exe"; ValueData: "{app}\Fork.Gui.exe";                 

[Code]
const

  PreviousAppID = 'A2478EBB-D480-49AC-ACA7-70422560F7CB';
  AppFolder     = 'Modem Fork';

  UninstallPath = 'Software\Microsoft\Windows\CurrentVersion\Uninstall\{'
                 + PreviousAppID + '}_is1';
  InstallKey    = 'InstallLocation';

function GetSrcPath(default: String): String;
var
ResultPath: String;

begin
ResultPath:='{#srcPath}';

if ResultPath='' then
    begin
    Result := 'false'; 
    RaiseException('Nothing found by source path, or source path has not been specified');
  end
    else
  begin

    Result:= ResultPath + '*.*';
  end
end;

procedure TaskKill(FileName: String);
var
  ResultCode: Integer;
begin
    Exec(ExpandConstant('taskkill.exe'), '/f /im ' + '"' + FileName + '"', '', SW_HIDE,
     ewWaitUntilTerminated, ResultCode);
end;

(*procedure InitializeWizard();
var 
  UserConfigPage: TInputOptionWizardPage;
  SavePreviousConfig: Boolean; 
begin
  UserConfigPage := CreateInputOptionPage(wpWelcome,
  'Информация о настройках', '',
  'Использовать настройки из предыдущей версии программы?', false, false);

  UserConfigPage.Add('Использовать');
  UserConfigPage.Values[0] := True;

  SavePreviousConfig := UserConfigPage.Values[0];
end;
  *)
/////////////////////////////////////////////////////////////////////
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;


/////////////////////////////////////////////////////////////////////
function IsUpgrade(): Boolean;
begin
  Result := (GetUninstallString() <> '');
end;


/////////////////////////////////////////////////////////////////////
function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
// Return Values:
// 1 - uninstall string is empty
// 2 - error executing the UnInstallString
// 3 - successfully executed the UnInstallString

  // default return value
  Result := 0;

  // get the uninstall string of the old app
  sUnInstallString := GetUninstallString();
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    if Exec(sUnInstallString, '/SILENT /NORESTART /SUPPRESSMSGBOXES','', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
      Result := 3
    else
      Result := 2;
  end else
    Result := 1;
end;

/////////////////////////////////////////////////////////////////////
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep=ssInstall) then
  begin
    if (IsUpgrade()) then
    begin
      UnInstallOldVersion();
    end;
  end;
end;

procedure CurUninstallStepChanged(CurStep: TUninstallStep);  
begin;
  case CurStep of
    usUninstall:
    begin
     TaskKill('Fork.Gui.exe');
    end;
  end;
end;
