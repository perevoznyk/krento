#include "stdafx.h"
#include "SystemInfo.h"
#include <shlwapi.h>
#include <shlobj.h>
#include <shellapi.h>
#include "Netlistmgr.h"
#include "Wininet.h"
#include "Winsock2.h"
#include <Dbghelp.h>
#include "FileOperations.h"
#include <psapi.h>
#include <Tlhelp32.h>

#ifndef _WIN64
typedef BOOL (WINAPI *LPFN_ISWOW64PROCESS) (HANDLE, PBOOL);
LPFN_ISWOW64PROCESS fnIsWow64Process;
#endif

typedef BOOL (WINAPI *LPFN_QUERYFULLPROCESSIMAGENAME) (
  HANDLE hProcess,
  DWORD dwFlags,
  LPTSTR lpExeName,
  PDWORD lpdwSize
);
LPFN_QUERYFULLPROCESSIMAGENAME fnQueryFullProcessImageName;

BOOL WINAPI IsWindows7()
{
	OSVERSIONINFOEX osvi;
	DWORDLONG dwlConditionMask = 0;
	int op=VER_GREATER_EQUAL;

	// Initialize the OSVERSIONINFOEX structure.
	ZeroMemory(&osvi, sizeof(OSVERSIONINFOEX));
	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFOEX);
	osvi.dwMajorVersion = 6;
	osvi.dwMinorVersion = 1; // Windows 7

	// Initialize the condition mask.
	VER_SET_CONDITION( dwlConditionMask, VER_MAJORVERSION, op );
	VER_SET_CONDITION( dwlConditionMask, VER_MINORVERSION, op );

	// Perform the test.
	return VerifyVersionInfo(&osvi,VER_MAJORVERSION | VER_MINORVERSION,dwlConditionMask);
}

BOOL WINAPI IsWindows8()
{
	OSVERSIONINFOEX osvi;
	DWORDLONG dwlConditionMask = 0;
	int op=VER_GREATER_EQUAL;

	// Initialize the OSVERSIONINFOEX structure.
	ZeroMemory(&osvi, sizeof(OSVERSIONINFOEX));
	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFOEX);
	osvi.dwMajorVersion = 6;
	osvi.dwMinorVersion = 2; // Windows 8

	// Initialize the condition mask.
	VER_SET_CONDITION( dwlConditionMask, VER_MAJORVERSION, op );
	VER_SET_CONDITION( dwlConditionMask, VER_MINORVERSION, op );

	// Perform the test.
	return VerifyVersionInfo(&osvi,VER_MAJORVERSION | VER_MINORVERSION,dwlConditionMask);
}

BOOL WINAPI IsWindowsVista()
{
	OSVERSIONINFOEX osvi;
	DWORDLONG dwlConditionMask = 0;
	int op=VER_GREATER_EQUAL;

	// Initialize the OSVERSIONINFOEX structure.
	ZeroMemory(&osvi, sizeof(OSVERSIONINFOEX));
	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFOEX);
	osvi.dwMajorVersion = 6;

	// Initialize the condition mask.
	VER_SET_CONDITION( dwlConditionMask, VER_MAJORVERSION, op );

	// Perform the test.
	return VerifyVersionInfo(&osvi,VER_MAJORVERSION | VER_MINORVERSION,dwlConditionMask);
}


BOOL WINAPI IsWindowsXP() 
{
   OSVERSIONINFOEX osvi;
   DWORDLONG dwlConditionMask = 0;
   int op=VER_GREATER_EQUAL;

   // Initialize the OSVERSIONINFOEX structure.

   ZeroMemory(&osvi, sizeof(OSVERSIONINFOEX));
   osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFOEX);
   osvi.dwMajorVersion = 5;
   osvi.dwMinorVersion = 1;

   // Initialize the condition mask.

   VER_SET_CONDITION( dwlConditionMask, VER_MAJORVERSION, op );
   VER_SET_CONDITION( dwlConditionMask, VER_MINORVERSION, op );

   // Perform the test.

   return VerifyVersionInfo(
      &osvi, 
      VER_MAJORVERSION | VER_MINORVERSION,
      dwlConditionMask);
}

