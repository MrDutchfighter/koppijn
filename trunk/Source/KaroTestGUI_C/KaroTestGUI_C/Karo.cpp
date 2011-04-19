#include "Karo.h"
#include "resource.h"

/////////////////////////////////////
// Constructors / Destructors      //
/////////////////////////////////////
CKaro::CKaro()
{
	this->m_strWindowTitle	 = "Karo Window";
	this->m_dwCreationWidth  = 400;
	this->m_dwCreationHeight = 300;
	this->m_hIcon			 = LoadIcon(m_hInstance, MAKEINTRESOURCE(IDI_SKELET));
	this->m_hMenu			 = LoadMenu(m_hInstance, MAKEINTRESOURCE(IDR_MENU));
	this->m_hAccelTable		 = LoadAccelerators(m_hInstance, MAKEINTRESOURCE(IDR_ACCELERATOR));
}

CKaro::~CKaro()
{
}

/////////////////////////////////////
// Member functions                  //
/////////////////////////////////////
LRESULT CKaro::About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message)
	{
		case WM_COMMAND:
			if (LOWORD(wParam) == IDOK) 
			{
				EndDialog(hDlg, LOWORD(wParam));
				return TRUE;
			}
			break;
	}
    return FALSE;
}

LRESULT CKaro::MsgProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	char		buf[80];
	int			wmId;
	int			wmEvent;
	static int	n;
	static KaroGame *theKaroGame;

	PAINTSTRUCT ps;
	HDC			hdc;
	RECT		rt;

	switch (uMsg) 
	{
		case WM_CREATE:
			theKaroGame = new KaroGame();
		break;

		case WM_COMMAND:
			wmId    = LOWORD(wParam); 
			wmEvent = HIWORD(wParam); 

			switch(wmId)
			{
				case IDM_EXIT:
					PostQuitMessage(0);
				break;

				case IDM_ABOUT:
					DialogBox(m_hInstance, MAKEINTRESOURCE(IDD_ABOUT), m_hWnd, 
						(DLGPROC)About);
				break;
			}
		break;

		case WM_PAINT:
			hdc = BeginPaint(hWnd, &ps);
			
			theKaroGame->RenderBoard(hdc);

			EndPaint(hWnd, &ps);
		break;

		case WM_DESTROY:
			//MessageBox(NULL, "Windows Closing!", "Bye", 0);
		break;
	}
	return CWin::MsgProc(hWnd, uMsg, wParam, lParam);
}

