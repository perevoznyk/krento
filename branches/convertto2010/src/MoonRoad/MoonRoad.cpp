// MoonRoad.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <tchar.h>
#include <stdio.h>
#include <wtypes.h>
#include <stdarg.h>
#include <varargs.h>
#include "PowrProf.h"
#include "MoonRoad.h"
#include "resource.h"
#include <shlwapi.h>
#include <shlobj.h>
#include <shellapi.h>
#include "CustomMessages.h"
#include "System.h"
#include "SystemInfo.h"
#include "XZip.h"
#include "XUnzip.h"
#include "FileOperations.h"

// Shared Data
#pragma data_seg(".shared")	// Make a new section that we'll make shared
HHOOK hMouseHook = NULL;	// HHOOK from SetWindowsHook
HWND hHookWindow = NULL;	// Handle to the window that hook the mouse
HWND mainWindow[255];
BOOL mouseDetected = FALSE;
RECT dockRect = {0};
BOOL bPause = FALSE;
BOOL bWheelTracking = FALSE;
BOOL bClickTracking = FALSE;
BOOL bMoveTracking = FALSE;
BOOL bClickTrackingWheel = FALSE;
BOOL bClickTrackingXButton = FALSE;
BOOL bInterceptClick = FALSE;
BOOL bPauseIntercept = FALSE;
BOOL bDesktopClick = FALSE;
#pragma data_seg()			// Back to regular, nonshared data
#pragma comment(linker, "/section:.shared,rws")


void WINAPI SetMouseDetected(BOOL value)
{
	mouseDetected = value;
}

BOOL WINAPI GetMouseDetected()
{
	return mouseDetected;
}

void WINAPI GetDockRectangle(LPRECT rect)
{
	*rect = dockRect;
}

void WINAPI SetDockRectangle(LPRECT rect)
{
	dockRect = *rect;
}

HWND WINAPI AllocateLaugrisWindow()
{
	return AllocateLayeredWindow(L"Laugris Window");
}

HWND WINAPI AllocateKarnaWindow()
{
	return AllocateLayeredWindow(L"Karna Window");
}

HWND WINAPI GetMainWindow(BYTE applicationNumber)
{
	return mainWindow[applicationNumber];
}

void WINAPI SetMainWindow(HWND window, BYTE applicationNumber)
{
	mainWindow[applicationNumber] = window;
}

BOOL WINAPI PostMainMessage(BYTE applicationNumber, UINT Msg, WPARAM wParam, LPARAM lParam)
{
	return PostMessage(mainWindow[applicationNumber], Msg, wParam, lParam);
}

BOOL WINAPI SendMainMessage(BYTE applicationNumber, UINT Msg, WPARAM wParam, LPARAM lParam)
{
	return SendMessage(mainWindow[applicationNumber], Msg, wParam, lParam);
}

BOOL WINAPI StartEngine(HWND handle, LPWSTR applicationName)
{
	if (ValidCopy(applicationName))
	{
		if (handle != 0)
		{
			if (SendMessageTimeout(handle, CM_STARTENGINE, 0, 0, SMTO_NORMAL, 5000, NULL) != 0)
				return true;
			else
				return false;
		}
		else
		{
			return false;
		}
	}
	else
	{
		return false;
	}
}



void WINAPI ExitKrento()
{
	ExitManagedApplication(0);
}

void WINAPI ExitManagedApplication(BYTE applicationNumber)
{
	if (mainWindow[applicationNumber] != NULL)
		PostMessage(mainWindow[applicationNumber], WM_CLOSE, 0, 0);
	else
		PostQuitMessage(0);
}

void WINAPI PauseMouseHook()
{
	bPause = TRUE;
}

void WINAPI ResumeMouseHook()
{
	bPause = FALSE;
}


