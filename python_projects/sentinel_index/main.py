import rasterio
import numpy as np
from matplotlib import pyplot as plt
from rasterio import plot


def read_file(number):
    data = rasterio.open("T33TWM_20211009T095029_B" + number + ".jp2")
    return data.read(1)


def calculate_EVI(band_2, band_4, band_8):
    EVI = np.float32(2.5 * (band_8 - band_4) / ((band_8 + 6.0 * band_4 - 7.5 * band_2) + 1.0))
    return EVI


def main():
    band_2 = read_file("02")
    band_4 = read_file("04")
    band_8 = read_file("08")
    EVI = calculate_EVI(band_2, band_4, band_8)
    plt.imshow(EVI, clim=(-5, 5))
    plt.show()


if __name__ == '__main__':
    main()
