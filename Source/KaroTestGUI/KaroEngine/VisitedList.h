#pragma once
#include <map>
using namespace std;

public class VisitedList
{
public:
	VisitedList(void);
	bool isInArray(int index);
	void insertAt(int index);
	void Clear();
private:
	map<int,int> visitedList;
	
};

