import ApiResponse from "../../models/ApiResponse";
import { ThunkDispatch } from "redux-thunk";
import { AppThunkAction } from "..";
import { AnyAction } from "redux";
import UserLogin from "../../models/UserLogin";
import { constants } from "../../constants/constant";
import { UserLoginResponse } from "../../models/UserLoginResponse";
import { login, createMultipleUsers, createSingleUser, getUserByEmail } from "../../services/User";
import { error, success, warning } from "../../utils";
import CreateUser from "../../models/CreateUser";

export const ACTIONS = {
    START_REQUEST_LOGIN:"START_REQUEST_LOGIN",
    STOP_REQUEST_LOGIN_WITH_ERRORS:"STOP_REQUEST_LOGIN_WITH_ERRORS",
    RECEIVE_SUCCESS_LOGIN:"RECEIVE_SUCCESS_LOGIN"

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

const requestCreateMultipleUsers = (zipFile: File, csvFile: File): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await createMultipleUsers(zipFile, csvFile);
    if(apiResponse.success) {
        var result = apiResponse.data;
        if(result.length > 0) {
            var msg = "These " + result.length + " users is saved without images: ";
            result.forEach((rollnumber: string) => {
                msg += rollnumber + " ";
            });
            warning(msg);
        } else {
            success("Save users successfully!");
        }
    } else {
        console.log(apiResponse.errors);
    }
}

const requestCreateSingleUser = (zipFile: File, user: CreateUser): AppThunkAction => async (dispatch, getState) => {
    var apiResponse: ApiResponse = await createSingleUser(zipFile, user);
    if(apiResponse.success) {
        var result = apiResponse.data;
        if(result) {
            success("Create user successfully!");
        } else {
            warning("User is saved without image!");
        }
    } else {
        console.log(apiResponse.errors);        
    }
}

const requestUserByEmail = (email: string, getSuccess: Function): AppThunkAction => async (dispatch, getState) => {
    var apiResponse: ApiResponse = await getUserByEmail(email);
    if(apiResponse.success) {
        getSuccess(apiResponse.data);
    }
}

export const userActionCreators = {
    requestLogin: requestLogin,
    requestCreateMultipleUsers: requestCreateMultipleUsers,
    requestCreateSingleUser: requestCreateSingleUser,
    requestUserByEmail: requestUserByEmail
};