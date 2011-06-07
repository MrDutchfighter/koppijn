#include "StdAfx.h"
#include <msclr\marshal_cppstd.h>
#include "KaroEngineWrapper.h"

using namespace msclr::interop;
using namespace KaroEngine;

/**
* Constructor
*/
KaroEngineWrapper::KaroEngineWrapper(void)
{
	_karoEngine = new KaroEngine();	
}

/**
* Destructor
*/
KaroEngineWrapper::~KaroEngineWrapper(void)
{
	if(_karoEngine)
		delete _karoEngine;
}

/**
* Gets a tile at the current position (x, y)
*/
Tile KaroEngineWrapper::GetByXY(int x, int y){
	return _karoEngine->GetByXY(x,y);
}

/**
* Executes a move
*/
bool KaroEngineWrapper::DoMove(int from, int to, int tile){	
	return _karoEngine->DoMove(from,to,tile);
}

/**
* Returns the state of the game
*/
GameState KaroEngineWrapper::GetGameState(){
	return _karoEngine->GetGameState();
}

/**
* Get the turn
*/
Player KaroEngineWrapper::GetTurn()
{
	return _karoEngine->GetTurn();
}

/**
* Insert a tile with x, y
*/
bool KaroEngineWrapper::InsertByXY(int x, int y) {
	int position=(y*_karoEngine->BOARDWIDTH)+x;
	Move * v = new Move(position);
	return _karoEngine->DoMove(v);
}

/**
* Get the top board boundary
*/
int KaroEngineWrapper::GetBoardTop() {
	return _karoEngine->boardTop;
}

/**
* Get the left board boundary
*/
int KaroEngineWrapper::GetBoardLeft() {
	return _karoEngine->boardLeft;
}

/**
* Get the right board boundary
*/
int KaroEngineWrapper::GetBoardRight() {
	return _karoEngine->boardRight;
}

/**
* Get the bottom board boundary
*/
int KaroEngineWrapper::GetBoardBottom() {
	return _karoEngine->boardBottom;
}

/**
* Return the evaluation score
*/
int KaroEngineWrapper::GetEvaluationScore(){
	return _karoEngine->GetEvaluationScore();
}

/**
* Calculates a computer move and return an array with the correct move (posFrom, posTo, tileFrom)
*/
array<int>^ KaroEngineWrapper::CalculateComputerMove(){

	Move* calculatedMove = _karoEngine->CalculateComputerMove();

	array<int>^ theMove = gcnew array<int>(4);

	theMove[0] = calculatedMove->positionFrom;
	theMove[1] = calculatedMove->positionTo;
	theMove[2] = calculatedMove->tileFrom;
	if(calculatedMove->isJumpMove)
		theMove[3] = 1;
	else
		theMove[3] = 0;
	
	return theMove;
}

/**
* Gets the content of the message log
*/
String ^KaroEngineWrapper::GetMessageLog(){
	return marshal_as<String ^>(_karoEngine->GetMessageLog());
}

/**
* Get an array of possible moves
*/
array<array<int>^>^ KaroEngineWrapper::GetPossibleMoves(int x, int y,int tileFromX,int tileFromY){
	vector<Move*>* possibleMoves = _karoEngine->GetPossibleMoves((y*_karoEngine->BOARDWIDTH+x));
	
	int tilefrom=(tileFromY*_karoEngine->BOARDWIDTH+tileFromX);
	if(tileFromX == -1){
		tilefrom=-1;
	}

	int index=0;
	for(int i=0;i<possibleMoves->size();i++){
		if(possibleMoves->at(i)->tileFrom ==tilefrom){
			index++;
		}
	}
	array<array<int>^>^ params = gcnew array<array<int>^>(index);
	index=0;
	for(int i=0;i<possibleMoves->size();i++){
		if(possibleMoves->at(i)->tileFrom ==tilefrom){
			params[index]=gcnew array<int>(2);
			params[index][0] = possibleMoves->at(i)->positionTo%_karoEngine->BOARDWIDTH;
			params[index][1] = possibleMoves->at(i)->positionTo/_karoEngine->BOARDWIDTH;
			index++;
		}
	}
	return params;
}

/**
* Undo the last move, returns the last move
*/
array<int>^ KaroEngineWrapper::UndoLastMove() {
	_karoEngine->UndoMove(_karoEngine->lastMove);

	array<int>^ theMove = gcnew array<int>(4);

	theMove[0] = _karoEngine->lastMove->positionFrom;
	theMove[1] = _karoEngine->lastMove->positionTo;
	theMove[2] = _karoEngine->lastMove->tileFrom;
	if(_karoEngine->lastMove->isJumpMove)
		theMove[3] = 1;
	else
		theMove[3] = 0;
	
	return theMove;
}