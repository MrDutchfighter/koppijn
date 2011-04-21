#pragma once

#include "KaroEngine.h"
#include <string.h>

using namespace std;
using namespace System;

namespace KaroEngine {
	public ref class KaroEngineWrapper{
	private:
		KaroEngine *_karoEngine; // native pointer
	public:

		KaroEngineWrapper(void);
		~KaroEngineWrapper(void);

		Tile GetByXY(int x, int y);
		void DoMove(int from,int to);
		GameState GetGameState();
		Player GetTurn();
	};
}