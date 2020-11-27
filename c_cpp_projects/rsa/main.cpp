#include <iostream>
#include <string>
#include "utils.h"
#include "boost/multiprecision/cpp_int.hpp"
#include "boost/chrono.hpp"
#include "boost/program_options.hpp"
#include "boost/dynamic_bitset.hpp"

#define PUB_PATH "pub.txt"
#define PRIV_PATH "priv.txt"

typedef boost::multiprecision::uint1024_t uint1024_t;
typedef boost::multiprecision::int1024_t int1024_t;
namespace mp = boost::multiprecision;

mp::cpp_int euler(const uint1024_t p, const uint1024_t q) {
    return (p-1) * (q-1);
}

void extended_euclid(
        mp::cpp_int a,
        mp::cpp_int b,
        mp::cpp_int& d,
        mp::cpp_int& x,
        mp::cpp_int& y) {

    if (b == 0) {
        d = a;
        x = 1;
        y = 0;
    } else {
        mp::cpp_int n_d, n_x, n_y;
        if (a < 0) {
            extended_euclid(b, a + b, n_d, n_x, n_y);
        } else {
            extended_euclid(b, utils::mod(a, b), n_d, n_x, n_y);
        }
        d = n_d;
        x = n_y;
        y = n_x - (a/b) * n_y;
    }
}

mp::cpp_int modular_linear_eq(mp::cpp_int a, mp::cpp_int b, mp::cpp_int n) {
    mp::cpp_int d, x, y;
    extended_euclid(a, n, d, x, y);

    if (d % b == 0) {
        return (mp::cpp_int)utils::mod(x * (b / d), n);
    } else {
        std::cout << "No result"  << "\n";
        return 0;
    }
}

void generate_pair(size_t num_of_bits, std::string p_path, std::string s_path) {
    // 1st step
    srand(time(nullptr));
    boost::random::mt19937 gen;
    uint1024_t p{utils::generate_random(num_of_bits, gen)};
    uint1024_t q{utils::generate_random(num_of_bits, gen)};
    while (p == q || p == 1 || q == 1) {
        q = utils::generate_random(num_of_bits, gen);
    }
    // 2nd step
    mp::cpp_int n{(mp::cpp_int)p * (mp::cpp_int)q};
    mp::cpp_int e_n{euler(p, q)};

    // 3rd step
    uint1024_t e{utils::generate_random(num_of_bits, gen)};
    if (e % 2 == 0) e++;
    while (e < 1 && e > e_n && utils::gcd(e, e_n) != 1) {
        e -= 2;
        if (e <= 1) e = utils::generate_random(num_of_bits, gen);
    }

    // 4th step
    mp::cpp_int d{modular_linear_eq(e, 1, e_n)};

    // 5th step
    // PUB pair
    std::ofstream pub_fs(p_path);
    pub_fs << boost::lexical_cast<std::string>(e) << "\n";
    pub_fs << boost::lexical_cast<std::string>(n);
    pub_fs.close();
//    std::cout << "Public pair written to: " << p_path << "\n";

    // PRIV pair
    std::ofstream priv_fs(s_path);
    priv_fs << boost::lexical_cast<std::string>(d) << "\n";
    priv_fs << boost::lexical_cast<std::string>(n);
    priv_fs.close();
//    std::cout << "Secret pair written to: " << s_path << "\n";
}

void read_keys(std::pair<mp::cpp_int, mp::cpp_int>& pair, const std::string& file) {
    std::ifstream fs(file);
    std::string line;
    size_t count{0};
    mp::cpp_int e, n;
    while (getline(fs, line)) {
        if (count == 0) {
            e = boost::lexical_cast<mp::cpp_int>(line);
            count++;
        } else {
            n = boost::lexical_cast<mp::cpp_int>(line);
        }
    }
    fs.close();
    pair = std::make_pair(e, n);
}

void test_encrypt_decrypt(uint1024_t& num, std::string p_path, std::string s_path) {
    boost::chrono::high_resolution_clock::time_point start;
    boost::chrono::high_resolution_clock::time_point end;
    // Reading keys
    mp::cpp_int enc{0};
    mp::cpp_int dec{0};
    std::pair<mp::cpp_int, mp::cpp_int> pub;
    read_keys(pub, p_path);
    std::pair<mp::cpp_int, mp::cpp_int> priv;
    read_keys(priv, s_path);
//    std::cout << "Encrypting " << num << " with:\n";
//    std::cout << "P(" << pub.first << ", " << pub.second << ")\n";
//    std::cout << "S(" << priv.first << ", " << priv.second << ")\n";
    start = boost::chrono::high_resolution_clock::now();
    utils::modular_exponentiation(num, pub.first, pub.second, enc);
    end = boost::chrono::high_resolution_clock::now();
    std::cout << "Encryption " << " took " << boost::chrono::duration_cast<boost::chrono::microseconds>(end - start) << "\n";

    start = boost::chrono::high_resolution_clock::now();
    utils::modular_exponentiation(enc, priv.first, priv.second, dec);
    end = boost::chrono::high_resolution_clock::now();
    std::cout << "Decryption " << " took " << boost::chrono::duration_cast<boost::chrono::microseconds>(end - start) << "\n";
    std::cout << "Decrypted number: " << dec << "\n\n";
}

void benchmark(uint1024_t num) {
    boost::chrono::high_resolution_clock::time_point start;
    boost::chrono::high_resolution_clock::time_point end;
    size_t bits[] = {6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 32, 64, 128, 256, 512};
    for (auto && b: bits) {
        start = boost::chrono::high_resolution_clock::now();
        generate_pair(b, PUB_PATH, PRIV_PATH);
        end = boost::chrono::high_resolution_clock::now();
        std::cout << "Generating " << b << "-bit key pair took " << boost::chrono::duration_cast<boost::chrono::microseconds>(end - start) << "\n";
        test_encrypt_decrypt(num, PUB_PATH, PRIV_PATH);
    }
}


