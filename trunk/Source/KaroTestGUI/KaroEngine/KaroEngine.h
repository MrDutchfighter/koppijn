// KaroEngine.h
#pragma once
#using <System.dll>
#include <stdlib.h>
#include <time.h>
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

	public enum class GameState:unsigned int{
		INSERTION = 0,
		PLAYING = 1,
		GAMEFINISHED = 2 
	};
	
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
		GameState GetGameState();
		bool IsValidMove(int from, int to);
		void DoMove(int to);
		void DoMove(int from, int to, int fromTile);
		void UndoMove();
		bool FreeForMove(int); // checks if a tile is empty
		bool IsGameTile(int); // checks if a tile exists
		bool InsertByXY(int x, int y);
		int * GetBoard(void);
		Tile GetByXY(int x,int y);
		bool IsWinner(Player p);
		void CalculateComputerMove();
	};
}