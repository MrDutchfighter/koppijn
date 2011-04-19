#pragma once

#include "Win.h"
#include <vector> // wordt niet gebruikt?
using namespace std;

class KaroGame
{
public:
	KaroGame(void);
	~KaroGame(void);

	void RenderBoard(HDC hdc);
	void ExecuteClick(int x, int y);

	bool gameOver;
	POINT firstClick;
	POINT secondClick;
};