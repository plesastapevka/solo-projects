//
//  iskanjeNizov.cpp
//  iskanje_nizov_vaja02
//
//  Created by Urban Vidovič on 28/03/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include <iostream>
#include <fstream>
#include <cstring>

using namespace std;

void naredikmpNext(const char *niz, int N, int *kmpNext) {
    int len = 0;
    kmpNext[0] = -1;
    kmpNext[1] = 0;
    int i = 1;
    while (i < N)
    {
        if (niz[i] == niz[len])
        {
            len++;
            kmpNext[i] = len;
            i++;
        }
        else {
            if (len != 0) {
                len = kmpNext[len-1];
            }
            else {
                kmpNext[i] = 0;
                i++;
            }
        }
    }
}

void najdiKMP(const char *niz, const char *besedilo) {
    size_t velNiza = strlen(niz);
    size_t velBesedila = strlen(besedilo);
    
    int *kmpNext = new int [velNiza];
    
    naredikmpNext(niz, velNiza, kmpNext);
    ofstream mojIzhod;
    mojIzhod.open("out.txt");
    int i = 0;  // index for txt[]
    int j  = 0;  // index for pat[]
    while (i < velBesedila)
    {
        if (niz[j] == besedilo[i]) {
            j++;
            i++;
        }
        if (j == velNiza) {
            cout << i-j << " ";
            mojIzhod << i-j << " ";
            j = kmpNext[j-1];
        }
        else if (i < velBesedila && niz[j] != besedilo[i]) {
            if (j != 0) {
                j = kmpNext[j-1];
            }
            else {
                i = i+1;
            }
        }
    }
    mojIzhod.close();
}

int main(int argc, const char * argv[]) {
    
    string line;
    ifstream mojFile(argv[2]);
    int c = 0;
    if (mojFile.is_open()) {
        while(getline(mojFile, line)) {
            c++;
        }
    }
    mojFile.close();
    
    ifstream mojVhod(argv[2]);
    string contents((istreambuf_iterator<char>(mojVhod)), istreambuf_iterator<char>());
    const char *besedilo = contents.c_str();
    const char *niz = argv[1];
    najdiKMP(niz, besedilo);
    mojVhod.close();
    
    return 0;
}
