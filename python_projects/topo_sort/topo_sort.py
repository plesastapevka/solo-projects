import sys


class Graf:
    def __init__(self, stevk):  # konstruktor
        # self.graf = defaultdict(list)
        self.V = stevk
        self.graf = {}
        for i in range(self.V):
            self.graf[i+1] = []

        self.visited = []
        self.queue = []
        self.developing = []
        self.cycle = False

    def dodaj(self, od, do):  # doda povezavo
        self.graf[od].append(do)

    def topo_sort(self):  # zalaufa algo
        for u in self.graf:
            if u not in self.visited:
                self.razvij(u)
        if self.cycle:
            print("nemogoce")
            return -1

        return self.queue

    def razvij(self, u):
        self.developing.append(u)
        if self.cycle:
            return

        for v in self.graf[u]:
            if v not in self.visited and v not in self.developing:
                self.razvij(v)
            elif v in self.developing:
                self.cycle = True
                break

        self.queue.insert(0, u)
        self.developing.remove(u)
        self.visited.append(u)


def find_max_digit(i, g, max, queue):
    index = queue[i]
    if i >= g.V - 1:
        return
    for e in g.graf[index]:
        if max[e-1] >= max[index - 1]:
            max[e-1] = max[index - 1] - 1
        find_max_digit(i + 1, g, max, queue)


def find_min_digit(i, g, queue, value=1):
    tmp = value
    for e in g.graf[i]:
        new = find_min_digit(e, g, queue, value + 1)
        if new > tmp:
            tmp = new
    return tmp


def find_numbers(q, graf):
    max_num = [9] * graf.V
    min_num = [1] * graf.V

    for i in range(graf.V):
        find_max_digit(i, graf, max_num, q)
        min_num[q[i] - 1] = find_min_digit(q[i], graf, q)

    if all(0 < i < 10 for i in max_num):
        return max_num, min_num
    else:
        return -1, -1


examples = 0
count = 0
conditions = 0
graph = None
for line in sys.stdin:
    line = line.rstrip()
    chars = line.split(" ")
    if examples == 0:
        examples = int(line)
        continue

    if ">" not in chars and "<" not in chars:  # nov graf
        count = 0
        graph = Graf(int(chars[0]))
        conditions = int(chars[1])

    elif ">" in chars and count < conditions:  # dodamo povezavo od do
        count += 1
        graph.dodaj(int(chars[0]), int(chars[2]))

    elif "<" in chars and count < conditions:  # dodamo povezavo do od
        count += 1
        graph.dodaj(int(chars[2]), int(chars[0]))

    if count == conditions:  # zalaufamo algo
        count = 0
        queue = graph.topo_sort()
        if queue != -1:
            max, min = find_numbers(queue, graph)
            if max != -1 and min != -1:
                max = map(str, max)
                min = map(str, min)
                # print("Min number: " + ''.join(min))
                # print("Max number: " + ''.join(max))
                print(''.join(min) + " " + ''.join(max))
            else:
                print("nemogoce")
