// KaroEngine.h
#pragma once
#using <System.dll>
#include <stdlib.h>
#include <time.h>
#include <string>
#include <map>
#include "Move.h"

using namespace System;
using namespace std;

namespace KaroEngine 
{	
	public enum class Tile : unsigned int {
		BORDER = 0,
		EMPTY = 1,
		SOLIDTILE = 2,
		MOVEABLETILE = 3,
		WHITEUNMARKED = 4,
		WHITEMARKED = 5,
		REDUNMARKED = 6,
		REDMARKED = 7
	};

	public  enum class Player : unsigned int { 
		WHITE = 0,
		RED = 1
	};

	public enum class GameState : unsigned int {
		INSERTION = 0,
		PLAYING = 1,
		GAMEFINISHED = 2 
	};
	
	public class KaroEngine
	{
	private:
		GameState gameState;
		std::string messageLog;
		Tile * board;
		Player turn;
		int insertionCount;
		static const int BOARDWIDTH = 17;
		int possibleSteps[8];
		int possibleJumps[8];		
		Move * MiniMax(Player p, int depth, int alpha, int beta);
	public:
		KaroEngine(void);
		~KaroEngine(void);
		Player Reverse(Player);
		Player GetTurn();
		GameState GetGameState();
		bool IsValidMove(int from, int to);
		void DoMove(int from, int to, int fromTile);
		void UndoMove();
		bool FreeForMove(int); // checks if a tile is empty
		bool IsGameTile(int); // checks if a tile exists
		bool InsertByXY(int x, int y);
		int * GetBoard(void);
		Tile GetByXY(int x,int y);
		bool IsWinner(Player p);
		void CalculateComputerMove();
		std::string GetMessageLog();
		void SetMessageLog(std::string s);
		map<int,bool> redPieces;
		map<int,bool> whitePieces;
	};
}