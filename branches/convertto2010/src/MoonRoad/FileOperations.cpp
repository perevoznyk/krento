#include "stdafx.h"
#include "FileOperations.h"
#include <shlwapi.h>
#include <shlobj.h>
#include <shellapi.h>
#include <tchar.h>
#include <stdio.h>
#include <io.h>
#include <wtypes.h>
#include "MoonRoad.h"

DWORD WINAPI FileGetSize(LPCWSTR fileName)
{
	if (!fileName)
		return 0;

	TCHAR fileSpec[MAX_PATH];
	StripFileName(fileName, fileSpec);

	HANDLE fHandle = CreateFile(fileSpec, GENERIC_READ , FILE_SHARE_READ, NULL, OPEN_EXISTING,
		FILE_ATTRIBUTE_NORMAL | FILE_FLAG_SEQUENTIAL_SCAN, 0);
	if (fHandle == INVALID_HANDLE_VALUE)
		return 0;
	else
	{
		DWORD result = GetFileSize(fHandle, NULL);
		CloseHandle(fHandle);
		return result;
	}
}

HGLOBAL WINAPI FileReadToBuffer(LPCWSTR fileName)
{
	if (!fileName)
		return NULL;

	TCHAR fileSpec[MAX_PATH];
	StripFileName(fileName, fileSpec);

	HANDLE fHandle = CreateFile(fileSpec, GENERIC_READ , FILE_SHARE_READ, NULL, OPEN_EXISTING,
		FILE_ATTRIBUTE_NORMAL | FILE_FLAG_SEQUENTIAL_SCAN, 0);
	if (fHandle == INVALID_HANDLE_VALUE)
		return NULL;
	else
	{
		DWORD fSize = GetFileSize(fHandle, NULL);
		HGLOBAL ReadBuffer = GlobalAlloc(GMEM_FIXED, fSize);
		if (ReadBuffer != NULL)
		{
			DWORD dwBytesRead;
			ReadFile(fHandle, ReadBuffer, fSize, &dwBytesRead, NULL);
		}
		CloseHandle(fHandle);
		return ReadBuffer;
	}
}

void WINAPI FreeFileBuffer(HGLOBAL buffer)
{
	if (buffer != NULL)
		GlobalFree(buffer);
}

void WINAPI CreateUnicodeFile(LPCWSTR fileName)
{
	if (!fileName)
		return;

	HANDLE hFile;
	DWORD dwBytesWritten;
	TCHAR fileSpec[MAX_PATH];
	StripFileName(fileName, fileSpec);

	hFile = CreateFile(fileSpec, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, 0, NULL);
	if (hFile == INVALID_HANDLE_VALUE) 
		return;

	WORD wMarker = 0xFEFF;
	WriteFile(hFile, &wMarker, sizeof(wMarker), &dwBytesWritten, NULL);
	CloseHandle(hFile);
}

BOOL WINAPI IsUnicodeFile(LPCWSTR fileName)
{
	if (!fileName)
		return TRUE;

	HANDLE hFile;
	DWORD dwBytesRead;

	TCHAR fileSpec[MAX_PATH];
	StripFileName(fileName, fileSpec);

	hFile = CreateFile(fileSpec, // file to open
		GENERIC_READ,            // open for reading
		FILE_SHARE_READ,         // share for reading
		NULL,                    // default security
		OPEN_EXISTING,           // existing file only
		FILE_ATTRIBUTE_NORMAL,   // normal file
		NULL);                   // no attr. template

	if (hFile == INVALID_HANDLE_VALUE) 
	{ 
		return TRUE; // by default all files are unicode files
	}

	WORD wMarker;

	if( FALSE == ReadFile(hFile, &wMarker, sizeof(wMarker), &dwBytesRead, NULL) )
	{
		CloseHandle(hFile);
		return FALSE;
	}

	BOOL result = TRUE;

	if (dwBytesRead == 2)
	{
		result = (wMarker == 0xFEFF);
	}
	else
	{
		result = FALSE;
	}

	CloseHandle(hFile);
	return result;
}


