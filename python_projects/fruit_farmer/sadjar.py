import sys
import numpy as np


def first_it(N, D, L, array, values, iteration, max_found):
    for i in range(N - 1, -1, -1):
        if i + D > N:  # setting the bottom elements of list
            L[i][iteration] = array[i]
            if i + 1 < N:  # accumulating values until
                L[i][iteration] += L[i + 1][iteration]
        else:  # setting all the other elements
            if values:
                L[i][iteration] = array[i] + values[0]
            else:
                L[i][iteration] = array[i]
            if i + D != N:
                L[i][iteration] -= array[i + D]
        values.insert(0, L[i][iteration])  # adds value to values
        if L[i][iteration] >= max_found:  # if better element is found
            max_found = L[i][iteration]
        L[i][iteration] = max_found
    return L, values, max_found


def other_it(N, D, L, values, iteration):
    max_found = 0
    for i in range(N - 1, -1, -1):
        if i + D < N:  # sum up current value and previous value D days ahead
            currval = values[i] + L[i + D][iteration - 1]
        else:
            currval = values[i]
        if currval > max_found:  # finding the best element
            max_found = currval
        L[i][iteration] = max_found
    return L


def compute(array):
    N = int(trees)
    D = int(per_day)
    K = int(days)
    L = np.zeros((N, K))
    max_found = 0
    values = []

    for j in range(K):
        if j == 0:  # first iteration for help array building
            L, values, max_found = first_it(N, D, L, array, values, j, max_found)
        else:
            L = other_it(N, D, L, values, j)
    print(int(L[0][K-1]))


first = True
second = False
trees = 0
per_day = 0
days = 0
apples = []
for line in sys.stdin:
    splitted = line.rstrip().split(" ")
    if first:  # prvi line N, D, K - st. jablan, st. nabranih/dan, st. dni
        trees = int(splitted[0])
        per_day = int(splitted[1])
        days = int(splitted[2])
        first = False
        second = True

    elif second:  # parsanje v apples
        apples = [int(i) for i in splitted]
        break

compute(apples)
