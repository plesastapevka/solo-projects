//
// Created by Urban Vidovič on 02/11/2020.
//

#include "utils.h"

#define PARAM_S 20
#define MAX_BITS 2048

typedef boost::multiprecision::uint1024_t uint1024_t;
typedef boost::multiprecision::int1024_t int1024_t;
namespace mp = boost::multiprecision;

std::vector<bool> utils::dec_to_bin(const mp::cpp_int& n) {
    std::vector<bool> bits;
    mp::cpp_int k;
    for(int16_t i = MAX_BITS - 1; i >= 0; i--) {
        k = n >> i;
        bits.push_back((k & 1) != 0);
    }

    return bits;
}

mp::cpp_int utils::mod(const mp::cpp_int& a, const mp::cpp_int& b) {
    mp::cpp_int res = a % b;
    return res >= 0 ? res : res + b;
}

void utils::modular_exponentiation(const mp::cpp_int& a, mp::cpp_int b, mp::cpp_int n, mp::cpp_int& value) {
    mp::cpp_int d{1};
    std::vector<bool> bin = dec_to_bin(b);

    for (int32_t i = 0; i < MAX_BITS; ++i) {
        d = mod((d * d), n);
        if (bin[i]) {
            d = mod((d * a), n);
        }
    }

    value = d;
}

uint1024_t utils::lcg() {
    unsigned seed1 = std::chrono::system_clock::now().time_since_epoch().count();
    std::minstd_rand0 n(seed1);
    return n();
}

uint1024_t utils::random(const uint1024_t a, uint1024_t b) {
    return (a + lcg()) % (b - a + 1);
}

std::string utils::file_to_bin(const std::string& path) {
    std::ifstream ifs(path, std::ios::in | std::ios::binary);

    if (!ifs.is_open()) {
        std::cerr << "Error opening the file.\n";
    }
    std::stringstream ss;
    char c;
    while (ifs.get(c)) {
        for (int i = 7; i >= 0; --i)
            ss << ((c >> i) & 1);
    }
    ifs.close();
    return ss.str();
}

bool utils::miller_rabin(const uint1024_t p, const uint1024_t s) {
    if (p <= 3) return true;

    uint1024_t d{p-1};
    uint1024_t k{0};
    uint1024_t a;
    mp::cpp_int x;

    while (d % 2 == 0) {
        d = d/2;
        k++;
    }

    for (size_t j = 1; j < s; ++j) {
        a = random(2, p-2);
        modular_exponentiation(int1024_t(a), int1024_t(d), int1024_t(p), x);

        if (x != 1) {
            for (size_t i = 0; i < k - 1; ++i) {
                if (x == p - 1) break;
                modular_exponentiation(x, int1024_t(2), int1024_t(p), x);
            }
            if (x != p - 1) return false;
        }
    }
    return true;
}

uint1024_t utils::generate_random(size_t num_of_bits, boost::mt19937& gen) {
    uint1024_t max = boost::lexical_cast<uint1024_t>(mp::pow(mp::cpp_int(2), num_of_bits) - 1);
    boost::random::uniform_int_distribution<uint1024_t> ui(1, max);

    size_t s{PARAM_S};
    uint1024_t n = boost::lexical_cast<uint1024_t>(ui(gen));
    if (n % 2 == 0) n++;
    while (true) {
        if (miller_rabin(n, s)) {
            break;
        } else {
            n += 2;
            if (n > mp::pow(mp::cpp_int(2), num_of_bits) - 1) {
                n = boost::lexical_cast<uint1024_t>(ui(gen));
            }
        }
    }
    return n;
}

mp::cpp_int utils::gcd(mp::cpp_int a, mp::cpp_int b) {
    for (;;) {
        if (a == 0) return b;
        b %= a;
        if (b == 0) return a;
        a %= b;
    }
}