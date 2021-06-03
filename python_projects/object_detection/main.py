import numpy as np
import tensorflow as tf
import pathlib
import cv2
from object_detection.utils import label_map_util
from object_detection.utils import visualization_utils as vis_util

PATH_TO_LABELS = 'object_detection/data/mscoco_label_map.pbtxt'
MODEL_NAME = 'ssd_inception_v2_coco_2017_11_17'


def load_model(model_name):
    url = 'http://download.tensorflow.org/models/object_detection/'
    file = model_name + '.tar.gz'
    directory = tf.keras.utils.get_file(
        fname=model_name,
        origin=url + file,
        untar=True)

    directory = pathlib.Path(directory) / "saved_model"

    model = tf.saved_model.load(str(directory))

    return model


CATEGORY_INDEX = label_map_util.create_category_index_from_labelmap(PATH_TO_LABELS, use_display_name=True)
DETECTION_MODEL = load_model(MODEL_NAME)


def detect(model, image):
    image = np.asarray(image)
    input_tensor = tf.convert_to_tensor(image)
    input_tensor = input_tensor[tf.newaxis, ...]

    # Run inference
    model_fn = model.signatures['serving_default']
    output_dict = model_fn(input_tensor)

    # We only need first num_detections
    num_detections = int(output_dict.pop('num_detections'))
    output_dict = {key: value[0, :num_detections].numpy()
                   for key, value in output_dict.items()}
    output_dict['num_detections'] = num_detections

    # Convert detection_classes to ints
    output_dict['detection_classes'] = output_dict['detection_classes'].astype(np.int64)

    return output_dict


def detect_objects(model, image):
    # Open image as an array
    image_np = np.array(image)

    output_dict = detect(model, image_np)
    # Visualization of the results of a detection.
    vis_util.visualize_boxes_and_labels_on_image_array(
        image_np,
        output_dict['detection_boxes'],
        output_dict['detection_classes'],
        output_dict['detection_scores'],
        CATEGORY_INDEX,
        instance_masks=output_dict.get('detection_masks_reframed', None),
        use_normalized_coordinates=True,
        line_thickness=8)

    return image_np


def main():
    test_video = False
    if test_video:
        cap = cv2.VideoCapture("videos/test_video.mp4")
    else:
        cap = cv2.VideoCapture(0)

    # Check if video can be read
    if not cap.isOpened():
        raise IOError("Cannot read video feed")

    while True:
        ret, frame = cap.read()
        frame = cv2.resize(frame, None, fx=0.5, fy=0.5, interpolation=cv2.INTER_AREA)
        frame = detect_objects(DETECTION_MODEL, frame)
        cv2.imshow('Input', frame)

        c = cv2.waitKey(1)  # Esc to quit
        if c == 27:
            break

    cap.release()
    cv2.destroyAllWindows()


if __name__ == "__main__":
    main()
