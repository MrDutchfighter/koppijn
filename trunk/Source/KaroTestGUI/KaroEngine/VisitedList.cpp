#include "StdAfx.h"
#include "VisitedList.h"


VisitedList::VisitedList(void)
{
	
}
bool VisitedList::isInArray(int index){
	map<int,int>::iterator iter;
	iter = this->visitedList.find(index);
    if (iter != this->visitedList.end()) {  //index is found in the map
		return true;
	}
    else{
		return false;
	}
}

void VisitedList::insertAt(int index){
	this->visitedList.insert(std::pair<int,int>(index, index));
}

void VisitedList::Clear(){
	this->visitedList.clear();
}
