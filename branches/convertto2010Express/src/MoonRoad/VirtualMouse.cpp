#include "stdafx.h"
#include "VirtualMouse.h"

void WINAPI MouseMove(int xDelta, int yDelta)
{
	mouse_event(MOUSEEVENTF_MOVE, xDelta, yDelta, 0, NULL);
}

void WINAPI MouseMoveTo(int x, int y)
{
	mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x, y, 0, NULL);
}

void WINAPI MouseLeftClick()
{
	POINT p;
	GetCursorPos(&p);
	mouse_event(MOUSEEVENTF_LEFTDOWN, p.x, p.y, 0, NULL);
	mouse_event(MOUSEEVENTF_LEFTUP, p.x, p.y, 0, NULL);
}

void WINAPI MouseRightClick()
{
	POINT p;
	GetCursorPos(&p);
	mouse_event(MOUSEEVENTF_RIGHTDOWN, p.x, p.y, 0, NULL);
	mouse_event(MOUSEEVENTF_RIGHTUP, p.x, p.y, 0, NULL);
}


