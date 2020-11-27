import numpy as np
import math
import matplotlib.pyplot as plt
from skimage import measure, draw
from scipy import ndimage
from decimal import *


def shoelace(contours, min_povrsina):
    x = 0
    y = 1
    output = []

    for contour in contours:
        result = 0
        for i in range(len(contour) - 1):
            result += contour[i][x] * contour[i + 1][y]  # seštevalni del
        result += contour[len(contour) - 1][x] * contour[0][y]

        for i in range(len(contour) - 1):
            result -= contour[i][y] * contour[i + 1][x]  # odštevalni del

        result -= contour[len(contour) - 1][y] * contour[0][x]
        povrsina = result/2

        if povrsina > min_povrsina:
            output.append(contour)

    return output


def poisci_konture(slika, min_povrsina):
    slika = slika/255.
    gx = ndimage.sobel(slika, 0, mode='mirror')  # dobimo dve sliki
    gy = ndimage.sobel(slika, 1, mode='mirror')
    G = np.hypot(gx, gy)
    binary = G > 0.8

    contours = measure.find_contours(binary, 0)
    nove_konture = shoelace(contours, min_povrsina)  # SHOELACE

    # plt.plot(nove_konture)
    # plt.show()

    return nove_konture


def surov_moment(x, y, i, j):
    moment = 0.
    for k in np.arange(len(x)):
        moment += y[k] ** i * x[k] ** j
    return moment


def analiziraj_konture(contours):
    output = []
    for contour in contours:
        kontura_info = []
        a, b = draw.polygon(contour[:, 0], contour[:, 1])

        # ax.plot(a, b, linewidth=2)  # visual draw
        # plt.show()

        # surovi momenti
        M00 = surov_moment(a, b, 0, 0)
        M01 = surov_moment(a, b, 0, 1)
        M10 = surov_moment(a, b, 1, 0)
        M20 = surov_moment(a, b, 2, 0)
        M11 = surov_moment(a, b, 1, 1)
        M02 = surov_moment(a, b, 0, 2)
        M30 = surov_moment(a, b, 3, 0)
        M21 = surov_moment(a, b, 2, 1)
        M12 = surov_moment(a, b, 1, 2)
        M03 = surov_moment(a, b, 0, 3)

        # center
        x_bar = M10/M00
        y_bar = M01/M00

        # centralni momenti
        u00 = M00
        u01 = 0
        u10 = 0
        u11 = M11 - x_bar * M01
        u20 = M20 - x_bar * M10
        u02 = M02 - y_bar * M01
        u30 = M30 - 3 * x_bar * M20 + 2 * (x_bar**2) * M10
        u21 = M21 - 2 * x_bar * M11 - y_bar * M20 + 2 * (x_bar**2) * M01
        u12 = M12 - 2 * y_bar * M11 - x_bar * M02 + 2 * (y_bar**2) * M10
        u03 = M03 - 3 * y_bar * M02 + 2 * (y_bar**2) * M01

        # preračunani centralni momenti
        u20_ = u20/u00
        u11_ = u11/u00
        u02_ = u02/u00

        theta = (np.arctan((2 * u11_)/(u20_ - u02_)))/2

        if u20_ < u02_:
            theta -= np.pi / 2

        lambda1 = ((u20_ + u02_) / 2) + ((math.sqrt(4 * (u11_ ** 2) + ((u20_ - u02_) ** 2))) / 2)
        lambda2 = ((u20_ + u02_) / 2) - ((math.sqrt(4 * (u11_ ** 2) + ((u20_ - u02_) ** 2))) / 2)

        l_1 = (100 * lambda1) / (lambda1 + lambda2)
        l_2 = (100 * lambda2) / (lambda1 + lambda2)

        a = math.cos(-theta)
        b = -(math.sin(-theta))

        u30_ = (a**3) * u30 + 3 * (a**2) * b * u21 + 3 * a * (b**2) * u12 + (b**3) * u03

        if u30_ < 0:
            theta -= np.pi

        # cy, cx, l1, l2, theta
        kontura_info.append(y_bar)
        kontura_info.append(x_bar)
        kontura_info.append(l_1)
        kontura_info.append(l_2)
        kontura_info.append(np.float64(theta))

        output.append(kontura_info)

    return output


def imgread(ime):
    slika = plt.imread(ime)
    if slika.dtype != np.uint8:
        slika = np.uint8(slika * 255)
    if slika.ndim == 3:
        slika = slika[:, :, 1]

    return slika


slika = imgread("./primer4.png")

nove_konture = poisci_konture(slika, 1000)
final_data = analiziraj_konture(nove_konture)

slika = slika/255.
gx = ndimage.sobel(slika, 0, mode='mirror')  # dobimo dve sliki
gy = ndimage.sobel(slika, 1, mode='mirror')
G = np.hypot(gx, gy)
binary = G > 0.8

# 0 cy
# 1 cx
# 2 l1
# 3 l2
# 4 theta
for c in final_data:
    x_bar = c[1]
    y_bar = c[0]
    lambda1 = c[2]
    lambda2 = c[3]
    theta = c[4]
    dx1 = np.cos(theta) * lambda1
    dy1 = np.sin(theta) * lambda1
    plt.plot([x_bar, x_bar + dx1], [y_bar, y_bar + dy1], 'o-')

    dx2 = np.cos(theta - np.pi/2) * lambda2
    dy2 = np.sin(theta - np.pi/2) * lambda2
    plt.plot([x_bar, x_bar + dx2], [y_bar, y_bar + dy2], 'o-')

    if math.degrees(-theta) < 0:
        theta_to_display = 360 + int(math.degrees(-theta))
        plt.text(x_bar, y_bar, theta_to_display, color='b')
    else:
        plt.text(x_bar, y_bar, int(math.degrees(-theta)), color='r')

plt.gray()
plt.imshow(slika)
plt.show()