unit NativeThemeManager;

interface

uses
  Windows, SysUtils, Classes, GDIPObj;

var
  ThemeLoaded : boolean;

type
   TNativeThemeManager = class sealed
   public
     class function LoadBitmap(resourceName : string) : TGPBitmap; static;
     class function Load(resourceName : string) : TGPImage; static;
   end;

implementation

var
  ThemeHandle : THandle;


{ TNativeThemeManager }

class function TNativeThemeManager.Load(resourceName: string): TGPImage;
begin
  if ThemeLoaded then
    begin
      Result := TGPImage(TGPBitmap.FromResource(ThemeHandle, resourceName));
    end
      else
       begin
         Result := nil;
       end;
end;

class function TNativeThemeManager.LoadBitmap(resourceName: string): TGPBitmap;
begin
  if ThemeLoaded then
    begin
      Result := TGPBitmap.FromResource(ThemeHandle, resourceName);
    end
      else
       begin
         Result := nil;
       end;
end;

initialization
  ThemeHandle := SafeLoadLibrary('Laugris.Standard.dll');
  ThemeLoaded := ThemeHandle <> 0;

finalization
 if ThemeLoaded then
 FreeLibrary(ThemeHandle);

end.
