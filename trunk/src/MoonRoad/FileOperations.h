#pragma once

#ifdef __cplusplus
extern "C" {
#endif

//
//  IID_PPV_ARG(IType, ppType) 
//      IType is the type of pType
//      ppType is the variable of type IType that will be filled
//
//      RESULTS in:  IID_IType, ppvType
//      will create a compiler error if wrong level of indirection is used.
//
//  macro for QueryInterface and related functions
//  that require a IID and a (void **)
//  this will insure that the cast is safe and appropriate on C++
//
#ifdef __cplusplus
#define IID_PPV_ARG(IType, ppType) IID_##IType, reinterpret_cast<void**>(static_cast<IType**>(ppType))
#else
#define IID_PPV_ARG(IType, ppType) &IID_##IType, (void**)(ppType))
#endif

typedef VOID (CALLBACK* FOLDERENUMPROC)(LPWSTR fileName, LPARAM lParam);

void WINAPI CreateUnicodeFile(LPCWSTR fileName);
BOOL WINAPI IsUnicodeFile(LPCWSTR fileName);
void WINAPI DeleteToRecycleBin(LPWSTR fileName, BOOL silent);
void WINAPI ShellCopyFile(LPWSTR oldName, LPWSTR newName);

HGLOBAL WINAPI FileReadToBuffer(LPCWSTR fileName);
void WINAPI FreeFileBuffer(HGLOBAL buffer);
DWORD WINAPI FileGetSize(LPCWSTR fileName);
BOOL WINAPI FileExists(LPWSTR fileName);
BOOL WINAPI FileExtensionIs(LPCWSTR fileName, LPCWSTR ext);
BOOL WINAPI FileIsImage(LPCWSTR fileName);
BOOL WINAPI FileIsLink(LPCWSTR fileName);
void WINAPI FileDelete(LPWSTR fileName);
void WINAPI ClearFileAttributes(LPWSTR fileName);
HANDLE WINAPI FileCreateRewrite(LPCWSTR fileName);
void WINAPI FileClose(HANDLE handle);
void WINAPI FileWrite(HANDLE handle, LPWSTR text);
void WINAPI FileWriteChar(HANDLE handle, WCHAR text);
void WINAPI FileWriteNewLine(HANDLE handle);
BOOL WINAPI IsAnimatedGIF(LPWSTR fileName);
BOOL WINAPI FileRename(LPWSTR oldName, LPWSTR newName);
BOOL WINAPI FileIsKrentoPackage(LPCWSTR fileName);
void WINAPI StripFileName(LPCWSTR fileName, LPWSTR fullName);
BOOL WINAPI FileCopy(LPWSTR oldName, LPWSTR newName);
BOOL WINAPI FullPath(LPWSTR fileName, LPWSTR fullName);
BOOL WINAPI FileOrFolderExists(LPWSTR fileName);
BOOL WINAPI DirectoryExists(LPWSTR fileName);
BOOL WINAPI IsDirectory(LPWSTR folderName);
BOOL WINAPI FileIsExe(LPWSTR fileName);
BOOL WINAPI FileIsIcon(LPWSTR fileName);
BOOL WINAPI IsValidFileName(LPWSTR fileName);
BOOL WINAPI IsValidPathName(LPWSTR fileName);
int WINAPI GetFilesCount(LPWSTR folderName);
BOOL WINAPI ResolveShellLink(LPWSTR lpszLinkFile, LPWSTR lpszPath, int iPathBufferSize); 
void WINAPI CreateShellLink(PCIDLIST_ABSOLUTE lpszPathObj, LPWSTR lpszPathLink);
BOOL WINAPI GetFileNameFromPidl(LPCITEMIDLIST pidlRelative, LPWSTR lpszPath, int iPathBufferSize);
void WINAPI GetAllFiles(FOLDERENUMPROC lpEnumProc, LPWSTR folderName, LPWSTR searchMask, LPARAM lParam);
void WINAPI GetFileDescription(LPWSTR fileName);


#ifdef __cplusplus
}
#endif
