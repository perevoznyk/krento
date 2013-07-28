unit SupportData;

interface

uses
  Windows, Messages, SysUtils, Math, GDIPApi, GDIPObj,
  NativeThemeManager, System.WideStrUtils;

const
   WM_BASE = $B000 + 100;
   WM_SET_OVERLAY = WM_BASE + 01;
   WM_SET_IMAGE   = WM_BASE + 02;
   WM_SET_FILE    = WM_BASE + 03;
   WM_PAINT_CAPTION = WM_BASE + 04;
   WM_SET_IMAGE_NATIVE = WM_BASE + 05;
   WM_SET_OVERLAY_NATIVE = WM_BASE + 06;

function  DockletIsVisible (ADocklet: HWND): BOOL; stdcall;
function  DockletGetRect (ADockletWindow: HWND; ADockletRect: PRect): BOOL; stdcall;
function  DockletGetLabel(ADockletWindow: HWND; ALabel: PWideChar): Integer; stdcall;
procedure DockletSetLabel(ADockletWindow: HWND; ALabel: Pointer); stdcall;
function  DockletLoadGDIPlusImage(AImageFileName: PWideChar): Pointer; stdcall;
procedure DockletSetImage(ADockletWindow: HWND; ANewImage: Pointer; AAutomaticallyDeleteImage: BOOL); stdcall;
procedure DockletSetImageFile(ADockletWindow: HWND; AImageFileName: PWideChar); stdcall;
procedure DockletSetImageOverlay(ADockletWindow: HWND; AImageOverlay: Pointer; AAutomaticallyDeleteImage: BOOL); stdcall;
function  DockletBrowseForImage(AParentWindow: HWND; AImageFileName: PWideChar; AAlternateRelativeRoot: PWideChar): BOOL; stdcall;
procedure DockletLockMouseEffect(ADockletWindow: HWND; ALock: BOOL); stdcall;
procedure DockletDoAttentionAnimation(ADockletWindow: HWND); stdcall;
procedure DockletGetRelativeFolder(ADockletWindow: HWND; AFolder: PWideChar); stdcall;
procedure DockletGetRootFolder(ADockletWindow: HWND; ARootFolder: PWideChar); stdcall;
procedure DockletDefaultConfigDialog(ADockletWindow: HWND); stdcall;
function  DockletQueryDockEdge(ADockletWindow: HWND): Integer; stdcall;
function  DockletQueryDockAlign(ADockletWindow: HWND): Integer; stdcall;
function  DockletSetDockEdge(ADockletWindow: HWND; ANewEdge: Integer): integer; stdcall;
function  DockletSetDockAlign (ADockletWindow: HWND; ANewAlign: Integer): integer; stdcall;

//Extra functions for Krento SDK

function  DockletGetThemeImage(AImageName : PWideChar) : Pointer; stdcall;
procedure DockletGetMainFolder(AFolderName : PWideChar); stdcall;
procedure DockletSetNativeImage(ADockletWindow: HWND; ANewImage: Pointer; AAutomaticallyDeleteImage: BOOL); stdcall;
procedure DockletSetNativeImageOverlay(ADockletWindow: HWND; AImageOverlay: Pointer; AAutomaticallyDeleteImage: BOOL); stdcall;
procedure DockletGetAbsoluteFolder(ADockletWindow: HWND; AFolder: PWideChar); stdcall;
procedure DockletGetLanguage(ALanguage : PWideChar); stdcall;

procedure DockletSetWindowWidth(ADockletWindow : HWND; ANewWidth : integer); stdcall;
procedure DockletSetWindowHeight(ADockletWindow : HWND; ANewHeight : integer); stdcall;

function LoadGDIPlusImage(AFileName : PWideChar) : pointer; stdcall; external 'DGraphics.dll';
function ExtractNativeImage(AImage : Pointer) : pointer; stdcall; external 'DGraphics.dll';
procedure DeleteImage(AImage : Pointer); stdcall; external 'DGraphics.dll';


exports
 DockletIsVisible,
 DockletGetRect,
 DockletGetLabel,
 DockletSetLabel,
 DockletLoadGDIPlusImage,
 DockletSetImage,
 DockletSetImageFile,
 DockletSetImageOverlay,
 DockletBrowseForImage,
 DockletLockMouseEffect,
 DockletDoAttentionAnimation,
 DockletGetRelativeFolder,
 DockletGetRootFolder,
 DockletDefaultConfigDialog,
 DockletQueryDockEdge,
 DockletQueryDockAlign,
 DockletSetDockEdge,
 DockletSetDockAlign,
 DockletGetThemeImage,
 DockletGetMainFolder,
 DockletSetNativeImage,
 DockletSetNativeImageOverlay,
 DockletGetAbsoluteFolder,
 DockletGetLanguage,
 DockletSetWindowWidth,
 DockletSetWindowHeight;


implementation

uses frm_main, SpecialFolders;

