//
//  iskanje_v_globino.cpp
//  iskanje_v_globino
//
//  Created by Urban Vidovič on 08/05/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include <iostream>
#include <cstring>
#include <cmath>
#include <stack>
#include <fstream>

using namespace std;

enum quo {none = 0, unchecked = 1, processing = 2, developed = 3};

struct Vozlisce {
    int predhodnik;
    int dolzina;
    quo status;
    int indeks;
    string ime;
};

/*function ISKANJE_V_GLOBINO(G, s, d) begin
for each v ∈ V-{s} do
begin
    v.status := NEPREGLEDANO;
    v.dolzina := ∞;
    v.predhodnik := -1;
end
s.status := V_OBDELAVI;
s.dolzina := 0;
s.predhodnik := -1;
PUSH(s);
while not SKLAD_PRAZEN() do begin
v=POP();
if v = d then return;
for each vozlišče u ∈ sosedi{v} do
if u.status = NEPREGLEDANO then begin
u.status := V_OBDELAVI;
u.dolzina := v.dolzina+1;
u.predhodnik := v.indeks;
PUSH(u);
end
v.status := RAZVITO;
end end*/

void iskanjeVglobino(int **G, Vozlisce *s, Vozlisce *d) {
    stack<Vozlisce *> sklad;
    Vozlisce *v = new Vozlisce;
    Vozlisce *u = new Vozlisce;
    bool stack_empty = true;
    for(int i = 0; i < 50; i++) {
        v->status = unchecked;
        v->dolzina = pow(2, 15) - 1;
        v->predhodnik = -1;
    }
    s->status = processing;
    s->dolzina = 0;
    s->predhodnik = -1;
    sklad.push(s);
    while(!stack_empty) {
        v = sklad.top();
        sklad.pop();
        if(v == d) {
            return;
        }
        for(int i = 0; i < 50; i++) {
            if(u->status == unchecked) {
                u->status = processing;
                u->dolzina = v->dolzina + 1;
                u->predhodnik = v->indeks;
                sklad.push(u);
            }
            v->status = developed;
        }
    }
}

int findIndex(string name, int size, string *V) {
    for(int i = 0; i < size; i++) {
        if(V[i] == name) {
            return i;
        }
    }
    
    return -1;
}

int main() {
    char choice;
    bool dela = true;
    int iStvozlisc, iStpovezav, cena;
    string imeV1, imeV2;
    
    ifstream f("graf.txt");
    f >> iStvozlisc;
    f >> iStpovezav;
    //ustvari C in V;
    
    //init matrika vozlišč - sosednost
    string *V = new string [iStvozlisc];
    
    int vrstice = iStvozlisc; //vrstice, št vozlišč
    int stolpci = vrstice; //stolpci, št povezav
    int counter = vrstice - 1;
    
    int **C = new int *[vrstice];
    for(int i = 0; i < vrstice; i++){
        C[i] = new int [stolpci];
    }
    
    for(int i = 0; i < iStpovezav; i++) {
        f >> imeV1 >> imeV2 >> cena;
        int indexV1 = findIndex(imeV1, iStvozlisc, V); //najdi indeks v seznamu vozlišč (V) glede na ime (imeV1)
        if(indexV1 == -1){ //vozlišče še ne obstaja v V
            V[counter] = imeV1; //Dodaj novo vozlišče z imenom imeV1 na konec polja V
            indexV1 = findIndex(imeV1, iStvozlisc, V); //najdi indeks v seznamu vozlišč (V) glede na ime (imeV1)
        }
        //...
        //v matriki C označi vozlišči v1 in v2 kot soseda
        //...
    }
    
    //delete matrika
    for(int i = 0; i < vrstice; i++){
        delete []C[i];
    }
    delete []C;
    
    while(dela) {
        cout << "Iskanje v globino in širino - izbira" << endl << endl;
        cout << "1) Preberi graf iz datoteke" << endl;
        cout << "2) Poženi iskanje v globino od s do d" << endl;
        cout << "3) Poženi iskanje v širino od s do d\n" << endl;
        cout << "0) Konec" << endl << endl;
        cout << "Izbira: ";
        cin >> choice;
        
        switch(choice) {
            case '1':
                break;
                
            case '2':
                break;
                
            case '3':
                break;
                
            case '0':
                dela = false;
                break;
                
            default:
                cout << "Ne prepoznam ukaza." << endl;
                break;
        }
    }
    
    return 0;
}
