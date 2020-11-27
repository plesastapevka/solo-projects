//
//  main.cpp
//  hanojski_stolpi
//
//  Created by Urban Vidovič on 08/03/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include <iostream>
#include <string>
using namespace std;

//globalna inicializacija spremenljivke max
int iMax;
int trigger = 0;

bool konec = true;

void fillSklad(int *a, int *b, int *c, int &vrh) { //sklad
    int x = iMax;
    for(int i = 0; i < iMax; i++) {
        vrh++;
        a[vrh] = x;
        x--;
    }
    for(int i = 0; i < iMax; i++) {
        b[i] = 0;
    }
    for(int i = 0; i < iMax; i++) {
        c[i] = 0;
    }
}

void izris(int *a, int *b, int *c) {
    cout << "Stolp 1" << endl;
    for(int i = iMax-1; i > -1; i--) {
        cout << string(a[i], '*') << endl;
    }
    cout << "\nStolp 2" << endl;
    for(int i = iMax-1; i > -1; i--) {
        cout << string(b[i], '*') << endl;
    }
    cout << "\nStolp 3" << endl;
    for(int i = iMax-1; i > -1; i--) {
        cout << string(c[i], '*') << endl;
    }
}

void premik(int *prvi, int *drugi, int &prvivrh, int &drugivrh) {
    if(prvi[prvivrh] == 0) {
        cout << "Stolp iz katerega želite premikati ploščice je prazen." << endl;
    }
    else if(drugi[drugivrh] == 0) {
        if(prvivrh-1 == -1) {
            drugi[drugivrh] = prvi[prvivrh];
            prvi[prvivrh] = 0;
        }
        else {
            drugi[drugivrh] = prvi[prvivrh];
            prvi[prvivrh] = 0;
            prvivrh--;
        }
    }
    else if(prvi[prvivrh] < drugi[drugivrh]) {
        if(prvivrh-1 == -1) {
            drugivrh++;
            drugi[drugivrh] = prvi[prvivrh];
            prvi[prvivrh] = 0;
        }
        else {
            drugivrh++;
            drugi[drugivrh] = prvi[prvivrh];
            prvi[prvivrh] = 0;
            prvivrh--;
        }
    }
    else {
        cout << "Ta premik ni dovoljen." << endl;
    }
}

int main() {
    int vrh1 = -1;
    int vrh2 = 0;
    int vrh3 = 0;
    cout << "HANOJSKI STOLPI" << endl;
    cout << "Vnesi stopnjo zahtevnosti: ";
    cin >> iMax;
    int *a = new int [iMax];
    int *b = new int [iMax];
    int *c = new int [iMax];
    
    fillSklad(a, b, c, vrh1);
    izris(a, b, c);
    
    while(vrh3 != iMax-1) {
        string izbira;
        cout << "Prestavi ploščico iz (oblika Y-Z): ";
        cin >> izbira;
        if(izbira == "1-2") { //prva
            premik(a, b, vrh1, vrh2);
        }
        else if(izbira == "1-3") { //druga
            premik(a, c, vrh1, vrh3);
        }
        else if(izbira == "2-1") { //tretja
            premik(b, a, vrh2, vrh1);
        }
        else if(izbira == "2-3") { //cetrta
            premik(b, c, vrh2, vrh3);
        }
        else if(izbira == "3-1") { //peta
            premik(c, a, vrh3, vrh1);
        }
        else if(izbira == "3-2") { //sesta
            premik(c, b, vrh3, vrh2);
        }
        else {
            cout << "Ne prepoznam željenega ukaza." << endl;
        }
        izris(a, b, c);
    }
    
    cout << "Čestitke!" << endl;
    
    
    
    return 0;
}
