import numpy as np
import math as math


def bayer_v_rgb(slika_bayer, vzorec, interpoliraj=False):
    if vzorec == "BGGR":
        slika_bayer = slika_bayer[::-1, ::-1]  # prezrcalimo v prefered RGGB

        if interpoliraj:
            output = interpolacija(slika_bayer)
        else:
            output = decimacija(slika_bayer)

        output = output[::-1, ::-1, ::]  # nazaj v določen vzorec
        return output

    elif vzorec == "GBRG":
        slika_bayer = slika_bayer[::-1, ::]

        if interpoliraj:
            output = interpolacija(slika_bayer)
        else:
            output = decimacija(slika_bayer)

        output = output[::-1, ::, ::]
        return output

    elif vzorec == "GRBG":
        slika_bayer = slika_bayer[::, ::-1]

        if interpoliraj:
            output = interpolacija(slika_bayer)
        else:
            output = decimacija(slika_bayer)

        output = output[::, ::-1, ::]

        return output


    else:
        if interpoliraj:
            output = interpolacija(slika_bayer)
        else:
            output = decimacija(slika_bayer)

        return output



def interpolacija(slika_bayer):
    slika_bayer = slika_bayer.astype(np.uint16)  # type v uint16

    # init izhodne slike, velikost WxHx3
    slika_rgb = np.zeros((int(slika_bayer.shape[0]), int(slika_bayer.shape[1]), 3))

    # začetek notranjega dela

    # notranji modri
    slika_rgb[1:-1:2, 1:-1:2, 0] = math.floor((slika_bayer[:-2:2, :-2:2] + slika_bayer[:-2:2, 2::2] + slika_bayer[2::2, :-2:2] + slika_bayer[2::2, 2::2]) / 4)
    slika_rgb[1:-1:2, 1:-1:2, 1] = math.floor((slika_bayer[0:-2:2, 1:-1:2] + slika_bayer[1:-1:2, 0:-2:2] + slika_bayer[1:-1:2, 2::2] + slika_bayer[2::2, 1:-1:2]) / 4)
    slika_rgb[1:-1:2, 1:-1:2, 2] = slika_bayer[1:-1:2, 1:-1:2]

    # notranji zeleni desni
    slika_rgb[1:-1:2, 2::2, 0] = (slika_bayer[0:-2:2, 2::2] + slika_bayer[2::2, 2::2]) / 2
    slika_rgb[1:-1:2, 2::2, 1] = slika_bayer[1:-1:2, 2::2]
    slika_rgb[1:-1:2, 2::2, 2] = (slika_bayer[1:-1:2, 1:-1:2] + slika_bayer[1:-1:2, 3::2]) / 2

    # notranji zeleni levi
    slika_rgb[2::2, 1:-1:2, 0] = (slika_bayer[2::2, :-2:2] + slika_bayer[2::2, 2::2]) / 2
    slika_rgb[2::2, 1:-1:2, 1] = slika_bayer[2::2, 1:-1:2]
    slika_rgb[2::2, 1:-1:2, 2] = (slika_bayer[1:-1:2, 1:-1:2] + slika_bayer[3::2, 1:-1:2]) / 2

    # notranji rdeči
    slika_rgb[2::2, 2::2, 0] = slika_bayer[2::2, 2::2]
    slika_rgb[2::2, 2::2, 1] = math.floor((slika_bayer[1:-1:2, 2::2] + slika_bayer[3::2, 2::2] + slika_bayer[2::2, 1:-1:2] + slika_bayer[2::2, 3::2]) / 4)
    slika_rgb[2::2, 2::2, 2] = math.floor((slika_bayer[1:-1:2, 1:-1:2] + slika_bayer[1:-1:2, 3::2] + slika_bayer[3::2, 1:-1:2] + slika_bayer[3::2, 3::2]) /4)

    # konec notranjega dela
    # začetek robov

    # prva vrstica zelen
    slika_rgb[0, 1:-1:2, 0] = (slika_bayer[0, 0:-2:2] + slika_bayer[0, 2::2]) / 2
    slika_rgb[0, 1:-1:2, 1] = slika_bayer[0, 1:-1:2]
    slika_rgb[0, 1:-1:2, 2] = slika_bayer[1, 1:-1:2]

    # prva vrstica rdeč
    slika_rgb[0, 2::2, 0] = slika_bayer[0, 2::2]
    slika_rgb[0, 2::2, 1] = math.floor((slika_bayer[0, 1:-1:2] + slika_bayer[0, 3::2] + slika_bayer[1, 2::2]) / 3)
    slika_rgb[0, 2::2, 2] = (slika_bayer[1, 1:-1:2] + slika_bayer[1, 3::2]) / 2

    # levi rob zelen
    slika_rgb[1:-1:2, 0, 0] = (slika_bayer[0:-2:2, 0] + slika_bayer[2::2, 0]) / 2
    slika_rgb[1:-1:2, 0, 1] = slika_bayer[1:-1:2, 0]
    slika_rgb[1:-1:2, 0, 2] = slika_bayer[1:-1:2, 1]

    # levi rob rdeč
    slika_rgb[2::2, 0, 0] = slika_bayer[2::2, 0]
    slika_rgb[2::2, 0, 1] = math.floor((slika_bayer[1:-1:2, 0] + slika_bayer[3::2, 0] + slika_bayer[2::2, 1]) / 3)
    slika_rgb[2::2, 0, 2] = (slika_bayer[1:-1:2, 1] + slika_bayer[3::2, 1]) / 2

    # desni rob moder
    slika_rgb[1:-1:2, -1, 0] = (slika_bayer[:-2:2, -2] + slika_bayer[2::2, -2]) / 2
    slika_rgb[1:-1:2, -1, 1] = math.floor((slika_bayer[:-2:2, -1] + slika_bayer[2::2, -1] + slika_bayer[1:-1:2, -2]) / 3)
    slika_rgb[1:-1:2, -1, 2] = slika_bayer[1:-1:2, -1]

    # desni rob zelen
    slika_rgb[2::2, -1, 0] = slika_bayer[2::2, -2]
    slika_rgb[2::2, -1, 1] = slika_bayer[2::2, -1]
    slika_rgb[2::2, -1, 2] = (slika_bayer[1:-1:2, -1] + slika_bayer[3::2, -1]) / 2

    # zadnja vrstica moder
    slika_rgb[-1, 1:-1:2, 0] = (slika_bayer[-2, :-2:2] + slika_bayer[-2, 2::2]) / 2
    slika_rgb[-1, 1:-1:2, 1] = math.floor((slika_bayer[-1, :-2:2] + slika_bayer[-1, 2::2] + slika_bayer[-2, 1:-1:2]) / 3)
    slika_rgb[-1, 1:-1:2, 2] = slika_bayer[-1, 1:-1:2]

    # zadnja vrstica zelen
    slika_rgb[-1, 2::2, 0] = slika_bayer[-2, 2::2]
    slika_rgb[-1, 2::2, 1] = slika_bayer[-1, 2::2]
    slika_rgb[-1, 2::2, 2] = (slika_bayer[-1, 1:-1:2] + slika_bayer[-1, 3::2]) / 2

    # konec robov
    # začetek kotnih

    # kotni rdeč
    slika_rgb[0, 0, 0] = slika_bayer[0, 0]
    slika_rgb[0, 0, 1] = (slika_bayer[0, 1] + slika_bayer[1, 0]) / 2
    slika_rgb[0, 0, 2] = slika_bayer[1, 1]

    # kotni zelen desni
    slika_rgb[0, -1, 0] = slika_bayer[0, -2]
    slika_rgb[0, -1, 1] = slika_bayer[0, -1]
    slika_rgb[0, -1, 2] = slika_bayer[1, -1]

    # kotni zelen levi
    slika_rgb[-1, 0, 0] = slika_bayer[-2, 0]
    slika_rgb[-1, 0, 1] = slika_bayer[-1, 0]
    slika_rgb[-1, 0, 2] = slika_bayer[-1, 1]

    # kotni moder
    slika_rgb[-1, -1, 0] = slika_bayer[-2, -2]
    slika_rgb[-1, -1, 1] = (slika_bayer[-1, -2] + slika_bayer[-2, -1]) / 2
    slika_rgb[-1, -1, 2] = slika_bayer[-1, -1]

    # konec kotnih

    slika_rgb = slika_rgb.astype(np.uint8)  # type v uint8

    return slika_rgb


def decimacija(slika_bayer):
    slika_bayer = slika_bayer.astype(np.uint16)

    r = slika_bayer[::2, ::2]
    g1 = slika_bayer[::2,1::2]
    g2 = slika_bayer[1::2,::2]
    b = slika_bayer[1::2,1::2]

    slika_rgb = np.zeros((int(slika_bayer.shape[0]/2), int(slika_bayer.shape[1]/2), 3))

    slika_rgb[:,:,0] = r
    slika_rgb[:,:,1] = (g1 + g2)/2
    slika_rgb[:,:,2] = b

    slika_rgb = slika_rgb.astype(np.uint8)

    return slika_rgb
