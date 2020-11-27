//
//  BinReader.h
//  huffmanovo_kodiranje_vaja03
//
//  Created by Urban Vidovič on 07/05/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#ifndef BinReader_h
#define BinReader_h
#include <string>

class BinReader {
  private:
    int tmp = 44;
    int bitCounter;
    int count;
    std::string path;
    
  public:
    //konstruktor
    BinReader(/*std::string path1*/);
    
    int getTmp() {return tmp;}
    int getBC() {return bitCounter;}
    int getCounter() {return count;}
    
    //ostale metode
    bool  readBit();
    char  readByte();
    int   readInt();
    float readFloat();
};


#endif /* BinReader_h */
