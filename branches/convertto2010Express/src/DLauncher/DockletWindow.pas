unit DockletWindow;

interface

uses
  GDIPAPI, GDIPObj,  Messages, Types, SysUtils, Classes, Controls,
  Windows, Math;

function  PSCLastErrorText: string;
procedure PSCMainLoop;
procedure PSCProcessMessages;
procedure PSCQuitMainLoop;

////////////////////////////////////////////////////////////////////////////////
// PSC Window
////////////////////////////////////////////////////////////////////////////////

type
  TPSCWindowClass = TWndClass;

  TPSCWindowStyle = record
    FStyle: Cardinal;
    FStyleEx: Cardinal;
    x, y, w, h: Integer;
    FParent: Cardinal;
    FMenu: Cardinal;
  end;

  TPSCWindow = class
  protected
    FWindowName: String;
    FClassName: String;
    FWindowHandle: Cardinal;
    function OnWindowMessage(AMessage, AWParam, ALParam: Integer): Integer; virtual;
    procedure SetupWindowClass(var AWindowClass: TPSCWindowClass); virtual;
    procedure SetupWindowStyle(var AWindowStyle: TPSCWindowStyle); virtual;
    procedure CreatePSCWindow; virtual;
    procedure DestroyPSCWindow; virtual;
    procedure DoOnMouseEnter; virtual;
  public
    constructor Create;
    destructor Destroy; override;
    procedure DefaultHandler(var AMessage); override;
    property WindowHandle: Cardinal read FWindowHandle;
  end;

////////////////////////////////////////////////////////////////////////////////
// PSC Layered Window
////////////////////////////////////////////////////////////////////////////////

type
  TPSCWindowMessage = packed record
    FMessage: Cardinal;
    FWParam: Integer;
    FLParam: Integer;
    FResult: Integer;
  end;

const
  WS_EX_LAYERED = $80000;
  LWA_COLORKEY = $1;
  LWA_ALPHA = $2;
  ULW_COLORKEY = $1;
  ULW_ALPHA = $2;

