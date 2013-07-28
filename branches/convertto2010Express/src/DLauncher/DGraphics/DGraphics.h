#pragma once

#include <Gdiplus.h>
using namespace Gdiplus;
#pragma comment(lib, "gdiplus.lib")

#if !defined( _DGRAPHICS_H_ )
#define _DGRAPHICS_H_

#ifdef __cplusplus
extern "C" {
#endif

class MyBitmap : public Image
{
public:
    friend class Image;
	GpImage * GetNativeImage();
};


void WINAPI About();
Bitmap *WINAPI LoadGDIPlusImage(const WCHAR *szImage);
GpImage *WINAPI ExtractNativeImage(Image *image); 
void WINAPI DeleteImage(Image *image);

#ifdef __cplusplus
}
#endif

#endif    
