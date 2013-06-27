unit Docklets;

interface

uses Windows, SysUtils;

procedure DoOnGetInformation (AName: PChar; AAuthor: PChar; AVersion: PInteger;	ANotes: PChar); stdcall;
function  DoOnCreate(ADockletWindow: HWND; AInstance: THANDLE; AIni: PChar; AIniGroup: PChar): Pointer; stdcall;
procedure DoOnDestroy (AInstance: Pointer; ADockletWindow: HWND); stdcall;
function  DoOnLeftButtonClick (AInstance: Pointer; ACursor: PPOINT; ASize: PSIZE): BOOL; stdcall;
function  DoOnRightButtonClick (AInstance: Pointer; ACursor: PPOINT; ASize: PSIZE): BOOL; stdcall;
function  DoOnDoubleClick (AInstance: Pointer; ACursor: PPOINT; ASize: PSIZE): BOOL; stdcall;
procedure DoOnSave (AInstance: Pointer; AIni: PChar; AIniGroup: PChar; AIsForExport: BOOL); stdcall;
procedure DoOnProcessMessage (AInstance: Pointer;	ADockletWindow: HWND; AMessage: UINT; AWParam: WPARAM; ALParam: LPARAM); stdcall;
function  DoOnExportFiles (AInstance: Pointer; BFileRelativeOut: PChar; BIteration: Integer): BOOL; stdcall;
procedure DoOnDropFiles (lpData : pointer; dropData : THandle); stdcall;
function  DoOnAcceptDropFiles (lpData : pointer) : BOOL; stdcall;
function  DoOnLeftButtonHeld(AInstance: Pointer; ACursor: PPOINT; ASize: PSIZE): BOOL; stdcall;
procedure DoOnConfigure(AInstance : Pointer); stdcall;


var
VOnGetInformation : procedure (AName: PChar; AAuthor: PChar;
	AVersion: PInteger;
	ANotes: PChar); stdcall;

VOnCreate : function (ADockletWindow: HWND; AInstance: THANDLE;
	AIni: PChar;
	AIniGroup: PChar): Pointer; stdcall;

VOnDestroy : procedure (AInstance: Pointer; ADockletWindow: HWND); stdcall;

VOnLeftButtonClick : function (AInstance: Pointer; ACursor: PPOINT; ASize: PSIZE): BOOL; stdcall;

VOnRightButtonClick : function (AInstance: Pointer; ACursor: PPOINT; ASize: PSIZE): BOOL; stdcall;

VOnDoubleClick : function (AInstance: Pointer; ACursor: PPOINT; ASize: PSIZE): BOOL; stdcall;

VOnLeftButtonHeld : function (AInstance: Pointer; ACursor: PPOINT; ASize: PSIZE): BOOL; stdcall;

VOnConfigure : procedure (AInstance : Pointer); stdcall;

VOnSave : procedure (AInstance: Pointer; AIni: PChar; AIniGroup: PChar; AIsForExport: BOOL); stdcall;

VOnProcessMessage : procedure (AInstance: Pointer;	ADockletWindow: HWND; AMessage: UINT;
	AWParam: WPARAM;
	ALParam: LPARAM); stdcall;

VOnExportFiles : function (
	AInstance: Pointer;
	BFileRelativeOut: PChar;
	BIteration: Integer): BOOL; stdcall;

VOnDropFiles : procedure (
	lpData : pointer;
	dropData : THandle); stdcall;

VOnAcceptDropFiles : function (lpData : pointer) : BOOL; stdcall;

implementation

procedure DoOnGetInformation (AName: PChar; AAuthor: PChar; AVersion: PInteger; ANotes: PChar); stdcall;
begin
  try
   if Assigned(VOnGetInformation) then
     VOnGetInformation(AName, AAuthor, AVersion, ANotes);
  except
    {$IFDEF DEBUG}
    on E : Exception do
      OutputDebugString(PChar(E.Message));
    {$ENDIF}
  end;
