// This is the main DLL file.

#include "stdafx.h"

#include "KaroEngine.h"

namespace KaroEngine 
{
	KaroEngine::KaroEngine(void)
	{
		this->board = new Tile[BOARDWIDTH * BOARDWIDTH];
		this->turn = Player::WHITE;
		this->gameState = GameState::INSERTION; 
		this->insertionCount = 0;	
		this->maxDepth = 3;
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

		this->SetMessageLog("Engine Initialized");
	}

	KaroEngine::~KaroEngine(void)
	{
	}
	
	/**
	* Executes a given move
	* MOET EEN GELDIGE MOVE ZIJN!! (Geen checks in deze functie)!
	*/
	void KaroEngine::DoMove(Move *move)
	{
		// Is it a "normal" move (not inserting state)
		// PLAYING STATE
		if(move->positionFrom > 0) {

			// Moet er een tegel mee verplaatst worden, check dan of de neighbours van die tegel movable worden
			if(move->tileFrom > 0) {
				board[move->tileFrom] = Tile::EMPTY;
				TransformToMovableTiles(move->tileFrom, true);
			}

			// Jump move?
			if(move->isJumpMove) {
				if(turn == Player::RED)
					board[move->positionTo] = (board[move->positionFrom] == Tile::REDUNMARKED ? Tile::REDMARKED : Tile::REDUNMARKED);
				if(turn == Player::WHITE)
					board[move->positionTo] = (board[move->positionFrom] == Tile::WHITEUNMARKED ? Tile::WHITEMARKED : Tile::WHITEUNMARKED);
			} 
			else {
				board[move->positionTo] = board[move->positionFrom];
			}
			
			// Verwijder / clear oude tegel
			board[move->positionFrom] = Tile::SOLIDTILE;
			TransformToMovableTiles(move->positionFrom, true);

			// Hoeft alleen maar de neighbours te 'herchecken' als er een tegel verplaatst is
			if(move->tileFrom > 0) {
				TransformToMovableTiles(move->positionTo, true);
			}

			bool flippedValue;
			if(turn == Player::RED){
				flippedValue = board[move->positionTo] == Tile::REDUNMARKED ? false : true;
				
				redPieces.insert(std::pair<int,bool>(move->positionTo, flippedValue));
				redPieces.erase(move->positionFrom);
			} else if(turn == Player::WHITE) {
				flippedValue = board[move->positionTo] == Tile::WHITEUNMARKED ? false : true;

				whitePieces.insert(std::pair<int,bool>(move->positionTo, flippedValue));
				whitePieces.erase(move->positionFrom);
			}
		}
		// INSERTING STATE
		else 
		{
			if(board[move->positionTo] == Tile::MOVEABLETILE) {
				moveableTiles.erase(move->positionTo);
			}

			if(turn == Player::RED) {
				board[move->positionTo] = Tile::REDUNMARKED;
				redPieces.insert(std::pair<int,bool>(move->positionTo, false));
			} else {
				board[move->positionTo] = Tile::WHITEUNMARKED;
				whitePieces.insert(std::pair<int,bool>(move->positionTo, false));
			}
		}

		turn = Reverse(turn);
		lastMove = move;
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
			int connectedTiles = this->GetAmmountConnectedTiles(to);
			std::string s;
			std::stringstream stringstream;
			stringstream << connectedTiles;
			s = stringstream.str();
			this->SetMessageLog("Ammount of connected tiles: "+s);
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
				if(this->turn == Player::WHITE) {
					board[position] = Tile::WHITEUNMARKED;
					whitePieces.insert(std::pair<int,bool>(position, false));
				}
				else
				{
					board[position] = Tile::REDUNMARKED;
					redPieces.insert(std::pair<int,bool>(position, false));
				}

				turn = this->Reverse(turn);
				insertionCount++;							
				if(insertionCount == 12) {
					gameState = GameState::PLAYING;
				}
				return true;
		}
		return false;
	}

	/**
	* Undo's the given move
	*/
	void KaroEngine::UndoMove(Move *move)
	{
		turn = Reverse(turn);

		// PLAYING STATE
		if(move->positionFrom > 0) {

			// Put old piece back
			if(move->isJumpMove) {
				if(turn == Player::RED)
					board[move->positionFrom] = (board[move->positionTo] == Tile::REDUNMARKED ? Tile::REDMARKED : Tile::REDUNMARKED);
				if(turn == Player::WHITE)
					board[move->positionFrom] = (board[move->positionTo] == Tile::WHITEUNMARKED ? Tile::WHITEMARKED : Tile::WHITEUNMARKED);
			} else {
				board[move->positionFrom] = board[move->positionTo];
			}

			// Check neighbours & self for moveable shizzle
			TransformToMovableTiles(move->positionFrom, true);

			if(move->tileFrom > 0) {
				board[move->tileFrom] = Tile::SOLIDTILE;
				TransformToMovableTiles(move->tileFrom, true);

				board[move->positionTo] = Tile::EMPTY;
				TransformToMovableTiles(move->positionTo, true);
			} else {
				board[move->positionTo] = Tile::SOLIDTILE;
				TransformToMovableTiles(move->positionTo, true);
			}

			bool flippedValue;
			if(turn == Player::RED){
				flippedValue = board[move->positionFrom] == Tile::REDUNMARKED ? false : true;
				
				redPieces.insert(std::pair<int,bool>(move->positionFrom, flippedValue));
				redPieces.erase(move->positionTo);
			} else if(turn == Player::WHITE) {
				flippedValue = board[move->positionFrom] == Tile::WHITEUNMARKED ? false : true;

				whitePieces.insert(std::pair<int,bool>(move->positionFrom, flippedValue));
				whitePieces.erase(move->positionTo);
			}
		}
		// INSERTION STATE
		else {
			int neighbours = GetAmmountConnectedNeighbours(move->positionTo);
			if(neighbours <= 2) {
				board[move->positionTo] = Tile::MOVEABLETILE;
				moveableTiles.insert(std::pair<int,int>(move->positionTo,neighbours));
			} else {
				board[move->positionTo] = Tile::SOLIDTILE;
			}
					
			if(turn == Player::RED) {
				redPieces.erase(move->positionTo);
			} else {
				whitePieces.erase(move->positionTo);
			}
		}
	}

	/**
	* Switches the turn
	*/
	Player KaroEngine::Reverse(Player turn)
	{
		switch(turn)
		{
			case Player::WHITE:
				return Player::RED;
			case Player::RED:
				return Player::WHITE;
			default:
				return turn;
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
	* Checks if the player (p) has won the game by moving his last tile
	*/
	bool KaroEngine::IsWinner(Player p, int lastMove)
	{
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

		//check if first piece is marked
		if(board[lastMove] == marked)
		{
			//check if four pieces are Marked else return false
			int countUnMarked = 0;
			for each (pair<int, bool> p in pieces)
			{
				if(!p.second)
					countUnMarked++;
				if(countUnMarked > 2)
					return false;
			}

			for each (pair<int, bool> piece in pieces)
			{
				for(int i = 0; i < 8; i++)
				{
					//check if piece in possibleStep
					if(piece.first == lastMove + possibleSteps[i])
					{
						//check if second is marked
						int second = lastMove + possibleSteps[i];
						if(board[second] != marked)
						{
							break;
						}
						
						int difference = second - lastMove;

						//check if third is unmarked
						int third = second + difference;
						if(board[third] != marked)
						{
							//check if minfirst is unmarked 
							int minFirst = lastMove - difference;
							if(board[minFirst] != marked)
							{
								break;
							}
							else
							{
								//check if minfirst is marked and is winning
								int minSecond = minFirst - difference;
								if(board[minSecond] == marked)
								{
									return true;
								}
							}
						}
						else
						{
							//check if fourth is marked and is winning
							int fourth = third + difference;
							if(board[fourth] == marked)
							{
								return true;
							}
						}
					}
				}
			}
		}		

		return false;
	}

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
	void KaroEngine::CalculateComputerMove() {
		
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
			Move * theMove = MiniMax(GetTurn(), 0, INT_MIN, INT_MAX);

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
					vector<Move*> *moves = GetPossibleMoves(it->first, it->second);
					possibleMoves->insert(possibleMoves->end(), moves->begin(), moves->end());
				}
			}
		} else if(forPlayer == Player::WHITE) {
			if(!this->whitePieces.empty()) {
				for(std::map<int, bool>::iterator it = this->whitePieces.begin(); it != this->whitePieces.end(); ++it) {
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

		// Check all the possible (normal) moves
		for(int i=0; i<8; i++) {
				// Kan ik verplaatsen naar deze tegel
			if(board[curTile+possibleSteps[i]] == Tile::SOLIDTILE ||
				board[curTile+possibleSteps[i]] == Tile::MOVEABLETILE ||
				(moveableTiles.size() > 0 && GetAmmountConnectedNeighbours(curTile+possibleSteps[i]) > 0 && board[curTile+possibleSteps[i]] == Tile::EMPTY)) {
					
					// Willen we een tegel mee verplaatsen?
					if(board[curTile+possibleSteps[i]] == Tile::EMPTY) {
						for each(pair<int,int> j in this->moveableTiles) {

							int neighbours = 1;
							if(i == 0 || i == 2 || i == 5 || i == 7) {
								Tile tempTile= board[j.first];
								board[j.first] = Tile::EMPTY;
								neighbours = GetAmmountConnectedNeighbours(curTile+possibleSteps[i]);
								//board[j.first] = Tile::MOVEABLETILE;
								board[j.first] = tempTile;
							}

							if(neighbours > 0) {
								possibleMoves->push_back(new Move(curTile, (curTile+possibleSteps[i]), j.first, false));
							}
						}
					} else {
						possibleMoves->push_back(new Move(curTile, (curTile+possibleSteps[i]), false));
					}
					
					//possibleMoves->push_back(new Move(curTile, (curTile+possibleSteps[i]), false));
			}
						// Kijken of er een pion tussenin staat
			else if (board[curTile+possibleSteps[i]] != Tile::EMPTY &&
						board[curTile+possibleSteps[i]] != Tile::BORDER) {
					
					// Kijken of tegel vrij is
				if(board[curTile+possibleJumps[i]] == Tile::SOLIDTILE ||
					board[curTile+possibleJumps[i]] == Tile::MOVEABLETILE ||
					(moveableTiles.size() > 0 && GetAmmountConnectedNeighbours(curTile+possibleJumps[i]) > 0 && board[curTile+possibleJumps[i]] == Tile::EMPTY)) {
						
						// Willen we een tegel mee verplaatsen?
						if(board[curTile+possibleJumps[i]] == Tile::EMPTY) {
							for each(pair<int,int> j in this->moveableTiles) {
								int neighbours = 1;
								if(i == 0 || i == 2 || i == 5 || i == 7) {
									Tile tempTile= board[j.first];
									board[j.first] = Tile::EMPTY;
									neighbours = GetAmmountConnectedNeighbours(curTile+possibleJumps[i]);
									//board[j.first] = Tile::MOVEABLETILE;
									board[j.first] = tempTile;
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
	void KaroEngine::TransformToMovableTiles(int tileNumber, bool checkNeighbours = true) {
		//return;
		// Staat er niks op deze tile?
		if(board[tileNumber] == Tile::SOLIDTILE || board[tileNumber] == Tile::MOVEABLETILE) {
			int amountNeighbours = GetAmmountConnectedNeighbours(tileNumber);
			if(amountNeighbours <= 2 && amountNeighbours > 0) {

				if(board[tileNumber] == Tile::MOVEABLETILE) {
						moveableTiles.erase(tileNumber);
				}
				board[tileNumber] = Tile::EMPTY;

				int totalConnected = 0;
				if(this->board[tileNumber-1] != Tile::EMPTY && this->board[tileNumber-1] != Tile::BORDER) {
					totalConnected = GetAmmountConnectedTiles(tileNumber-1);
				}
				else if(this->board[tileNumber-BOARDWIDTH] != Tile::EMPTY && this->board[tileNumber-BOARDWIDTH] != Tile::BORDER){
					totalConnected = GetAmmountConnectedTiles(tileNumber-BOARDWIDTH);
				}
				else if(this->board[tileNumber+1] != Tile::EMPTY && this->board[tileNumber+1] != Tile::BORDER){
					totalConnected = GetAmmountConnectedTiles(tileNumber+1);
				}
				else if(this->board[tileNumber+BOARDWIDTH] != Tile::EMPTY && this->board[tileNumber+BOARDWIDTH] != Tile::BORDER){
					totalConnected = GetAmmountConnectedTiles(tileNumber+BOARDWIDTH);
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
				if(board[tileNumber] == Tile::MOVEABLETILE) {
					moveableTiles.erase(tileNumber);
				}
				board[tileNumber] = Tile::SOLIDTILE;
			}
		} else if(board[tileNumber] == Tile::EMPTY) {
			moveableTiles.erase(tileNumber);
		}

		// Check the neighbours
		if(checkNeighbours) {
			if(this->board[tileNumber-1] != Tile::EMPTY && this->board[tileNumber-1] != Tile::BORDER) {
				TransformToMovableTiles(tileNumber-1, false);
			}
			if(this->board[tileNumber-BOARDWIDTH] != Tile::EMPTY && this->board[tileNumber-BOARDWIDTH] != Tile::BORDER){
				TransformToMovableTiles(tileNumber-BOARDWIDTH, false);
			}
			if(this->board[tileNumber+1] != Tile::EMPTY && this->board[tileNumber+1] != Tile::BORDER){
				TransformToMovableTiles(tileNumber+1, false);
			}
			if(this->board[tileNumber+BOARDWIDTH] != Tile::EMPTY && this->board[tileNumber+BOARDWIDTH] != Tile::BORDER){
				TransformToMovableTiles(tileNumber+BOARDWIDTH, false);
			}
		}
	}

	/**
	* Make a call to find the ammount of ammount of connected tiles
	*/
	int KaroEngine::GetAmmountConnectedTiles(int tileNumber){
		visitedList->Clear();
		return GetAmmountConnectedTilesRecursive(tileNumber);
	}

	/**
	* Make a recursive call to find the ammount of connected tiles
	*/
	int KaroEngine::GetAmmountConnectedTilesRecursive(int tileNumber){
		int ammountTiles=0;
		if(this->board[tileNumber] == Tile::EMPTY ||this->board[tileNumber] == Tile::BORDER){
			return ammountTiles;
		}
		if(this->visitedList->isInArray(tileNumber)){
			return ammountTiles;
		}
		this->visitedList->insertAt(tileNumber);
		ammountTiles+=1;
		ammountTiles+=this->GetAmmountConnectedTilesRecursive(tileNumber-1);
		ammountTiles+=this->GetAmmountConnectedTilesRecursive(tileNumber-BOARDWIDTH);
		ammountTiles+=this->GetAmmountConnectedTilesRecursive(tileNumber+1);
		ammountTiles+=this->GetAmmountConnectedTilesRecursive(tileNumber+BOARDWIDTH);
		return ammountTiles;
	}

	/**
	* Check how many neighbours the tile had
	*/
	int KaroEngine::GetAmmountConnectedNeighbours(int tileNumber){
		int ammountTiles=0;
		if(this->board[tileNumber-1] != Tile::EMPTY && this->board[tileNumber-1] != Tile::BORDER){
			ammountTiles+=1;
		}
		if(this->board[tileNumber-BOARDWIDTH] != Tile::EMPTY && this->board[tileNumber-BOARDWIDTH] != Tile::BORDER){
			ammountTiles+=1;
		}
		if(this->board[tileNumber+1] != Tile::EMPTY && this->board[tileNumber+1] != Tile::BORDER){
			ammountTiles+=1;
		}
		if(this->board[tileNumber+BOARDWIDTH] != Tile::EMPTY && this->board[tileNumber+BOARDWIDTH] != Tile::BORDER){
			ammountTiles+=1;
		}
		return ammountTiles;
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
	Move * KaroEngine::MiniMax(Player p, int depth, int alpha, int beta)
	{
		// Hash the current board?
		//Position currentPosition = new Position(board);

		// Create new move
		Move *bestMove = new Move();
		if(p == Player::RED) {
			bestMove->score = INT_MIN; // Int32.MinValue
		} else {
			bestMove->score = INT_MAX; // Int32.MaxValue
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
		
		// If a player won
		//if(IsWinner(Reverse(p))) {
		//	bestMove->score = p == Player::RED ? 10000000 : -1000000; // Does this work??
		//	return bestMove;
		//}

		// Empty transposition table on first move?
		if(depth == 0) {
			transpositionTable.clear();
		} else if(depth >= 3) {
			// Is this move in the transposition table?
			int hash = GetHash();
			map<int,int>::iterator it = transpositionTable.find(hash);
            if (it != transpositionTable.end())
            {
                bestMove->score = it->second;
                return bestMove;
            }
		}

		// Find next moves for the current player
		vector<Move*> * possibleMoves = GetPossibleMoves(p);

		// Loop through all the moves
		for(int i=0; i < possibleMoves->size(); i++) {
			// Execute the move
			DoMove(possibleMoves->at(i));

			// Was this the winning move? (has to be here, because IsWinner needs the last move...)
			if(IsWinner(p, possibleMoves->at(i)->positionTo)) {
				bestMove = possibleMoves->at(i);
				if(p == Player::RED) {
					bestMove->score = INT_MAX;
				} else if(p == Player::WHITE) {
					bestMove->score = INT_MIN;
				}
				UndoMove(possibleMoves->at(i));
				return bestMove;
			}

			// Get the last best move
			Move * lastBestMove = MiniMax(Reverse(p), depth+1, alpha, beta);

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
				if(bestMove->score >= alpha) {
					alpha = bestMove->score;
				}
			}

			if(p == Player::WHITE) {
				if(bestMove->score <= beta) {
					beta = bestMove->score;
				}
			}

			// Prunning
			if(beta <= alpha) {
				return bestMove;
			}
		}

		// Put best score in transposition table
		//if(depth <= tableDepth) {
			int hash = GetHash();
			map<int,int>::iterator it = transpositionTable.find(hash);
			if (it == transpositionTable.end()){
                transpositionTable.insert(pair<int,int>(hash, bestMove->score));
			}
		//}

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
							calculatedScore += this->EvaluateNumRows(p, it->first);
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
							calculatedScore += this->EvaluateNumRows(p, it->first);
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
	* --------------- Setters ------------------------	//2
	*/													//
	void KaroEngine::SetMessageLog(std::string txt){
		this->messageLog+=txt+ "\r\n";
	}

	int KaroEngine::GetHash()
	{
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

	int KaroEngine::GetRandomNumber()
	{
		int randomNumber = rand() % 99999 + 10000;

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
}