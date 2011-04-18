// This is the main DLL file.

#include "stdafx.h"

#include "KaroEngine.h"

KaroEngine::KaroEngine(void)
{
	board = new Tile[BOARDWIDTH * BOARDWIDTH];
	this->turn = WHITE;
	int gamestate = INSERTION; // 0 is insertionstate 1 is gameplaystate 2 is gameover

	for(int i = 0; i < BOARDWIDTH * BOARDWIDTH ; i ++ )
		board[i] = EMPTY;

	for(int j = 4; j < 8; j++)
		for( int k = 5; k < 10; k++ )
			board[j*BOARDWIDTH+k] = SOLIDTILE;
}

KaroEngine::~KaroEngine(void)
{

}
void KaroEngine::DoMove(int from, int to)
{
	if(IsValidMove(from, to))
	{
		board[to] = board[from];
	}
}

bool KaroEngine::IsValidMove(int from, int to)
{
	int rowFrom = from/BOARDWIDTH;
	int rowTo = to/BOARDWIDTH;

	int colFrom = from%BOARDWIDTH;
	int colTo = to%BOARDWIDTH;

	int rowDifference = rowFrom-rowTo;
	int colDifference = colFrom-colTo;

	int rowDifferencePos = rowDifference;
	if(rowDifference < 0) { rowDifference *= -1; } 

	int colDifferencePos = colDifference;
	if(colDifferencePos < 0) { colDifferencePos *= -1; }

	// Distance bigger than 2 steps
	if(rowDifference < -2 || rowDifference > 2 || colDifference < -2 && colDifference > 2) {
		return false;
	}

	// Can you move this tile?
	if(!IsGameTile(from) || FreeForMove(from)) {
		return false;
	}

	// If moveto tile not a valid tile
	if(!IsGameTile(to) || !FreeForMove(to)) {
		return false;
	}

	// If impossible move
	if(rowDifferencePos+colDifferencePos == 3) {
		return false;
	}

	// If possible move
	if(rowDifferencePos+colDifferencePos == 1 || (rowDifferencePos == 1 && colDifferencePos == 1)) {
		return true;
	}

	// Tile to check
	int checkableTile = ((from-to)/2)+to;
	if(FreeForMove(checkableTile) || !IsGameTile(checkableTile)) {
		return false;
	}

	return true; // VICTORIOUSSSSS
}
void KaroEngine::UndoMove()
{
}

bool KaroEngine::FreeForMove(int tile)
{
	if(board[tile] == SOLIDTILE || board[tile] == MOVEABLETILE) {
		return false;
	}
	return true;
}

bool KaroEngine::IsGameTile(int tile)
{
	return board[tile] != EMPTY;
}