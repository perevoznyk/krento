      {******************************************************************}
      { GDI+ Util                                                         }
      {                                                                  }
      { home page : http://www.progdigy.com                              }
      { email     : hgourvest@progdigy.com                               }
      {                                                                  }
      { date      : 15-02-2002                                           }
      {                                                                  }
      { The contents of this file are used with permission, subject to   }
      { the Mozilla Public License Version 1.1 (the "License"); you may  }
      { not use this file except in compliance with the License. You may }
      { obtain a copy of the License at                                  }
      { http://www.mozilla.org/MPL/MPL-1.1.html                          }
      {                                                                  }
      { Software distributed under the License is distributed on an      }
      { "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or   }
      { implied. See the License for the specific language governing     }
      { rights and limitations under the License.                        }
      {                                                                  }
      { *****************************************************************}

unit GDIPUTIL;

interface
uses
  Windows,
  Classes,
  SysUtils,
  Consts,
  ActiveX,
  Math,
  ShellAPI,
  Forms,
  ImgList,
  commctrl,
  Controls,
  Graphics,
  GDIPAPI,
  GDIPOBJ;

const
  GDIP_NOWRAP = 4096;
  
const
  GrayColorMatrix : TColorMatrix = (
  (0.299, 0.299, 0.299, 0.0, 0.0),
  (0.587, 0.587, 0.587,	0.0, 0.0),
  (0.114, 0.114, 0.114, 0.0, 0.0),
  (0.0,   0.0,   0.0,	1.0, 0.0),
  (0.0,   0.0,   0.0,   0.0, 1.0 ));

  SepiaColorMatrix : TColorMatrix = (
  (0.393, 0.349, 0.272, 0.0, 0.0),
  (0.769, 0.686, 0.534, 0.0, 0.0),
  (0.189, 0.168, 0.131, 0.0, 0.0),
  (0.0,   0.0,   0.0,   1.0, 0.0),
  (0.0,   0.0,   0.0,   0.0, 1.0));


  DimColorMatrix : TColorMatrix = (
  (0.9, 0.0, 0.0, 0.0, 0.0),
  (0.0, 0.9, 0.0, 0.0, 0.0),
  (0.0, 0.0, 0.9, 0.0, 0.0),
  (0.0, 0.0, 0.0, 1.0, 0.0),
  (0.0, 0.0, 0.0, 0.0, 1.0));

type
  TAntiAlias = (aaNone, aaClearType, aaAntiAlias);

  TGPImageFormat = (ifUndefined, ifMemoryBMP, ifBMP, ifEMF, ifWMF, ifJPEG,
    ifPNG, ifGIF, ifTIFF, ifEXIF, ifIcon);

  PHICON = ^HICON;

function ValueTypeFromULONG(Type_: ULONG): String;
function GetMetaDataIDString(id: ULONG): string;
function GetEncoderClsid(format: String; out pClsid: TGUID): integer;
function GetStatus(Stat: TStatus): string;
function PixelFormatString(PixelFormat: TPixelFormat): string;

function ColorToARGB(Color: TColor): ARGB;
function MakeRectF(x, y, width, height: Single): TGPRectF;
function MakePointF(X, Y: Single): TGPPointF;


procedure DrawRoundRectangle(graphic: TGPGraphics; R: TRect; Radius: Integer; Clr: TColor);
function  GetGPImageFormat(Image : TGPImage): TGPImageFormat;
function  GDIPlusBitampIntoLayeredWindow(frm:Tform; bmp: TGPBitmap):Integer;

procedure ConvertTo32BitImageList(const ImageList: TCustomImageList);
procedure AddIconFileToImageList(const FileName: string; IconIndex: Integer; const ImageList: TImageList);
function  AddIconResourceToImageList(const ResourceName: string; const ImageList: TImageList): integer;
function  GetFileIcon(const FileName: string; IconIndex: Integer): THandle;
procedure AddPngFileToImageList(const FileName: string; const ImageList: TImageList; smoothingMode: TSmoothingMode = SmoothingModeAntiAlias);

function  GetGPImageFromStream(Stream : TStream) : TGPBitmap;
function  GetGPImageFromMemoryStream(Stream : TMemoryStream) : TGPImage;

function  GetHIconFromBitmap(Image : TGPBitmap; Size : integer = 32) : HICON;

procedure ImageListDrawDisabled(Images: TCustomImageList; Canvas: TCanvas;
  X, Y, Index: Integer; HighlightColor, GrayColor: TColor; DrawHighlight: Boolean);

function  GrayColor(ACanvas: TCanvas; clr: TColor; Value: integer): TColor;
procedure GrayBitmap(ABitmap: TBitmap; Value: integer);
procedure GrayBitmapTransparent(ABitmap: TBitmap; Value: integer; TransparentColor: TColor);
procedure DrawTransparentBitmap(DC: HDC; hBmp : HBITMAP ;
          xStart: integer; yStart : integer; cTransparentColor : COLORREF);

function GetRealColor(AColor : TColor) : TColor;

function NewColors(DC : HDC; clr: TColor; RedPart, GreenPart, BluePart: integer): TColor;
function NewColor (DC : HDC; clr: TColor; Value: integer; AllColors : boolean = true): TColor;

function GetHighlightColor(BaseColor: TColor): TColor;
function GetShadowColor(BaseColor: TColor): TColor;
function Darker(Color:TColor; Percent:Byte):TColor;
function Lighter(Color:TColor; Percent:Byte):TColor;

procedure DrawRealFrame(DC : HDC; Rect : TRect; AColor : TColor; Down : boolean);

{from WinNT.h}
// creates a language identifier from a primary language identifier and a
// sublanguage identifier for the TStringFormat & TFontFamily class.
function MakeLangID(PrimaryLanguage, SubLanguage: LANGID): WORD;

implementation

type
  //bug-fixing for CodeGear implementation
  TGPStreamAdapter = class(TStreamAdapter)
  public
    function Stat(out statstg: TStatStg; grfStatFlag: Longint): HResult; override; stdcall;
  end;

function GetGPImageFormat(Image : TGPImage): TGPImageFormat;
var
  format: TGUID;
begin

  Image.GetRawFormat(format);

  Result := ifUndefined;

  if IsEqualGUID(format, ImageFormatMemoryBMP) then
    Result := ifMemoryBMP;

  if IsEqualGUID(format, ImageFormatBMP) then
    Result := ifBMP;

  if IsEqualGUID(format, ImageFormatEMF) then
    Result := ifEMF;

  if IsEqualGUID(format, ImageFormatWMF) then
    Result := ifWMF;

  if IsEqualGUID(format, ImageFormatJPEG) then
    Result := ifJPEG;

  if IsEqualGUID(format, ImageFormatGIF) then
    Result := ifGIF;

  if IsEqualGUID(format, ImageFormatPNG) then
    Result := ifPNG;

  if IsEqualGUID(format, ImageFormatTIFF) then
    Result := ifTIFF;

  if IsEqualGUID(format, ImageFormatEXIF) then
    Result := ifEXIF;

  if IsEqualGUID(format, ImageFormatIcon) then
    Result := ifIcon;
end;

function ColorToARGB(Color: TColor): ARGB;
var
  c: TColor;
begin
  c := ColorToRGB(Color);
  Result := ARGB( $FF000000 or ((DWORD(c) and $FF) shl 16) or ((DWORD(c) and $FF00) or ((DWORD(c) and $ff0000) shr 16)));
end;

function MakeRectF(x, y, width, height: Single): TGPRectF;
begin
  Result.X      := x;
  Result.Y      := y;
  Result.Width  := width;
  Result.Height := height;
end;


function MakePointF(X, Y: Single): TGPPointF;
begin
  Result.X := X;
  result.Y := Y;
end;

function ValueTypeFromULONG(Type_: ULONG): String;
begin
  case Type_ of
    0 : result := 'Nothing';
    1 : result := 'PropertyTagTypeByte';
    2 : result := 'PropertyTagTypeASCII';
    3 : result := 'PropertyTagTypeShort';
    4 : result := 'PropertyTagTypeLong';
    5 : result := 'PropertyTagTypeRational';
    6 : result := 'Nothing';
    7 : result := 'PropertyTagTypeUndefined';
    8 : result := 'Nothing';
    9 : result := 'PropertyTagTypeSLONG';
    10: result := 'PropertyTagTypeSRational';
  else
    result := '<Unknown>';
  end;
end;

function GetMetaDataIDString(id: ULONG): string;
begin
  case id of
    PropertyTagExifIFD                        : result := 'PropertyTagExifIFD';
    PropertyTagGpsIFD                         : result := 'PropertyTagGpsIFD';
    PropertyTagNewSubfileType                 : result := 'PropertyTagNewSubfileType';
    PropertyTagSubfileType                    : result := 'PropertyTagSubfileType';
    PropertyTagImageWidth                     : result := 'PropertyTagImageWidth';
    PropertyTagImageHeight                    : result := 'PropertyTagImageHeight';
    PropertyTagBitsPerSample                  : result := 'PropertyTagBitsPerSample';
    PropertyTagCompression                    : result := 'PropertyTagCompression';
    PropertyTagPhotometricInterp              : result := 'PropertyTagPhotometricInterp';
    PropertyTagThreshHolding                  : result := 'PropertyTagThreshHolding';
    PropertyTagCellWidth                      : result := 'PropertyTagCellWidth';
    PropertyTagCellHeight                     : result := 'PropertyTagCellHeight';
    PropertyTagFillOrder                      : result := 'PropertyTagFillOrder';
    PropertyTagDocumentName                   : result := 'PropertyTagDocumentName';
    PropertyTagImageDescription               : result := 'PropertyTagImageDescription';
    PropertyTagEquipMake                      : result := 'PropertyTagEquipMake';
    PropertyTagEquipModel                     : result := 'PropertyTagEquipModel';
    PropertyTagStripOffsets                   : result := 'PropertyTagStripOffsets';
    PropertyTagOrientation                    : result := 'PropertyTagOrientation';
    PropertyTagSamplesPerPixel                : result := 'PropertyTagSamplesPerPixel';
    PropertyTagRowsPerStrip                   : result := 'PropertyTagRowsPerStrip';
    PropertyTagStripBytesCount                : result := 'PropertyTagStripBytesCount';
    PropertyTagMinSampleValue                 : result := 'PropertyTagMinSampleValue';
    PropertyTagMaxSampleValue                 : result := 'PropertyTagMaxSampleValue';
    PropertyTagXResolution                    : result := 'PropertyTagXResolution';
    PropertyTagYResolution                    : result := 'PropertyTagYResolution';
    PropertyTagPlanarConfig                   : result := 'PropertyTagPlanarConfig';
    PropertyTagPageName                       : result := 'PropertyTagPageName';
    PropertyTagXPosition                      : result := 'PropertyTagXPosition';
    PropertyTagYPosition                      : result := 'PropertyTagYPosition';
    PropertyTagFreeOffset                     : result := 'PropertyTagFreeOffset';
    PropertyTagFreeByteCounts                 : result := 'PropertyTagFreeByteCounts';
    PropertyTagGrayResponseUnit               : result := 'PropertyTagGrayResponseUnit';
    PropertyTagGrayResponseCurve              : result := 'PropertyTagGrayResponseCurve';
    PropertyTagT4Option                       : result := 'PropertyTagT4Option';
    PropertyTagT6Option                       : result := 'PropertyTagT6Option';
    PropertyTagResolutionUnit                 : result := 'PropertyTagResolutionUnit';
    PropertyTagPageNumber                     : result := 'PropertyTagPageNumber';
    PropertyTagTransferFuncition              : result := 'PropertyTagTransferFuncition';
    PropertyTagSoftwareUsed                   : result := 'PropertyTagSoftwareUsed';
    PropertyTagDateTime                       : result := 'PropertyTagDateTime';
    PropertyTagArtist                         : result := 'PropertyTagArtist';
    PropertyTagHostComputer                   : result := 'PropertyTagHostComputer';
    PropertyTagPredictor                      : result := 'PropertyTagPredictor';
    PropertyTagWhitePoint                     : result := 'PropertyTagWhitePoint';
    PropertyTagPrimaryChromaticities          : result := 'PropertyTagPrimaryChromaticities';
    PropertyTagColorMap                       : result := 'PropertyTagColorMap';
    PropertyTagHalftoneHints                  : result := 'PropertyTagHalftoneHints';
    PropertyTagTileWidth                      : result := 'PropertyTagTileWidth';
    PropertyTagTileLength                     : result := 'PropertyTagTileLength';
    PropertyTagTileOffset                     : result := 'PropertyTagTileOffset';
    PropertyTagTileByteCounts                 : result := 'PropertyTagTileByteCounts';
    PropertyTagInkSet                         : result := 'PropertyTagInkSet';
    PropertyTagInkNames                       : result := 'PropertyTagInkNames';
    PropertyTagNumberOfInks                   : result := 'PropertyTagNumberOfInks';
    PropertyTagDotRange                       : result := 'PropertyTagDotRange';
    PropertyTagTargetPrinter                  : result := 'PropertyTagTargetPrinter';
    PropertyTagExtraSamples                   : result := 'PropertyTagExtraSamples';
    PropertyTagSampleFormat                   : result := 'PropertyTagSampleFormat';
    PropertyTagSMinSampleValue                : result := 'PropertyTagSMinSampleValue';
    PropertyTagSMaxSampleValue                : result := 'PropertyTagSMaxSampleValue';
    PropertyTagTransferRange                  : result := 'PropertyTagTransferRange';
    PropertyTagJPEGProc                       : result := 'PropertyTagJPEGProc';
    PropertyTagJPEGInterFormat                : result := 'PropertyTagJPEGInterFormat';
    PropertyTagJPEGInterLength                : result := 'PropertyTagJPEGInterLength';
    PropertyTagJPEGRestartInterval            : result := 'PropertyTagJPEGRestartInterval';
    PropertyTagJPEGLosslessPredictors         : result := 'PropertyTagJPEGLosslessPredictors';
    PropertyTagJPEGPointTransforms            : result := 'PropertyTagJPEGPointTransforms';
    PropertyTagJPEGQTables                    : result := 'PropertyTagJPEGQTables';
    PropertyTagJPEGDCTables                   : result := 'PropertyTagJPEGDCTables';
    PropertyTagJPEGACTables                   : result := 'PropertyTagJPEGACTables';
    PropertyTagYCbCrCoefficients              : result := 'PropertyTagYCbCrCoefficients';
    PropertyTagYCbCrSubsampling               : result := 'PropertyTagYCbCrSubsampling';
    PropertyTagYCbCrPositioning               : result := 'PropertyTagYCbCrPositioning';
    PropertyTagREFBlackWhite                  : result := 'PropertyTagREFBlackWhite';
    PropertyTagICCProfile                     : result := 'PropertyTagICCProfile';
    PropertyTagGamma                          : result := 'PropertyTagGamma';
    PropertyTagICCProfileDescriptor           : result := 'PropertyTagICCProfileDescriptor';
    PropertyTagSRGBRenderingIntent            : result := 'PropertyTagSRGBRenderingIntent';
    PropertyTagImageTitle                     : result := 'PropertyTagImageTitle';
    PropertyTagCopyright                      : result := 'PropertyTagCopyright';
    PropertyTagResolutionXUnit                : result := 'PropertyTagResolutionXUnit';
    PropertyTagResolutionYUnit                : result := 'PropertyTagResolutionYUnit';
    PropertyTagResolutionXLengthUnit          : result := 'PropertyTagResolutionXLengthUnit';
    PropertyTagResolutionYLengthUnit          : result := 'PropertyTagResolutionYLengthUnit';
    PropertyTagPrintFlags                     : result := 'PropertyTagPrintFlags';
    PropertyTagPrintFlagsVersion              : result := 'PropertyTagPrintFlagsVersion';
    PropertyTagPrintFlagsCrop                 : result := 'PropertyTagPrintFlagsCrop';
    PropertyTagPrintFlagsBleedWidth           : result := 'PropertyTagPrintFlagsBleedWidth';
    PropertyTagPrintFlagsBleedWidthScale      : result := 'PropertyTagPrintFlagsBleedWidthScale';
    PropertyTagHalftoneLPI                    : result := 'PropertyTagHalftoneLPI';
    PropertyTagHalftoneLPIUnit                : result := 'PropertyTagHalftoneLPIUnit';
    PropertyTagHalftoneDegree                 : result := 'PropertyTagHalftoneDegree';
    PropertyTagHalftoneShape                  : result := 'PropertyTagHalftoneShape';
    PropertyTagHalftoneMisc                   : result := 'PropertyTagHalftoneMisc';
    PropertyTagHalftoneScreen                 : result := 'PropertyTagHalftoneScreen';
    PropertyTagJPEGQuality                    : result := 'PropertyTagJPEGQuality';
    PropertyTagGridSize                       : result := 'PropertyTagGridSize';
    PropertyTagThumbnailFormat                : result := 'PropertyTagThumbnailFormat';
    PropertyTagThumbnailWidth                 : result := 'PropertyTagThumbnailWidth';
    PropertyTagThumbnailHeight                : result := 'PropertyTagThumbnailHeight';
    PropertyTagThumbnailColorDepth            : result := 'PropertyTagThumbnailColorDepth';
    PropertyTagThumbnailPlanes                : result := 'PropertyTagThumbnailPlanes';
    PropertyTagThumbnailRawBytes              : result := 'PropertyTagThumbnailRawBytes';
    PropertyTagThumbnailSize                  : result := 'PropertyTagThumbnailSize';
    PropertyTagThumbnailCompressedSize        : result := 'PropertyTagThumbnailCompressedSize';
    PropertyTagColorTransferFunction          : result := 'PropertyTagColorTransferFunction';
    PropertyTagThumbnailData                  : result := 'PropertyTagThumbnailData';
    PropertyTagThumbnailImageWidth            : result := 'PropertyTagThumbnailImageWidth';
    PropertyTagThumbnailImageHeight           : result := 'PropertyTagThumbnailImageHeight';
    PropertyTagThumbnailBitsPerSample         : result := 'PropertyTagThumbnailBitsPerSample';
    PropertyTagThumbnailCompression           : result := 'PropertyTagThumbnailCompression';
    PropertyTagThumbnailPhotometricInterp     : result := 'PropertyTagThumbnailPhotometricInterp';
    PropertyTagThumbnailImageDescription      : result := 'PropertyTagThumbnailImageDescription';
    PropertyTagThumbnailEquipMake             : result := 'PropertyTagThumbnailEquipMake';
    PropertyTagThumbnailEquipModel            : result := 'PropertyTagThumbnailEquipModel';
    PropertyTagThumbnailStripOffsets          : result := 'PropertyTagThumbnailStripOffsets';
    PropertyTagThumbnailOrientation           : result := 'PropertyTagThumbnailOrientation';
    PropertyTagThumbnailSamplesPerPixel       : result := 'PropertyTagThumbnailSamplesPerPixel';
    PropertyTagThumbnailRowsPerStrip          : result := 'PropertyTagThumbnailRowsPerStrip';
    PropertyTagThumbnailStripBytesCount       : result := 'PropertyTagThumbnailStripBytesCount';
    PropertyTagThumbnailResolutionX           : result := 'PropertyTagThumbnailResolutionX';
    PropertyTagThumbnailResolutionY           : result := 'PropertyTagThumbnailResolutionY';
    PropertyTagThumbnailPlanarConfig          : result := 'PropertyTagThumbnailPlanarConfig';
    PropertyTagThumbnailResolutionUnit        : result := 'PropertyTagThumbnailResolutionUnit';
    PropertyTagThumbnailTransferFunction      : result := 'PropertyTagThumbnailTransferFunction';
    PropertyTagThumbnailSoftwareUsed          : result := 'PropertyTagThumbnailSoftwareUsed';
    PropertyTagThumbnailDateTime              : result := 'PropertyTagThumbnailDateTime';
    PropertyTagThumbnailArtist                : result := 'PropertyTagThumbnailArtist';
    PropertyTagThumbnailWhitePoint            : result := 'PropertyTagThumbnailWhitePoint';
    PropertyTagThumbnailPrimaryChromaticities : result := 'PropertyTagThumbnailPrimaryChromaticities';
    PropertyTagThumbnailYCbCrCoefficients     : result := 'PropertyTagThumbnailYCbCrCoefficients';
    PropertyTagThumbnailYCbCrSubsampling      : result := 'PropertyTagThumbnailYCbCrSubsampling';
    PropertyTagThumbnailYCbCrPositioning      : result := 'PropertyTagThumbnailYCbCrPositioning';
    PropertyTagThumbnailRefBlackWhite         : result := 'PropertyTagThumbnailRefBlackWhite';
    PropertyTagThumbnailCopyRight             : result := 'PropertyTagThumbnailCopyRight';
    PropertyTagLuminanceTable                 : result := 'PropertyTagLuminanceTable';
    PropertyTagChrominanceTable               : result := 'PropertyTagChrominanceTable';
    PropertyTagFrameDelay                     : result := 'PropertyTagFrameDelay';
    PropertyTagLoopCount                      : result := 'PropertyTagLoopCount';
    PropertyTagPixelUnit                      : result := 'PropertyTagPixelUnit';
    PropertyTagPixelPerUnitX                  : result := 'PropertyTagPixelPerUnitX';
    PropertyTagPixelPerUnitY                  : result := 'PropertyTagPixelPerUnitY';
    PropertyTagPaletteHistogram               : result := 'PropertyTagPaletteHistogram';
    PropertyTagExifExposureTime               : result := 'PropertyTagExifExposureTime';
    PropertyTagExifFNumber                    : result := 'PropertyTagExifFNumber';
    PropertyTagExifExposureProg               : result := 'PropertyTagExifExposureProg';
    PropertyTagExifSpectralSense              : result := 'PropertyTagExifSpectralSense';
    PropertyTagExifISOSpeed                   : result := 'PropertyTagExifISOSpeed';
    PropertyTagExifOECF                       : result := 'PropertyTagExifOECF';
    PropertyTagExifVer                        : result := 'PropertyTagExifVer';
    PropertyTagExifDTOrig                     : result := 'PropertyTagExifDTOrig';
    PropertyTagExifDTDigitized                : result := 'PropertyTagExifDTDigitized';
    PropertyTagExifCompConfig                 : result := 'PropertyTagExifCompConfig';
    PropertyTagExifCompBPP                    : result := 'PropertyTagExifCompBPP';
    PropertyTagExifShutterSpeed               : result := 'PropertyTagExifShutterSpeed';
    PropertyTagExifAperture                   : result := 'PropertyTagExifAperture';
    PropertyTagExifBrightness                 : result := 'PropertyTagExifBrightness';
    PropertyTagExifExposureBias               : result := 'PropertyTagExifExposureBias';
    PropertyTagExifMaxAperture                : result := 'PropertyTagExifMaxAperture';
    PropertyTagExifSubjectDist                : result := 'PropertyTagExifSubjectDist';
    PropertyTagExifMeteringMode               : result := 'PropertyTagExifMeteringMode';
    PropertyTagExifLightSource                : result := 'PropertyTagExifLightSource';
    PropertyTagExifFlash                      : result := 'PropertyTagExifFlash';
    PropertyTagExifFocalLength                : result := 'PropertyTagExifFocalLength';
    PropertyTagExifMakerNote                  : result := 'PropertyTagExifMakerNote';
    PropertyTagExifUserComment                : result := 'PropertyTagExifUserComment';
    PropertyTagExifDTSubsec                   : result := 'PropertyTagExifDTSubsec';
    PropertyTagExifDTOrigSS                   : result := 'PropertyTagExifDTOrigSS';
    PropertyTagExifDTDigSS                    : result := 'PropertyTagExifDTDigSS';
    PropertyTagExifFPXVer                     : result := 'PropertyTagExifFPXVer';
    PropertyTagExifColorSpace                 : result := 'PropertyTagExifColorSpace';
    PropertyTagExifPixXDim                    : result := 'PropertyTagExifPixXDim';
    PropertyTagExifPixYDim                    : result := 'PropertyTagExifPixYDim';
    PropertyTagExifRelatedWav                 : result := 'PropertyTagExifRelatedWav';
    PropertyTagExifInterop                    : result := 'PropertyTagExifInterop';
    PropertyTagExifFlashEnergy                : result := 'PropertyTagExifFlashEnergy';
    PropertyTagExifSpatialFR                  : result := 'PropertyTagExifSpatialFR';
    PropertyTagExifFocalXRes                  : result := 'PropertyTagExifFocalXRes';
    PropertyTagExifFocalYRes                  : result := 'PropertyTagExifFocalYRes';
    PropertyTagExifFocalResUnit               : result := 'PropertyTagExifFocalResUnit';
    PropertyTagExifSubjectLoc                 : result := 'PropertyTagExifSubjectLoc';
    PropertyTagExifExposureIndex              : result := 'PropertyTagExifExposureIndex';
    PropertyTagExifSensingMethod              : result := 'PropertyTagExifSensingMethod';
    PropertyTagExifFileSource                 : result := 'PropertyTagExifFileSource';
    PropertyTagExifSceneType                  : result := 'PropertyTagExifSceneType';
    PropertyTagExifCfaPattern                 : result := 'PropertyTagExifCfaPattern';
    PropertyTagGpsVer                         : result := 'PropertyTagGpsVer';
    PropertyTagGpsLatitudeRef                 : result := 'PropertyTagGpsLatitudeRef';
    PropertyTagGpsLatitude                    : result := 'PropertyTagGpsLatitude';
    PropertyTagGpsLongitudeRef                : result := 'PropertyTagGpsLongitudeRef';
    PropertyTagGpsLongitude                   : result := 'PropertyTagGpsLongitude';
    PropertyTagGpsAltitudeRef                 : result := 'PropertyTagGpsAltitudeRef';
    PropertyTagGpsAltitude                    : result := 'PropertyTagGpsAltitude';
    PropertyTagGpsGpsTime                     : result := 'PropertyTagGpsGpsTime';
    PropertyTagGpsGpsSatellites               : result := 'PropertyTagGpsGpsSatellites';
    PropertyTagGpsGpsStatus                   : result := 'PropertyTagGpsGpsStatus';
    PropertyTagGpsGpsMeasureMode              : result := 'PropertyTagGpsGpsMeasureMode';
    PropertyTagGpsGpsDop                      : result := 'PropertyTagGpsGpsDop';
    PropertyTagGpsSpeedRef                    : result := 'PropertyTagGpsSpeedRef';
    PropertyTagGpsSpeed                       : result := 'PropertyTagGpsSpeed';
    PropertyTagGpsTrackRef                    : result := 'PropertyTagGpsTrackRef';
    PropertyTagGpsTrack                       : result := 'PropertyTagGpsTrack';
    PropertyTagGpsImgDirRef                   : result := 'PropertyTagGpsImgDirRef';
    PropertyTagGpsImgDir                      : result := 'PropertyTagGpsImgDir';
    PropertyTagGpsMapDatum                    : result := 'PropertyTagGpsMapDatum';
    PropertyTagGpsDestLatRef                  : result := 'PropertyTagGpsDestLatRef';
    PropertyTagGpsDestLat                     : result := 'PropertyTagGpsDestLat';
    PropertyTagGpsDestLongRef                 : result := 'PropertyTagGpsDestLongRef';
    PropertyTagGpsDestLong                    : result := 'PropertyTagGpsDestLong';
    PropertyTagGpsDestBearRef                 : result := 'PropertyTagGpsDestBearRef';
    PropertyTagGpsDestBear                    : result := 'PropertyTagGpsDestBear';
    PropertyTagGpsDestDistRef                 : result := 'PropertyTagGpsDestDistRef';
    PropertyTagGpsDestDist                    : result := 'PropertyTagGpsDestDist';
  else
    result := '<UnKnown>';
  end;
end;

function GetEncoderClsid(format: String; out pClsid: TGUID): integer;
var
  num, size, j: UINT;
  ImageCodecInfo: PImageCodecInfo;
Type
  ArrIMgInf = array of TImageCodecInfo;
begin
  num  := 0; // number of image encoders
  size := 0; // size of the image encoder array in bytes
  result := -1;

  GetImageEncodersSize(num, size);
  if (size = 0) then exit;

  GetMem(ImageCodecInfo, size);
  if(ImageCodecInfo = nil) then exit;

  GetImageEncoders(num, size, ImageCodecInfo);

  for j := 0 to num - 1 do
  begin
    if( ArrIMgInf(ImageCodecInfo)[j].MimeType = format) then
    begin
      pClsid := ArrIMgInf(ImageCodecInfo)[j].Clsid;
      result := j;  // Success
    end;
  end;
  FreeMem(ImageCodecInfo, size);
end;

function GetStatus(Stat: TStatus): string;
begin
  case Stat of
    Ok                        : result := 'Ok';
    GenericError              : result := 'GenericError';
    InvalidParameter          : result := 'InvalidParameter';
    OutOfMemory               : result := 'OutOfMemory';
    ObjectBusy                : result := 'ObjectBusy';
    InsufficientBuffer        : result := 'InsufficientBuffer';
    NotImplemented            : result := 'NotImplemented';
    Win32Error                : result := 'Win32Error';
    WrongState                : result := 'WrongState';
    Aborted                   : result := 'Aborted';
    FileNotFound              : result := 'FileNotFound';
    ValueOverflow             : result := 'ValueOverflow';
    AccessDenied              : result := 'AccessDenied';
    UnknownImageFormat        : result := 'UnknownImageFormat';
    FontFamilyNotFound        : result := 'FontFamilyNotFound';
    FontStyleNotFound         : result := 'FontStyleNotFound';
    NotTrueTypeFont           : result := 'NotTrueTypeFont';
    UnsupportedGdiplusVersion : result := 'UnsupportedGdiplusVersion';
    GdiplusNotInitialized     : result := 'GdiplusNotInitialized';
    PropertyNotFound          : result := 'PropertyNotFound';
    PropertyNotSupported      : result := 'PropertyNotSupported';
  else
    result := '<UnKnown>';
  end;
end;

function PixelFormatString(PixelFormat: TPixelFormat): string;
begin
  case PixelFormat of
    PixelFormatIndexed        : result := 'PixelFormatIndexed';
    PixelFormatGDI            : result := 'PixelFormatGDI';
    PixelFormatAlpha          : result := 'PixelFormatAlpha';
    PixelFormatPAlpha         : result := 'PixelFormatPAlpha';
    PixelFormatExtended       : result := 'PixelFormatExtended';
    PixelFormatCanonical      : result := 'PixelFormatCanonical';
    PixelFormatUndefined      : result := 'PixelFormatUndefined';
    PixelFormat1bppIndexed    : result := 'PixelFormat1bppIndexed';
    PixelFormat4bppIndexed    : result := 'PixelFormat4bppIndexed';
    PixelFormat8bppIndexed    : result := 'PixelFormat8bppIndexed';
    PixelFormat16bppGrayScale : result := 'PixelFormat16bppGrayScale';
    PixelFormat16bppRGB555    : result := 'PixelFormat16bppRGB555';
    PixelFormat16bppRGB565    : result := 'PixelFormat16bppRGB565';
    PixelFormat16bppARGB1555  : result := 'PixelFormat16bppARGB1555';
    PixelFormat24bppRGB       : result := 'PixelFormat24bppRGB';
    PixelFormat32bppRGB       : result := 'PixelFormat32bppRGB';
    PixelFormat32bppARGB      : result := 'PixelFormat32bppARGB';
    PixelFormat32bppPARGB     : result := 'PixelFormat32bppPARGB';
    PixelFormat48bppRGB       : result := 'PixelFormat48bppRGB';
    PixelFormat64bppARGB      : result := 'PixelFormat64bppARGB';
    PixelFormat64bppPARGB     : result := 'PixelFormat64bppPARGB';
    PixelFormatMax            : result := 'PixelFormatMax';
  else
    result := '<UnKnown>';
  end;
end;

function MakeLangID(PrimaryLanguage, SubLanguage: LANGID): Word;
begin
  result := (SubLanguage shl 10) or PrimaryLanguage;
end;


function ExtractIconEx(lpszFile: PChar; nIconIndex: Integer;  phIconLarge, phIconSmall: PHICON; nIcons: UINT): UINT; stdcall; external shell32;


function GetFileIcon(const FileName: string; IconIndex: Integer): THandle;
var
  IconHandle: HICON;
  IconCount: UINT;
begin
  IconHandle := 0;
  IconCount := ExtractIconEx(PChar(FileName), IconIndex, @IconHandle, nil, 1);
  if (IconCount > 0) and (IconHandle > 0) then
    Result := IconHandle
  else
    Result := 0;
end;

procedure AddIconFileToImageList(const FileName: string; IconIndex: Integer; const ImageList: TImageList);
var
  TempIcon: TIcon;
begin
  TempIcon := TIcon.Create;
  try
    TempIcon.Width  := ImageList.Width;
    TempIcon.Height := ImageList.Height;
    TempIcon.Handle := GetFileIcon(FileName, IconIndex);
    if (TempIcon.Handle > 0) then
    begin
      ImageList.AddIcon(TempIcon);
      DestroyIcon(TempIcon.Handle);
    end;
  finally
    FreeAndNil(TempIcon);
  end;
end;

function AddIconResourceToImageList(const ResourceName: string; const ImageList: TImageList): integer;
var
  TempIcon: TIcon;
begin
  Result := -1;

  TempIcon := TIcon.Create;
  try
    TempIcon.Width  := ImageList.Width;
    TempIcon.Height := ImageList.Height;
    TempIcon.Handle := LoadIcon(HInstance, PChar(ResourceName));
    if (TempIcon.Handle > 0) then
    begin
      Result := ImageList.AddIcon(TempIcon);
      DestroyIcon(TempIcon.Handle);
    end;
  finally
    FreeAndNil(TempIcon);
  end;
end;

procedure AddPngFileToImageList(const FileName: string; const ImageList: TImageList; smoothingMode: TSmoothingMode = SmoothingModeAntiAlias);
var
  Image : TGPImage;
  W : integer;
  H : integer;
  Bmp : TGPBitmap;
  Graphics : TGPGraphics;
  Icon : HICON;
begin
  Image := TGPImage.Create(FileName);
  W := ImageList.Width;
  H := ImageList.Height;

  Bmp := TGPBitmap.Create(W, H, PixelFormat32bppPARGB );

  Graphics := TGPGraphics.Create(Bmp);
  Graphics.SetSmoothingMode(SmoothingMode);
  Graphics.DrawImage(Image, 0, 0, W, H);
  Graphics.Free;
  Image.Free;

  bmp.GetHICON(Icon);

  ImageList_AddIcon(ImageList.Handle, Icon);
  bmp.Free;
  DestroyIcon(Icon);
end;

procedure DrawRoundRectangle(graphic: TGPGraphics; R: TRect; Radius: Integer; Clr: TColor);
var
  path: TGPGraphicsPath;
  l, t, w, h, d: Integer;
  gppen: TGPPen;
begin
  if not Assigned(graphic) then
    Exit;
  path := TGPGraphicsPath.Create;
  l := R.Left;
  t := R.Top;
  w := R.Right;
  h := R.Bottom;
  d := Radius shl 1;
  path.AddArc(l, t, d, d, 180, 90); // topleft
  path.AddLine(l + radius, t, l + w - radius, t); // top
  path.AddArc(l + w - d, t, d, d, 270, 90); // topright
  path.AddLine(l + w, t + radius, l + w, t + h - radius); // right
  path.AddArc(l + w - d, t + h - d, d, d, 0, 90); // bottomright
  path.AddLine(l + w - radius, t + h, l + radius, t + h); // bottom
  path.AddArc(l, t + h - d, d, d, 90, 90); // bottomleft
  path.AddLine(l, t + h - radius, l, t + radius); // left
  path.CloseFigure();
  gppen := TGPPen.Create(ColorToARGB(Clr));
  graphic.DrawPath(gppen, path);
  gppen.Free;
  path.Free;
end;





function GDIPlusBitampIntoLayeredWindow(frm:Tform; bmp: TGPBitmap):Integer;
var
  winLong:Cardinal;
  ABlendFunction:Blendfunction;
  AScreenDC,ASourceDC:HDC;
  ASourcePosition,FPosition:TPoint;
  FSize:TSize;
  ABufferBitmap,AOldBitmapSelectedInCanvasDC :HBitmap;
  FColorKey:TColor;
  FFlags:DWORD;
  res:Integer;
begin
  (*
  Conditions:
  pre: bmp is not nil, frm is not nil.
  post: TRUE has been returned iff the bitmap has been set to the
  window, otherwise, FALSE has been returned.
  *)
  if (frm = nil)or(bmp = nil) then
  begin
    GDIPlusBitampIntoLayeredWindow := 0;
  end
  else
  begin
    (* ensure we are dealing with a layered window by setting
    the window exstyle to WS_EX_LAYERED *)
    winLong:=GetWindowLong(frm.Handle,GWL_EXSTYLE);
    winLong:= winLong or WS_EX_LAYERED;
    SetWindowLong(frm.Handle,GWL_EXSTYLE,winLong);

    {Added by RIMMER
    ULW consumes only 32bit bitmaps with PREMULTYPLIED alpha.
    GDI+ can do it automatically through the following code}
    If bmp.GetPixelFormat <> PixelFormat32bppPARGB then begin
    bmp:=bmp.Clone(0,0,bmp.GetWidth,bmp.GetHeight,PixelFormat32bppPARGB);
    end;
    {end of Added by RIMMER}

    (* create a device context compatible with the screen *)
    AScreenDC := GetDC(0);
    ASourceDC := CreateCompatibleDC(AScreenDC);

    (* get a HBITMAP from the GDI+ bitmap *)
    bmp.GetHBITMAP(0, ABufferBitmap);

    (* select it into the device context we have just created *)
    (* we don't forget to save the old bitmap that was selected in the dc, *)
    (* otherwise, we will have a GDI mem-leak *)
    AOldBitmapSelectedInCanvasDC := SelectObject(ASourceDC, ABufferBitmap);

    (* set up source position *)
    ASourcePosition.x := 0;
    ASourcePosition.y := 0;

    (* set up a blendfunction structure to pass to ULW *)
    ABlendFunction.BlendOp := AC_SRC_OVER;
    ABlendFunction.BlendFlags := 0;
    ABlendFunction.SourceConstantAlpha := 255;
    ABlendFunction.AlphaFormat := AC_SRC_ALPHA;

    (* set up x,y pos and size *)
    FPosition.X := frm.Left;
    FPosition.Y := frm.Top;
    {Corrected by RIMMER
    TSize doesn't have Width and Height members, but cx and cy instead}
    FSize.cx :=bmp.GetWidth();
    FSize.cy := bmp.GetHeight();
    {end of Corrected by RIMMER}

    (* not used colorkey, because we use the hbitmaps native *)
    (* per-pixel alpha level *)
    FColorKey := RGB(255,0,255);
    FFlags := ULW_ALPHA;

    (* call the ULW api that copies or hbitmap derived from the *)
    (* GDI+ bitmap to the screen content *)
    {Corrected by RIMMER
    UpdateLayeredWindow is declared to return BOOL in Delphi,
    so we return -1 if it returns True (ie OK), ... }
    if UpdateLayeredWindow(frm.Handle, AScreenDC,@FPosition,
    @FSize,
    ASourceDC,
    @ASourcePosition,
    FColorKey,
    @ABlendFunction,
    FFlags
    )
    then res :=-1
    {... and the code of last error if it fails.}
    else res:=GetLastError;
    {end of Corrected by RIMMER}

    (* clean up used GDI objects *)
    SelectObject(ASourceDC, AOldBitmapSelectedInCanvasDC);
    DeleteObject(ABufferBitmap);
    DeleteDC(ASourceDC);
    ReleaseDC(0, AScreenDC);

    GDIPlusBitampIntoLayeredWindow := res;

  end;
end;




procedure ConvertTo32BitImageList(const ImageList: TCustomImageList);
const
  Mask: array[Boolean] of Longint = (0, ILC_MASK);
var
  TempList: TImageList;
begin
  if Assigned(ImageList) then
  begin
    TempList := TImageList.Create(nil);
    try
      TempList.Assign(ImageList);
      with ImageList do
      begin
        Handle := ImageList_Create(
          Width, Height, ILC_COLOR32 or Mask[Masked], 0, AllocBy);

        if not HandleAllocated then
          raise EInvalidOperation.Create(SInvalidImageList);
      end;

      Imagelist.AddImages(TempList);
    finally
      FreeAndNil(TempList);
    end;
  end;
end;


function  GetGPImageFromMemoryStream(Stream : TMemoryStream) : TGPImage;
var
  FImgStream : IStream;
begin
  FImgStream := TGPStreamAdapter.Create(Stream);
  Result := TGPImage.Create(FImgStream);
  FImgStream := nil;
end;



function DateTimeToFileTime(DateTime: TDateTime): TFileTime;
const
  FileTimeBase      = -109205.0;
  FileTimeStep: Extended = 24.0 * 60.0 * 60.0 * 1000.0 * 1000.0 * 10.0; // 100 nSek per Day
var
  E: Extended;
  F64: Int64;
begin
  E := (DateTime - FileTimeBase) * FileTimeStep;
  F64 := Round(E);
  Result := TFileTime(F64);
end;

{ TGPStreamAdapter }

function TGPStreamAdapter.Stat(out statstg: TStatStg;
  grfStatFlag: Integer): HResult;
begin
Result := S_OK;
  try
    if (@statstg <> nil) then
      with statstg do
      begin
        FillChar(statstg, sizeof(statstg), 0);
        dwType := STGTY_STREAM;
        cbSize := Stream.size;
        mTime := DateTimeToFileTime(now);
        cTime := DateTimeToFileTime(now);
        aTime := DateTimeToFileTime(now);
        grfLocksSupported := LOCK_WRITE;
      end;
  except
    Result := E_UNEXPECTED;
  end;
end;

function  GetGPImageFromStream(Stream : TStream) : TGPBitmap;
var
 FImgStream : IStream;
begin
  FImgStream := TGPStreamAdapter.Create(Stream);
  Result := TGPBitmap.Create(FImgStream);
  FImgStream := nil;
end;


function  GetHIconFromBitmap(Image : TGPBitmap; Size : integer = 32) : HICON;
var
 W : integer;
 Graphics : TGPGraphics;
 Bitmap : TGPBitmap;
begin
  W := Image.GetWidth;
  if W = Size then
   begin
     Image.GetHICON(Result);
     Exit;
   end;

  Bitmap := TGPBitmap.Create(Size, Size, PixelFormat32bppPARGB);
  Graphics := TGPGraphics.Create(Bitmap);
  Graphics.SetSmoothingMode(SmoothingModeAntiAlias);
  Graphics.DrawImage(Image, 0, 0, Size, Size);
  Graphics.Free;
  Bitmap.GetHICON(Result);
  Bitmap.Free;
end;

// procedure ImageListDrawDisabled was copied from RX library VCLUtils unit
//
procedure ImageListDrawDisabled(Images: TCustomImageList; Canvas: TCanvas;
  X, Y, Index: Integer; HighlightColor, GrayColor: TColor; DrawHighlight: Boolean);
const
 ROP_DSPDxax = $00E20746;
  
var
  Bmp: TBitmap;
  SaveColor: TColor;
begin
  SaveColor := Canvas.Brush.Color;
  Bmp := TBitmap.Create;
  try
    Bmp.Width := Images.Width;
    Bmp.Height := Images.Height;
    with Bmp.Canvas do begin
      Brush.Color := clWhite;
      FillRect(Rect(0, 0, Images.Width, Images.Height));
      ImageList_Draw(Images.Handle, Index, Handle, 0, 0, ILD_MASK);
    end;
    Bmp.Monochrome := True;
    if DrawHighlight then begin
      Canvas.Brush.Color := HighlightColor;
      SetTextColor(Canvas.Handle, clWhite);
      SetBkColor(Canvas.Handle, clBlack);
      BitBlt(Canvas.Handle, X + 1, Y + 1, Images.Width,
        Images.Height, Bmp.Canvas.Handle, 0, 0, ROP_DSPDxax);
    end;
    Canvas.Brush.Color := GrayColor;
    SetTextColor(Canvas.Handle, clWhite);
    SetBkColor(Canvas.Handle, clBlack);
    BitBlt(Canvas.Handle, X, Y, Images.Width,
      Images.Height, Bmp.Canvas.Handle, 0, 0, ROP_DSPDxax);
  finally
    Bmp.Free;
    Canvas.Brush.Color := SaveColor;
  end;
end;

function GrayColor(ACanvas: TCanvas; clr: TColor; Value: integer): TColor;
var
  r, g, b, avg: integer;
  clrRef : COLORREF;
begin
  if Value > 100 then
    Value := 100;
  clrRef := ColorToRGB(clr);

  r := ClrRef and $000000FF;
  g := (ClrRef and $0000FF00) shr 8;
  b := (ClrRef and $00FF0000) shr 16;

  Avg := (r + g + b) div 3;
  Avg := Avg + Value;

  if Avg > 240 then Avg := 240;

  Result := Windows.GetNearestColor (ACanvas.Handle,RGB(Avg, avg, avg));
end;


procedure GrayBitmapTransparent(ABitmap: TBitmap; Value: integer; TransparentColor: TColor);
var
  x, y: integer;
  LastColor1, LastColor2, Color: TColor;
begin
  LastColor1 := 0;
  LastColor2 := 0;

  for y := 0 to ABitmap.Height do
    for x := 0 to ABitmap.Width do
    begin
      Color := ABitmap.Canvas.Pixels[x, y];
      if Color = LastColor1 then
        ABitmap.Canvas.Pixels[x, y] := LastColor2
      else
      begin
        if Color = TransparentColor then LastColor2 := Color else
        LastColor2 := GrayColor(ABitmap.Canvas , Color, Value);
        ABitmap.Canvas.Pixels[x, y] := LastColor2;
        LastColor1 := Color;
      end;
    end;
end;

procedure GrayBitmap(ABitmap: TBitmap; Value: integer);
begin
  GrayBitmapTransparent( ABitmap, Value, clNone);
end;


procedure DrawTransparentBitmap(DC: HDC; hBmp : HBITMAP ;
          xStart: integer; yStart : integer; cTransparentColor : COLORREF);
var
      bm:                                                  BITMAP;
      cColor:                                              COLORREF;
      bmAndBack, bmAndObject, bmAndMem, bmSave:            HBITMAP;
      bmBackOld, bmObjectOld, bmMemOld, bmSaveOld:         HBITMAP;
      hdcMem, hdcBack, hdcObject, hdcTemp, hdcSave:        HDC;
      ptSize:                                              TPOINT;

begin
   hdcTemp := CreateCompatibleDC(dc);
   SelectObject(hdcTemp, hBmp);   // Select the bitmap

   GetObject(hBmp, sizeof(BITMAP), @bm);
   ptSize.x := bm.bmWidth;            // Get width of bitmap
   ptSize.y := bm.bmHeight;           // Get height of bitmap
   DPtoLP(hdcTemp, ptSize, 1);      // Convert from device
                                     // to logical points

   // Create some DCs to hold temporary data.
   hdcBack   := CreateCompatibleDC(dc);
   hdcObject := CreateCompatibleDC(dc);
   hdcMem    := CreateCompatibleDC(dc);
   hdcSave   := CreateCompatibleDC(dc);

   // Create a bitmap for each DC. DCs are required for a number of
   // GDI functions.

   // Monochrome DC
   bmAndBack   := CreateBitmap(ptSize.x, ptSize.y, 1, 1, nil);

   // Monochrome DC
   bmAndObject := CreateBitmap(ptSize.x, ptSize.y, 1, 1, nil);

   bmAndMem    := CreateCompatibleBitmap(dc, ptSize.x, ptSize.y);
   bmSave      := CreateCompatibleBitmap(dc, ptSize.x, ptSize.y);

   // Each DC must select a bitmap object to store pixel data.
   bmBackOld   := SelectObject(hdcBack, bmAndBack);
   bmObjectOld := SelectObject(hdcObject, bmAndObject);
   bmMemOld    := SelectObject(hdcMem, bmAndMem);
   bmSaveOld   := SelectObject(hdcSave, bmSave);

   // Set proper mapping mode.
   SetMapMode(hdcTemp, GetMapMode(dc));

   // Save the bitmap sent here, because it will be overwritten.
   BitBlt(hdcSave, 0, 0, ptSize.x, ptSize.y, hdcTemp, 0, 0, SRCCOPY);

   // Set the background color of the source DC to the color.
   // contained in the parts of the bitmap that should be transparent
   cColor := SetBkColor(hdcTemp, cTransparentColor);

   // Create the object mask for the bitmap by performing a BitBlt
   // from the source bitmap to a monochrome bitmap.
   BitBlt(hdcObject, 0, 0, ptSize.x, ptSize.y, hdcTemp, 0, 0,
          SRCCOPY);

   // Set the background color of the source DC back to the original
   // color.
   SetBkColor(hdcTemp, cColor);

   // Create the inverse of the object mask.
   BitBlt(hdcBack, 0, 0, ptSize.x, ptSize.y, hdcObject, 0, 0,
          NOTSRCCOPY);

   // Copy the background of the main DC to the destination.
   BitBlt(hdcMem, 0, 0, ptSize.x, ptSize.y, dc, xStart, yStart,
          SRCCOPY);

   // Mask out the places where the bitmap will be placed.
   BitBlt(hdcMem, 0, 0, ptSize.x, ptSize.y, hdcObject, 0, 0, SRCAND);

   // Mask out the transparent colored pixels on the bitmap.
   BitBlt(hdcTemp, 0, 0, ptSize.x, ptSize.y, hdcBack, 0, 0, SRCAND);

   // XOR the bitmap with the background on the destination DC.
   BitBlt(hdcMem, 0, 0, ptSize.x, ptSize.y, hdcTemp, 0, 0, SRCPAINT);

   // Copy the destination to the screen.
   BitBlt(dc, xStart, yStart, ptSize.x, ptSize.y, hdcMem, 0, 0,
          SRCCOPY);

   // Place the original bitmap back into the bitmap sent here.
   BitBlt(hdcTemp, 0, 0, ptSize.x, ptSize.y, hdcSave, 0, 0, SRCCOPY);

   // Delete the memory bitmaps.
   DeleteObject(SelectObject(hdcBack, bmBackOld));
   DeleteObject(SelectObject(hdcObject, bmObjectOld));
   DeleteObject(SelectObject(hdcMem, bmMemOld));
   DeleteObject(SelectObject(hdcSave, bmSaveOld));

   // Delete the memory DCs.
   DeleteDC(hdcMem);
   DeleteDC(hdcBack);
   DeleteDC(hdcObject);
   DeleteDC(hdcSave);
   DeleteDC(hdcTemp);
end;


function GetRealColor(AColor : TColor) : TColor;
var
  DC : HDC;
  C : COLORREF;
begin
  DC := GetDC(0);
  C := ColorToRGB(AColor);
  Result := GetNearestColor(DC, C);
  ReleaseDC(0, DC);
end;


function NewColor(DC : HDC; clr: TColor; Value: integer; AllColors : boolean = true): TColor;
var
  r, g, b: integer;
begin
  if Value > 100 then Value := 100;
  clr := ColorToRGB(clr);

  r := Clr and $000000FF;
  g := (Clr and $0000FF00) shr 8;
  b := (Clr and $00FF0000) shr 16;


  if AllColors or (r > 0) then
   begin
     r := r + Round((255 - r) * (value / 100));
     if r < 0 then r := 0;
   end;

  if AllColors or (g > 0) then
   begin
     g := g + Round((255 - g) * (value / 100));
     if g < 0 then g := 0;
   end;

  if AllColors or (b > 0) then
   begin
     b := b + Round((255 - b) * (value / 100));
     if b < 0 then b := 0;
   end;

  Result := Windows.GetNearestColor(DC, RGB(r, g, b));

end;

function NewColors(DC : HDC; clr: TColor; RedPart, GreenPart, BluePart: integer): TColor;
var
  r, g, b: integer;
begin
  if RedPart > 100 then RedPart := 100;
  if GreenPart > 100 then GreenPart := 100;
  if BluePart > 100 then BluePart := 100;

  clr := ColorToRGB(clr);
  r := Clr and $000000FF;
  g := (Clr and $0000FF00) shr 8;
  b := (Clr and $00FF0000) shr 16;


  r := r + Round((255 - r) * (RedPart / 100));
  if r < 0 then r := 0;
  g := g + Round((255 - g) * (GreenPart / 100));
  if g < 0 then g := 0;
  b := b + Round((255 - b) * (BluePart / 100));
  if b < 0 then b := 0;

  Result := Windows.GetNearestColor(DC, RGB(r, g, b));
end;


function GetHighlightColor(BaseColor: TColor): TColor;
begin
  Result := RGB(Min(GetRValue(ColorToRGB(BaseColor)) + 64, 255),
    Min(GetGValue(ColorToRGB(BaseColor)) + 64, 255),
    Min(GetBValue(ColorToRGB(BaseColor)) + 64, 255));
end;


function GetShadowColor(BaseColor: TColor): TColor;
begin
  Result := RGB(Max(GetRValue(ColorToRGB(BaseColor)) - 64, 0),
    Max(GetGValue(ColorToRGB(BaseColor)) - 64, 0),
    Max(GetBValue(ColorToRGB(BaseColor)) - 64, 0));
end;

function Darker(Color:TColor; Percent:Byte):TColor;
var
  r,g,b:Byte;
begin
  Color:=ColorToRGB(Color);
  r := GetRValue(Color);
  g := GetGValue(Color);
  b := GetBValue(Color);
  r := r-muldiv(r,Percent,100);  //Percent% closer to black
  g := g-muldiv(g,Percent,100);
  b := b-muldiv(b,Percent,100);
  Result:=RGB(r,g,b);
end;

function Lighter(Color:TColor; Percent:Byte):TColor;
var
  r,g,b:Byte;
begin
 Color:=ColorToRGB(Color);
 r:=GetRValue(Color);
 g:=GetGValue(Color);
 b:=GetBValue(Color);
 r:=r+muldiv(255-r,Percent,100); //Percent% closer to white
 g:=g+muldiv(255-g,Percent,100);
 b:=b+muldiv(255-b,Percent,100);
 result:=RGB(r,g,b);
end;


procedure DrawRealFrame(DC : HDC; Rect : TRect; AColor : TColor; Down : boolean);
var
 MidColor : COLORREF;
 DarkColor : COLORREF;
 LightColor : COLORREF;
 Pen : HPEN;
begin
  MidColor   := NewColor(DC, AColor, 50, true);
  if Down then
   begin
     LightColor  := GetNearestColor(DC, Darker(MidColor, 30));
     DarkColor   := GetNearestColor(DC, Lighter(MidColor, 30));
   end
     else
       begin
         DarkColor  := GetNearestColor(DC, Darker(MidColor, 30));
         LightColor := GetNearestColor(DC, Lighter(MidColor, 30));
       end;

  Pen := SelectObject(DC, CreatePen(PS_SOLID, 1, LightColor));
  MoveToEx(DC, Rect.Left, Rect.Bottom, nil);
  LineTo(DC, Rect.Left, Rect.Top);
  LineTo(DC, Rect.Right-1, Rect.Top);
  DeleteObject(SelectObject(DC, Pen));

  Pen := SelectObject(DC, CreatePen(PS_SOLID, 1, DarkColor));
  MoveToEx(DC, Rect.Right-1, Rect.Top, nil);
  LineTo(DC,   Rect.Right-1, Rect.Bottom-1);
  LineTo(DC,   Rect.Left ,  Rect.Bottom-1);
  DeleteObject(SelectObject(DC, Pen));



end;

end.
