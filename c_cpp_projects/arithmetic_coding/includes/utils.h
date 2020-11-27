//
// Created by Urban Vidovič on 15/11/2020.
//

#ifndef VAJA01_UTILS_H
#define VAJA01_UTILS_H

#include <iostream>
#include <fstream>
#include "boost/algorithm/string.hpp"

namespace utils{
    std::string file_to_bin(const std::string& path);
    std::vector<size_t> find_bits(const std::string& path, const std::string& bits);
    void find_replace_bits(const std::string& path, const std::string& bits, const std::string& replace);
}

#endif //VAJA01_UTILS_H
