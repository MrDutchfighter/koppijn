//NATIVE PERSON CLASS
#pragma once

#include <string>
#include <sstream>

using namespace std;

class Person
{
public:
	string _name;
	int _age;

	Person(string, int);
	string ToString();
};

