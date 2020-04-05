import os
import pickle
import shutil
import time
import cv2
import imutils
import numpy as np
from imutils import paths
from sklearn import svm
from sklearn.preprocessing import LabelEncoder

from config import my_constant
from helper import my_face_detection, my_face_recognition, my_face_generator


def recognize_image(imagePath, threshold=0):
    image = cv2.imread(imagePath)
    return recognize_image_after_read(image, threshold)


def recognize_image_after_read(image, threshold=0, alignFace=False):
    boxes = my_face_detection.face_locations(image)
    if len(boxes) == 1:
        if (alignFace == True):
            aligned_image = my_face_detection.align_face(image, boxes[0])
            vecs = my_face_recognition.face_encodings(aligned_image)
        else:
            vecs = my_face_recognition.face_encodings(image, boxes)
        vec = vecs[0]
        name, proba = _get_label(vec, threshold)
        return boxes[0], name, proba
    return None


def _get_label(vec, threshold=0):
    recognizer_model = pickle.loads(open(my_constant.recognizerModelPath, "rb").read())
    recognizer = recognizer_model["recognizer"]
    le = recognizer_model["le"]

    preds = recognizer.predict_proba([vec])[0]
    j = np.argmax(preds)
    proba = preds[j]
    name = le.classes_[j]
    if (proba > threshold):
        return name, proba
    return "Unknown", None


def generate_more_embeddings(datasetPath, outputDir, alignFace=False):
    imagePaths = list(paths.list_images(datasetPath))
    data = pickle.loads(open(my_constant.embeddingsPath, "rb").read())
    knownEmbeddings = data["embeddings"]
    knownNames = data["names"]
    totalAdded = 0
    for (i, imagePath) in enumerate(imagePaths):
        # extract the person name from the image path
        print("[INFO] processing image {}/{}".format(i + 1,
                                                     len(imagePaths)))

        name = imagePath.split(cv2.os.path.sep)[-2]

        # load the image
        image = cv2.imread(imagePath)
        boxes = my_face_detection.face_locations(image)
        if len(boxes) > 1:
            print(imagePath, "> 1")
            print(len(boxes))
            continue
        if len(boxes) == 0:
            print(imagePath, "= 0")
            print(len(boxes))
            continue
        if (alignFace == True):
            aligned_image = my_face_detection.align_face(image, boxes[0])
            vecs = my_face_recognition.face_encodings(aligned_image)
        else:
            vecs = my_face_recognition.face_encodings(image, boxes)
        vec = vecs[0]
        knownEmbeddings.append(vec.flatten())
        knownNames.append(name)
        totalAdded += 1

    # dump the facial embeddings + names to disk
    print("[INFO] serializing more {} encodings...".format(totalAdded))
    print("[INFO] serializing total {} encodings...".format(len(knownEmbeddings)))
    data = {"embeddings": knownEmbeddings, "names": knownNames}
    path = os.path.join(outputDir, my_constant.embeddingsPath)
    f = open(path, "wb+")
    f.write(pickle.dumps(data))
    f.close()


def generate_embeddings(datasetPath, outputDir, alignFace=False):
    imagePaths = list(paths.list_images(datasetPath))
    knownEmbeddings = []
    knownNames = []
    totalAdded = 0
    for (i, imagePath) in enumerate(imagePaths):
        # extract the person name from the image path
        print("[INFO] processing image {}/{}".format(i + 1,
                                                     len(imagePaths)))

        name = imagePath.split(cv2.os.path.sep)[-2]

        # load the image
        image = cv2.imread(imagePath)
        boxes = my_face_detection.face_locations(image)
        if len(boxes) > 1:
            print(imagePath, "> 1")
            print(len(boxes))
            continue
        if len(boxes) == 0:
            print(imagePath, "= 0")
            print(len(boxes))
            continue
        if (alignFace == True):
            aligned_image = my_face_detection.align_face(image, boxes[0])
            vecs = my_face_recognition.face_encodings(aligned_image)
        else:
            vecs = my_face_recognition.face_encodings(image, boxes)
        vec = vecs[0]

        knownEmbeddings.append(vec.flatten())
        knownNames.append(name)
        totalAdded += 1

    # dump the facial embeddings + names to disk
    print("[INFO] serializing total {} encodings...".format(totalAdded))
    data = {"embeddings": knownEmbeddings, "names": knownNames}
    path = os.path.join(outputDir, my_constant.embeddingsPath)
    os.makedirs(os.path.dirname(path), exist_ok=True)
    f = open(path, "wb+")
    f.write(pickle.dumps(data))
    f.close()


