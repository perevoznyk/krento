unit frm_main;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs,  PsAPI, ShellAPI, SupportData,  SyncObjs, Multimon,
  DockletWindow, GDIPAPI, GDIPOBJ,  ActiveX, IniFiles, ExtCtrls, Menus, AppEvnts,
  tlHelp32, ParentCheckerThread;

const
  ShadowOffsetXSizePercent = -3;
  ShadowOffsetYSizePercent = -3;
  ShadowOffsetXConstant    = 0;
  ShadowOffsetYConstant    = 0;
  ShadowWidthSizePercent   = 3;
  ShadowWidthConstant      = 0;

const
  GDIP_NOWRAP = 4096;

type
  TfrmMain = class(TForm)
    CloseTimer: TTimer;
    DockletMenu: TPopupMenu;
    miConfigure: TMenuItem;
    miClose: TMenuItem;
    dlgOpen: TOpenDialog;
    ApplicationEvents1: TApplicationEvents;
    TimerFush: TTimer;
    miDelete: TMenuItem;
    AnimationTimer: TTimer;
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure CloseTimerTimer(Sender: TObject);
    procedure miCloseClick(Sender: TObject);
    procedure miConfigureClick(Sender: TObject);
    procedure ApplicationEvents1Exception(Sender: TObject; E: Exception);
    procedure TimerFushTimer(Sender: TObject);
    procedure miDeleteClick(Sender: TObject);
    procedure AnimationTimerTimer(Sender: TObject);
  private
    Locker : TCriticalSection;
    CaptionLocker : TCriticalSection;
    ParentChecker : TParentChecker;
    IniFile : TIniFile;
    RunAsChild : boolean;

    procedure MouseHandler(Sender: TObject; Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
    function  FontStyleToFlag(Style : TFontStyles) : integer;

    procedure WmSetOverlay(var Message : TMessage); message WM_SET_OVERLAY;
    procedure WmSetImage(var Message : TMessage); message WM_SET_IMAGE;
    procedure WmSetFile(var Message : TMessage); message WM_SET_FILE;
    procedure WmPaintCaption(var Message : TMessage); message WM_PAINT_CAPTION;
    procedure WmTerminate(var Message : TMessage); message WM_TERMINATE;
    procedure WmSetOverlayNative(var Message : TMessage); message WM_SET_OVERLAY_NATIVE;
    procedure WmSetImageNative(var Message : TMessage); message WM_SET_IMAGE_NATIVE;
 public
    ConfigFileName  : string;
    CaptionWidth : integer;
    DockletSize : integer;
    W : TDockletWindow;
    HintWindow : TDockletHintWindow;
    DockletData : Pointer;
    DockletHandle : THandle;
    DockletLabel : string;

    DockletEnabled  : boolean;

    DockletImage : TGPBitmap;
    DockletOverlay : TGPBitmap;

    DockletImagePointer : pointer;
    DockletOverlayPointer : pointer;

    CanUseDrop : boolean;
    RelativePath : string;
    BackgroundImage : string;
    LanguageName : string;
    AnimationStep : integer;
    procedure ApplyGUILanguage;
    procedure CreateBase;

    function LoadDocklet(FileName : string) : boolean;
    procedure StringDraw(const AText: WideString; AFont: TGPFont; AX, AY, AW,
    AH: Integer; ACanvas: TGPGraphics; GFamily : TGPFontFamily; format : TGPStringFormat);
    procedure PaintCaption;

    procedure HintMouseEnter(Sender : TObject);
    procedure HintMouseLeave(Sender : TObject);
    procedure WindowMoved (ANewPosition: TPoint);

    procedure MoveHintWindow;
    procedure ClearImageCache;
    procedure ClearOverlayCache;
    procedure RepaintBase;

    function GetWorkArea : TRect;
  end;

var
  frmMain: TfrmMain;

  MessageID : integer;

implementation

uses Docklets, SpecialFolders, frm_settings;

{$R *.dfm}

type TMyGPBitmap = class(TGPBitmap);

var
  WProc: TFNWndProc = Nil;


function CurrentLanguage : string;
var
  Res: Integer;
  W: PWideChar;
  Sub : string;
begin
    Res := GetLocaleInfoW(GetUserDefaultLCID,  LOCALE_SISO639LANGNAME , nil, 0);
    if Res > 0 then
    begin
      GetMem(W, Res * SizeOf(WideChar));
      Res := Windows.GetLocaleInfoW(GetUserDefaultLCID,  LOCALE_SISO639LANGNAME , W, Res);
      Result := WideCharToString(W);
      FreeMem(W);
    end;

  if Res = 0 then
    Result := '';

    Res := GetLocaleInfoW(GetUserDefaultLCID,  LOCALE_SISO3166CTRYNAME , nil, 0);
    if Res > 0 then
    begin
      GetMem(W, Res * SizeOf(WideChar));
      Res := Windows.GetLocaleInfoW(GetUserDefaultLCID,  LOCALE_SISO3166CTRYNAME , W, Res);
      Sub := WideCharToString(W);
      FreeMem(W);
    end;

  if Res = 0 then
    Sub := '';

  if Sub <> '' then
    Result := Result + '-' + Sub;

  if Result = '' then
    Result := 'en-US';
end;

{ TfrmMain }


procedure TfrmMain.AnimationTimerTimer(Sender: TObject);
begin
  if AnimationStep > 2 then
   begin
     AnimationStep := 0;
     AnimationTimer.Enabled := false;
   end
     else
     begin
       AnimationTimer.Enabled := false;
       W.Move(W.Position.X, W.Position.Y + (AnimationStep + 1));
       W.Update(true);
       Sleep(100);
       W.Move(W.Position.X, W.Position.Y - (AnimationStep + 1));
       W.Update(true);
       Inc(AnimationStep);
       AnimationTimer.Enabled := true;
     end;
end;

procedure TfrmMain.ApplicationEvents1Exception(Sender: TObject; E: Exception);
begin
  Application.Terminate;
end;

procedure TfrmMain.ApplyGUILanguage;
var
  LangFile : TIniFile;
  LangFileName : string;
  Ini : TIniFile;
begin

  LanguageName := CurrentLanguage;

  ini := TIniFile.Create(KrentoSettingsFileName);
  LanguageName := Ini.ReadString('General', 'Language', LanguageName);
  ini.Free;

  langFileName := LanguageName + '.lng';
  LangFileName := IncludeTrailingPathDelimiter(LanguagesFolder) + LangFileName;
  LangFile := TIniFile.Create(LangFileName);
  miConfigure.Caption := LangFile.ReadString('strings', 'Settings', 'Settings');
  miClose.Caption := LangFile.ReadString('strings', 'Close', 'Close');
  miDelete.Caption := LangFile.ReadString('strings', 'Delete', 'Delete');
  LangFile.Free;
end;

procedure TfrmMain.ClearImageCache;
begin
   if DockletImagePointer <> nil then
       try
         DeleteImage(DockletImagePointer);
       finally
         DockletImagePointer := nil;
       end;

   if DockletImage <> nil then
       try
          DockletImage.Free;
       finally
          DockletImage := nil;
       end;
end;

procedure TfrmMain.ClearOverlayCache;
begin
   if DockletOverlayPointer <> nil then
     begin
       try
         DeleteImage(DockletOverlayPointer);
       finally
         DockletOverlayPointer := nil;
       end;
     end;

   if DockletOverlay <> nil then
       try
          DockletOverlay.Free;
       finally
          DockletOverlay := nil;
       end;
end;

procedure TfrmMain.miCloseClick(Sender: TObject);
begin
 Close;
end;

procedure TfrmMain.CloseTimerTimer(Sender: TObject);
begin
  CloseTimer.Enabled := false;
  Close;
end;

procedure TfrmMain.miConfigureClick(Sender: TObject);
begin
  if Assigned(VOnConfigure) then
    DoOnConfigure(DockletData)
      else
        begin
          frmSettings := TfrmSettings.Create(Application);
          frmSettings.edtLabel.Text := DockletLabel;
          frmSettings.edtIcon.Text := BackgroundImage;
          if frmSettings.ShowModal = mrOK then
           begin
             //
             BackgroundImage := frmSettings.edtIcon.Text;
             DockletLabel := frmSettings.edtLabel.Text;
             DockletSetImageFile(W.WindowHandle, PChar(BackgroundImage)); 
           end;
           frmSettings.Free;
        end;
end;

procedure TfrmMain.miDeleteClick(Sender: TObject);
begin
 DockletEnabled := false;
 Close;
end;

procedure TfrmMain.CreateBase;
var
  ini : TIniFile;
begin
  ini := TIniFile.Create(KrentoSettingsFileName);
  DockletSize := ini.ReadInteger('General', 'DockletSize', 96);
  ini.Free;

  if DockletSize < 16 then
    DockletSize := 16;
  if DockletSize > 256 then
    DockletSize := 256;

  W := TDockletWindow.Create;
  W.Hide;
  W.OnMoved := WindowMoved;
  W.OnMouseUp := MouseHandler;
  W.Size(DockletSize, DockletSize);
  W.Alpha := 255;
  W.UseAlpha := true;
  W.ColorKey := MakeColor(0, 255, 255, 255);
  W.UseColorKey := false;
  W.Canvas.SetSmoothingMode(SmoothingModeAntiAlias);

  HintWindow := TDockletHintWindow.Create;

  W.OnMouseEnter := HintMouseEnter;
  W.OnMouseLeave := HintMouseLeave;

  HintWindow.Hide;
  HintWindow.Alpha := 255;
  HintWindow.UseAlpha := true;
  HintWindow.ColorKey := MakeColor(0, 255, 255, 255);
  HintWindow.UseColorKey := false;
end;

function NewWndProc(Handle: HWND; Msg: Integer; wParam, lParam: Longint):
  Longint; stdcall;
begin
  { If this is the registered message... }
  if Msg = MessageID then
  begin
    if Application <> nil then
     try
       Application.Terminate;
     except
     end;  
    Result := 0;
  end
  { Otherwise, pass message on to old window proc }
  else
    Result := CallWindowProc(WProc, Handle, Msg, wParam, lParam);
end;


procedure TfrmMain.RepaintBase;
begin
  Locker.Enter;
  try

  W.Clear();

  try
      W.Canvas.SetInterpolationMode(InterpolationModeBicubic);
      W.Canvas.SetCompositingQuality(CompositingQualityHighQuality);
      W.Canvas.SetSmoothingMode(SmoothingModeHighQuality);

      if (DockletImage <> nil) then
        W.Canvas.DrawImage(DockletImage, 0, 0, W.GetSize.cx, W.GetSize.cy);

      if (DockletOverlay <> nil) then
        W.Canvas.DrawImage(DockletOverlay, 0, 0, W.GetSize.cx, W.GetSize.cy);

  except

  end;
  W.Update(false);

  finally
    Locker.Leave;
  end;

end;

function TfrmMain.FontStyleToFlag(Style: TFontStyles): integer;
begin
  Result := 0;
  if (fsBold in Style) then
    Result := Result + 1;
  if (fsItalic in Style) then
    Result := Result + 2;
  if (fsUnderline in Style) then
    Result := Result + 4;
end;

function GetParentProcessId : cardinal;
var
  HandleSnapShot  : THandle;
  EntryParentProc : TProcessEntry32;
  CurrentProcessId: THandle;
  HandleParentProc: THandle;
  ParentProcessId : THandle;
  ParentProcessFound  : Boolean;
begin
  ParentProcessId := 0;
  ParentProcessFound := False;
  HandleSnapShot := CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);   //enumerate the process
  if HandleSnapShot <> INVALID_HANDLE_VALUE then
  begin
    EntryParentProc.dwSize := SizeOf(EntryParentProc);
    if Process32First(HandleSnapShot, EntryParentProc) then    //find the first process
    begin
      CurrentProcessId := GetCurrentProcessId(); //get the id of the current process
      repeat
        if EntryParentProc.th32ProcessID = CurrentProcessId then
        begin
          ParentProcessId := EntryParentProc.th32ParentProcessID; //get the id of the parent process
          HandleParentProc := OpenProcess(PROCESS_QUERY_INFORMATION or PROCESS_VM_READ, False, ParentProcessId);
          if HandleParentProc <> 0 then
          begin
              ParentProcessFound := True;
              CloseHandle(HandleParentProc);
          end;
          break;
        end;
      until not Process32Next(HandleSnapShot, EntryParentProc);
    end;
    CloseHandle(HandleSnapShot);
  end;

  if ParentProcessFound then
    Result := ParentProcessId
  else
    Result := 0;
