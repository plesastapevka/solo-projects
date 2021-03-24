import numpy as np
import matplotlib.pyplot as plt
from scipy.fft import fft, fftfreq

SAMPLE_RATE = 44100
DURATION = 5


def generate_sine(freq, sample_rate=SAMPLE_RATE, duration=DURATION):
    x = np.linspace(0, duration, sample_rate * duration, endpoint=False)
    frequencies = x * freq
    y = np.sin((2 * np.pi) * frequencies)
    return x, y


def plot_frames(frames):
    fig = plt.figure()
    sub = fig.add_subplot(111)
    amplitude = np.frombuffer(frames, np.int16)
    sub.plot(amplitude)
    plt.show()
    fig.savefig('sinusoid.png')


def dft(x, y):
    N = SAMPLE_RATE * DURATION
    yf = fft(y)
    xf = fftfreq(N, 1 / SAMPLE_RATE)
    plt.plot(xf, np.abs(yf))
    plt.show()