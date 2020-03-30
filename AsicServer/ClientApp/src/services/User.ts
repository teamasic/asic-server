import ApiResponse from "../models/ApiResponse";
import axios from "axios";
import UserLogin from "../models/UserLogin";
import { routes } from "../constants/routes";
import CreateUser from "../models/CreateUser";

const baseRoute  = routes.BASE_ROUTE + "user";
const apify = (path: string) => `${baseRoute}/${path}`;

export const login = async (userLogin: UserLogin): Promise<ApiResponse> => {
    const response = await axios.post(apify('login'), userLogin);
    return await response.data;
};

export const createMultipleUsers = async (zipFile: File, csvFile: File): Promise<ApiResponse> => {
    var formData = new FormData();
    formData.append("zipFile", zipFile);
    formData.append("users", csvFile);
    const response = await axios.post(apify("multiple"), formData);
    return await response.data;
}

export const createSingleUser = async (zipFile: File, user: CreateUser): Promise<ApiResponse> => {
    var formData = new FormData();
    formData.append("zipFile", zipFile);
    const response = await axios.post(apify("single"), formData, {
        params: {
            Email: user.email,
            RollNumber: user.rollNumber,
            Fullname: user.fullname
        }
    });
    return await response.data;
}

export const getUserByEmail = async (email: string): Promise<ApiResponse> => {
    var response = await axios.get(baseRoute, {
        params: {
            email: email
        }
    });
    return await response.data;
}
