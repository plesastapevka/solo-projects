from utils import Utils

TEST_PATH = "data/DNK1.txt"


def brute_force(L, n):
    M = max(L)
    X = []
    for i in range(2, L.index(M) + 1):
        X.append(i)
        delta_X = create_delta(X)
        if delta_X == L:
            return X

    return None


def create_delta(X):
    return 1


def main():
    data = Utils.read_file(TEST_PATH)


if __name__ == "__main__":
    main()
