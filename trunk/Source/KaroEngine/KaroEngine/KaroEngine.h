// KaroEngine.h

#pragma once

using namespace System;

	enum FieldTile{ 
		EMPTY,
		TILE,
		MOVEABLETILE,
		WHITEUNMARKED,
		WHITEMARKED,
		REDUNMARKED,
		REDMARKED
	};
	enum Player{ WHITE, RED };

	enum GameState{ INSERTION, PLAYING, GAMEFINISHED };

	
	public ref class KaroEngine
	{
		// TODO: Add your methods for this class here.
	private:
		GameState gameState;
		FieldTile *board;
		Player turn;
		static const int BOARDWIDTH = 15;

	public:
		KaroEngine(void);
		~KaroEngine(void);
		bool IsValidMove(int from, int to);
		void DoMove(int, int);
		void UndoMove();
		bool IsTileEmpty(int); // checks if a tile is empty
		bool IsTile(int); // checks if a tile exists
		
	};