end;

procedure TfrmMain.FormCreate(Sender: TObject);
var
  WPosition       : TPoint;
  fileName        : string;
  LoadDockletName : string;

  MaxLeft : integer;
  MaxTop : integer;
  WorkArea : TRect;
  ParentProcessID : Cardinal;
  I : integer;
begin
  RunAsChild := false;
  Randomize;
  Locker := TCriticalSection.Create;
  CaptionLocker := TCriticalSection.Create;

   for I := 1 to ParamCount  do
    begin
      if SameText(ParamStr(I), '-p') then
       PortableVersion := true
         else
           if SameText(ParamStr(I), '-child') then
              RunAsChild := true
                 else
                   ConfigFileName  := ParamStr(I);
    end;

  if RunAsChild then
    begin
      MessageID := RegisterWindowMessage('KrentoStop');
      WProc := TFNWndProc(SetWindowLong(Application.Handle, GWL_WNDPROC,
                                        Longint(@NewWndProc)));
       ParentProcessID := GetParentProcessId();
       if ParentProcessID > 0 then
         begin
           ParentChecker := TParentChecker.Create(true);
           ParentChecker.FreeOnTerminate := true;
           ParentChecker.ParentId := ParentProcessID;
           ParentChecker.Start;
         end
          else
            ParentChecker := nil;
    end;

   ApplyGUILanguage;
   CreateBase;

   WorkArea := GetWorkArea;
   MaxLeft := WorkArea.Right - DockletSize;
   MaxTop  := round ((WorkArea.Bottom - DockletSize) / 2);



    IniFile := TIniFile.Create(ConfigFileName);
    LoadDockletName := IniFile.ReadString('Docklet', 'Assembly', '');
    if LoadDockletName = '' then
    begin
      CloseTimer.Enabled := true;
      Exit;
    end;

    LoadDockletName := IncludeTrailingPathDelimiter(ExtractFilePath(ConfigFileName)) + LoadDockletName;

   //check for right name of the relative folder
    RelativePath := ExtractFilePath(ConfigFileName);
    RelativePath := GetRelativeFolder(RelativePath);
    RelativePath := IncludeTrailingPathDelimiter(RelativePath);
   //

   DockletEnabled := IniFile.ReadBool('Docklet', 'Enabled', true);

   if not DockletEnabled then
    begin
      CloseTimer.Enabled := true;
      Exit;
    end;

   if not LoadDocklet(LoadDockletName) then
    begin
      CloseTimer.Enabled := true;
      Exit;
    end;

    if not Assigned(VOnConfigure) then
      begin
       BackgroundImage := IniFile.ReadString('Settings', 'Image', 'default.png');
       if (SameText('default.png', BackgroundImage)) then
       BackgroundImage := 'Icons\Stardock ObjectDock.png';

       if ExtractFileDrive(BackgroundImage) = '' then
         fileName := IncludeTrailingPathDelimiter(MainFolder) +  BackgroundImage
           else
             fileName := BackgroundImage;

       if (not FileExists(FileName)) then
         begin
          BackgroundImage := 'Icons\Stardock ObjectDock.png';
          fileName := IncludeTrailingPathDelimiter(MainFolder) +  BackgroundImage;
         end;

        if BackgroundImage <> '' then
         begin
            DockletImage := TGPBitmap.Create(fileName);
            RepaintBase;
         end;
      end;

   if IniFile.SectionExists('Settings') then
    DockletData := DoOnCreate(W.WindowHandle, DockletHandle,
        PChar(ConfigFileName), PChar('Settings'))
          else
            DockletData := DoOnCreate(W.WindowHandle, DockletHandle, nil, nil);

    if DockletData = nil then
      begin
        CloseTimer.Enabled := true;
        Exit;
      end;

    WPosition.X := IniFile.ReadInteger('Position', 'Left', Random(MaxLeft) );
    WPosition.Y := IniFile.ReadInteger('Position', 'Top', Random(MaxTop));
    W.Move(WPosition);

   if (DockletHandle > 0) then
    begin
      CanUseDrop := DoOnAcceptDropFiles(DockletData);
      if CanUseDrop then
          DragAcceptFiles( W.WindowHandle, True );

      W.Update(false);
      W.Show;
      SetWindowPos(W.WindowHandle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE or SWP_NOMOVE);
      EmptyWorkingSet(GetCurrentProcess);
    end
      else
        begin
          CloseTimer.Enabled := true;
        end;