// Installing mouse hook
BOOL WINAPI InstallMouseHook(HWND reportWindow, HINSTANCE AppHandle)
{
	HOOKPROC hkprcMouse;

	if ((reportWindow == NULL) || (!IsWindow(reportWindow))) 
	{
		return FALSE;	// Invalid handle to a window, exit.
	}

	hkprcMouse = (HOOKPROC)GetProcAddress(hDllInstance, "MouseHookProc");

	// Hook the mouse messages
	hMouseHook = SetWindowsHookEx (
		WH_MOUSE,				// hook type (mouse)
		hkprcMouse,				// hook procedure
		hDllInstance,           // handle to application instance
		0						// thread identifier
		);

	// Set the HookWindow if SetWindowsHookEx succeeded
	if (hMouseHook == NULL) 
	{
		hHookWindow = NULL;
		return FALSE;
	}

	// Everything went fine 
	hHookWindow = reportWindow;
	return TRUE;

}

void WINAPI RemoveMouseHook()
{
	if ( hMouseHook > 0 )
	{
		UnhookWindowsHookEx(hMouseHook);
		hMouseHook = NULL;
		hHookWindow = NULL;
	}
}

INT WINAPI InstallKeyboardHook(HWND reportWindow, UINT modifiers, UINT key)
{

	if ((reportWindow == NULL) || (!IsWindow(reportWindow))) 
	{
		return 0;	// Invalid handle to a window, exit.
	}

	//Disable sticky keys
	STICKYKEYS StickyKeys;
	StickyKeys.cbSize = sizeof(STICKYKEYS);
	StickyKeys.dwFlags = 0;
	SystemParametersInfo(SPI_SETSTICKYKEYS, sizeof(STICKYKEYS), &StickyKeys, 0);

	GUID gNewGuid;
	CoCreateGuid(&gNewGuid);
	WCHAR  wszCLSID[CLSIDSize];

	StringFromGUID2(gNewGuid, wszCLSID, ARRAYSIZE(wszCLSID)); 
	ATOM id = GlobalAddAtom(wszCLSID);

	if (IsWindows7())
		modifiers |= MOD_NOREPEAT;
	if (RegisterHotKey(reportWindow, id, modifiers, key))
		return id;
	else
		return 0;
}

void WINAPI RemoveKeyboardHook(HWND reportWindow, int id)
{
	if ((reportWindow == NULL) || (!IsWindow(reportWindow))) 
	{
		return;	// Invalid handle to a window, exit.
	}

	if (id == 0)
		return;

	UnregisterHotKey(reportWindow, id);
}


