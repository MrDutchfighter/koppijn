#pragma once
#include "NativeClass.h"

using namespace System;
using namespace System::Collections::Generic;

namespace CppLib
{

	public ref class NativeClassWrapper
	{
	private:
		NativeClass *nativeClass;

	public:
		NativeClassWrapper(void);
		~NativeClassWrapper(void);

		int SquareWrapper(int n);
		List<int> ^GetNumbersWrapper();
	};

}