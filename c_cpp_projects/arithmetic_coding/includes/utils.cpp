//
// Created by Urban Vidovič on 15/11/2020.
//

#include "utils.h"


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

std::vector<size_t> utils::find_bits(const std::string& path, const std::string& bits) {
    std::string bit_string{file_to_bin(path)};
    std::vector<size_t> occurs;

    size_t pos{bit_string.find(bits, 0)};
    while (pos != std::string::npos) {
        occurs.push_back(pos);
        pos = bit_string.find(bits, ++pos);
    }

    return occurs;
}

void utils::find_replace_bits(const std::string& path, const std::string& bits, const std::string& replace) {
    std::string bit_string{file_to_bin(path)};
    boost::replace_all(bit_string, bits, replace);
    size_t remainder = bit_string.size() % 8;
    if (remainder != 0) {
        std::stringstream ss;
        ss << bit_string;
        for (size_t i = 0; i < 8 - remainder; ++i)
            ss << "0";
        bit_string = ss.str();
    }
    std::string tmp;
    size_t sum{0};
    size_t b = 0;
    auto* bytes = new unsigned char(bit_string.length() / 8);
    for (size_t i = 0; i < bit_string.length(); i += 8) {
        tmp = bit_string.substr(i, 8);
        for (char c : tmp) {
            sum <<= 1;
            sum += c - '0';
        }
        bytes[b] = sum;
        sum = 0;
        b++;
    }
    std::stringstream ss;
    ss << "out_" << path;

    std::ofstream ofs(ss.str(), std::ios::out | std::ios::binary);
    ofs.write((char*)bytes, bit_string.length() / 8 * sizeof(char));
    ofs.close();
}