// KaroEngine.h
#pragma once
#using <System.dll>
#include <stdlib.h>
#include <time.h>
#include <string>
#include <vector>
#include <map>
#include "Move.h"
#include "LIMITS.H"
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
		/**
		* Properties
		*/
		GameState gameState;
		std::string messageLog;
		Tile * board;
		Player turn;
		int insertionCount;
		int evaluationScore;		
		int possibleSteps[8];
		int possibleJumps[8];

		map<int,bool> redPieces;
		map<int,bool> whitePieces;

		/**
		* Functions
		*/
		// Algorithm functions
		Move * MiniMax(Player p, int depth, int alpha, int beta);
		
	public:
		
		/**
		* Properties
		*/
		static const int BOARDWIDTH = 17;	// Width of the board
		int maxDepth;						// Max depth for minmax

		/**
		* Functions
		*/
		KaroEngine(void);
		~KaroEngine(void);

		// Normal functions
		bool InsertByXY(int position);
		void DoMove(int from, int to, int fromTile);
		void DoMove(Move *move);		
		void UndoMove(Move *move);

		bool IsValidMove(int from, int to);
		bool IsWinner(Player p, int lastMove);
		Player Reverse(Player);
		void SetMessageLog(std::string s);

		int EvaluateBoard(Player p);

		// Get possible moves
		vector<Move*> * GetPossibleMoves(Player forPlayer);
		vector<Move*> * GetPossibleMoves(int tile, bool isTurned);

		// Algorithm functions
		void CalculateComputerMove();

		// 'Getters'
		bool FreeForMove(int); // checks if a tile is empty
		bool IsGameTile(int); // checks if a tile exists
		GameState GetGameState();
		Player GetTurn();

		// Getters
		int * GetBoard(void);
		Tile GetByXY(int x,int y);
		std::string GetMessageLog();
		int GetEvaluationScore();
	};
}