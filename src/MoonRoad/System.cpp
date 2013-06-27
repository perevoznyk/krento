#include "stdafx.h"
#include "PowrProf.h"
#include "MoonRoad.h"
#include "resource.h"
#include <shlwapi.h>
#include <shlobj.h>
#include <shellapi.h>
#include "System.h"
#include "SystemInfo.h"
#include "Mmsystem.h"


void WINAPI BringWindowToFront(HWND hwnd)
{
	HWND window;

	SetForegroundWindow(hwnd);
	window = GetForegroundWindow();

	if (hwnd != window)
	{
		SetForegroundWindow(hwnd);
		SetActiveWindow(hwnd);
		BringWindowToTop(hwnd);

		SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, (LPVOID)0, SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE);
		SetWindowPos(hwnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
		if (IsWindowVisible(hwnd) && IsWindowEnabled(hwnd))
		{
			SetFocus(hwnd);
		}

		HWND hwndFrgnd = GetForegroundWindow();
		if (hwndFrgnd != hwnd) {
		
			if (!hwndFrgnd)
				hwndFrgnd = FindWindow(L"Shell_TrayWnd", NULL);

			DWORD idFrgnd = GetWindowThreadProcessId(hwndFrgnd, NULL);
			DWORD idSwitch = GetWindowThreadProcessId(hwnd, NULL);

			AttachThreadInput(idFrgnd, idSwitch, TRUE);
			SetForegroundWindow(hwnd);
			SetActiveWindow(hwnd);
			BringWindowToTop(hwnd);
			if (IsWindowVisible(hwnd) && IsWindowEnabled(hwnd))
			{
				SetFocus(hwnd);
			}

			hwndFrgnd = GetForegroundWindow();
			if (hwndFrgnd != hwnd) {
				INPUT inp[4];
				ZeroMemory(&inp, sizeof(inp));
				inp[0].type = inp[1].type = inp[2].type = inp[3].type = INPUT_KEYBOARD;
				inp[0].ki.wVk = inp[1].ki.wVk = inp[2].ki.wVk = inp[3].ki.wVk = VK_MENU;
				inp[0].ki.dwFlags = inp[2].ki.dwFlags = KEYEVENTF_EXTENDEDKEY;
				inp[1].ki.dwFlags = inp[3].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
				SendInput(4, inp, sizeof(INPUT));

				SetForegroundWindow(hwnd);
			}

			AttachThreadInput(idFrgnd, idSwitch, FALSE);
		}
	}
	else
	{
		if (IsWindowVisible(hwnd) && IsWindowEnabled(hwnd))
		{
			SetFocus(hwnd);
		}
	}

	if (IsIconic(hwnd)) 
	{
		PostMessage(hwnd, WM_SYSCOMMAND, SC_RESTORE, 0);
	}
}

//Invisible empty window. This procedure creates window if you need the window handle only
HWND WINAPI AllocateDefaultHWND()
{
	return AllocateWindowClass(L"TPUtilWindow");
}


HWND WINAPI AllocateWindowClass(LPCWSTR className)
{
	WNDCLASS UtilWindowClass;
	UtilWindowClass.style = 0;
    UtilWindowClass.lpfnWndProc = DefWindowProc;
    UtilWindowClass.cbClsExtra = 0;
    UtilWindowClass.cbWndExtra = 0;
    UtilWindowClass.hInstance = 0;
    UtilWindowClass.hIcon = 0;
    UtilWindowClass.hCursor = LoadCursor(NULL, IDC_ARROW); 
    UtilWindowClass.hbrBackground = 0;
    UtilWindowClass.lpszMenuName = NULL;
    UtilWindowClass.lpszClassName = className;

	WNDCLASS TempClass;

	if (!GetClassInfo(hDllInstance, UtilWindowClass.lpszClassName, &TempClass))
	{
		UtilWindowClass.hInstance = GetModuleHandle(NULL);
		RegisterClass(&UtilWindowClass);
	}

	HWND handle = CreateWindowEx(WS_EX_TOOLWINDOW, UtilWindowClass.lpszClassName, L"", WS_POPUP, 
		0, 0, 0, 0, 0, 0, GetModuleHandle(NULL), NULL);

	return handle;
}

