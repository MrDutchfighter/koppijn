// This is the main DLL file.

#include "stdafx.h"

#include "KaroEngine.h"
/* example code for move ordering: for move ordering call getpossiblemoves, call the evaluation function, fill the moves with the score and then call sort (vectorList.begin(), vectorList.end(), bigger_than_second);
		Move* move1 = new Move();
		move1->score = 2;
		Move* move2 = new Move();
		move2->score = 1;
		Move* move3 = new Move();
		move3->score = 0;
		Move* move4 = new Move();
		move4->score = 4;
	
		vector<Move*> vectorList;
		vectorList.push_back(move1);
		vectorList.push_back(move2);
		vectorList.push_back(move3);
		vectorList.push_back(move4);
		
		for (int i=0; i<vectorList.size(); i++) {
		 Console::WriteLine(vectorList[i]->score);
		}

		sort (vectorList.begin(), vectorList.end(), bigger_than_second);
		for (int i=0; i<vectorList.size(); i++) {
		 Console::WriteLine(vectorList[i]->score);
		}
		*/
namespace KaroEngine 
{
	KaroEngine::KaroEngine(void)
	{
		this->board = new Tile[BOARDWIDTH * BOARDWIDTH];
		this->turn = Player::WHITE;
		this->gameState = GameState::INSERTION; 
		this->insertionCount = 0;	
		this->maxDepth = 6;
		this->evaluationScore = 0;
		this->visitedList = new VisitedList();

		for(int i = 0; i < BOARDWIDTH * BOARDWIDTH ; i ++ )
			board[i] = Tile::EMPTY;
		for(int y=0;y<BOARDWIDTH;y++){
			for(int x=0;x<BOARDWIDTH;x++){
				if(x==0 || y==0 ||x==(BOARDWIDTH-1) || y==(BOARDWIDTH-1)){
					board[y*BOARDWIDTH + x] = Tile::BORDER;
				}
				else{
					board[y*BOARDWIDTH + x] = Tile::EMPTY;
				}
			}
		}

		for(int j = 4; j < 8; j++)
			for( int k = 5; k < 10; k++ )
				if((j == 4 && k == 5) || (j == 4 && k == 9) || (j == 7 && k == 5) || (j == 7 && k == 9)) {
					board[j  *BOARDWIDTH + k] = Tile::MOVEABLETILE;
					//board[j  *BOARDWIDTH + k] = Tile::SOLIDTILE;
					moveableTiles.insert(std::pair<int,int>((j  *BOARDWIDTH + k),2));
				}
				else {
					board[j * BOARDWIDTH + k] = Tile::SOLIDTILE;
				}

		// Fill the array of possible steps;
		possibleSteps[0]= 0-(BOARDWIDTH-1);
		possibleSteps[1]= 0-BOARDWIDTH;
		possibleSteps[2]= 0-(BOARDWIDTH+1);
		possibleSteps[3]=-1;
		possibleSteps[4]= 1;
		possibleSteps[5]= (BOARDWIDTH-1);
		possibleSteps[6]= BOARDWIDTH;
		possibleSteps[7]= (BOARDWIDTH+1);;

		// Fill the array of possible jumps
		possibleJumps[0]= 0-(BOARDWIDTH*2)+2;
		possibleJumps[1]= 0-(BOARDWIDTH*2);
		possibleJumps[2]= 0-(BOARDWIDTH*2)-2;
		possibleJumps[3]=-2;
		possibleJumps[4]= 2;
		possibleJumps[5]= (BOARDWIDTH*2)-2;
		possibleJumps[6]= (BOARDWIDTH*2);
		possibleJumps[7]= (BOARDWIDTH*2)+2;


		//Fill the random sets with random numbers
		for(int i = 0; i < 289; i++)
		{
			randomTile[i] = GetRandomNumber();
			randomWhiteUnmarked[i] = GetRandomNumber();
			randomRedUnmarked[i] = GetRandomNumber();
			randomWhiteMarked[i] = GetRandomNumber();
			randomRedMarked[i] = GetRandomNumber();
		}
		this->_leftBoundairy.first=5;
		this->_leftBoundairy.second=4;
		this->_topBoundairy.first=4;
		this->_topBoundairy.second=5;
		this->SetMessageLog("Engine Initialized");
	}

	KaroEngine::~KaroEngine(void)
	{
	}
	
	/**
	* Executes a given move (if valid)
	*/
	void KaroEngine::DoMove(int from, int to, int tileFrom)
	{
		bool validMove=false;
		Move* move;
		// Boolean is neccesairy for future things
		if(turn== Player::RED){
			if(board[from] != Tile::REDUNMARKED &&
				board[from] != Tile::REDMARKED) {
					return;
			}
		}
		else if(turn== Player::WHITE){
			if(board[from] != Tile::WHITEUNMARKED &&
				board[from] != Tile::WHITEMARKED) {
					return;
			}
		}

		vector<Move*>* moves= this->GetPossibleMoves(from, true);
		for(int i=0;i<moves->size();i++){
			if(moves->at(i)->positionTo == to && moves->at(i)->positionFrom == from && moves->at(i)->tileFrom == tileFrom) {
				move = moves->at(i);
				validMove = true;
				continue;
			}
		}

		// If not a valid move, then return, stop proces
		if(!validMove) {
			return;
		}
		else {
			DoMove(move);

			int connectedTiles = this->GetAmountConnectedTiles(to);
			std::string s;
			std::stringstream stringstream;
			stringstream << connectedTiles;
			s = stringstream.str();
			this->SetMessageLog("Amount of connected tiles: "+s);
			if(this->IsWinner(Reverse(turn), to))
			{
				this->SetMessageLog("WIN!");
			}
		}
	}

	/**
	* Inserts a tile on the given position
	*/
	bool KaroEngine::InsertByXY(int position) {
		if(board[position] == Tile::SOLIDTILE || board[position] == Tile::MOVEABLETILE ){
		
			map<int,bool> whitePieces2 = whitePieces;
			map<int,bool> redPieces2 = redPieces;

			Move * x = new Move(position);

			DoMove(x);

			//turn = this->Reverse(turn);
			insertionCount++;							
			if(insertionCount == 12) {
				gameState = GameState::PLAYING;
			}
			return true;
		}
		return false;
	}

