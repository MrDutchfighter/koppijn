// WinClass.cpp : Defines the entry point for the application.
//
#include "Karo.h"

int APIENTRY WinMain(HINSTANCE hInstance,
                     HINSTANCE hPrevInstance,
                     LPSTR     lpCmdLine,
                     int       nCmdShow)
{
	CKaro app;

	if (!app.Create())
		return 0;
	
	return app.Run();
}
