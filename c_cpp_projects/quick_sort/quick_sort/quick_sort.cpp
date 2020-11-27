//
//  quick_sort.cpp
//  quick_sort
//
//  Created by Urban Vidovič on 15/04/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include <iostream>
#include <ctime>
#include <chrono>

using namespace std;
using chrono::duration_cast;
using chrono::microseconds;
using chrono::steady_clock;

int deliZm(int *a, int dno, int vrh) {
    int m = dno + (vrh-dno)/2;
    swap(a[dno], a[m]);
    int pe = a[dno];
    int l = dno;
    int d = vrh;
    while(l < d) {
        while(a[l] <= pe && l < vrh) {
            l++;
        }
        while(a[d] >= pe && d > dno) {
            d--;
        }
        if(l < d) {
            swap(a[l], a[d]);
        }
    }
    swap(a[dno], a[d]);
    return d;
}

int deliBrezM(int *a, int dno, int vrh) {
    int pe = a[dno];
    int l = dno;
    int d = vrh;
    while(l < d) {
        while(a[l] <= pe && l < vrh) {
            l++;
        }
        while(a[d] >= pe && d > dno) {
            d--;
        }
        if(l < d) {
            swap(a[l], a[d]);
        }
    }
    swap(a[dno], a[d]);
    return d;
}

void qSortzM(int *a, int dno, int vrh) {
    if(dno < vrh) {
        int j = deliZm(a, dno, vrh);
        qSortzM(a, dno, j-1);
        qSortzM(a, j+1, vrh);
    }
}

void qSortBrezM(int *a, int dno, int vrh) {
    if(dno < vrh) {
        int j = deliBrezM(a, dno, vrh);
        qSortBrezM(a, dno, j-1);
        qSortBrezM(a, j+1, vrh);
    }
}

void generateZap(int *a, int size) {
    for(int i = 0; i < size; i++) {
        a[i] = rand() % 200001 + (-100000);
    }
}

void izpisZap(int *a, int size) {
    for(int i = 0; i < size; i++) {
        cout << a[i] << endl;
    }
}

void selectionSort(int *polje, int n) {
    for(int i = 0; i < n - 1; i++) {
        int min;
        min = i;
        for(int j = i + 1; j < n; j++) {
            if(polje[min] > polje[j]) {
                min = j;
            }
        }
        if(min != i) {
            swap(polje[min], polje[i]);
        }
    }
}

bool preveri(int *a, int size) {
    for(int i = 0; i < size; i++) {
        if(a[i] > a[i+1]) {
            return false;
        }
    }
    return true;
}

void generatePzap(int *a, int size) {
    int x = rand() % 20001 + 10000;
    for(int i = 0; i < size; i++) {
        a[i] = x - (rand() % 1001 + 0);
        x = a[i];
    }
}

void generateNzap(int *a, int size) {
    int x = rand() % 10001 + (-10000);
    for(int i = 0; i < size; i++) {
        a[i] = x + (rand() % 1001 + 0);
        x = a[i];
    }
}

int main() {
    
    srand(time_t(NULL));
    bool dela = true;
    int size;
    int *a = new int [size];
    char choice;
    
    steady_clock::time_point start;
    steady_clock::time_point end;
    
    while(dela) {
        cout << "Hitro uredi - izbira" << endl << endl;
        cout << "1) Generiraj naključno zaporedje" << endl;
        cout << "2) Generiraj naraščajoče zaporedje" << endl;
        cout << "3) Generiraj padajoče zaporedje" << endl;
        cout << "4) Izpis zaporedja" << endl;
        cout << "5) Preveri ali je zaporedje urejeno" << endl;
        cout << "6) Hitro urejanje brez mediane" << endl;
        cout << "7) Hitro urejanje z mediano" << endl;
        cout << "8) Urejanje z drugim algoritmom" << endl << endl;
        cout << "0) Konec" << endl << endl;
        cout << "Izbira: ";
        cin >> choice;
        
        switch(choice) {
            case '1': {
                cout << "Vnesi velikost polja: ";
                cin >> size;
                generateZap(a, size);
                cout << "Generirano." << endl;
                break;
            }
                
            case '2':
                cout << "Vnesi velikost polja: ";
                cin >> size;
                generateNzap(a, size);
                cout << "Generirano." << endl;
                break;
                
            case '3':
                cout << "Vnesi velikost polja: ";
                cin >> size;
                generatePzap(a, size);
                cout << "Generirano." << endl;
                break;
                
            case '4':
                izpisZap(a, size);
                break;
                
            case '5':
                if(preveri(a, size) == true) {
                    cout << "Je." << endl;
                }
                else {
                    cout << "Ni." << endl;
                }
                break;
                
            case '6':
                start = steady_clock::now();
                qSortBrezM(a, 0, size-1);
                end = steady_clock::now(); std::cout << "Trajanje: "<<
                duration_cast<microseconds>(end - start).count() << "µs" << endl;
                break;
                
            case '7':
                start = steady_clock::now();
                qSortzM(a, 0, size-1);
                end = steady_clock::now(); std::cout << "Trajanje: "<<
                duration_cast<microseconds>(end - start).count() << "µs" << endl;
                break;
                
            case '8':
                start = steady_clock::now();
                selectionSort(a, size);
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

/*
 10 000              20 000              30 000              40 000              50 000
 Z MEDIANO
 naključno            740µs               1736µs              2498µs              3446µs              4851µs
 naraščajoče          139µs               286µs               432µs               611µs               1094µs
 padajoče             166µs               339µs               522µs               695µs               911µs
 
 BREZ MEDIANE
 naključno            715µs               1583µs              2891µs              3681µs              4259µs
 naraščajoče          27590µs             108568µs            235602µs            421327µs            639290µs
 padajoče             27960µs             109377µs            237884µs            433832µs            653016µs
 
 SELECTION SORT
 naključno            141857µs            554531µs            1219780µs           2148639µs           3385776µs
 naraščajoče          135745µs            544938µs            1220717µs           2144956µs           3377758µs
 padajoče             143270µs            536881µs            1228915µs           2172654µs           3351720µs
 */

