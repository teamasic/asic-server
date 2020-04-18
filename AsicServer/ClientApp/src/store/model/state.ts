import TrainingResult from "../../models/TrainingResult";

export interface ModelState {
    lastTrainResult?: TrainingResult;
    isTraining: boolean;
}