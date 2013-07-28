// DGraphics.cpp : Defines the exported functions for the DLL application.
//

#include <windows.h>
#include "DGraphics.h"

GpImage *MyBitmap::GetNativeImage()
{
	return nativeImage;
}

void WINAPI About()
{
}

Bitmap *WINAPI LoadGDIPlusImage(const WCHAR *szImage)
{
	try
	{
		if (szImage == NULL)
			return NULL;
		else
		{
			return Bitmap::FromFile(szImage);
		}
	}
	catch(...)
	{
		return NULL;
	}
}

GpImage *WINAPI ExtractNativeImage(Image *image)
{
	try
	{
		if (image == NULL)
			return NULL;
		else
		{
			MyBitmap *bmp = (MyBitmap *)(image);
			return bmp->GetNativeImage();
		}
	}
	catch(...)
	{
		return NULL;
	}
}

void WINAPI DeleteImage(Image *image)
{
	if (image == NULL)
		return;
	else
	   delete image;
}
