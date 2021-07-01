import getopt
import sys


TEST_DATA = "data/DNK.txt"


def main():
    try:
        opts, args = getopt.getopt(sys.argv[1:], "f:m:n:l:t:h")
    except getopt.GetoptError as err:
        print(err)  # will print something like "option -a not recognized"
        print("...py -n X -l Y -t Z -f INPUT.txt")
        sys.exit(2)


if __name__ == '__main__':
    main()
