#include <iostream>
#include <vector>
#include <map>
#include <fstream>
#include <sstream>
#include <queue>
#include <limits>

enum STATUS {
    IN_PROGRESS, DEVELOPED, NA
};

struct edge_t {
    size_t v1, v2, capacity;
};

struct vertice_t {
    int prev, len;
    size_t index;
    STATUS status;
};

bool operator==(const vertice_t& v1, const vertice_t& v2) {
    return (v1.prev == v2.prev && v1.len == v2.len && v1.status == v2.status && v1.index == v2.index);
}

struct graph_t {
    graph_t(std::vector<vertice_t> vertices, std::vector<edge_t> edges, std::vector<std::vector<size_t>> adjacency):
        m_vertices(vertices),
        m_edges(edges),
        m_adjacency(adjacency){}

    std::vector<vertice_t> m_vertices;
    std::vector<edge_t> m_edges;
    std::vector<std::vector<size_t>> m_adjacency;
};

// prototypes
size_t edmonds_karp(graph_t& G, size_t s, size_t t);
void bfs(graph_t& G, size_t s);
std::shared_ptr<graph_t> init_graph(const std::string& path);

size_t edmonds_karp(graph_t& G, size_t s, size_t t) {
    size_t max_flow = 0;
    for (auto e : G.m_edges) {

    }
}

void bfs(std::shared_ptr<graph_t> G, size_t s, size_t d) {
    std::queue<vertice_t> queue;
    for (auto v : G->m_vertices) {
        if (v.index == s) {
            v.status = IN_PROGRESS;
            v.len = 0;
            queue.push(v);
            break;
        }
    }
    vertice_t v{};
    while (!queue.empty()) {
        v = queue.front();
        queue.pop();
        if (v == G->m_vertices[d]) return;
        for (auto i : G->m_adjacency[v.index]) {
            auto u = G->m_vertices[i];
            if (u.status == NA) {
                u.status = IN_PROGRESS;
                u.len = v.len + 1;
                u.prev = v.index;
                queue.push(u);
            }
        }
        v.status = DEVELOPED;
    }
}

std::shared_ptr<graph_t> init_graph(const std::string& path) {
    std::ifstream file(path);
    if (!file.is_open()) {
        std::cerr << "Cannot open file.\n";
        return nullptr;
    }
    std::vector<vertice_t> vertices;
    std::vector<edge_t> edges;
    std::string line, val;
    std::vector<size_t> vals;
    std::vector<std::vector<size_t>> adjacency;
    while (std::getline(file, line)) {
        std::stringstream ss(line);
        while (getline(ss, val, ' ')) {
            vals.push_back(std::stoi(val));
        }
        if (vals.size() == 2) {
            for (size_t i = 0; i < vals[0]; ++i) {
                vertice_t v{};
                v.prev = -1;
                v.status = NA;
                v.len = std::numeric_limits<int>::max();
                v.index = i;
                vertices.push_back(v);
            }
            adjacency.resize(vals[0]);
            vals.clear();
        } else {
            edge_t edge{};
            edge.v1 = vals[0];
            edge.v2 = vals[1];
            edge.capacity = vals[2];
            edges.push_back(edge_t(edge));
            adjacency[edge.v1].push_back(edge.v2);
            adjacency[edge.v2].push_back(edge.v1);
            vals.clear();
        }
    }
    file.close();
    graph_t graph (vertices, edges, adjacency);
    return std::make_shared<graph_t>(graph);
}

int main() {
    std::shared_ptr<graph_t> graph{init_graph("input.txt")};
    if (graph == nullptr) {
        return 1;
    }
    return 0;
}
