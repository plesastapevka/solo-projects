import networkx as nx
import collections


class HopcroftKarp:
    infinity = -1

    def __init__(self, G):
        self.G = G
        self.U, self.V = self.partition()
        self.pair = {}
        self.dist = {}
        self.queue = collections.deque()

    def bfs(self):
        for u in self.U:
            if self.pair[u] is None:
                self.dist[u] = 0
                self.queue.append(u)
            else:
                self.dist[u] = HopcroftKarp.infinity

        self.dist[None] = HopcroftKarp.infinity

        while len(self.queue) > 0:
            u = self.queue.popleft()
            if u is not None:
                for v in self.G.neighbors(u):
                    if self.dist[self.pair[v]] == HopcroftKarp.infinity:
                        self.dist[self.pair[v]] = self.dist[u] + 1
                        self.queue.append(self.pair[v])

        return self.dist[None] != HopcroftKarp.infinity

    def dfs(self, u):
        if u is not None:
            for v in self.G.neighbors(u):
                if self.dist[self.pair[v]] == self.dist[u] + 1 and self.dfs(self.pair[v]):
                    self.pair[v] = u
                    self.pair[u] = v

                    return True

            self.dist[u] = HopcroftKarp.infinity
            return False

        return True

    def compute(self):
        for u in self.G:
            self.pair[u] = None
            self.dist[u] = HopcroftKarp.infinity
        matching = 0
        while self.bfs():
            for u in self.U:
                if self.pair[u] is None and self.dfs(u):
                    matching = matching + 1
        return matching

    def partition(self):
        return nx.algorithms.bipartite.sets(self.G)
