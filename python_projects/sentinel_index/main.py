import rasterio
import numpy as np
from matplotlib import pyplot as plt


def read_file(number):
    data = rasterio.open("T33TWM_20211009T095029_B" + number + ".jp2")
    return data.read(1)


def calculate_EVI(band_2, band_4, band_8):
    EVI = np.float32(2.5 * (band_8 - band_4) / ((band_8 + 6.0 * band_4 - 7.5 * band_2) + 1.0))
    return EVI


def calculate_NDVI(band_4, band_8):
    NDVI = np.float32((band_8 - band_4)/(band_8 + band_4))
    return NDVI


def calculate_GNDVI(band_3, band_8):
    NDVI = np.float32((band_8 - band_3)/(band_8 + band_3))
    return NDVI


def calculate_MSI(band_8, band_11):
    MSI = np.float32(band_11 / band_8)
    return MSI


def calculate_NDWI(band_3, band_11):
    NDWI = np.float32((band_3 - band_11) / (band_3 + band_11))
    return NDWI


def calculate_NDBI(band_8, band_11):
    NDBI = np.float32((band_11 - band_8) / (band_11 + band_8))
    return NDBI


def calculate_NDMI(band_8, band_9):
    NDMI = np.float32((band_9 - band_8)/(band_9 + band_8))
    return NDMI


def main():
    band_2 = read_file("02")
    band_3 = read_file("03")
    band_4 = read_file("04")
    band_8 = read_file("08")
    band_9 = read_file("09")
    band_9 = band_9.repeat(6, axis=0)
    band_9 = band_9.repeat(6, axis=1)
    band_11 = read_file("11")
    band_11 = band_11.repeat(2, axis=0)
    band_11 = band_11.repeat(2, axis=1)

    EVI = calculate_EVI(band_2, band_4, band_8)
    NDVI = calculate_NDVI(band_4, band_8)
    GNDVI = calculate_GNDVI(band_3, band_8)
    MSI = calculate_MSI(band_8, band_11)
    NDWI = calculate_NDWI(band_3, band_11)
    NDBI = calculate_NDBI(band_8, band_11)
    NDMI = calculate_NDMI(band_8, band_9)

    plt.title("EVI")
    plt.imshow(EVI, clim=(-1, 1))
    plt.show()
    plt.imsave("EVI.tiff", EVI, vmin=-1, vmax=1)

    plt.title("NDVI")
    plt.imshow(NDVI, clim=(-1, 1))
    plt.show()
    plt.imsave("NDVI.tiff", NDVI, vmin=-1, vmax=1)

    plt.title("GNDVI")
    plt.imshow(GNDVI, clim=(0, 1))
    plt.show()
    plt.imsave("GNDVI.tiff", GNDVI, vmin=0, vmax=1)

    plt.title("MSI")
    plt.imshow(MSI, clim=(0, 3))
    plt.show()
    plt.imsave("MSI.tiff", MSI, vmin=0, vmax=3)

    plt.title("NDWI")
    plt.imshow(NDWI, clim=(0, 50))
    plt.show()
    plt.imsave("NDWI.tiff", NDWI, vmin=0, vmax=50)

    plt.title("NDBI")
    plt.imshow(NDBI, clim=(0, 50))
    plt.show()
    plt.imsave("NDBI.tiff", NDBI, vmin=0, vmax=50)

    plt.title("NDMI")
    plt.imshow(NDMI, clim=(0, 50))
    plt.show()
    plt.imsave("NDMI.tiff", NDMI, vmin=0, vmax=50)


if __name__ == '__main__':
    main()
