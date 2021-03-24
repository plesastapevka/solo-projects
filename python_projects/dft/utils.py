import numpy as np
import matplotlib.pyplot as plt
from scipy.fft import fft, fftfreq
import constants


def generate_sine(freq, sample_rate=constants.SAMPLE_RATE, duration=constants.DURATION):
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
    N = int(constants.SAMPLE_RATE / constants.CHUNK * constants.DURATION) * constants.CHUNK
    yf = fft(np.frombuffer(y, np.int16))
    xf = fftfreq(N, 1 / constants.SAMPLE_RATE)
    plt.plot(xf, np.abs(yf))
    plt.xlabel("frequency (Hz)")
    plt.ylabel("amplitude (???)")
    plt.show()


def dft_impl(amplitudes):
    N = constants.SAMPLE_RATE * constants.DURATION
    ft_vals = []
    for k in range(int(len(amplitudes))):
        sum_re = 0
        sum_im = 0
        for n in range(N):
            sum_re += amplitudes[n] * np.cos(- (2*np.pi*k*n) / N)
            sum_im += -amplitudes[n] * np.sin(- (2*np.pi*k*n) / N)
        val = np.sqrt(np.power(sum_re, 2) + np.power(sum_im, 2))
        ft_vals.append(val)

    xf = fftfreq(N, 1 / constants.SAMPLE_RATE)
    plt.stem(xf, ft_vals)
    plt.show()
    return ft_vals