void WINAPI ShellCopyFile(LPWSTR oldName, LPWSTR newName)
{
	if ((!oldName) || (!newName))
		return;

	SHFILEOPSTRUCT sfo;

	WCHAR wFrom[MAX_PATH];
	WCHAR wTo[MAX_PATH];
	ZeroMemory(wFrom, sizeof(wFrom));
	ZeroMemory(wTo, sizeof(wTo));

	size_t i;
	size_t len;

	len = wcslen(oldName);
	for (i = 0; i < len; i++)
	{
		wFrom[i] = oldName[i];
	}

	wFrom[i] = '\0';

	len = wcslen(newName);
	for (i = 0; i < len; i++)
	{
		wTo[i] = newName[i];
	}
	wTo[i] = '\0';

	sfo.hwnd = NULL;
	sfo.wFunc = FO_COPY;
	sfo.fFlags = FOF_ALLOWUNDO;
	sfo.pFrom = wFrom;
	sfo.pTo = wTo;

	SHFileOperation(&sfo);
}

void WINAPI DeleteToRecycleBin(LPWSTR fileName, BOOL silent)
{
	if (!fileName)
		return;

	SHFILEOPSTRUCT sfo;
	TCHAR fileSpec[MAX_PATH];
	StripFileName(fileName, fileSpec);

	sfo.hwnd = NULL;
	sfo.wFunc = FO_DELETE;
	if (silent)
		sfo.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION | FOF_NOERRORUI | FOF_SILENT;
	else
		sfo.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION | FOF_WANTNUKEWARNING;
	sfo.pFrom = fileSpec;
	sfo.pTo = NULL;

	SHFileOperation(&sfo);

}

BOOL WINAPI FileExists(LPWSTR fileName)
{
	if (fileName)
	{
		TCHAR  infoBuf[MAX_PATH];
		WCHAR wFileName[MAX_PATH];
		ZeroMemory(wFileName, sizeof(wFileName));

		size_t len = wcslen(fileName);
		size_t start = 0;
		if (fileName[len-1] == '"')
			len--;
		if (fileName[start] == '"')
			start ++;

		_tcsncpy(wFileName, fileName + start, len - start);
		if (!PathIsURL(fileName))
		{
			ExpandEnvironmentStrings(wFileName, infoBuf, MAX_PATH);
		}
		else
		{
			_tcscpy(infoBuf, wFileName);
		}

		return PathFileExists(infoBuf);
	}
	else
		return FALSE;
}

void WINAPI FileDelete(LPWSTR fileName)
{
	if (!fileName)
		return;

	TCHAR  infoBuf[MAX_PATH];
	WCHAR wFileName[MAX_PATH];
	ZeroMemory(wFileName, sizeof(wFileName));

	size_t len = wcslen(fileName);
	size_t start = 0;
	if (fileName[len-1] == '"')
		len--;
	if (fileName[start] == '"')
		start ++;

	_tcsncpy(wFileName, fileName + start, len - start);
	if (!PathIsURL(fileName))
	{
		ExpandEnvironmentStrings(wFileName, infoBuf, MAX_PATH);
	}
	else
	{
		_tcscpy(infoBuf, wFileName);
	}

	if (PathFileExists(infoBuf))
		DeleteFile(infoBuf);
}

BOOL WINAPI IsValidFileName(LPWSTR fileName)
{
	if (!fileName)
		return FALSE;

	WCHAR InvalidFileNameChars[] =  { 
		'"', '<', '>', '|', '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n', '\v', 
		'\f', '\r', '\x000e', '\x000f', '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001a', '\x001b', 
		'\x001c', '\x001d', '\x001e', '\x001f', ':', '*', '?', '\\', '/'
	};

	int ifn = sizeof(InvalidFileNameChars) / sizeof(WCHAR);
	for (UINT i = 0; i < wcslen(fileName); i++)
	{
		for (int j = 0; j < ifn; j++)
		{
			if (fileName[i] == InvalidFileNameChars[j])
				return FALSE;
		}
	}
	return TRUE;
}