end;

procedure TfrmMain.FormDestroy(Sender: TObject);
begin
 AnimationTimer.Enabled := false;
 if ParentChecker <> nil then
   begin
     TerminateThread(ParentChecker.Handle, 0);
   end;
 try

  try
    if (HintWindow <> nil) then
     begin
       HintWindow.Hide;
       HintWindow.Free;
       HintWindow := nil;
     end;
  except
  end;

  if W <> nil then
    W.Hide;

  if RunAsChild then
    begin
      if WProc <> Nil then
      { Restore old window procedure }
       SetWindowLong(Application.Handle, GWL_WNDPROC, LongInt(WProc));
    end;

  try
    if DockletHandle > 0 then
     try
       IniFile.WriteInteger('Position', 'Left', W.Position.X);
       IniFile.WriteInteger('Position', 'Top', W.Position.Y);
       IniFile.WriteBool('Docklet', 'Enabled', DockletEnabled);
       if BackgroundImage <> '' then
        begin
           if not Assigned(VOnConfigure) then
            begin
              IniFile.WriteString('Settings', 'Image', BackgroundImage);
              IniFile.WriteString('Settings', 'Title', DockletLabel);
            end;
        end;
       IniFile.Free;
       DoOnSave(DockletData, PChar(ConfigFileName), PChar('Settings'), false);

       DoOnDestroy(DockletData, W.WindowHandle);
     except  
     end;
    if DockletImage <> nil then
      DockletImage.Free;
    if DockletOverlay <> nil then
      DockletOverlay.Free;
  finally
    if W <> nil then
     begin
       W.Free;
       W := nil;
     end;
  end;

 Locker.Free;
 CaptionLocker.Free;
  
 except

 end;
