import sys
import hka
import networkx as nx
import numpy as np


class Gostje:
    def __init__(self, st):
        self.V = st
        self.gostje = {}
        for i in range(st):
            self.gostje[i] = []

    def dodaj_gosta(self, guest, likes):
        self.gostje[guest].append(likes)


class Sladice:
    def __init__(self, st):
        self.V = st
        self.sladice = {}
        for i in range(st):
            self.sladice[i] = 0

    def dodaj_sladico(self, position, quantity):
        self.sladice[position] = quantity


first = True
s_turn = False
g_turn = False
sladice = {}
gostje = {}
count = 0
for line in sys.stdin:
    splitted = line.rstrip().split(" ")
    if first:  # prvi line so N M, N = st sladic, M = st gostov
        sladice = Sladice(int(splitted[0]))
        gostje = Gostje(int(splitted[1]))
        first = False
        s_turn = True

    elif s_turn:  # parsanje sladic
        for i in range(len(splitted)):
            sladice.dodaj_sladico(i, int(splitted[i]))

        s_turn = False
        g_turn = True

    elif g_turn:  # parsanje gostov
        for i in range(len(splitted)):
            gostje.dodaj_gosta(count, int(splitted[i]) - 1)
        count += 1
        if count == gostje.V:
            break

# main
cols = 0
rows = gostje.V
graph = nx.Graph()

# spodnja zanka presteje stolpcev ki jih bomo potrebovali
for i in range(sladice.V):
    cols += sladice.sladice[i]

# spodnja zanka ustvari referencno matriko
ref_mat = np.zeros(shape=(rows, cols))
for row in range(rows):
    for pov in gostje.gostje[row]:
        index = 0
        for s in range(sladice.V):
            if pov == s:
                for i in range(index, index + sladice.sladice[s]):
                    ref_mat[row][i] = 1
            index += sladice.sladice[s]

# spodnja zanka ustvari vozlisca v grafu
for i in range(rows + cols):
    graph.add_node(i)

# spodnja zanka doda povezave
i_diff = gostje.V
for row in range(rows):
    for col in range(cols):
        if ref_mat[row][col] == 1:
            graph.add_edge(row, col + i_diff)

matching = hka.HopcroftKarp(graph).compute()
if matching < gostje.V:
    print("NE")
else:
    print("DA")
