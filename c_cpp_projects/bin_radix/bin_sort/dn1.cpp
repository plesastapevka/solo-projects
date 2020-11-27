//
//  main.cpp
//  binarni_radix_sort_DN01
//
//  Created by Urban Vidovič on 25/04/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include <iostream>
#include <vector>
#include <fstream>

using namespace std;

void radix(vector<unsigned char> &A, int size) {
    int k = 0;
    int B[size];
    
    for(int f = 0; f < 8; f++) {
    //start counting sort///////////////////////
        int C[] = {0, 0};                     //
        for(int i = 0; i < size; i++) {       //
            C[(A[i]>>k)&1]++;                 //
        }                                     //
        C[1] += C[0];                         //
                                              //
                                              //
        for(int i = size - 1; i >= 0; --i) {  //
            B[--C[(A[i]>>k)&1]] = A[i];       //
        }                                     //
    //end counting sort/////////////////////////
        for(int i = 0; i < size; i++) {
            int tmp1 = A[i];
            A[i] = B[i];
            B[i] = tmp1;
        }
        k++;
    }
    ofstream data;
    data.open("out.txt");
    for(int i = 0; i < size; i++) {
        int x = A[i];
        data << x << " ";
        cout << x << " ";
    }
    data.close();
}

int main(int argc, const char * argv[]) {
    int max = 0;
    int ind = 0;
    int x;
    ifstream input;
    //start counting elements/////////////////////////
    input.open(argv[1]);                            //
    if (!input) {                                   //
        cout << "Napaka pri odpiranju." << endl;    //
    }                                               //
    while(input >> x) {                             //
        max++;                                      //
    }                                               //
    input.close();                                  //
    //end counting elements///////////////////////////
    
    //start reading elements//////////////////////////
    vector<unsigned char> A(max);                   //
    input.open(argv[1]);                            //
    if (!input) {                                   //
        cout << "Napaka pri odpiranju." << endl;    //
    }                                               //
    while(input >> x) {                             //
        A[ind] = x;                                 //
        ind++;                                      //
    }                                               //
    input.close();                                  //
    //end reading elements////////////////////////////
    
    radix(A, max);
    
    return 0;
}
