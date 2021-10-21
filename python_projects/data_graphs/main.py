import csv
from row import Row, Rows

DATA_PATH = "data/DebelinaRazreza.csv"


def main():
    file = open(DATA_PATH)
    reader = csv.reader(file)
    data = Rows()
    first = True
    for row in reader:
        if first:
            first = False
            continue
        data.add_row(row[0], row[1], row[2], row[3], row[4], row[5], row[6])
    data.plot_histo()


if __name__ == '__main__':
    main()
