import getopt
import sys
import time

from reg import Reg
from utils import Utils

TEST_DATA = "data/DNK.txt"


def greedy(reg, index=0):
    if index >= reg.m_t:  # poiscemo vse variacije, za vsako vrstico vse mozne zamike
        profile_score = 0  # profil oz score trenutnega okna
        for i in range(reg.m_l):
            reg.m_gene_count = [0] * len(reg.m_gene_type)
            for j in range(len(reg.m_nmer)):  # za vsaki nmer...
                for occ in range(len(reg.m_gene_type)):  # za vse mozne crke (ATGC)
                    # ugotovimo kateri znak je na trenutnem mestu
                    if reg.m_nmer[j][reg.m_off[j] + i] == reg.m_gene_type[occ]:
                        reg.m_gene_count[occ] += 1  # povecamo na tistem indexu
                        break
            max_tmp = 0  # najdemo maximalno v rezultatu pa dodamo k profilu
            for j in range(len(reg.m_gene_count)):  # najdemo najboljso crko po pojavitvi
                if reg.m_gene_count[j] > max_tmp:
                    max_tmp = reg.m_gene_count[j]
                    reg.m_cons_possible[i] = reg.m_gene_type[j]
            profile_score += max_tmp

        if profile_score > reg.m_score_max:  # ce je bolsi ko prejsni ga nastavimo
            reg.m_score_max = profile_score
            reg.m_off_best = reg.m_off.copy()
            reg.m_cons_best = reg.m_cons_possible.copy()
        return
    else:
        for i in range(reg.m_n - reg.m_l + 1):  # izberi nmer na odmiku 1 do n-l+1...
            reg.m_off[index] = i
            greedy(reg, index + 1)  # rekurzivno dalje
        return


def branch_bound(reg, index=0):
    if index >= reg.m_l:  # poiscemo vse variacije
        current_ham_dist = reg.m_l * reg.m_t
        cons_dist = 0
        for i in range(len(reg.m_nmer)):  # konsenz skozi vse n-mere
            lowest_ham = reg.m_l  # min hamming razdalja
            for off in range(reg.m_n - reg.m_l + 1):  # hamming med trenutnimi konsenzi in offseti
                hamm_dist = 0
                break_loop = False
                for j in range(len(reg.m_cons_possible)):  # skozi vse mozne
                    if reg.m_cons_possible[j] != reg.m_nmer[i][off + j]:
                        hamm_dist += 1
                    if hamm_dist >= lowest_ham:  # ne rabimo vec dalje
                        break_loop = True
                        break
                if break_loop:
                    continue
                lowest_ham = hamm_dist
                reg.m_off[i] = off

            cons_dist += lowest_ham  # imamo najboljsi offset ze trenutni konsenz, sestevamo
            if cons_dist >= current_ham_dist:
                break
        current_ham_dist = cons_dist

        # ce je hammingova razdalja tega konsenza boljsa kot trenutni najboljsi, ga nastavimo
        if current_ham_dist < reg.m_score_min:
            reg.m_score_min = current_ham_dist
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
        print(err)  # will print something like "option -a not recognized"
        print("...py -n X -l Y -t Z -f INPUT.txt")
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

    if l > 10 or l < 2:
        print("Argument l should be between 2 and 10")
        exit(1)

    if n > 100 or n < l:
        print("Argument n should be between l and 100")
        exit(1)

    if t > 5 or t < 2:
        print("Argument t should be between 2 and 5")
        exit(1)

    # print(f"Starting with arguments:\n"
    #       f"t: {t}\n"
    #       f"l: {l}\n"
    #       f"n: {n}\n"
    #       f"input_file: {input_file}\n"
    #       f"mode: {mode}\n")

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

    print(f"{mode} finished in {end - start} seconds")
    print(f"Found consensus: ")
    reg.print_cons()


if __name__ == '__main__':
    main()