	/**
	* Switches the turn
	*/
	Player KaroEngine::Reverse(Player turnPro)
	{
		switch(turnPro)
		{
			case Player::WHITE:
				return Player::RED;
			case Player::RED:
				return Player::WHITE;
			default:
				return turnPro;
		}
	}

	/**
	* Checks if from 'from' to 'to' is a valid move
	*/
	bool KaroEngine::IsValidMove(int from, int to)
	{
		// check if the move is valid by validating with the turn of the current player
		if(turn == Player::RED) {
			if(board[from] != Tile::REDUNMARKED && board[from] != Tile::REDMARKED)
				return false;
		}

		else if(turn == Player::WHITE) {
			if(board[from] != Tile::WHITEUNMARKED && board[from] != Tile::WHITEMARKED)
				return false;
		}

		if(board[to] != Tile::SOLIDTILE && board[to] != Tile::MOVEABLETILE) {
			return false;
		}

		int moved = to - from;

		// If possible move ( one step )
		for(int i=0;i<8;i++){
			if(possibleSteps[i]==moved) {
				return true;
			}
		}

		//check if it is a jumpmove
		for(int i=0;i<8;i++){
			if(possibleJumps[i]==moved){
				//if the jump is possible, check if there is a piece between the 2 places.
				int checkingTilenr=from+possibleSteps[i];
				Tile checkTile=board[checkingTilenr];

				if(checkTile== Tile::WHITEUNMARKED ||
					checkTile == Tile::WHITEMARKED ||
					checkTile == Tile::REDUNMARKED ||
					checkTile == Tile::REDMARKED) {
						this->SetMessageLog("Jumped succesfully");
						//switch the mark
						switch(board[from]){
							case Tile::WHITEUNMARKED:
								board[from]=Tile::WHITEMARKED;
							break;
							case Tile::WHITEMARKED:
								board[from]=Tile::WHITEUNMARKED;
							break;
							case Tile::REDUNMARKED:
								board[from]=Tile::REDMARKED;
							break;
							case Tile::REDMARKED:
								board[from]=Tile::REDUNMARKED;
							break;
						}
						return true;
				}
			}
		}

		return false; // VICTORIOUSSSSS
	}	
	/**
	*returns the pieces from the Player (p)
	**/
	map<int,bool> KaroEngine::GetPlayerPieces(Player p)
	{
		if(p == Player::WHITE)
			return whitePieces;
		else if(p == Player::RED)
			return redPieces;
	}

	/**
	* Checks if the player (p) has won the game by moving his last tile
	*/
	bool KaroEngine::IsWinner(Player p, int lastMove)
	{
		Tile marked;

		//Right player color 
		if (p == Player::WHITE) 
			marked = Tile::WHITEMARKED;
		if (p == Player::RED)
			marked = Tile::REDMARKED;

		//check if first piece is marked
		if(board[lastMove] == marked)
		{			
			int countUnMarked = 0;
			for each (pair<int, bool> piece in GetPlayerPieces(p))
			{
				//check if four pieces are Marked else return false
				if(!piece.second)
					countUnMarked++;
				if(countUnMarked > 2)
					return false;

				for(int i = 0; i < 8; i++)
				{
					//check if piece in possibleStep
					if(piece.first == lastMove + possibleSteps[i])
					{
						//check if second is marked
						int second = lastMove + possibleSteps[i];
						if(board[second] != marked)
							break;
												
						int difference = second - lastMove;

						//check if third is unmarked
						int third = second + difference;
						if(board[third] != marked)
						{
							//check if minfirst is unmarked 
							int minFirst = lastMove - difference;
							if(board[minFirst] != marked)
								break;
							else
							{
								//check if minfirst is marked and is winning
								int minSecond = minFirst - difference;
								if(board[minSecond] == marked)
									return true;
							}
						}
						else
						{
							//check if fourth is marked and is winning
							int fourth = third + difference;
							if(board[fourth] == marked)
								return true;
						}
					}
				}
			}
		}		

		return false;
	}

	/**
	* Evaluates the amount of rows
	*/
	int KaroEngine::EvaluateNumRows(Player p, int pieceIndex)
	{
		int score = 0;
		Tile marked;
		map<int,bool> pieces;

		//Right player color 
		if (p == Player::WHITE) 
		{
			marked = Tile::WHITEMARKED;
			pieces = whitePieces;
		}
		if (p == Player::RED)
		{
			marked = Tile::REDMARKED;
			pieces = redPieces;
		}

			//check if at least 2 pieces are Marked else return false
			int countUnMarked = 0;
			for each (pair<int, bool> p in pieces)
			{
				if(!p.second)
					countUnMarked++;
				if(countUnMarked > 5)
					return 0;
			}

			for each (pair<int, bool> piece in pieces)
			{
				for(int i = 0; i < 8; i++)
				{
					//check if piece in possibleStep
					if(piece.first == pieceIndex + possibleSteps[i])
					{
						//check if second is marked
						int second = pieceIndex + possibleSteps[i];
						if(board[second] != marked)
						{
							break;
						}
						
						score = score + 2; // 2 in a row
						int difference = second - pieceIndex;

						//check if third is unmarked
						int third = second + difference;
						if(board[third] != marked)
						{
							//check if minfirst is unmarked 
							int minFirst = pieceIndex - difference;
							if(board[minFirst] != marked)
							{
								break;
							}
							else
							{
								score= score + 4; // 3 in a row
							}
						}
					}
				}
			}		

		return score;
	}

