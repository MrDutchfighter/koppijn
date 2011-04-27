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
		this->evaluationScore = 0;

		for(int i = 0; i < BOARDWIDTH * BOARDWIDTH ; i ++ )
			board[i] = Tile::EMPTY;
		for(int y=0;y<BOARDWIDTH;y++){
			for(int x=0;x<BOARDWIDTH;x++){
				if(x==0 || y==0 ||x==(BOARDWIDTH-1) || y==(BOARDWIDTH-1)){
					board[y*BOARDWIDTH + x]=Tile::BORDER;
				}
				else{
				board[y*BOARDWIDTH + x]=Tile::EMPTY;
				}
			}
		}


		for(int j = 4; j < 8; j++)
			for( int k = 5; k < 10; k++ )
				if((j == 4 && k == 5) || (j == 4 && k == 9) || (j == 7 && k == 5) || (j == 7 && k == 9))
					board[j  *BOARDWIDTH + k] = Tile::MOVEABLETILE;
				else
					board[j * BOARDWIDTH + k] = Tile::SOLIDTILE;

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

		this->SetMessageLog("Engine Initialized");
	}

	GameState KaroEngine::GetGameState(){
		return this->gameState;
	}
	
	int KaroEngine::GetEvaluationScore()
	{
		return this->evaluationScore;
	}

	KaroEngine::~KaroEngine(void)
	{
	}
	
	void KaroEngine::DoMove(Move *move)
	{
		if(move->positionFrom != -1) // PLAYING STATE
		{
			if(move->isJumpMove) // Flip piece on the board
			{
				if(turn == Player::RED)
					board[move->positionTo] = (board[move->positionFrom] == Tile::REDUNMARKED ? Tile::REDMARKED : Tile::REDUNMARKED);
				if(turn == Player::WHITE)
					board[move->positionTo] = (board[move->positionFrom] == Tile::WHITEUNMARKED ? Tile::WHITEMARKED : Tile::WHITEUNMARKED);
			}
			else
				board[move->positionTo] = board[move->positionFrom];
			
			
			board[move->positionFrom] = Tile::SOLIDTILE; // Solid or movable
			if(move->tileFrom != -1)
				board[move->tileFrom] = Tile::EMPTY; // Empty moved tile

			bool flippedValue;
			if(turn == Player::RED){
				if(move->isJumpMove)
					flippedValue = (redPieces[move->positionFrom] ? false : true); // Flip piece in map
				else
					flippedValue = redPieces[move->positionFrom];
				
				redPieces.insert(std::pair<int,bool>(move->positionTo,flippedValue));
				redPieces.erase(move->positionFrom);
			}else{
				if(move->isJumpMove)
					flippedValue = (whitePieces[move->positionFrom] ? false : true); // Flip piece in map
				else
					flippedValue = whitePieces[move->positionFrom];

				whitePieces.insert(std::pair<int,bool>(move->positionTo,false));
				whitePieces.erase(move->positionFrom);
			}
		}
		else //INSERTING STATE
		{
			if(turn == Player::RED){
				board[move->positionTo] = Tile::REDUNMARKED;
				redPieces.insert(std::pair<int,bool>(move->positionTo,false));
			}else{
				board[move->positionTo] = Tile::WHITEUNMARKED;
				whitePieces.insert(std::pair<int,bool>(move->positionTo,false));
			}
		}
	}

	void KaroEngine::DoMove(int from, int to, int tileFrom)
	{
		if (tileFrom != -1) { //move the tile
			if(board[tileFrom] != Tile::MOVEABLETILE) {
				this->SetMessageLog("Tried to move a tile that is not moveable ");
				return;
			}
			if(board[to] != Tile::EMPTY){
				tileFrom =-1;
				this->SetMessageLog("Did not move the MOVEABLETILE");				
			} else {
				board[tileFrom] = Tile::EMPTY;
				board[to]=Tile::SOLIDTILE;
			}
		}
		if(IsValidMove(from, to)) {			
			board[to] = board[from];
			board[from] = Tile::SOLIDTILE;
			if(turn == Player::RED)
			{
				redPieces.insert(std::pair<int,bool>(to,(board[to] == Tile::REDMARKED)));
				redPieces.erase(from);
			}
			else
			{
				whitePieces.insert(std::pair<int,bool>(to,(board[to] == Tile::WHITEMARKED)));
				whitePieces.erase(from);
			}


			turn = Reverse(turn);

			this->SetMessageLog("Move succesful! ");
		} else { // if not a valid move, undo moving of the boardtiles.
			if (tileFrom != -1) {
				board[tileFrom] = Tile::MOVEABLETILE;
				board[to]		= Tile::EMPTY;
			}
			this->SetMessageLog("Move failed!");
		}
		this->EvaluateBoard(turn);
	}

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
							calculatedScore++;
					}
				}
			break;
			}
		case Player::RED:
			{
				if(!this->redPieces.empty()) {
					for(std::map<int, bool>::iterator it = this->redPieces.begin(); it != this->redPieces.end(); ++it) {
						if (it->second == true)
							calculatedScore++;
					}
				}
			break;
			}
		}
		this->evaluationScore = calculatedScore;
		return calculatedScore;

	}


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

	void KaroEngine::UndoMove(Move m)
	{
		
	}

	bool KaroEngine::IsWinner(Player p)
	{
		Tile marked;
		//Right player color 
		if (p == Player::WHITE) 
			marked = Tile::WHITEMARKED;
		if (p == Player::RED)
			marked = Tile::REDMARKED;


		for(int i = 0; i < BOARDWIDTH; i++) {
			for(int j = 0; j < BOARDWIDTH; j++) {
				//Current position
				int current = i * BOARDWIDTH + j;
				
				// Is current tile marked
				if(board[current] == marked)
				{
					//Check vertical boundery 
					if(i <= BOARDWIDTH - 4)
					{
						//Vertical
						if(board[current + 1 * BOARDWIDTH] == marked && board[current + 2 * BOARDWIDTH] == marked && board[current + 3 * BOARDWIDTH] == marked)
						{
							return true;
						}
					}

					//Check horizontal boundery
					if(j <= BOARDWIDTH - 4)
					{
						//Horizontal
						if(board[current + 1] == marked && board[current + 2] == marked && board[current + 3] == marked)
						{
							return true;
						}
					}
					
					//Check horizontal and vertical bounderies
					if(i <= BOARDWIDTH - 4 && j <= BOARDWIDTH - 4)
					{
						//Diagonal down
						if(board[current + 1 + (1 * BOARDWIDTH)] == marked && board[current + 2 + (2 * BOARDWIDTH)] == marked && board[current + 3 + (3 * BOARDWIDTH)] == marked)
						{
							return true;
						}

						//Diagonal up
						if(board[current + 3 + (0 * BOARDWIDTH)] == marked && board[current + 2 + (1 * BOARDWIDTH)] == marked && board[current + 1 + (2 * BOARDWIDTH)] == marked && board[current + 0 + (3 * BOARDWIDTH)] == marked)
						{
							return true;
						}
					}
				}
			}
		}

		return false;
	}

	bool KaroEngine::InsertByXY(int x, int y){
		int position=(y*BOARDWIDTH)+x;
		
		if(board[position] == Tile::SOLIDTILE || board[position] == Tile::MOVEABLETILE ){
				if(this->turn == Player::WHITE) {
					board[position] = Tile::WHITEUNMARKED;
					whitePieces.insert(std::pair<int,bool>(position,false));
				}
				else
				{
					board[position] =Tile::REDUNMARKED;
					redPieces.insert(std::pair<int,bool>(position,false));
				}

				turn=this->Reverse(turn);
				insertionCount++;							
				if(insertionCount == 12) {
					gameState = GameState::PLAYING;
				}
				return true;
		}
		return false;
	}

	void KaroEngine::CalculateComputerMove() {
		// If the game is in insertion state, insert a random item on a tile
		if(gameState == GameState::INSERTION) {
			bool foundInsertPosition=false;
			while(!foundInsertPosition){
				int x = 5+rand()%5;
				int y = 4+rand()%4;
				if(this->InsertByXY(x,y)){
					foundInsertPosition=true;
				}
			}
		} else if(gameState == GameState::PLAYING) {
			// Generate a real computer move with minmax etc.
		}
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
					vector<Move*> *move = GetPossibleMoves(it->first, it->second);
					possibleMoves->insert(possibleMoves->end(), move->begin(), move->end());
				}
			}
		} else if(forPlayer == Player::RED) {
			if(!this->whitePieces.empty()) {
				for(std::map<int, bool>::iterator it = this->whitePieces.begin(); it != this->whitePieces.end(); ++it) {
					vector<Move*> *move = GetPossibleMoves(it->first, it->second);
					possibleMoves->insert(possibleMoves->end(), move->begin(), move->end());
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
				board[curTile+possibleSteps[i]] == Tile::MOVEABLETILE) {
					possibleMoves->push_back(new Move(curTile, (curTile+possibleSteps[i]), false));
			}
						// Kijken of er een pion tussenin staat
			else if (board[curTile+possibleSteps[i]] != Tile::EMPTY &&
						board[curTile+possibleSteps[i]] != Tile::BORDER) {
					
					// Kijken of tegel vrij is
				if(board[curTile+possibleJumps[i]] == Tile::SOLIDTILE ||
					board[curTile+possibleJumps[i]] == Tile::MOVEABLETILE) {
					possibleMoves->push_back(new Move(curTile, (curTile+possibleJumps[i]), true));
				}
			}
		}

		return possibleMoves;
	}

	bool KaroEngine::FreeForMove(int tile)
	{
		if(board[tile] == Tile::SOLIDTILE || board[tile] == Tile::MOVEABLETILE) {
			return true;
		}
		return false;
	}

	bool KaroEngine::IsGameTile(int tile)
	{
		return (board[tile] != Tile::EMPTY && board[tile] != Tile::BORDER);
	}

	std::string KaroEngine::GetMessageLog(){
		std::string s = this->messageLog;
		this->messageLog="";
		return s;
	}

	void KaroEngine::SetMessageLog(std::string txt){
		this->messageLog+=txt+ "\r\n";
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

	Move * KaroEngine::MiniMax(Player p, int depth, int alpha, int beta)
	{
		return new Move();
	}
}