BOOL WINAPI IsValidPathName(LPWSTR fileName)
{
	if (!fileName)
		return FALSE;

	WCHAR InvalidPathChars[] = { 
		'"', '<', '>', '|', '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n', '\v', 
		'\f', '\r', '\x000e', '\x000f', '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001a', '\x001b', 
		'\x001c', '\x001d', '\x001e', '\x001f'
	};

	int ifn = sizeof(InvalidPathChars) / sizeof(WCHAR);
	for (int i = 0; i < lstrlen(fileName); i++)
	{
		for (int j = 0; j < ifn; j++)
		{
			if (fileName[i] == InvalidPathChars[j])
				return FALSE;
		}
	}
	return TRUE;
}

BOOL WINAPI FileExtensionIs(LPCWSTR fileName, LPCWSTR ext)
{
	if ((!fileName) || (!ext))
		return FALSE;

	WCHAR fileExt[MAX_PATH] = {0};
	fileExt[0] = '.';
	int idx = 1;
	if (ext[0] == '.')
		idx = 0;

	_tcscpy(fileExt + idx, ext);

	return SameText(PathFindExtension(fileName), fileExt);
}

BOOL WINAPI FileIsImage(LPCWSTR fileName)
{
	if (!fileName)
		return FALSE;
	
	if (PathIsDirectory(fileName))
		return FALSE;

	if (!FileExists((LPWSTR)fileName))
		return FALSE;

	LPWSTR ext = PathFindExtension(fileName);
	if (wcslen(ext) == 0)
		return FALSE;

	_wcslwr(ext);

	LPCWSTR result = wcsstr(L".png .jpg .jpeg .gif .bmp .tiff .tif .ico", ext);
	return (result != NULL);
}

BOOL WINAPI FileIsKrentoPackage(LPCWSTR fileName)
{
	if (!fileName)
		return FALSE;

	if (PathIsDirectory(fileName))
		return false;

	if (!FileExists((LPWSTR)fileName))
		return FALSE;

	LPWSTR ext = PathFindExtension(fileName);
	if (wcslen(ext) == 0)
		return false;
	_wcslwr(ext);
	return (wcsstr(L".kmenu .kskin .stone .lng .kadd .toy .circle .docklet", ext) != NULL);
}

STDAPI LoadFromFile(const CLSID *pclsid, LPCTSTR pszFile, REFIID riid, void **ppv)
{
    *ppv = NULL;

    IPersistFile *ppf;
    HRESULT hr = SHCoCreateInstance(NULL, pclsid, NULL, IID_IPersistFile, (void **)&ppf);
    if (SUCCEEDED(hr))
    {
        WCHAR wszPath[MAX_PATH];
        SHTCharToUnicode(pszFile, wszPath, ARRAYSIZE(wszPath));

        hr = ppf->Load(wszPath, STGM_READ);
        if (SUCCEEDED(hr))
            hr = ppf->QueryInterface(riid, ppv);
        ppf->Release();
    }
    return hr;
}

BOOL WINAPI FileIsLink(LPCWSTR fileName)
{
	if (!fileName)
		return FALSE;

	if (PathIsDirectory(fileName))
		return FALSE;

	if (!FileExists((LPWSTR)fileName))
		return FALSE;

	if (FileExtensionIs(fileName, L".lnk"))
	{
		TCHAR fileSpec[MAX_PATH];
		StripFileName(fileName, fileSpec);

		IShellLink* psl;
		HRESULT hr = LoadFromFile(&CLSID_ShellLink, fileSpec, IID_PPV_ARG(IShellLink, &psl));
		if (SUCCEEDED(hr)) 
		{ 
			psl->Release();
			return TRUE;
		}
		else
			return FALSE;
	}
	else
		return FALSE;
}

BOOL WINAPI GetFileNameFromPidl(LPCITEMIDLIST pidlRelative, LPWSTR lpszPath, int iPathBufferSize)
{
	HRESULT hr;
	BOOL result = FALSE;
	IShellFolder *psfDesktop = NULL;
	STRRET strDispName;
	TCHAR szDisplayName[MAX_PATH];

	hr = SHGetDesktopFolder(&psfDesktop);
	if (SUCCEEDED(hr))
	{
		hr = psfDesktop->GetDisplayNameOf(pidlRelative, SHGDN_INFOLDER, &strDispName);
		if (SUCCEEDED(hr))
		{
			hr = StrRetToBuf(&strDispName, pidlRelative, szDisplayName, sizeof(szDisplayName));
			if (SUCCEEDED(hr))
			{
				_tcsncpy(lpszPath, szDisplayName, iPathBufferSize);
				result = true;
			}
		}

		psfDesktop->Release();
	}
	return result;
}

