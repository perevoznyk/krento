#include "stdafx.h"
#include <tchar.h>
#include "Graphics.h"
#include "zlib.h"
#include "png.h"
#include "pnginfo.h"
#include "pngstruct.h"
#include "jpeglib.h"
#include <algorithm>
#include "FileOperations.h"


SIZE WINAPI GetTextSize(LPCWSTR s, HFONT hFont, UINT flags)
{
	HDC dc;
	HGDIOBJ saveFont;
	RECT rc = {0};
	SIZE size;

	dc = GetDC(0);
	saveFont = SelectObject(dc, hFont);

	int textLen = (int)wcslen(s);
	flags = (flags & ~DT_WORDBREAK) | DT_CALCRECT ;
	int result = DrawText(dc, s, textLen, &rc, flags);

	SelectObject(dc, saveFont);
	ReleaseDC(0, dc);

	size.cx = rc.right;
	size.cy = rc.bottom;
	return size;
}

int WINAPI EmToPixels(int em)
{
	HDC dc = GetDC(0);
	int pt = MulDiv(em, 72, 96);
	return MulDiv(pt, GetDeviceCaps(dc, LOGPIXELSY), 96); 
	ReleaseDC(0, dc);
}

int WINAPI DpiY()
{
	HDC dc = GetDC(0);
	return GetDeviceCaps(dc, LOGPIXELSY);
	ReleaseDC(0, dc);
}

void WINAPI AlphaBlendNative(HDC hdcDest,
							 int xoriginDest,
							 int yoriginDest,
							 int wDest,
							 int hDest,
							 HDC hdcSrc,
							 int xoriginSrc,
							 int yoriginSrc,
							 int wSrc,
							 int hSrc,
							 BYTE alpha)

{
	BLENDFUNCTION BlendFunction = {0};

	BlendFunction.BlendOp = AC_SRC_OVER;
	BlendFunction.BlendFlags = 0;
	BlendFunction.SourceConstantAlpha = alpha;
	BlendFunction.AlphaFormat = AC_SRC_ALPHA;

	AlphaBlend(hdcDest, xoriginDest, yoriginDest, wDest, hDest, hdcSrc, xoriginSrc, yoriginSrc, wSrc, hSrc, BlendFunction);
}

SIZE WINAPI GetTextSizeEx(LPCWSTR s, HFONT hFont, UINT proposedWidth, UINT flags, BOOL margins)
{
	HDC dc;
	HGDIOBJ saveFont;
	RECT rc = {0};
	SIZE size;
	TEXTMETRIC tm = {0};

	dc = GetDC(0);
	saveFont = SelectObject(dc, hFont);

	int textLen = lstrlen(s);
	rc.right = proposedWidth;
	rc.bottom = 1;
	DrawText(dc, s, textLen, &rc, flags | DT_CALCRECT);

	size.cx = rc.right;
	size.cy = rc.bottom;

	if (margins)
	{
		GetTextMetrics(dc, &tm);
		if ((TMPF_TRUETYPE &  tm.tmPitchAndFamily) == TMPF_TRUETYPE)
		{
			if (tm.tmItalic > 0)
				tm.tmOverhang = (tm.tmAscent + 1) >> 2;
		}
		if (tm.tmOverhang == 0)
			tm.tmOverhang = (int)(tm.tmHeight / 6.0f);
		int extraMargins = (int)(tm.tmOverhang * 1.5f);
		size.cx += (extraMargins);
	}

	SelectObject(dc, saveFont);
	ReleaseDC(0, dc);

	return size;
}

SIZE WINAPI GetTextLineSize(LPCWSTR s, HFONT hFont)
{
	SIZE size = {0};
	TEXTMETRIC tm = {0};
	HDC dc = GetDC(0);
	HGDIOBJ oldFont = SelectObject(dc, hFont);
	GetTextExtentPoint32(dc, s, lstrlen(s), &size);
	GetTextMetrics(dc, &tm);
	SelectObject(dc, oldFont);
	ReleaseDC(0, dc);

	if ((TMPF_TRUETYPE &  tm.tmPitchAndFamily) == TMPF_TRUETYPE)
	{
		if (tm.tmItalic > 0)
			tm.tmOverhang = (tm.tmAscent + 1) >> 2;
	}
	size.cx += tm.tmOverhang;
	return size;
}

void WINAPI DrawTextDirect(HDC hDC, LPCWSTR s, HFONT hFont, COLORREF color, int left, int top)
{
	SelectObject(hDC, hFont);
	INT oldMode = SetBkMode(hDC, TRANSPARENT);
	SetTextColor(hDC, color);
	TextOut(hDC, left, top, s, lstrlen(s));
	SetBkMode(hDC, oldMode);
}

void WINAPI DrawTextDirectEx(HDC hDC, LPCWSTR s, HFONT hFont, COLORREF color, COLORREF background, int left, int top)
{
	SelectObject(hDC, hFont);
	SetTextColor(hDC, color);
	SetBkColor(hDC, background);
	TextOut(hDC, left, top, s, lstrlen(s));
}

void WINAPI DrawTextRect(HDC hDC, LPCWSTR s, HFONT hFont, COLORREF color, COLORREF background, LPRECT lpRect, UINT flags)
{
	SelectObject(hDC, hFont);
	SetTextColor(hDC, color);
	SetBkColor(hDC, background);
	DrawText(hDC, s, lstrlen(s), lpRect, flags);
}

