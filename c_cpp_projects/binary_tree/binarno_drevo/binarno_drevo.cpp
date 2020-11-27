//
//  binarno_drevo.cpp
//  binarno_drevo
//
//  Created by Urban Vidovič on 25/03/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include <iostream>

using namespace std;

struct Vozlisce {
    int key;
    Vozlisce* oce = NULL;
    Vozlisce* lSin = NULL;
    Vozlisce* dSin = NULL;
};

Vozlisce *y = NULL;
Vozlisce *z = NULL;
Vozlisce *x = NULL;
Vozlisce *T = NULL;
Vozlisce *temp = NULL;


void najdi(Vozlisce *ptr, int k) {
    if(ptr == NULL) {
        cout << "Ni podatka." << endl;
    }
    else if(ptr->key == k) {
        cout << ptr->key << endl;
        temp = ptr;
    }
    else {
        if(k < ptr->key) {
            najdi(ptr->lSin, k);
        }
        else {
            najdi(ptr->dSin, k);
        }
    }
}

void insert(int k) {
    y = NULL;
    x = T;
    while(x != NULL) {
        y = x;
        if(k > x->key) {
            x = x->dSin;
        }
        else if(k < x->key) {
            x = x->lSin;
        }
        else {
            cout << "Napaka." << endl;
            return;
        }
    }
    z = new Vozlisce;
    z->key = k;
    z->oce = y;
    if(y == NULL) {
        T = z;
    }
    else {
        if(z->key < y->key) {
            y->lSin = z;
        }
        else {
            y->dSin = z;
        }
    }
    cout << "Podatek vnešen" << endl;
}

void izpis(Vozlisce *ptr) {
    if(ptr != NULL) {
        izpis(ptr->lSin);
        cout << ptr->key << endl;
        izpis(ptr->dSin);
    }
}

void povezave(Vozlisce *ptr) {
    if(ptr->lSin != NULL) {
        cout << ptr->key << " -> " << ptr->lSin->key << endl;
        povezave(ptr->lSin);
    }
    if(ptr->dSin != NULL) {
        cout << ptr->key << " -> " << ptr->dSin->key << endl;
        povezave(ptr->dSin);
    }
}

int min(Vozlisce *ptr) {
    while(ptr->lSin != NULL){
        ptr = ptr->lSin;
    }
    temp = ptr;
    return ptr->key;
}

int max(Vozlisce *ptr) {
    while(ptr->dSin != NULL){
        ptr = ptr->dSin;
    }
    temp = ptr;
    return ptr->key;
}

int next(Vozlisce *ptr) {
    if(ptr->dSin != NULL){
        return min(ptr->dSin);
    }
    y = ptr->oce;
    while(y != NULL && x == y->dSin) {
        x = y;
        y = y->oce;
    }
    temp = y;
    return y->key;
}

int prev(Vozlisce *ptr) {
    if(ptr->lSin != NULL){
        return max(ptr->lSin);
    }
    y = ptr->oce;
    while(y != NULL && x == y->lSin) {
        x = y;
        y = y->oce;
    }
    temp = y;
    return y->key;
}

void brisi(Vozlisce *ptr) {
    if(ptr->lSin == NULL || ptr->dSin == NULL) {
        y = ptr;
    }
    else {
        next(ptr);
        y = temp;
    }
    if(y->lSin != NULL) {
        x = y->lSin;
    }
    else {
        x = y->dSin;
    }
    if(x != NULL) {
        x->oce = y->oce;
    }
    if(y->oce == NULL) {
        T = x;
    }
    else {
        if(y == y->oce->lSin) {
            y->oce->lSin = x;
        }
        else {
            y->oce->dSin = x;
        }
    }
    if(y != ptr) {
        ptr->key = y->key;
    }
    delete y;
}

int main() {
    //int st;
    bool dela = true;
    
    char choice;
    while(dela) {
        cout << "Binarno iskalno drevo - izbira" << endl << endl;
        cout << "1) Vnos podatka" << endl;
        cout << "2) Urejen izpis vrednosti" << endl;
        cout << "3) Izpis povezav" << endl;
        cout << "4) Iskanje" << endl;
        cout << "5) Poišči minimum ali maksimum" << endl;
        cout << "6) Poišči predhodnika in naslednika" << endl;
        cout << "7) Briši vrednost" << endl;
        cout << "8) Konec" << endl << endl;
        cout << "Izbira: ";
        cin >> choice;
    
        switch(choice) {
            case '1':
                cout << "Vnesi st.: ";
                int st;
                cin >> st;
                insert(st);
                cout << endl;
                break;
                
            case '2':
                izpis(T);
                cout << endl;
                break;
                
            case '3':
                povezave(T);
                cout << endl;
                break;
                
            case '4':
                cout << "Vnesi iskani podatek: ";
                cin >> st;
                najdi(T, st);
                break;
                
            case '5':
                cout << "1) Maksimum" << endl;
                cout << "2) Minimum" << endl;
                cout << "Izbira: ";
                char choice2;
                cin >> choice2;
                if(choice2 == '1') {
                    cout << "Maksimum: " << max(T) << endl << endl;
                }
                else if(choice2 == '2') {
                    cout << "Minimum: " << min(T) << endl << endl;
                }
                else {
                    cout << "Ne prepoznam ukaza." << endl << endl;
                }
                break;
                
            case '6':
                cout << "1) Naslednik" << endl;
                cout << "2) Predhodnik" << endl;
                cout << "Izbira: ";
                char choice3;
                cin >> choice3;
                if(choice3 == '1') {
                    cout << "Najdi naslednika od: ";
                    int st2;
                    cin >> st2;
                    najdi(T, st2);
                    cout << "Naslednik: " << next(temp) << endl << endl;
                }
                else if(choice3 == '2') {
                    cout << "Najdi predhodnika od: ";
                    int st3;
                    cin >> st3;
                    najdi(T, st3);
                    cout << "Predhodnik: " << prev(temp) << endl << endl;
                }
                else {
                    cout << "Ne prepoznam ukaza." << endl << endl;
                }
                break;
                
            case '7':
                cout << "Vnesi vrednost za izbris: ";
                int st4;
                cin >> st4;
                najdi(T, st4);
                brisi(temp);
                cout << endl;
                break;
                
            case '8':
                dela = false;
                break;
                
            default:
                cout << "Ne prepoznam ukaza." << endl;
                break;
        }
    }
    return 0;
}
