// KaroEngine.h

#pragma once

using namespace System;

namespace KaroEngine {
	enum Color{
		Black,
		Red
	};

	enum State{
		Marked,
		Unmarked
	};

	public struct Piece
	{
		Color color;
		State state;
	};

	public ref class KaroEngine
	{
		// TODO: Add your methods for this class here.
	private:
		int gameState;
		int board;
		Color turn;

	public:
		void DoMove();
		void UndoMove();
		
	};
}
