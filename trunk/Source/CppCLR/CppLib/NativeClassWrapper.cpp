#include "StdAfx.h"
#include "NativeClassWrapper.h"

using namespace CppLib;

NativeClassWrapper::NativeClassWrapper(void)
{
	nativeClass = new NativeClass;
}

NativeClassWrapper::~NativeClassWrapper(void)
{
	delete nativeClass;
}

int NativeClassWrapper::SquareWrapper(int n)
{
	return nativeClass->Square(n);
}

List<int> ^NativeClassWrapper::GetNumbersWrapper()
{
	vector<int> vec = nativeClass->GetNumbers(); // NATIVE
	List<int> ^nums = gcnew List<int>(); // MANAGED

	for each (int n in vec) // NATIVE
		nums->Add(n); // MANAGED

	return nums;
}
