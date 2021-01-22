#include <iostream>
#include <vector>
#include <map>
#include <fstream>
#include <sstream>

struct edge_t {
    size_t v1, v2, capacity;
};

struct graph_t {
    graph_t(std::vector<size_t> vertices, std::vector<edge_t> edges):
        m_vertices(vertices),
        m_edges(edges) {}

    std::vector<size_t> m_vertices;
    std::vector<edge_t> m_edges;
};

size_t edmonds_karp();

size_t edmonds_karp(graph_t& G, size_t s, size_t t) {
    size_t max_flow = 0;
    for (auto e : G.m_edges) {

    }
}

std::shared_ptr<graph_t> init_graph(const std::string& path) {
    std::ifstream file(path);
    if (!file.is_open()) {
        std::cerr << "Cannot open file.\n";
        return nullptr;
    }
    std::vector<size_t> vertices;
    std::vector<edge_t> edges;
    std::string line, val;
    std::vector<size_t> vals;
    while (std::getline(file, line)) {
        std::stringstream ss(line);
        while (getline(ss, val, ' ')) {
            vals.push_back(std::stoi(val));
        }
        if (vals.size() == 2) {
            for (size_t i = 0; i < vals[0]; ++i) {
                vertices.push_back(i);
            }
            vals.clear();
        } else {
            edge_t edge{};
            edge.v1 = vals[0];
            edge.v2 = vals[1];
            edge.capacity = vals[2];
            edges.push_back(edge_t(edge));
            vals.clear();
        }
    }
    file.close();
    graph_t graph (vertices, edges);
    return std::make_shared<graph_t>(graph);
}

int main() {
    std::shared_ptr<graph_t> graph{init_graph("input.txt")};
    if (graph == nullptr) {
        return 1;
    }
    return 0;
}