//internal callback function for mouse hook
LRESULT WINAPI MouseHookProc(int nCode, WPARAM wParam, LPARAM lParam) 
{
	LRESULT intCode = 0;
	LRESULT nextCode;

	if (bPause)
		return CallNextHookEx(hMouseHook, nCode, wParam, lParam);

	UINT Msg = (UINT)wParam;
	DWORD dwMousePos;
	UINT keyInfo;

	if (Msg == WM_NCHITTEST)
	{
		return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
	}

	//extra mouse structure
	MOUSEHOOKSTRUCTEX *pInfo = (MOUSEHOOKSTRUCTEX *) lParam;

	if (nCode < 0)					// Windows tell us not to handle this msg
		return CallNextHookEx(hMouseHook, nCode, wParam, lParam);

	if (hMouseHook == NULL) 		// We do not know our hook handle
		return 0;

	if (hHookWindow == NULL)				// Something wrong happened...
		UnhookWindowsHookEx(hMouseHook);	// We don't have any window target
	// So we stop the hook.

	if (hHookWindow == NULL)		// No window has requested a hook
		return CallNextHookEx(hMouseHook, nCode, wParam, lParam);

	if (pInfo->hwnd == hHookWindow) 
	{
		return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
	}

	//===================================
	// We will handle this mouse message
	//===================================

	if (nCode == HC_ACTION)
	{
		dwMousePos = MAKELONG(pInfo->pt.x, pInfo->pt.y);

		if (bDesktopClick)
		{
			if (Msg == WM_LBUTTONDBLCLK)
			{
				if (DesktopDblClick(pInfo->pt))
				{
					PostMessage(hHookWindow, CM_DESKTOPCLICK, 0, dwMousePos);
				}
			}
		}

		if (bClickTracking)
		{
			switch (Msg) {

			case WM_LBUTTONDOWN:
			case WM_NCLBUTTONDOWN:
				PostMessage(hHookWindow, CM_LBUTTONDOWN, 0, dwMousePos);
				break;

			case WM_MBUTTONDOWN:
			case WM_NCMBUTTONDOWN:
				if (bClickTrackingWheel)
				{

					PostMessage(hHookWindow, CM_MBUTTONDOWN, 0, dwMousePos);
					intCode = 1;
				}
				break;

			case WM_RBUTTONDOWN:
				if (bInterceptClick)
				{
					PostMessage(hHookWindow, CM_RBUTTONDOWN, 0, dwMousePos);
					if (!bPauseIntercept)
						intCode = 1;
				}
				break;

			case WM_NCRBUTTONDOWN:
				if (bInterceptClick)
				{
					PostMessage(hHookWindow, CM_RBUTTONDOWN, 0, dwMousePos);
					if  (!bPauseIntercept)
						intCode = 1;
				}
				break;

			case WM_XBUTTONDOWN:
			case WM_NCXBUTTONDOWN:
				if (bClickTrackingXButton)
				{
					PostMessage(hHookWindow, CM_XBUTTONDOWN, GET_XBUTTON_WPARAM (pInfo->mouseData), dwMousePos);
					intCode = 1;
				}
				break;

			case WM_XBUTTONUP:
			case WM_NCXBUTTONUP:
				if (bClickTrackingXButton)
				{
					keyInfo = 0;
					if (GetKeyState(VK_SHIFT) < 0)
						keyInfo |= CK_SHIFT;
					if (GetKeyState(VK_CONTROL) < 0)
						keyInfo |= CK_CONTROL;
					if (GetKeyState(VK_MENU) < 0)
						keyInfo |= CK_MENU;
					PostMessage(hHookWindow, CM_XBUTTONUP, GET_XBUTTON_WPARAM(pInfo->mouseData), keyInfo);
					intCode = 1;
				}
				break;

			case WM_XBUTTONDBLCLK:
			case WM_NCXBUTTONDBLCLK:
				if (bClickTrackingXButton)
				{
					PostMessage(hHookWindow, CM_XBUTTONDBLCLK, GET_XBUTTON_WPARAM(pInfo->mouseData), dwMousePos);
					intCode = 1;
				}
				break;

			case WM_LBUTTONUP:
			case WM_NCLBUTTONUP:
				PostMessage(hHookWindow, CM_LBUTTONUP, 0, dwMousePos);
				break;

			case WM_MBUTTONUP:
			case WM_NCMBUTTONUP:
				if (bClickTrackingWheel)
				{
					keyInfo = 0;
					if (GetKeyState(VK_SHIFT) < 0)
						keyInfo |= CK_SHIFT;
					if (GetKeyState(VK_CONTROL) < 0)
						keyInfo |= CK_CONTROL;
					if (GetKeyState(VK_MENU) < 0)
						keyInfo |= CK_MENU;
					PostMessage(hHookWindow, CM_MBUTTONUP, 0, keyInfo);
					intCode = 1;
				}
				break;

			case WM_RBUTTONUP:
				if (bInterceptClick)
				{
					PostMessage(hHookWindow, CM_RBUTTONUP, 0, dwMousePos);
					if (!bPauseIntercept)
						intCode = 1;
				}
				break;

			case WM_NCRBUTTONUP:
				if (bInterceptClick)
				{
					PostMessage(hHookWindow, CM_RBUTTONUP, 0, dwMousePos);
					if (!bPauseIntercept)
						intCode = 1;
				}
				break;

			case WM_LBUTTONDBLCLK:
			case WM_NCLBUTTONDBLCLK:
				PostMessage(hHookWindow, CM_LBUTTONDBLCLK, 0, dwMousePos);
				break;

			case WM_MBUTTONDBLCLK:
			case WM_NCMBUTTONDBLCLK:
				if (bClickTrackingWheel)
				{
					PostMessage(hHookWindow, CM_MBUTTONDBLCLK, 0, dwMousePos);
					intCode = 1;
				}
				break;

			case WM_RBUTTONDBLCLK:
			case WM_NCRBUTTONDBLCLK:
				PostMessage(hHookWindow, CM_RBUTTONDBLCLK, 0, dwMousePos);
				break;
			default:
				break;
			}
		}

		switch (Msg) {

			//Posting "move message"
		case WM_MOUSEMOVE:
		case WM_NCMOUSEMOVE:
			if (bMoveTracking)
			{
				PostMessage(hHookWindow, CM_MOUSEMOVE, (WPARAM)pInfo->hwnd, dwMousePos);
			}
			break;

		case WM_MOUSEWHEEL:
			if (bWheelTracking)
			{
				PostMessage(hHookWindow, CM_MOUSEWHEEL, GET_WHEEL_DELTA_WPARAM(pInfo->mouseData), dwMousePos);
			}
			break;

		default:
			break;
		}
	}


	// We don't care about this message so we pass it to other processes.
	nextCode = CallNextHookEx(hMouseHook, nCode, wParam, lParam);
	if (intCode == 0)
		intCode = nextCode;
	return intCode;
}