BOOL WINAPI IsWindowsXPSP2() 
{
   OSVERSIONINFOEX osvi;
   DWORDLONG dwlConditionMask = 0;
   int op=VER_GREATER_EQUAL;

   // Initialize the OSVERSIONINFOEX structure.

   ZeroMemory(&osvi, sizeof(OSVERSIONINFOEX));
   osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFOEX);
   osvi.dwMajorVersion = 5;
   osvi.dwMinorVersion = 1;
   osvi.wServicePackMajor = 2;
   osvi.wServicePackMinor = 0;

   // Initialize the condition mask.

   VER_SET_CONDITION( dwlConditionMask, VER_MAJORVERSION, op );
   VER_SET_CONDITION( dwlConditionMask, VER_MINORVERSION, op );
   VER_SET_CONDITION( dwlConditionMask, VER_SERVICEPACKMAJOR, op );
   VER_SET_CONDITION( dwlConditionMask, VER_SERVICEPACKMINOR, op );

   // Perform the test.

   return VerifyVersionInfo(
      &osvi, 
      VER_MAJORVERSION | VER_MINORVERSION | 
      VER_SERVICEPACKMAJOR | VER_SERVICEPACKMINOR,
      dwlConditionMask);
}

BOOL WINAPI IsMultiTouchReady()
{
	BYTE digitizerStatus = (BYTE)GetSystemMetrics(SM_DIGITIZER);
	if ((digitizerStatus & (0x80 + 0x40)) == 0) //Stack Ready + MultiTouch
	{
		return FALSE;
	}
	else
		return TRUE;

}

BOOL WINAPI IsTabletPC()
{
	return (GetSystemMetrics(SM_TABLETPC) != 0);
}

BOOL WINAPI IsMediaCenter()
{
	return (GetSystemMetrics(SM_MEDIACENTER) != 0);
}

BOOL WINAPI IsConnectedToInternet()
{
	if (IsWindows7())
	{
		HRESULT hr;
		INetworkListManager *pNetworkListManager = NULL;
		hr = CoCreateInstance(CLSID_NetworkListManager, NULL,
			CLSCTX_LOCAL_SERVER, IID_INetworkListManager,
			(LPVOID *)&pNetworkListManager);
		if (SUCCEEDED(hr))
		{
			VARIANT_BOOL bConnected;
			pNetworkListManager->get_IsConnectedToInternet(&bConnected);
			if (bConnected)
				return TRUE;
			else
				return FALSE;
		}
		else
			return FALSE;

	}
	else
	{
		//Windows XP way
		DWORD dwConnectedStateFlags;
		BOOL fIsConnected = InternetGetConnectedState(&dwConnectedStateFlags, 0);
		return fIsConnected;
	}
}

INT WINAPI GetScreenColors()
{
	HDC dc = GetDC(NULL);
	int bitsPerPixel = GetDeviceCaps(dc, BITSPIXEL);
	ReleaseDC(NULL, dc);
	return bitsPerPixel;
}

BOOL WINAPI IsTrueColorMonitor()
{
	return (GetScreenColors() >= 32);
}

INT WINAPI GetTaskbarPosition()
{
	try
	{
		int result = 0;
		APPBARDATA AppBar = {0};
		SHAppBarMessage(ABM_GETTASKBARPOS, &AppBar);
		if ((AppBar.rc.top == AppBar.rc.left) && (AppBar.rc.bottom > AppBar.rc.right))
			result = 0; //left
		else
			if ((AppBar.rc.top == AppBar.rc.left) && (AppBar.rc.bottom < AppBar.rc.right))
				result = 1; //top
			else
				if (AppBar.rc.top > AppBar.rc.left)
					result = 2; //bottom
				else
					result = 3; //right
		return result;
	}
	catch (...)
	{
		return 0;
	}
}