	/**
	* Calculates the next computer move
	*/
	float KaroEngine::CalculateComputerMove() {
		
		__int64 ctr1 = 0, ctr2 = 0, freq = 0;
		int acc = 0, i = 0;

		QueryPerformanceCounter((LARGE_INTEGER *)&ctr1);

		// If the game is in insertion state, insert a random item on a tile
		if(gameState == GameState::INSERTION) {
			bool foundInsertPosition=false;
			while(!foundInsertPosition){
				int x = 5+rand()%5;
				int y = 4+rand()%4;

				int position=(y*BOARDWIDTH)+x;

				if(this->InsertByXY(position)){
					foundInsertPosition=true;
				}
			}
		} else if(gameState == GameState::PLAYING) {
			// Generate a real computer move with minmax
			int hash= GetHash();
			Move * theMove = MiniMax(GetTurn(), 0, INT_MIN, INT_MAX,hash);

			// Execute the final move
			if(theMove->positionFrom > 0) {
				DoMove(theMove);
			} else {
				SetMessageLog("Geen move gevonden");
			}
		}
		QueryPerformanceCounter((LARGE_INTEGER *)&ctr2);
		QueryPerformanceFrequency((LARGE_INTEGER *)&freq);
		
		char* str = new char[30];
 
		float flt = ((ctr2 - ctr1) * 1.0 / freq);
		sprintf(str, "%.4g", flt );    
		
		std::string s((LPCSTR)str);
		s = "Move took " + s + " seconds";
		SetMessageLog(s);

		return flt;
	}

	/**
	* Get all the possible moves for one player
	*/
	vector<Move*>* KaroEngine::GetPossibleMoves(Player forPlayer) {
		vector<Move*> *possibleMoves = new vector<Move*>();

		// Loop through all the stones of the current player
		if(forPlayer == Player::RED) {
			if(!this->redPieces.empty()) {
				for(std::map<int, bool>::iterator it = this->redPieces.begin(); it != this->redPieces.end(); ++it) {
					
					int i = it->first;
					Tile x = board[it->first];
					if(board[it->first] != Tile::REDMARKED && board[it->first] != Tile::REDUNMARKED) {
						return new vector<Move*>();
					}
					
					vector<Move*> *moves = GetPossibleMoves(it->first, it->second);
					possibleMoves->insert(possibleMoves->end(), moves->begin(), moves->end());
				}
			}
		} else if(forPlayer == Player::WHITE) {
			if(!this->whitePieces.empty()) {
				for(std::map<int, bool>::iterator it = this->whitePieces.begin(); it != this->whitePieces.end(); ++it) {

					int i = it->first;
					Tile x = board[it->first];
					if(board[it->first] != Tile::WHITEMARKED && board[it->first] != Tile::WHITEUNMARKED) {
						return new vector<Move*>();
					}

					vector<Move*> *moves = GetPossibleMoves(it->first, it->second);
					possibleMoves->insert(possibleMoves->end(), moves->begin(), moves->end());
				}
			}
		}

		return possibleMoves;
	}

	/**
	* Get all the possible moves from the current tile
	*/
	vector<Move*>* KaroEngine::GetPossibleMoves(int curTile, bool isTurned) {

		vector<Move*> *possibleMoves = new vector<Move*>();
		//vector<int> *moveableTileIndexes = new vector<int>();

		if(this->board[curTile]==Tile::EMPTY || 
			this->board[curTile]==Tile::BORDER ||
			(turn==Player::RED && this->board[curTile]==Tile::WHITEMARKED) ||
			(turn==Player::RED && this->board[curTile]==Tile::WHITEUNMARKED) ||
			(turn==Player::WHITE && this->board[curTile]==Tile::REDMARKED) ||
			(turn==Player::WHITE && this->board[curTile]==Tile::REDUNMARKED)) {
				return new vector<Move*>();
		} 
		
		/*
		for(int i=0;i<289;i++){
			if(board[i]==Tile::MOVEABLETILE){
				moveableTileIndexes->push_back(i);
			}
		}*/

		// Check all the possible (normal) moves
		for(int i=0; i<8; i++) {
				// Kan ik verplaatsen naar deze tegel
			if(board[curTile+possibleSteps[i]] == Tile::SOLIDTILE ||
				board[curTile+possibleSteps[i]] == Tile::MOVEABLETILE ||
				(moveableTiles.size() > 0 && GetAmountConnectedNeighbours(curTile+possibleSteps[i]) > 0 && board[curTile+possibleSteps[i]] == Tile::EMPTY)) {
					
					// Willen we een tegel mee verplaatsen?
					if(board[curTile+possibleSteps[i]] == Tile::EMPTY) {
						for each(std::pair<int,int> j in moveableTiles) {

							int neighbours = 1;
							if(i == 0 || i == 2 || i == 5 || i == 7) {
								//Tile tempTile= board[j.first];
								board[j.first] = Tile::EMPTY;
								neighbours = GetAmountConnectedNeighbours(curTile+possibleSteps[i]);
								//board[j.first] = Tile::MOVEABLETILE;
								board[j.first] = Tile::MOVEABLETILE;
							}

							if(neighbours > 0) {
								possibleMoves->push_back(new Move(curTile, (curTile+possibleSteps[i]), j.first, false));
							}
						}
					} else {
						possibleMoves->push_back(new Move(curTile, (curTile+possibleSteps[i]), false));
					}
			}
						// Kijken of er een pion tussenin staat
			else if (board[curTile+possibleSteps[i]] != Tile::EMPTY &&
						board[curTile+possibleSteps[i]] != Tile::BORDER) {
					
					// Kijken of tegel vrij is
				if(board[curTile+possibleJumps[i]] == Tile::SOLIDTILE ||
					board[curTile+possibleJumps[i]] == Tile::MOVEABLETILE ||
					(moveableTiles.size() > 0 && GetAmountConnectedNeighbours(curTile+possibleJumps[i]) > 0 && board[curTile+possibleJumps[i]] == Tile::EMPTY)) {
						
						// Willen we een tegel mee verplaatsen?
						if(board[curTile+possibleJumps[i]] == Tile::EMPTY) {
							for each(std::pair<int,int> j in moveableTiles) {

							int neighbours = 1;
							if(i == 0 || i == 2 || i == 5 || i == 7) {
								//Tile tempTile= board[j.first];
								board[j.first] = Tile::EMPTY;
								neighbours = GetAmountConnectedNeighbours(curTile+possibleJumps[i]);
								//board[j.first] = Tile::MOVEABLETILE;
								board[j.first] = Tile::MOVEABLETILE;
							}
							if(neighbours > 0) {
								possibleMoves->push_back(new Move(curTile, (curTile+possibleJumps[i]), j.first, true));
							}
						}
						} else {
							possibleMoves->push_back(new Move(curTile, (curTile+possibleJumps[i]), true));
						}
				}
			}
		}

		return possibleMoves;
	}

