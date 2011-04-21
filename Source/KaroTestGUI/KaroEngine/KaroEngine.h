// KaroEngine.h
#pragma once
#using <System.dll>
using namespace System;

namespace KaroEngine 
{	
	public enum class Tile : unsigned int{
		EMPTY=0,
		SOLIDTILE=1,
		MOVEABLETILE=2,
		WHITEUNMARKED=3,
		WHITEMARKED=4,
		REDUNMARKED=5,
		REDMARKED=6
	};

	public  enum class Player: unsigned int{ 
		WHITE=0,
		RED=1
	};

	public enum GameState{ INSERTION, PLAYING, GAMEFINISHED };
	
	public class KaroEngine
	{
	private:
		GameState gameState;
		Tile * board;
		Player turn;
		int insertionCount;
		static const int BOARDWIDTH = 15;

	public:
		KaroEngine(void);
		~KaroEngine(void);
		Player Reverse(Player);
		Player GetTurn();
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