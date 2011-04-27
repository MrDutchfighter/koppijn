// KaroEngine.h
#pragma once
#using <System.dll>
#include <stdlib.h>
#include <time.h>
#include <string>
#include <vector>
#include <map>
#include "Move.h"
using namespace std;
using namespace System;

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
		int evaluationScore;		
		int possibleSteps[8];
		int possibleJumps[8];		
		Move * MiniMax(Player p, int depth, int alpha, int beta);
		map<int,bool> redPieces;
		map<int,bool> whitePieces;
	public:
		static const int BOARDWIDTH = 17;
		KaroEngine(void);
		~KaroEngine(void);
		Player Reverse(Player);
		Player GetTurn();
		GameState GetGameState();
		bool IsValidMove(int from, int to);
		void DoMove(Move *m);
		void DoMove(int from, int to, int fromTile);
		void UndoMove(Move *m);
		bool FreeForMove(int); // checks if a tile is empty
		bool IsGameTile(int); // checks if a tile exists
		bool InsertByXY(int x, int y);
		int * GetBoard(void);
		Tile GetByXY(int x,int y);
		bool IsWinner(Player p, int lastMove);
		void CalculateComputerMove();
		std::string GetMessageLog();
		void SetMessageLog(std::string s);
		int EvaluateBoard(Player p);
		int GetEvaluationScore();

		// Get possible moves
		vector<Move*> * GetPossibleMoves(Player forPlayer);
		vector<Move*> * GetPossibleMoves(int tile, bool isTurned);

		int maxDepth; // Maximum moves
	};
}