LRESULT CALLBACK LayeredWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message) 
	{

	case WM_SYSCOMMAND:
		switch (GET_SC_WPARAM(wParam))
		{
		case SC_MAXIMIZE:
		case SC_MINIMIZE:
		case SC_RESTORE:
		case SC_TASKLIST:
		case SC_SIZE:
			return 0;
		default:
			return DefWindowProc(hWnd, message, wParam, lParam);
		}

		break;
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;  
}

void WINAPI DrawLayeredWindow(HWND handle, int left, int top, int width, int height, HDC buffer, COLORREF colorKey, byte alpha, BOOL redrawOnly)
{
	POINT SourcePosition = {0};
	BLENDFUNCTION BlendFunction = {0};
	POINT NativePosition;
	SIZE NativeSize;
	HDC ScreenDC;

	BlendFunction.BlendOp = AC_SRC_OVER;
	BlendFunction.BlendFlags = 0;
	BlendFunction.SourceConstantAlpha = alpha;
	BlendFunction.AlphaFormat = AC_SRC_ALPHA;

	NativePosition.x = left;
	NativePosition.y = top;

	NativeSize.cx = width;
	NativeSize.cy = height;

	ScreenDC = GetDC(0);

	__try
	{
		if (redrawOnly)
		{
			UpdateLayeredWindow(
				handle,
				ScreenDC,
				NULL,
				&NativeSize,
				buffer,
				&SourcePosition,
				colorKey,
				&BlendFunction,
				ULW_ALPHA);

		}
		else
		{
			UpdateLayeredWindow(
				handle,
				ScreenDC,
				&NativePosition,
				&NativeSize,
				buffer,
				&SourcePosition,
				colorKey,
				&BlendFunction,
				ULW_ALPHA);
		}
	}
	__finally
	{
		ReleaseDC(0, ScreenDC);
	}


}

HWND WINAPI AllocateLayeredWindow(LPCWSTR className)
{
	WNDCLASS UtilWindowClass;
	UtilWindowClass.style = CS_DBLCLKS;
    UtilWindowClass.lpfnWndProc = LayeredWndProc;
    UtilWindowClass.cbClsExtra = 0;
    UtilWindowClass.cbWndExtra = 0;
    UtilWindowClass.hInstance = 0;
    UtilWindowClass.hIcon = 0;
    UtilWindowClass.hCursor = 0; 
    UtilWindowClass.hbrBackground = 0;
    UtilWindowClass.lpszMenuName = NULL;
    UtilWindowClass.lpszClassName = className;

	WNDCLASS TempClass;

	if (!GetClassInfo(hDllInstance, UtilWindowClass.lpszClassName, &TempClass))
	{
		UtilWindowClass.hInstance = GetModuleHandle(NULL);
		RegisterClass(&UtilWindowClass);
	}

	HWND handle = CreateWindowEx(WS_EX_TOOLWINDOW | WS_EX_LAYERED, UtilWindowClass.lpszClassName, L"", WS_POPUP, 
		0, 0, 10, 10, 0, 0, GetModuleHandle(NULL), NULL);

	return handle;
}

HWND WINAPI AllocateHWND(LONG_PTR method)
{
	WNDCLASS UtilWindowClass;
	UtilWindowClass.style = 0;
    UtilWindowClass.lpfnWndProc = DefWindowProc;
    UtilWindowClass.cbClsExtra = 0;
    UtilWindowClass.cbWndExtra = 0;
    UtilWindowClass.hInstance = 0;
    UtilWindowClass.hIcon = 0;
    UtilWindowClass.hCursor = 0;
    UtilWindowClass.hbrBackground = 0;
    UtilWindowClass.lpszMenuName = NULL;
    UtilWindowClass.lpszClassName = L"TPUtilWindow";

	WNDCLASS TempClass;

	if (!GetClassInfo(hDllInstance, UtilWindowClass.lpszClassName, &TempClass))
	{
		UtilWindowClass.hInstance = GetModuleHandle(NULL);
		RegisterClass(&UtilWindowClass);
	}


	HWND handle = CreateWindowEx(WS_EX_TOOLWINDOW, UtilWindowClass.lpszClassName, L"", WS_POPUP, 
		0, 0, 0, 0, 0, 0, GetModuleHandle(NULL), NULL);

	if (method)
  	 SetWindowLongPtr(handle, GWLP_WNDPROC, method);

	return handle;
}