end;

function DoOnCreate(ADockletWindow: HWND; AInstance: THANDLE; AIni: PChar; AIniGroup: PChar): Pointer; stdcall;
begin
   try
    if Assigned(VOnCreate) then
      Result := VOnCreate(ADockletWindow, AInstance, AIni, AIniGroup)
       else
         Result := nil;
   except
     Result := nil;
   end;
end;

procedure DoOnDestroy (AInstance: Pointer; ADockletWindow: HWND); stdcall;
begin
   if Assigned(VOnDestroy) then
    try
      VOnDestroy(AInstance, ADockletWindow);
    except
    {$IFDEF DEBUG}
    on E : Exception do
      OutputDebugString(PChar(E.Message));
    {$ENDIF}
    end;
end;

function  DoOnLeftButtonClick (AInstance: Pointer; ACursor: PPOINT; ASize: PSIZE): BOOL; stdcall;
begin
   if Assigned(VOnLeftButtonClick) then
    Result := VOnLeftButtonClick(AInstance, ACursor, ASize)
     else
       Result := false;
end;

function  DoOnRightButtonClick (AInstance: Pointer; ACursor: PPOINT; ASize: PSIZE): BOOL; stdcall;
begin
   if Assigned(VOnRightButtonClick) then
    Result := VOnRightButtonClick(AInstance, ACursor, ASize)
     else
      Result := false;
end;

function  DoOnDoubleClick (AInstance: Pointer; ACursor: PPOINT; ASize: PSIZE): BOOL; stdcall;
begin
  if Assigned(VOnDoubleClick) then
   Result := VOnDoubleClick(AInstance, ACursor, ASize)
     else
       Result := false;
end;

procedure DoOnSave (AInstance: Pointer; AIni: PChar; AIniGroup: PChar; AIsForExport: BOOL); stdcall;
begin
   try
   if Assigned(VOnSave) then
    begin
      if AInstance <> nil then
        VOnSave(AInstance, AIni, AIniGroup, AIsForExport);
    end;
   except

   end;
end;

procedure DoOnProcessMessage (AInstance: Pointer; ADockletWindow: HWND; AMessage: UINT; AWParam: WPARAM; ALParam: LPARAM); stdcall;
begin
  try
   if Assigned(VOnProcessMessage) then
     VOnProcessMessage(AInstance, ADockletWindow, AMessage, AWParam, ALParam);
  except
    {$IFDEF DEBUG}
    on E : Exception do
      OutputDebugString(PChar(E.Message));
    {$ENDIF}
  end;
end;

function  DoOnExportFiles (AInstance: Pointer; BFileRelativeOut: PChar; BIteration: Integer): BOOL; stdcall;
begin
  if Assigned(VOnExportFiles) then
    Result := VOnExportFiles(AInstance, BFileRelativeOut, BIteration)
      else
        Result := false;
end;

procedure DoOnDropFiles (lpData : pointer; dropData : THandle); stdcall;
begin
   if Assigned(VOnDropFiles) then
    VOnDropFiles(lpData, dropData);
end;

function  DoOnAcceptDropFiles (lpData : pointer) : BOOL; stdcall;
begin
   if Assigned(VOnAcceptDropFiles) then
    Result := VOnAcceptDropFiles(lpData)
      else
        Result := false;
end;

function  DoOnLeftButtonHeld(AInstance: Pointer; ACursor: PPOINT; ASize: PSIZE): BOOL; stdcall;
begin
  if Assigned(VOnLeftButtonHeld) then
    Result := VOnLeftButtonHeld(AInstance, ACursor, ASize)
      else
        Result := false;
end;

procedure DoOnConfigure(AInstance : Pointer); stdcall;
begin
   if Assigned(VOnConfigure) then
    try
      VOnConfigure(AInstance);
    except
    {$IFDEF DEBUG}
    on E : Exception do
      OutputDebugString(PChar(E.Message));
    {$ENDIF}
    end;
end;

end.
