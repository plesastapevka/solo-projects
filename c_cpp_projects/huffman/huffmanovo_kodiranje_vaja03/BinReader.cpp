//
//  BinReader.cpp
//  huffmanovo_kodiranje_vaja03
//
//  Created by Urban Vidovič on 07/05/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include "BinReader.h"
#include <iostream>
#include <string>
#include <fstream>

using namespace std;

BinReader::BinReader(/*string path1): path(path1*/): bitCounter(7), count(1) {}

bool BinReader::readBit() {
    ifstream data("latlon.bin", ios::binary);
    char c;
    bool bit;
    
    for(int i = 0; i < count; i++) {
        data.get(c);
    }
    if(!data.eof()) {
        bit = (c >> bitCounter) & 1;
        bitCounter--;
        if(bitCounter == -1) {
            count++;
            bitCounter = 7;
        }
        data.close();
        return bit;
    }
    
    return 0;
}

char BinReader::readByte() {
    ifstream data("latlon.bin", ios::binary);
    char c = '\0';
    if(!data.eof()) {
        for(int i = 0; i < count; i++) {
            data.get(c);
        }
        data.close();
        count++;
        return c;
    }
    data.close();
    return '\0';
}

int BinReader::readInt() {
    ifstream data("latlon.bin, ios::binary");
    int tmp = 0;
    char c = '\0';
    for(int i = 0; i < count; i++) {
        data.get(c);
        cout << c << endl;
    }
    tmp += c;
    for(int i = 0; i < sizeof(int); i++) {
        data.get(c);
        count++;
        tmp += c;
    }
    data.close();
    return tmp;
}
