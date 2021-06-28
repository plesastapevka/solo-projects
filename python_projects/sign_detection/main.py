import time
import cv2
import matplotlib.pyplot as plt
import numpy as np
import scipy.ndimage as ndimage
import tensorflow as tf
from mlxtend.plotting import plot_confusion_matrix
from sklearn.metrics import confusion_matrix
from tensorflow.keras.utils import to_categorical


def compile_model(optimizer, shape=(32, 32, 3)):
    model = tf.keras.Sequential([
        tf.keras.layers.Conv2D(filters=6, kernel_size=(5, 5), strides=(1, 1), padding='valid', input_shape=shape,
                               activation='relu'),
        tf.keras.layers.MaxPooling2D((2, 2), strides=(2, 2)),
        tf.keras.layers.Conv2D(filters=16, kernel_size=(5, 5), strides=(1, 1), activation='relu'),
        tf.keras.layers.MaxPool2D((2, 2), strides=(2, 2)),
        tf.keras.layers.Flatten(),
        tf.keras.layers.Dense(120, activation='relu'),
        tf.keras.layers.Dense(80, activation='relu'),
        tf.keras.layers.Dense(43),
    ])

    model.compile(optimizer=optimizer,
                  loss=tf.keras.losses.CategoricalCrossentropy(from_logits=True),
                  metrics=['accuracy'])

    return model


def train_model(model, x_train, y_train, x_validation, y_validation, epochs=20):
    start = time.time()
    history = model.fit(x_train, y_train, epochs=epochs, validation_data=(x_validation, y_validation), batch_size=32)
    end = time.time()
    print("Model trained in " + str(end - start) + "s")

    return model


def test_model(model, test):
    start = time.time()
    predictions = model.predict(test)
    end = time.time()
    print("Test time " + str(end - start) + "s")

    return predictions


def build_confusion_matrix(predictions, test, labels):
    prediction_res = []
    real_res = []
    for i in range(len(predictions)):
        prediction_res.append(np.argmax(predictions[i]))
        real_res.append(np.argmax(test[i]))
    mat = confusion_matrix(prediction_res, real_res)
    plot_confusion_matrix(conf_mat=mat, figsize=(15, 15), class_names=labels)
    plt.show()


def distort_gauss(data, sigma):
    retval = []
    for e in data:
        retval.append(ndimage.gaussian_filter(e, sigma=sigma))
    return np.array(retval)


def add_dimension(data):
    converted = []
    for p in data:  # for every picture
        x_dim = []
        for e in p:  # for every X
            y_dim = []
            for dim in e:  # for every Y
                z_dim = [dim]
                y_dim.append(z_dim)
            x_dim.append(y_dim)
        converted.append(x_dim)
    print("Grayscale conversion done")
    return np.array(converted)


def distort_grayscale(train, validation):
    grayscale_train = []
    grayscale_validation = []
    for i in range(len(train)):
        grayscale_train.append(cv2.cvtColor(np.float32(train[i]), cv2.COLOR_BGR2GRAY))
    for i in range(len(validation)):
        grayscale_validation.append(cv2.cvtColor(np.float32(validation[i]), cv2.COLOR_BGR2GRAY))
    grayscale_train = np.array(grayscale_train)
    grayscale_validation = np.array(grayscale_validation)

    train_ret = add_dimension(grayscale_train)
    validation_ret = add_dimension(grayscale_validation)

    return train_ret, validation_ret


