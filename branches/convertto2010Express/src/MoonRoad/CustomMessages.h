#pragma once

#ifdef __cplusplus
extern "C" {
#endif

//custom messages table
#define CM_BASE				0xB000
#define CM_MOUSEMOVE		CM_BASE + 1
#define CM_LBUTTONDOWN		CM_BASE + 2
#define CM_MBUTTONDOWN		CM_BASE + 3
#define CM_RBUTTONDOWN		CM_BASE + 4
#define CM_LBUTTONUP		CM_BASE + 5
#define CM_MBUTTONUP		CM_BASE + 6
#define CM_RBUTTONUP		CM_BASE + 7
#define CM_LBUTTONDBLCLK	CM_BASE + 8
#define CM_MBUTTONDBLCLK	CM_BASE + 9
#define CM_RBUTTONDBLCLK	CM_BASE + 10
#define CM_MOUSEWHEEL		CM_BASE + 11
#define CM_XBUTTONUP		CM_BASE + 12
#define CM_XBUTTONDOWN		CM_BASE + 13
#define CM_XBUTTONDBLCLK	CM_BASE + 14
#define CM_STARTENGINE		CM_BASE + 15
#define CM_DESKTOPCLICK     CM_BASE + 20

#ifdef __cplusplus
}
#endif

