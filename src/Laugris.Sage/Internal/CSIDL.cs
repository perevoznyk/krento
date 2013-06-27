using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    // Used to retrieve directory paths to system special folders
    internal enum CSIDL
    {
        ADMINTOOLS = 0x30,
        ALTSTARTUP = 0x1d,
        APPDATA = 0x1a,
        BITBUCKET = 10,
        CDBURN_AREA = 0x3b,
        COMMON_ADMINTOOLS = 0x2f,
        COMMON_ALTSTARTUP = 30,
        COMMON_APPDATA = 0x23,
        COMMON_DESKTOPDIRECTORY = 0x19,
        COMMON_DOCUMENTS = 0x2e,
        COMMON_FAVORITES = 0x1f,
        COMMON_MUSIC = 0x35,
        COMMON_PICTURES = 0x36,
        COMMON_PROGRAMS = 0x17,
        COMMON_STARTMENU = 0x16,
        COMMON_STARTUP = 0x18,
        COMMON_TEMPLATES = 0x2d,
        COMMON_VIDEO = 0x37,
        CONTROLS = 3,
        COOKIES = 0x21,
        DESKTOP = 0,
        DESKTOPDIRECTORY = 0x10,
        DRIVES = 0x11,
        FAVORITES = 6,
        FLAG_CREATE = 0x8000,
        FONTS = 20,
        HISTORY = 0x22,
        INTERNET = 1,
        INTERNET_CACHE = 0x20,
        LOCAL_APPDATA = 0x1c,
        MYDOCUMENTS = 5, // == to PERSONAL
        MYMUSIC = 13,
        MYPICTURES = 0x27,
        MYVIDEO = 14,
        NETHOOD = 0x13,
        NETWORK = 0x12,
        PERSONAL = 5,
        PRINTERS = 4,
        PRINTHOOD = 0x1b,
        PROFILE = 40,
        PROFILES = 0x3e,
        PROGRAM_FILES = 0x26,
        PROGRAM_FILES_COMMON = 0x2b,
        PROGRAMS = 2,
        RECENT = 8,
        SENDTO = 9,
        STARTMENU = 11,
        STARTUP = 7,
        SYSTEM = 0x25,
        TEMPLATES = 0x15,
        WINDOWS = 0x24
    }
}