void WINAPI DrawTextLine(HDC hDC, LPCWSTR s, HFONT hFont, COLORREF color, int left, int top)
{
	SIZE size = {0};
	TEXTMETRIC tm = {0};
	HDC dc = GetDC(0);
	HGDIOBJ oldFont = SelectObject(dc, hFont);
	GetTextExtentPoint32(dc, s, lstrlen(s), &size);
	GetTextMetrics(dc, &tm);
	SelectObject(dc, oldFont);
	ReleaseDC(0, dc);

	if ((TMPF_TRUETYPE &  tm.tmPitchAndFamily) == TMPF_TRUETYPE)
	{
		if (tm.tmItalic > 0)
			tm.tmOverhang = (tm.tmAscent + 1) >> 2;
	}
	size.cx += tm.tmOverhang;

	RECT rect = {0};
	rect.left = left;
	rect.top = top;
	rect.right = rect.left + size.cx;
	rect.bottom = rect.top + size.cy;

	DrawAlphaTextRect(hDC, s, hFont, color, &rect, DT_SINGLELINE | DT_LEFT | DT_VCENTER |  DT_NOPREFIX);
}


void WINAPI DrawAlphaText(HDC hDC, LPCWSTR s, HFONT hFont, COLORREF color, int width, int height, UINT flags)
{
	RECT rect = {0};
	rect.right = width;
	rect.bottom = height;
	DrawAlphaTextRect(hDC, s, hFont, color, &rect, flags);
}

void WINAPI DrawAlphaTextRect(HDC hDC, LPCWSTR s, HFONT hFont, COLORREF color, LPRECT lpRect, UINT flags)
{

	if (lpRect == NULL)
		return;

	int width = lpRect->right - lpRect->left;
	int height = lpRect->bottom - lpRect->top;

	RECT drawRect = {0};
	drawRect.right = width;
	drawRect.bottom = height;
	
	DWORD pixelBlue;
	DWORD pixelGreen;
	DWORD pixelRed;
	DWORD pixelAlpha;
	DWORD BackgroundPixel;

	HGDIOBJ oldFont;

	LPDWORD dataBack;
	LPDWORD dataText;


	int pixels = width * height;

	BITMAPINFO bmBack;
	ZeroMemory(&bmBack.bmiHeader, sizeof(BITMAPINFOHEADER));
	bmBack.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	bmBack.bmiHeader.biWidth = width;
	bmBack.bmiHeader.biHeight = -height;  // Use a top-down DIB
	bmBack.bmiHeader.biPlanes = 1;
	bmBack.bmiHeader.biBitCount = 32;


	BITMAPINFO bmText;
	ZeroMemory(&bmText.bmiHeader, sizeof(BITMAPINFOHEADER));
	bmText.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	bmText.bmiHeader.biWidth = width;
	bmText.bmiHeader.biHeight = -height;  // Use a top-down DIB
	bmText.bmiHeader.biPlanes = 1;
	bmText.bmiHeader.biBitCount = 32;

	HDC hdcBack = CreateCompatibleDC(NULL);
	HDC hdcText = CreateCompatibleDC(NULL);

	HBITMAP bmpBack = CreateDIBSection(hdcBack, &bmBack, DIB_RGB_COLORS, (LPVOID *)&dataBack, NULL, 0);
	HBITMAP bmpText = CreateDIBSection(hdcText, &bmText, DIB_RGB_COLORS, (LPVOID *)&dataText, NULL, 0);

	SelectObject(hdcBack, bmpBack);
	SelectObject(hdcText, bmpText);

	memset(dataText, 0, pixels * 4);
	memset(dataBack, 0, pixels * 4);
	oldFont = SelectObject(hdcText, hFont);
	SetBkMode(hdcText, TRANSPARENT);
	SetTextColor(hdcText, 0x00FFFFFF);
	DrawText(hdcText, s, lstrlen(s), &drawRect, flags);
	SelectObject(hdcText, oldFont);

	DWORD textRed = (color) & 0xFF;
	DWORD textGreen = (color >> 8) & 0xFF;
	DWORD textBlue = (color >> 16) & 0xFF;

	for (int i=0; i < pixels; i++)
	{
		// take the blue channel of the white text as alpha channel
		DWORD AlphaChannel = dataText[i] & 0xFF;
		if (!AlphaChannel)
			continue; // At this pixel there has no text been printed

		pixelRed = (AlphaChannel * textRed) / 255;
		pixelGreen = (AlphaChannel * textGreen) / 255;
		pixelBlue = (AlphaChannel * textBlue) / 255;
		pixelAlpha =  AlphaChannel;

		BackgroundPixel = pixelBlue | (pixelGreen << 8) | (pixelRed << 16) | (pixelAlpha << 24);
		dataBack[i] = BackgroundPixel;
	}

	BLENDFUNCTION bf = {};
	bf.AlphaFormat = AC_SRC_ALPHA;
	bf.BlendFlags = 0;
	bf.BlendOp = AC_SRC_OVER;
	bf.SourceConstantAlpha = 0xFF;

	AlphaBlend(hDC, lpRect->left, lpRect->top, width, height, hdcBack, 0, 0, width, height, bf);

	DeleteDC(hdcBack);
	DeleteDC(hdcText);

	DeleteObject(bmpText);
	DeleteObject(bmpBack);
}

