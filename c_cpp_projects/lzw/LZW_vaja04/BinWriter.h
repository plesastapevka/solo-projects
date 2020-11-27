//
//  BinWriter.h
//  LZW_vaja04
//
//  Created by Urban Vidovič on 09/06/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#ifndef BinWriter_h
#define BinWriter_h

class BinWriter {
    
public:
    std::ofstream ofd;
    int velikost;
    
    BinWriter(std::string out, int vel): velikost(vel) {
        ofd.open(out, std::ios::binary);
    }
    
    ~BinWriter() {
        ofd.close();
    }
    
    void writeInt(int &st) {
        ofd.write((char*)&st, velikost);
    }
};

#endif /* BinWriter_h */
