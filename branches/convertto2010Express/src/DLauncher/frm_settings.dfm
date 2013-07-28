object frmSettings: TfrmSettings
  Left = 0
  Top = 0
  BorderStyle = bsDialog
  Caption = 'Settings'
  ClientHeight = 141
  ClientWidth = 246
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  Position = poScreenCenter
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 8
    Top = 8
    Width = 48
    Height = 13
    Margins.Bottom = 0
    Caption = 'Label text'
  end
  object Label2: TLabel
    Left = 8
    Top = 51
    Width = 21
    Height = 13
    Margins.Bottom = 0
    Caption = 'Icon'
  end
  object edtLabel: TEdit
    Left = 8
    Top = 24
    Width = 230
    Height = 21
    TabOrder = 0
  end
  object btnOK: TButton
    Left = 82
    Top = 104
    Width = 75
    Height = 25
    Caption = '&OK'
    Default = True
    ModalResult = 1
    TabOrder = 3
  end
  object btnCancel: TButton
    Left = 163
    Top = 104
    Width = 75
    Height = 25
    Cancel = True
    Caption = '&Cancel'
    ModalResult = 2
    TabOrder = 4
  end
  object edtIcon: TEdit
    Left = 8
    Top = 67
    Width = 199
    Height = 21
    TabOrder = 1
  end
  object btnSelectImage: TButton
    Left = 217
    Top = 67
    Width = 21
    Height = 21
    Caption = '...'
    TabOrder = 2
    OnClick = btnSelectImageClick
  end
  object dlgOpen: TOpenDialog
    DefaultExt = '.png'
    Filter = 'Images (*.png)|*.png'
    Options = [ofHideReadOnly, ofFileMustExist, ofNoNetworkButton, ofEnableSizing, ofDontAddToRecent]
    OptionsEx = [ofExNoPlacesBar]
    Left = 144
    Top = 104
  end
end
