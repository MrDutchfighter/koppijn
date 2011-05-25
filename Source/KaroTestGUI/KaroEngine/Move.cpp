#include "StdAfx.h"
#include "Move.h"


Move::Move(void)
{
}

Move::Move(int to)
{
	this->positionFrom = -1;
	this->positionTo = to;
	this->tileFrom = -1;
	this->isJumpMove = false;
	this->isWinningMove = false;
}

Move::Move(int from, int to, bool isJumpMove)
{
	this->positionFrom = from;
	this->positionTo = to;
	this->tileFrom = -1;
	this->isJumpMove = isJumpMove;
	this->isWinningMove = false;
}

Move::Move(int from, int to, int tileFrom, bool isJumpMove)
{
	this->positionFrom = from;
	this->positionTo = to;
	this->tileFrom = tileFrom;
	this->isJumpMove = isJumpMove;
	this->isWinningMove = false;
}