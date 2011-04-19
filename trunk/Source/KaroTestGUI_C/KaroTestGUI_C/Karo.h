#ifndef Karo_H
#define Karo_H

#include "Win.h"
#include "KaroGame.h"

class CKaro : public CWin
{
public:
	CKaro();
	~CKaro();

	LRESULT MsgProc(HWND, UINT, WPARAM, LPARAM);
	static LRESULT About(HWND, UINT, WPARAM, LPARAM);
};

#endif
