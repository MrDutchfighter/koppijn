#pragma once
public class Move
{
public:
	int positionFrom;
	int positionTo;
	int score;
	Move(void);
	Move(int to);
	Move(int from, int to);
};