type TMyGPBitmap = class(TGPBitmap);

function IsUnicode ( pData : Pointer) : boolean;
var
  S : PAnsiChar;
begin
  S := PAnsiChar(pData);
  if Strlen(S) = 1 then
   begin
     if ( byte(S[0]) <> 0 ) and ( byte(S[1]) = 0) then
      Result := true
        else
          Result := false;
   end
     else
     begin
       Result := False;
     end;
end;

function DockletIsVisible (ADocklet: HWND): BOOL; stdcall;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletIsVisible');
  {$ENDIF}
  Result := IsWindowVisible(ADocklet);
end;

function  DockletGetRect (ADockletWindow: HWND; ADockletRect: PRect): BOOL; stdcall;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletGetRect');
  {$ENDIF}
  if (ADockletRect <> nil) then
    begin
      ADockletRect^.Left :=  frmMain.W.Position.X;
      ADockletRect^.Top := frmMain.W.Position.Y;
      ADockletRect^.Right := frmMain.W.Position.X + frmMain.W.GetSize.cx;
      ADockletRect^.Bottom := frmMain.W.Position.Y + frmMain.W.GetSize.cy;
      Result := true;
    end
      else
        Result := false;
end;

function  DockletGetLabel(ADockletWindow: HWND; ALabel: PWideChar): Integer; stdcall;
var
 L : integer;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletGetLabel');
  {$ENDIF}

  L := Length(frmMain.DockletLabel);
  if (ALabel <> nil) then
    StrLCopy(ALabel, PWideChar(frmMain.DockletLabel), L);

  Result := L;
end;


