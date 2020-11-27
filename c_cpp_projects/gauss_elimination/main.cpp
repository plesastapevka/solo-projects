#include <iostream>
#include <fstream>
#include <vector>
#include <cmath>
#include <chrono>

enum STATUS {
    SUCCESS=true, FAIL=false
};

template <typename T>
void print_matrix(std::vector<std::vector<T>>& A) {
    for (auto& a: A) {
        for (auto& e: a) {
            std::cout << e << " ";
        }
        std::cout << "\n";
    }
}

template <typename T>
std::vector<T> gauss(std::vector<std::vector<T>>& A, size_t n, STATUS& s) {
    std::vector<T> x(n);
    T val, abs_val;
    size_t index;
    for (size_t k = 0; k < n - 1; ++k) {
        // Find the smallest A[j][k] != 0 (j >= k)
        index = -1;
        val = A[k][k];
        for (size_t j = k; j < n; ++j) {
            abs_val = std::abs(A[j][k]);
            if (abs_val != 0 && std::abs(val) >= abs_val) {
                val = A[j][k];
                index = j;
            }
        }
        if (index == -1) {
            s = FAIL;
            return x;
        } else {
            std::swap(A[index], A[k]);
            // Divide here
            for (auto& e: A[k]) {
                e /= val;
            }
        }
        size_t i;
        for (size_t l = k+1; l < n; ++l) {
            i = 0;
            val = A[l][k];
            for (auto& e: A[l]) {
                e -= A[k][i] * val;
                i++;
            }
        }
    }

    if (A[n-1][n-1] == 0) {
        s = FAIL;
        return x; //ERROR
    }

    T sum;
    x.resize(n);
    x[n-1] = A[n-1][n] / A[n-1][n-1];
    for (int16_t i = n-2; i >= 0; --i) {
        sum = 0.0;
        for (int16_t j = i+1; j < n; ++j) {
            sum += A[i][j] * x[j];
        }
        x[i] = A[i][i] * (A[i][n] - sum);
    }
    s = SUCCESS;
    return x;
}

void test() {
    srand(time(nullptr));
    std::vector<std::vector<double>> A;
    std::vector<double> tmp, res;
    size_t sum;
    STATUS s;
    std::chrono::high_resolution_clock::time_point start;
    std::chrono::high_resolution_clock::time_point end;
    for (size_t i = 3; i <= 20; ++i) {
        sum = 0;
        for (size_t t = 0; t < 20; ++t) {
            A.clear();
            for (size_t j = 0; j < i; ++j) {
                tmp.clear();
                for (size_t k = 0; k < i + 1; ++k) {
                    tmp.push_back(rand() % 100);
                }
                A.push_back(tmp);
            }
            start = std::chrono::high_resolution_clock::now();
            res = gauss(A, i, s);
            end = std::chrono::high_resolution_clock::now();
            sum += std::chrono::duration_cast<std::chrono::microseconds>(end - start).count();
        }
        if (s) {
            std::cout << "Solution for size " << i << " found in " << sum / 20 << " μs\n";
        }
    }
}

void driver(std::string path) {
    std::ifstream ifs(path);
    std::string line;
    size_t n{0}, pos;
    std::vector<std::vector<double>> A;
    std::vector<double> tmp;

    bool first = true;
    while (std::getline(ifs, line)) {
        if (first) {
            n = std::stoi(line);
            first = false;
        } else {
            while ((pos = line.find(" ")) != std::string::npos) {
                tmp.push_back(std::stod(line.substr(0, pos)));
                line.erase(0, pos + 1);
            }
            tmp.push_back(std::stod(line));
            A.push_back(tmp);
            tmp.clear();
        }
    }
    ifs.close();
    STATUS s;
    auto x = gauss(A, n, s);
    if (s) {
        size_t i{1};
        for (auto& e: x) {
            std::cout << "x" << i << ": " << std::setprecision(25) << e << "\n";
            i++;
        }
    } else {
        std::cout << "No solution\n";
    }
}

// za edge.txt primer sta rešitvi glede na tip sledeči:
// float:
//     x1: 0
//     x2: 1
// double:
//     x1: 1
//     x2: 0.9999999999980000442434402

int main(int argc, char** argv) {
    if (!std::strcmp(argv[1], "t")) {
        test();
    } else {
        driver("problem2Gaus.txt");
    }

    return 0;
}