	/**
	* Checks & transforms neighbours and self for movable tiles
	*/
	void KaroEngine::TransformToMovableTiles(int tileNumber, bool checkNeighbours = true, bool checkDiagonalNeighbours = false) {
		//return;
		// Staat er niks op deze tile?
		if(board[tileNumber] == Tile::SOLIDTILE || board[tileNumber] == Tile::MOVEABLETILE) {
			
			//if(board[tileNumber] == Tile::MOVEABLETILE) {
				moveableTiles.erase(tileNumber);
			//}
			board[tileNumber] = Tile::EMPTY;

			int amountNeighbours = GetAmountConnectedNeighbours(tileNumber);
			if(amountNeighbours <= 2 && amountNeighbours > 0) {
				int totalConnected = 0;
				if(this->board[tileNumber-1] != Tile::EMPTY && this->board[tileNumber-1] != Tile::BORDER) {
					totalConnected = GetAmountConnectedTiles(tileNumber-1);
				}
				else if(this->board[tileNumber-BOARDWIDTH] != Tile::EMPTY && this->board[tileNumber-BOARDWIDTH] != Tile::BORDER){
					totalConnected = GetAmountConnectedTiles(tileNumber-BOARDWIDTH);
				}
				else if(this->board[tileNumber+1] != Tile::EMPTY && this->board[tileNumber+1] != Tile::BORDER){
					totalConnected = GetAmountConnectedTiles(tileNumber+1);
				}
				else if(this->board[tileNumber+BOARDWIDTH] != Tile::EMPTY && this->board[tileNumber+BOARDWIDTH] != Tile::BORDER){
					totalConnected = GetAmountConnectedTiles(tileNumber+BOARDWIDTH);
				}

				// Het bord is nog compleet!
				if(totalConnected == 19) {
					board[tileNumber] = Tile::MOVEABLETILE;
					moveableTiles.insert(std::pair<int,int>(tileNumber, amountNeighbours));
				} else { 
					// Het bord is niet meer compleet!
					board[tileNumber] = Tile::SOLIDTILE;
				}
			}
			else {
				board[tileNumber] = Tile::SOLIDTILE;
			}
		} else {
			moveableTiles.erase(tileNumber);
		}

		// Check the neighbours
		if(checkNeighbours) {
			if(this->board[tileNumber-1] != Tile::BORDER) {
				TransformToMovableTiles(tileNumber-1, false);
			}
			if(this->board[tileNumber-BOARDWIDTH] != Tile::BORDER){
				TransformToMovableTiles(tileNumber-BOARDWIDTH, false);
			}
			if(this->board[tileNumber+1] != Tile::BORDER){
				TransformToMovableTiles(tileNumber+1, false);
			}
			if(this->board[tileNumber+BOARDWIDTH] != Tile::BORDER){
				TransformToMovableTiles(tileNumber+BOARDWIDTH, false);
			}
		}

		// Check the awesome neighbours
		if(checkDiagonalNeighbours) {
			if(this->board[tileNumber-(BOARDWIDTH+1)] != Tile::BORDER) {
				TransformToMovableTiles(tileNumber-(BOARDWIDTH+1), false);
			}
			if(this->board[tileNumber-(BOARDWIDTH-1)] != Tile::BORDER){
				TransformToMovableTiles(tileNumber-(BOARDWIDTH-1), false);
			}
			if(this->board[tileNumber+(BOARDWIDTH+1)] != Tile::BORDER){
				TransformToMovableTiles(tileNumber+(BOARDWIDTH+1), false);
			}
			if(this->board[tileNumber+(BOARDWIDTH-1)] != Tile::BORDER){
				TransformToMovableTiles(tileNumber+(BOARDWIDTH-1), false);
			}
		}
	}

	/**
	* Make a call to find the Amount of Amount of connected tiles
	*/
	int KaroEngine::GetAmountConnectedTiles(int tileNumber){
		visitedList->Clear();
		return GetAmountConnectedTilesRecursive(tileNumber);
	}

	/**
	* Make a recursive call to find the Amount of connected tiles
	*/
	int KaroEngine::GetAmountConnectedTilesRecursive(int tileNumber){
		int AmountTiles = 0;
		if(this->board[tileNumber] == Tile::EMPTY || this->board[tileNumber] == Tile::BORDER){
			return AmountTiles;
		}
		if(this->visitedList->isInArray(tileNumber)){
			return AmountTiles;
		}
		this->visitedList->insertAt(tileNumber);
		AmountTiles+=1;
		AmountTiles+=this->GetAmountConnectedTilesRecursive(tileNumber-1);
		AmountTiles+=this->GetAmountConnectedTilesRecursive(tileNumber-BOARDWIDTH);
		AmountTiles+=this->GetAmountConnectedTilesRecursive(tileNumber+1);
		AmountTiles+=this->GetAmountConnectedTilesRecursive(tileNumber+BOARDWIDTH);
		return AmountTiles;
	}

	/**
	* Check how many neighbours the tile had
	*/
	int KaroEngine::GetAmountConnectedNeighbours(int tileNumber){
		int AmountTiles=0;
		if(this->board[tileNumber-1] != Tile::EMPTY && this->board[tileNumber-1] != Tile::BORDER){
			AmountTiles+=1;
		}
		if(this->board[tileNumber-BOARDWIDTH] != Tile::EMPTY && this->board[tileNumber-BOARDWIDTH] != Tile::BORDER){
			AmountTiles+=1;
		}
		if(this->board[tileNumber+1] != Tile::EMPTY && this->board[tileNumber+1] != Tile::BORDER){
			AmountTiles+=1;
		}
		if(this->board[tileNumber+BOARDWIDTH] != Tile::EMPTY && this->board[tileNumber+BOARDWIDTH] != Tile::BORDER){
			AmountTiles+=1;
		}
		return AmountTiles;
	}