void WINAPI SetDesktopClick(BOOL value)
{
	bDesktopClick = value;
}

BOOL WINAPI GetDesktopClick()
{
	return bDesktopClick;
}

void WINAPI SetInterceptClick(BOOL value)
{
	bInterceptClick = value;
}

BOOL WINAPI GetInterceptClick()
{
	return bInterceptClick;
}

void WINAPI SetClickTrackingWheel(BOOL value)
{
	bClickTrackingWheel = value;
}

void WINAPI SetClickTrackingXButton(BOOL value)
{
	bClickTrackingXButton = value;
}

void WINAPI SetClickTracking(BOOL value)
{
	bClickTracking = value;
}

void WINAPI SetMoveTracking(BOOL value)
{
	bMoveTracking = value;
}

void WINAPI SetWheelTracking(BOOL value)
{
	bWheelTracking = value;
}

BOOL WINAPI GetClickTracking()
{
	return bClickTracking;
}

BOOL WINAPI GetWheelTracking()
{
	return bWheelTracking;
}

BOOL WINAPI GetMoveTracking()
{
	return bMoveTracking;
}

BOOL WINAPI GetClickTrackingWheel()
{
	return bClickTrackingWheel;
}

BOOL WINAPI GetClickTrackingXButton()
{
	return bClickTrackingXButton;
}

void WINAPI PauseIntercept()
{
	bPauseIntercept = TRUE;
}

void WINAPI ResumeIntercept()
{
	bPauseIntercept = FALSE;
}

BOOL WINAPI GetHookPaused()
{
	return bPause;
}

BOOL WINAPI SameText(LPCWSTR s1, LPCWSTR s2)
{
	return (_wcsicmp(s1, s2) == 0);
}

int CmpDate(const SYSTEMTIME *pst1, const SYSTEMTIME *pst2)
{
    int iRet;

    if (pst1->wYear < pst2->wYear)
        iRet = -1;
    else if (pst1->wYear > pst2->wYear)
        iRet = 1;
    else if (pst1->wMonth < pst2->wMonth)
        iRet = -1;
    else if (pst1->wMonth > pst2->wMonth)
        iRet = 1;
    else if (pst1->wDay < pst2->wDay)
        iRet = -1;
    else if (pst1->wDay > pst2->wDay)
        iRet = 1;
    else
        iRet = 0;

    return(iRet);
}

INT WINAPI WMessageBox(
	HWND hwnd,							// Parent window for message display
	UINT uiStyle,						// Style of message box
	WCHAR* pwszTitle,					// Title for message
	WCHAR* pwszFmt,						// Format string
	...									// Substitution parameters
	)
{
	va_list		marker;
	WCHAR		wszBuffer[4096];

	// Use format and arguements as input
	//This version will not overwrite the stack, since it only copies
	//upto the max size of the array
	va_start(marker, pwszFmt);
	_vsnwprintf_s(wszBuffer, 4096, _TRUNCATE, pwszFmt, marker);
	va_end(marker);
   
	//Make sure there is a NULL Terminator, vsnwprintf will not copy
	//the terminator if length==MAX_QUERY_LEN
	wszBuffer[4095] = wEOL;

	//Unicode version is supported on both Win95/WinNT
	return MessageBoxW(hwnd, wszBuffer, pwszTitle, uiStyle);
}