int main(int argc, char** argv) {
//    encrypt();
    if (argc == 1) {
        std::cerr << "No arguments passed.\n";
        return 1;
    }
    try {
        bool gen{false}, test{false};
        boost::program_options::options_description desc("Allowed options");
        desc.add_options()
                ("help,h", "Produce help message")
                ("generate,g", boost::program_options::bool_switch(&gen), "Generate keys, needs parameter --bits|-b <count>")
                ("test,t", boost::program_options::bool_switch(&test), "Test keys, needs parameter --public|-p <path> and --secret|-s <path>")
                ("bits,b", boost::program_options::value<size_t>(), "Bits for RSA key")
                ("public,p", boost::program_options::value<std::string>(), "Public key path")
                ("secret,s", boost::program_options::value<std::string>(), "Secret key path")
                ;
        boost::program_options::variables_map vm;
        boost::program_options::store(boost::program_options::parse_command_line(argc, argv, desc), vm);
        boost::program_options::notify(vm);

        if (vm.count("help")) {
            std::cout << desc << "\n";
            return 0;
        }
        if (gen) {
            std::string s_path="priv.txt", p_path="pub.txt";
            if (!vm.count("bits")) {
                std::cout << "No bits parameter provided.\n";
                return 0;
            }
            size_t bits = vm["bits"].as<size_t>();
            if (vm.count("public")) {
                p_path = vm["public"].as<std::string>();
            }
            if (vm.count("secret")) {
                s_path = vm["secret"].as<std::string>();
            }
            std::cout << "Generating " << bits << " bit key pair\n";
            generate_pair(bits, p_path, s_path);

        } else if (test) {
            if (!vm.count("public") || !vm.count("secret")) {
                std::cout << "No keys provided.\n";
                return 0;
            }
            std::string p_path = vm["public"].as<std::string>();
            std::string s_path = vm["secret"].as<std::string>();

            std::cout << "Running tests ...\n";
            std::cout << "Input number: ";
            uint1024_t num;
            std::cin >> num;
            benchmark(num);
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
        return 1;
    }
    catch (...) {
        std::cerr << "Exception of unknown type!\n";
        return 1;
    }

    return 0;
}



//void encrypt(const std::string& key_path="pub.txt", const std::string& msg_path="msg.txt") {
//    // Init
//    std::pair<mp::cpp_int, mp::cpp_int> pub;
//    read_keys(pub, key_path);
//    std::string bin{utils::file_to_bin(msg_path)};
//    size_t bits_req{mp::msb(pub.second)};
//    size_t remainder{bin.length() % bits_req};
//    std::stringstream ss;
//    ss << bin;
//    for (size_t i{0}; i < bits_req - remainder; ++i) {
//        ss << "0";
//    }
//    bin = ss.str();
//
//    // Encrypt here
//    std::string tmp;
//    uint128_t num;
//    mp::cpp_int value;
//    ss.str(std::string());
//    for (size_t i{0}; i < bin.length(); i += bits_req) {
//        tmp = bin.substr(i, bits_req);
//        boost::dynamic_bitset<unsigned char> dynbs(tmp);
//        num = dynbs.to_ulong();
//        utils::modular_exponentiation(num, pub.first, pub.second, value);
//        std::string outer = std::bitset<61>(boost::lexical_cast<uint64_t>(value)).to_string();
//        ss << outer;
//    }
//    std::string encrypted = ss.str(); // encrypted string
//    size_t enc_size = encrypted.length();
//    size_t msg_size = bin.length();
//    std::ofstream ofs("enc.txt", std::ios::out | std::ios::binary);
//    unsigned long bin_val;
//    for (size_t i = 0; i < encrypted.length(); i += 61) {
//        tmp = encrypted.substr(i, 61);
//        std::bitset<61> bits_out (tmp);
//        bin_val = bits_out.to_ulong();
//        ofs.write((const char*)&bin_val, 61);
//    }
//    ofs.close();
//}
//
//void decrypt(const std::string& key_path="priv.txt", const std::string& msg_path="enc.txt") {
//    // Init
//    std::pair<uint128_t, uint128_t> priv;
//    read_keys(priv, key_path);
//    std::string bin_encrypted{utils::file_to_bin(msg_path)};
//    size_t enc_size = bin_encrypted.length();
//    size_t bits{(size_t)std::ceil(std::log2(priv.second.convert_to<double>()))};
//    std::stringstream ss;
//
//    // Decrypt here
//    std::string tmp;
//    uint128_t num;
//    uint128_t value;
//    for (size_t i{0}; i < bin_encrypted.length(); i += bits) {
//        tmp = bin_encrypted.substr(i, bits);
//        boost::dynamic_bitset<unsigned char> dynbs(tmp);
//        num = dynbs.to_ulong();
//        utils::modular_exponentiation(num, priv.first, priv.second, value);
//        ss << std::bitset<61>(boost::lexical_cast<uint64_t>(value)).to_string();
//    }
//    std::string decrypted = ss.str(); // decrypted string
//    std::ofstream ofs("dec.txt", std::ios::out | std::ios::binary);
//    unsigned long bin_val;
//    for (size_t i = 0; i < decrypted.length(); i += 61) {
//        tmp = decrypted.substr(i, 61);
//        std::bitset<61> bits_out (tmp);
//        bin_val = bits_out.to_ulong();
//        ofs.write((const char*)&bin_val, 61);
//    }
//    ofs.close();
//}