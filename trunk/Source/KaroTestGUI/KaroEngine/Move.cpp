#include "StdAfx.h"
#include "Move.h"


Move::Move(void)
{
}

Move::Move(int to)
{
	this->positionFrom = 0;
	this->positionTo = to;
}

Move::Move(int from, int to)
{
	this->positionFrom = from;
	this->positionTo = to;
}