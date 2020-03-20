import ApiResponse from "../../models/ApiResponse";
import { ThunkDispatch } from "redux-thunk";
import { AppThunkAction } from "..";
import { AnyAction } from "redux";
import { constants } from "../../constants/constant";
import CreateDataSet from '../../models/CreateDataSet';
import { create, getAll } from '../../services/DataSet'
import { success } from '../../utils'

export const ACTIONS = {
    CREATE_DATASET: 'CREATE_DATASET'
}

const createDataSet = (createDataSet: CreateDataSet, closeModal: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await create(createDataSet);
    if(apiResponse.success) {
        success("Create dataset successfully!");
        closeModal();
    } else {
        console.log(apiResponse.errors);
    }
}

const getAllDataSet = (setData: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await getAll();
    if(apiResponse.success) {
        setData(apiResponse.data);
    } else {
        console.log(apiResponse.errors);
    }
}

export const dataSetActionCreators = {
    createDataSet: createDataSet,
    getAllDataSet: getAllDataSet
};