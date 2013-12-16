// cipher_gen.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <Windows.h>
#include <fstream>
using namespace std;

int _tmain(int argc, _TCHAR* argv[])
{
	fstream f;
	bool* tab = new bool[256];
	int* crypttab = new int[256];
	int* decrypttab = new int[256];

	srand(GetTickCount());

	//generator
	for(int i=0; i<256; i++) tab[i] = false;
	for(int i=0; i<16; i++)
	{
		for(int j=0; j<16; j++)
		{
			int k;
			do
			{
				k = rand()%256;
			}while(tab[k]==true);
			tab[k] = true;
			crypttab[16*i+j] = k;
			decrypttab[k] = 16*i+j;
		}
	}

	//wypisywanie do pliku

	f.open("tabs.txt", ios::out);

	f << "private byte[] crypttab={\n";
	for(int i=0; i<16; i++)
	{
		for(int j=0; j<16; j++)
			f << "0x" << hex << crypttab[16*i+j] << (i==15 && j==15?"\n":", ");
		if(i!=15)
			f << "\n";
		else
			f << "};\n\n";
	}

	f << "private byte[] decrypttab={\n";
	for(int i=0; i<16; i++)
	{
		for(int j=0; j<16; j++)
			f << "0x" << hex << decrypttab[16*i+j] << (i==15 && j==15?"\n":", ");
		if(i!=15)
			f << "\n";
		else
			f << "};\n\n";
	}

	f.close();
	delete [] tab;
	delete [] crypttab;
	delete [] decrypttab;

	return 0;
}

