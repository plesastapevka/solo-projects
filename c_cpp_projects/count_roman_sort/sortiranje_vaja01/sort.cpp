//
//  sort.cpp
//  sortiranje_vaja01
//
//  Created by Urban Vidovič on 12/03/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include <iostream>
#include <fstream>
#include <vector>

using namespace std;

void countingSort(vector<int> &A, int size) {
    vector<int> B(size);
    int max = A[0];
    int min = A[0];
    for(int i = 0; i < size; i++) { //iskanje največjega
        if(A[i] > max) {
            max = A[i];
        }
    }
    for(int i = 0; i < size; i++) { //iskanje najmanjšega
        if(A[i] < min) {
            min = A[i];
        }
    }
    int noviI = max - min + 1;
    vector<int> C(noviI);
    for(int i = 0; i < noviI; i++) { //inicializacija na 0 polja C
        C[i] = 0;
    }
    for(int i = 0; i < size; i++) {
            C[A[i] - min]++;
    }
    for(int i = 1; i < noviI; i++) {
        C[i] = C[i] + C[i-1];
    }
    for(int i = size-1; i > -1; i--) {
        B[C[A[i] - min]-1] = A[i];
        C[A[i] - min]--;
    }
    ofstream output;
    output.open("izhod.txt");
    
    for(int i = 0; i < size; i++) {
        cout << B[i] << " ";
        output << B[i] << " ";
    }
    output.close();
}

void romanSort(vector<int> &A, int size) {
    vector<int> B(size);
    int max = A[0];
    int min = A[0];
    for(int i = 0; i < size; i++) { //iskanje največjega
        if(A[i] > max) {
            max = A[i];
        }
    }
    for(int i = 0; i < size; i++) { //iskanje najmanjšega
        if(A[i] < min) {
            min = A[i];
        }
    }
    int noviI = max - min + 1;
    vector<int> C(noviI);
    for(int i = 0; i < noviI; i++) { //inicializacija na 0 polja C
        C[i] = 0;
    }
    for(int i = 0; i < size; i++) {
        C[A[i] - min]++;
    }
    int index = 0;
    for(int i = 0; i < noviI; i++) {
        if(C[i] != 0) {
            for(int j = 0; j < C[i]; j++) {
                B[index] = i + min;
                index++;
            }
        }
    }
    ofstream output;
    output.open("izhod.txt");
    
    for(int i = 0; i < size; i++) {
        cout << B[i] << " ";
        output << B[i] << " ";
    }
    output.close();
}

int main(int argc, char *argv[]) {
    int x;
    int max = 0;
    int ind = 0;
    char myarg = argv[1][0];
    ifstream input;
    input.open(argv[2]);
    if (!input) {
        cout << "Napaka pri odpiranju." << endl;
    }
    while(input >> x) {
        max++;
    }
    input.close();
    vector<int> A(max);
    input.open(argv[2]);
    if (!input) {
        cout << "Napaka pri odpiranju." << endl;
    }
    while(input >> x) {
        A[ind] = x;
        ind++;
    }
    input.close();
    
    if(myarg == '0') {
        countingSort(A, max);
    }
    else if(myarg == '1') {
        romanSort(A, max);
    }
    cout << endl;

    return 0;
}
