unit ParentCheckerThread;

interface

uses
  Windows, Messages, SysUtils, Classes;

type
  TParentChecker = class(TThread)
  private
    { Private declarations }
  protected
    procedure Execute; override;
    procedure Shutdown;
  public
    ParentId : cardinal;
  end;

implementation

uses frm_main;

{ Important: Methods and properties of objects in visual components can only be
  used in a method called using Synchronize, for example,

      Synchronize(UpdateCaption);

  and UpdateCaption could look like,

    procedure ParentChecker.UpdateCaption;
    begin
      Form1.Caption := 'Updated in a thread';
    end; }

{ ParentChecker }

procedure TParentChecker.Execute;
var
  ParentProcHandle : THandle;
  WaitHandle : THandle;
begin
  ParentProcHandle := OpenProcess(Windows.SYNCHRONIZE, FALSE, ParentId);
  if ParentProcHandle > 0 then
    begin
       WaitHandle := WaitForSingleObject(ParentProcHandle, INFINITE);
       if WaitHandle = WAIT_OBJECT_0 then
         Synchronize(Shutdown);
    end;
end;

procedure TParentChecker.Shutdown;
begin
  PostMessage(frmMain.Handle, WM_CLOSE, 0, 0);
end;

end.
