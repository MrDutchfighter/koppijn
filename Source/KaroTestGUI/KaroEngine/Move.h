#pragma once
public class Move
{
public:
	int positionFrom;
	int positionTo;
	int tileFrom;
	int tileTo;
	int score;
	Move(void);
	Move(int to);
	Move(int from, int to, int tileFrom, int tileTo);
};