type
  TOnLayerWindowMoved = procedure(ANewPosition: TPoint) of object;

  TPSCBlendFunction = record
    BlendOp: Byte;
    BlendFlags: Byte;
    SourceConstantAlpha: Byte;
    AlphaFormat: Byte;
  end;

  PPSCBlendFunction = ^TPSCBlendFunction;
  TPSCUpdateLayeredWindow = function(
    AHWND: HWND;
    AScreenDC: HDC;
    AWindowPosition: PPoint;
    AWindowSize: PSize;
    ASourceDC: HDC;
    ASourcePosition: PPoint;
    AColorKey: COLORREF;
    ABlendFunction: PPSCBlendFunction;
    AFlags: DWORD
 ): BOOL; stdcall;

  TPSCLayeredWindow = class(TPSCWindow)
  private
    FOnMouseUp: TMouseEvent;
    FOnMouseDown: TMouseEvent;
    clicked : boolean;
    ClickPoint : TPoint;
    Dragged : boolean;
    FOnMouseLeave: TNotifyEvent;
    FMouseWindow : THandle;
    FOnMouseEnter: TNotifyEvent;
  protected
    procedure SetupWindowClass(var AWindowClass: TPSCWindowClass); override;
    procedure SetupWindowStyle(var AWindowStyle: TPSCWindowStyle); override;
  protected
    UpdateLayeredWindow: TPSCUpdateLayeredWindow;
    FOnMoved: TOnLayerWindowMoved;
    FRect: TRect;
    FSize: TSize;
    FPosition: TPoint;
    FAlpha: Byte;
    FColorKey: COLORREF;
    FBuffer: TGPBitmap;
    FCanvas: TGPGraphics;
    FFlags: DWORD;
    procedure WMWIndowPosChanged(var AWindowsMessage : TMessage); message WM_WINDOWPOSCHANGED;
    procedure WMMouseLeave(var AWindowsMessage : TMessage); message WM_MOUSELEAVE;

    procedure WMMoved(var AWindowsMessage: TWMMove); message WM_MOVE;
    procedure WMMouseMove(var Message: TWMMouseMove); message WM_MOUSEMOVE;
    procedure AcceptFiles( var msg : TMessage );
      message WM_DROPFILES;

    procedure WMLButtonUp(var Message: TWMLButtonUp); message WM_LBUTTONUP;
    procedure WMNCLButtonUp(var Message: TWMLButtonUp); message WM_NCLBUTTONUP;
    procedure WMRButtonUp(var Message: TWMRButtonUp); message WM_RBUTTONUP;
    procedure WMNCRButtonUp(var Message: TWMLButtonUp); message WM_NCRBUTTONUP;

    procedure WMLButtonDown(var Message: TWMLButtonDown); message WM_LBUTTONDOWN;
    procedure WMNCLButtonDown(var Message: TWMLButtonDown); message WM_NCLBUTTONDOWN;
    procedure WMRButtonDown(var Message: TWMRButtonDown); message WM_RBUTTONDOWN;
    procedure WMNCRButtonDown(var Message: TWMLButtonDown); message WM_NCRBUTTONDOWN;

    procedure WMLButtonDblClk(var Message: TWMLButtonDblClk); message WM_LBUTTONDBLCLK;
    
    function GetUseAlpha: boolean;
    function GetUseColorKey: boolean;
    procedure SetUseAlpha(const Value: boolean);
    procedure SetUseColorKey(const Value: boolean);
    procedure DoMouseUp(var Message: TWMMouse; Button: TMouseButton);
    procedure MouseUp(Button: TMouseButton; Shift: TShiftState;
      X, Y: Integer); dynamic;
    procedure DoMouseDown(var Message: TWMMouse; Button: TMouseButton;
      Shift: TShiftState);
    procedure MouseDown(Button: TMouseButton; Shift: TShiftState;
      X, Y: Integer); dynamic;
    procedure DoOnMouseEnter;  override;
  public
    constructor Create;
    destructor Destroy; override;
    function GetSize : TSize;
    procedure DefaultHandler(var AMessage); override;
    property WindowHandle: Cardinal read FWindowHandle;
    procedure Move(ANewPosition: TPoint); overload;
    procedure Move(ANewPositionX, ANewPositionY: Integer); overload;
    procedure Move(ANewPositionX, ANewPositionY: Single); overload;
    procedure Size(ANewSize: TSize); overload;
    procedure Size(ANewSizeX, ANewSizeY: Integer); overload;
    procedure Size(ANewSizeX, ANewSizeY: Single); overload;
    procedure Rect(ANewRect: TRect); overload;
    procedure Rect(ANewRectLeft, ANewRectTop, ANewRectRight, ANewRectBottom: Integer); overload;
    procedure Clear; overload;
    procedure Clear(AX, AY, AW, AH: Integer); overload;
    procedure Clear(AX, AY, AW, AH: Single); overload;
    procedure Update(AMoveOnly: Boolean);
    procedure Show;
    procedure Hide;
    procedure SetWidth(Value : integer);
    procedure SetHeight(Value : integer);
    property Canvas: TGPGraphics read FCanvas;
    property Buffer: TGPBitmap read FBuffer;
    property Alpha: Byte read FAlpha write FAlpha;
    property ColorKey: COLORREF read FColorKey write FColorKey;
    property UseColorKey: boolean read GetUseColorKey write SetUseColorKey;
    property UseAlpha: boolean read GetUseAlpha write SetUseAlpha;
    property OnMoved: TOnLayerWindowMoved read FOnMoved write FOnMoved;
    property Position : TPoint read FPosition;
    property OnMouseUp: TMouseEvent read FOnMouseUp write FOnMouseUp;
    property OnMouseDown: TMouseEvent read FOnMouseDown write FOnMouseDown;
    property OnMouseLeave : TNotifyEvent read FOnMouseLeave write FOnMouseLeave;
    property OnMouseEnter : TNotifyEvent read FOnMouseEnter write FOnMouseEnter;
  end;

  TDockletWindow = class(TPSCLayeredWindow)
  private
    procedure WMClose(var AWindowsMessage: TWMClose); message WM_CLOSE;
  protected
    function OnWindowMessage(AMessage, AWParam, ALParam: Integer): Integer; override;
  end;

  TDockletHintWindow = class(TPSCLayeredWindow)
  public
    procedure DefaultHandler(var AMessage); override;
  end;

var
    WM_MOUSEENTER : integer;

const
   WM_TERMINATE = $B000 + 110;
    
implementation

uses Docklets, frm_main;

