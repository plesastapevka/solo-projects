//
//  main.cpp
//  LZW_vaja04
//
//  Created by Urban Vidovič on 31/05/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include <iostream>
#include <string>
#include <bitset>
#include <vector>
#include <sstream>
#include <cmath>
#include <fstream>
#include "BinWriter.h"
#include "BinReader.h"

using namespace std;

int find(vector<string> &S, string prev) {
    for(int i = 0; i < S.size(); i++) {
        if(prev == S[i]) {
            return i;
        }
        else if(prev == " ") {
            return 32;
        }
    }
    return -1;
}

bool find2(vector<string> &S, int prev) {
    for(int i = 0; i < S.size(); i++) {
        if(prev == i) {
            return true;
        }
    }
    return false;
}

double getSize(string path) {
    ifstream file(path, ios::binary);
    
    if(!file.is_open()) {
        return -1;
    }
    
    file.seekg(0, ios::end);
    double fileSize = file.tellg();
    file.close();
    return fileSize;
}

void compress(int size, string path) {
    string T = "", C = "", x = "";
    int counter = 0, koda;
    const int vel = ceil(log2(size));
    string coding = "";
    vector<string> slovar;
    for(int i = 0; i < 256; i++) {
        char znak = i;
        stringstream ss;
        string s;
        ss << znak;
        ss >> s;
        slovar.push_back(s);
    }
    slovar[32] = " ";
    
    BinWriter *bw = new BinWriter("out.bin", vel); //v to piše
    BinReader *br = new BinReader("testnifile.bin", vel); //od tu bere
    
    while(true) {
        koda = br->readInt();
        if(koda == -88276) {
            break;
        }
        coding += slovar[koda];
    }
    
    for(int i = 0; i <= coding.size(); i++) {
        if(slovar.size() >= size) {
            for(int j = i; j < coding.size(); j++) {
                T = coding[i];
                counter = find(slovar, T);
                bw->writeInt(counter);
            }
            return;
        }
        C = coding[i];
        x = T+C;
        if(find(slovar, x) != -1) { //če ga najde je T = x
            T = x;
        }
        else { //če ga ne najde
            counter = find(slovar, T);
            bw->writeInt(counter);
            if(i < coding.size()) {
                slovar.push_back(x);
            }
            T = C;
        }
    }
    delete bw;
    delete br;
}

void decompress(int size, string path) {
    const int vel = ceil(log2(size));
    vector<string> slovar;
    for(int i = 0; i < 256; i++) {
        char znak = i;
        stringstream ss;
        string s;
        ss << znak;
        ss >> s;
        slovar.push_back(s);
    }
    slovar[32] = " ";
    
    string P = "", T = "";
    
    int B;
    char C;
    
    BinReader *br = new BinReader(path, vel);
    BinWriter *bw = new BinWriter("out.bin", vel);
    //B = br->readInt();
    //cout << B << endl;
    while(true) {
        
        B = br->readInt();
        cout << B << " ";
        
        if(B == -88276) {
            break;
        }
        T = slovar[B];
        //cout << T;
        bw->writeInt(B);
        if(find2(slovar, B) == false) {
            slovar.push_back(P+P[0]);
        }
        else {
            C = slovar[B][0];
            if((P+C).size() != 1) {
                slovar.push_back(P+C);
            }
            P = T;
        }
    }
    cout << endl;
    delete br;
    delete bw;
}

void test(int size) {         //FUNKCIJA KI GENERIRA TESTNO DATOTEKO ZA KODIRANJE
    const int vel = ceil(log2(size)); //GLAVNINA PROGRAMA JE ZGORAJ, MAIN PA SPODAJ
    vector<string> slovar;
    for(int i = 0; i < 256; i++) {
        char znak = i;
        stringstream ss;
        string s;
        ss << znak;
        ss >> s;
        slovar.push_back(s);
    }
    slovar[32] = " ";
    
    BinWriter *bw = new BinWriter("testnifile.bin", vel);
    string example = "RABARABARA";
    string T = "";
    int emp;
    for(int i = 0; i < example.size(); i++) {
        T = T+example[i];
        emp = find(slovar, T);
        bw->writeInt(emp);
        T = "";
    }
    delete bw;
}

int main(int argc, const char * argv[]) {
    
    //argv[1] = c/d
    //argv[2] = max velikost
    //argv[3] = pot
    int maxVel = 290;//atoi(argv[2]);
    test(maxVel);
    string pot("testoutput.bin");//argv[3]);
    //if(*argv[1] == 'c') {
        //compress(maxVel, pot);
        //double rate = getSize(argv[3])/getSize("out.bin");
        //cout << "Kompresijsko razmerje: " << rate << endl;
    //}
    //else if(*argv[1] == 'd') {
        //decompress(maxVel, pot);
        double rate = getSize("out.bin")/getSize(argv[3]);
        cout << "Kompresijsko razmerje: " << rate << endl;
    //}
    //else {
        //cout << "Napaka." << endl;
    //}
    
    return 0;
}
