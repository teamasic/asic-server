import argparse

# construct the argument parser and parse the arguments
from helper import my_service

# import the necessary packages

ap = argparse.ArgumentParser()
ap.add_argument("-i", "--dataset", default="dataset_more",
                help="path to input directory of faces + images")
ap.add_argument("-o", "--output", default="augmented",
                help="path to output directory of augmented images")
ap.add_argument("-n", "--names", default="",
                help="a comma separated list of identifiers of people to augment")
args = vars(ap.parse_args())

my_service.augment_images(datasetDir=args["dataset"], augmentedDir=args["output"], nameString=args["names"])
