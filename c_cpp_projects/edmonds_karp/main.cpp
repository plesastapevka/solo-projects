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
    size_t v1, v2;
    int capacity;
    int fuv, fvu;
};

struct vertice_t {
    int prev, len;
    size_t index;
    STATUS status;
};

struct graph_t {
    graph_t(std::vector<vertice_t> vertices, std::vector<edge_t> edges):
        m_vertices(vertices),
        m_edges(edges){}

    std::vector<vertice_t> m_vertices;
    std::vector<edge_t> m_edges;
    size_t max_flow = 0;
};

// prototypes
size_t edmonds_karp(std::shared_ptr<graph_t> G, size_t s, size_t t);
int get_path(std::shared_ptr<graph_t> G, size_t prev, size_t index);
void set_f(std::shared_ptr<graph_t> G, size_t prev, size_t index, int path);
bool bfs(std::shared_ptr<graph_t> G, size_t s, size_t d);
std::shared_ptr<graph_t> init_graph(const std::string& path);

size_t edmonds_karp(std::shared_ptr<graph_t> G, size_t s, size_t t) {
    while (bfs(G, s, t)) {
        graph_t tmp = *G;
        vertice_t v = G->m_vertices[t];
        int path = get_path(G, v.prev, v.index);
        while (v.index != s) {
            int new_path = get_path(G, v.prev, v.index);
            if (new_path < path) path = new_path;
            v = G->m_vertices[v.prev];
        }

        v = G->m_vertices[t];
        while (v.index != s) {
            set_f(G, v.prev, v.index, path);
            v = G->m_vertices[v.prev];
        }
        G->max_flow += path;
    }
}

int get_path(std::shared_ptr<graph_t> G, size_t prev, size_t index) {
    if (prev == -1) return std::numeric_limits<int>::max();
    for (auto const& e : G->m_edges) {
        if (e.v1 == G->m_vertices[prev].index && e.v2 == G->m_vertices[index].index) {
            return e.capacity;
        }
    }
    return std::numeric_limits<int>::max();
}

void set_f(std::shared_ptr<graph_t> G, size_t prev, size_t index, int path) {
    if (prev == -1) return;
    for (auto& e : G->m_edges) {
        if (e.v1 == G->m_vertices[prev].index && e.v2 == G->m_vertices[index].index) {
            e.fuv += path;
            e.fvu -= path;
        }
    }
}

bool bfs(std::shared_ptr<graph_t> G, size_t s, size_t d) {
    std::queue<vertice_t> queue;
    for (auto& v : G->m_vertices) {
        if (v.index == s) {
            v.status = IN_PROGRESS;
            v.len = 0;
            queue.push(v);
        } else {
            v.status = NA;
            v.len = std::numeric_limits<int>::max();
            v.prev = -1;
        }
    }
    graph_t tmp = *G;
    vertice_t v{};
    while (!queue.empty()) {
        v = queue.front();
        queue.pop();
        if (v.index == G->m_vertices[d].index) return true;
        for (auto& link : G->m_edges) { // iterate through v's neighbors
            if (link.v1 == v.index) {
                int neighbor = link.v2;
                if (G->m_vertices[neighbor].status == NA && (link.capacity - link.fuv) > 0) {
                    G->m_vertices[neighbor].status = IN_PROGRESS;
                    G->m_vertices[neighbor].len = v.len + link.capacity;
                    G->m_vertices[neighbor].prev = v.index;
                    queue.push(G->m_vertices[neighbor]);
                }
            }
        }
        G->m_vertices[v.index].status = DEVELOPED;
    }
    return false;
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
            vals.clear();
        } else {
            edge_t edge{};
            edge.v1 = vals[0];
            edge.v2 = vals[1];
            edge.capacity = vals[2];
            edge.fuv = 0;
            edge.fvu = 0;
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
    size_t start = 0, end = 5;
    edmonds_karp(graph, start, end);
//    (0, 1) [12/16]
//    (0, 2) [11/13]
//    (1, 2) [0/10]
//    (1, 3) [12/12]
//    (2, 4) [11/14]
//    (3, 2) [0/9]
//    (3, 5) [19/20]
//    (4, 3) [7/7]
//    (4, 5) [4/4]
    for (auto& e: graph->m_edges) {
        std::cout << "(" << e.v1 << ", " << e.v2 << ") [" << e.fuv << "/" << e.capacity << "]\n";
    }
    std::cout << "\nMax flow: " << graph->max_flow << "\n";
    return 0;
}
