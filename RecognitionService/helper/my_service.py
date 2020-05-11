import multiprocessing
import os
import pickle
import shutil
import time
import cv2
import imutils
import json
from imutils import paths
from sklearn import svm, linear_model
from sklearn.preprocessing import LabelEncoder
from datetime import datetime

from config import my_constant
from helper import my_face_detection, my_face_recognition, my_face_generator


def getVecAndName(imagePath):
    print("[INFO] processing image {}".format(imagePath))

    name = imagePath.split(cv2.os.path.sep)[-2]

    # load the image
    image = cv2.imread(imagePath)
    image = imutils.resize(image, width=400)
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    boxes = my_face_detection.face_locations(image)
    if len(boxes) > 1:
        print(imagePath, "> 1")
        print(len(boxes))
        return None, None
    elif len(boxes) == 0:
        print(imagePath, "= 0")
        print(len(boxes))
        return None, None
    else:
        vecs = my_face_recognition.face_encodings(image, boxes)
        vec = vecs[0]
        return vec, name


def generate_embeddings(datasetPath, outputDir):
    myPool = multiprocessing.Pool()
    imagePaths = list(paths.list_images(datasetPath))

    results = myPool.map(getVecAndName, imagePaths)
    knownEmbeddings = [x[0] for x in results if x[0] is not None and x[1] is not None]
    knownNames = [x[1] for x in results if x[0] is not None and x[1] is not None]

    # dump the facial embeddings + names to disk
    print("[INFO] serializing total {} encodings...".format(len(knownEmbeddings)))
    data = {"embeddings": knownEmbeddings, "names": knownNames}
    path = os.path.join(outputDir, my_constant.embeddingsPath)
    os.makedirs(os.path.dirname(path), exist_ok=True)
    f = open(path, "wb+")
    f.write(pickle.dumps(data))
    f.close()
    myPool.close()


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

    imagePathsNeedToTrain = []

    # add image need
    for (i, imagePath) in enumerate(imagePaths):
        name = imagePath.split(os.path.sep)[-2]
        if (not onlyTrainSomePeople) or (onlyTrainSomePeople and name in peopleToAugment):
            imagePathsNeedToTrain.append(imagePath)

    print(len(imagePathsNeedToTrain))

    # loop over the image paths
    for (i, imagePath) in enumerate(imagePathsNeedToTrain):
        print("Process images {}/{}".format(i, len(imagePathsNeedToTrain)) )
        name = imagePath.split(os.path.sep)[-2]
        image = cv2.imread(imagePath)
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
            name_image_dict[name] = [image]
    # add all unknown images into the dictionary
    name_image_dict["unknown"] = unknown_batch

    if onlyTrainSomePeople:
        # only remove specified people's folders
        for name in name_image_dict.keys():
            path_to_delete = os.path.sep.join([augmented_path, name])
            if os.path.exists(path_to_delete):
                shutil.rmtree(path_to_delete)
                time.sleep(1)  # Delays for 1 second because shutil functions are async and may block os.mkdir
            os.mkdir(os.path.sep.join([augmented_path, name]))
    else:
        # remove the entire folder and build it new again    
        if os.path.exists(augmented_path):
            shutil.rmtree(augmented_path)
            time.sleep(1)  # Delays for 1 second because shutil functions are async and may block os.mkdir
        os.mkdir(augmented_path)
        for name in name_image_dict.keys():
            os.mkdir(os.path.sep.join([augmented_path, name]))

    # write each augmented image into their respective folder
    for name, images in name_image_dict.items():
        for i, image in enumerate(images):
            full_file_name = os.path.sep.join([augmented_path, name, str(i + 1) + ".jpg"])
            cv2.imwrite(full_file_name, image)


