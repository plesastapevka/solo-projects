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
#include <omp.h>
#include <limits>
#include "definicijaProblema.cpp"

#define NpSize 100
#define DIMENSIONS 10
#define FUNC_NUM 8
#define Bmin -100
#define Bmax 100
#define F 0.5
#define Cr 0.9

/*
 *Np.....population size
 *D......dimensions
 *Bmin...minimal num
 *Bmax...maximal num
 *[-8192, 8192]
 */

void init(std::vector<std::vector<double>> &P, int D, std::mt19937 &gen, std::vector<double> &p_values, double &bestX) {
    std::uniform_real_distribution<> dis(0.0, 1.0);
    std::vector<double> tmp;
    P.clear();
    P.resize(NpSize);
    p_values.resize(NpSize);
    for(auto& p: P) {
        p.resize(D);
    }
    int i = 0;
    for(auto& p: P) {
        for (auto &element: p) {
            element = Bmin + (Bmax - Bmin) * dis(gen);
        }
        double f;
#pragma omp critical
        {
            cec19_test_func(&p[0], &f, D, 1, 8);
        }

        if (f < bestX) {
#pragma omp critical
            {
                bestX = f;
            }
        }
        p_values[i] = f;
        i++;
    }
}

void mutation(std::vector<std::vector<double>> &P, std::vector<double> &M, int i, int D, std::mt19937 &gen) {
    std::uniform_int_distribution<> dis(0, NpSize - 1);
    int r1 = dis(gen);
    int r2 = dis(gen);
    int r3 = dis(gen);
    M.clear();
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
}

void crossing(std::vector<double> &Xi, std::vector<double> &M, std::vector<double> &K, int D, std::mt19937 &gen) {
    std::uniform_int_distribution<> dis(0, D - 1);
    std::uniform_real_distribution<> dis1(0, 1);
    int randj = dis(gen);
    double tmpRand;
    K.clear();
    for (int j = 0; j < D; j++) {
        tmpRand = dis1(gen);
        if (tmpRand < Cr || randj == j) {
            K.push_back(M[j]);
        } else {
            K.push_back(Xi[j]);
        }
    }
}

void fixing(std::vector<double> &K, std::vector<double> &X, int D) {
    X.clear();
    for (int j = 0; j < D; j++) {
        if (K[j] < Bmin) {
            X.push_back(Bmin + (Bmin - K[j]));
        } else if (K[j] > Bmax) {
            X.push_back(Bmax - (K[j] - Bmax));
        } else {
            X.push_back(K[j]);
        }
    }
}

void selection(std::vector<double> &Pi, std::vector<double> &Xnew, double &p_value, std::vector<double> &values, double &bestX, int &thread,
               void (*f)(double *, double *, int, int, int)) {
    double fOdX;
#pragma omp critical
    {
        f(&Xnew[0], &fOdX, DIMENSIONS, 1, FUNC_NUM);
    }
    if (fOdX < p_value && fOdX > 0) {
        Pi = Xnew;
//        values.push_back(fOdX);
        if (fOdX < bestX) {
#pragma omp critical
            {
                std::cout << "Thread " << omp_get_thread_num() << " has found new best value with f(x) = " << fOdX << std::endl;
                bestX = fOdX;
                thread = omp_get_thread_num();
            }
        }
    } //else {
//        values.push_back(p_value);
//    }
    p_value = fOdX;
}

void diff_evo(std::vector<double> &values, int D, int MAX_NFES1, std::vector<std::vector<double>> &P, double &bestX, int &NFES, int &thread) {
    std::random_device rd;  //Will be used to obtain a seed for the random number engine
    std::mt19937 gen(rd()); //Standard mersenne_twister_engine seeded with rd()
    std::uniform_real_distribution<> dis(1.0, 2.0);
    int MAX_NFES = MAX_NFES1;

    std::vector<double> M;
    std::vector<double> K;
    std::vector<double> X;
    std::vector<double> Xnew;
    std::vector<double> p_values;

    init(P, DIMENSIONS, gen, p_values, bestX);
    while (NFES < MAX_NFES) {
        for (int i = 0; i < NpSize; i++) {
            mutation(P, M, i, D, gen);
            crossing(P[i], M, K, D, gen);
            fixing(K, Xnew, D);
            selection(P[i], Xnew, p_values[i], values, bestX, thread, cec19_test_func);

            NFES++;
//#pragma omp critical
//            {
//                std::cout << "Iteration number " << i << " on thread: " << omp_get_thread_num()
//                          << " with current best value: " << bestX << " | NFES = " << NFES << std::endl;
//            }
//            if (bestX - 1 < 0.0000000001) {
//                return;
//            }
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

int main(int argc, const char *argv[]) {
    int MAX_NFES;
    int thread;
    double sum = 0;
    double bestX = std::numeric_limits<double>::max();

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
    std::vector<std::vector<double>> P;
    int NFES = 0;
    int threads = 4;
    omp_set_num_threads(threads);
    auto general_start = std::chrono::system_clock::now();
#pragma omp parallel for default(none) shared(times, values, MAX_NFES, bestX, std::cout, thread) private(P) firstprivate(NFES, sum) schedule(dynamic)
        for (int i = 0; i < 25; i++) {
//        start measuring time
            auto start = std::chrono::system_clock::now();
            NFES = 0;

            diff_evo(values, DIMENSIONS, MAX_NFES, P, bestX, NFES, thread);
//        end measuring time
            auto end = std::chrono::system_clock::now();
#pragma omp atomic
            sum += std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
//            auto finalTime = std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();

#pragma omp critical
            {
                std::cout << "Time elapsed on thread " << omp_get_thread_num() << ": " << sum << " | current best X = " << bestX << std::endl;
//                times.push_back(finalTime);
            }
        }
    auto general_end = std::chrono::system_clock::now();
    auto general_final_time = std::chrono::duration_cast<std::chrono::milliseconds>(general_end - general_start).count();
    std::cout << "--------------------------------------------------------------------------------------------------------------" << std::endl;
    std::cout << "Program run on " << threads << " threads finished in: " << general_final_time << " milliseconds" <<  std::endl;
    std::cout << "For MAX_NFES: " << MAX_NFES << std::endl;
    std::cout << "Thread " << thread << " found the best value of: " << bestX << std::endl;
//    calculateTime(times);
//    calculateValues(values);

    return 0;
}
