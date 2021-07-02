class Reg:
    def __init__(self, l, t, n, dna):
        self.m_dna = dna
        self.m_l = l
        self.m_t = t
        self.m_n = n

        self.m_nmer = []
        for i in range(self.m_t):
            self.m_nmer.append(self.m_dna[i * self.m_n:(i + 1) * self.m_n])

        self.m_off = [0] * self.m_t
        self.m_off_best = [0] * self.m_t

        self.m_cons_possible = [""] * self.m_l
        self.m_cons_best = [""] * self.m_l

        self.m_score_max = 0
        self.m_score_min = self.m_t * self.m_l

        self.m_gene_type = ['A', 'C', 'G', 'T']
        self.m_gene_count = [0] * len(self.m_gene_type)

    def __str__(self):
        return f"dna: {self.m_dna}\n" \
               f"l: {self.m_l}\n" \
               f"t: {self.m_t}\n" \
               f"n: {self.m_n}"

    def greedy_print_offset(self):
        ret_str = "[ "
        for i in self.m_off_best:
            ret_str += str(i) + " "
        ret_str += "] -> " + str(self.m_score_max)
        print(ret_str)

    def print_branch_offset(self):
        res_str = "[ "
        for i in self.m_off_best:
            res_str += str(i) + " "
        res_str += "] -> " + str(self.m_score_min)
        print(res_str)

    def print_cons(self):
        print(self.m_cons_best)