BOOL WINAPI DeallocateHWND(HWND hwnd)
{
	SetWindowLongPtr(hwnd, GWLP_WNDPROC, (LONG_PTR)DefWindowProc);
	return DestroyWindow(hwnd);
}


void WINAPI RestoreWindowSubclass(HWND hwnd)
{
	SetWindowLongPtr(hwnd, GWLP_WNDPROC, (LONG_PTR)DefWindowProc);
}

BOOL WINAPI ShutdownWindows(UINT flags)
{
	HANDLE hToken; 
	TOKEN_PRIVILEGES tkp; 

	// Get a token for this process. 

	if (!OpenProcessToken(GetCurrentProcess(), 
		TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &hToken)) 
		return( FALSE ); 

	// Get the LUID for the shutdown privilege. 

	LookupPrivilegeValue(NULL, SE_SHUTDOWN_NAME, 
		&tkp.Privileges[0].Luid); 

	tkp.PrivilegeCount = 1;  // one privilege to set    
	tkp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED; 

	// Get the shutdown privilege for this process. 

	AdjustTokenPrivileges(hToken, FALSE, &tkp, 0, 
		(PTOKEN_PRIVILEGES)NULL, 0); 

	if (GetLastError() != ERROR_SUCCESS) 
		return FALSE; 

	// Shut down the system and force all applications to close. 

	if (!ExitWindowsEx(flags, SHTDN_REASON_MAJOR_OTHER | SHTDN_REASON_MINOR_OTHER | SHTDN_REASON_FLAG_PLANNED)) 
		return FALSE; 

	//shutdown was successful
	return TRUE;
}


BOOL WINAPI SuspendWindows()
{
	return SetSuspendState(false, true, false);
}

BOOL WINAPI HibernateWindows()
{
	return SetSuspendState(true, true, false);
}

BOOL WINAPI GetWorkingArea(LPRECT area)
{
	MONITORINFOEX info;
	info.cbSize = sizeof(info);

	HWND tray = FindWindow(L"Shell_TrayWnd", NULL);
	if (tray)
	{
		HMONITOR monitor = MonitorFromWindow(tray, MONITOR_DEFAULTTOPRIMARY);
		if (monitor)
		{
			if (GetMonitorInfo(monitor, &info))
			{
				area->left = info.rcWork.left;
				area->top = info.rcWork.top;
				area->bottom = info.rcWork.bottom;
				area->right = info.rcWork.right;
				return TRUE;
			}
			else
				return FALSE;
		}
		else
			return FALSE;
	}
	else
		return FALSE;
}

void WINAPI UpdateWindowPosition(HWND handle, int x, int y)
{
	SetWindowPos(handle, 0, x, y, 0, 0, SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOREDRAW | SWP_NOZORDER);
}


void WINAPI TurnMonitorOn()
{
	// Eliminate user's interaction for 500 ms
	Sleep(500);
	// Turn on monitor
	SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, (LPARAM) -1);
}

void WINAPI TurnMonitorOff()
{
	// Eliminate user's interaction for 500 ms
	Sleep(500);
	// Turn off monitor
	SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, (LPARAM) 2);
}

void WINAPI BroadcastApplicationMessage(UINT Msg)
{
	DWORD dwRecipients = BSM_APPLICATIONS;
	BroadcastSystemMessage(BSF_IGNORECURRENTTASK | BSF_POSTMESSAGE, &dwRecipients, Msg, 0, 0);
}

BOOL WINAPI CursorInsideWindow(HWND hwnd)
{
	POINT pt;
	RECT rect;
	GetCursorPos(&pt);
	GetWindowRect(hwnd, &rect);

	if ((((rect.left <= pt.x) && (pt.x < rect.right)) && ((rect.top <= pt.y) && (pt.y < rect.bottom))) || (GetCapture() == hwnd))
	{
		return true;
	}

	return false;
}

