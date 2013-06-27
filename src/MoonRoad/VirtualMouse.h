#pragma once

#ifdef __cplusplus
extern "C" {
#endif

void WINAPI MouseMove(int xDelta, int yDelta);
void WINAPI MouseMoveTo(int x, int y);
void WINAPI MouseLeftClick();
void WINAPI MouseRightClick();

#ifdef __cplusplus
}
#endif
