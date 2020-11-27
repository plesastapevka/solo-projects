//
//  main.cpp
//  sklad_vrsta
//
//  Created by Urban Vidovič on 06/03/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include <iostream>
#include <array>

using namespace std;

int vrh = 0; //indeks za sklad
int max1, max2; //velikosti posameznih polj
int glava = 0, rep = 0; //indeksa za krožno vrsto

void push(int *a, int n) { //sklad
    if(vrh == max1) {
        cout << "Sklad je že poln." << endl;
    }
    else {
        a[vrh] = n;
        vrh++;
        cout << a[vrh-1] << endl;
        cout << "Podatek vnešen." << endl;
    }
}

void pop(int *a) { //sklad
    if(vrh == 0) {
        cout << "Sklad je prazen." << endl;
    }
    else {
        int rez = a[vrh - 1];
        vrh = vrh - 1;
        cout << rez << endl;
    }
}

void izpis(int *a) { //sklad
    if(vrh == 0) {
        cout << "Sklad je prazen." << endl;
    }
    else {
        for(int i = 0; i < vrh; i++) {
            cout << a[i] << endl;
        }
    }
}

void beri(int *Q) { //vrsta
    if(glava == rep) {
        cout << "Vrsta je prazna." << endl;
    }
    else {
        int x = Q[glava];
        glava = (glava % max2) + 1;
        cout << x << endl;
    }
}

void vpisi(int *Q, int x) { //vrsta
    int novi_rep = (rep % max2) + 1;
    if(glava == novi_rep) {
        cout << "Vrsta je polna." << endl;
    }
    else {
        Q[rep] = x;
        rep = novi_rep;
        cout << "Podatek vnešen." << endl;
    }
}

void izpis_v(int *Q) { //vrsta
    if(glava == rep) {
        cout << "Vrsta je prazna." << endl;
    }
    else {
        for(int i = glava; i < rep; i++) {
            cout << Q[i] << endl;
        }
    }
}

int main() {
    char to = NULL;
    cout << "Vnesi velikost sklada: ";
    cin >> max1;
    cout << "Vnesi velikost vrste: ";
    cin >> max2;
    int S[max1];
    int Q[max2];
    
    while(to != '7') {
        cout << "=================================" << endl;
        cout << "Sklad - izbira:\n";
        cout << "1) Vnos podatka\n";
        cout << "2) Branje podatka\n";
        cout << "3) Izpis vsebine sklada\n" << endl;
        cout << "Krozna vrsta - izbira:\n";
        cout << "4) Vnos podatka\n";
        cout << "5) Branje podatka\n";
        cout << "6) Izpis vrste od glave do repa\n" << endl;
        cout << "7) Konec\n";
        cout << "=================================" << endl << endl;
        cout << "Izbira: ";
        cin >> to;
                
        switch(to) {
            case '1': //vnos podatka
                int num1;
                cout << "Vnesi podatek: ";
                cin >> num1;
                push(S, num1);
                break;
            
            case '2': //branje podatka
                pop(S);
                break;
                
            case '3': //izpis vsebine sklada
                izpis(S);
                break;
                
            case '4': //vnos podatka k.v.
                int num2;
                cout << "Vnesi podatek: ";
                cin >> num2;
                vpisi(Q, num2);
                break;
                
            case '5': //branje podatka k.v.
                beri(Q);
                break;
                
            case '6': //izpis vsebine krozne vrste
                izpis_v(Q);
                break;
                
            default:
                cout << "Napačna izbira." << endl;
        }
    }
    return 0;
}