def main():
    # LOAD AND EXTRACT DATA
    print("Loading data ...")
    test_data = np.load('data/test.npz')
    train_data = np.load('data/train.npz')
    validation_data = np.load('data/validation.npz')
    unique_labels = np.unique(train_data['labels'])
    print(unique_labels)
    num_cat = len(unique_labels)

    insp_test = 0
    x_test = test_data['images']
    x_test = x_test / 255.0
    y_test = test_data['labels']
    print("Category: " + str(y_test[insp_test]))
    plt.imshow(x_test[insp_test])
    plt.show()

    insp_train = 0
    x_train = train_data['images']
    x_train = x_train / 255.0
    y_train = train_data['labels']
    print("Category: " + str(y_train[insp_train]))
    plt.imshow(x_train[insp_train])
    plt.show()

    insp_validation = 0
    x_validation = validation_data['images']
    x_validation = x_validation / 255.0
    y_validation = validation_data['labels']
    print("Category: " + str(y_validation[insp_validation]))
    plt.imshow(x_validation[insp_validation])
    plt.show()

    y_test = to_categorical(y_test, num_cat)
    y_train = to_categorical(y_train, num_cat)
    y_validation = to_categorical(y_validation, num_cat)

    print("Data loaded.")

    # FIRST MODEL
    print("MODEL 1 without distortion")
    model_1 = compile_model("adam")
    model_1 = train_model(model_1, x_train, y_train, x_validation, y_validation)
    pred = test_model(model_1, x_test)
    build_confusion_matrix(pred, y_test, unique_labels)

    # SECOND MODEL
    print("MODEL 2 without distortion")
    model_2 = compile_model(tf.keras.optimizers.Adam(learning_rate=0.009))
    model_2 = train_model(model_2, x_train, y_train, x_validation, y_validation)
    pred = test_model(model_2, x_test)
    build_confusion_matrix(pred, y_test, unique_labels)

    # THIRD MODEL
    print("MODEL 3 without distortion")
    model_3 = compile_model(tf.keras.optimizers.Adam(learning_rate=0.0001))
    model_3 = train_model(model_3, x_train, y_train, x_validation, y_validation)
    pred = test_model(model_3, x_test)
    build_confusion_matrix(pred, y_test, unique_labels)

    # DISTORTION WITH GAUSS
    print("Showing original image")
    plt.title('original')
    plt.imshow(x_test[0])

    print("Distorting images with Gaussian Blur")
    data_1 = distort_gauss(x_test, 1)
    plt.title('1x1 gaussian')
    plt.imshow(data_1[0])
    plt.show()

    data_1_5 = distort_gauss(x_test, 1.5)
    plt.title('1.5x1.5 gaussian')
    plt.imshow(data_1_5[0])
    plt.show()

    data_2 = distort_gauss(x_test, 2)
    plt.title('2x2 gaussian')
    plt.imshow(data_2[0])
    plt.show()

    data_2_5 = distort_gauss(x_test, 2.5)
    plt.title('2.5x2.5 gaussian')
    plt.imshow(data_2_5[0])
    plt.show()

    print("MODEL 1 with distorted images - Gaussian Blur")
    model_1 = compile_model("adam")
    model_1 = train_model(model_1, x_train, y_train, x_validation, y_validation)
    # 1x1 gauss test
    pred = test_model(model_1, data_1)
    build_confusion_matrix(pred, y_test, unique_labels)

    # 1.5x1.5 gauss test
    pred = test_model(model_1, data_1_5)
    build_confusion_matrix(pred, y_test, unique_labels)

    # 2x2 gauss test
    pred = test_model(model_1, data_2)
    build_confusion_matrix(pred, y_test, unique_labels)

    # 2.5x2.5 gauss test
    pred = test_model(model_1, data_2_5)
    build_confusion_matrix(pred, y_test, unique_labels)

    # DISTORTION WITH GRAYSCALE
    print("Distorting data with grayscale")
    grayscale_train, grayscale_validation = distort_grayscale(x_train, x_validation)
    print("MODEL 4 with distorted images - Grayscale")
    model_4 = compile_model("adam", (32, 32, 1))
    model_4 = train_model(model_4, grayscale_train, y_train, grayscale_validation, y_validation)

    # TEST FOR SINGLE IMAGE
    print("Starting tests for single image")
    single_x_train = np.array([x_train[0]])
    single_y_train = np.array([y_train[0]])

    single_x_validation = np.array([x_validation[0]])
    single_y_validation = np.array([y_validation[0]])

    single_x_test = np.array([x_test[0]])
    single_y_test = np.array([y_test[0]])

    # FIRST MODEL WITH SINGLE IMAGE
    print("MODEL 1 with single image")
    model_1 = train_model(model_1, single_x_train, single_y_train, single_x_validation, single_y_validation, epochs=1)
    pred = test_model(model_1, single_x_test)


if __name__ == "__main__":
    main()