	/**
	* Can someone place his 'marble' here
	*/
	bool KaroEngine::FreeForMove(int tile)
	{
		if(board[tile] == Tile::SOLIDTILE || board[tile] == Tile::MOVEABLETILE) {
			return true;
		}
		return false;
	}

	/**
	* Does this tile takes part in the game
	*/
	bool KaroEngine::IsGameTile(int tile)
	{
		return (board[tile] != Tile::EMPTY && board[tile] != Tile::BORDER);
	}

	/**													//
	* --------------- Algorithm functions ---------		//
	*/													//

	/**
	* MinMax function
	*/
	Move * KaroEngine::MiniMax(Player p, int depth, int alpha, int beta,int hash)
	{
		// Hash the current board?
		//Position currentPosition = new Position(board);
		p = turn;
		// Create new move
		Move *bestMove = new Move();
		if(p == Player::RED) {
			bestMove->score = INT_MIN; // Int32.MinValue
		} else {
			bestMove->score = INT_MAX; // Int32.MaxValue
		}


		//First check if the score is already know, then evaluate. 
		if(depth != 0) {
			// Is this move in the transposition table?
			//int hash = GetHash();
			map<int,int>::iterator it = transpositionTable.find(hash);
            if (it != transpositionTable.end())
            {
                bestMove->score = it->second;
                return bestMove;
            }
		}

		// Evaluate the current board, game ended? Return empty move with the max/min score
		int scoreRed = EvaluateBoard(Player::RED);
		int scoreWhite = EvaluateBoard(Player::WHITE);
		int evaluationScore = scoreRed-scoreWhite;

		// If maximum depth is reached
		if(depth == maxDepth) {
			bestMove->score = evaluationScore;
			return bestMove;
		}		

		// Find next moves for the current player
		vector<Move*> * possibleMoves = GetPossibleMoves(p);

		// Loop through all the moves
		for(int i=0; i < possibleMoves->size(); i++) {
			// Execute the move
			DoMove(possibleMoves->at(i));
			_boundairyChanged=false;
			//Check the boundairies
			//boundairy left check the FROM position
			if(possibleMoves->at(i)->positionFrom%BOARDWIDTH == this->_leftBoundairy.first ){
				this->_leftBoundairy.second-=1;
				if(this->_leftBoundairy.second==0){ //if zero reset the left boundairy
					this->_leftBoundairy.first+=1;
					this->_leftBoundairy.second=0;
					for(int i=0;i<BOARDWIDTH;i++){
						if(board[this->_leftBoundairy.first+(i*BOARDWIDTH)] != Tile::EMPTY || board[this->_leftBoundairy.first+(i*BOARDWIDTH)] != Tile::BORDER){
							this->_leftBoundairy.second+=1;
						}						
					}
					_boundairyChanged=true;
				}
			}

			//boundairy Top check the FROM position
			if(possibleMoves->at(i)->positionFrom/BOARDWIDTH == this->_topBoundairy.first ){
				this->_topBoundairy.second-=1;
				if(this->_topBoundairy.second==0){ //if zero reset the left boundairy
					this->_topBoundairy.first+=1;
					this->_topBoundairy.second=0;
					for(int i=0;i<BOARDWIDTH;i++){
						if(board[i+(this->_leftBoundairy.first*BOARDWIDTH)] != Tile::EMPTY || board[i+(this->_leftBoundairy.first*BOARDWIDTH)] != Tile::BORDER){
							this->_leftBoundairy.second+=1;
						}
					}
					_boundairyChanged=true;
				}
			}
			
			//Check the TO position
			//boundairy left
			if(possibleMoves->at(i)->positionTo%BOARDWIDTH < this->_leftBoundairy.first ){
				this->_leftBoundairy.first=possibleMoves->at(i)->positionTo%BOARDWIDTH;
				this->_leftBoundairy.second=1;
				_boundairyChanged=true;
			}
			if(possibleMoves->at(i)->positionTo%BOARDWIDTH == this->_leftBoundairy.first ){
				this->_leftBoundairy.second+=1;
			}

			//boundairy top
			if(possibleMoves->at(i)->positionTo/BOARDWIDTH < this->_topBoundairy.first ){
				this->_topBoundairy.first=possibleMoves->at(i)->positionTo/BOARDWIDTH;
				this->_topBoundairy.second=1;
				_boundairyChanged=true;
			}
			if(possibleMoves->at(i)->positionTo/BOARDWIDTH == this->_topBoundairy.first ){
				this->_topBoundairy.second+=1;
			}
			if(_boundairyChanged){
				hash=GetHash();
			}else{
				hash=GetHash(hash,possibleMoves->at(i));
			}

			// Was this the winning move? (has to be here, because IsWinner needs the last move...)
			if(IsWinner(p, possibleMoves->at(i)->positionTo)) {
				bestMove = possibleMoves->at(i);
				if(p == Player::RED) {
					bestMove->score = INT_MAX-10000;
				} else if(p == Player::WHITE) {
					bestMove->score = INT_MIN+10000;
				}
				UndoMove(possibleMoves->at(i));
				return bestMove;
			}

			// Get the last best move
			//Move * lastBestMove = possibleMoves->at(i);
			Move * lastBestMove = MiniMax(Reverse(p), depth+1, alpha, beta,hash);

			// Directly undo this move
			UndoMove(possibleMoves->at(i));

			// Was the last move the best move?
			if(lastBestMove->score > bestMove->score && p == Player::RED) {
				bestMove = possibleMoves->at(i);
				bestMove->score = lastBestMove->score;
			} else if(lastBestMove->score < bestMove->score && p == Player::WHITE) {
				bestMove = possibleMoves->at(i);
				bestMove->score = lastBestMove->score;
			}

			// Is current player RED?
			if(p == Player::RED) {
				if(bestMove->score > alpha) {
					alpha = bestMove->score;
				}
			}

			if(p == Player::WHITE) {
				if(bestMove->score < beta) {
					beta = bestMove->score;
				}
			}

			// Prunning
			if(beta <= alpha) {
				return bestMove;
			}
		}

		// Put best score in transposition table
		if(depth == 1 || depth == 2) {		
			//hash=GetHash();
			map<int,int>::iterator it = transpositionTable.find(hash);
			if (it == transpositionTable.end()){
                transpositionTable.insert(pair<int,int>(hash, bestMove->score));
			}else{
				if(it->second != bestMove->score){
					SetMessageLog("Tried to insert a double hash value for different value..");
				}
			}
		}

		return bestMove;
	}