void WINAPI DrawTextOutline(HDC hDC, LPCWSTR s, HFONT hFont, COLORREF foreColor, COLORREF backColor, int left, int top)
{
	SIZE size = GetTextLineSize(s, hFont);
	size.cx += 6;
	size.cy += 2;

	int width = size.cx;
	int height = size.cy;

	RECT drawRect = {0};
	drawRect.right = width;
	drawRect.bottom = height;
	
	DWORD pixelBlue;
	DWORD pixelGreen;
	DWORD pixelRed;
	DWORD pixelAlpha;
	DWORD BackgroundPixel;

	DWORD textRed;
	DWORD textGreen;
	DWORD textBlue;

	HGDIOBJ oldFont;

	LPDWORD dataBack;
	LPDWORD dataText;


	int pixels = width * height;

	BITMAPINFO bmBack;
	ZeroMemory(&bmBack.bmiHeader, sizeof(BITMAPINFOHEADER));
	bmBack.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	bmBack.bmiHeader.biWidth = width;
	bmBack.bmiHeader.biHeight = -height;  // Use a top-down DIB
	bmBack.bmiHeader.biPlanes = 1;
	bmBack.bmiHeader.biBitCount = 32;


	BITMAPINFO bmText;
	ZeroMemory(&bmText.bmiHeader, sizeof(BITMAPINFOHEADER));
	bmText.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	bmText.bmiHeader.biWidth = width;
	bmText.bmiHeader.biHeight = -height;  // Use a top-down DIB
	bmText.bmiHeader.biPlanes = 1;
	bmText.bmiHeader.biBitCount = 32;

	HDC hdcBack = CreateCompatibleDC(NULL);
	HDC hdcText = CreateCompatibleDC(NULL);

	HBITMAP bmpBack = CreateDIBSection(hdcBack, &bmBack, DIB_RGB_COLORS, (LPVOID *)&dataBack, NULL, 0);
	HBITMAP bmpText = CreateDIBSection(hdcText, &bmText, DIB_RGB_COLORS, (LPVOID *)&dataText, NULL, 0);

	SelectObject(hdcBack, bmpBack);
	SelectObject(hdcText, bmpText);

	memset(dataBack, 0, pixels * 4);
	memset(dataText, 0, pixels * 4);
	oldFont = SelectObject(hdcText, hFont);
	SetBkMode(hdcText, TRANSPARENT);
	SetTextColor(hdcText, 0x00FFFFFF);

	textRed = (backColor) & 0xFF;
	textGreen = (backColor >> 8) & 0xFF;
	textBlue = (backColor >> 16) & 0xFF;

	TextOut(hdcText, 4, 2, s, lstrlen(s));
	TextOut(hdcText, 4, 0, s, lstrlen(s));
	TextOut(hdcText, 6, 2, s, lstrlen(s));
	TextOut(hdcText, 6, 0, s, lstrlen(s));

	for (int i=0; i < pixels; i++)
	{
		// take the blue channel of the white text as alpha channel
		DWORD AlphaChannel = dataText[i] & 0xFF;
		if (!AlphaChannel)
			continue; // At this pixel there has no text been printed

		pixelRed = (AlphaChannel * textRed) / 255;
		pixelGreen = (AlphaChannel * textGreen) / 255;
		pixelBlue = (AlphaChannel * textBlue) / 255;
		pixelAlpha =  AlphaChannel;

		BackgroundPixel = pixelBlue | (pixelGreen << 8) | (pixelRed << 16) | (pixelAlpha << 24);
		dataBack[i] = BackgroundPixel;
	}


	textRed = (foreColor) & 0xFF;
	textGreen = (foreColor >> 8) & 0xFF;
	textBlue = (foreColor >> 16) & 0xFF;

	memset(dataText, 0, pixels * 4);
	TextOut(hdcText, 5, 1, s, lstrlen(s));

	for (int i=0; i < pixels; i++)
	{
		// take the blue channel of the white text as alpha channel
		DWORD AlphaChannel = dataText[i] & 0xFF;
		if (!AlphaChannel)
			continue; // At this pixel there has no text been printed

		pixelRed = (AlphaChannel * textRed) / 255;
		pixelGreen = (AlphaChannel * textGreen) / 255;
		pixelBlue = (AlphaChannel * textBlue) / 255;
		if (AlphaChannel < 255)
			pixelAlpha =  (dataBack[i] >> 24) & 0xFF;
		else
			pixelAlpha = AlphaChannel;

		BackgroundPixel = pixelBlue | (pixelGreen << 8) | (pixelRed << 16) | (pixelAlpha << 24);
		dataBack[i] = BackgroundPixel;
	}


	SelectObject(hdcText, oldFont);

	BLENDFUNCTION bf = {};
	bf.AlphaFormat = AC_SRC_ALPHA;
	bf.BlendFlags = 0;
	bf.BlendOp = AC_SRC_OVER;
	bf.SourceConstantAlpha = 0xFF;

	AlphaBlend(hDC, left, top, width, height, hdcBack, 0, 0, width, height, bf);

	DeleteDC(hdcBack);
	DeleteDC(hdcText);

	DeleteObject(bmpText);
	DeleteObject(bmpBack);

}



