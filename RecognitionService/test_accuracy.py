from helper import my_service

if __name__ == "__main__":
    my_service.augment_images(datasetDir="dataset", augmentedDir="augmented", nameString=None)
    my_service.generate_embeddings(datasetPath="augmented", outputDir="output_dlib")
    my_service.generate_train_model_softmax(outputDir="output_dlib")