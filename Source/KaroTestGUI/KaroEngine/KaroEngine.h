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
		
		pair<int,int> _leftBoundairy;
		pair<int,int> _topBoundairy;
		bool _boundairyChanged;
		//pair<int,int> _rightBoundairy;
		//pair<int,int> _bottomBoundairy;

		map<int,bool> redPieces;
		map<int,bool> whitePieces;
		map<int,int> moveableTiles;
		VisitedList * visitedList;

		map<int, int> transpositionTableWhite;
		map<int, int> transpositionTableRed;
		int randomTile[289];
		int randomRedUnmarked[289];
		int randomWhiteUnmarked[289];
		int randomRedMarked[289];
		int randomWhiteMarked[289];

		int GetHash();
		int GetHash(int hash,Move *move);
		int GetRandomNumber();
		/**
		* Functions
		*/
		// Algorithm functions
		Move * MiniMax(Player p, int depth, int alpha, int beta,int hash);
		int GetAmountConnectedTilesRecursive(int tileNumber);
		int GetAmountConnectedNeighbours(int tileNumber);
		void TransformToMovableTiles(int tileNumber, bool checkNeighbours, bool checkDiagonalNeighbours);


		// Do Move
		bool DoMove(int from);
		bool DoMove(int from, int to, bool isJumpMove);
		bool DoMove(int from, int to, int tileFrom, bool isJumpMove);

		// Undo Move
		bool UndoMove(int from);
		bool UndoMove(int from, int to, bool isJumpMove);
		bool UndoMove(int from, int to, int tileFrom, bool isJumpMove);

		
	public:
		
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
		bool InsertByXY(int position);
		void DoMove(int from, int to, int fromTile);
		void DoMove(Move *move);		
		void UndoMove(Move *move);

		bool IsValidMove(int from, int to);
		bool IsWinner(Player p, int lastMove);
		Player Reverse(Player);
		void SetMessageLog(std::string s);

		int EvaluateBoard(Player p);
		map<int,bool> KaroEngine::GetPlayerPieces(Player p);

		// Get possible moves
		vector<Move*> * GetPossibleMoves(Player forPlayer);
		vector<Move*> * GetPossibleMoves(int tile, bool isTurned);

		// Algorithm functions
		float CalculateComputerMove();
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
}