procedure DockletSetLabel(ADockletWindow: HWND; ALabel: Pointer); stdcall;
var
 iLen : integer;
 LA : PAnsiChar;
 LW : PWideChar;
 S : AnsiString;
 W : UnicodeString;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletSetLabel');
  {$ENDIF}

  try
  if (ALabel = nil) then
     begin
       frmMain.DockletLabel := '';
       Exit;
     end;

  if (IsUnicode(ALabel)) then
    begin
        LW := PWideChar(ALabel);
        ilen := strlen(LW);
        SetLength(frmMain.DockletLabel, ilen);
        FillChar(frmMain.DockletLabel[1], ilen, #0);
        StrLCopy(PWideChar(frmMain.DockletLabel), LW, ilen);
    end
      else
      begin
        LA := PAnsiChar(ALabel);
        S := AnsiString(La);
        W := WideString(S);
        LW := PWideChar(W);
        ilen := strlen(LW);
        SetLength(frmMain.DockletLabel, ilen);
        FillChar(frmMain.DockletLabel[1], ilen, #0);
        StrLCopy(PWideChar(frmMain.DockletLabel), LW, ilen);
      end;


  except
    frmMain.DockletLabel := '';
  end;
  SendMessage(frmMain.Handle, WM_PAINT_CAPTION, 0, 0);
end;

function  DockletLoadGDIPlusImage(AImageFileName: PWideChar): pointer; stdcall;
var
  S : string;
  SW : WideString;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletLoadGDIPlusImage');
  {$ENDIF}
  if (AImageFileName = nil) then
    begin
      Result := nil;
      exit;
    end;
  if (strlen(AImageFileName) = 0) then
    begin
      Result := nil;
      exit;
    end;


  S := IncludeTrailingPathDelimiter(MainFolder) + AImageFileName;

  SW := S;
  Result := LoadGDIPlusImage(PWideChar(SW));
end;


procedure DockletSetImage(ADockletWindow: HWND; ANewImage: Pointer; AAutomaticallyDeleteImage: BOOL); stdcall;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletSetImage');
  {$ENDIF}

  SendMessage(frmMain.Handle, WM_SET_IMAGE, integer(ANewImage), integer(AAutomaticallyDeleteImage));

end;

procedure DockletSetImageFile(ADockletWindow: HWND; AImageFileName: PWideChar); stdcall;
var
  LocalImageFile : PWideChar;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletSetImageFile');
  {$ENDIF}

  LocalImageFile := StrNew(AImageFileName);

  SendMessage(frmMain.Handle, WM_SET_FILE, integer(LocalImageFile), 0);
end;

procedure DockletSetImageOverlay(ADockletWindow: HWND; AImageOverlay: Pointer; AAutomaticallyDeleteImage: BOOL); stdcall;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletSetImageOverlay');
  {$ENDIF}

  SendMessage(frmMain.Handle, WM_SET_OVERLAY, integer(AImageOverlay), integer(AAutomaticallyDeleteImage));

end;

function  DockletBrowseForImage(AParentWindow: HWND; AImageFileName: PWideChar; AAlternateRelativeRoot: PWideChar): BOOL; stdcall;
var
 FileName : string;
 RelativePath : string;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletBrowseForImage');
  {$ENDIF}
  frmMain.dlgOpen.InitialDir := MainFolder;
  if (frmMain.dlgOpen.Execute(frmMain.W.WindowHandle)) then
   begin
     FileName := frmMain.dlgOpen.FileName;
     RelativePath := ExtractRelativePath(IncludeTrailingPathDelimiter(MainFolder), FileName);
     StrLCopy(AImageFileName, PWideChar(RelativePath), length(RelativePath));
     Result := true;
   end
     else
        Result := false;
end;

procedure DockletLockMouseEffect(ADockletWindow: HWND; ALock: BOOL); stdcall;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletLockMouseEffect');
  {$ENDIF}
end;

procedure DockletDoAttentionAnimation(ADockletWindow: HWND); stdcall;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletDoAttentionAnimation');
  {$ENDIF}
  frmMain.AnimationStep := 0;
  frmMain.AnimationTimer.Enabled := true;
end;

procedure DockletGetRelativeFolder(ADockletWindow: HWND; AFolder: PWideChar); stdcall;
var
 dir : string;
 L : integer;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletGetRelativeFolder');
  {$ENDIF}
  dir := IncludeTrailingPathDelimiter(frmMain.RelativePath);
  L := length(dir);
  StrLCopy(AFolder, PWideChar(Dir), L);
end;

procedure DockletGetAbsoluteFolder(ADockletWindow: HWND; AFolder: PWideChar); stdcall;
var
 dir : string;
 L : integer;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletGetAbsoluteFolder');
  {$ENDIF}
  dir := IncludeTrailingPathDelimiter(ExtractFilePath(frmmain.ConfigFileName));
  L := length(dir);
  StrLCopy(AFolder, PWideChar(Dir), L);
end;

procedure DockletGetRootFolder(ADockletWindow: HWND; ARootFolder: PWideChar); stdcall;
var
 dir : string;
 L : integer;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletGetRootFolder');
  {$ENDIF}
  dir := IncludeTrailingPathDelimiter(MainFolder);
  L := length(dir);
  StrLCopy(ARootFolder, PWideChar(Dir), L);
end;

procedure DockletGetLanguage(ALanguage : PWideChar); stdcall;
begin
  StrCopy(ALanguage, PWideChar(frmMain.LanguageName));
end;

procedure DockletDefaultConfigDialog(ADockletWindow: HWND); stdcall;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletDefaultConfigDialog');
  {$ENDIF}
end;

function  DockletQueryDockEdge(ADockletWindow: HWND): Integer; stdcall;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletQueryDockEdge');
  {$ENDIF}
  Result := 0;
end;

function  DockletQueryDockAlign(ADockletWindow: HWND): Integer; stdcall;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletQueryDockAlign');
  {$ENDIF}
  Result := 1;
end;

function  DockletSetDockEdge(ADockletWindow: HWND; ANewEdge: Integer): integer; stdcall;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletQueryDockAlign');
  {$ENDIF}
  Result := 0;
end;

function DockletSetDockAlign (ADockletWindow: HWND; ANewAlign: Integer): integer; stdcall;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletSetDockAlign');
  {$ENDIF}
  Result := 1;
end;

function DockletGetThemeImage(AImageName : PWideChar) : Pointer; stdcall;
var
  Image : TGPBitmap;
begin
  Image := TNativeThemeManager.LoadBitmap(AImageName);
  if Image = nil then
   Exit(nil)
     else
       begin
         Result := TMyGPBitmap(Image).nativeImage;
       end;
end;

procedure DockletGetMainFolder(AFolderName : PWideChar); stdcall;
begin
  StrCopy(AFolderName, PWideChar(MainFolder));
end;

procedure DockletSetNativeImage(ADockletWindow: HWND; ANewImage: Pointer; AAutomaticallyDeleteImage: BOOL); stdcall;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletSetNativeImage');
  {$ENDIF}

  SendMessage(frmMain.Handle, WM_SET_IMAGE_NATIVE, integer(ANewImage), integer(AAutomaticallyDeleteImage));
end;

procedure DockletSetNativeImageOverlay(ADockletWindow: HWND; AImageOverlay: Pointer; AAutomaticallyDeleteImage: BOOL); stdcall;
begin
  {$IFDEF DEBUG}
  OutputDebugString('DockletSetNativeImageOverlay');
  {$ENDIF}

  SendMessage(frmMain.Handle, WM_SET_OVERLAY_NATIVE, integer(AImageOverlay), integer(AAutomaticallyDeleteImage));
end;

procedure DockletSetWindowWidth(ADockletWindow : HWND; ANewWidth : integer); stdcall;
begin
   frmMain.W.SetWidth(ANewWidth);
   frmMain.MoveHintWindow;
end;

procedure DockletSetWindowHeight(ADockletWindow : HWND; ANewHeight : integer); stdcall;
begin
  frmMain.W.SetHeight(ANewHeight);
  frmMain.MoveHintWindow;
end;

end.
