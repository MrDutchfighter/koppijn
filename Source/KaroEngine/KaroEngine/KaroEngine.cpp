// This is the main DLL file.

#include "stdafx.h"

#include "KaroEngine.h"

namespace KaroEngine 
{
	KaroEngine::KaroEngine(void)
	{
		board = new Tile[BOARDWIDTH * BOARDWIDTH];
		this->turn = WHITE;
		int gameState = INSERTION; 
		insertionCount = 0;

		for(int i = 0; i < BOARDWIDTH * BOARDWIDTH ; i ++ )
			board[i] = EMPTY;

		for(int j = 4; j < 8; j++)
			for( int k = 5; k < 10; k++ )
				board[j*BOARDWIDTH+k] = SOLIDTILE;
	}

	KaroEngine::~KaroEngine(void)
	{

	}

	int * KaroEngine::GetBoard(void)
	{
		int * ret = new int[BOARDWIDTH * BOARDWIDTH];
		
		for (int i = 0 ; i < BOARDWIDTH * BOARDWIDTH; i ++)
		{
			ret[i] = (int)this->board[i];
		}

		return ret;
	}

	void KaroEngine::DoMove(int to)
	{
			if(board[to] == SOLIDTILE)
			{
				if(turn == WHITE)
					board[to] = WHITEUNMARKED;
				else
					board[to] = REDUNMARKED;

				insertionCount++;
			}
			if(insertionCount == 12)
				gameState = PLAYING;

			turn = Reverse(turn);
	}
	void KaroEngine::DoMove(int from, int to)
	{
		if(gameState == PLAYING)
		{
			if(IsValidMove(from, to))
			{
				board[to] = board[from];
				board[from] = SOLIDTILE;
			}
		}

		turn = Reverse(turn);
	}

	Player KaroEngine::Reverse(Player turn)
	{
		if(turn == WHITE )
			return RED;
		else
			return WHITE;
	}

	bool KaroEngine::IsValidMove(int from, int to)
	{

		// check if the move is valid by validating with the turn of the current player
		if(turn == RED && (board[from] != REDUNMARKED || board[from] != REDMARKED))
			return false;
		else if(turn == WHITE && (board[from] != WHITEUNMARKED || board[from] != WHITEMARKED))
			return false;


		int rowFrom = from/BOARDWIDTH;
		int rowTo = to/BOARDWIDTH;

		int colFrom = from%BOARDWIDTH;
		int colTo = to%BOARDWIDTH;

		int rowDifference = rowFrom-rowTo;
		int colDifference = colFrom-colTo;

		int rowDifferencePos = rowDifference;
		if(rowDifference < 0) { rowDifferencePos *= -1; } 

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
			return true;
		}
		return false;
	}

	bool KaroEngine::IsGameTile(int tile)
	{
		return board[tile] != EMPTY;
	}

	Player KaroEngine::GetTurn()
	{
		return turn;
	}

	Tile KaroEngine::GetByXY(int x,int y){
		return board[(y*15)+x];
	}

	bool KaroEngine::IsWinner(Player p)
	{
		Tile marked;
		//Right player color 
		if (p == WHITE) 
			marked = WHITEMARKED;
		if (p == RED)
			marked = REDMARKED;


		for(int i = 0; i < BOARDWIDTH; i++) {
			for(int j = 0; j < BOARDWIDTH; j++) {
				//Current position
				int current = i * BOARDWIDTH + j;
				
				// Is current tile marked
				if(board[current] == marked)
				{
					//Check vertical boundery 
					if(i <= BOARDWIDTH - 4)
					{
						//Vertical
						if(board[current + 1 * BOARDWIDTH] == marked && board[current + 2 * BOARDWIDTH] == marked && board[current + 3 * BOARDWIDTH] == marked)
						{
							return true;
						}
					}

					//Check horizontal boundery
					if(j <= BOARDWIDTH - 4)
					{
						//Horizontal
						if(board[current + 1] == marked && board[current + 2] == marked && board[current + 3] == marked)
						{
							return true;
						}
					}
					
					//Check horizontal and vertical bounderies
					if(i <= BOARDWIDTH - 4 && j <= BOARDWIDTH - 4)
					{
						//Diagonal down
						if(board[current + 1 + (1 * BOARDWIDTH)] == marked && board[current + 2 + (2 * BOARDWIDTH)] == marked && board[current + 3 + (3 * BOARDWIDTH)] == marked)
						{
							return true;
						}

						//Diagonal up
						if(board[current + 3 + (0 * BOARDWIDTH)] == marked && board[current + 2 + (1 * BOARDWIDTH)] == marked && board[current + 1 + (2 * BOARDWIDTH)] == marked && board[current + 0 + (3 * BOARDWIDTH)] == marked)
						{
							return true;
						}
					}
				}
			}
		}

		return false;
	}
}