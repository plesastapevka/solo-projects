from matplotlib import pyplot as plt
import time
import datetime


class Rows:
    def __init__(self):
        self._data = []

    def add_row(self, timestamp, p1, p2, p3, p4, p5, p6):
        self._data.append(Row(timestamp, p1, p2, p3, p4, p5, p6))

    def plot_histo(self):
        poses = ["p1", "p2", "p3", "p4", "p5", "p6"]
        timestamps = [time.mktime(datetime.datetime.strptime(r._timestamp, "%Y-%m-%d %H:%M:%S").timetuple()) for r in self._data]
        p1s = [float(r._p1) for r in self._data]
        p2s = [float(r._p2) for r in self._data]
        p3s = [float(r._p3) for r in self._data]
        p4s = [float(r._p4) for r in self._data]
        p5s = [float(r._p5) for r in self._data]
        p6s = [float(r._p6) for r in self._data]
        plt.plot(timestamps, p1s, label="p1", linewidth=0.3)
        plt.plot(timestamps, p2s, label="p2", linewidth=0.3)
        plt.plot(timestamps, p3s, label="p3", linewidth=0.3)
        plt.plot(timestamps, p4s, label="p4", linewidth=0.3)
        plt.plot(timestamps, p5s, label="p5", linewidth=0.3)
        plt.plot(timestamps, p6s, label="p6", linewidth=0.3)
        plt.legend()
        plt.show()


class Row:
    def __init__(self, timestamp, p1, p2, p3, p4, p5, p6):
        self._timestamp = timestamp
        self._p1 = p1
        self._p2 = p2
        self._p3 = p3
        self._p4 = p4
        self._p5 = p5
        self._p6 = p6