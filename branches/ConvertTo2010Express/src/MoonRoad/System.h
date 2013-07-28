#pragma once

EXTERN_C HINSTANCE hDllInstance;

#ifdef __cplusplus
extern "C" {
#endif

void WINAPI BringWindowToFront(HWND window);
HWND WINAPI AllocateDefaultHWND();
HWND WINAPI AllocateHWND(LONG_PTR method);
BOOL WINAPI DeallocateHWND(HWND hwnd);
void WINAPI RestoreWindowSubclass(HWND hwnd);
BOOL WINAPI ShutdownWindows(UINT flags);
BOOL WINAPI SuspendWindows();
BOOL WINAPI HibernateWindows();
BOOL WINAPI GetWorkingArea(PRECT area);
void WINAPI UpdateWindowPosition(HWND handle, int x, int y);
void WINAPI TurnMonitorOn();
void WINAPI TurnMonitorOff();
BOOL WINAPI CursorInsideWindow(HWND hwnd);
BOOL WINAPI IsGoodWindow(HWND hwnd);
LONG_PTR WINAPI GetWindowLongPointer(
    __in HWND hWnd,
    __in int nIndex);
LONG_PTR WINAPI SetWindowLongPointer(
    __in HWND hWnd,
    __in int nIndex,
    __in LONG_PTR dwNewLong);
void WINAPI RemoveSystemMenu(HWND hWnd);
void WINAPI ClearUnusedMemory();
void WINAPI NotifyOfChange();
HWND WINAPI AllocateWindowClass(LPCWSTR className);
HWND WINAPI AllocateLayeredWindow(LPCWSTR className);
void WINAPI ReportSystemEvent(HWND hwnd, int eventId);
void WINAPI MakeSound(LPCWSTR soundName);
void WINAPI MakeSoundFromFile(LPCWSTR soundName);
void WINAPI MakeSoundFromResource(HMODULE hModule, LPCWSTR soundName);
void WINAPI EmptyRecycleBin();
BOOL WINAPI IsTopMostWindow(HWND handle);
void WINAPI SetTopMostWindow(HWND handle, BOOL value);
void WINAPI SetBottomMostWindow(HWND handle, BOOL value);
void WINAPI WindowInsertAfter(HWND hwnd, HWND hwndInsertAfter);
void WINAPI LockForegroundWindow();
void WINAPI UnlockForegroundWindow();
void WINAPI BroadcastApplicationMessage(UINT Msg);
void WINAPI AddRemoveMessageFilter(UINT message, DWORD dwFlags);
LRESULT CALLBACK LayeredWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);
void WINAPI DrawLayeredWindow(HWND handle, int left, int top, int width, int height, HDC buffer, COLORREF colorKey, byte alpha, BOOL redrawOnly);
void WINAPI ShowDesktop();

BOOL WINAPI GetStartup(LPCWSTR appName);
void WINAPI SetStartup(LPCWSTR appName, LPCWSTR appPath);
void WINAPI RemoveStartup(LPCWSTR appName);




#ifdef __cplusplus
}
#endif

