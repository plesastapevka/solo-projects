#!/usr/bin/python3.6
import sys


def split3(str1, str2):
    part_size = int(len(str1) / 3)
    # split_str1, split_str2 = [], []
    #
    # for i in range(3):
    #     split_str1.append(str1[i * part_size:part_size + (i * part_size)])
    #     split_str2.append(str2[i * part_size:part_size + (i * part_size)])

    return \
        [str1[i * part_size:part_size + (i * part_size)]for i in range(3)],\
        [str2[i * part_size:part_size + (i * part_size)]for i in range(3)]

"""
:param arr1: vsebuje podnize prvega niza
:type arr1: polje
:param arr2: vsebuje podnize drugega niza
:type arr1: polje
"""
def equalSubStrings(arr1, arr2):
    index = 0
    el_del = False

    while not index == len(arr1):
        for i in range(len(arr2)):
            if arr1[index] == arr2[i]:
                del arr1[index], arr2[i]
                el_del = True
                break

        if el_del:
            el_del = False
            continue

        index += 1

    if not arr1:  # is empty
        return True

    elif len(arr1[0]) > 1:
        arr1_split, arr2_split = [], []

        for arr1_el, arr2_el in zip(arr1, arr2):
            arr1_split[len(arr1_split):], arr2_split[len(arr2_split):] = split3(arr1_el, arr2_el)

        return equalSubStrings(arr1_split, arr2_split)

    return False


def equalStrings(str1, str2):
    if str1 == str2:
        return True

    split1, split2 = split3(str1, str2)

    return equalSubStrings(split1, split2)


# Driver program to test the above function
def main():
    input_data = sys.stdin.readlines()
    numb_of_pairs = int(input_data.pop(0))

    for _ in range(numb_of_pairs):
        str1 = input_data[0]
        input_data.pop(0)

        str2 = input_data[0]
        input_data.pop(0)

        if equalStrings(str1, str2):
            print('enaka')
        else:
            print('razlicna')


if __name__ == "__main__":
    main()