	/**
	* Evaluation function of the current board for the given player
	*/
	int KaroEngine::EvaluateBoard(Player p)
	{
		int calculatedScore = 0;
		switch(p)
		{
		case Player::WHITE:
			{
				if(!this->whitePieces.empty()) {
					for(std::map<int, bool>::iterator it = this->whitePieces.begin(); it != this->whitePieces.end(); ++it) {
						if (it->second == true)
						{
							calculatedScore += 2;
							calculatedScore += 0;
							//calculatedScore += this->EvaluateNumRows(p, it->first);
						}
							
					}
				}
			break;
			}
		case Player::RED:
			{
				if(!this->redPieces.empty()) {
					for(std::map<int, bool>::iterator it = this->redPieces.begin(); it != this->redPieces.end(); ++it) {
						if (it->second == true)
						{
							calculatedScore += 2;
							calculatedScore += 0;//this->EvaluateNumRows(p, it->first);
						}
					}
				}
			break;
			}
		}
		this->evaluationScore = this->evaluationScore; // DON'T USE IT!
		return calculatedScore;
	}

	/**													//
	* --------------- Getters ------------------------	//
	*/													//
	std::string KaroEngine::GetMessageLog(){
		std::string s = this->messageLog;
		this->messageLog="";
		return s;
	}

	int * KaroEngine::GetBoard(void)
	{
		int * ret = new int[BOARDWIDTH * BOARDWIDTH];
		
		for (int i = 0 ; i < BOARDWIDTH * BOARDWIDTH; i ++)
		{
			ret[i] = (int)this->board[i];
		}

		return ret;
	}

	Player KaroEngine::GetTurn()
	{
		return turn;
	}

	Tile KaroEngine::GetByXY(int x,int y){
		return board[(y*BOARDWIDTH)+x];
	}

	GameState KaroEngine::GetGameState(){
		return this->gameState;
	}
	
	int KaroEngine::GetEvaluationScore()
	{
		return EvaluateBoard(Player::RED)-EvaluateBoard(Player::WHITE);
	}

	/**													//
	* --------------- Setters ------------------------	//
	*/													//
	void KaroEngine::SetMessageLog(std::string txt){
		this->messageLog+=txt+ "\r\n";
	}

	int KaroEngine::GetHash()
	{
		/*
		vector<int> tileIndexes; // found tiles
		int left = BOARDWIDTH; // most left tile
		int right = 0; // most right tile

		int count = 0; // tile index for iterating

		while(tileIndexes.size() != 20)
		{
			if(board[count] != Tile::EMPTY && board[count] != Tile::BORDER)
			{
				int tempCol = count % BOARDWIDTH;
				
				if(tempCol < left)
					left = tempCol;
				if(tempCol > right)
					right = tempCol;

				tileIndexes.push_back(count);
			}
			count ++;
		}

		int topRow = ceil((float)tileIndexes[0] / BOARDWIDTH) - 1;
		int topLeftCorner = (topRow * BOARDWIDTH) + left;
		int bottomRow = ceil((float)tileIndexes[19] / BOARDWIDTH) - 1;
		int bottomLeftCorner = (bottomRow * BOARDWIDTH) + left;

		int squareWidth = right - left;

		int hash = 0;
		for(int i = topLeftCorner; i < bottomLeftCorner; i += BOARDWIDTH)
		{
			for(int k = 0; k < squareWidth; k++)
			{
				int index = i + k;
				int randomIndex = (index - (left -1 )) - (topRow * BOARDWIDTH);

				if(board[index] != Tile::EMPTY)
				{
					if(board[index] == Tile::SOLIDTILE || board[index] == Tile::MOVEABLETILE)
						hash ^= randomTile[randomIndex];
					else if(board[index] == Tile::REDMARKED)
						hash ^= randomRedMarked[randomIndex];
					else if(board[index] == Tile::WHITEMARKED)
						hash ^= randomWhiteMarked[randomIndex];
					else if(board[index] == Tile::REDUNMARKED)
						hash ^= randomRedUnmarked[randomIndex];
					else if(board[index] == Tile::WHITEUNMARKED)
						hash ^= randomWhiteUnmarked[randomIndex];
				}
			}
		}

		*/

		int hash = 0;
		for(int i = 0; i < BOARDWIDTH * BOARDWIDTH; i++)
		{
			if(board[i] != Tile::EMPTY)
			{
				if(board[i] == Tile::SOLIDTILE || board[i] == Tile::MOVEABLETILE)
					hash ^= randomTile[i];
				else if(board[i] == Tile::REDMARKED)
					hash ^= randomRedMarked[i];
				else if(board[i] == Tile::WHITEMARKED)
					hash ^= randomWhiteMarked[i];
				else if(board[i] == Tile::REDUNMARKED)
					hash ^= randomRedUnmarked[i];
				else if(board[i] == Tile::WHITEUNMARKED)
					hash ^= randomWhiteUnmarked[i];
			}
		}
		return hash;
	}
	