def generate_train_model(outputDir):
    print("[INFO] loading face embeddings...")
    embeddingsPath = os.path.join(outputDir, my_constant.embeddingsPath)
    data = pickle.loads(open(embeddingsPath, "rb").read())

    # encode the labels
    print("[INFO] encoding labels...")
    le = LabelEncoder()
    labels = le.fit_transform(data["names"])

    # train the model used to accept the 128-d embeddings of the face and
    # then produce the actual face recognition
    print("[INFO] training model...")
    recognizer = svm.SVC(C=1.0, kernel="linear", probability=True)
    recognizer.fit(data["embeddings"], labels)

    # write the actual face recognition model to disk

    recognizer_model = {"recognizer": recognizer, "le": le}
    path = os.path.join(outputDir, my_constant.recognizerModelPath)
    f = open(path, "wb+")
    f.write(pickle.dumps(recognizer_model))
    f.close()


def augment_images(datasetDir, augmentedDir, nameString, genImageNum=4):
    onlyTrainSomePeople = False
    peopleToAugment = []
    if nameString:
        onlyTrainSomePeople = True
        peopleToAugment = nameString.split(",")

    # grab the paths to the input images in our dataset
    print("[INFO] quantifying faces...")
    imagePaths = list(paths.list_images(datasetDir))
    augmented_path = augmentedDir

    unknown_batch = []
    original_batch = []
    name_batch = []
    count = genImageNum  # generate 4 fake images for 1 raw image

    # loop over the image paths
    for (i, imagePath) in enumerate(imagePaths):
        # extract the person name from the image path
        print("[INFO] processing image {}/{}".format(i + 1,
                                                     len(imagePaths)))

        name = imagePath.split(os.path.sep)[-2]

        if (not onlyTrainSomePeople) or (onlyTrainSomePeople and name in peopleToAugment):
            # load the image, resize it to have a width of 400 pixels (while
            # maintaining the aspect ratio)
            image = cv2.imread(imagePath)
            image = imutils.resize(image, width=400)
            if name == "unknown":
                unknown_batch.append(image)
            else:
                original_batch.append(image)
                name_batch.append(name)

    augmented_batch = my_face_generator.face_generate(original_batch, count)
    name_batch = name_batch * count

    # add all augmented images into a dictionary
    name_image_dict = dict()
    for name, image in zip(name_batch, augmented_batch):
        if name in name_image_dict:
            name_image_dict[name].append(image)
        else:
            name_image_dict[name] = []
    # add all unknown images into the dictionary
    name_image_dict["unknown"] = unknown_batch

    if onlyTrainSomePeople:
        # only remove specified people's folders
        for name in name_image_dict.keys():
            shutil.rmtree(os.path.sep.join([augmented_path, name]))
            time.sleep(1) # Delays for 1 second because shutil functions are async and may block os.mkdir
            os.mkdir(os.path.sep.join([augmented_path, name]))
    else:
        # remove the entire folder and build it new again    
        if os.path.exists(augmented_path):
            shutil.rmtree(augmented_path)
            time.sleep(1) # Delays for 1 second because shutil functions are async and may block os.mkdir
        os.mkdir(augmented_path)
        for name in name_image_dict.keys():
            os.mkdir(os.path.sep.join([augmented_path, name]))

    # write each augmented image into their respective folder
    for name, images in name_image_dict.items():
        for i, image in enumerate(images):
            full_file_name = os.path.sep.join([augmented_path, name, str(i + 1) + ".jpg"])
            cv2.imwrite(full_file_name, image)


