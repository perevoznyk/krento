object frmMain: TfrmMain
  Left = 0
  Top = 0
  Caption = 'DLauncher'
  ClientHeight = 197
  ClientWidth = 406
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -15
  Font.Name = 'Tahoma'
  Font.Style = [fsBold]
  OldCreateOrder = False
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  PixelsPerInch = 96
  TextHeight = 18
  object CloseTimer: TTimer
    Enabled = False
    OnTimer = CloseTimerTimer
    Left = 48
    Top = 64
  end
  object DockletMenu: TPopupMenu
    Left = 176
    Top = 64
    object miConfigure: TMenuItem
      Caption = 'Configure'
      OnClick = miConfigureClick
    end
    object miClose: TMenuItem
      Caption = 'Close'
      OnClick = miCloseClick
    end
    object miDelete: TMenuItem
      Caption = 'Delete'
      OnClick = miDeleteClick
    end
  end
  object dlgOpen: TOpenDialog
    DefaultExt = '.png'
    Filter = 'Images (*.png)|*.png'
    Options = [ofHideReadOnly, ofFileMustExist, ofNoNetworkButton, ofEnableSizing, ofDontAddToRecent]
    OptionsEx = [ofExNoPlacesBar]
    Left = 64
    Top = 136
  end
  object ApplicationEvents1: TApplicationEvents
    OnException = ApplicationEvents1Exception
    Left = 264
    Top = 64
  end
  object TimerFush: TTimer
    Interval = 3600000
    OnTimer = TimerFushTimer
    Left = 256
    Top = 136
  end
  object AnimationTimer: TTimer
    Enabled = False
    Interval = 100
    OnTimer = AnimationTimerTimer
    Left = 112
    Top = 16
  end
end