void WINAPI DrawTextGlow(HDC hDC, LPCWSTR s, HFONT hFont, COLORREF foreColor, int left, int top)
{
	SIZE size = GetTextLineSize(s, hFont);
	size.cx += 8;
	size.cy += 8;

	int width = size.cx;
	int height = size.cy;

	RECT drawRect = {0};
	drawRect.right = width;
	drawRect.bottom = height;
	
	DWORD pixelBlue;
	DWORD pixelGreen;
	DWORD pixelRed;
	DWORD pixelAlpha;
	DWORD BackgroundPixel;

	DWORD textRed;
	DWORD textGreen;
	DWORD textBlue;

	HGDIOBJ oldFont;

	LPDWORD dataBack;
	LPDWORD dataText;


	int pixels = width * height;

	BITMAPINFO bmBack;
	ZeroMemory(&bmBack.bmiHeader, sizeof(BITMAPINFOHEADER));
	bmBack.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	bmBack.bmiHeader.biWidth = width;
	bmBack.bmiHeader.biHeight = -height;  // Use a top-down DIB
	bmBack.bmiHeader.biPlanes = 1;
	bmBack.bmiHeader.biBitCount = 32;


	BITMAPINFO bmText;
	ZeroMemory(&bmText.bmiHeader, sizeof(BITMAPINFOHEADER));
	bmText.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	bmText.bmiHeader.biWidth = width;
	bmText.bmiHeader.biHeight = -height;  // Use a top-down DIB
	bmText.bmiHeader.biPlanes = 1;
	bmText.bmiHeader.biBitCount = 32;

	HDC hdcBack = CreateCompatibleDC(NULL);
	HDC hdcText = CreateCompatibleDC(NULL);

	HBITMAP bmpBack = CreateDIBSection(hdcBack, &bmBack, DIB_RGB_COLORS, (LPVOID *)&dataBack, NULL, 0);
	HBITMAP bmpText = CreateDIBSection(hdcText, &bmText, DIB_RGB_COLORS, (LPVOID *)&dataText, NULL, 0);

	SelectObject(hdcBack, bmpBack);
	SelectObject(hdcText, bmpText);

	memset(dataBack, 0, pixels * 4);
	memset(dataText, 0, pixels * 4);
	oldFont = SelectObject(hdcText, hFont);
	SetBkMode(hdcText, TRANSPARENT);
	SetTextColor(hdcText, 0x00FFFFFF);

	textRed = (foreColor) & 0xFF;
	textGreen = (foreColor >> 8) & 0xFF;
	textBlue = (foreColor >> 16) & 0xFF;

	int x = 4;
	int y = 4;

	TextOut(hdcText, x - 4, y - 4, s, lstrlen(s));
	TextOut(hdcText, x, y - 4, s, lstrlen(s));
	TextOut(hdcText, x + 4, y - 4, s, lstrlen(s));
	TextOut(hdcText, x - 4 , y, s, lstrlen(s));
	TextOut(hdcText, x + 4, y, s, lstrlen(s));
	TextOut(hdcText, x - 4, y + 4, s, lstrlen(s));
	TextOut(hdcText, x, y + 4, s, lstrlen(s));
	TextOut(hdcText, x + 4, y + 4, s, lstrlen(s));


	for (int i=0; i < pixels; i++)
	{
		// take the blue channel of the white text as alpha channel
		DWORD AlphaChannel = dataText[i] & 0xFF;
		if (!AlphaChannel)
			continue; // At this pixel there has no text been printed

		pixelRed = textRed;
		pixelGreen = textGreen;
		pixelBlue = textBlue;
		pixelAlpha =  20;

		BackgroundPixel = pixelBlue | (pixelGreen << 8) | (pixelRed << 16) | (pixelAlpha << 24);
		dataBack[i] = BackgroundPixel;
	}


	memset(dataText, 0, pixels * 4);

	TextOut(hdcText, x - 1, y, s, lstrlen(s));
	TextOut(hdcText, x + 1, y, s, lstrlen(s));
	TextOut(hdcText, x, y - 1, s, lstrlen(s));
	TextOut(hdcText, x, y + 1, s, lstrlen(s));

	for (int i=0; i < pixels; i++)
	{
		// take the blue channel of the white text as alpha channel
		DWORD AlphaChannel = dataText[i] & 0xFF;
		if (!AlphaChannel)
			continue; // At this pixel there has no text been printed

		pixelRed = 0;
		pixelGreen = 0;
		pixelBlue = 0;
		pixelAlpha =  AlphaChannel;

		BackgroundPixel = pixelBlue | (pixelGreen << 8) | (pixelRed << 16) | (pixelAlpha << 24);
		dataBack[i] = BackgroundPixel;
	}

	memset(dataText, 0, pixels * 4);
	TextOut(hdcText, 4, 4, s, lstrlen(s));

	for (int i=0; i < pixels; i++)
	{
		// take the blue channel of the white text as alpha channel
		DWORD AlphaChannel = dataText[i] & 0xFF;
		if (!AlphaChannel)
			continue; // At this pixel there has no text been printed

		pixelRed = (AlphaChannel * textRed) / 255;
		pixelGreen = (AlphaChannel * textGreen) / 255;
		pixelBlue = (AlphaChannel * textBlue) / 255;
		if (AlphaChannel < 255)
			pixelAlpha =  (dataBack[i] >> 24) & 0xFF;
		else
			pixelAlpha = AlphaChannel;

		BackgroundPixel = pixelBlue | (pixelGreen << 8) | (pixelRed << 16) | (pixelAlpha << 24);
		dataBack[i] = BackgroundPixel;
	}



	SelectObject(hdcText, oldFont);

	BLENDFUNCTION bf = {};
	bf.AlphaFormat = AC_SRC_ALPHA;
	bf.BlendFlags = 0;
	bf.BlendOp = AC_SRC_OVER;
	bf.SourceConstantAlpha = 0xFF;

	AlphaBlend(hDC, left-4, top-4, width, height, hdcBack, 0, 0, width, height, bf);

	DeleteDC(hdcBack);
	DeleteDC(hdcText);

	DeleteObject(bmpText);
	DeleteObject(bmpBack);

}

