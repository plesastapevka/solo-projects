import numpy as np
import matplotlib.pyplot as plt
from scipy import ndimage
import skimage
from skimage import measure


class procrustes_ransac():
    # oceni model
    def estimate(self, data):
        a = data[:, :2]
        b = data[:, 2:]
        self.R, self.t = resi_procrustes(a, b)

    # oceni napako
    def residuals(self, data):
        a = data[:, :2]
        b = data[:, 2:]
        c = a.dot(self.R) + self.t
        return np.sum((b - c) ** 2, 1) ** 0.5


def resi_procrustes(tocke_a, tocke_b):
    Pt_ = np.mean(tocke_a[:,:], axis=0, keepdims=True)  # povprecje za tocke_A
    Qt_ = np.mean(tocke_b[:,:], axis=0, keepdims=True)

    p = tocke_a - Pt_
    q = tocke_b - Qt_

    PtT = p.transpose()
    Q = q
    K = PtT.dot(Q)
    U, S, Vh = np.linalg.svd(K)

    delta = np.zeros((2, 2))
    delta[0][0] = 1
    delta[0][1] = 0
    delta[1][0] = 0

    VhT = Vh.transpose()
    UT = U.transpose()
    # VhTUT = VhT.dot(UT)
    determinant = np.linalg.det(VhT) * np.linalg.det(UT)
    delta[1][1] = determinant
    # delta[1][1] = np.linalg.det(VhTUT)
    Rt1 = U.dot(delta)
    Rt = Rt1.dot(Vh)
    tr = Qt_ - (Pt_.dot(Rt))

    return Rt, tr


def oceni_transformacijo_ICP(slika_A, slika_B, barve=False, ransac=False, max_itt=10):
    thresh = 200
    slika_A = np.uint8(plt.imread(slika_A) * 255)
    slika_B = np.uint8(plt.imread(slika_B) * 255)
    # ICP - izbira točk - za vsako sliko posebej
    mean1 = np.mean(slika_A[:, :, :3], 2)  # tu smo naredili sivinsko sliko
    mean2 = np.mean(slika_B[:, :, :3], 2)

    so1x = ndimage.sobel(mean1, 0, mode='mirror')  # s tem dobimo sliko robov
    so1y = ndimage.sobel(mean1, 1, mode='mirror')
    so1 = np.hypot(so1x, so1y)
    so2x = ndimage.sobel(mean2, 0, mode='mirror')  # s tem dobimo sliko robov
    so2y = ndimage.sobel(mean2, 1, mode='mirror')
    so2 = np.hypot(so2x, so2y)

    s1 = so1 > thresh  # pogledamo če je večje od threshholda in dobimo binarno sliko robov
    s2 = so2 > thresh

    t1 = s1.nonzero()  # poberemo ven samo točke ki niso 0, njihove indekse, s tem dobimo koordinate točk, ki so nonzero
    t2 = s2.nonzero()

    t1 = np.array(t1)  # !! podatki so v stolpcih, ne vrsticah
    t2 = np.array(t2)

    plt.imshow(slika_A)
    plt.plot(t1[1, :], t1[0, :], ".")

    D = np.zeros((t1.shape[1], t2.shape[1]))  # D je matrika razdalj
    final_R = np.identity(2)  # sem padejo 4 cifre, ki predstavljajo rotacijo v prostoru
    final_t = np.zeros((1, 2))  # translacija
    if barve:
        c1 = np.float32(slika_A[t1[0, :], t1[1, :], :3])
        c2 = np.float32(slika_B[t2[0, :], t2[1, :], :3])

    for iteration in range(max_itt):
        for i in range(D.shape[0]):
            if barve:
                dx = (t1[0, i] - t2[0, :]) / ((t1.shape[0] + t1.shape[1])/2)
                dy = (t1[1, i] - t2[1, :]) / ((t1.shape[0] + t1.shape[1])/2)

                dR = (c1[i, 0] - c2[:, 0]) / 255
                dG = (c1[i, 1] - c2[:, 1]) / 255
                dB = (c1[i, 2] - c2[:, 2]) / 255

                D[i, :] = dx*dx + dy*dy + dR*dR + dG*dG + dB*dB
            else:
                dx = t1[0, i] - t2[0, :]  # vektor razdalj
                dy = t1[1, i] - t2[1, :]
                D[i, :] = dx * dx + dy * dy

        DD = np.argmin(D, axis=1)  # poiščemo indekse najkrajših razdalij
        t2_ = t2[:, DD]  # to so točke iz t2, ki so najbližje točkam t1
        # IZRIS REZULTATOV SPROTI NA TEJ TOČKI

        if ransac:
            data = np.concatenate((t1.transpose(), t2_.transpose()), axis=1)
            model, inliers = skimage.measure.ransac(
                data,
                procrustes_ransac,  # metoda za ransac procrustesa
                min_samples=10,  # koliko jih jemljemo
                residual_threshold=4,  # dovoljena napaka
                max_trials=1000  # kolikokrat izbira točke
            )
            R, t = model.R, model.t
        else:
            R, t = resi_procrustes(t1.transpose(), t2_.transpose())

        # izris rezultatov
        t1 = (t1.transpose().dot(R) + t).transpose()
        plt.plot(t1[1, :], t1[0, :], '.')
        # konec izrisa

        final_R = final_R.dot(R)
        final_t = final_t.dot(R) + t
        # to1 = (t1.transpose().dot(final_R) + final_t).transpose()
        # plt.plot(to1[1, :], to1[0, :], '.')
    plt.show()
    return final_R, final_t


def transformiraj_kanal(kanal, Rf, tf):
    A = np.identity(3)
    A[:2, :2] = Rf[:, :]
    A[:2, 2] = tf[:, :]
    A[2, :2] = 0
    A[2, 2] = 1

    return ndimage.interpolation.affine_transform(kanal, A)


def transformiraj_sliko(slika, Rf, tf):
    tr = (-tf).dot(Rf.transpose())
    transformed = slika.copy()
    for i in range(4):
        transformed[:, :, i] = transformiraj_kanal(slika[:, :, i], Rf, tr)
    return transformed


# main:
# R, t = oceni_transformacijo_ICP("slika_1.png", "slika_2.png", ransac=True)
# img = np.uint8(plt.imread("slika_1.png") * 255)

R, t = oceni_transformacijo_ICP("primer1_1.png", "primer1_2.png", barve=False)
img = np.uint8(plt.imread("primer1_1.png") * 255)

# R, t = oceni_transformacijo_ICP("primer_preprost_3.png", "primer_preprost_4.png")
# img = np.uint8(plt.imread("primer_preprost_3.png") * 255)

# R, t = oceni_transformacijo_ICP("primer_zahteven_3.png", "primer_zahteven_4.png")
# img = np.uint8(plt.imread("primer_zahteven_3.png") * 255)

plt.figure()
plt.imshow(img)
plt.show()

newimg = transformiraj_sliko(img, R, t)

plt.figure()
plt.imshow(newimg)
plt.show()
