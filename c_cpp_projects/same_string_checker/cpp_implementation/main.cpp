#include <iostream>
#include <memory>
#include <string>
#include <fstream>
#include <vector>
#include <algorithm>
#define MAX_CHAR 26

enum mode {
    count = 0,
    first = 1,
    second = 2
};

std::string sortString(std::string& str) {
    std::shared_ptr<std::vector<int>> charCount = std::make_shared<std::vector<int>>(MAX_CHAR);
    std::fill(charCount->begin(), charCount->end(), 0);

    for(char i : str) {
        charCount->at(i - 'a')++;
    }

    std::string sorted;

    for (int i = 0; i < MAX_CHAR; i++) {
        for (int j = 0; j < charCount->at(i); j++) {
            sorted += (char)('a' + i);
        }
    }

    return sorted;
}

std::shared_ptr<std::vector<std::string>> splitString(const std::string& str) {
    std::vector<std::string> chunks;
    unsigned long chunk_size = str.length() / 3;

    chunks.push_back(str.substr(0, chunk_size));
    chunks.push_back(str.substr(chunk_size, chunk_size));
    chunks.push_back(str.substr(2 * chunk_size, chunk_size));

    return std::make_shared<std::vector<std::string>>(chunks);
}

std::string sortAndMerge(const std::shared_ptr<std::vector<std::string>>& chunks) {
    sort(chunks->begin(), chunks->end());
    std::string merged;
    for(auto& chunk : *chunks) {
        merged += chunk;
    }
    return merged;
}

std::string simplify(std::string str) {
    if(str.length() == 1) {
        return str;
    } else if(str.length() == 3) {
        return sortString(str);
    } else {
        std::shared_ptr<std::vector<std::string>> splitted = splitString(str);
        std::shared_ptr<std::vector<std::string>> simplified = std::make_shared<std::vector<std::string>>();
        simplified->reserve(splitted->size());

        for(const auto& a: *splitted) {
            simplified->push_back(simplify(a));
        }

        return sortAndMerge(simplified);
    }
}

int main(int argc, char* argv[]) {
    bool debug = getenv("DEBUG") != nullptr;
    mode m = count;
    std::shared_ptr<std::istream> input = nullptr;

    if(!debug) {
        input.reset(&std::cin, [](...){});
    } else if(std::strcmp(argv[1], "") != 0) {
        std::shared_ptr<std::string> path = std::make_shared<std::string>(argv[1]);
        input = std::make_shared<std::ifstream>(path->c_str());
    } else {
        return 1;
    }
    int examples;
    std::string line;
    std::string firstString;
    std::string secondString;

    while(!std::getline(*input, line).eof()) {
        switch(m) {
            case count:
                try {
                    examples = stoi(line);
                } catch(...) {
                    std::cout << "First element not a number" << std::endl;
                    return 1;
                }
                m = first;
                break;
            case first:
                firstString = line;
                m = second;
                break;
            case second:
                secondString = line;
                std::cout << (simplify(firstString) == simplify(secondString) ? "enaka\n" : "razlicna\n");
                m = first;
                break;
        }
    }

    return 0;
}