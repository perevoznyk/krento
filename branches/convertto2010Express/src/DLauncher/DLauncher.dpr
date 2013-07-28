program DLauncher;

uses
  GDIPOBJ,
  Windows,
  SysUtils,
  Forms,
  SupportData in 'SupportData.pas',
  SpecialFolders in 'SpecialFolders.pas',
  Docklets in 'Docklets.pas',
  frm_main in 'frm_main.pas' {frmMain},
  DockletWindow in 'DockletWindow.pas',
  frm_settings in 'frm_settings.pas' {frmSettings},
  ParentCheckerThread in 'ParentCheckerThread.pas',
  NativeThemeManager in 'NativeThemeManager.pas';

{$R *.res}

begin
 // ReportMemoryLeaksOnShutdown := true;
  if (ParamCount > 0) then
     begin
       Application.Initialize;
       Application.ShowMainForm := false;
       Application.MainFormOnTaskbar := False;
       Application.Title := 'Krento Docklet Launcher';
       Application.CreateForm(TfrmMain, frmMain);
  ShowWindow(frmMain.Handle, SW_HIDE);
       Application.Run;
     end;
end.