HFONT WINAPI CreateWindowsFont(LPCWSTR fontFamily, INT size, INT fontStyle, INT fontQuality)
{
	HDC hdc = GetDC(NULL);

	LOGFONT lf = {0};
	lf.lfCharSet = DEFAULT_CHARSET;
	lf.lfOutPrecision = OUT_OUTLINE_PRECIS;
	lf.lfClipPrecision = CLIP_DEFAULT_PRECIS;
	lf.lfPitchAndFamily = VARIABLE_PITCH | FF_DONTCARE;
	if (fontFamily != NULL)
		_tcscpy_s(lf.lfFaceName, _countof(lf.lfFaceName), fontFamily);
	else
		_tcscpy_s(lf.lfFaceName, _countof(lf.lfFaceName), _T("Tahoma"));
	lf.lfHeight = -MulDiv(size, GetDeviceCaps(hdc, LOGPIXELSY), 72);
	lf.lfQuality = fontQuality;
	lf.lfItalic = (fontStyle & FONT_ITALIC) == FONT_ITALIC;
	lf.lfStrikeOut = (fontStyle & FONT_STRIKEOUT) == FONT_STRIKEOUT;
	lf.lfUnderline = (fontStyle & FONT_UNDERLINE) == FONT_UNDERLINE;
	if ( (fontStyle & FONT_BOLD) == FONT_BOLD)
		lf.lfWeight = FW_BOLD;
	else
		lf.lfWeight = FW_NORMAL;

	ReleaseDC(NULL, hdc);

	return CreateFontIndirect(&lf);

}

HFONT WINAPI CloneFont(HFONT hFont)
{
	LOGFONT lf = {0};
	GetObject(hFont, sizeof(LOGFONT), &lf);
	HFONT result = CreateFontIndirect(&lf);
	return result;
}

void WINAPI DestroyFont(HFONT hFont)
{
	if (hFont != NULL)
		DeleteObject(hFont);
}

HBITMAP WINAPI MakeCompatibleBitmap(HBITMAP source, int width, int height)
{
	HDC screenDC = GetDC(NULL);
	HDC memDC = CreateCompatibleDC(screenDC);
	HGDIOBJ oldBitmap = SelectObject(memDC, source);
	HDC dstDC = CreateCompatibleDC(screenDC);
	HBITMAP dstBitmap = CreateCompatibleBitmap(screenDC, width, height);
	HGDIOBJ oldDst = SelectObject(dstDC, dstBitmap);
	BitBlt(dstDC, 0, 0, width, height, memDC, 0, 0, SRCCOPY);
	SelectObject(memDC, oldBitmap);
	DeleteDC(memDC);
	SelectObject(dstDC, oldDst);
	DeleteDC(dstDC);
	ReleaseDC(0, screenDC);
	return dstBitmap;
}

void WINAPI DrawNativeBitmap(HBITMAP src, HBITMAP dst, int width, int height)
{
	HDC screenDC = GetDC(NULL);
	HDC srcDC = CreateCompatibleDC(screenDC);
	HDC dstDC = CreateCompatibleDC(screenDC);
	HGDIOBJ oldSrc = SelectObject(srcDC, src);
	HGDIOBJ oldDst = SelectObject(dstDC, dst);
	BitBlt(dstDC, 0, 0, width, height, srcDC, 0, 0, SRCCOPY);
	SelectObject(srcDC, oldSrc);
	SelectObject(dstDC, oldDst);


	DeleteDC(srcDC);
	DeleteDC(dstDC);
	ReleaseDC(0, screenDC);
}

void WINAPI CopyNativeBitmap(HBITMAP src, HDC dstDC, int width, int height, int left, int top)
{
	HDC screenDC = GetDC(NULL);
	HDC srcDC = CreateCompatibleDC(screenDC);
	HGDIOBJ oldSrc = SelectObject(srcDC, src);
	BitBlt(dstDC, left, top, width, height, srcDC, 0, 0, SRCCOPY);
	SelectObject(srcDC, oldSrc);
	DeleteDC(srcDC);
	ReleaseDC(0, screenDC);
}


BOOL WINAPI StretchNativeBitmap(HBITMAP src, HBITMAP dst, int srcWidth, int srcHeight, int dstWidth, int dstHeight)
{
	HDC screenDC = GetDC(NULL);
	HDC srcDC = CreateCompatibleDC(screenDC);
	HDC dstDC = CreateCompatibleDC(screenDC);
	HGDIOBJ oldSrc = SelectObject(srcDC, src);
	HGDIOBJ oldDst = SelectObject(dstDC, dst);
	SetStretchBltMode(dstDC, COLORONCOLOR);
	BOOL result = StretchBlt(dstDC, 0, 0, dstWidth, dstHeight, srcDC, 0, 0, srcWidth, srcHeight, SRCCOPY);
	SelectObject(srcDC, oldSrc);
	SelectObject(dstDC, oldDst);
	DeleteDC(srcDC);
	DeleteDC(dstDC);
	ReleaseDC(0, screenDC);
	return result;
}