function PSCLastErrorText: string;
begin
  Result := SysErrorMessage(GetLastError);
end;

procedure PSCProcessMessages;
  var
    m: TMsg;
    q: Boolean;
begin
  q := False;
  while PeekMessage(m, 0, 0, 0, PM_REMOVE) do begin
    if m.message = WM_QUIT then begin
      q := True;
    end
    else begin
      TranslateMessage(m);
      DispatchMessage(m);
    end;
  end;
  if q then begin
    PostQuitMessage(0);
  end;
end;

procedure PSCMainLoop;
  var
    m: TMsg;
    r: Integer;
begin
  while TRUE do begin
    r := Integer(GetMessage(m, 0, 0, 0));
    if r <= 0 then break;
    TranslateMessage(m);
    DispatchMessage(m);
  end;
end;

procedure PSCQuitMainLoop;
begin
  PostThreadMessage(GetCurrentThreadId, WM_QUIT, 0, 0);
end;

function PSCWindowProcedure(AWindow: Cardinal; AMessage, FWParam, FLParam: Integer): Integer; stdcall;
  var
    BInstance: Integer;
begin
  try
    BInstance := GetWindowLong(AWindow, GWL_USERDATA);
    if BInstance = 0 then Result := DefWindowProc(AWindow, AMessage, FWParam, FLParam)
    else Result := TPSCLayeredWindow(BInstance).OnWindowMessage(AMessage, FWParam, FLParam);
  except
    on E: Exception do begin
      MessageBox(AWindow, PChar(E.Message), 'Uncaught exception', MB_OK + MB_ICONERROR + MB_SYSTEMMODAL);
      Result := 0; 
    end;
  end;
end;

{ TPSCLayeredWindow }

constructor TPSCLayeredWindow.Create;
begin
  WM_MOUSEENTER := RegisterWindowMessage('DLAUNCHER_MOUSE');
  UpdateLayeredWindow := TPSCUpdateLayeredWindow(GetProcAddress(GetModuleHandle('User32.dll'), 'UpdateLayeredWindow'));
  if not Assigned(UpdateLayeredWindow) then raise Exception.Create('No layered windows support detected. This program is running only on W2k or later.');
  //
  FBuffer := TGPBitmap.Create(10, 10, PixelFormat32bppPARGB);
  FCanvas := TGPGraphics.Create(FBuffer);
  //
  FPosition.x := 0;
  FPosition.y := 0;
  FSize.cx := 0;
  FSize.cy := 0;
  FAlpha := 255;
  FColorKey := 0;
  FFlags := ULW_ALPHA or ULW_COLORKEY;
  //
  FClassName := 'PSC LAYERED WINDOW';
  //
  FOnMoved := NIL;
  //
  inherited Create;
end;

destructor TPSCLayeredWindow.Destroy;
begin
  inherited Destroy;
  FCanvas.Free;
  FBuffer.Free;
end;

function KeysToShiftState(Keys: Word): TShiftState;
begin
  Result := [];
  if Keys and MK_SHIFT <> 0 then Include(Result, ssShift);
  if Keys and MK_CONTROL <> 0 then Include(Result, ssCtrl);
  if Keys and MK_LBUTTON <> 0 then Include(Result, ssLeft);
  if Keys and MK_RBUTTON <> 0 then Include(Result, ssRight);
  if Keys and MK_MBUTTON <> 0 then Include(Result, ssMiddle);
  if GetKeyState(VK_MENU) < 0 then Include(Result, ssAlt);
end;

procedure TPSCLayeredWindow.DoMouseDown(var Message: TWMMouse;
  Button: TMouseButton; Shift: TShiftState);
var
 Location : TPoint;
begin
  MouseDown(Button, Shift, Message.XPos, Message.YPos);
  Location.X := Message.XPos;
  Location.Y := Message.YPos;
  Windows.ClientToScreen(WindowHandle, Location);
  ClickPoint.X := Location.x;
  ClickPoint.Y := Location.y;

  SetCapture(WindowHandle);
  clicked := true;
end;

procedure TPSCLayeredWindow.DoMouseUp(var Message: TWMMouse;
  Button: TMouseButton);
begin
  ReleaseCapture;
  if (clicked and not dragged) then
   begin
     with Message do MouseUp(Button, KeysToShiftState(Keys), XPos, YPos);
   end;
  clicked := false;
  Dragged := false;