BOOL WINAPI ResolveShellLink(LPWSTR lpszLinkFile, LPWSTR lpszPath, int iPathBufferSize) 
{ 
	HRESULT hres;  
	IShellLink* psl; 
	WCHAR szGotPath[MAX_PATH]; 
	WIN32_FIND_DATA wfd; 
	BOOL result = FALSE;
	*lpszPath = 0; // assume failure 
	TCHAR fileSpec[MAX_PATH];

	if (!lpszLinkFile)
		return FALSE;

	if (PathIsDirectory(lpszLinkFile))
		return FALSE;

	if (!FileExists((LPWSTR)lpszLinkFile))
		return FALSE;

	if (!FileExtensionIs(lpszLinkFile, L".lnk"))
		return FALSE;

	StripFileName(lpszLinkFile, fileSpec);

	// Get a pointer to the IShellLink interface. 
	hres = CoCreateInstance(CLSID_ShellLink, NULL, CLSCTX_INPROC_SERVER, 
		IID_IShellLink, (LPVOID*)&psl); 
	if (SUCCEEDED(hres)) 
	{ 
		IPersistFile* ppf; 

		// Get a pointer to the IPersistFile interface. 
		hres = psl->QueryInterface(IID_IPersistFile, (void**)&ppf); 

		if (SUCCEEDED(hres)) 
		{ 
			// Load the shortcut. 
			hres = ppf->Load(fileSpec, STGM_READ); 

			if (SUCCEEDED(hres)) 
			{ 
				// Resolve the link. 
				hres = psl->Resolve(0, SLR_NO_UI | SLR_NOSEARCH); 

				if (SUCCEEDED(hres)) 
				{ 
					// Get the path to the link target. 
					hres = psl->GetPath(szGotPath, 
						MAX_PATH, 
						(WIN32_FIND_DATA*)&wfd, 
						SLGP_SHORTPATH); 

					if (SUCCEEDED(hres)) 
					{ 
						_tcsncpy(lpszPath,  
							szGotPath, iPathBufferSize);
						// Handle success
						result = TRUE;
					}
						else
							// application-defined function
							result = FALSE;
					
				} 
			} 

			// Release the pointer to the IPersistFile interface. 
			ppf->Release(); 
		} 

		// Release the pointer to the IShellLink interface. 
		psl->Release(); 
	} 
	return result;
}


void WINAPI CreateShellLink(PCIDLIST_ABSOLUTE lpszPathObj, LPWSTR lpszPathLink) 
{ 
    HRESULT hres; 
    IShellLink* psl; 
 
	if (!lpszPathLink)
		return;

	TCHAR fileSpec[MAX_PATH];
	StripFileName(lpszPathLink, fileSpec);

    // Get a pointer to the IShellLink interface. 
    hres = CoCreateInstance(CLSID_ShellLink, NULL, CLSCTX_INPROC_SERVER, 
                            IID_IShellLink, (LPVOID*)&psl); 
    if (SUCCEEDED(hres)) 
    { 
        IPersistFile* ppf; 
 
        // Set the path to the shortcut target. 
		psl->SetIDList(lpszPathObj); 
 
        // Query IShellLink for the IPersistFile interface for saving the 
        // shortcut in persistent storage. 
        hres = psl->QueryInterface(IID_IPersistFile, (LPVOID*)&ppf); 
 
        if (SUCCEEDED(hres)) 
        { 
            // Save the link by calling IPersistFile::Save. 
            hres = ppf->Save(fileSpec, TRUE); 
            ppf->Release(); 
        } 
        psl->Release(); 
    } 
}


void WINAPI ClearFileAttributes(LPWSTR fileName)
{
	if (!fileName)
		return;

	TCHAR fileSpec[MAX_PATH];
	StripFileName(fileName, fileSpec);

	if (FileExists(fileSpec))
	{
		SetFileAttributes(fileSpec, FILE_ATTRIBUTE_NORMAL);
	}
}

