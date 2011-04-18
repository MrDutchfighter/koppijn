// KaroEngine.h

#pragma once

using namespace System;

namespace KaroEngine 
{
	public enum Tile
	{
		EMPTY,
		SOLIDTILE,
		MOVEABLETILE,
		WHITEUNMARKED,
		WHITEMARKED,
		REDUNMARKED,
		REDMARKED
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
	};
}