BOOL ValidCopy(LPWSTR applicationName)
{
	return TRUE;

	//SYSTEMTIME lt;
	//SYSTEMTIME check = {0}; 
	//GetLocalTime(&lt);

	//check.wYear = 2012;
	//check.wMonth = 11;
	//check.wDay = 01;

	//if (CmpDate(&lt, &check) >= 0)
	//{
	//	WMessageBox(0,  MB_OK | MB_ICONERROR | MB_TASKMODAL, L"Software update", L"Your freeware copy of %s is expired.\n Please download the more recent version from http://users.telenet.be/serhiy.perevoznyk",
	//		applicationName);
	//	ShellExecute(0, L"open", L"http://users.telenet.be/serhiy.perevoznyk/krento.html", NULL, NULL, SW_SHOWNORMAL);
	//	return FALSE;
	//}
	//else
	//	return TRUE;
}

void WINAPI ExtractArchive(LPWSTR archiveName, LPWSTR destination)
{
	TCHAR PathToSearchInto [MAX_PATH] = {0};
	TCHAR fileSpec[MAX_PATH];
	StripFileName(archiveName, fileSpec);

	HZIP hz = OpenZip(fileSpec,0,ZIP_FILENAME);
	if (hz != NULL)
	{
		SetArchiveDirectory(hz, destination);
		ZIPENTRYW ze = {0}; 
		GetZipItem(hz,-1,&ze); 
		int numitems=ze.index;
		for (int i=0; i<numitems; i++)
		{ 
			GetZipItem(hz,i,&ze);
			_tcscpy(PathToSearchInto, destination);
			_tcscat(PathToSearchInto, _T("\\"));
			_tcscat(PathToSearchInto, ze.name);
			TCHAR *c = PathToSearchInto;
			while (*c) 
			{
				if (*c == _T('/') ) 
					*c = _T('\\');
				c++;
			}
			UnzipItem(hz,i,PathToSearchInto,0,ZIP_FILENAME);
			ZeroMemory(&PathToSearchInto, sizeof(PathToSearchInto));
		}
	CloseZip(hz);
	}
}

void WINAPI CreateBackup(LPWSTR archiveName, LPWSTR baseFolder)
{
	TCHAR PathToSearchInto [MAX_PATH] = {0};
	TCHAR fileSpec[MAX_PATH];
	StripFileName(archiveName, fileSpec);


	HZIP archive = CreateZip(fileSpec, 0, ZIP_FILENAME);
	AddFolderContent(archive, baseFolder, L"Classes");
	AddFolderContent(archive, baseFolder, L"Docklets");
	AddFolderContent(archive, baseFolder, L"Extensions");
	AddFolderContent(archive, baseFolder, L"Icons");
	AddFolderContent(archive, baseFolder, L"Languages");
	AddFolderContent(archive, baseFolder, L"Menus");
	AddFolderContent(archive, baseFolder, L"Shortcuts");
	AddFolderContent(archive, baseFolder, L"Skins");
	AddFolderContent(archive, baseFolder, L"Stones");
	AddFolderContent(archive, baseFolder, L"Toys");
	
	_tcscpy(PathToSearchInto, baseFolder);
	_tcscat(PathToSearchInto, _T("\\"));
	_tcscat(PathToSearchInto, L"Krento.ini");
	ZipAdd(archive, L"Krento.ini", PathToSearchInto,0,ZIP_FILENAME);
	ZeroMemory(&PathToSearchInto, sizeof(PathToSearchInto));

	_tcscpy(PathToSearchInto, baseFolder);
	_tcscat(PathToSearchInto, _T("\\"));
	_tcscat(PathToSearchInto, L"AppImages.ini");
	ZipAdd(archive, L"AppImages.ini", PathToSearchInto,0,ZIP_FILENAME);
	ZeroMemory(&PathToSearchInto, sizeof(PathToSearchInto));

	_tcscpy(PathToSearchInto, baseFolder);
	_tcscat(PathToSearchInto, _T("\\"));
	_tcscat(PathToSearchInto, L"KeyMapping.ini");
	ZipAdd(archive, L"KeyMapping.ini", PathToSearchInto,0,ZIP_FILENAME);
	ZeroMemory(&PathToSearchInto, sizeof(PathToSearchInto));

	_tcscpy(PathToSearchInto, baseFolder);
	_tcscat(PathToSearchInto, _T("\\"));
	_tcscat(PathToSearchInto, L"WebImages.ini");
	ZipAdd(archive, L"WebImages.ini", PathToSearchInto,0,ZIP_FILENAME);
	ZeroMemory(&PathToSearchInto, sizeof(PathToSearchInto));

	CloseZip(archive);
}


