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
#include "time.h"
#include <windows.h>
#include <tchar.h>
#include <stdio.h>
#include "VisitedList.h"
#include <sstream>
#include <algorithm>
#include "mtrand.h"
#include <iostream>
using namespace std;
using namespace System;

class VisitedList;
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
		map<int,int> moveableTiles;
		map<int,int> allEmptyTiles;
		VisitedList * visitedList;

		map<int, pair<int,int>> transpositionTableWhite; //<hash, <depth,score>>
		map<int, pair<int,int>> transpositionTableRed; //<hash, <depth,score>>
		
		long randomTile[289];
		long randomRedUnmarked[289];
		long randomWhiteUnmarked[289];
		long randomRedMarked[289];
		long randomWhiteMarked[289];

		long GetHash();
		long GetHash(long hash,Move *move);
		long GetRandomNumber();
		int CountMarkedPieces(map<int,bool>);

		/**
		* Functions
		*/
		// Algorithm functions
		Move * MiniMax(Player p, int depth, int alpha, int beta,long hash, int currentEvaluation);
		int GetAmountConnectedTilesRecursive(int tileNumber);
		int GetAmountConnectedNeighbours(int tileNumber);
		void TransformToMovableTiles(int tileNumber, bool checkNeighbours, bool checkDiagonalNeighbours);
		void TransformToMoveableTiles();

		// Do Move
		bool DoMove(int from);
		bool DoMove(int from, int to, bool isJumpMove);
		bool DoMove(int from, int to, int tileFrom, bool isJumpMove);

		// Undo Move
		bool UndoMove(int from);
		bool UndoMove(int from, int to, bool isJumpMove);
		bool UndoMove(int from, int to, int tileFrom, bool isJumpMove);

		
	public:
		
		int  boardLeft;
		int  boardRight;
		int  boardTop;
		int  boardBottom;

		/**
		* Properties
		*/
		static const int BOARDWIDTH = 17;	// Width of the board
		int maxDepth;						// Max depth for minmax
		Move * lastMove;

		/**
		* Functions
		*/
		KaroEngine(void);
		~KaroEngine(void);

		// Normal functions
		void DoMove(int from, int to, int fromTile);
		bool DoMove(Move *move);		
		void UndoMove(Move *move);

		bool IsValidMove(int from, int to);
		bool IsWinner(Player p, int lastMove);
		Player Reverse(Player &p);
		void SetMessageLog(std::string s);

		int EvaluateBoard(Player p);
		map<int,bool> KaroEngine::GetPlayerPieces(Player p);

		// Get possible moves
		vector<Move*> * GetPossibleMoves(Player forPlayer,int hash);
		vector<Move*> * GetPossibleMoves(int tile);

		// Algorithm functions
		Move* CalculateComputerMove();
		int EvaluateNumRows(Player p, int pieceIndex);


		// 'Getters'
		bool FreeForMove(int); // checks if a tile is empty
		bool IsGameTile(int); // checks if a tile exists
		GameState GetGameState();
		Player GetTurn();
		int GetAmountConnectedTiles(int tileNumber);

			
		// Getters
		int * GetBoard(void);
		Tile GetByXY(int x,int y);
		std::string GetMessageLog();
		int GetEvaluationScore();
	};

	// function to compare moves in a vector
	inline bool bigger_than_second(const Move* pMove1, const Move* pMove2){
		return (pMove1->score > pMove2->score);
	}

	inline bool smaller_than_second(const Move* pMove1, const Move* pMove2){
		return (pMove1->score < pMove2->score);
	}
}