#include <iostream>
#include <exception>
#include <fstream>
#include <bitset>
#include <boost/program_options.hpp>
#include <boost/algorithm/string.hpp>

#define BUFFER

std::string file_to_bin(const std::string& path) {
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

std::vector<size_t> find_bits(const std::string& path, const std::string& bits) {
    std::string bit_string{file_to_bin(path)};
    std::vector<size_t> occurs;

    size_t pos{bit_string.find(bits, 0)};
    while (pos != std::string::npos) {
        occurs.push_back(pos);
        pos = bit_string.find(bits, ++pos);
    }

    return occurs;
}

void find_replace_bits(const std::string& path, const std::string& bits, const std::string& replace) {
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

int main(int argc, char* argv[]) {
    if (argc == 1) {
        std::cerr << "No arguments passed.\n";
        return 1;
    }
    try {

        boost::program_options::options_description desc("Allowed options");
        desc.add_options()
                ("help,h", "Produce help message")
                ("input,i", boost::program_options::value<std::string>(), "Input file")
                ("operation,o", boost::program_options::value<std::string>(), "Set operation")
                ("find,s", boost::program_options::value<std::string>(), "Find bit sequence")
                ("replace,r", boost::program_options::value<std::string>(), "Replace with bit sequence");

        boost::program_options::variables_map vm;
        boost::program_options::store(boost::program_options::parse_command_line(argc, argv, desc), vm);
        boost::program_options::notify(vm);

        if (vm.count("help")) {
            std::cout << desc << "\n";
            return 0;
        }
        // test data 000000000000000011111111 and 111111110000000000000000
        if (vm.count("operation") && vm.count("input") && vm.count("find")) {
            std::string op{vm["operation"].as<std::string>()};
            std::cout << "Recognized operation: "
                << vm["operation"].as<std::string>() << "\n"
                << "File path: " << vm["input"].as<std::string>() << ".\n";

            if (op == "f") {
                std::vector<size_t> positions{find_bits(vm["input"].as<std::string>(), vm["find"].as<std::string>())};
                for (auto pos: positions) std::cout << pos << " ";
            } else if (op == "fr") {
                find_replace_bits(vm["input"].as<std::string>(), vm["find"].as<std::string>(), vm["replace"].as<std::string>());
            } else {
                throw "Operation not recognized.\n";
            }
        } else {
            std::cout << "Invalid or missing parameter.\n";
        }
    }
    catch (std::exception& e) {
        std::cerr << "error: " << e.what() << "\n";
        return 1;
    }
    catch (const char* msg) {
        std::cerr << msg;
    }
    catch (...) {
        std::cerr << "Exception of unknown type!\n";
        return 1;
    }
    return 0;
}
