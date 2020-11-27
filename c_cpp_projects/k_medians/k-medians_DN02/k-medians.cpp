//
//  main.cpp
//  grucenje_vaja05
//
//  Created by Urban Vidovič on 11/06/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#include <iostream>
#include <ctime>
#include <cmath>
#include <vector>
#include <fstream>
#include "Point.h"
#include "Cluster.h"

using namespace std;

double decDouble(double min, double max) {
    double num = (double)rand() / RAND_MAX;
    return (min + num * (max - min));
}

double selectionSort(vector<double> polje, int n) {
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
    
    /*for(int i = 0; i < n; i++) {
        cout << polje[i] << " " << endl;
    }
    cout << endl;*/
    
    if(polje.size() % 2 == 0) {
        return ((polje[(polje.size()/2)] + polje[(polje.size()/2)-1])/2);
    }
    else {
        return polje[polje.size()/2];
    }
}

int main(int argc, char *argv[]) {
    
    //argv[1] = t - st iteracij algoritma
    //argv[2] = k - st iskanih gruc
    //argv[3] = pot
    int t = atoi(argv[1]);
    int k = atoi(argv[2]);
    string path(argv[3]);
    
    //inicializacija vseh potrebnih spremenljivk za algoritem
    srand(time(NULL));
    double x, y, MAX_RANGE_X = 0, MAX_RANGE_Y = 0,
                 MIN_RANGE_X = 9999999999, MIN_RANGE_Y = 9999999999;
    vector<Point*> dataSet;     //polje tock
    vector<Cluster*> clusters;  //polje nakljucnih gruc
    vector<double> tempValuesX;  //polje double stevilk za iskanje mediane in sortiranje
    vector<double> tempValuesY;
    ifstream in;
    in.open(path);
    Point *tmpP = NULL;
    
    while(in) { //branje iz vhodne datoteke in inicializacija tock
        if(in.eof()) {
            break;
        }
        in >> x >> y;
        tmpP = new Point(x, y);
        dataSet.push_back(tmpP);
        if(tmpP->getX() > MAX_RANGE_X) {
            MAX_RANGE_X = tmpP->getX();
        }
        
        if(tmpP->getX() < MIN_RANGE_X) {
            MIN_RANGE_X = tmpP->getX();
        }
        
        if(tmpP->getY() > MAX_RANGE_Y) {
            MAX_RANGE_Y = tmpP->getY();
        }
        
        if(tmpP->getY() < MIN_RANGE_Y) {
            MIN_RANGE_Y = tmpP->getY();
        }
        delete tmpP;
    }
    in.close();
    
    //cout << "X: " << MIN_RANGE_X << " - " << MAX_RANGE_X << endl;
    //cout << "Y: " << MIN_RANGE_Y << " - " << MAX_RANGE_Y << endl;
    
    for(int i = 0; i < k; i++) {  //inicializacija clusterjev
        clusters.push_back(new Cluster(decDouble(MIN_RANGE_X, MAX_RANGE_X), decDouble(MIN_RANGE_Y, MAX_RANGE_Y)));
    }
    
    double min = 0;
    int tempIndex = 0;
    double distance = 0;
    
    while(t != 0) {  //zacetek iteracij algoritma k-means clustering
        for(int i = 0; i < k; i++) {
            clusters[i]->clearContent();
        }
        //racunanje evklidske razdalje med clusterjem in tocko in dolocanje gruc
        for(int i = 0; i < dataSet.size(); i++) {
            min = Point::calculateEDistance(dataSet[i], clusters[0]);
            tempIndex = 0;
            for(int j = 1; j < k; j++) {
                distance = Point::calculateEDistance(dataSet[i], clusters[j]);
                if(distance < min) {
                    min = distance;
                    tempIndex = j;
                }
            }
            clusters[tempIndex]->addToVector(dataSet[i]);
        }
        
        //recomputing clusterjev - iskanje mediane
        for(int i = 0; i < k; i++) {
            tempValuesX.clear();
            tempValuesY.clear();
            for(int j = 0; j < clusters[i]->getSize(); j++) {
                //cout << clusters[i]->access(j)->getX() << endl;
                tempValuesX.push_back(clusters[i]->access(j)->getX());
                tempValuesY.push_back(clusters[i]->access(j)->getY());
            }
            if(clusters[i]->getSize() != 0) {
                clusters[i]->setX(selectionSort(tempValuesX, tempValuesX.size()));
                clusters[i]->setY(selectionSort(tempValuesY, tempValuesY.size()));
            }
        }
        /*for(int i = 0; i < k; i++) {
            cout << "(" << clusters[i]->getX() << ", " << clusters[i]->getY() << ")\n";
        }
        cout << endl;*/
        t--;
    }
    ofstream out;
    out.open("out.txt");
    for(int i = 0; i < k; i++) { //zapis rezultatov v novi out.txt file
        for(int j = 0; j < clusters[i]->getSize(); j++) {
            out << clusters[i]->getInd() << " " << clusters[i]->access(j)->getIndex() << endl;
        }
    }
    out.close();
    
    
    
    return 0;
}

