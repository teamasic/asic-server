import argparse

from helper import my_service

ap = argparse.ArgumentParser()
ap.add_argument("-i", "--dataset", default="dataset",
                help="path to input directory of faces + images")
ap.add_argument("-o", "--output", default="output_dlib",
                help="path to output directory of embedding and model files")
args = vars(ap.parse_args())

my_service.generate_embeddings(args["dataset"], args["output"])
