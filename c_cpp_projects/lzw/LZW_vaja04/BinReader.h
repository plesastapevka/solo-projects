//
//  BinReader.h
//  LZW_vaja04
//
//  Created by Urban Vidovič on 09/06/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#ifndef BinReader_h
#define BinReader_h

class BinReader {
    
public:
    std::ifstream ifd;
    int velikost;
    
    BinReader(std::string in, int vel): velikost(vel) {
        ifd.open(in, std::ios::binary);
    }
    
    ~BinReader() {
        ifd.close();
    }
    
    int readInt() {
        if(ifd.eof()) {
            return -88276;
        }
        int B;
        ifd.read((char*)&B, velikost);
        return B;
    }
    
};

#endif /* BinReader_h */
