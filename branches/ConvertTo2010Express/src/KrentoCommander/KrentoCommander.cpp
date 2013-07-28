// KrentoCommander.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "KrentoCommander.h"
#include <shellapi.h>
#include <shlwapi.h>
#include <string.h>
#include <tchar.h>

// Global Variables:
HINSTANCE hInst;								// current instance

UINT KrentoShow;
UINT KrentoHide;
UINT KrentoAbout;
UINT KrentoOptions;
UINT KrentoClose;
UINT PulsarShow;
UINT PulsarHide;
UINT KrentoHelp;

UINT MessageId;

BOOL WINAPI IsWindows7()
{
	OSVERSIONINFO osver;
	osver.dwOSVersionInfoSize = sizeof(osver);
	if (GetVersionEx(&osver))
		return (osver.dwMajorVersion == 6 && osver.dwMinorVersion == 1)? TRUE: FALSE;
	else
		return FALSE;
}

int APIENTRY _tWinMain(HINSTANCE hInstance,
					   HINSTANCE hPrevInstance,
					   LPTSTR    lpCmdLine,
					   int       nCmdShow)
{
	
	UNREFERENCED_PARAMETER(hPrevInstance);

	TCHAR FullName[MAX_PATH] = {0};
	
    LPWSTR lpCmdLineW = GetCommandLine();
	_tcsncpy_s(FullName, MAX_PATH, lpCmdLineW, wcslen(lpCmdLineW)); 

	PathRemoveArgs(FullName);
	PathUnquoteSpaces(FullName);
	PathRemoveFileSpec(FullName);
	PathAddBackslash(FullName);
	PathAppend(FullName, L"Krento.exe");
	int cnt = 0;
	HANDLE mutex;
	
	LPTSTR KrentoName = FullName;

	DWORD dwRecipients = BSM_APPLICATIONS;
	BOOL Win7 = IsWindows7();

	if (lpCmdLine == NULL)
		return 0;

	if (wcslen(lpCmdLine) == 0)
		return 0;

	hInst = hInstance;

	KrentoShow = RegisterWindowMessage(L"krento_show");
	// allow our message to come in even if sent by lower privilege 
	// process
	if (Win7)
		ChangeWindowMessageFilter(KrentoShow, MSGFLT_ADD);

	KrentoHide = RegisterWindowMessage(L"krento_hide");
	if (Win7)
		ChangeWindowMessageFilter(KrentoHide, MSGFLT_ADD);

	KrentoAbout = RegisterWindowMessage(L"krento_about");
	if (Win7)
		ChangeWindowMessageFilter(KrentoAbout, MSGFLT_ADD);

	KrentoOptions = RegisterWindowMessage(L"krento_options");
	if (Win7)
		ChangeWindowMessageFilter(KrentoOptions, MSGFLT_ADD);

	KrentoClose = RegisterWindowMessage(L"krento_close");
	if (Win7)
		ChangeWindowMessageFilter(KrentoClose, MSGFLT_ADD);

	PulsarHide = RegisterWindowMessage(L"pulsar_hide");
	if (Win7)
		ChangeWindowMessageFilter(PulsarHide, MSGFLT_ADD);

	PulsarShow = RegisterWindowMessage(L"pulsar_show");
	if (Win7)
		ChangeWindowMessageFilter(PulsarShow, MSGFLT_ADD);

	KrentoHelp = RegisterWindowMessage(L"krento_help");
	if (Win7)
		ChangeWindowMessageFilter(KrentoHelp, MSGFLT_ADD);


	MessageId = 0;

	if (wcsstr(lpCmdLine, L"-krento_show") != NULL
		|| wcsstr(lpCmdLine, L"/krento_show") != NULL)
	{
		MessageId = KrentoShow;
	}

	if (wcsstr(lpCmdLine, L"-krento_hide") != NULL
		|| wcsstr(lpCmdLine, L"/krento_hide") != NULL)
	{
		MessageId = KrentoHide;
	}

	if (wcsstr(lpCmdLine, L"-krento_about") != NULL
		|| wcsstr(lpCmdLine, L"/krento_about") != NULL)
	{
		MessageId = KrentoAbout;
	}

	if (wcsstr(lpCmdLine, L"-krento_options") != NULL
		|| wcsstr(lpCmdLine, L"/krento_options") != NULL)
	{
		MessageId = KrentoOptions;
	}

	if (wcsstr(lpCmdLine, L"-krento_close") != NULL
		|| wcsstr(lpCmdLine, L"/krento_close") != NULL)
	{
		MessageId = KrentoClose;
	}

	if (wcsstr(lpCmdLine, L"-pulsar_show") != NULL
		|| wcsstr(lpCmdLine, L"/pulsar_show") != NULL)
	{
		MessageId = PulsarShow;
	}

	if (wcsstr(lpCmdLine, L"-pulsar_hide") != NULL
		|| wcsstr(lpCmdLine, L"/pulsar_hide") != NULL)
	{
		MessageId = PulsarHide;
	}

	if (wcsstr(lpCmdLine, L"-krento_help") != NULL
		|| wcsstr(lpCmdLine, L"/krento_help") != NULL)
	{
		MessageId = KrentoHelp;
	}

	if (MessageId != 0)
		BroadcastSystemMessage(BSF_IGNORECURRENTTASK | BSF_POSTMESSAGE, &dwRecipients, MessageId, 0, 0);

	if (wcsstr(lpCmdLine, L"-restart") != NULL
		|| wcsstr(lpCmdLine, L"/restart") != NULL)
	{
		MessageId = KrentoClose;
		BroadcastSystemMessage(BSF_IGNORECURRENTTASK | BSF_POSTMESSAGE, &dwRecipients, MessageId, 0, 0);
		Sleep(1000);
		while (cnt < 9)
		{
			mutex = OpenMutex(READ_CONTROL, FALSE, L"Krento");
			if (mutex != NULL)
			{
				CloseHandle(mutex);
				Sleep(1000);
			}
			else
				break;
			cnt++;
		}
		ShellExecute(NULL, L"open", KrentoName, NULL, NULL, SW_SHOWNORMAL);
	}

	return (int) 0;
}



