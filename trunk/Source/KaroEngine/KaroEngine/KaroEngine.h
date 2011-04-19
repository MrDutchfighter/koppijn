// KaroEngine.h

#pragma once

using namespace System;

namespace KaroEngine 
{
	public enum Tile
	{
		EMPTY=0,
		SOLIDTILE=1,
		MOVEABLETILE=2,
		WHITEUNMARKED=3,
		WHITEMARKED=4,
		REDUNMARKED=5,
		REDMARKED=6
	};

	public enum class Managed_Tile
	{
		EMPTY = EMPTY,
		SOLIDTILE = SOLIDTILE,
		MOVEABLETILE = MOVEABLETILE,
		WHITEUNMARKED = WHITEUNMARKED,
		WHITEMARKED = WHITEMARKED,
		REDUNMARKED = REDUNMARKED,
		REDMARKED = REDMARKED
	};

	enum Player{ WHITE, RED };

	enum GameState{ INSERTION, PLAYING, GAMEFINISHED };

	
	public ref class KaroEngine
	{
	private:
		GameState gameState;
		Tile *board;
		Player turn;
		static const int BOARDWIDTH = 15;

	public:
		KaroEngine(void);
		~KaroEngine(void);
		bool IsValidMove(int from, int to);
		void DoMove(int, int);
		void UndoMove();
		bool FreeForMove(int); // checks if a tile is empty
		bool IsGameTile(int); // checks if a tile exists
		int * GetBoard(void);
		Tile GetByXY(int x,int y);
		bool IsWinner(Player p);
	};
}