end;

function TfrmMain.GetWorkArea: TRect;
var
 Monitor : HMonitor;
 info : TMonitorInfoEx;
 tray : THandle;
 found : boolean;
 pt : TPoint;
begin
  found := false;
  FillChar(info, SizeOf(info), 0);
  info.cbSize := SizeOf(info);

  tray := FindWindow('Shell_TrayWnd', nil);
  if (tray > 0) then
    begin
      monitor := MonitorFromWindow(tray, MONITOR_DEFAULTTOPRIMARY);
      if (monitor > 0) then
         begin
           if (GetMonitorInfo(monitor, @info)) then
             begin
               found := true;
               Result := info.rcWork;
             end;
         end;
    end
     else
       begin
         pt := Point(0,0);
         monitor := MonitorFromPoint(pt, MONITOR_DEFAULTTOPRIMARY);
          if (monitor > 0) then
             begin
               if (GetMonitorInfo(monitor, @info)) then
                 begin
                   found := true;
                   Result := info.rcWork;
                 end;
             end;
       end;

    if (not found) then
      Result := Rect(0, 0, 800, 600);
end;

procedure TfrmMain.HintMouseEnter(Sender: TObject);
begin
   if (HintWindow <> nil) then
      begin
        HintWindow.Hide;
        PaintCaption;
        HintWindow.Show;
      end;