def add_embeddings(datasetPath, outputDir, embeddingsInputDir, nameString, alignFace=False):
    namesToAdd = nameString.split(",")

    print("[INFO] loading face embeddings...")
    embeddingsPath = os.path.join(embeddingsInputDir, my_constant.embeddingsPath)
    data = pickle.loads(open(embeddingsPath, "rb").read())

    imagePaths = list(paths.list_images(datasetPath))
    knownEmbeddings = data["embeddings"]
    knownNames = data["names"]
    totalAdded = 0

    # remove all embeddings of attendees in the to-add list
    filteredKnownNames = []
    filteredKnownNamesIndex = []
    for i, name in enumerate(knownNames):
        if not name in namesToAdd:
            filteredKnownNames.append(name)
            filteredKnownNamesIndex.append(i)

    filteredKnownEmbeddings = [emb for i, emb in enumerate(knownEmbeddings) if i in filteredKnownNamesIndex]
    # end remove


    for (i, imagePath) in enumerate(imagePaths):
        # extract the person name from the image path
        print("[INFO] processing image {}/{}".format(i + 1,
                                                     len(imagePaths)))
        name = imagePath.split(cv2.os.path.sep)[-2]

        if name in namesToAdd:
            # load the image
            image = cv2.imread(imagePath)
            boxes = my_face_detection.face_locations(image)
            if len(boxes) > 1:
                print(imagePath, "> 1")
                print(len(boxes))
                continue
            if len(boxes) == 0:
                print(imagePath, "= 0")
                print(len(boxes))
                continue
            if (alignFace == True):
                aligned_image = my_face_detection.align_face(image, boxes[0])
                vecs = my_face_recognition.face_encodings(aligned_image)
            else:
                vecs = my_face_recognition.face_encodings(image, boxes)
            vec = vecs[0]

            filteredKnownEmbeddings.append(vec.flatten())
            filteredKnownNames.append(name)
            totalAdded += 1

    # dump the facial embeddings + names to disk
    print("[INFO] serializing total {} encodings...".format(totalAdded))
    data = {"embeddings": filteredKnownEmbeddings, "names": filteredKnownNames}
    path = os.path.join(outputDir, my_constant.embeddingsPath)
    os.makedirs(os.path.dirname(path), exist_ok=True)
    f = open(path, "wb+")
    f.write(pickle.dumps(data))
    f.close()


def remove_embeddings(datasetPath, outputDir, embeddingsInputDir, nameString, alignFace=False):
    namesToAdd = nameString.split(",")

    print("[INFO] loading face embeddings...")
    embeddingsPath = os.path.join(embeddingsInputDir, my_constant.embeddingsPath)
    data = pickle.loads(open(embeddingsPath, "rb").read())

    imagePaths = list(paths.list_images(datasetPath))
    knownEmbeddings = data["embeddings"]
    knownNames = data["names"]

    # remove all embeddings of attendees in the to-add list
    filteredKnownNames = []
    filteredKnownNamesIndex = []
    for i, name in enumerate(knownNames):
        if not name in namesToAdd:
            filteredKnownNames.append(name)
            filteredKnownNamesIndex.append(i)

    filteredKnownEmbeddings = [emb for i, emb in enumerate(knownEmbeddings) if i in filteredKnownNamesIndex]
    # end remove

    # remove all folders of attendees to remove
    for name in namesToAdd:
        datasetPath = os.path.join("dataset", name)
        augmentedPath = os.path.join("augmented", name)
        if os.path.exists(datasetPath):
            shutil.rmtree(datasetPath)
        if os.path.exists(augmentedPath):
            shutil.rmtree(augmentedPath)

    # dump the facial embeddings + names to disk
    print("[INFO] removing total {} encodings...".format(len(namesToAdd)))
    data = {"embeddings": filteredKnownEmbeddings, "names": filteredKnownNames}
    path = os.path.join(outputDir, my_constant.embeddingsPath)
    os.makedirs(os.path.dirname(path), exist_ok=True)
    f = open(path, "wb+")
    f.write(pickle.dumps(data))
    f.close()
