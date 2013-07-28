unit frm_settings;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls;

type
  TfrmSettings = class(TForm)
    Label1: TLabel;
    edtLabel: TEdit;
    btnOK: TButton;
    btnCancel: TButton;
    Label2: TLabel;
    edtIcon: TEdit;
    btnSelectImage: TButton;
    dlgOpen: TOpenDialog;
    procedure btnSelectImageClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  frmSettings: TfrmSettings;

implementation

uses SpecialFolders;

{$R *.dfm}

procedure TfrmSettings.btnSelectImageClick(Sender: TObject);
var
  FileName : string;
  RelativePath : string;
begin
  dlgOpen.InitialDir := MainFolder;
  if (dlgOpen.Execute) then
    begin
      FileName := dlgOpen.FileName;
      RelativePath := ExtractRelativePath(IncludeTrailingPathDelimiter(MainFolder), FileName);
      edtIcon.Text := RelativePath;
     end;
end;

end.