end;

procedure TfrmMain.HintMouseLeave(Sender: TObject);
begin
  if (HintWindow <> nil) then
    begin
      HintWindow.Hide;
    end;
end;

function TfrmMain.LoadDocklet(FileName: string) : boolean;
begin
  Result := false;
   if FileExists(FileName) then
     begin
       DockletHandle := SafeLoadLibrary(PChar(FileName), SEM_FAILCRITICALERRORS or SEM_NOOPENFILEERRORBOX);
       if (DockletHandle > 0) then
         begin
            VOnGetInformation   := GetProcAddress(DockletHandle, 'OnGetInformation');
            VOnCreate           := GetProcAddress(DockletHandle, 'OnCreate');
            VOnSave             := GetProcAddress(DockletHandle, 'OnSave');
            VOnDestroy          := GetProcAddress(DockletHandle, 'OnDestroy');
            VOnExportFiles      := GetProcAddress(DockletHandle, 'OnExportFiles');
            VOnLeftButtonClick  := GetProcAddress(DockletHandle, 'OnLeftButtonClick');
            VOnDoubleClick      := GetProcAddress(DockletHandle, 'OnDoubleClick');
            VOnLeftButtonHeld   := GetProcAddress(DockletHandle, 'OnLeftButtonHeld');
            VOnRightButtonClick := GetProcAddress(DockletHandle, 'OnRightButtonClick');
            VOnConfigure        := GetProcAddress(DockletHandle, 'OnConfigure');
            VOnAcceptDropFiles  := GetProcAddress(DockletHandle, 'OnAcceptDropFiles');
            VOnDropFiles        := GetProcAddress(DockletHandle, 'OnDropFiles');
            VOnProcessMessage   := GetProcAddress(DockletHandle, 'OnProcessMessage');

            Result := Assigned(VOnCreate);
         end;
     end;