static BOOL WINAPI CALLBACK MyInfoEnumProc(
  HMONITOR hMonitor,  // handle to display monitor
  HDC hdcMonitor,     // handle to monitor DC
  LPRECT lprcMonitor, // monitor intersection rectangle
  LPARAM dwData       // data
)
{
	MONITORINFO mi = {0};
	mi.cbSize = sizeof(MONITORINFO);
	GetMonitorInfo(hMonitor, &mi);
	if (mi.dwFlags == MONITORINFOF_PRIMARY)
	{
		PrimaryMonitor = hMonitor;
		return FALSE;
	}
	else
		return TRUE;
}

HMONITOR WINAPI GetPrimaryMonitor()
{
	EnumDisplayMonitors(NULL, NULL, MyInfoEnumProc, 0);
	return PrimaryMonitor;
}

void WINAPI GetPrimaryMonitorBounds(LPRECT rect)
{
	if (PrimaryMonitor == 0)
		GetPrimaryMonitor();
	MONITORINFO mi = {0};
	mi.cbSize = sizeof(MONITORINFO);
	GetMonitorInfo(PrimaryMonitor, &mi);
	*rect = mi.rcMonitor;
}

void WINAPI GetPrimaryMonitorArea(LPRECT rect)
{
	if (PrimaryMonitor == 0)
		GetPrimaryMonitor();
	MONITORINFO mi = {0};
	mi.cbSize = sizeof(MONITORINFO);
	GetMonitorInfo(PrimaryMonitor, &mi);
	*rect = mi.rcWork;
}

void WINAPI GetFullScreenSize(LPRECT rect)
{
	HDC screenDC = GetDC(NULL);
	GetClipBox(screenDC, rect);
	ReleaseDC(0, screenDC);
}

BOOL WINAPI RecycleBinEmpty()
{
	BOOL bResult = FALSE;
	LPSHELLFOLDER pDesktop = NULL;
	LPITEMIDLIST pidlRecycleBin = NULL;
	HRESULT hr;
	LPSHELLFOLDER m_pRecycleBin;
	LPENUMIDLIST penumFiles	= NULL;
	LPITEMIDLIST pidl = NULL;
	LPMALLOC pMalloc = NULL;
	BOOL EmptyFlag;

	SHGetDesktopFolder(&pDesktop);
	SHGetSpecialFolderLocation (NULL, CSIDL_BITBUCKET, &pidlRecycleBin);
	pDesktop->BindToObject(pidlRecycleBin, NULL, IID_IShellFolder, (LPVOID *)&m_pRecycleBin);

	SHGetMalloc(&pMalloc); // windows memory management pointer needed later

	hr = m_pRecycleBin->EnumObjects(NULL, SHCONTF_FOLDERS|SHCONTF_NONFOLDERS| SHCONTF_INCLUDEHIDDEN, &penumFiles);
	if(SUCCEEDED (hr))
	{
		while(penumFiles->Next(1, &pidl, NULL) != S_FALSE)
		{
			bResult = TRUE;
			break;
		}

	}
	if (NULL != penumFiles)
	{
		penumFiles->Release ();
		penumFiles = NULL;
	}
	pMalloc->Release();

	if(bResult)
		EmptyFlag = FALSE;
	else
		EmptyFlag = TRUE;

	return EmptyFlag;
}


BOOL WINAPI IsUserPlayingFullscreen()
{
	TCHAR buff[MAX_PATH];
	HWND hWnd = GetForegroundWindow();
	HWND parent = GetParent(hWnd);
	HWND grandParent = GetParent(parent);
	HWND desktopHandle = GetDesktopWindow();
	HWND shellHandle = GetShellWindow();

	RealGetWindowClass(hWnd, buff, MAX_PATH);

	if (hWnd == desktopHandle || hWnd == shellHandle ||
		parent == desktopHandle || parent == shellHandle ||
		grandParent == desktopHandle || grandParent == shellHandle ||
		(0 == wcscmp(L"WorkerW", buff)) || (0 == wcscmp(L"Progman", buff)))
		return false;

	RECT rcWindow;
	GetWindowRect(hWnd, &rcWindow);
	HMONITOR hm = MonitorFromRect(&rcWindow, MONITOR_DEFAULTTONULL);
	if (hm == NULL) return false;
	MONITORINFO mi = {0};
	mi.cbSize = sizeof(MONITORINFO);
	GetMonitorInfo(hm, &mi);

	return EqualRect(&rcWindow, &mi.rcMonitor);
}