end;

procedure TPSCLayeredWindow.DoOnMouseEnter;
begin
  if Assigned(FOnMouseEnter) then
    FOnMouseEnter(Self);
end;

procedure TPSCLayeredWindow.SetupWindowStyle(var AWindowStyle: TPSCWindowStyle);
begin
  inherited;
  AWindowStyle.FStyle := WS_POPUP;// or WS_VISIBLE;
  AWindowStyle.FStyleEx := WS_EX_TOOLWINDOW or WS_EX_LAYERED;
  AWindowStyle.FParent := 0;
end;


procedure TPSCLayeredWindow.WMLButtonDblClk(var Message: TWMLButtonDblClk);
var
  P : TPoint;
  S : TSize;
begin
 P.X := Message.XPos;
 P.Y := Message.YPos;

  S.cx := frmMain.DockletSize;
  S.cy := frmMain.DockletSize;
  DoOnDoubleClick(frmMain.DockletData, @P, @S);
  inherited;
end;

procedure TPSCLayeredWindow.WMLButtonDown(var Message: TWMLButtonDown);
begin
  DoMouseDown(Message, mbLeft, []);
  inherited;
end;

procedure TPSCLayeredWindow.WMLButtonUp(var Message: TWMLButtonUp);
begin
  DoMouseUp(Message, mbLeft);
  inherited;
end;

procedure TPSCLayeredWindow.Update(AMoveOnly: Boolean);
  var
    AScreenDC: HDC;
    ASourceDC: HDC;
    ABufferBitmap: HBITMAP;
    AOldBitmapSelectedInCanvasDC: HBITMAP;
    ASourcePosition: TPoint;
    ABlendFunction: TPSCBlendFunction;
begin
  if AMoveOnly then begin
    MoveWindow(WindowHandle, FPosition.X, FPosition.Y, FSize.cx, FSize.cy, False);
    Exit;
  end;
  //
  AScreenDC := GetDC(0);
  //
  ASourcePosition.x := 0;
  ASourcePosition.y := 0;
  //
  ABlendFunction.BlendOp := AC_SRC_OVER;
  ABlendFunction.BlendFlags := 0;
  ABlendFunction.SourceConstantAlpha := FAlpha;
  ABlendFunction.AlphaFormat := $01;
  //
  ASourceDC := CreateCompatibleDC(AScreenDC);
  FBuffer.GetHBITMAP(0, ABufferBitmap);
  AOldBitmapSelectedInCanvasDC := SelectObject(ASourceDC, ABufferBitmap);
  //
  UpdateLayeredWindow(
    FWindowHandle,
    AScreenDC,
    @FPosition,
    @FSize,
    ASourceDC,
    @ASourcePosition,
    FColorKey,
    @ABlendFunction,
    FFlags
  );
  SelectObject(ASourceDC, AOldBitmapSelectedInCanvasDC);
  DeleteObject(ABufferBitmap);
  DeleteDC(ASourceDC);
  //
  ReleaseDC(0, AScreenDC);
end;

procedure TPSCLayeredWindow.WMMouseLeave(var AWindowsMessage: TMessage);
begin
  FMouseWindow := 0;
  if Assigned(FOnMouseLeave) then
     FOnMouseLeave(Self);
   inherited;  
end;

procedure TPSCLayeredWindow.WMMouseMove(var Message: TWMMouseMove);
var
  Location : TPoint;
  ScreenLocation : TPoint;
  deltaX : integer;
  deltaY : integer;
  event : TTrackMouseEvent;
begin
  if (FMouseWindow <> WindowHandle) then
    begin
      FMouseWindow := WindowHandle;
      event.dwFlags := 3;
      event.hwndTrack := WindowHandle;
      event.cbSize := sizeof(TTrackmouseEvent);
      TrackMouseEvent(event);
      SendMessage(WindowHandle, WM_MOUSEENTER, 0, 0);
     end;

  Location.X := Message.XPos;
  Location.Y := Message.YPos;
  ScreenLocation := Location;
  if (clicked) then
     begin
       Windows.ClientToScreen(WindowHandle, ScreenLocation);
       deltaX := (ScreenLocation.x - ClickPoint.X);
       deltaY := (ScreenLocation.y - ClickPoint.Y);

       if ((Abs(deltaX) > 1) or (Abs(deltaY) > 1)) then
        begin
            ClickPoint.X := ScreenLocation.x;
            ClickPoint.Y := ScreenLocation.y;
            Move(Position.X + deltaX, position.Y + deltaY);
            Update(true);
            Dragged := true;
        end
     end
       else
          begin
            Dragged := false;
          end;

  inherited;
