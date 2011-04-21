//MANAGED WRAPPER FOR THE PERSON CLASS
#pragma once

#include "Person.h"
#include <string.h>

using namespace std;
using namespace System;

namespace CppLib
{
	public ref class PersonWrapper
	{
	private:
		Person *_person; // native pointer
	public:
		property String ^Name
		{
			String ^get();
			void set(String ^);
		}

		PersonWrapper(String ^, int);
		~PersonWrapper(void);

		String^ ToString() new;
	};
}
