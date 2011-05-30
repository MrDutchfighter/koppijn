#include "StdAfx.h"
#include <msclr\marshal_cppstd.h>
#include "KaroEngineWrapper.h"


using namespace msclr::interop;
using namespace KaroEngine;

KaroEngineWrapper::KaroEngineWrapper(void)
{
	_karoEngine = new KaroEngine();	
}

KaroEngineWrapper::~KaroEngineWrapper(void)
{
	if(_karoEngine)
		delete _karoEngine;
}

Tile KaroEngineWrapper::GetByXY(int x, int y){
	return _karoEngine->GetByXY(x,y);
}

bool KaroEngineWrapper::DoMove(int from, int to, int tile){	
	return _karoEngine->DoMove(from,to,tile);
}

GameState KaroEngineWrapper::GetGameState(){
	return _karoEngine->GetGameState();
}

Player KaroEngineWrapper::GetTurn()
{
	return _karoEngine->GetTurn();
}

bool KaroEngineWrapper::InsertByXY(int x, int y) {
	int position=(y*_karoEngine->BOARDWIDTH)+x;
	Move * v = new Move(position);
	return _karoEngine->DoMove(v);
}

int KaroEngineWrapper::GetBoardTop(){
	return _karoEngine->boardTop;
}
int KaroEngineWrapper::GetBoardLeft(){
	return _karoEngine->boardLeft;
}
int KaroEngineWrapper::GetBoardRight(){

	return _karoEngine->boardRight;
}
int KaroEngineWrapper::GetBoardBottom(){
	return _karoEngine->boardBottom;
}


int KaroEngineWrapper::GetEvaluationScore(){
	return _karoEngine->GetEvaluationScore();
}

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

String ^KaroEngineWrapper::GetMessageLog(){
	return marshal_as<String ^>(_karoEngine->GetMessageLog());
}

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

void KaroEngineWrapper::UndoLastMove() {
	_karoEngine->UndoMove(_karoEngine->lastMove);
}