#include "StdAfx.h"
#include <msclr\marshal_cppstd.h>
#include "KaroEngineWrapper.h"


using namespace msclr::interop;
using namespace KaroEngine;

KaroEngineWrapper::KaroEngineWrapper(void)
{
	_karoEngine = new KaroEngine();
	//_person = new Person(s, age);
}

KaroEngineWrapper::~KaroEngineWrapper(void)
{
	if(_karoEngine)
		delete _karoEngine;
}