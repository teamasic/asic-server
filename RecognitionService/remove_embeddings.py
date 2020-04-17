import argparse

from helper import my_service

ap = argparse.ArgumentParser()
ap.add_argument("-i", "--dataset", default="dataset",
                help="path to input directory of faces + images")
ap.add_argument("-o", "--output", default="output_dlib",
                help="path to output directory of embedding and model files")
ap.add_argument("-p", "--input", default="output_dlib",
                help="path to input the existing embedding file")
ap.add_argument("-n", "--names", default="",
                help="a comma separated list of identifiers of people to remove")
args = vars(ap.parse_args())

my_service.remove_embeddings(args["dataset"], args["output"], args["input"], args["names"])
