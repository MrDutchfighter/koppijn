// This is the main DLL file.

#include "stdafx.h"

#include "KaroEngine.h"

namespace KaroEngine 
{
	KaroEngine::KaroEngine(void)
	{
		board = new Tile[BOARDWIDTH * BOARDWIDTH];
		this->turn = Player::WHITE;
		gameState = GameState::INSERTION; 
		insertionCount = 0;		

		for(int i = 0; i < BOARDWIDTH * BOARDWIDTH ; i ++ )
			board[i] = Tile::EMPTY;

		for(int j = 4; j < 8; j++)
			for( int k = 5; k < 10; k++ )
				if((j == 4 && k == 5) || (j == 4 && k == 9) || (j == 7 && k == 5) || (j == 7 && k == 9))
					board[j  *BOARDWIDTH + k] = Tile::MOVEABLETILE;
				else
					board[j * BOARDWIDTH + k] = Tile::SOLIDTILE;

		// Fill the array of possible steps;
		possibleSteps[0]=-14;
		possibleSteps[1]=-15;
		possibleSteps[2]=-16;
		possibleSteps[3]=-1;
		possibleSteps[4]= 1;
		possibleSteps[5]= 14;
		possibleSteps[6]= 15;
		possibleSteps[7]= 16;

		// Fill the array of possible jumps
		possibleJumps[0]=-28;
		possibleJumps[1]=-30;
		possibleJumps[2]=-32;
		possibleJumps[3]=-2;
		possibleJumps[4]= 2;
		possibleJumps[5]= 28;
		possibleJumps[6]= 30;
		possibleJumps[7]= 32;

		this->SetMessageLog("Engine Initialized");
	}

	GameState KaroEngine::GetGameState(){
		return this->gameState;
	}

	KaroEngine::~KaroEngine(void)
	{

	}

	void KaroEngine::DoMove(int from, int to, int tileFrom)
	{		
		if (tileFrom != -1) { //move the tile
			
			if(board[tileFrom] != Tile::MOVEABLETILE) {
				this->SetMessageLog(" Tried to move a tile that is not moveable ");
				return;
			}
			if(board[to] != Tile::EMPTY){
				tileFrom =-1;
				this->SetMessageLog(" Did not move the MOVEABLETILE");				
			} else {
				board[tileFrom] = Tile::EMPTY;
				board[to]=Tile::SOLIDTILE;
			}
		}
		if(IsValidMove(from, to)) {			
			board[to] = board[from];
			board[from] = Tile::SOLIDTILE;
			turn = Reverse(turn);

			this->SetMessageLog(" Move succesful! ");
		} else { // if not a valid move, undo moving of the boardtiles.
			if (tileFrom != -1) {
				board[tileFrom] = Tile::MOVEABLETILE;
				board[to]		= Tile::EMPTY;
			}
			this->SetMessageLog(" Move failed!");
		}
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

	void KaroEngine::UndoMove()
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
		int position=(y*15)+x;
		
		if(board[position] == Tile::SOLIDTILE || board[position] == Tile::MOVEABLETILE ){
				if(this->turn == Player::WHITE){
					board[position] =Tile::WHITEUNMARKED;
				}
				else{
					board[position] =Tile::REDUNMARKED;
				}
				turn=this->Reverse(turn);
				insertionCount++;							
				if(insertionCount == 12){
					gameState = GameState::PLAYING;
				}
				return true;
		}
		return false;
	}

	void KaroEngine::CalculateComputerMove() {
		//if in insertionstate, then insert on random position
		if(gameState == GameState::INSERTION){
			bool foundInsertPosition=false;
			while(!foundInsertPosition){
				int x = 5+rand()%5;
				int y = 4+rand()%4;
				if(this->InsertByXY(x,y)){
					foundInsertPosition=true;
				}
			}
		}
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
		return board[tile] != Tile::EMPTY;
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
		return board[(y*15)+x];
	}

	Move * KaroEngine::MiniMax(Player p, int depth, int alpha, int beta)
	{
		return new Move();
	}
}