end;

procedure TPSCLayeredWindow.WMMoved(var AWindowsMessage: TWMMove);
begin
  FPosition.x := AWindowsMessage.XPos;
  FPosition.y := AWindowsMessage.YPos;
  AWindowsMessage.Result := 0;
  if Assigned(FOnMoved) then FOnMoved(FPosition);
end;

procedure TPSCLayeredWindow.WMNCLButtonDown(var Message: TWMLButtonDown);
begin
  DoMouseDown(Message, mbLeft, []);
  inherited;
end;

procedure TPSCLayeredWindow.WMNCLButtonUp(var Message: TWMLButtonUp);
begin
  DoMouseUp(Message, mbLeft);
  inherited;
end;

procedure TPSCLayeredWindow.WMNCRButtonDown(var Message: TWMLButtonDown);
begin
  DoMouseDown(Message, mbRight, []);
  inherited;
end;

procedure TPSCLayeredWindow.WMNCRButtonUp(var Message: TWMLButtonUp);
begin
  DoMouseUp(Message, mbRight);
  inherited;
end;

procedure TPSCLayeredWindow.WMRButtonDown(var Message: TWMRButtonDown);
begin
  DoMouseDown(Message, mbRight, []);
  inherited;
end;

procedure TPSCLayeredWindow.WMRButtonUp(var Message: TWMRButtonUp);
begin
  DoMouseUp(Message, mbRight);
  inherited;
end;

procedure TPSCLayeredWindow.WMWIndowPosChanged(var AWindowsMessage: TMessage);
begin
  SetWindowPos(WindowHandle, HWND_BOTTOM, 0, 0, 0, 0, $413);
  AWindowsMessage.Result := 0;
  inherited;
end;

procedure TPSCLayeredWindow.Move(ANewPosition: TPoint);
begin
  with ANewPosition do Move(x, y);
end;

procedure TPSCLayeredWindow.Move(ANewPositionX, ANewPositionY: Integer);
begin
  if (FPosition.x = ANewPositionX) and (FPosition.y = ANewPositionY) then Exit;
  FPosition.x := ANewPositionX;
  FPosition.y := ANewPositionY;
  with FRect do begin
    Left := ANewPositionX;
    Top := ANewPositionY;
    Right := Left + FSize.cx;
    Bottom := Top + FSize.cy;
  end;
end;

procedure TPSCLayeredWindow.Size(ANewSize: TSize);
begin
  Size(ANewSize.cx, ANewSize.cy);
end;

procedure TPSCLayeredWindow.Size(ANewSizeX, ANewSizeY: Integer);
begin
  if (FSize.cx = ANewSizeX) and (FSize.cy = ANewSizeY) then Exit;
  FSize.cx := ANewSizeX;
  FSize.cy := ANewSizeY;
  FCanvas.Free;
  FBuffer.Free;
  FBuffer := TGPBitmap.Create(ANewSizeX, ANewSizeY, PixelFormat32bppPARGB);
  FCanvas := TGPGraphics.Create(FBuffer);
  with FRect do begin
    Left := FPosition.x;
    Top := FPosition.y;
    Right := Left + ANewSizeX;
    Bottom := Top + ANewSizeY;
  end;
end;

procedure TPSCLayeredWindow.Rect(ANewRect: TRect);
begin
  with ANewRect do Rect(Left, Top, Right, Bottom);
end;

procedure TPSCLayeredWindow.Rect(ANewRectLeft, ANewRectTop, ANewRectRight, ANewRectBottom: Integer);
begin
  with FRect do begin
    Left := ANewRectLeft;
    Top := ANewRectTop;
    Right := ANewRectRight;
    Bottom := ANewRectBottom;
  end;
  with FPosition do begin
    x := ANewRectLeft;
    y := ANewRectTop;
  end;
  with FSize do begin
    cx := ANewRectRight-ANewRectLeft;
    cy := ANewRectBottom-ANewRectTop;
  end;
