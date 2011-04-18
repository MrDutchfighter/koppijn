// This is the main DLL file.

#include "stdafx.h"

#include "KaroEngine.h"

KaroEngine::KaroEngine(void)
{
	board = new FieldTile[BOARDWIDTH * BOARDWIDTH];
	this->turn = WHITE;
	int gamestate = INSERTION; // 0 is insertionstate 1 is gameplaystate 2 is gameover

	for(int i = 0; i < BOARDWIDTH * BOARDWIDTH ; i ++ )
		board[i] = EMPTY;

	for(int j = 4; j < 8; j++)
		for( int k = 5; k < 10; k++ )
			board[j*BOARDWIDTH+k] = TILE;
}

KaroEngine::~KaroEngine(void)
{

}
void KaroEngine::DoMove(int from, int to)
{
	if(IsTile(to))
	{

		board[to] = board[from];
	}
}

bool KaroEngine::IsValidMove(int from, int to)
{
	bool valid = false;
	// grenzende move
	int move = from - to;
	if(move == -1 || move == 1 || move == -BOARDWIDTH+1 || move == -BOARDWIDTH || move == -BOARDWIDTH-1 || move == BOARDWIDTH-1 || move == BOARDWIDTH || move == BOARDWIDTH+1 )
	{
		if(IsTile(to))
			valid = true;
	}
	// jump move
	else if(move == -2 || move == 2 || move == -(2*BOARDWIDTH)+1 || move == -2*BOARDWIDTH || move == -(2*BOARDWIDTH)-1 || move == (2*BOARDWIDTH)-1 || move == 2*BOARDWIDTH || move == (2*BOARDWIDTH)+1 )
	{
		if(IsTile(to))
		{
			FieldTile tempField = board[((from - to) / 2) + to];
			if ( tempField == WHITEUNMARKED || tempField == WHITEUNMARKED || tempField == WHITEUNMARKED || tempField == WHITEUNMARKED )
			{
				valid = true;
			}
		}
	}

	return valid;

}
void KaroEngine::UndoMove()
{
}

bool KaroEngine::IsTileEmpty(int tile)
{
	return board[tile] == EMPTY;
}
bool KaroEngine::IsTile(int tile)
{
	return board[tile] == TILE;
}