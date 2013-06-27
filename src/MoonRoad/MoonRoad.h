#pragma once

#if !defined( _MOONROAD_H_ )
#define _MOONROAD_H_

#ifdef __cplusplus
extern "C" {
#endif

#define wEOL	L'\0'

#define CLSIDSize 45		//size required for CLSID

#define CK_SHIFT	65536
#define CK_MENU		262144
#define CK_CONTROL	131072

HWND WINAPI AllocateLaugrisWindow();
HWND WINAPI AllocateKarnaWindow();

HWND WINAPI GetMainWindow(BYTE applicationNumber);
void WINAPI SetMainWindow(HWND window, BYTE applicationNumber);
BOOL WINAPI StartEngine(HWND handle, LPWSTR applicationName);
void WINAPI ExitManagedApplication(BYTE applicationNumber);
void WINAPI ExitKrento();
BOOL WINAPI PostMainMessage(BYTE applicationNumber, UINT Msg, WPARAM wParam, LPARAM lParam);
BOOL WINAPI SendMainMessage(BYTE applicationNumber, UINT Msg, WPARAM wParam, LPARAM lParam);
BOOL WINAPI SameText(LPCWSTR s1, LPCWSTR s2);


//Dock related
void WINAPI SetMouseDetected(BOOL value);
BOOL WINAPI GetMouseDetected();
void WINAPI GetDockRectangle(LPRECT rect);
void WINAPI SetDockRectangle(LPRECT rect);

BOOL WINAPI InstallMouseHook(HWND reportWindow);
void WINAPI RemoveMouseHook();
INT WINAPI InstallKeyboardHook(HWND reportWindow, UINT modifiers, UINT key);
void WINAPI RemoveKeyboardHook(HWND reportWindow, int id);
LRESULT WINAPI MouseHookProc(int nCode, WPARAM wParam, LPARAM lParam); 
void WINAPI PauseMouseHook();
void WINAPI ResumeMouseHook();
void WINAPI PauseIntercept();
void WINAPI ResumeIntercept();

void WINAPI SetClickTracking(BOOL value);
void WINAPI SetMoveTracking(BOOL value);
void WINAPI SetWheelTracking(BOOL value);
void WINAPI SetClickTrackingWheel(BOOL value);
void WINAPI SetClickTrackingXButton(BOOL value);
void WINAPI SetInterceptClick(BOOL value);
BOOL WINAPI GetClickTrackingXButton();
BOOL WINAPI GetClickTrackingWheel();
BOOL WINAPI GetClickTracking();
BOOL WINAPI GetWheelTracking();
BOOL WINAPI GetMoveTracking();
BOOL WINAPI GetInterceptClick();
BOOL WINAPI GetHookPaused();
INT  WINAPI WMessageBox(HWND hDlg, UINT uiStyle, WCHAR* pwszTitle, WCHAR* pwszFmt, ...);
void WINAPI ExtractArchive(LPWSTR archiveName, LPWSTR destination);
void WINAPI CreateBackup(LPWSTR archiveName, LPWSTR baseFolder);
BOOL WINAPI DesktopDblClick(POINT point);
void WINAPI SetDesktopClick(BOOL value);
BOOL WINAPI GetDesktopClick();



int CmpDate(const SYSTEMTIME *pst1, const SYSTEMTIME *pst2);
BOOL ValidCopy(LPWSTR applicationName);

BOOL WINAPI Matches(LPWSTR itemText, LPWSTR searchText);

#ifdef __cplusplus
}
#endif

#endif     // __MOONROAD_H__
