//
// Created by Urban Vidovič on 02/11/2020.
//

#ifndef VAJA02_RSA_UTILS_H
#define VAJA02_RSA_UTILS_H

#include <iostream>
#include <random>
#include <fstream>
#include "boost/multiprecision/cpp_int.hpp"
#include "boost/random.hpp"
#include "boost/random/random_device.hpp"

namespace mp = boost::multiprecision;
typedef mp::uint1024_t uint1024_t;

namespace utils {
    mp::uint1024_t lcg();
    mp::uint1024_t random(mp::uint1024_t a, mp::uint1024_t b);
    mp::uint1024_t generate_random(size_t num_of_bits, boost::mt19937& gen);
    mp::cpp_int gcd(mp::cpp_int a, mp::cpp_int b);
    mp::cpp_int mod(const mp::cpp_int& a, const mp::cpp_int& b);

    std::vector<bool> dec_to_bin(const mp::cpp_int& n);

    std::string file_to_bin(const std::string& path);

    void modular_exponentiation(const mp::cpp_int& a, mp::cpp_int b, mp::cpp_int n, mp::cpp_int& value);

    bool miller_rabin(mp::uint1024_t p, mp::uint1024_t s);

}

#endif //VAJA02_RSA_UTILS_H
