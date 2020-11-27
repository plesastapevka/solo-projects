/*
 * KMP.cpp
 *
 *  Created on: 7. apr. 2018
 *      Author: Tadej
 */

#include <iostream>
#include <fstream>
#include <stdio.h>
#include <string.h>
#include <cstring>

using namespace std;

void makeKMPNext(int* kmpNext, const char* sample, int sampleSize) {

	kmpNext[0] = -1;
	kmpNext[1] = 0;
	int i = 1;
	int x = 0;

	while(i < sampleSize) {
		if (sample[i] != sample[x]) {
			if (x == 0) {
				kmpNext[i] = 0;
				i++;
			} else {
				x = kmpNext[x-1];
			}
		} else {
			x++;
			kmpNext[i] = x;
			i++;
		}
	}
}

void search(const char* txt, const char* sample, int sampleSize) {

	int i, j = 0;
	size_t txtSize = strlen(txt);
	int* kmpNext = new int [sampleSize];
    makeKMPNext(kmpNext, sample, sampleSize);

    ofstream izhod;
    izhod.open("out.txt");
    while(j < txtSize) {
    	if(sample[i] == txt[j]) { //ujemanje znakov
    		i++;
    		j++;
    	} else {
    		if(i == 0) {
    			j++;
    		} else {
    			i = kmpNext[i-1];
    		}
    	}
    	if(i == sampleSize) { //ujemanje celotnega niza
    		izhod << j-i << " ";
    		cout << j-i << " ";
    		i = kmpNext[i-1];
    	}
    }
    izhod.close();
}


int main(int argc, const char* argv[]) {

    string line;
    ifstream datoteka(argv[2]);
    	int count = 0;
    	if (datoteka.is_open()) {
    		while(getline(datoteka, line)) {
    			count++;
    		}
    	}
    datoteka.close();

    ifstream FileIn(argv[2]);
        string vsebina((istreambuf_iterator<char>(FileIn)), istreambuf_iterator<char>());
        const char *sample = argv[1];
        size_t sampleSize = strlen(sample);
        const char *txt = vsebina.c_str();
        search(txt, sample, sampleSize);
    FileIn.close();

	return 0;
}

