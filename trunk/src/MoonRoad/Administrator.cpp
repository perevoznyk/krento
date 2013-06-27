#include "stdafx.h"
#include <windows.h>
#include "Administrator.h"
#include "SystemInfo.h"

/*---------------------------------------------------------------------------
IsElevatedAdministrator (hInputToken)

Checks whether the access token belongs to a user account that is a member of 
the local Administrators group and is elevated.  

Parameters
   hInputToken [in, optional]
      A handle to an access token.  It must be opened with TOKEN_QUERY and 
      TOKEN_DUPLICATE access.  If this parameter is NULL, the token of the 
      calling thread is used if it is impersonating; otherwise, the token of 
      the calling process is used.

Return Value
   Returns TRUE if the token belongs to a user who is a member of the local 
   Administrators group and is elevated.  Returns FALSE otherwise.

Exceptions Thrown
   If this function fails, it throws a C++ DWORD exception that contains 
   the Win32 error code of the failure.  For example, if hInputToken is an 
   invalid handle, the error code will be ERROR_INVALID_HANDLE.
---------------------------------------------------------------------------*/
BOOL WINAPI IsElevatedAdministrator (HANDLE hInputToken)
{
   BOOL fIsAdmin = FALSE;
   HANDLE hTokenToCheck = NULL;
   DWORD  lastErr;
   
   // If the caller supplies a token, duplicate it as an impersonation token, 
   // because CheckTokenMembership requires an impersonation token.
   if (hInputToken)
   {
      if (!DuplicateToken (hInputToken, SecurityIdentification, 
                           &hTokenToCheck))
      {
         lastErr = GetLastError();
         goto CLEANUP;
      }
   }
 
   DWORD sidLen = SECURITY_MAX_SID_SIZE;
   BYTE localAdminsGroupSid[SECURITY_MAX_SID_SIZE];
 
   if (!CreateWellKnownSid (WinBuiltinAdministratorsSid, NULL, 
                            localAdminsGroupSid, &sidLen))
   {
      lastErr = GetLastError();
      goto CLEANUP;
   }
 
   // Now, determine whether the user is an administrator.
   if (CheckTokenMembership (hTokenToCheck, localAdminsGroupSid, &fIsAdmin))
   {
      lastErr = ERROR_SUCCESS;
   }
   else
   {
      lastErr = GetLastError();
   }
 
CLEANUP:
   // Close the impersonation token only if we opened it.
   if (hTokenToCheck)
   {
      CloseHandle (hTokenToCheck);
      hTokenToCheck = NULL;
   }
 
   if (ERROR_SUCCESS != lastErr)
   {
      return FALSE;
   }
   
   return (fIsAdmin);
}


/*---------------------------------------------------------------------------
IsMemberOfAdministratorsGroup (hInputToken)

Checks whether the access token belongs to a user account that is a member of 
the local Administrators group even if it is not currently elevated.  

Parameters
   hInputToken [in, optional]
      A handle to an access token.  It must be opened with TOKEN_QUERY and 
      TOKEN_DUPLICATE access.  If this parameter is NULL, the token of the 
      calling thread is used if it is impersonating; otherwise, the token of 
      the calling process is used.

Return Value
   Returns TRUE if the token belongs to a user who is a member of the local 
   Administrators group.  Returns FALSE otherwise.

Exceptions Thrown
   If this function fails, it throws a C++ DWORD exception that contains 
   the Win32 error code of the failure.  For example, if hInputToken is an 
   invalid handle, the error code will be ERROR_INVALID_HANDLE.
---------------------------------------------------------------------------*/
BOOL WINAPI IsMemberOfAdministratorsGroup (HANDLE hInputToken)
{
   BOOL fIsAdmin = FALSE;
   HANDLE hTokenToCheck = NULL;
   HANDLE hToken = hInputToken;
   DWORD lastErr;
 
   // If the caller didn't supply a token, open the current thread's token 
   // (if present) or the token of the current process otherwise.
   if (!hToken)
   {
      if (!OpenThreadToken (GetCurrentThread(), TOKEN_QUERY|TOKEN_DUPLICATE, 
                            TRUE, &hToken))
      {
         if (!OpenProcessToken(GetCurrentProcess(), 
                               TOKEN_QUERY|TOKEN_DUPLICATE, &hToken))
         {
            lastErr = GetLastError();
            goto CLEANUP;
         }
      }
   }
 
   /*
      Determine whether the system is running Windows Vista or later 
      (major version >= 6) because they support linked tokens, but previous 
      versions do not.  If running Windows Vista or later and the token is a
      limited token, get its linked token and check it.  Otherwise, just 
      check the token we have.
   */
   OSVERSIONINFO osver;
   osver.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
   if (!GetVersionEx (&osver))
   {
      lastErr = GetLastError();
      goto CLEANUP;
   }
 
   if (osver.dwMajorVersion >= 6)
   {
      TOKEN_ELEVATION_TYPE elevType;
      DWORD cbSize;
      if (!GetTokenInformation (hToken, TokenElevationType, &elevType, 
                                sizeof(TOKEN_ELEVATION_TYPE), &cbSize))
      {
         lastErr = GetLastError();
         goto CLEANUP;
      }
 
      if (TokenElevationTypeLimited == elevType)
      {
         if (!GetTokenInformation (hToken, TokenLinkedToken, &hTokenToCheck, 
                                   sizeof(HANDLE), &cbSize))
         {
            lastErr = GetLastError();
            goto CLEANUP;
         }
      }
   }
 
   /*
      CheckTokenMembership requires an impersonation token. If we just got a 
      linked token, it already is an impersonation token.  If we didn't get a 
      linked token, duplicate the original as an impersonation token for 
      CheckTokenMembership.
   */
   if (!hTokenToCheck && !DuplicateToken (hToken, SecurityIdentification, 
                                          &hTokenToCheck))
   {
      lastErr = GetLastError();
      goto CLEANUP;
   }
 
   DWORD sidLen = SECURITY_MAX_SID_SIZE;
   BYTE localAdminsGroupSid[SECURITY_MAX_SID_SIZE];
   if (!CreateWellKnownSid (WinBuiltinAdministratorsSid, NULL,
                            localAdminsGroupSid, &sidLen))
   {
      lastErr = GetLastError();
      goto CLEANUP;
   }
 
   // Now, determine whether the user is an administrator.
   if (CheckTokenMembership (hTokenToCheck, localAdminsGroupSid, &fIsAdmin))
   {
      lastErr = ERROR_SUCCESS;
   }
   else
   {
      lastErr = GetLastError();
   }
 
CLEANUP:
   // Close the thread/process token handle only if we opened it.  We open 
   // a token handle only when a caller passes NULL in the hInputToken 
   // parameter.
   if (!hInputToken && hToken)
   {
      CloseHandle (hToken);
      hToken = NULL;  // Set variable to same state as resource.
   }
   if (hTokenToCheck)
   {
      CloseHandle (hTokenToCheck);
      hTokenToCheck = NULL;
   }
 
   if (ERROR_SUCCESS != lastErr)
   {
      return FALSE;
   }
 
   return (fIsAdmin);
}


BOOL WINAPI IsFullAdministrator()
{
	if (IsMemberOfAdministratorsGroup (NULL))
	{
		if (IsWindowsVista())
		{
			return IsElevatedAdministrator(NULL);
		}
		else
			return TRUE;
	}
	else
		return FALSE;
}

