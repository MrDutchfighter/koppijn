#include "StdAfx.h"
#include "NativeClass.h"

int NativeClass::Square(int n)
{
	return n * n;
}

vector<int> NativeClass::GetNumbers()
{
	vector<int> vec;
	vec.push_back(500);
	vec.push_back(300);
	vec.push_back(15);

	return vec;
}