	int KaroEngine::GetHash(int hash,Move *move){
		if(board[move->positionTo] == Tile::REDMARKED){
			hash ^= randomRedMarked[move->positionTo];
			if(move->isJumpMove){
				hash ^= randomRedUnmarked[move->positionFrom];
			}
			else{
				hash ^= randomRedMarked[move->positionFrom];
			}
		}
		else if(board[move->positionTo] == Tile::REDUNMARKED){
			hash ^= randomRedUnmarked[move->positionTo];
			if(move->isJumpMove){
				hash ^= randomRedMarked[move->positionFrom];
			}
			else{
				hash ^= randomRedUnmarked[move->positionFrom];
			}
		}
		else if(board[move->positionTo] == Tile::WHITEMARKED){
			hash ^= randomWhiteMarked[move->positionTo];
			if(move->isJumpMove){
				hash ^= randomWhiteUnmarked[move->positionFrom];
			}
			else{
				hash ^= randomWhiteMarked[move->positionFrom];
			}
		}
		else if(board[move->positionTo] == Tile::WHITEUNMARKED){
			hash ^= randomWhiteUnmarked[move->positionTo];
			if(move->isJumpMove){
				hash ^= randomWhiteMarked[move->positionFrom];
			}
			else{
				hash ^= randomWhiteUnmarked[move->positionFrom];
			}
		}
		return hash;
	}

	int KaroEngine::GetRandomNumber()
	{
		int randomNumber = rand() % 899999 + 100000;

		for(int i = 0; i < sizeof(randomTile) / sizeof(randomTile[0]); i++){
			if(randomTile[i] == randomNumber)
				randomNumber = GetRandomNumber();
		}
		for(int i = 0; i < sizeof(randomRedUnmarked) / sizeof(randomRedUnmarked[0]); i++){
			if(randomRedUnmarked[i] == randomNumber)
				randomNumber = GetRandomNumber();
		}
		for(int i = 0; i < sizeof(randomWhiteUnmarked) / sizeof(randomWhiteUnmarked[0]); i++){
			if(randomWhiteUnmarked[i] == randomNumber)
				randomNumber = GetRandomNumber();
		}
		for(int i = 0; i < sizeof(randomRedMarked) / sizeof(randomRedMarked[0]); i++){
			if(randomRedMarked[i] == randomNumber)
				randomNumber = GetRandomNumber();
		}
		for(int i = 0; i < sizeof(randomWhiteMarked) / sizeof(randomWhiteMarked[0]); i++){
			if(randomWhiteMarked[i] == randomNumber)
				randomNumber = GetRandomNumber();
		}

		return randomNumber;
	}


	/**													//
	* --------------- DO MOVE ------------------------	//
	*/													//
	// Exectues the given move
		// The move has to be valid!
	void KaroEngine::DoMove(Move *move)
	{
		map<int,bool> whitePieces2 = whitePieces;
		map<int,bool> redPieces2 = redPieces;
		map<int,int> moveableTiles2 = moveableTiles;

		if(move->positionFrom > 0 && move->positionTo > 0 && move->tileFrom > 0) {
			DoMove(move->positionFrom, move->positionTo, move->tileFrom, move->isJumpMove);

			// Check all changed positions for a change in moveable tiles
			TransformToMovableTiles(move->positionFrom, true, false);
			TransformToMovableTiles(move->positionTo, true, true);
			TransformToMovableTiles(move->tileFrom, true, true);

		} else if(move->positionFrom > 0 && move->positionTo > 0) {
			DoMove(move->positionFrom, move->positionTo, move->isJumpMove);

			// Check all changed positions for a change in moveable tiles
			TransformToMovableTiles(move->positionFrom, true, false);
			TransformToMovableTiles(move->positionTo, true, false);

		} else if(move->positionTo > 0) {
			DoMove(move->positionTo);

			// Check all changed positions for a change in moveable tiles
			TransformToMovableTiles(move->positionTo, true, false);
		} else {
			SetMessageLog("Not a valid move given");
		}

		

		turn = Reverse(turn);
		lastMove = move;
	}

	// Do insert move
	bool KaroEngine::DoMove(int to) {
		if(board[to] == Tile::SOLIDTILE || board[to] == Tile::MOVEABLETILE) {
			board[to] = (turn == Player::RED) ? Tile::REDUNMARKED : Tile::WHITEUNMARKED;
		} else {
			SetMessageLog("[ERROR] You can't place a pawn here!");
			return false;
		}

		// Put the whitePieces / redPieces into place
		if(turn == Player::RED) {
			redPieces.insert(std::pair<int,bool>(to,false));
		}
		else if(turn == Player::WHITE) {
			whitePieces.insert(std::pair<int,bool>(to,false));
		} else {
			SetMessageLog("[ERROR] An invalid player inserted a piece!");
			return false;
		}
		return true;
	}

	// Do a normal move (or jump move)
	bool KaroEngine::DoMove(int from, int to, bool isJumpMove) {
		
		Tile x = board[from];
		Tile y = board[to];

		if((board[to] == Tile::SOLIDTILE || board[to] == Tile::MOVEABLETILE) && board[from] != Tile::EMPTY && board[from] != Tile::BORDER) {
			if(!isJumpMove) {
				board[to] = board[from];

				bool isTurned = (board[to] == Tile::WHITEMARKED || board[to] == Tile::REDMARKED) ? true : false;
				if(board[to] == Tile::WHITEMARKED || board[to] == Tile::WHITEUNMARKED) {
					whitePieces.erase(from);
					whitePieces.insert(std::pair<int,bool>(to,isTurned));
				} else if(board[to] == Tile::REDMARKED || board[to] == Tile::REDUNMARKED) {
					redPieces.erase(from);
					redPieces.insert(std::pair<int,bool>(to,isTurned));
				} else {
					SetMessageLog("[ERROR] No valid tile placed on the board[to].");
					return false;
				}

			} else {
					// Was the old pawn from player white?
				if(board[from] == Tile::WHITEUNMARKED || board[from] == Tile::WHITEMARKED) {
					board[to] = (board[from] == Tile::WHITEUNMARKED) ? Tile::WHITEMARKED : Tile::WHITEUNMARKED;

					bool isTurned = (board[to] == Tile::WHITEMARKED) ? true : false;
					whitePieces.erase(from);
					whitePieces.insert(std::pair<int,bool>(to,isTurned));
					
					// Was the old pawn from player red?
				} else if(board[from] == Tile::REDUNMARKED || board[from] == Tile::REDMARKED) {
					board[to] = (board[from] == Tile::REDUNMARKED) ? Tile::REDMARKED : Tile::REDUNMARKED;

					bool isTurned = (board[to] == Tile::REDMARKED) ? true : false;
					redPieces.erase(from);
					redPieces.insert(std::pair<int,bool>(to,isTurned));
					
					// Was it an invalid pawn?
				} else {
					SetMessageLog("[ERROR] Position from is not valid a valid tile!");
					return false;
				}
			}

			// Remove the tile from the 'from' position
			board[from] = Tile::SOLIDTILE;
		} else {
			SetMessageLog("[ERROR] You can't place a pawn here, or the 'from' place is empty!");
			return false;
		}

		return true;
	}

