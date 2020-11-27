//
//  main.cpp
//  vaja03_cpp_threads
//
//  Created by Urban Vidovič on 08/11/2019.
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


//std::random_device rd;  //Will be used to obtain a seed for the random number engine
//std::mt19937 gen(rd()); //Standard mersenne_twister_engine seeded with rd()

int MAX_NFES;
std::mutex critical;

//double fRand(double fMin, double fMax) {
//    double f = (double) rand() / RAND_MAX;
//    return fMin + f * (fMax - fMin);
//}

void init(std::vector<std::vector<double>> &P, int D, std::mt19937 &gen, std::vector<double> &p_values, double &bestX) {
    std::uniform_real_distribution<> dis(0.0, 1.0);
    std::vector<double> tmp;
//    std::mutex critical;
    P.clear();
    P.resize(NpSize);
    p_values.resize(NpSize);
    for(auto& p: P) {
        p.resize(D);
    }
    int i = 0;
    for(auto& p: P) {
        for(auto& element: p) {
            element = Bmin + (Bmax - Bmin) * dis(gen);
        }
        double f;
        critical.lock();
        cec19_test_func(&p[0], &f, D, 1, 8);
        critical.unlock();

        if (f < bestX) {
            critical.lock();
            bestX = f;
            critical.unlock();
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

void verboseData(int iteration, int threadNum, std::chrono::system_clock::time_point start, int NFES, double bestX, double bestGlobalX) {
    auto end = std::chrono::system_clock::now();
    std::cout << "Current thread: " << threadNum << std::endl;
    std::cout << "Thread ID: " << std::this_thread::get_id() << std::endl;
    std::cout << "Run: " << iteration << std::endl;
    std::cout << "Runtime: " << std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count() << "ms" << std::endl;
    std::cout << "NFES: " << NFES << std::endl;
    std::cout << "Current best: " << bestX << std::endl;
    std::cout << "Current program-wide best: " << bestGlobalX << std::endl;
    std::cout << std::endl;
}

void selection(bool verbose,
               std::vector<double> &Pi,
               std::vector<double> &Xnew,
               double &p_value,
               std::vector<double> &values,
               double &bestX,
               void (*f)(double *, double *, int, int, int),
               int iteration,
               int threadNum,
               std::chrono::system_clock::time_point &start,
               int NFES,
               double &bestGlobalX,
               std::vector<double> &bestGlobal) {
    double fOdX;

    critical.lock();
    cec19_test_func(&Xnew[0], &fOdX, DIMENSIONS, 1, FUNC_NUM);
    critical.unlock();

    if (fOdX < p_value) {
        Pi = Xnew;
        bestGlobal = Xnew;
        //        values.push_back(fOdX);
        if(fOdX < bestX) {
            critical.lock();
            bestX = fOdX;
            if(verbose) verboseData(iteration, threadNum, start, NFES, bestX, bestGlobalX);
            critical.unlock();
        }
    } //else {
//        values.push_back(p_value);
//    }
    p_value = fOdX;
}

void diff_evo(bool verbose,
              std::vector<double> &values,
              int D,
              int MAX_NFES1,
              double &bestGlobalX,
              std::vector<double> &bestGlobal,
              int threadNum,
              int iteration,
              std::chrono::system_clock::time_point &start,
              int seed) {
//    std::random_device rd;  //Will be used to obtain a seed for the random number engine
    std::mt19937 gen(seed); //Standard mersenne_twister_engine seeded with rd()
//    std::uniform_real_distribution<> dis(1.0, 2.0);
    int MAX_NFES = MAX_NFES1;
    std::atomic<int> NFES(0);
    double bestX = INT_MAX;
    std::vector<double> M;
    std::vector<double> K;
    std::vector<double> X;
    std::vector<double> Xnew;
    std::vector<double> p_values;
    std::vector<std::vector<double>> P;
//    std::mutex crit;

    init(P, DIMENSIONS, gen, p_values, bestX);
    while (NFES < MAX_NFES) {
        for (int i = 0; i < NpSize; i++) {
            mutation(P, M, i, D, gen);
            crossing(P[i], M, K, D, gen);
            fixing(K, Xnew, D);
            selection(verbose, P[i], Xnew, p_values[i], values, bestX, cec19_test_func, iteration, threadNum, start, NFES, bestGlobalX, bestGlobal);
            if(bestX < bestGlobalX) {
                critical.lock();
                bestGlobalX = bestX;
                critical.unlock();
            }
            NFES++;
        }
    }
}

void dataPrint(int threads, int runs, int NFES, double speed, double bestX, std::vector<double> solution) {
    time_t now = time(0);
    std::string dt = ctime(&now);
    std::cout << "Program finished computing DE with " << threads << " thread/s" << std::endl;
    std::cout << "Date: " << dt;
    std::cout << "Runs: " << runs << std::endl;
    std::cout << "NFES: " << NFES << std::endl;
    std::cout << "Elapsed time: " << speed << "ms" << std::endl;
    std::cout << "Best f(x): " << bestX << std::endl;
    std::cout << "Solution:\n";
    for(int i = 0; i < solution.size(); i++)
        std::cout << "[" << i << "] " << solution[i] << "\n";
}


int main(int argc, const char *argv[]) {
    //parameters parsing
    int runsLmt = 1;
    int seed = 41;
    int threadsLmt = std::thread::hardware_concurrency();
    bool verbose = false;
    MAX_NFES = INT_MAX;
    std::string flag;
    int i = 1;
    while(i < argc) {
        flag = (std::string)argv[i];
        try {
            if(flag != "-nfes" &&
               flag != "-verbose" &&
               flag != "-thread" &&
               flag != "-seed" &&
               flag != "-runs") {
                throw (std::string)flag; }

            if(flag == "-nfes") {
                MAX_NFES = atoi(argv[i+1]);
                i += 2;
            } else if(flag == "-verbose") {
                verbose = !verbose;
                i++;
            } else if(flag == "-runs") {
                runsLmt = atoi(argv[i+1]);
                i += 2;
            } else if(flag == "-thread") {
                threadsLmt = atoi(argv[i+1]);
                if(threadsLmt > (int)std::thread::hardware_concurrency()) throw 1;
                else if(threadsLmt <= 0) throw 2;
                i += 2;
            } else if(flag == "-seed") {
                seed = atoi(argv[i+1]);
                i += 2;
            }
        } catch(std::string e) {
            std::cout << e << " is not a valid flag." << std::endl;
            std::cout << "Usage: ./vaja03_cpp_threads [-nfes <number>] [-runs <number>] [-verbose] [-thread <number>] [-seed <number>]" << std::endl;
            std::cout << "All parameters are optional." << std::endl;
            return 1;
        } catch(int err) {
            switch(err) {
                case 1:
                    std::cout << threadsLmt << " exceedes number of available concurrent threads: " << std::thread::hardware_concurrency() << std::endl;
                    break;
                case 2:
                    std::cout << "You must use at least 1 thread." << std::endl;
                    break;
            }
            return err;
        }
    }

    //condition description
    std::cout << "Now running DE with parameters:" << std::endl;
    std::cout << "MAX_NFES: " << MAX_NFES << std::endl;
    std::cout << "VERBOSE: " << verbose << std::endl;
    std::cout << "THREADS: " << threadsLmt << std::endl;
    std::cout << "RUNS: " << runsLmt << std::endl;
    std::cout << "SEED: " << seed << std::endl;
    std::cout << std::endl;

    std::vector<double> bestGlobal; //best vector of size D
    double bestGlobalX = INT_MAX; //best f(x) value
    {
        std::vector<std::thread> threads(threadsLmt); //filling vector with threads
        auto startGeneral = std::chrono::system_clock::now();
        for(int t = 0; t < threadsLmt; t++) {
            threads[t] = std::thread(std::bind([&](const int bi, const int ei, const int t) {
                std::vector<double> threadValues;
                for(int i = bi; i < ei; i++) {
                    auto start = std::chrono::system_clock::now();
                    diff_evo(verbose, threadValues, DIMENSIONS, MAX_NFES, bestGlobalX, bestGlobal, t, i, start, seed);
                }
            },t * runsLmt / threadsLmt, (t+1) == threadsLmt ? runsLmt : (t+1) * runsLmt / threadsLmt, t));
        }
        std::for_each(threads.begin(), threads.end(), [](std::thread& x) {x.join();});
        auto endGeneral = std::chrono::system_clock::now();
        auto finalGeneralTime = std::chrono::duration_cast<std::chrono::milliseconds>(endGeneral - startGeneral).count();

        dataPrint(threadsLmt, runsLmt, MAX_NFES, finalGeneralTime, bestGlobalX, bestGlobal);
        std::cout << std::endl;
    }

    return 0;
}
