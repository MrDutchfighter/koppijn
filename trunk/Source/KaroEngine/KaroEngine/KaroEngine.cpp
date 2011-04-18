// This is the main DLL file.

#include "stdafx.h"

#include "KaroEngine.h"

KaroEngine::KaroEngine(void)
{
	board = new int[255];
	this->turn = Black;
	int gamestate = Insertion; // 0 is insertionstate 1 is gameplaystate 2 is gameover
}

KaroEngine::~KaroEngine(void)
{

}
void KaroEngine::DoMove()
{
	
}

void KaroEngine::UndoMove()
{
}

bool KaroEngine::IsTileEmpty(int tile)
{
	return true;
}
bool KaroEngine::IsTile(int tile)
{
	return true;
}