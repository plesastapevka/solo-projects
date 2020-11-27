//
//  main.cpp
//  vaja01_diferencialna_evolucija
//
//  Created by Urban Vidovič on 10/10/2019.
//  Copyright © 2019 Urban Vidovič. All rights reserved.
//

#include <iostream>
#include <cmath>
#include <chrono>
#include <random>
#include <algorithm>
#include <vector>
#include <thread>
//#include <omp.h>
#include "definicijaProblema.cpp"

#define NpSize 100
#define DIMENSIONS 9
#define FUNC_NUM 1
#define Bmin -8192
#define Bmax 8192
#define F 0.5
#define Cr 0.9

/*
 *Np.....population size
 *D......dimensions
 *Bmin...minimal num
 *Bmax...maximal num
 *[-8192, 8192]
 */

std::vector<std::vector<double>> P;
std::random_device rd;  //Will be used to obtain a seed for the random number engine
std::mt19937 gen(rd()); //Standard mersenne_twister_engine seeded with rd()

int MAX_NFES, NFES;

double fRand(double fMin, double fMax) {
    double f = (double) rand() / RAND_MAX;
    return fMin + f * (fMax - fMin);
}

void init(int D, // dimensions
//          int Bmin, // minimum
//          int Bmax, // maximum
          int MAX_NFES) {

    std::uniform_real_distribution<> dis(0.0, 1.0);
    std::vector<double> tmp;
    P.clear();
    for (int i = 0; i < NpSize; i++) {
        tmp.clear();
        for (int j = 0; j < D; j++) {
            tmp.push_back(Bmin + (Bmax - Bmin) * dis(gen));
        }
        P.push_back(tmp);
    }
}

std::vector<double> mutation(int i, int D) {
    std::uniform_int_distribution<> dis(0, NpSize - 1);
    int r1 = dis(gen);
    int r2 = dis(gen);
    int r3 = dis(gen);
    std::vector<double> M;

    while (r1 == i) {
        r1 = dis(gen);
    }
    while (r2 == i || r2 == r1) {
        r2 = dis(gen);
    }
    while (r3 == i || r3 == r2 || r3 == r1) {
        r3 = dis(gen);
    }
    for (int j = 0; j < D; j++) {
        M.push_back(P[r1][j] + F * ((P[r2][j] - P[r3][j])));
    }

    return M;
}

std::vector<double> crossing(std::vector<double> Xi, std::vector<double> M, int D) {
    std::uniform_int_distribution<> dis(0, D - 1);
    std::uniform_real_distribution<> dis1(0, 1);
    int randj = dis(gen);
    int tmpRand;
    std::vector<double> K;
    for (int j = 0; j < D; j++) {
        tmpRand = dis1(gen);
        if (tmpRand < Cr || randj == j) {
            K.push_back(M[j]);
        } else {
            K.push_back(Xi[j]);
        }
    }
    return K;
}

std::vector<double> fixing(std::vector<double> K, int D) {
    std::vector<double> X;
    for (int j = 0; j < D; j++) {
        if (K[j] < Bmin) {
            X.push_back(Bmin + (Bmin - K[j]));
        } else if (K[j] > Bmax) {
            X.push_back(Bmax - (K[j] - Bmax));
        } else {
            X.push_back(K[j]);
        }
    }
    return X;
}

std::vector<double> selection(std::vector<double> &X, std::vector<double> &Xnew, int D, int i, std::vector<double> &values, double &bestX,
          void (*f)(double *, double *, int, int, int)) {
    double fOdX;
    double fOdX1;

    f(&Xnew[0], &fOdX, (int) D, FUNC_NUM, FUNC_NUM);
    f(&X[0], &fOdX1, (int) D, FUNC_NUM, FUNC_NUM);

    if (fOdX < fOdX1) {
        X = Xnew;
        values.push_back(fOdX);
    } else {
        values.push_back(fOdX1);
    }

    if (fOdX < bestX) {
        bestX = fOdX;
    }
    NFES+=2;
    return X;
}