void WINAPI CurrentIPAddress(LPWSTR address, UINT len)
{
	WSADATA wsaData;
	hostent* localHost;
	char* localIP;

	WSAStartup( MAKEWORD(2,2), &wsaData );

	// Get the local host information
	localHost = gethostbyname("");
	localIP = inet_ntoa (*(struct in_addr *)*localHost->h_addr_list);
	
    if( len == 0 )
        len = (UINT)strlen( localIP );

    MultiByteToWideChar(CP_ACP,
                        MB_PRECOMPOSED,
                        localIP,
                        len + 1,
                        address,
                        len + 1 );
    //
    // Ensure NULL termination.
    //
    address[len] = 0;
	WSACleanup();
}

BOOL WINAPI PortAvailable(int port)
{
	BOOL result = FALSE;
	WSADATA wsaData;
	WSAStartup(MAKEWORD(2,2), &wsaData);
    SOCKET ConnectSocket;
	ConnectSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
 
  //----------------------
  // The sockaddr_in structure specifies the address family,
  // IP address, and port of the server to be connected to.
  //----------------------
  sockaddr_in clientService; 
  clientService.sin_family = AF_INET;
  clientService.sin_addr.s_addr = inet_addr( "127.0.0.1" );
  clientService.sin_port = htons( port );
  if ( connect( ConnectSocket, (SOCKADDR*) &clientService, sizeof(clientService) ) == SOCKET_ERROR)
	  result = TRUE;

  WSACleanup();
  return result;
}


BOOL WINAPI IsAssembly(LPCWSTR FileName)
{
	PVOID Base;
	HANDLE Handle;
	HANDLE Map;
	PIMAGE_DOS_HEADER DosHeader;
	ULONG Size;
	bool Result = FALSE;

	TCHAR fileSpec[MAX_PATH];
	StripFileName(FileName, fileSpec);

	Handle = CreateFile(fileSpec, GENERIC_READ, FILE_SHARE_READ,
		NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, 0);
	Map = CreateFileMapping(Handle, NULL, PAGE_READONLY, 0, 0, NULL);
	Base = MapViewOfFile(Map, FILE_MAP_READ, 0, 0, 0);
	DosHeader = (PIMAGE_DOS_HEADER)Base;

	__try
	{
		if ((DosHeader == NULL) || (ImageDirectoryEntryToData( Base, FALSE,
			IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR, &Size) == NULL)) 
			Result = FALSE;
		else
			Result = TRUE;
	}
	__finally
	{
		if (Handle != 0)
		{
			UnmapViewOfFile(Base);
			CloseHandle(Map);
			CloseHandle(Handle);
		}
	}

	return Result;
}


BOOL WINAPI IsWow64()
{
#ifndef _WIN64
    BOOL bIsWow64 = FALSE;

    fnIsWow64Process = (LPFN_ISWOW64PROCESS)GetProcAddress(
        GetModuleHandle(TEXT("kernel32")),"IsWow64Process");
  
    if (NULL != fnIsWow64Process)
    {
        if (!fnIsWow64Process(GetCurrentProcess(),&bIsWow64))
        {
            // handle error
        }
    }
    return bIsWow64;
#else
	return false;
#endif
}

BOOL WINAPI IsMetroActive()
{
	if (IsWindows8())
	{
		QUERY_USER_NOTIFICATION_STATE state;
		SHQueryUserNotificationState(&state);
		if (state == 7)
			return TRUE;
		else
			return FALSE;
	}
	else
		return FALSE;
}


