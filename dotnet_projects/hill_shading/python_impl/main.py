from PIL import Image
import numpy as np
import matplotlib.pyplot as plt
import math

"""
def hillshade(array, azimuth, zenith, imgArr, height, width):
    x, y = np.gradient(array)

    slope = np.arctan(np.sqrt(x * x + y * y))
    aspect = np.arctan2(y, -x)
    # shaded = np.sin(zenith) * np.sin(slope) \
    #          + np.cos(zenith) * np.cos(slope) \
    #          * np.cos(azimuth - aspect)
    shaded = np.cos(-zenith) * np.cos(slope) + (np.cos(azimuth - aspect) * np.sin(-zenith) * np.sin(slope))
    print (shaded.shape)
    return shaded
"""

def hillshade(array, azimuth, zenith, imgArr, height, width):
    azimuth = math.radians(azimuth)
    zenith = math.radians(zenith)
    # print (array.shape)
    # x, y = np.gradient(array)

    for j in range(1, height-1):
        for i in range(1, width-1):
            x = ((array[j + 1][i - 1] + 2*array[j + 1][i] + array[j + 1][i + 1]) - (array[j - 1][i - 1] + 2*array[j - 1][i] + array[j - 1][i + 1])) / 8
            y = ((array[j - 1][i + 1] + 2*array[j][i + 1] + array[j + 1][i + 1]) - (array[j - 1][i - 1] + 2*array[j][i - 1] + array[j + 1][i - 1])) / 8
            slope = max(np.arctan(np.sqrt(pow(x, 2) + pow(y, 2))), 0)
            aspect = 0
            if slope > 0:
                aspect = np.mod(np.arctan2(y, -x) + 2*np.pi, 2*np.pi)
            else:
                if y > 0:
                    aspect = np.pi / 2
                else:
                    aspect = np.pi * 3 / 2
            imgArr[j][i] = np.cos(-zenith) * np.cos(slope) + (np.cos(azimuth - aspect) * np.sin(-zenith) * np.sin(slope))

    return imgArr


im = Image.open('mb-center_vhod.tif')
width, height = im.size
arr = np.asarray(im)
array = np.zeros([height, width])
print ("WIDTH ", width, " HEIGHT ", height)

hs_array = hillshade(arr, 230, 38, array, height, width)
print(hs_array)
plt.imshow(hs_array, cmap='Greys_r')
plt.show()