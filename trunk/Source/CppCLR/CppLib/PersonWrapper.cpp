#include "StdAfx.h"
#include <msclr\marshal_cppstd.h>
#include "PersonWrapper.h"

//Read about marshaling: http://msdn.microsoft.com/en-us/library/bb384865.aspx

using namespace msclr::interop;
using namespace CppLib;

PersonWrapper::PersonWrapper(String ^name, int age)
{
	//Marshall to Native C++ string
	string s = marshal_as<string>(name);

	//Create a new native object
	_person = new Person(s, age);
}

PersonWrapper::~PersonWrapper(void)
{
	//Delete the Native object from memory
	delete _person;
}

String ^PersonWrapper::Name::get()
{
	return marshal_as<String ^>(_person->_name);
}

void PersonWrapper::Name::set(String ^name)
{
	_person->_name = marshal_as<string>(name);
}

String^ PersonWrapper::ToString()
{
	//Return the marshalled string
	return marshal_as<String ^>(_person->ToString());
}