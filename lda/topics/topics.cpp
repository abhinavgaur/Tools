// topics.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include <string>
#include <vector>
#include <map>
#include <fstream>
#include <iostream>
#include <sstream>
using namespace std;

int _tmain(int argc, _TCHAR* argv[])
{
	if (argc < 3) {
		printf("Usage: topics <vocab> <beta>");
		return 0;
	}
	ifstream ivocab(argv[1]);
	string word;
	vector<string> vocab;
	while(ivocab >> word)
	{
		vocab.push_back(word);
	}

	map<double,string> terms;
	ifstream betafile(argv[2]);
	string beta;
	while(getline(betafile,beta))
	{
		double v;
		istringstream ss(beta);
		int i = 0;
		while(ss>>v)
		{
			terms.insert(map<double,string>::value_type(v,vocab[i]));
			i++;
		}
		map<double,string>::reverse_iterator iter = terms.rbegin();
		for( int i = 0;i < 25;i++,iter++)
		{
			cout<<(*iter).second<<" ";
		}
		cout<<endl;
		terms.clear();
	}

	system("pause");
	return 0;
}

