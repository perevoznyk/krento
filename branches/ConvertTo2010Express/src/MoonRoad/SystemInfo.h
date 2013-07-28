#pragma once

#ifdef __cplusplus
extern "C" {
#endif

static HMONITOR PrimaryMonitor;

BOOL WINAPI IsWindows8();

BOOL WINAPI IsMetroActive();

//Checks if PC is running Windows 7 or better
BOOL WINAPI IsWindows7();

//Checks if PC is running Windows Vista or better
BOOL WINAPI IsWindowsVista();

//Checks if PC is running Windows XP
BOOL WINAPI IsWindowsXP();

//Checks if PC is running Windows XP with Service Pack 2 installed
BOOL WINAPI IsWindowsXPSP2(); 

//Checks if the system is multi touch ready
BOOL WINAPI IsMultiTouchReady();

//Checks if the application is running on the Tablet PC
BOOL WINAPI IsTabletPC();

//Checks if the Media Center version of Windows is installed
BOOL WINAPI IsMediaCenter();

//Checks if PC is connected to Internet
BOOL WINAPI IsConnectedToInternet();

INT WINAPI GetScreenColors();

BOOL WINAPI IsTrueColorMonitor();

INT WINAPI GetTaskbarPosition();

HMONITOR WINAPI GetPrimaryMonitor();

void WINAPI GetPrimaryMonitorBounds(LPRECT rect);

void WINAPI GetPrimaryMonitorArea(LPRECT rect);

void WINAPI GetFullScreenSize(LPRECT rect);

BOOL WINAPI RecycleBinEmpty();

BOOL WINAPI IsUserPlayingFullscreen();

void WINAPI CurrentIPAddress(LPWSTR address, UINT len);

BOOL WINAPI PortAvailable(int port);

BOOL WINAPI IsAssembly(LPCWSTR FileName);

BOOL WINAPI IsWow64();

BOOL WINAPI GetApplicationFromWindow(HWND window, LPWSTR appName);

BOOL WINAPI IsNativeWin64();

HICON WINAPI GetAppicationIcon(HWND hwnd);

DWORD GetParentProcessId();

#ifdef __cplusplus
}
#endif