HANDLE WINAPI FileCreateRewrite(LPCWSTR fileName)
{
	if (!fileName)
		return NULL;

	TCHAR fileSpec[MAX_PATH];
	StripFileName(fileName, fileSpec);

	HANDLE hFile;
	DWORD dwBytesWritten;

	hFile = CreateFile(fileSpec, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, 0, NULL);
	if (hFile == INVALID_HANDLE_VALUE) 
		return NULL;

	WORD wMarker = 0xFEFF;
	WriteFile(hFile, &wMarker, sizeof(wMarker), &dwBytesWritten, NULL);
	return hFile;
}

void WINAPI FileWrite(HANDLE handle, LPWSTR text)
{
	if (!text)
		return;

	DWORD dwBytesWritten;
	WriteFile(handle, text, (DWORD)wcslen(text) * sizeof(WCHAR), &dwBytesWritten, NULL);
}

void WINAPI FileWriteNewLine(HANDLE handle)
{
	DWORD dwBytesWritten;
	WCHAR buff[3] = L"\r\n";
	WriteFile(handle, buff, (DWORD)wcslen(buff) * sizeof(WCHAR), &dwBytesWritten, NULL);
}

void WINAPI FileWriteChar(HANDLE handle, WCHAR text)
{
	DWORD dwBytesWritten;
	WriteFile(handle, &text, sizeof(text), &dwBytesWritten, NULL);
}

void WINAPI FileClose(HANDLE handle)
{
	CloseHandle(handle);
}


BOOL WINAPI IsAnimatedGIF(LPWSTR fileName)
{
	if (!fileName)
		return FALSE;

	if (!FileExists(fileName))
		return FALSE;

	/* GIF89a (GIF image signature) */
	const char GIF_HEADER[] = { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 };
	const int   GIF_HEADER_LEN = sizeof (GIF_HEADER);

	/* NETSCAPE2.0 (extension describing animated GIF, starts on 0x310) */
	const char GIF_APPEXT[] = { 0x4E, 0x45, 0x54, 0x53, 0x43, 0x41,
		0x50, 0x45, 0x32, 0x2E, 0x30 };
	const int   GIF_APPEXT_LEN = sizeof (GIF_APPEXT);

	unsigned char buf[16];
	char c;
	DWORD dwBytes;
	BOOL result = false;

	TCHAR fileSpec[MAX_PATH];
	StripFileName(fileName, fileSpec);

	if (!FileExtensionIs(fileSpec, L".gif"))
		return FALSE;

	HANDLE fHandle = CreateFile(fileSpec, GENERIC_READ , FILE_SHARE_READ, NULL, OPEN_EXISTING,
		FILE_ATTRIBUTE_NORMAL | FILE_FLAG_SEQUENTIAL_SCAN, 0);
	if (fHandle == INVALID_HANDLE_VALUE)
		return FALSE;


    if (!ReadFile(fHandle, buf, GIF_HEADER_LEN, &dwBytes, NULL))
    {
        goto exitPoint;
    }

	if (GetFileSize(fHandle, NULL) < 0x331)
    {
        goto exitPoint;
    }

	if (memcmp(buf, GIF_HEADER, GIF_HEADER_LEN) != 0)
	{
        goto exitPoint;
	}

	for (;; ) // our appetite now knows no bounds save termination or error
	{
		if (!ReadFile(fHandle, &c, 1, &dwBytes, NULL))
		{
			goto exitPoint;
		}

		if (dwBytes == 0)
			break;

        if (c == ';')
        {                       /* GIF terminator */
            break;
        }

		if (c == '!')
		{                       /* Extension */
			if (!ReadFile(fHandle, &c, 1, &dwBytes, NULL))
			{
				goto exitPoint;
			}
			if (dwBytes == 0)
				break;

			if ((byte)c == 0xff)
			{
				if (!ReadFile(fHandle, &c, 1, &dwBytes, NULL)) //skip block size
				{
					goto exitPoint;
				}

				if (dwBytes == 0)
					break;

				if (!ReadFile(fHandle, buf, GIF_APPEXT_LEN, &dwBytes, NULL))
				{
					goto exitPoint;
				}

				if (memcmp (buf, GIF_APPEXT, GIF_APPEXT_LEN) == 0) 
				{
					result = TRUE;
					goto exitPoint;
				}
			}
		}
	}


exitPoint:
	CloseHandle(fHandle);
	return result;
}


