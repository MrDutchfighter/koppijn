#pragma once
public class Move
{
public:
	int positionFrom;
	int positionTo;
	int tileFrom;
	int score;
	bool isJumpMove;
	bool isWinningMove;

	Move(void);
	Move(int to);
	Move(int from, int to, bool isJumpMove);
	Move(int from, int to, int tileFrom, bool isJumpMove);
};