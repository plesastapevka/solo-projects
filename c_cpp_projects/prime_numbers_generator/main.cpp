#include <iostream>
#include <cmath>
#include <random>
#include <chrono>


uint32_t lcg() {
    unsigned seed1 = std::chrono::system_clock::now().time_since_epoch().count();
    std::minstd_rand0 n(seed1);
    uint32_t tmp = n();
    return n();
}

uint32_t random(const uint32_t a, uint32_t b) {
    return (a + lcg()) % (b - a + 1);
}

std::vector<bool> dec_to_bin(const uint32_t n) {
    std::vector<bool> bits;
    uint32_t k;
    for(int32_t i = 31; i >= 0; i--) {
        k = n >> i;
        bits.push_back((k & 1) != 0);
    }

    return bits;
}

uint32_t modular_exponentiation(int32_t a, uint32_t b, int32_t n) {
    uint64_t d{1};
    std::vector<bool> bin = dec_to_bin(b);
    for (int32_t i = 0; i < 32; i++) {
        d = (d * d) % (uint64_t)n;
        if (bin[i]) {
            d = (d * a) % n;
        }
    }

    return d;
}

uint32_t brute_force(const uint32_t num_of_bits) {
    uint32_t j{3};
    double r;
    uint32_t p{rand() % (uint32_t)(std::pow(2, num_of_bits) - 1)};

    if (p % 2 == 0) p++;

    while (true) {
        j = 3;
        while (p % j != 0 && j <= std::sqrt(p)) {
            j += 2;
        }
        if (j > std::sqrt(p)) {
            return p;
        }

        p += 2;
    }
}

bool miller_rabin(const uint32_t p, const uint32_t s) {
    if (p <= 3) return true;

    uint32_t d{p-1};
    uint32_t k{0};
    uint32_t a;
    uint32_t x;

    while (d % 2 == 0) {
        d = d/2;
        k++;
    }

    for (size_t j = 1; j < s; ++j) {
        a = random(2, p-2);
        x = modular_exponentiation(a, d, p);

        if (x != 1) {
            for (size_t i = 0; i < k - 1; ++i) {
                if (x == p - 1) break;
                x = modular_exponentiation(x, 2, p);
            }
            if (x != p - 1) return false;
        }
    }
    return true;
}

void test() {
    size_t bits[] = {4, 8, 16, 32};
    size_t count{0};
    uint32_t n;
    std::vector<size_t> avg_times;
    size_t sum{0};
    size_t tests{20};
    // Run tests for brute force
    for (auto b: bits) {
        for(size_t i = 0; i < tests; ++i) { // test loop
            count = 0;
            while (count < 1) {
                auto start = std::chrono::system_clock::now();
                n = brute_force(b);
                auto end = std::chrono::system_clock::now();
                if (miller_rabin(n, 10)) {
                    count++;
                    sum += std::chrono::duration_cast<std::chrono::microseconds>(end - start).count();
                }
            }
        }
        std::cout << "BF - average time elapsed for " << b << " bits: " << sum/tests << " µs" << std::endl;
        sum = 0;
    }
    std::cout << "---------------------------------------------------------------" << std::endl;

    sum = 0;
    bool is_prime;
    // Tests for Miller-Rabin method
    for (auto b: bits) {
        for (size_t i = 1; i < 21; ++i) {
            for (size_t j = 0; j < tests; ++j) { // test loop
                count = 0;
                n = rand() % (uint32_t) (std::pow(2, b) - 1);
                if (n % 2 == 0) n++;
                auto start = std::chrono::system_clock::now();
                while (count < 1) {
                    is_prime = miller_rabin(n, i);
                    if (is_prime) {
                        auto end = std::chrono::system_clock::now();
                        sum += std::chrono::duration_cast<std::chrono::microseconds>(end - start).count();
                        count++;
                    }
                    n += 2;
                }
            }
            std::cout << "Miller-Rabin - time elapsed for " << b << " bits with s=" << i <<  ": " << sum/tests << " µs" << std::endl;
            sum = 0;
        }
    }
}

int main(int argc, char* argv[]) {
    if (argc < 2 && argc > 0) return 0;
    if (std::strncmp(argv[1], "-t", 2) == 0) {
        test();
        return 0;
    }

    srand((unsigned) time(NULL));

    size_t num_of_bits;
    std::cout << "Number of bits: " << std::endl;
    std::cin >> num_of_bits;

    if (num_of_bits > 32 || num_of_bits <= 0) {
        std::cout << "Invalid number." << std::endl;
    }

    size_t mode;
    std::cout << "Choose mode:\n1: Brute force\n2: Miller-Rabin" << std::endl;
    std::cin >> mode;

    size_t s;
    std::cout << "Enter s:" << std::endl;
    std::cin >> s;

    uint32_t n;
    size_t count{0};

    switch(mode) {
        case 1:
            while(count < 1) {
                n = brute_force(num_of_bits);
                if (miller_rabin(n, s)) {
                    count++;
                    std::cout << n << std::endl;
                }
            }
            break;
        case 2:
            n = rand() % (uint32_t)(std::pow(2, num_of_bits) - 1);
            if (n % 2 == 0) n++;
            while (count < 1) {
                if (miller_rabin(n, s)) {
                    std::cout << n << std::endl;
                    count++;
                }
                n += 2;
            }
            break;
        default:
            break;
    }

    return 0;
}