end;

procedure TfrmMain.MouseHandler(Sender: TObject; Button: TMouseButton;
  Shift: TShiftState; X, Y: Integer);
var
  P : TPoint;
  S : TSize;
begin
   P.X := X;
   P.Y := Y;

   S.cx := W.GetSize.cx;
   S.cy := W.GetSize.cy;
   if (Button = mbLeft) then
    DoOnLeftButtonClick(DockletData, @P, @S)
     else
     if (Button = mbRight) then
       begin
         if not DoOnRightButtonClick(DockletData, @P, @S) then
           begin
             P.X := X;
             P.Y := Y;
             Windows.ClientToScreen(W.WindowHandle, P);
             DockletMenu.Popup(P.X, P.Y);
           end;
       end;
end;

procedure TfrmMain.MoveHintWindow;
var
  posX : integer;
  posY : integer;
begin
  if (HintWindow = nil) then
    Exit;

  if IsWindowVisible(HintWindow.WindowHandle) then
    begin
     posX := round(W.GetSize.cx / 2) + W.Position.X - round(captionWidth / 2);
     posY := round(W.Position.Y + W.GetSize.cy + 4);
     HintWindow.Move(Point(posX, PosY));
     HintWindow.Update(false);
    end;
end;

procedure TfrmMain.PaintCaption;
var
  format : TGPStringFormat;
  captionSizeF : TGPRectF;
  GFont : TGPFont;
  GFamily : TGPFontFamily;
  FS : integer;
  captionHeight : integer;
  posX : integer;
  posY : integer;