end;

procedure TPSCLayeredWindow.Clear;
begin
  Canvas.Clear(FColorKey);
end;

function TPSCLayeredWindow.GetSize: TSize;
begin
   Result := FSize;
end;

function TPSCLayeredWindow.GetUseAlpha: boolean;
begin
  Result := (FFlags and ULW_ALPHA) <> 0;
end;

function TPSCLayeredWindow.GetUseColorKey: boolean;
begin
  Result := (FFlags and ULW_COLORKEY) <> 0;
end;

procedure TPSCLayeredWindow.SetUseAlpha(const Value: boolean);
begin
  if Value then begin
    FFlags := FFlags or ULW_ALPHA;
  end
  else begin
    FFlags := FFlags and not ULW_ALPHA;
  end;
end;

procedure TPSCLayeredWindow.SetUseColorKey(const Value: boolean);
begin
  if Value then begin
    FFlags := FFlags or ULW_COLORKEY;
  end
  else begin
    FFlags := FFlags and not ULW_COLORKEY;
  end;
end;

procedure TPSCLayeredWindow.SetWidth(Value: integer);
begin
  Size(Value, FSize.cy);
end;

procedure TDockletWindow.WMClose(var AWindowsMessage: TWMClose);
begin
  AWindowsMessage.Result := 0;
  //PSCQuitMainLoop;
  PostMessage(frmMain.Handle, WM_TERMINATE, 0, 0);
end;

procedure TPSCLayeredWindow.DefaultHandler(var AMessage);
begin

  with TPSCWindowMessage(AMessage) do begin
    if FMessage = cardinal(WM_MOUSEENTER) then
      DoOnMouseEnter
        else
           FResult := DefWindowProc(FWindowHandle, FMessage, FWParam, FLParam);
  end;
end;

procedure TPSCLayeredWindow.SetHeight(Value: integer);
begin
  Size(FSize.cx, Value);
end;

procedure TPSCLayeredWindow.SetupWindowClass(var AWindowClass: TPSCWindowClass);
begin
  inherited;
end;

procedure TPSCLayeredWindow.Clear(AX, AY, AW, AH: Integer);
  var
    BB: TGPSolidBrush;
begin
  BB := TGPSolidBrush.Create(FColorKey);
  Canvas.FillRectangle(BB, AX, AY, AW, AH);
  BB.Free;
end;

procedure TPSCLayeredWindow.AcceptFiles(var msg: TMessage);
begin
   DoOnDropFiles(frmMain.DockletData, msg.WParam);
end;

procedure TPSCLayeredWindow.Clear(AX, AY, AW, AH: Single);
  var
    BB: TGPSolidBrush;
begin
  BB := TGPSolidBrush.Create(FColorKey);
  Canvas.FillRectangle(BB, AX, AY, AW, AH);
  BB.Free;
end;

procedure TPSCLayeredWindow.MouseDown(Button: TMouseButton; Shift: TShiftState;
  X, Y: Integer);

begin
  if Assigned(FOnMouseDown) then FOnMouseDown(Self, Button, Shift, X, Y);
end;

procedure TPSCLayeredWindow.MouseUp(Button: TMouseButton; Shift: TShiftState; X,
  Y: Integer);
begin
  if Assigned(FOnMouseUp) then FOnMouseUp(Self, Button, Shift, X, Y);
end;

procedure TPSCLayeredWindow.Move(ANewPositionX, ANewPositionY: Single);
begin
  Move(Integer(Round(ANewPositionX)), Integer(Round(ANewPositionY)));
end;

procedure TPSCLayeredWindow.Size(ANewSizeX, ANewSizeY: Single);
begin
  Size(Integer(Round(ANewSizeX)), Integer(Round(ANewSizeY)));
end;

procedure TPSCLayeredWindow.Hide;
begin
  ShowWindow(WindowHandle, SW_HIDE);
end;

procedure TPSCLayeredWindow.Show;
begin
  ShowWindow(WindowHandle, SW_SHOW);
end;

{ TPSCWindow }

constructor TPSCWindow.Create;
begin
  FWindowHandle := 0;
  CreatePSCWindow;
end;

procedure TPSCWindow.CreatePSCWindow;
  var
    BWindowClass: TPSCWindowClass;
    BWindowStyle: TPSCWindowStyle;
    BRes: Cardinal;