void WINAPI AlphaBlendBitmap(HBITMAP src, HDC hdc, int left, int top, int width, int height, int alpha)
{
	BLENDFUNCTION BlendFunction = {0};
	HGDIOBJ oldSrc;
	HDC screenDC = GetDC(NULL);
	HDC srcDC = CreateCompatibleDC(screenDC);
	oldSrc = SelectObject(srcDC, src);

	BlendFunction.BlendOp = AC_SRC_OVER;
	BlendFunction.BlendFlags = 0;
	BlendFunction.SourceConstantAlpha = alpha;
	BlendFunction.AlphaFormat = AC_SRC_ALPHA;

	AlphaBlend(hdc, left, top, width, height, srcDC,
		0, 0, width, height, BlendFunction);

	SelectObject(srcDC, oldSrc);
	DeleteDC(srcDC);
	ReleaseDC(0, screenDC);
}

// prototypes, optionally connect these to whatever you log errors with

void PNGAPI user_error_fn(png_structp png, png_const_charp sz) 
{ 
	OutputDebugStringA(sz);
}
void PNGAPI user_warning_fn(png_structp png, png_const_charp sz) 
{ 
	OutputDebugStringA(sz);
}
 
BYTE* ExtractImageBuffer( HINSTANCE hInstance, LPCWSTR ImageID )
{
    BYTE* Buffer = NULL;
    HRSRC        hResInfo;
    HANDLE        hRes;
    LPSTR        lpRes    = NULL; 
    int            nLen    = 0;
    bool        bResult    = FALSE;
    // Find the resource
    hResInfo = FindResource(hInstance, ImageID, _T("PNG") );
    if (hResInfo == NULL) 
        return Buffer;
    // Load the resource
    hRes = LoadResource(hInstance, hResInfo);
    if (hRes == NULL) 
        return Buffer;
    // Lock the resource
    lpRes = (char*)LockResource(hRes);
    if (lpRes != NULL)
    { 
        int nBufSize = SizeofResource(hInstance, hResInfo);
            
        if(nBufSize >= (int)SizeofResource(hInstance, hResInfo))
        {
            Buffer = new BYTE[nBufSize];//the caller should delete
            memcpy(Buffer, lpRes, nBufSize);
        } 
        UnlockResource(hRes);  
    }
    // Free the resource
    FreeResource(hRes);
    return Buffer;
}

void __cdecl png_read_from_resource(png_structp png_struct, png_bytep pbyte, png_size_t size)
{  
   memcpy( pbyte, png_struct->io_ptr, size );
   
   BYTE* pb = (BYTE*)png_struct->io_ptr;
   pb += size;
   png_struct->io_ptr = pb;
}

HBITMAP WINAPI LoadPNGResource(HMODULE handle, LPCWSTR szName, LPINT lpiWidth, LPINT lpiHeight, __deref_opt_out void **ppvBits)
{
	double gamma;
	HBITMAP hbm = NULL;
    BITMAPINFO      bi;
    BYTE * pixelData;
	bool retVal = false;
	int size = 0;
	png_byte color_type;
	png_bytepp row_pointers;
	png_size_t rowbytes;

	
	void* stream = ExtractImageBuffer( handle, szName );
	if (stream == NULL)
		return NULL;

	// now allocate stuff
	png_structp png_ptr = 
		png_create_read_struct(PNG_LIBPNG_VER_STRING, 
		NULL, user_error_fn, user_warning_fn);
	
	if (!png_ptr)
	{
		delete [] stream;
		return NULL;
	}

	png_infop info_ptr = png_create_info_struct(png_ptr);
	if (!info_ptr)
	{
		png_destroy_read_struct(&png_ptr,
			(png_infopp)NULL, (png_infopp)NULL);
		delete [] stream;
		return NULL;
	}

	png_infop end_info = png_create_info_struct(png_ptr);
	if (!end_info)
	{
		png_destroy_read_struct(&png_ptr, &info_ptr,
			(png_infopp)NULL);
		delete [] stream;
		return NULL;
	}

	png_set_read_fn( png_ptr, stream, png_read_from_resource );


		if (png_ptr != NULL)
		{

			BOOL update = FALSE;

			png_read_info(png_ptr, info_ptr);

			int width = png_get_image_width(png_ptr, info_ptr);
			int height = png_get_image_height(png_ptr, info_ptr);
			int bpp = png_get_bit_depth(png_ptr, info_ptr);
			color_type = png_get_color_type(png_ptr, info_ptr);

			*lpiWidth = width;
			*lpiHeight = height;

			if (color_type == PNG_COLOR_TYPE_PALETTE)
			{
				png_set_palette_to_rgb(png_ptr);
				update = TRUE;
			}
			if (png_get_valid(png_ptr, info_ptr, PNG_INFO_tRNS))
			{
				png_set_tRNS_to_alpha(png_ptr);
				update = TRUE;
			}
			if (bpp == 16)
			{
				png_set_strip_16(png_ptr);
				update = TRUE;
			}
			if ((color_type == PNG_COLOR_TYPE_GRAY)
				|| (color_type == PNG_COLOR_TYPE_GRAY_ALPHA))
			{
				png_set_gray_to_rgb(png_ptr);
				update = TRUE;
			}

			if (update) {
				png_read_update_info(png_ptr, info_ptr);
				color_type = png_get_color_type(png_ptr, info_ptr);
				bpp = png_get_bit_depth(png_ptr, info_ptr);
			}
			
			png_set_bgr(png_ptr);
			if (png_get_gAMA(png_ptr, info_ptr, &gamma))
				png_set_gamma(png_ptr, 2.2, gamma);
			else
				png_set_gamma(png_ptr, 2.2, 0.45455);


			rowbytes = png_get_rowbytes(png_ptr, info_ptr);
			row_pointers = (png_bytepp)malloc(height * png_sizeof(png_bytep));
			for (int y = 0; y < height; y++)
			{
				row_pointers[y] = (png_bytep)png_malloc(png_ptr, rowbytes);
			}

			png_read_image(png_ptr, row_pointers);
			png_read_end(png_ptr, NULL);

			// now for a tonne of ugly DIB setup crap
			bpp = info_ptr->channels * 8;

			ZeroMemory(&bi.bmiHeader, sizeof(BITMAPINFOHEADER));
			bi.bmiHeader.biSize        = sizeof(BITMAPINFOHEADER);
			bi.bmiHeader.biWidth       = width;
			bi.bmiHeader.biHeight      = -height;
			bi.bmiHeader.biPlanes      = 1;
			bi.bmiHeader.biBitCount    = bpp;
			bi.bmiHeader.biCompression = BI_RGB;

			int memWidth = WIDTHBYTES(width * bi.bmiHeader.biBitCount);

			hbm = CreateDIBSection(NULL, &bi, DIB_RGB_COLORS, (VOID **)&pixelData,  NULL, 0);

			if (hbm && pixelData)
			{
				*ppvBits = (VOID *)pixelData;
				// now copy the rows
				for (int i = 0; i < height; i++)
					CopyMemory(pixelData + i * memWidth, row_pointers[i], rowbytes);

			}
			free(row_pointers);

		}

	png_destroy_read_struct(&png_ptr, &info_ptr, &end_info);
	
	return hbm;

    if( stream != NULL )
    {
        delete [] stream;
        stream = NULL;
    }
}

