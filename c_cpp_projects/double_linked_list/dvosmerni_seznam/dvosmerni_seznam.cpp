//
//  dvosmerni_seznam.cpp
//  dvosmerni_seznam
//
//  Created by Urban Vidovič on 19/03/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include <iostream>
#include <ctime>
#include <chrono>
#include <vector>

using namespace std;

struct element {
    int key;
    element *prev, *next;
};

element *head = NULL;
element *tail = NULL;
element *temp = NULL;
element *temp2 = NULL;

bool najdi(int key) {
    temp = head;
    bool exist = false;
    while(temp != NULL){
        if(temp->key == key) {
            exist = true;
            break;
        }
        else {
            temp = temp->next;
        }
    }
    return exist;
}

void vstaviZa(element *novi, int x) {
    if(najdi(x)) {
        novi->prev = temp;
        novi->next = temp->next;
        temp->next = novi;
        if(novi->next != NULL){
            novi->next->prev = novi;
        }
        else {
            tail = novi;
        }
    }
    else {
        cout << "Takšen element ne obstaja." << endl;
    }
}

void brisi(int x) {
    najdi(x);
    if(temp->prev == NULL && temp->next == NULL) {
        head = NULL;
        tail = NULL;
    }
    else {
        if(temp->prev != NULL) {
            temp->prev->next = temp->next;
        }
        else {
            head = temp->next;
            head->prev = NULL;
        }
        if(temp->next != NULL) {
            temp->next->prev = temp->prev;
        }
        else {
            tail = temp->prev;
            tail->next = NULL;
        }
    }
    delete temp;
}



void izbrisSeznama() {
    temp = head;
    temp2 = head;
    while(temp != NULL) {
        temp->key = 0;
        temp2 = temp;
        temp = temp->next;
        delete temp2;
    }
    head = NULL;
    temp = NULL;
    tail = NULL;
}

void izpisOdRepa() {
    temp = tail;
    while(temp != NULL) {
        cout << temp->key << " ";
        temp = temp->prev;
    }
}

void vstaviVg(element *novi) {
    novi->next = head;
    novi->prev = NULL;
    if(head != NULL) {
        head->prev = novi;
        head = novi;
    }
    else {
        head = novi;
        tail = novi;
    }
}

void zaRepom(element *novi) {
    if(tail != NULL) {
        novi->prev = tail;
        tail->next = novi;
        tail = novi;
        tail->next = NULL;
        cout << "Podatek vnešen." << endl;
    }
    else {
        cout << "Rep ni definiran." << endl;
    }
}

void izpisMoznosti() {
    cout << "1) Vstavljanje v glavo seznama in vsota seznama" << endl;
    cout << "2) Vstavljanje n elementov na zacetek polja in vsota polja" << endl;
    cout << "3) Vstavljanje n elementov na konec polja in vsota polja" << endl << endl;
    cout << "Izbira: ";
}

void vstaviNvG(int n) {
    izbrisSeznama();
    auto start = chrono::steady_clock::now(); //začetek merjenja
    
    for(int i = 0; i < n; i++) {
        element *novi = new element;
        novi->key = i+1;
        vstaviVg(novi);
    }
    
    auto end = chrono::steady_clock::now(); //konec merjenja
    cout << "Vstavljanje v glavo-čas trajanja: " << chrono::duration_cast<chrono::microseconds> (end - start).count() << " μs." << endl;
}

void vsotaSeznama(int n) {
    long long count = 0;
    temp = head;
    auto start = chrono::steady_clock::now(); //začetek merjenja
    if(temp != NULL) {
        for(int i = 0; i < n; i++) {
            count += temp->key;
            if(temp->next != NULL) {
                temp = temp->next;
            }
            else {
                cout << "Konec seznama." << endl;
            }
        }
    }
    else {
        cout << "Seznam ne vsebuje elementov." << endl;
    }
    auto end = chrono::steady_clock::now(); //konec merjenja
    cout << "Vsota-čas trajanja: " << chrono::duration_cast<chrono::microseconds> (end - start).count() << " μs." << endl;
    cout << "Vsota: " << count << endl;
}