	// Do a normal move and replace the tile (including jumpMove)
	bool KaroEngine::DoMove(int from, int to, int tileFrom, bool isJumpMove) {
		
		if(board[tileFrom] != Tile::MOVEABLETILE) {
			SetMessageLog("[ERROR] This is not a moveable tile!");
			return false;
		}

		if(board[to] != Tile::EMPTY) {
			SetMessageLog("[ERROR] Can't place a tile here, the spot is not empty!");
			return false;
		}

		board[to] = Tile::SOLIDTILE;
		
		// The same as a normal move, only empty the "tileFrom"
		if(DoMove(from, to, isJumpMove)) {
			board[tileFrom] = Tile::EMPTY;
		} else {
			return false;
		}

		return true;
	}

	/**													//
	* --------------- UNDO MOVE ----------------------	//
	*/													//
	// Undo's the given move
	void KaroEngine::UndoMove(Move *move)
	{
		turn = Reverse(turn);

		if(move->positionFrom > 0 && move->positionTo > 0 && move->tileFrom > 0) {
			UndoMove(move->positionFrom, move->positionTo, move->tileFrom, move->isJumpMove);

			// Check all changed positions for a change in moveable tiles
			TransformToMovableTiles(move->positionFrom, true, false);
			TransformToMovableTiles(move->positionTo, true, true);
			TransformToMovableTiles(move->tileFrom, true, true);

		} else if(move->positionFrom > 0 && move->positionTo > 0) {
			UndoMove(move->positionFrom, move->positionTo, move->isJumpMove);

			// Check all changed positions for a change in moveable tiles
			TransformToMovableTiles(move->positionFrom, true, false);
			TransformToMovableTiles(move->positionTo, true, false);

		} else if(move->positionTo > 0) {
			UndoMove(move->positionTo);

			// Check all changed positions for a change in moveable tiles
			TransformToMovableTiles(move->positionTo, true, false);
		} else {
			SetMessageLog("Not a valid undo move given");
		}
	}

	// Undo insert move
	bool KaroEngine::UndoMove(int to) {
		if(board[to] == Tile::REDMARKED || board[to] == Tile::REDUNMARKED) {
			board[to] = Tile::SOLIDTILE;
			redPieces.erase(to);			// Removes the pawn from the redPieces list
		} else if(board[to] == Tile::WHITEMARKED || board[to] == Tile::WHITEUNMARKED) {
			board[to] = Tile::SOLIDTILE;
			whitePieces.erase(to);			// Removes the pawn from the whitePieces list
		} else {
			SetMessageLog("[ERROR] Can't remove this pawn because it was not there.");
			return false;
		}

		return true;
	}

	// Undo a normal move (or jump move)
	bool KaroEngine::UndoMove(int from, int to, bool isJumpMove) {

		Tile x = board[from];
		Tile y = board[to];

		// Quick check board[from] is a valid tile
		if(board[from] != Tile::SOLIDTILE && board[from] != Tile::MOVEABLETILE) {
			SetMessageLog("[ERROR] Can't undo this move bacause board[from] is not a solidtile or moveabletile.");
			return false;
		}

		// Switch the pawns
		if(board[to] == Tile::REDMARKED || board[to] == Tile::REDUNMARKED) {
			if(isJumpMove)
				board[from] = (board[to] == Tile::REDMARKED) ? Tile::REDUNMARKED : Tile::REDMARKED;
			else
				board[from] = board[to];

			// Remove & add pawn in redPieces
			bool isTurned = (board[from] == Tile::REDMARKED) ? true : false;
			redPieces.erase(to);
			redPieces.insert(std::pair<int,bool>(from, isTurned));
		} 
		else if(board[to] == Tile::WHITEMARKED || board[to] == Tile::WHITEUNMARKED) {
			if(isJumpMove)
				board[from] = (board[to] == Tile::WHITEMARKED) ? Tile::WHITEUNMARKED : Tile::WHITEMARKED;
			else
				board[from] = board[to];

			// Remove & add pawn in whitePieces
			bool isTurned = (board[from] == Tile::WHITEMARKED) ? true : false;
			whitePieces.erase(to);
			whitePieces.insert(std::pair<int,bool>(from, isTurned));
		} 
		else {
			SetMessageLog("[ERROR] Can't undo this move bacause there is not a valid pawn on the board[to].");
			return false;
		}

		// Clear the old to
		board[to] = Tile::SOLIDTILE;

		return true;
	}

	// Undo a normal move and place the tile back (including jumpMove)
	bool KaroEngine::UndoMove(int from, int to, int tileFrom, bool isJumpMove) {

		// First put the old piece back in place
		if(UndoMove(from, to, isJumpMove)) {
			
			// Now remove the 'to'
			if(board[to] == Tile::SOLIDTILE || board[to] == Tile::MOVEABLETILE) {
				board[to] = Tile::EMPTY;
			} else {
				SetMessageLog("[ERROR] Can't remove the tile because the spot isn't empty!");
				return false;
			}

			// And place back the tileFrom
			if(board[tileFrom] == Tile::EMPTY) {
				board[tileFrom] = Tile::SOLIDTILE;
			} else {
				SetMessageLog("[ERROR] Can't put back the tile because the spot isn't empty!");
				return false;
			}

		} else {
			return false;	
		}

		return true;
	}
}