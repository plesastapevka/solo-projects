//
//  Point.h
//  grucenje_vaja05
//
//  Created by Urban Vidovič on 11/06/2018.
//  Copyright © 2018 Urban Vidovič. All rights reserved.
//

#ifndef Point_h
#define Point_h
#include "Cluster.h"


class Point {
private:
    double x, y;
    double value;
    int index;
    static int indCounter;
    
public:
    Point(double x1, double y1): x(x1), y(y1) {
        index = indCounter;
        indCounter++;
    }
    ~Point() {}
    
    void setX(double x1) {x = x1;}
    void setY(double y1) {y = y1;}
    double getX() {return x;}
    double getY() {return y;}
    int getIndex() {return index;}
    
    //p1 = (px, py)
    //p2 = (qx, qy)
    //distance = sqrt(pow((qx - px),2) + pow((qy - py),2))
    static double calculateEDistance(Point *p1, Cluster *p2) {
        double euclDis =
        sqrt(pow((p2->getX() - p1->getX()),2) + pow((p2->getY() - p1->getY()),2));
        return euclDis;
    }
};

int Point::indCounter = 0;

#endif /* Point_h */
