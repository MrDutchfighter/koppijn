#pragma once

#include "KaroEngine.h"
#include <string>
using namespace System;

namespace KaroEngine {
	public ref class KaroEngineWrapper{
	private:
		KaroEngine *_karoEngine; // native pointer
	public:

		KaroEngineWrapper(void);
		~KaroEngineWrapper(void);

		Tile GetByXY(int x, int y);
		bool InsertByXY(int x, int y);
		void DoMove(int from,int to,int tile);
		GameState GetGameState();
		Player GetTurn();
		void CalculateComputerMove();
		String^ GetMessageLog();
		int GetEvaluationScore();
		array<array<int>^>^ GetPossibleMoves(int x, int y);
	};
}