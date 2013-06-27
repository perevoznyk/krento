unit SpecialFolders;

interface
uses
 Windows, SysUtils, SHFolder;

const
  ApplicationData = $1a;
  CommonApplicationData = $23;
  CommonProgramFiles = $2b;
  Cookies = $21;
  Desktop = 0;
  DesktopDirectory = $10;
  Favorites = 6;
  History = $22;
  InternetCache = $20;
  LocalApplicationData = $1c;
  MyComputer = $11;
  MyDocuments = 5;
  MyMusic = 13;
  MyPictures = $27;
  Personal = 5;
  ProgramFiles = $26;
  Programs = 2;
  Recent = 8;
  SendTo = 9;
  StartMenu = 11;
  Startup = 7;
  System = $25;
  Templates = $15;

var
  PortableVersion : boolean;

function GetFolderPath(folder : integer) : string;
function ConcatenatePath(rootPath : string; relPath : string) : string;
function MainFolder : string;
function DockletsFolder : string;
function GetRelativeFolder(AbsoluteFolder : string) : string;
function KrentoSettingsFileName : string;
function LanguagesFolder : string;

implementation

function GetFolderPath(folder : integer) : string;
var
  lpszPath : array[0..260] of char;
  path : string;
begin
   SHGetFolderPath(0, folder, 0, 0, lpszPath);
   path := string(lpszPath);
   result := path;
end;

function ConcatenatePath(rootPath : string; relPath : string) : string;
begin
   Result := IncludeTrailingPathDelimiter(rootPath) + relPath;
   try
    if not (DirectoryExists(Result)) then
      CreateDir(Result);
   except

   end;
end;

function MainFolder : string;
begin
  if PortableVersion then
    begin
      Result :=  ExcludeTrailingPathDelimiter(ExtractFilePath(ParamStr(0)));
    end
      else
        begin
          Result := ConcatenatePath(GetFolderPath(ApplicationData) , 'Krento');
        end;
end;

function KrentoSettingsFileName : string;
begin
  Result := IncludeTrailingPathDelimiter(MainFolder) + 'Krento.ini';
end;

function DockletsFolder : string;
begin
  Result := ConcatenatePath(MainFolder, 'Docklets');
end;

function LanguagesFolder : string;
begin
  Result := ConcatenatePath(MainFolder, 'Languages');
end;

function GetRelativeFolder(AbsoluteFolder : string) : string;
var
 RelativePath : string;
begin
  RelativePath := ExtractRelativePath(IncludeTrailingPathDelimiter(MainFolder), AbsoluteFolder);
  if RelativePath = '' then
     RelativePath := 'Docklets';

  if RelativePath[1] = '.' then
    begin
      Delete(RelativePath, 1, 1);
    end;

   if RelativePath[1] = '.' then
    begin
      Delete(RelativePath, 1, 1);
    end;

    if RelativePath[1] = '\' then
    begin
      Delete(RelativePath, 1, 1);
    end;

    RelativePath := IncludeTrailingPathDelimiter(RelativePath);

    Result := RelativePath;
end;

initialization
   PortableVersion := false;
end.