begin
  if (HintWindow = nil) then
    Exit;

  CaptionLocker.Enter;
  try
  if (DockletLabel = '') then
     begin
       HintWindow.Clear;
       HintWindow.Update(false);
       HintWindow.Hide;
       Exit;
     end;

  HintWindow.Canvas.SetTextRenderingHint(TextRenderingHintClearTypeGridFit);   
  GFamily := TGPFontFamily.Create(Self.Font.Name);
  fs := FontStyleToFlag(Self.Font.Style);
  GFont := TGPFont.Create(GFamily, Self.Font.Size * 2, fs, UnitPoint);

  format := TGPStringFormat.Create(GDIP_NOWRAP);
  format.SetTrimming(StringTrimmingEllipsisWord);
  try
      format.SetFormatFlags(format.GetFormatFlags or StringFormatFlagsMeasureTrailingSpaces);
      hintWindow.Canvas.MeasureString(DockletLabel, length(DockletLabel),  GFont, MakePoint(0.0, 0.0), format, captionSizeF);
      captionHeight := round(captionSizeF.Height);
      captionWidth := round(captionSizeF.Width)  ;

      posX := round(W.GetSize.cx / 2) + W.Position.X - round(captionWidth / 2);
      posY := round(W.Position.Y + W.GetSize.cy + 4);

      HintWindow.Size(captionWidth, captionHeight);
      HintWindow.Move(Point(posX, PosY));
      HintWindow.Clear;
      StringDraw(DockletLabel, GFont, 2, 2, captionWidth + 1, captionHeight + 1,
      HintWindow.Canvas, GFamily, format);
      HintWindow.Update(false);

  finally
      GFamily.Free;
      GFont.Free;
      format.Free;
  end;

  finally
    CaptionLocker.Leave;
  end;
end;

procedure TfrmMain.StringDraw(const AText: WideString; AFont: TGpFont; AX, AY, AW,
  AH: Integer; ACanvas: TGPGraphics; GFamily : TGPFontFamily; format : TGPStringFormat);
var
 BSW, BOX, BOY: Single;
 BTB: TGPSolidBrush;
 BTP: TGPGraphicsPath;
 BTE: TGPPen;
begin
  ACanvas.SetTextRenderingHint(TextRenderingHintAntiAlias);
  BTB := TGPSolidBrush.Create(aclWhite);

  BOX := ShadowOffsetXSizePercent / 100 * AFont.GetSize + ShadowOffsetXConstant;
  BOY := ShadowOffsetYSizePercent / 100 * AFont.GetSize + ShadowOffsetYConstant;
  BSW := ShadowWidthSizePercent / 100   * AFont.GetSize + ShadowWidthConstant;

  format.SetAlignment(StringAlignmentCenter);
  BTP := TGPGraphicsPath.Create;
  BTP.AddString(
    AText, Length(AText),
    GFamily,
    AFont.GetStyle,
    AFont.GetSize,
    MakeRect(0.0+AX, AY, AW, AH),
    format
  );
  BTE := TGPPen.Create(aclBlack, BSW);
  BTE.SetLineJoin(LineJoinRound);
  ACanvas.SetSmoothingMode(SmoothingModeAntiAlias);
  ACanvas.DrawPath(BTE, BTP);
  ACanvas.TranslateTransform(BOX, BOY);
  ACanvas.FillPath(BTB, BTP);
  ACanvas.TranslateTransform(-BOX, -BOY);


  BTB.Free;
  BTE.Free;
  BTP.Free;
end;

procedure TfrmMain.TimerFushTimer(Sender: TObject);
begin
  try
    EmptyWorkingSet(GetCurrentProcess);

  except on E: Exception do
    TimerFush.Enabled := false;
  end;
end;

procedure TfrmMain.WindowMoved(ANewPosition: TPoint);
begin
  MoveHintWindow;
end;

procedure TfrmMain.WmPaintCaption(var Message: TMessage);
begin
  PaintCaption;
end;

procedure TfrmMain.WmSetFile(var Message: TMessage);
var
  fileName : string;
  BackgroundName : string;
  AImageFileName : PChar;