HBITMAP WINAPI LoadBitmapPNG(LPCWSTR szFile, LPINT lpiWidth, LPINT lpiHeight, __deref_opt_out void **ppvBits)
{
	double gamma;
	HBITMAP hbm = NULL;
	errno_t err;
    BITMAPINFO      bi;
    BYTE * pixelData;
	int size = 0;
	png_byte color_type;
	FILE *fp;
	png_bytepp      row_pointers;
	png_size_t rowbytes;

	TCHAR fileSpec[MAX_PATH];
	StripFileName(szFile, fileSpec);

	err = _wfopen_s(&fp, fileSpec, L"rb");
	if (err != 0)
		return NULL;
	
	BYTE header[8];
	fread(header, 1, 8, fp);
	fseek(fp, 0, SEEK_END);
	size = ftell(fp);

	fclose(fp);
	
	if (png_sig_cmp(header, 0, 8))
		return NULL;
	
	// now allocate stuff
	png_structp png_ptr = 
		png_create_read_struct(PNG_LIBPNG_VER_STRING, 
		NULL, user_error_fn, user_warning_fn);
	
	if (!png_ptr)
		return NULL;
	
	png_infop info_ptr = png_create_info_struct(png_ptr);
	if (!info_ptr)
	{
		png_destroy_read_struct(&png_ptr,
			(png_infopp)NULL, (png_infopp)NULL);
		return NULL;
	}
	
	png_infop end_info = png_create_info_struct(png_ptr);
	if (!end_info)
	{
		png_destroy_read_struct(&png_ptr, &info_ptr,
			(png_infopp)NULL);
		return NULL;
	}
	
	err = _wfopen_s(&fp,fileSpec, L"rb");
	if (err == 0)
	{
		if (png_ptr != NULL)
		{

			BOOL update = FALSE;

			png_init_io(png_ptr, fp);
			png_read_info(png_ptr, info_ptr);

			int width = png_get_image_width(png_ptr, info_ptr);
			int height = png_get_image_height(png_ptr, info_ptr);
			int bpp = png_get_bit_depth(png_ptr, info_ptr);
			color_type = png_get_color_type(png_ptr, info_ptr);

			*lpiWidth = width;
			*lpiHeight = height;

			if (color_type == PNG_COLOR_TYPE_PALETTE)
			{
				png_set_palette_to_rgb(png_ptr);
				update = TRUE;
			}
			
			if (png_get_valid(png_ptr, info_ptr, PNG_INFO_tRNS))
			{
				png_set_tRNS_to_alpha(png_ptr);
				update = TRUE;
			}

			if (bpp == 16)
			{
				png_set_strip_16(png_ptr);
				update = TRUE;
			}
			if ((color_type == PNG_COLOR_TYPE_GRAY)
				|| (color_type == PNG_COLOR_TYPE_GRAY_ALPHA))
			{
				png_set_gray_to_rgb(png_ptr);
				update = TRUE;
			}

			if (color_type != PNG_COLOR_TYPE_RGB_ALPHA)
			{
				png_set_add_alpha(png_ptr, 0xff, PNG_FILLER_AFTER);
				png_set_interlace_handling(png_ptr);
				update = TRUE;
			}


			if (color_type != PNG_COLOR_TYPE_RGB_ALPHA)
			{
				png_set_add_alpha(png_ptr, 0xff, PNG_FILLER_AFTER);
				png_set_interlace_handling(png_ptr);
				update = TRUE;
			}

			if (update) {
				png_read_update_info(png_ptr, info_ptr);
				color_type = png_get_color_type(png_ptr, info_ptr);
				bpp = png_get_bit_depth(png_ptr, info_ptr);
			}
			


			png_set_bgr(png_ptr);
			
			if (png_get_gAMA(png_ptr, info_ptr, &gamma))
				png_set_gamma(png_ptr, 2.2, gamma);
			else
				png_set_gamma(png_ptr, 2.2, 0.45455);



			rowbytes = png_get_rowbytes(png_ptr, info_ptr);
			row_pointers = (png_bytepp)malloc(height * png_sizeof(png_bytep));
			for (int i=0; i<height; i++)
				row_pointers[i]=NULL;  /* security precaution */
			
			for (int y = 0; y < height; y++)
			{
				row_pointers[y] = (png_bytep)png_malloc(png_ptr, width * 4);
			}

			png_read_image(png_ptr, row_pointers);
			png_read_end(png_ptr, NULL);

			// now for a tonne of ugly DIB setup crap
			bpp = info_ptr->channels * 8;

			ZeroMemory(&bi.bmiHeader, sizeof(BITMAPINFOHEADER));
			bi.bmiHeader.biSize        = sizeof(BITMAPINFOHEADER);
			bi.bmiHeader.biWidth       = width;
			bi.bmiHeader.biHeight      = -height;
			bi.bmiHeader.biPlanes      = 1;
			bi.bmiHeader.biBitCount    = bpp;
			bi.bmiHeader.biCompression = BI_RGB;

			int memWidth = WIDTHBYTES(width * bi.bmiHeader.biBitCount);

			hbm = CreateDIBSection(NULL, &bi, DIB_RGB_COLORS, (VOID **)&pixelData,  NULL, 0);

			if (hbm && pixelData)
			{
				*ppvBits = (VOID *)pixelData;
				// now copy the rows
				for (int i = 0; i < height; i++)
					CopyMemory(pixelData + i * memWidth, row_pointers[i], rowbytes);

			}
			free(row_pointers);

		}
		fclose(fp);
	}
	
	png_destroy_read_struct(&png_ptr, &info_ptr, &end_info);
	
	return hbm;
}


