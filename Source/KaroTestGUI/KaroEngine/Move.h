#pragma once
public class Move
{
public:
	int positionFrom;
	int positionTo;
	int tileFrom;
	int tileTo;
	int score;
	bool isJumpMove;

	Move(void);
	Move(int to);
	Move(int from, int to, bool isJumpMove);
	Move(int from, int to, int tileFrom, int tileTo, bool isJumpMove);
};