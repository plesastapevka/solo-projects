import sys
import time
from itertools import combinations
from utils import Utils
from DNA import DNA

TEST_PATH = "data/DNK1.txt"


def brute_force(dna):
    M = dna.m_L[-1]
    all_combinations = combinations(dna.m_L, dna.m_N - 2)
    for combo in all_combinations:
        combo = list(combo)
        combo.insert(0, 0)
        combo.append(M)
        result = create_delta(combo)
        if result == dna.m_L:
            print(combo)
            dna.m_results.append(combo)


def create_delta(false_l):
    s_len = len(false_l)
    delta = []
    for i in range(s_len):
        for j in range(i, s_len):
            if j != i:
                delta.append(abs(false_l[i] - false_l[j]))
    delta.sort()
    return delta


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
    dists = []
    for element in X:
        dists.append(abs(element - y))
    dists.sort()
    return dists


def find_string(dna, find_list):
    init_len = len(dna.m_L)
    found = 0
    for element in find_list:
        res = [i for i in range(len(dna.m_data)) if dna.m_data.startswith(element, i)]
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

        data = Utils.read_file(input_file)
        # data = Utils.read_file(TEST_PATH)
        dna = DNA(data)
        find_list = substring.split(",")
        find_string(dna, find_list)
        build_input(dna)
        print("String: ")
        print(find_list)

        if mode == "partial_digest":
            print("Mode: PARTIAL DIGEST")
            start_time = time.time()
            partial_digest(dna)
            print(f"{(time.time() - start_time)} seconds elapsed")
            Utils.write_file("results_partial_digest.txt", dna.m_results)

        elif mode == "naive":
            print("Mode: NAIVE")
            start_time = time.time()
            brute_force(dna)
            print(f"{(time.time() - start_time)} seconds elapsed")
            Utils.write_file("results_naive.txt", dna.m_results)

    else:
        print("Invalid arguments")


if __name__ == "__main__":
    main()