def add_embeddings(datasetPath, outputDir, inputDir, nameString):
    myPool = multiprocessing.Pool()
    namesToAdd = nameString.split(",")

    print("[INFO] loading face embeddings...")
    embeddingsPath = os.path.join(inputDir, my_constant.embeddingsPath)
    data = pickle.loads(open(embeddingsPath, "rb").read())

    imagePaths = list(paths.list_images(datasetPath))
    knownEmbeddings = data["embeddings"]
    knownNames = data["names"]
    # totalAdded = 0

    # remove all embeddings of attendees in the to-add list
    filteredKnownNames = []
    filteredKnownNamesIndex = []
    for i, name in enumerate(knownNames):
        if not name in namesToAdd:
            filteredKnownNames.append(name)
            filteredKnownNamesIndex.append(i)

    filteredKnownEmbeddings = [emb for i, emb in enumerate(knownEmbeddings) if i in filteredKnownNamesIndex]
    # end remove
    imagePathsNeedToAdd = []
    for (i, imagePath) in enumerate(imagePaths):
        name = imagePath.split(cv2.os.path.sep)[-2]
        if name in namesToAdd:
            imagePathsNeedToAdd.append(imagePath)
    results = myPool.map(getVecAndName, imagePathsNeedToAdd)
    newKnownEmbeddings = [x[0] for x in results if x[0] is not None and x[1] is not None]
    newKnownNames = [x[1] for x in results if x[0] is not None and x[1] is not None]

    filteredKnownEmbeddings = filteredKnownEmbeddings + newKnownEmbeddings
    filteredKnownNames = filteredKnownNames + newKnownNames

    data = {"embeddings": filteredKnownEmbeddings, "names": filteredKnownNames}
    path = os.path.join(outputDir, my_constant.embeddingsPath)
    os.makedirs(os.path.dirname(path), exist_ok=True)
    f = open(path, "wb+")
    f.write(pickle.dumps(data))
    f.close()
    myPool.close()


def remove_embeddings(outputDir, embeddingsInputDir, nameString):
    namesToRemove = nameString.split(",")

    print("[INFO] loading face embeddings...")
    embeddingsPath = os.path.join(embeddingsInputDir, my_constant.embeddingsPath)
    data = pickle.loads(open(embeddingsPath, "rb").read())

    knownEmbeddings = data["embeddings"]
    knownNames = data["names"]

    # remove all embeddings of attendees in the to-add list
    filteredKnownNames = []
    filteredKnownNamesIndex = []
    for i, name in enumerate(knownNames):
        if not name in namesToRemove:
            filteredKnownNames.append(name)
            filteredKnownNamesIndex.append(i)

    filteredKnownEmbeddings = [emb for i, emb in enumerate(knownEmbeddings) if i in filteredKnownNamesIndex]
    # end remove

    # remove all folders of attendees to remove
    for name in namesToRemove:
        datasetPath = os.path.join("dataset", name)
        augmentedPath = os.path.join("augmented", name)
        if os.path.exists(datasetPath):
            shutil.rmtree(datasetPath)
        if os.path.exists(augmentedPath):
            shutil.rmtree(augmentedPath)

    # dump the facial embeddings + names to disk
    print("[INFO] removing total {} encodings...".format(len(namesToRemove)))
    data = {"embeddings": filteredKnownEmbeddings, "names": filteredKnownNames}
    path = os.path.join(outputDir, my_constant.embeddingsPath)
    os.makedirs(os.path.dirname(path), exist_ok=True)
    f = open(path, "wb+")
    f.write(pickle.dumps(data))
    f.close()


def generate_train_model_softmax(outputDir):
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
    recognizer = linear_model.LogisticRegression(C=1e5,
                                                 solver='lbfgs', multi_class='multinomial', max_iter=1000)
    recognizer.fit(data["embeddings"], labels)

    # write the actual face recognition model to disk

    recognizer_model = {"recognizer": recognizer, "le": le}
    path = os.path.join(outputDir, my_constant.recognizerModelPath)
    f = open(path, "wb+")
    f.write(pickle.dumps(recognizer_model))
    f.close()

    attendee_count = len(set(data["names"]))
    image_count = len(data["embeddings"])
    result_dict = {"attendeeCount": attendee_count, "imageCount": image_count,
                   "timeFinished": datetime.now().isoformat()}
    f_result = open(my_constant.resultFile, "w+")
    f_result.write(json.dumps(result_dict))
    f_result.close()