begin

  AImageFileName := PChar(Message.WParam);

  ClearImageCache;

  if (AImageFileName <> nil) then
     begin
       BackgroundName := AImageFileName;
       if (SameText('default.png', BackgroundName)) then
       BackgroundName := 'Icons\Stardock ObjectDock.png';

       if ExtractFileDrive(BackgroundName) = '' then
         fileName := IncludeTrailingPathDelimiter(MainFolder) +  BackgroundName
           else
             fileName := BackgroundName;

       if (not FileExists(FileName)) then
        begin
          BackgroundName := ExtractFileName(FileName);
          FileName := IncludeTrailingPathDelimiter(DockletsFolder) + BackgroundName;
        end;

       if (not FileExists(FileName)) then
         begin
          BackgroundName := 'Icons\Stardock ObjectDock.png';
          fileName := IncludeTrailingPathDelimiter(MainFolder) +  BackgroundName;
         end;

       DockletImage := TGPBitmap.Create(fileName);
       BackgroundImage := BackgroundName;
     end;

  RepaintBase;
end;

procedure TfrmMain.WmSetImage(var Message: TMessage);
var
 AAutomaticallyDeleteImage : boolean;
 ANewImage : pointer;
 image : GPImage;
 clone : GPImage;
begin

  ANewImage := pointer(Message.WParam);
  AAutomaticallyDeleteImage := boolean(Message.LParam);

  if AAutomaticallyDeleteImage then
   begin
     ClearImageCache;
   end;

   try
     if DockletImage <> nil then
      DockletImage.Free;
   finally
      DockletImage := nil;
   end;

  DockletImagePointer := ANewImage;

  if (ANewImage <> nil) then
    begin
      image := ExtractNativeImage(ANewImage);
      GdipCloneImage(image, clone);
      DockletImage := TMyGPBitmap.Create(clone);
    end;

   RepaintBase();

end;

procedure TfrmMain.WmSetImageNative(var Message: TMessage);
var
 AAutomaticallyDeleteImage : boolean;
 ANewImage : pointer;
 clone : GPImage;
begin

  ANewImage := pointer(Message.WParam);
  AAutomaticallyDeleteImage := boolean(Message.LParam);

  if AAutomaticallyDeleteImage then
   begin
     ClearImageCache;
   end;

   try
     if DockletImage <> nil then
      DockletImage.Free;
   finally
      DockletImage := nil;
   end;

  DockletImagePointer := nil;

  if (ANewImage <> nil) then
    begin
      GdipCloneImage(ANewImage, clone);
      DockletImage := TMyGPBitmap.Create(clone);
    end;

   RepaintBase();

end;

procedure TfrmMain.WmSetOverlay(var Message: TMessage);
var
 AAutomaticallyDeleteImage : boolean;
 AImageOverlay : Pointer;
 image : GPImage;
 clone : GPImage;
begin

  AImageOverlay := Pointer(Message.WParam);
  AAutomaticallyDeleteImage := boolean(Message.LParam);

  if AAutomaticallyDeleteImage then
   begin
     ClearOverlayCache;
   end;

   try
     if DockletOverlay <> nil then
      DockletOverlay.Free;
   finally
      DockletOverlay := nil;
   end;

  DockletOverlayPointer := AImageOverlay;

  if (AImageOverlay <> nil) then
    begin
      image := ExtractNativeImage(AImageOverlay);
      GdipCloneImage(image, clone);
      DockletOverlay := TMyGPBitmap.Create(clone);
    end;

   RepaintBase();
end;

procedure TfrmMain.WmSetOverlayNative(var Message: TMessage);
var
 AAutomaticallyDeleteImage : boolean;
 AImageOverlay : Pointer;
 clone : GPImage;
begin

  AImageOverlay := Pointer(Message.WParam);
  AAutomaticallyDeleteImage := boolean(Message.LParam);

  if AAutomaticallyDeleteImage then
   begin
     ClearOverlayCache;
   end;

   try
     if DockletOverlay <> nil then
      DockletOverlay.Free;
   finally
      DockletOverlay := nil;
   end;

  DockletOverlayPointer := AImageOverlay;

  if (AImageOverlay <> nil) then
    begin
      GdipCloneImage(AImageOverlay, clone);
      DockletOverlay := TMyGPBitmap.Create(clone);
    end;

   RepaintBase();
end;

procedure TfrmMain.WmTerminate(var Message: TMessage);
begin
  Close;
end;

end.
