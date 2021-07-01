import sys
import time
from itertools import combinations
from utils import Utils
from DNA import DNA

TEST_PATH = "data/DNK1.txt"


def brute_force(dna):
    M = dna.m_L[-1]
    combos = combinations(dna.m_L, dna.m_N - 2)
    for item in combos:
        item = list(item)
        item.insert(0, 0)
        item.append(M)
        res = create_delta(item)
        if res == dna.m_L:
            print(item)  # todo polepsaj
            dna.m_results.append(item)
            # return item


def create_delta(fake_l):
    s_len = len(fake_l)
    dist = []
    for i in range(s_len):
        for j in range(i, s_len):
            if i != j:
                dist.append(abs(fake_l[i] - fake_l[j]))
    dist.sort()
    return dist


def partial_digest(dna):
    width = dna.m_L[-1]
    dna.m_L.remove(width)
    X = [0, width]
    place(dna, X, width)


def place(dna, X, width):
    if not dna.m_L:
        print(X)
        dna.m_results.append(list(X))
        return
    y = dna.m_L[-1]
    D = delta(y, X)
    if set(D).issubset(dna.m_L):
        X.append(y)
        for i in D:
            dna.m_L.remove(i)
        X.sort()
        place(dna, X, width)
        X.remove(y)
        for i in D:
            dna.m_L.append(i)
        dna.m_L.sort()
    D = delta(width - y, X)
    if set(D).issubset(dna.m_L):
        X.append(width - y)
        for i in D:
            dna.m_L.remove(i)
        X.sort()
        place(dna, X, width)
        X.remove(width - y)
        for i in D:
            dna.m_L.append(i)
        dna.m_L.sort()
    return


def delta(y, X):
    distances = []
    for item in X:
        distances.append(abs(item - y))
    distances.sort()
    return distances


def find_string(dna, find_list):
    init_len = len(dna.m_L)
    found = 0
    for item in find_list:
        res = [i for i in range(len(dna.m_data)) if dna.m_data.startswith(item, i)]
        for r in res:
            dna.m_L.append(r)
            found += 1
    if len(dna.m_L) != init_len:
        dna.m_N = found + 2
        return True
    else:
        return False


def build_input(dna):
    dna.m_L.append(len(dna.m_data) - 1)
    s_len = len(dna.m_L)
    for i in range(s_len):
        for j in range(i, s_len):
            if i == j:
                continue
            dna.m_L.append(abs(dna.m_L[i] - dna.m_L[j]))
    dna.m_L.sort()


def main():
    if len(sys.argv) == 4:
        print("Restriction mapping started.")
        mode = sys.argv[1]
        input_file = sys.argv[2]
        substring = sys.argv[3]

        # data = Utils.read_file(input_file)  # Read from file
        data = Utils.read_file(TEST_PATH)
        dna = DNA(data)
        find_list = substring.split(",")
        find_string(dna, find_list)
        build_input(dna)
        print("String: ")
        print(find_list)

        if mode == "naive":
            print("Mode: NAIVE")
            print("NAIVE METHOD")
            start_time = time.time()
            brute_force(dna)
            print("--- %s seconds ---" % (time.time() - start_time))
            if dna.m_results:
                Utils.write_file("results_naive.txt", dna.m_results)

        elif mode == "partial_digest":
            print("PARTIAL DIGEST METHOD")
            start_time = time.time()
            partial_digest(dna)
            print("--- %s seconds ---" % (time.time() - start_time))
            if dna.m_results:
                Utils.write_file("results_partial_digest.txt", dna.m_results)

    else:
        print("Invalid arguments")


if __name__ == "__main__":
    main()
