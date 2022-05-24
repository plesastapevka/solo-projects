import scipy.ndimage as ndimage
import matplotlib.pyplot as plt
import numpy as np

import tensorflow as tf
import tensorflow.python.keras as keras

import os

window = (256, 256)
sample_per_image = 800

# creating a mask
diag = int((2*window[0]**2+2*window[1]**2)**0.5+0.5)
int_mask = np.ones((diag, diag, 1))
int_mask[diag//2:, :] += 1
int_mask[:, diag//2:] += 2
int_mask = int_mask/2
int_mask = ndimage.gaussian_filter(int_mask, 2.0)

current_wd = 'samples_raw/'
out_wd = 'samples/'

file_count = 0
print('Generating samples')
# iterate through all the samples ready for augmentation
for filename in os.listdir(current_wd):
    print('file:', filename)
    try:
        os.mkdir(path=out_wd + filename.split('.')[0])
    except:
        pass
    for i in range(sample_per_image):
        # choose random point in the image
        image = plt.imread(current_wd + filename) / 255
        x = np.random.randint(0, image.shape[0] - window[0])
        y = np.random.randint(0, image.shape[1] - window[1])
        sample = image[x:x+window[0], y:y+window[1]]
        # random rotation
        rotation = np.random.randint(0, 360)
        int_mask_rot = ndimage.rotate(int_mask, rotation, reshape=False)
        # cut out the mask
        top = (diag-window[0])//2
        left = (diag-window[1])//2
        top_shift = np.random.randint(0, window[0] / 2)
        left_shift = np.random.randint(0, window[1] / 2)
        int_mask_rot_cut = int_mask_rot[top + top_shift:top + window[0] + top_shift, left + left_shift:left + window[1] + left_shift]

        v1 = 0.54 # nakljucni faktor, ~0.5
        v2 = 1.1 # nakljucni faktor, ~1.0
        v3 = 1.93 # nakljucni faktor, ~2.0

        sample1 = sample*int_mask_rot_cut*v1
        sample2 = sample*int_mask_rot_cut*v2
        sample3 = sample*int_mask_rot_cut*v3

        # appropriately clip the pixel values
        sample1 = np.uint8(np.maximum(np.minimum(sample1, 1), 0)*255)/255.
        sample2 = np.uint8(np.maximum(np.minimum(sample2, 1), 0)*255)/255.
        sample3 = np.uint8(np.maximum(np.minimum(sample3, 1), 0)*255)/255.

        plt.imsave(out_wd + filename.split('.')[0] + '/' + str(i) + '_0.png', sample)
        plt.imsave(out_wd + filename.split('.')[0] + '/' + str(i) + '_1.png', sample1)
        plt.imsave(out_wd + filename.split('.')[0] + '/' + str(i) + '_2.png', sample2)
        plt.imsave(out_wd + filename.split('.')[0] + '/' + str(i) + '_3.png', sample3)
    file_count += 1

print('Done')