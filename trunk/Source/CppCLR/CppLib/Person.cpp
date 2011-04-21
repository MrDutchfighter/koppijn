#include "StdAfx.h"
#include "Person.h"

Person::Person(string name, int age)
{
	_name = name;
	_age = age;
}

string Person::ToString()
{
	stringstream ss;
	ss << "Person " << _name << " is " << _age << " years old.";
	return ss.str();
}