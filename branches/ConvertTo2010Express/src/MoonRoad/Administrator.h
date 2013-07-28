#pragma once

#if !defined( _ADMINISTRATOR_H_ )
#define _ADMINISTRATOR_H_

#ifdef __cplusplus
extern "C" {
#endif

BOOL WINAPI IsElevatedAdministrator (HANDLE hInputToken);
BOOL WINAPI IsMemberOfAdministratorsGroup (HANDLE hInputToken);
BOOL WINAPI IsFullAdministrator();


#ifdef __cplusplus
}
#endif

#endif     // __ADMINISTRATOR_H__