BOOL WINAPI IsGoodWindow(HWND hwnd)
{
	LONG_PTR dwStyle = GetWindowLongPtr(hwnd, GWL_STYLE);

	if (dwStyle & WS_VISIBLE) 
	{
		HWND g_hwndShell = GetShellWindow(); // for service messages
		HWND hwndOwner, hwndTmp = hwnd;
		do {
			hwndOwner = hwndTmp;
			hwndTmp = GetWindow(hwndTmp, GW_OWNER);
		} while (hwndTmp && hwndTmp != g_hwndShell); // service messages

		LONG_PTR dwStyleEx = GetWindowLongPtr(hwndOwner, GWL_EXSTYLE);
		LONG_PTR dwStyleEx2 = (hwnd != hwndOwner) ? GetWindowLongPtr(hwnd, GWL_EXSTYLE) : dwStyleEx;

		if (dwStyleEx2 & WS_EX_TOOLWINDOW)
			return FALSE;

		if (hwnd != hwndOwner)
		{
			if (hwndOwner)
			{
				if (IsWindow(hwndOwner))
				{
					if (!IsWindowVisible(hwndOwner))	
					 return FALSE;
				}
			}

		}

		if (!(dwStyleEx & WS_EX_TOOLWINDOW) || dwStyleEx2 & WS_EX_APPWINDOW || 
			(!(dwStyleEx2 & WS_EX_TOOLWINDOW) && dwStyleEx2 & WS_EX_CONTROLPARENT)) 
		{
			return TRUE;
		}
		else
			return FALSE;
	}
	else
		return FALSE;
}


LONG_PTR WINAPI GetWindowLongPointer(
    __in HWND hWnd,
    __in int nIndex)
{
	return GetWindowLongPtr(hWnd, nIndex);
}


LONG_PTR WINAPI SetWindowLongPointer(
    __in HWND hWnd,
    __in int nIndex,
    __in LONG_PTR dwNewLong)
{
	return SetWindowLongPtr(hWnd, nIndex, dwNewLong);
}

void WINAPI RemoveSystemMenu(HWND hWnd)
{
	ShowWindow(hWnd, SW_HIDE);
	HMENU SysMenu = GetSystemMenu(hWnd, FALSE);
	if (SysMenu != NULL)
	{
		DeleteMenu(SysMenu, SC_TASKLIST, MF_BYCOMMAND);
		DeleteMenu(SysMenu, 7, MF_BYPOSITION);
		DeleteMenu(SysMenu, 5, MF_BYPOSITION);
		DeleteMenu(SysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
		DeleteMenu(SysMenu, SC_MINIMIZE, MF_BYCOMMAND);
		DeleteMenu(SysMenu, SC_SIZE, MF_BYCOMMAND);
		DeleteMenu(SysMenu, SC_RESTORE, MF_BYCOMMAND);
	}
}

void WINAPI ClearUnusedMemory()
{
	try
	{
		SetProcessWorkingSetSize(GetCurrentProcess(), (SIZE_T)-1, (SIZE_T)-1);
	}
	catch (...)
	{
	}
}

void WINAPI NotifyOfChange()
{
	SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, NULL, NULL);
}

void WINAPI ReportSystemEvent(HWND hwnd, int eventId)
{
	NotifyWinEvent(eventId, hwnd, OBJID_CLIENT, CHILDID_SELF);
}

void WINAPI MakeSound(LPCWSTR soundName)
{
	PlaySound(soundName, hDllInstance, SND_RESOURCE | SND_ASYNC | SND_NOWAIT);
}

void WINAPI MakeSoundFromFile(LPCWSTR soundName)
{
	PlaySound(soundName, hDllInstance, SND_FILENAME | SND_ASYNC | SND_NOWAIT);
}

void WINAPI LockForegroundWindow()
{
	LockSetForegroundWindow(LSFW_LOCK);
}

