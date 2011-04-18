// KaroEngine.h

#pragma once

using namespace System;

	enum PieceColor{
		Black,
		Red
	};

	enum GameState{
		Insertion,
		Play,
		GameFinished
	};

	enum PieceState{
		Marked,
		Unmarked
	};

	public struct Piece
	{
		PieceColor color;
		PieceState state;
	};

	public ref class KaroEngine
	{
		// TODO: Add your methods for this class here.
	private:
		GameState gameState;
		int *board;
		PieceColor turn;

	public:
		KaroEngine(void);
		~KaroEngine(void);
		void DoMove();
		void UndoMove();
		
	};