void calculateValues(std::vector<double> values) {
    double tmpMin = *std::min_element(values.begin(), values.end());
    double tmpMax = *std::max_element(values.begin(), values.end());

    double avg = accumulate(values.begin(), values.end(), 0.0) / values.size();

    std::vector<double> tmpValues;
    for (double value: values) {
        tmpValues.push_back(abs(value - avg));
    }
    double avgValue = accumulate(tmpValues.begin(), tmpValues.end(), 0.0) / tmpValues.size();
    double std_dev = sqrt(avgValue);

    std::cout << "\tFUNKCIJSKE VREDNOSTI:" << std::endl;
    std::cout << "\t\t-MIN: " << tmpMin << std::endl;
    std::cout << "\t\t-MAX: " << tmpMax << std::endl;
    std::cout << "\t\t-AVG: " << avg << std::endl;
    std::cout << "\t\t-STD_DEV: " << std_dev << std::endl;
}

void diff_evo(std::vector<double> &values, int D, int MAX_NFES1) {
    std::random_device rd;  //Will be used to obtain a seed for the random number engine
    std::mt19937 gen(rd()); //Standard mersenne_twister_engine seeded with rd()
    std::uniform_real_distribution<> dis(1.0, 2.0);
    NFES = 0;
    MAX_NFES = MAX_NFES1;
//    Bmin = Bmin1;
//    Bmax = Bmax1;

//    std::vector<double> RES;
//    RES.resize(100);
    double bestX = 1000000000000000;
    std::vector<double> M;
    std::vector<double> K;
    std::vector<double> Xnew;

    Xnew.resize(D);
    init(D, MAX_NFES);
    while (NFES < MAX_NFES) {
        for (int i = 0; i < NpSize; i++) {
            M = mutation(i, D);
            K = crossing(P[i], M, D);
            Xnew = fixing(K, D);
            P[i] = selection(P[i], Xnew, D, i, values, bestX, cec19_test_func);
            if (bestX - 1 < 0.0000000001) {
                return;
            }

        }
    }
}


void calculateTime(std::vector<float> times) {
    double tmpMin = *std::min_element(times.begin(), times.end());
    double tmpMax = *std::max_element(times.begin(), times.end());
    double avg = accumulate(times.begin(), times.end(), 0.0) / times.size();

    std::vector<double> tmpTimes;
    for (double time: times) {
        tmpTimes.push_back(abs(time - avg));
    }
    double avgTimes = accumulate(tmpTimes.begin(), tmpTimes.end(), 0.0) / tmpTimes.size();
    double std_dev = sqrt(avgTimes);

    std::cout << "\tČAS IZVAJANJA:" << std::endl;
    std::cout << "\t\t-MIN: " << tmpMin << std::endl;
    std::cout << "\t\t-MAX: " << tmpMax << std::endl;
    std::cout << "\t\t-AVG: " << avg << std::endl;
    std::cout << "\t\t-STD_DEV: " << std_dev << std::endl;
}

void printsmth(int id) {
    std::cout << "TO JE THREAD " << std::this_thread::get_id() << std::endl;
}

int main(int argc, const char *argv[]) {
    
    if (argc > 3) {
        std::cout << "Too many arguments" << std::endl;
        return 0;
    }

    try {
        std::string arg = argv[1];
        if (arg != "-MAX_NFES") { throw; }

        MAX_NFES = atoi(argv[2]);

    } catch (...) {
        std::cout << "Usage: ./main -MAX_NFES <num>" << std::endl;
        return 0;
    }

    std::vector<float> times;
    std::vector<double> values;
    auto start = std::chrono::system_clock::now();
    auto end = std::chrono::system_clock::now();

    for (int i = 0; i < 25; i++) {
//        start measuring time
        start = std::chrono::system_clock::now();

        diff_evo(values, DIMENSIONS, MAX_NFES);
//        end measuring time
        end = std::chrono::system_clock::now();
        auto finalTime = std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
        times.push_back(finalTime);
    }
    calculateTime(times);
    calculateValues(values);

    return 0;
}
