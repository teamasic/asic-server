from helper import my_service

my_service.augment_images("dataset", "augmented", "")
my_service.generate_embeddings("augmented", "output_dlib")
my_service.generate_train_model_softmax("output_dlib")