void WINAPI StripFileName(LPCWSTR fileName, LPWSTR fullName)
{
	if ((!fileName) || (!fullName))
		return;

	WCHAR wFileName[MAX_PATH];
	ZeroMemory(wFileName, sizeof(wFileName));

	size_t len = wcslen(fileName);
	size_t start = 0;
	if (fileName[len-1] == '"')
		len--;
	if (fileName[start] == '"')
		start ++;

	_tcsncpy(wFileName, fileName + start, len - start);
	if (!PathIsURL(fileName))
	{
		ExpandEnvironmentStrings(wFileName, fullName, MAX_PATH);
	}
	else
	{
		_tcscpy(fullName, wFileName);
	}
}

BOOL WINAPI FileRename(LPWSTR oldName, LPWSTR newName)
{
	if ((!oldName) || (!newName))
		return FALSE;

	TCHAR  oldBuf[MAX_PATH];
	TCHAR  newBuf[MAX_PATH];

	StripFileName(oldName, oldBuf);
	StripFileName(newName, newBuf);
	return MoveFile(oldBuf, newBuf);
}


BOOL WINAPI FileCopy(LPWSTR oldName, LPWSTR newName)
{
	if ((!oldName) || (!newName))
		return FALSE;

	TCHAR  oldBuf[MAX_PATH];
	TCHAR  newBuf[MAX_PATH];

	StripFileName(oldName, oldBuf);
	StripFileName(newName, newBuf);
	return CopyFile(oldBuf, newBuf, FALSE);
}

BOOL WINAPI FullPath(LPWSTR fileName, LPWSTR fullName)
{
	if ((!fileName) || (!fullName))
		return FALSE;

	TCHAR fileSpec[MAX_PATH];
	StripFileName(fileName, fileSpec);
	BOOL result = PathFindOnPath(fileSpec, NULL);
	memcpy(fullName, fileSpec, MAX_PATH);
	return result;
}

BOOL WINAPI FileOrFolderExists(LPWSTR fileName)
{
	if (!fileName)
		return FALSE;

	TCHAR fileSpec[MAX_PATH];
	StripFileName(fileName, fileSpec);
	return (_waccess( fileSpec, 0 ) != -1); 
}

BOOL WINAPI DirectoryExists(LPWSTR fileName)
{
	if (!fileName)
		return FALSE;

	TCHAR fileSpec[MAX_PATH];
	DWORD dwFileAttributes;

	StripFileName(fileName, fileSpec);
	dwFileAttributes = GetFileAttributes( fileSpec );
	if (dwFileAttributes == INVALID_FILE_ATTRIBUTES)
		return FALSE;
	if( !( FILE_ATTRIBUTE_DIRECTORY & dwFileAttributes ) )
		return FALSE;
	else
		return TRUE;
}

BOOL WINAPI IsDirectory(LPWSTR folderName)
{
	if (!folderName)
		return FALSE;

	return PathIsDirectory(folderName);
}

BOOL WINAPI FileIsIcon(LPWSTR fileName)
{
	if (!fileName)
		return FALSE;

	if (PathIsDirectory(fileName))
		return FALSE;

	if (!FileExists(fileName))
		return FALSE;

	if (FileExtensionIs(fileName, L".ico"))
	{
		if (FileExists(fileName))
			return TRUE;
		else
			return FALSE;
	}
	else
		return FALSE;
}

