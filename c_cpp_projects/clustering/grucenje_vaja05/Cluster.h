//
//  Cluster.h
//  grucenje_vaja05
//
//  Created by Urban Vidovič on 12/06/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#ifndef Cluster_h
#define Cluster_h
#include "Point.h"
#include <vector>

class Point;

class Cluster {
private:
    double x, y;
    std::vector<Point*> content;
    static int counter;
    int index;
    
public:
    Cluster(double x1, double y1): x(x1), y(y1) {
        index = counter;
        counter++;
    }
    ~Cluster() {}
    
    double getX() {return x;}
    void setX(double x1) {x = x1;}
    double getY() {return y;}
    void setY(double y1) {y = y1;}
    int getInd() {return index;}
    
    void clearContent() {content.clear();}
    
    int getSize() {return content.size();}
    
    void addToVector(Point *p) {
        content.push_back(p);
    }
    
    Point* access(int index) {
        return content[index];
    }
    
};

int Cluster::counter = 0;

#endif /* Cluster_h */
