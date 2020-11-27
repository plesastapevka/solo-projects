//
//  urejanje_seznama.cpp
//  urejanje_seznama
//
//  Created by Urban Vidovič on 30/04/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include <iostream>
#include <ctime>
#include <chrono>

using namespace std;
using chrono::duration_cast;
using chrono::microseconds;
using chrono::steady_clock;


struct element {
    int key;
    element *prev, *next;
};

element *head = NULL;
element *tail = NULL;
element *temp = NULL;
element *l = NULL;
element *d = NULL;

element* deliBrezM(element *dno, element *vrh) {
    int pe = dno->key;
    int tmpVal, tmpVal2;
    l = dno;
    d = vrh;
    bool cross = false;
    while(cross == false) {
        while(l->key <= pe && l != vrh) {
            l = l->next;
            if(l == d) {
                cross = true;
            }
        }
        while(d->key >= pe && d != dno) {
            d = d->prev;
            if(d == l) {
                cross = true;
            }
        }
        if(cross == false) {
            tmpVal = l->key;
            l->key = d->key;
            d->key = tmpVal;
        }
    }
    tmpVal2 = dno->key;
    dno->key = d->key;
    d->key = tmpVal2;
    return d;
}

void qSortBrezM(element *dno, element *vrh) {
    if(dno != vrh) {
        element *j = deliBrezM(dno, vrh);
        if(dno != j) {
            qSortBrezM(dno, j->prev);
        }
        if(vrh != j) {
            qSortBrezM(j->next, vrh);
        }
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

void generateZap(int size) {
    for(int i = 0; i < size; i++) {
        element *novi = new element;
        novi->key = rand() % 200001 + (-100000);
        vstaviVg(novi);
    }
}

void izpisZap() {
    temp = head;
    while(temp != NULL) {
        cout << temp->key << " ";
        temp = temp->next;
    }
}

bool preveri() {
    element *tmp = head;
    while(tmp->next) {
        if(tmp->next->key < tmp->key) {
            return false;
        }
        tmp = tmp->next;
    }
    return true;
}

int vsota() {
    int sum = 0;
    element *tmp = head;
    while(tmp) {
        sum += tmp->key;
        tmp = tmp->next;
    }
    return sum;
}

void generateNzap(int size) {
    int tmp = 10000;
    for(int i = 0; i < size; i++) {
        element *novi = new element;
        tmp += rand() % 100 + (-100);
        novi->key = tmp;
        vstaviVg(novi);
    }
}

int main() {
    
    srand(time_t(NULL));
    bool dela = true;
    int size;
    char choice;
    
    steady_clock::time_point start;
    steady_clock::time_point end;
    
    while(dela) {
        cout << "Hitro uredi dvojno-povezan seznam - izbira" << endl << endl;
        cout << "1) Generiraj naključno zaporedje" << endl;
        cout << "2) Generiraj naraščajoče zaporedje" << endl;
        cout << "3) Izpis zaporedja" << endl;
        cout << "4) Preveri ali je zaporedje urejeno" << endl;
        cout << "5) Izpiši vsoto elementov" << endl;
        cout << "6) Uredi" << endl;
        cout << "0) Konec" << endl << endl;
        cout << "Izbira: ";
        cin >> choice;
        
        switch(choice) {
            case '1': {
                cout << "Vnesi velikost polja: ";
                cin >> size;
                if(head != NULL) {
                    temp = head;
                    while(temp != NULL) {
                        temp = head->next;
                        delete head;
                        head = temp;
                    }
                    generateZap(size);
                }
                else {
                    generateZap(size);
                }
                cout << "Generirano." << endl;
                break;
            }
                
            case '2':
                cout << "Vnesi velikost polja: ";
                cin >> size;
                if(head != NULL) {
                    temp = head;
                    while(temp != NULL) {
                        temp = head->next;
                        delete head;
                        head = temp;
                    }
                    generateNzap(size);
                }
                else {
                    generateNzap(size);
                }
                cout << "Generirano." << endl;
                break;
                
            case '3':
                izpisZap();
                cout << endl;
                break;
                
            case '4':
                if(preveri()) {
                 cout << "Zaporedje JE urejeno." << endl;
                 }
                 else {
                 cout << "Zaporedje NI urejeno." << endl;
                 }
                break;
                
            case '5':
                if(head != NULL) {
                    cout << "Vsota je " << vsota() << "." << endl;
                }
                else {
                    cout << "Seznam ne obstaja." << endl;
                }
                break;
                
            case '6':
                start = steady_clock::now();
                qSortBrezM(head, tail);
                end = steady_clock::now(); std::cout << "Trajanje: "<<
                duration_cast<microseconds>(end - start).count() << "µs" << endl;
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

