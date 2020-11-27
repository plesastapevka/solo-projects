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

int main(int argc, char *argv[]) {
    
    //argv[1] = t - st iteracij algoritma
    //argv[2] = k - st iskanih gruc
    //argv[3] = pot
    int t = 2;//atoi(argv[1]);
    int k = 4;//atoi(argv[2]);
    string path("vhod.txt"/*argv[3]*/);
    
    //inicializacija vseh potrebnih spremenljivk za algoritem
    srand(time(NULL));
    double x, y, MAX_RANGE_X = 0, MAX_RANGE_Y = 0,
                 MIN_RANGE_X = 9999999999, MIN_RANGE_Y = 9999999999;
    vector<Point*> dataSet;     //polje tock
    vector<Cluster*> clusters;  //polje nakljucnih gruc
    ifstream in;
    in.open(path);
    Point *tmpP = NULL;
    Cluster *tmpC = NULL;
    
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
    }
    in.close();
    
    /*for(int i = 0; i < k; i++) {  //inicializacija clusterjev
        tmpC = new Cluster(decDouble(MIN_RANGE_X, MAX_RANGE_X), decDouble(MIN_RANGE_Y, MAX_RANGE_Y));
        clusters.push_back(tmpC);
    }*/
    
    clusters.push_back(new Cluster(2, 6));
    clusters.push_back(new Cluster(0.8, 4));
    clusters.push_back(new Cluster(3, 2));
    clusters.push_back(new Cluster(1, 0.7));
    
    double min = 0;
    int tempIndex = 0;
    double distance = 0, sumX = 0, sumY = 0, tempX = 0, tempY = 0;
    
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
        
        //recomputing clusterjev
        for(int i = 0; i < k; i++) {
            sumX = 0;
            sumY = 0;
            for(int j = 0; j < clusters[i]->getSize(); j++) {
                tempX = clusters[i]->access(j)->getX();
                sumX += tempX;
                tempY = clusters[i]->access(j)->getY();
                sumY += tempY;
            }
            if(clusters[i]->getSize() != 0) {
                clusters[i]->setX(sumX/clusters[i]->getSize());
                clusters[i]->setY(sumY/clusters[i]->getSize());
            }
        }
        /*for(int i = 0; i < k; i++) {
            cout << "(" << clusters[i]->getX() << ", " << clusters[i]->getY() << ")" << endl;
        }*/
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
