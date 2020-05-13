import ApiResponse from "../../models/ApiResponse";
import { ThunkDispatch } from "redux-thunk";
import { AppThunkAction } from "..";
import { AnyAction } from "redux";
import UserLogin from "../../models/UserLogin";
import { constants } from "../../constants/constant";
import { UserLoginResponse } from "../../models/UserLoginResponse";
import { login, createMultipleUsers, createSingleUser, getUserByEmail, getUsersFromTrainMoreList } from "../../services/User";
import { error, success, warning } from "../../utils";
import CreateUser from "../../models/CreateUser";
import User from "../../models/User";

export const ACTIONS = {
    START_REQUEST_LOGIN:"START_REQUEST_LOGIN",
    STOP_REQUEST_LOGIN_WITH_ERRORS:"STOP_REQUEST_LOGIN_WITH_ERRORS",
    RECEIVE_SUCCESS_LOGIN:"RECEIVE_SUCCESS_LOGIN",
    USER_INFO_NOT_IN_LOCAL: "USER_INFO_NOT_IN_LOCAL",
    LOG_OUT: "LOG_OUT",
    RECEIVE_USERS_TO_TRAIN_MORE: 'RECEIVE_USERS_TO_TRAIN_MORE'
}

function startRequestLogin() {
    return {
        type: ACTIONS.START_REQUEST_LOGIN
    };
}

function stopRequestLoginWithError(errors: any[]) {
    return {
        type: ACTIONS.STOP_REQUEST_LOGIN_WITH_ERRORS,
        errors
    };
}

function receiveSuccessLogin(userLoginResponse: UserLoginResponse) {
    return {
        type: ACTIONS.RECEIVE_SUCCESS_LOGIN,
        user: userLoginResponse
    };
}

function checkUserInfo() {
    const authData = localStorage.getItem(constants.AUTH_IN_LOCAL_STORAGE);
    if (authData) {
        const user = JSON.parse(authData);
        return {
            type: ACTIONS.RECEIVE_SUCCESS_LOGIN,
            user
        }
    }
    return {
        type: ACTIONS.USER_INFO_NOT_IN_LOCAL
    }
}

function logout(){
    return {
        type: ACTIONS.LOG_OUT
    }
}

const requestLogin = (userLogin: UserLogin, redirect: Function): AppThunkAction => async (dispatch, getState) => {
    dispatch(startRequestLogin());

    const apiResponse: ApiResponse = await login(userLogin);
    if (apiResponse.success) {
        console.log(apiResponse.data);
        localStorage.setItem(constants.AUTH_IN_LOCAL_STORAGE, JSON.stringify(apiResponse.data.user));
        localStorage.setItem(constants.ACCESS_TOKEN, apiResponse.data.accessToken);
        dispatch(receiveSuccessLogin(apiResponse.data));
        redirect();
    } else {
        dispatch(stopRequestLoginWithError(apiResponse.errors));
    }
}

const requestCreateMultipleUsers = (zipFile: File, csvFile: File, resetUsersTable: Function, isAppendTrain: boolean): AppThunkAction => async (dispatch, getState) => {
    console.log(isAppendTrain);
    const apiResponse: ApiResponse = await createMultipleUsers(zipFile, csvFile, isAppendTrain);
    if(apiResponse.success) {
        var result = apiResponse.data;
        console.log(result);
        resetUsersTable(result);
    } else {
        console.log(apiResponse.errors);
    }
}

const requestCreateSingleUser = (zipFile: File, user: CreateUser, createUserSuccess: Function, createUserWithError: Function, isAppendTrain: boolean): AppThunkAction => async (dispatch, getState) => {
    console.log(isAppendTrain);
    var apiResponse: ApiResponse = await createSingleUser(zipFile, user, isAppendTrain);
    if(apiResponse.success) {
        var result = apiResponse.data;
        if(result) {
            success("Create user successfully!");
            createUserSuccess();
        } else {
            warning("User is saved without image!");
            createUserWithError();
        }
    } else {
        console.log(apiResponse.errors);        
    }
}

const requestUserByEmail = (email: string, getSuccess: Function): AppThunkAction => async (dispatch, getState) => {
    var apiResponse: ApiResponse = await getUserByEmail(email);
    if(apiResponse.success) {
        getSuccess(apiResponse.data);
    } else {
        getSuccess(null);
        console.log(apiResponse.errors);
    }
}

function receiveListUsersToTrainMore(users: User[]) {
    return {
        type: ACTIONS.RECEIVE_USERS_TO_TRAIN_MORE,
        users
    };
}

const getListUsersToTrainMore = (): AppThunkAction => async (dispatch, getState) => {
    var apiResponse: ApiResponse = await getUsersFromTrainMoreList();
    if (apiResponse.success) {
        dispatch(receiveListUsersToTrainMore(apiResponse.data))
    } else {
        console.log(apiResponse.errors);
    }
};

export const userActionCreators = {
    requestLogin: requestLogin,
    requestCreateMultipleUsers: requestCreateMultipleUsers,
    requestCreateSingleUser: requestCreateSingleUser,
    requestUserByEmail: requestUserByEmail,
    checkUserInfo: checkUserInfo,
    logout: logout,
    getListUsersToTrainMore
};