static BOOL CALLBACK EnumWindowsProc(HWND hwnd, LPARAM lParam)
{
	HWND hChild = FindWindowEx( hwnd, NULL, _T("SHELLDLL_DefView"), NULL );
	if( hChild )
	{
		HWND hDesk = FindWindowEx( hChild, NULL, _T("SysListView32"), NULL );
		if( hDesk )
		{
			*(reinterpret_cast<HWND*>(lParam)) = hDesk;
			return FALSE;
		}
	}

	return TRUE;
}


HWND WINAPI GetDesktopWindowHandle()
{
	HWND DesktopW = NULL;

	HWND hwnd = FindWindow(L"Progman", L"Program Manager");
	if (!hwnd) return NULL;  // Default Shell (Explorer) not started

	if (!(hwnd = FindWindowEx(hwnd, NULL, L"SHELLDLL_DefView", L"")) ||
		!(DesktopW = FindWindowEx(hwnd, NULL, L"SysListView32", L"FolderView")))  // for Windows 7 (with Aero)
	{
		HWND WorkerW = NULL;
		while (WorkerW = FindWindowEx(NULL, WorkerW, L"WorkerW", L""))
		{
			if ((hwnd = FindWindowEx(WorkerW, NULL, L"SHELLDLL_DefView", L"")) &&
				(DesktopW = FindWindowEx(hwnd, NULL, L"SysListView32", L"FolderView"))) break;
		}
	}

	return DesktopW;
}

BOOL WINAPI DesktopDblClick(POINT point)
{
#ifndef _WIN64
	if (IsWow64())
		return false;
#endif

	LV_HITTESTINFO info = {0};
	info.pt = point;
	HWND w = WindowFromPoint(point);
	if (w != NULL)
	{
		HWND dsk = GetDesktopWindowHandle();
		if (dsk == w)
		{
			ListView_HitTest(dsk, &info);
			if (info.iItem == -1)
				return TRUE;
		}
	}

	return FALSE;
}

BOOL WINAPI Matches(LPWSTR itemText, LPWSTR searchText)
{
	if ( (!itemText) || (!searchText) )
		return FALSE;

	size_t size = wcslen(itemText);
	size_t txtSize = wcslen(searchText);
	size_t curChar = 0;

	LPWSTR itemBuf = new WCHAR[size + 1];
	LPWSTR searchBuf = new WCHAR[txtSize + 1];

	ZeroMemory(itemBuf, size + 1);
	ZeroMemory(searchBuf, txtSize + 1);

	_tcscpy(itemBuf, itemText);
	_tcscpy(searchBuf, searchText);

	itemBuf = _wcslwr(itemBuf);
	searchBuf = _wcslwr(searchBuf);

	for (size_t i = 0; i < size; i++)
	{

		if (itemBuf[i] == searchBuf[curChar])
		{
			curChar++;
			if (curChar >= txtSize)
			{
				delete [] itemBuf;
				delete [] searchBuf;
				return TRUE;
			}
		}
	}

	delete [] itemBuf;
	delete [] searchBuf;
	return FALSE;
}