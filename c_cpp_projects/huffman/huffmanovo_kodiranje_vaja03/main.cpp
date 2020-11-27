//
//  main.cpp
//  huffmanovo_kodiranje_vaja03
//
//  Created by Urban Vidovič on 07/05/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include <iostream>
#include <string>
#include <queue>
#include <fstream>
#include <vector>
#include "BinReader.h"

using namespace std;

struct Node {
    char znak; //znak
    unsigned int freq; //frekvenca znaka
    Node *l, *d; //levi in desni otrok
    
    Node(char znak1, unsigned int freq1) { //nastavitveni konstruktor za node
        l = d = NULL;
        this->znak = znak1;
        this->freq = freq1;
    }
};

struct primerjaj {
    bool operator()(Node* l, Node* d) {
        return (l->freq > d->freq);
    }
};

void izpisBinKode(struct Node *K, string str) {
    if(!K) {
        return;
    }
    if (K->znak != '#') {
        cout << K->znak << ": " << str << "\n";
    }
    izpisBinKode(K->l, str + "0");
    izpisBinKode(K->d, str + "1");
}

void huffmanovoKodiranje(vector<char> znaki, vector<int> freq, int vel) {
    Node *levi, *desni, *oce; //definicija kazalcev,ki se uporabljajo spodaj
    
    //ustvari minimalno kopico in vstavi vse znake iz arraya
    priority_queue<Node*, vector<Node*>, primerjaj> minKopica;
    
    for(int i = 0; i < vel; i++) {
        minKopica.push(new Node(znaki[i], freq[i]));
    }
    
    //dokler ima kopica velikost > 1
    while (minKopica.size() != 1) {
        //izvozi dva elementa z najmanjšo frekvenco iz minimalne kopice
        levi = minKopica.top();
        minKopica.pop();
        
        desni = minKopica.top();
        minKopica.pop();
        
        //ustvari nov node s frekvenco vsote zgornjih dveh izvoženih kopic, # je poseben znak, ki
        //omogoča preverjanje, ali gre le za vozlišče iz katerega peljeta še otroka,
        //ali za dejanski znak, ki ga kodiramo
        oce = new Node('#', levi->freq + desni->freq);
        
        //levi in desni node sta otroka tega novega nodea
        oce->l = levi;
        oce->d = desni;
        
        //vstavi to podkopico v celotno
        minKopica.push(oce);
    }
    // izpis huffmanovega drevesa, ki smo ga sestavli zgoraj
    izpisBinKode(minKopica.top(), "");
}

int main(int argc, const char * argv[]) {
    vector<char> arr = { 'u', 'r', 'b', 'a', 'n', 'x' };
    vector<int> freq = { 5, 9, 12, 13, 16, 45 };
    
    unsigned int size = arr.size();// sizeof(arr[0]);
    
    huffmanovoKodiranje(arr, freq, size);

    BinReader *br = new BinReader();
    
    cout << br->getTmp() << endl;
    
    for(int i = 7; i >= 0; i--) {
        cout << br->readBit();
    }
    cout << endl;
    
    for(int i = 7; i >= 0; i--) {
        cout << br->readBit();
    }
    cout << endl;
    
    cout << br->readByte() << endl;
    cout << br->readByte() << endl;

    return 0;
}