begin
  SetupWindowClass(BWindowClass);
  SetupWindowStyle(BWindowStyle);
  //
  BRes := Windows.RegisterClass(BWindowClass);
  if BRes <> 0 then begin
    BRes := GetLastError;
    if (BRes <> 2)and(BRes <> 0) then begin // class exists
      raise Exception.Create('Could not register window class: ' + PSCLastErrorText);
    end;
  end;
  //
  FWindowHandle := Windows.CreateWindowEx(
    BWindowStyle.FStyleEx,
    BWindowClass.lpszClassName,
    PChar(FWindowName),
    BWindowStyle.FStyle,
    BWindowStyle.x,
    BWindowStyle.y,
    BWindowStyle.w,
    BWindowStyle.h,
    BWindowStyle.FParent,
    BWindowStyle.FMenu,
    BWindowClass.hInstance,
    NIL);
  if FWindowHandle = 0 then raise Exception.Create('Could not create window: ' + PSCLastErrorText);
  SetWindowLong(FWindowHandle, GWL_USERDATA, Integer(self));
end;

procedure TPSCWindow.DefaultHandler(var AMessage);
begin
  inherited;
end;

destructor TPSCWindow.Destroy;
begin
  DestroyPSCWindow;
  inherited;
end;

procedure TPSCWindow.DestroyPSCWindow;
 // var
 //   BRes: Boolean;
begin
  if FWindowHandle <> 0 then
  begin
    SetWindowLong(FWindowHandle, GWL_USERDATA, 0);
    //BRes :=
    DestroyWindow(FWindowHandle);
    FWindowHandle := 0;
    //if not BRes then raise Exception.Create('Could not destroy window: ' + PSCLastErrorText);
  end;
  UnregisterClass(PChar(FClassName), hinstance);
end;

procedure TPSCWindow.DoOnMouseEnter;
begin

end;

function TPSCWindow.OnWindowMessage(AMessage, AWParam,
  ALParam: Integer): Integer;
  var
    BWm: TPSCWindowMessage;
begin
  if (AMessage = WM_MOUSEENTER) then
   DoOnMouseEnter;
  BWm.FMessage := AMessage;
  BWm.FWParam := AWParam;
  BWm.FLParam := ALParam;
  BWm.FResult := 1;
  Dispatch(BWm);
  Result := BWm.FResult;
end;

procedure TPSCWindow.SetupWindowClass(var AWindowClass: TPSCWindowClass);
begin
  AWindowClass.Style := CS_DBLCLKS;
  AWindowClass.lpfnWndProc := @PSCWindowProcedure;
  AWindowClass.cbClsExtra := 0;
  AWindowClass.cbWndExtra := 0;
  AWindowClass.hInstance := hinstance;
  AWindowClass.hIcon := 0;
  AWindowClass.hCursor := LoadCursor(0, IDC_ARROW);
  AWindowClass.hbrBackground := 0;
  AWindowClass.lpszMenuName := NIL;
  AWindowClass.lpszClassName := PChar(FClassName);
end;

procedure TPSCWindow.SetupWindowStyle(var AWindowStyle: TPSCWindowStyle);
begin
  AWindowStyle.FStyle := 0;
  AWindowStyle.FStyleEx := 0;
  AWindowStyle.x := 0;
  AWindowStyle.y := 0;
  AWindowStyle.w := 0;
  AWindowStyle.h := 0;
  AWindowStyle.FParent := Cardinal(HWND_MESSAGE);
  AWindowStyle.FMenu := 0;
end;

{ TDockletWindow }

function TDockletWindow.OnWindowMessage(AMessage, AWParam,
  ALParam: Integer): Integer;
begin
  DoOnProcessMessage(frmMain.DockletData, frmMain.W.WindowHandle, AMessage, AWParam, ALParam);
  Result := inherited OnWindowMessage(AMessage, AWParam, ALParam);
end;

{ TDockletHintWindow }

procedure TDockletHintWindow.DefaultHandler(var AMessage);
begin
  with TPSCWindowMessage(AMessage) do begin
    if FMessage = cardinal(WM_SYSCOMMAND) then
      begin
        if FWParam = SC_CLOSE then
          exit;
      end;
  end;
  inherited;
end;

end.
