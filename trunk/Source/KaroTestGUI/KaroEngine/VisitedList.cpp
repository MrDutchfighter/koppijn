#include "StdAfx.h"
#include "VisitedList.h"


VisitedList::VisitedList(void)
{
	
}
bool VisitedList::isInArray(int index){
	for each(pair<int,int> i in this->visitedList){
		if(i.second==index){
			return true;
		}
	}
	return false;
}

void VisitedList::insertAt(int index){
	this->visitedList.insert(std::pair<int,int>(index, index));
}

void VisitedList::Clear(){
	this->visitedList.clear();
}
