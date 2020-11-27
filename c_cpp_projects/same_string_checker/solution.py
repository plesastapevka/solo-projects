#!/usr/bin/python3.6
import sys


def norm(s):
    if len(s) == 1:
        return s
    if len(s) == 3:
        return ''.join(sorted(s))
    n = len(s)//3
    t = sorted([norm(s[:n]), norm(s[n:2*n]), norm(s[2*n:3*n])])
    return ''.join(t)


# Driver program to test the above function
def main():
    input_data = sys.stdin.readlines()
    numb_of_pairs = int(input_data.pop(0))

    for _ in range(numb_of_pairs):
        s1 = input_data[0].rstrip()  # remove new line
        input_data.pop(0)

        s2 = input_data[0].rstrip()
        input_data.pop(0)

        if norm(s1) == norm(s2):
            print('enaka')
        else:
            print('razlicna')


if __name__ == "__main__":
    main()