HBITMAP WINAPI LoadBitmapJPG(LPCWSTR szFile, LPINT lpiWidth, LPINT lpiHeight, __deref_opt_out void **ppvBits)
{	
	jpeg_decompress_struct cinfo;	
	jpeg_error_mgr jerr;	
	errno_t err;
	FILE * infile;	
	BYTE * pBitmapBits;	
	HBITMAP hBitmap;
	BITMAPINFO bi;

	TCHAR fileSpec[MAX_PATH];
	StripFileName(szFile, fileSpec);

	err = _tfopen_s(&infile, fileSpec, _T("rb"));
	if (err != 0)
		return NULL;	

	cinfo.err = jpeg_std_error(&jerr);	
	jpeg_create_decompress(&cinfo);	
	jpeg_stdio_src(&cinfo, infile);	
	jpeg_read_header(&cinfo, TRUE);	
	jpeg_start_decompress(&cinfo);	

	int width = cinfo.output_width;
	int height = cinfo.output_height;


	ZeroMemory(&bi.bmiHeader, sizeof(BITMAPINFOHEADER));
	bi.bmiHeader.biSize        = sizeof(BITMAPINFOHEADER);
	bi.bmiHeader.biWidth       = width;
	bi.bmiHeader.biHeight      = -height;
	bi.bmiHeader.biPlanes      = 1;
	bi.bmiHeader.biBitCount    = cinfo.output_components*8;
	bi.bmiHeader.biCompression = BI_RGB;

	hBitmap = CreateDIBSection(NULL, &bi, DIB_RGB_COLORS, (VOID **)&pBitmapBits, NULL, 0);	

	if (hBitmap && pBitmapBits)	
	{		
		*ppvBits = (VOID *)pBitmapBits;
		*lpiWidth = cinfo.output_width;
		*lpiHeight = cinfo.output_height;

		int memWidth = WIDTHBYTES(width * bi.bmiHeader.biBitCount);

		PBYTE CurrentLine = PBYTE(pBitmapBits);
		for (UINT line = 0; line < cinfo.output_height; ++line)		
		{			
			UINT read = jpeg_read_scanlines(&cinfo, &CurrentLine, 1);			
			_ASSERTE(read == 1);		
			PBYTE CurrentPixel = CurrentLine;
			for (UINT pixel = 0; pixel < cinfo.output_width; ++pixel)			
			{				
				std::swap(CurrentPixel[0], CurrentPixel[2]);				
				CurrentPixel += cinfo.output_components;			
			}		
			CurrentLine += memWidth;			
		}
	}	
	
	jpeg_finish_decompress(&cinfo);	
	jpeg_destroy_decompress(&cinfo);	
	fclose(infile);
	return hBitmap;
}


HBITMAP WINAPI CreateNativeBitmap(INT width, INT height, __deref_opt_out void **ppvBits)
{
	BYTE * pBitmapBits;	
	HBITMAP hBitmap;
	BITMAPINFO bi;

	ZeroMemory(&bi.bmiHeader, sizeof(BITMAPINFOHEADER));
	bi.bmiHeader.biSize        = sizeof(BITMAPINFOHEADER);
	bi.bmiHeader.biWidth       = width;
	bi.bmiHeader.biHeight      = -height;
	bi.bmiHeader.biPlanes      = 1;
	bi.bmiHeader.biBitCount    = 32;
	bi.bmiHeader.biCompression = BI_RGB;

	hBitmap = CreateDIBSection(NULL, &bi, DIB_RGB_COLORS, (VOID **)&pBitmapBits, NULL, 0);	

	if (hBitmap && pBitmapBits)	
	{
		*ppvBits = (VOID *)pBitmapBits;

	}
	return hBitmap;
}