void poljeNaPrviIndeks(int *polje, int n) {
    int count = 0;
    auto start = chrono::steady_clock::now(); //začetek merjenja
    for(int i = 1; i <= n; i++) {
        polje[0] = i;
        for(int j = 0; j < n-1-count; j++) { //premakne vse v desno
            swap(polje[j], polje[j+1]);
        }
        count++;
    }
    auto end = chrono::steady_clock::now(); //konec merjenja
    cout << "Vstavljanje na prvi indeks-čas trajanja: " << chrono::duration_cast<chrono::microseconds> (end - start).count() << " μs." << endl;
}

void poljeNaZadniIndeks(int *polje, int n) {
    auto start = chrono::steady_clock::now(); //začetek merjenja
    for(int i = 0; i < n; i++) {
        polje[i]=i+1;
    }
    auto end = chrono::steady_clock::now(); //konec merjenja
    cout << "Vstavljanje na zadnji indeks-čas trajanja: " << chrono::duration_cast<chrono::microseconds> (end - start).count() << " μs." << endl;
}

void vsotaPolja(int *polje, int n) {
    long long vsota = 0;
    auto start = chrono::steady_clock::now(); //začetek merjenja
    for(int i = 0; i < n; i++) {
        vsota += polje[i];
    }
    auto end = chrono::steady_clock::now(); //konec merjenja
    cout << "Vsota-čas trajanja: " << chrono::duration_cast<chrono::microseconds> (end - start).count() << " μs." << endl;
    cout << "Vsota polja: " << vsota << endl;
}

int main() {
    
    bool dela = true;
    char choice, choice2;
    int st, mesto, n;
    while(dela) {
        cout << "Dvojno povezan seznam - izbira" << endl << endl;
        cout << "1) Iskanje podatka" << endl;
        cout << "2) Vnos v glavo" << endl;
        cout << "3) Vnos za elementom" << endl;
        cout << "4) Vnos za repom" << endl;
        cout << "5) Brisanje podatka" << endl;
        cout << "6) Izpis od glave proti repu" << endl;
        cout << "7) Izpis od repa proti glavi" << endl;
        cout << "8) Testiraj hitrost" << endl;
        cout << "9) Konec" << endl << endl;
        cout << "Izbira: ";
        cin >> choice;
        
        element *novi = new element;
        switch(choice) {
            case '1':
                cout << "Vnesi podatek: ";
                cin >> st;
                if(najdi(st)) {
                    cout << "Podatek obstaja." << endl;
                }
                else {
                    cout << "Podatek še ne obstaja." << endl;
                }
                break;
                
            case '2':
                cout << "Vnesi podatek: ";
                cin >> st;
                novi->key = st;
                vstaviVg(novi);
                cout << "Podatek vnešen." << endl;
                break;
                
            case '3':
                cout << "Vnesi podatek: ";
                cin >> st;
                novi->key = st;
                cout << "Vnesi za katerim elementom želiš podatek: ";
                cin >> mesto;
                vstaviZa(novi, mesto);
                break;
                
            case '4':
                cout << "Vnesi podatek: ";
                cin >> st;
                novi->key = st;
                zaRepom(novi);
                break;
                
            case '5':
                cout << "Vnesi vrednost za izbris: ";
                cin >> st;
                brisi(st);
                cout << "Podatek izbrisan." << endl;
                break;
                
            case '6':
                izpisOdGlave();
                cout << endl << endl;
                break;
                
            case '7':
                izpisOdRepa();
                cout << endl << endl;
                break;
                
            case '8':
                cout << "Vnesi št. elementov za testiranje hitrosti: ";
                cin >> n;
                izpisMoznosti();
                cin >> choice2;
                if(choice2 == '1') {
                    vstaviNvG(n);
                    vsotaSeznama(n);
                }
                else if(choice2 == '2') {
                    int *polje = new int [n];
                    poljeNaPrviIndeks(polje, n);
                    vsotaPolja(polje, n);
                }
                else if(choice2 == '3') {
                    int *polje = new int [n];
                    poljeNaZadniIndeks(polje, n);
                    vsotaPolja(polje, n);
                }
                else {
                    cout << "Ne prepoznam ukaza." << endl;
                }
                break;
                
            case '9':
                dela = false;
                break;
                
            default:
                cout << "Ne prepoznam ukaza." << endl;
                break;
        }
    }
    
    return 0;
}
