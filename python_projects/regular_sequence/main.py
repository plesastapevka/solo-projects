import getopt
import sys
import time

from reg import Reg
from utils import Utils

TEST_DATA = "data/DNK.txt"


def greedy(reg, index=0):
    if index >= reg.m_t:  # find all variations
        profile_score = 0  # score of current profile
        for i in range(reg.m_l):
            reg.m_gene_count = [0] * len(reg.m_gene_type)
            for j in range(len(reg.m_nmer)):  # for each nmer
                for occ in range(len(reg.m_gene_type)):  # for ATGC
                    # get current character
                    if reg.m_nmer[j][reg.m_off[j] + i] == reg.m_gene_type[occ]:
                        reg.m_gene_count[occ] += 1  # increment index
                        break
            max_tmp = 0  # find max, add to profile
            for j in range(len(reg.m_gene_count)):  # find best letter based on occurence
                if reg.m_gene_count[j] > max_tmp:
                    max_tmp = reg.m_gene_count[j]
                    reg.m_cons_possible[i] = reg.m_gene_type[j]
            profile_score += max_tmp

        if profile_score > reg.m_score_max:  # set if better
            reg.m_score_max = profile_score
            reg.m_off_best = reg.m_off.copy()
            reg.m_cons_best = reg.m_cons_possible.copy()
        return
    else:
        for i in range(reg.m_n - reg.m_l + 1):  # 1 - n-l+1...
            reg.m_off[index] = i
            greedy(reg, index + 1)  # recursion
        return


def branch_bound(reg, index=0):
    if index >= reg.m_l:  # find all variations
        current_hamming_dist = reg.m_l * reg.m_t
        cons_dist = 0
        for i in range(len(reg.m_nmer)):  # consensus of all nmers
            lowest_hamming = reg.m_l  # min hamming
            for off in range(reg.m_n - reg.m_l + 1):  # hamming between current consensus and offsets
                hamming_dist = 0
                break_loop = False
                for j in range(len(reg.m_cons_possible)):
                    if reg.m_cons_possible[j] != reg.m_nmer[i][off + j]:
                        hamming_dist += 1
                    if hamming_dist >= lowest_hamming:  # stop
                        break_loop = True
                        break
                if break_loop:
                    continue
                lowest_hamming = hamming_dist
                reg.m_off[i] = off

            cons_dist += lowest_hamming  # best offset for current consensus, sum
            if cons_dist >= current_hamming_dist:
                break
        current_hamming_dist = cons_dist

        if current_hamming_dist < reg.m_score_min:
            reg.m_score_min = current_hamming_dist
            reg.m_off_best = reg.m_off.copy()
            reg.m_cons_best = reg.m_cons_possible.copy()
        return
    else:
        for i in range(len(reg.m_gene_type)):
            reg.m_cons_possible[index] = reg.m_gene_type[i]
            branch_bound(reg, index + 1)
        return


def main():
    try:
        opts, args = getopt.getopt(sys.argv[1:], "f:m:n:l:t:h")
    except getopt.GetoptError as err:
        print(err)
        sys.exit(2)

    t = None
    l = None
    n = None

    mode = "branch"
    input_file = TEST_DATA

    for opt, arg in opts:
        if opt == "-f":
            input_file = arg
        elif opt == "-m":
            mode = arg
        elif opt == "-n":
            n = int(arg)
        elif opt == "-l":
            l = int(arg)
        elif opt == "-t":
            t = int(arg)
        else:
            print(f"Argument {opt} with value {arg} not recognized")
            exit(1)

    if t is None or l is None or n is None or mode is None or input_file is None:
        print("Error in passed arguments")
        exit(1)

    if t > 5 or t < 2:
        print("Argument t should be between 2 and 5")
        exit(1)

    if l > 10 or l < 2:
        print("Argument l should be between 2 and 10")
        exit(1)

    if n > 100 or n < l:
        print("Argument n should be between l and 100")
        exit(1)

    data = Utils.read_file(input_file)
    reg = Reg(l, t, n, data)
    start = 0
    end = 0

    if mode == "greedy":
        mode = "GREEDY algorithm"
        print(mode)
        print(reg)
        start = time.time()
        greedy(reg)
        end = time.time()
        reg.greedy_print_offset()

    elif mode == "branch":
        mode = "BRANCH-BOUND algorithm"
        print(mode)
        print(reg)
        start = time.time()
        branch_bound(reg)
        end = time.time()
        reg.print_branch_offset()

    else:
        print("Invalid mode.")
        exit(1)

    print(f"{mode} finished in {round((end - start) * 1000)} miliseconds")
    print(f"Consensus: ")
    reg.print_cons()
    print("\n")


if __name__ == '__main__':
    main()