BOOL WINAPI FileIsExe(LPWSTR fileName)
{
	if (!fileName)
		return FALSE;

	if (PathIsDirectory(fileName))
		return FALSE;

	if (!FileExists(fileName))
		return FALSE;

	if (FileExtensionIs(fileName, L".exe"))
	{
		if (!FileExists(fileName))
			return FALSE;

		HANDLE            hFile;
		IMAGE_DOS_HEADER  doshdr;    // Original EXE Header
		DWORD             cbRead;

		// Open File
		hFile = CreateFile (fileName, GENERIC_READ, FILE_SHARE_READ, NULL,
			OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
		if (hFile == INVALID_HANDLE_VALUE)
			return FALSE;

		// Get Original Dos Header
		if ((! ReadFile (hFile, (LPVOID)&doshdr, sizeof(doshdr), &cbRead, NULL)) ||
			(cbRead != sizeof(doshdr)) ||             // Read Error
			(doshdr.e_magic != IMAGE_DOS_SIGNATURE))  // Invalid DOS Header
		{
			goto error;        /* Abort("Not an exe",h); */
		}
		CloseHandle (hFile);
		return TRUE;

error:
		CloseHandle (hFile);
		return FALSE;
	}
	else
		return FALSE;

}

int WINAPI GetFilesCount(LPWSTR folderName)
{
	if (!folderName)
		return 0;

	HANDLE hFind; // file handle
	WIN32_FIND_DATA FindFileData;
	TCHAR PathToSearchInto [MAX_PATH] = {0};
	int result = 0;

	// Construct the path to search into "C:\\Windows\\System32\\*"
	_tcscpy(PathToSearchInto, folderName);
	_tcscat(PathToSearchInto, _T("\\*"));
	
	hFind = FindFirstFile(PathToSearchInto,&FindFileData); // find the first file
	if(hFind == INVALID_HANDLE_VALUE)
	{
		return 0;
	}
	
	bool bSearch = true;
	while(bSearch) // until we finds an entry
	{
		if(FindNextFile(hFind,&FindFileData))
		{
			// Don't care about . and ..
			//if(IsDots(FindFileData.cFileName))
			if ((_tcscmp(FindFileData.cFileName, _T(".")) == 0) ||
				(_tcscmp(FindFileData.cFileName, _T("..")) == 0))
				continue;
			
			// We have found a hidden file
			if((FindFileData.dwFileAttributes & FILE_ATTRIBUTE_HIDDEN))
			{
				continue;
			}
			// We have found a file
			else
			{
				result++;
			}
			
		}//FindNextFile
		else
		{
			if(GetLastError() == ERROR_NO_MORE_FILES) // no more files there
				bSearch = false;
			else {
				// some error occured, close the handle and return result
				FindClose(hFind);
				return result;
			}
		}
	}//while
	
	FindClose(hFind); // closing file handle
	return result;

}

void WINAPI GetAllFiles(FOLDERENUMPROC lpEnumProc, LPWSTR folderName, LPWSTR searchMask, LPARAM lParam)
{
	HANDLE hFind; // file handle
	WIN32_FIND_DATA FindFileData;
	TCHAR PathToSearchInto [MAX_PATH] = {0};
	TCHAR FullName [MAX_PATH];
	int result = 0;

	_tcscpy(PathToSearchInto, folderName);
	_tcscat(PathToSearchInto, _T("\\*"));

	hFind = FindFirstFile(PathToSearchInto,&FindFileData); // find the first file
	if(hFind == INVALID_HANDLE_VALUE)
	{
		return;
	}

	do // until we finds an entry
	{
		// Don't care about . and ..
		//if(IsDots(FindFileData.cFileName))
		if ((_tcscmp(FindFileData.cFileName, _T(".")) == 0) ||
			(_tcscmp(FindFileData.cFileName, _T("..")) == 0))
			continue;

		// We have found a file
		else
		{
			_tcscpy(FullName, folderName);
			_tcscat(FullName, _T("\\"));
			_tcscat(FullName, FindFileData.cFileName);

			if (FindFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) 
			{
				GetAllFiles(lpEnumProc, FullName, searchMask, lParam);
			}
			else
			{
				if (PathMatchSpec(FindFileData.cFileName, searchMask))
				{
					if (lpEnumProc)
						lpEnumProc(FullName, lParam);
				}
			}

		}


	} while (FindNextFile(hFind,&FindFileData) != 0);
	FindClose(hFind); // closing file handle

}

void WINAPI GetFileDescription(LPWSTR fileName)
{
	PathStripPath(fileName);
	PathRemoveExtension(fileName);
}