void WINAPI UnlockForegroundWindow()
{
	LockSetForegroundWindow(LSFW_UNLOCK);
}

void WINAPI EmptyRecycleBin()
{
	SHEmptyRecycleBin(NULL, NULL, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI);
}

BOOL WINAPI IsTopMostWindow(HWND handle)
{
	LONG_PTR dwStyle = GetWindowLongPtr(handle, GWL_EXSTYLE);  
	LONG_PTR styleCheck = (dwStyle & WS_EX_TOPMOST);
	return (dwStyle == WS_EX_TOPMOST);
}

void WINAPI SetTopMostWindow(HWND handle, BOOL value)
{
	HWND hWndInsertAfter;
	if (value) 
		hWndInsertAfter = HWND_TOPMOST;
	else  
		hWndInsertAfter = HWND_NOTOPMOST;

	SetWindowPos(handle, hWndInsertAfter, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE);
}


void WINAPI SetBottomMostWindow(HWND handle, BOOL value)
{
	HWND hWndInsertAfter;
	if (value) 
		hWndInsertAfter = HWND_BOTTOM;
	else  
		hWndInsertAfter = HWND_NOTOPMOST;

	SetWindowPos(handle, hWndInsertAfter, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
}

void WINAPI WindowInsertAfter(HWND hwnd, HWND hwndInsertAfter)
{
	SetWindowPos(hwnd, hwndInsertAfter, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
}

 
void WINAPI MakeSoundFromResource(HMODULE hModule, LPCWSTR soundName)
{
	PlaySound(soundName, hModule, SND_RESOURCE | SND_ASYNC | SND_NOWAIT);
}

void WINAPI AddRemoveMessageFilter(UINT message, DWORD dwFlags)
{
	if (IsWindows7())
		ChangeWindowMessageFilter(message, dwFlags);
}

void WINAPI ShowDesktop()
{
    HWND tray = FindWindow(L"Shell_TrayWnd", NULL);
    PostMessage(tray, WM_COMMAND, 407, 0);
}

void WINAPI SetStartup(LPCWSTR appName, LPCWSTR appPath)
{
	HKEY hKey;
	DWORD dwPosition;
	const TCHAR szSubKey[] =  __TEXT("Software\\Microsoft\\Windows\\CurrentVersion\\Run");

	if ( ERROR_SUCCESS == RegCreateKeyEx(HKEY_CURRENT_USER, szSubKey, 0, NULL,
		REG_OPTION_NON_VOLATILE, KEY_QUERY_VALUE | KEY_SET_VALUE, NULL, &hKey, &dwPosition))
	{
		RegSetValueEx(hKey, appName, 0, REG_SZ, (CONST BYTE*)appPath, (lstrlen(appPath)+1)*sizeof(TCHAR) );
		RegCloseKey(hKey);
	}

}

void WINAPI RemoveStartup(LPCWSTR appName)
{
	HKEY hKey;
	DWORD dwPosition;
	const TCHAR szSubKey[] =  __TEXT("Software\\Microsoft\\Windows\\CurrentVersion\\Run");

	if ( ERROR_SUCCESS == RegCreateKeyEx(HKEY_CURRENT_USER, szSubKey, 0, NULL,
		REG_OPTION_NON_VOLATILE, KEY_QUERY_VALUE | KEY_SET_VALUE, NULL, &hKey, &dwPosition))
	{
		RegDeleteValue(hKey, appName);
		RegCloseKey(hKey);
	}
}

BOOL WINAPI GetStartup(LPCWSTR appName)
{
	HKEY Key;
	DWORD Size;
	LONG rc;

	rc = RegOpenKeyEx (
		HKEY_CURRENT_USER,
		L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
		0,
		KEY_READ,
		&Key
		);

	if (rc == ERROR_SUCCESS) {
		rc = RegQueryValueEx (
			Key,
			appName,
			NULL,
			NULL,
			NULL,
			&Size
			);

		if (rc == ERROR_MORE_DATA) {
			rc = ERROR_SUCCESS;
		}

		RegCloseKey (Key);
	}
		return (rc == ERROR_SUCCESS);
}