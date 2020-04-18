import copy
import os
import shutil
from pathlib import Path

import cv2
import imutils
from imutils import paths

from helper import my_face_detection, my_utils

def test_detect_full(dir, removed=False):
    imagePaths = list(paths.list_images(dir))
    print(len(imagePaths))
    listFailed = []
    failedDetectDir = "temp/failed_detect"
    for (i, imagePath) in enumerate(imagePaths):
        print("Try detect image {}/{}".format(i + 1, len(imagePaths)))
        try:
            image = cv2.imread(imagePath)
            # image = imutils.resize(image, width=400)
            image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        except:
            print(imagePath + "not ok")
            continue
        boxes = my_face_detection.face_locations(image)
        if len(boxes) != 1:
            print("FALSE - {} - Length: {}".format(imagePath, len(boxes)))
            listFailed.append(imagePath)

            Path(failedDetectDir).mkdir(parents=True, exist_ok=True)
            shutil.copy(imagePath, failedDetectDir)

            if (removed is True):
                os.remove(imagePath)
    print("Failed {}/{}".format(len(listFailed), len(imagePaths)))



def detectAndCrop(origin_dir, target_dir):
    imagePaths = list(paths.list_images(origin_dir))
    for (i, imagePath) in enumerate(imagePaths):
        # extract the person name from the image path
        print("[INFO] processing image {}/{}".format(i + 1,
                                                     len(imagePaths)))

        name = imagePath.split(cv2.os.path.sep)[-2]

        # load the image
        try:
            image = cv2.imread(imagePath)
            image = imutils.resize(image, width=400)
            imageRaw = copy.deepcopy(image)
            image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        except:
            print(imagePath + "Eror")
            continue

        boxes = my_face_detection.face_locations(image)
        if len(boxes) > 1:
            print(imagePath, "> 1")
            print(len(boxes))
            continue
        if len(boxes) == 0:
            print(imagePath, "= 0")
            print(len(boxes))
            continue
        pathimage = os.path.join(target_dir, name)
        Path(pathimage).mkdir(parents=True, exist_ok=True)
        my_utils.saveImageFunction(imageRaw, boxes[0], pathimage, 40)

detectAndCrop(r"rawImage", r"cropimage")
test_detect_full(r"cropimage", removed=True)