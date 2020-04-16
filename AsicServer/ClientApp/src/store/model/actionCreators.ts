import ApiResponse from "../../models/ApiResponse";
import { ThunkDispatch } from "redux-thunk";
import { AppThunkAction } from "..";
import { AnyAction } from "redux";
import UserLogin from "../../models/UserLogin";
import { constants } from "../../constants/constant";
import * as services from "../../services/Model";
import { error, success, warning } from "../../utils";
import TrainingResult from "../../models/TrainingResult";

export const ACTIONS = {
    START_TRAINING: 'START_TRAINING',
    STOP_TRAINING: 'STOP_TRAINING',
    RECEIVE_TRAIN_RESULT: 'RECEIVE_TRAIN_RESULT',
    RECEIVE_IS_TRAINING: 'RECEIVE_IS_TRAINING'
};

function startTraining() {
    return {
        type: ACTIONS.START_TRAINING
    };
}

function stopTraining() {
    return {
        type: ACTIONS.STOP_TRAINING
    };
}

const train = (): AppThunkAction => async (dispatch, getState) => {
    dispatch(startTraining());
    var apiResponse: ApiResponse = await services.train();
    dispatch(stopTraining());
    if (apiResponse.success) {
        success("Successfully trained face recognition model.");
        getLastTrainResult();
    } else {
        error("An error happened during the training process. Please try again.");
        console.log(apiResponse.errors);
    }
}

function receiveTrainResult(result: TrainingResult) {
    return {
        type: ACTIONS.RECEIVE_TRAIN_RESULT,
        result
    };
}

const getLastTrainResult = (): AppThunkAction => async (dispatch, getState) => {
    var apiResponse: ApiResponse = await services.getLastTrainResult();
    if (apiResponse.success) {
        dispatch(receiveTrainResult(apiResponse.data));
    }
}

function receiveIsTraining(isTraining: boolean) {
    return {
        type: ACTIONS.RECEIVE_IS_TRAINING,
        isTraining
    };
}

const checkIsTraining = (): AppThunkAction => async (dispatch, getState) => {
    var apiResponse: ApiResponse = await services.getIsTraining();
    if (apiResponse.success) {
        dispatch(receiveIsTraining(apiResponse.data));
    }
}

export const modelActionCreators = {
    train,
    getLastTrainResult,
    checkIsTraining
};