BOOL SetPrivilege( 
	HANDLE hToken,  // token handle 
	LPCTSTR Privilege,  // Privilege to enable/disable 
	BOOL bEnablePrivilege  // TRUE to enable. FALSE to disable 
) 
{ 
	TOKEN_PRIVILEGES tp = { 0 }; 
	// Initialize everything to zero 
	LUID luid; 
	DWORD cb=sizeof(TOKEN_PRIVILEGES); 
	if(!LookupPrivilegeValue( NULL, Privilege, &luid ))
		return FALSE; 
	tp.PrivilegeCount = 1; 
	tp.Privileges[0].Luid = luid; 
	if(bEnablePrivilege) { 
		tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED; 
	} else { 
		tp.Privileges[0].Attributes = 0; 
	} 
	AdjustTokenPrivileges( hToken, FALSE, &tp, cb, NULL, NULL ); 
	if (GetLastError() != ERROR_SUCCESS) 
		return FALSE; 

	return TRUE;
}

BOOL WINAPI GetApplicationFromWindow(HWND window, LPWSTR appName)
{
	BOOL result = FALSE;
	DWORD processId = 0;
	DWORD dwSize = MAX_PATH;

	GetWindowThreadProcessId(window, &processId);
	HANDLE handle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, processId);
	if (handle == 0)
		return result;
	if (IsWindowsVista())
	{
		fnQueryFullProcessImageName = (LPFN_QUERYFULLPROCESSIMAGENAME)GetProcAddress(
			GetModuleHandle(TEXT("kernel32")),"QueryFullProcessImageName");

		if (NULL != fnQueryFullProcessImageName)
		{
			result = fnQueryFullProcessImageName(handle, 0, appName, &dwSize);
		}
		else
			result = GetModuleFileNameEx(handle, 0, appName, MAX_PATH);

	}
	else
		result = GetModuleFileNameEx(handle, 0, appName, MAX_PATH);
	CloseHandle(handle);
	return result;
}

BOOL WINAPI IsNativeWin64()
{
#ifndef _WIN64
	return FALSE;
#else
	return TRUE;
#endif
}


HICON WINAPI GetAppicationIcon(HWND hwnd)
{
	HICON iconHandle = (HICON)SendMessage(hwnd,WM_GETICON,ICON_SMALL2,0);
	if(!iconHandle)
		iconHandle = (HICON)SendMessage(hwnd,WM_GETICON,ICON_SMALL,0);
	if(!iconHandle)
		iconHandle = (HICON)SendMessage(hwnd,WM_GETICON,ICON_BIG,0);
	if (!iconHandle)
		iconHandle = (HICON)GetClassLongPtr(hwnd, GCLP_HICON);
	if (!iconHandle)
		iconHandle = (HICON)GetClassLongPtr(hwnd, GCLP_HICONSM);
	return iconHandle;
}


DWORD GetParentProcessId()
{
	DWORD pHandle = 0;
	HANDLE hProcessSnap;
	HANDLE hProcess;
	PROCESSENTRY32 pe32;
	DWORD dwCurrentProcessId = 0;

	dwCurrentProcessId = GetCurrentProcessId();


	// Take a snapshot of all processes in the system.
	hProcessSnap = CreateToolhelp32Snapshot( TH32CS_SNAPPROCESS, 0 );
	if( hProcessSnap == INVALID_HANDLE_VALUE )
	{
		goto exit;
	}

	// Set the size of the structure before using it.
	pe32.dwSize = sizeof( PROCESSENTRY32 );

	// Retrieve information about the first process.
	// We can use this to get hte process Id of our parent processs
	if( !Process32First( hProcessSnap, &pe32 ) )
	{
		goto exit;
	}

	do
	{
		// Retrieve a handle to the process
		hProcess = OpenProcess( PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, pe32.th32ProcessID );
		if( hProcess != NULL )
		{
			if (dwCurrentProcessId == pe32.th32ProcessID)
			{
				CloseHandle( hProcess );

				pHandle = pe32.th32ParentProcessID;
				break;
			}

			CloseHandle( hProcess );
		}

	} while( Process32Next( hProcessSnap, &pe32 ) );


exit:
	CloseHandle( hProcessSnap );
	return pHandle;
}
