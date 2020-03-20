import ApiResponse from "../models/ApiResponse";
import axios from "axios";
import { routes } from "../constants/routes";
import CreateDataSet from "../models/CreateDataSet";

const baseRoute  = routes.BASE_ROUTE + "dataset";
const apify = (path: string) => `${baseRoute}/${path}`;

export const create = async (dataSet: CreateDataSet): Promise<ApiResponse> => {
    const response = await axios.post(baseRoute, dataSet);
    return await response.data;
};

export const getAll = async (): Promise<ApiResponse> => {
    const response = await axios.get(baseRoute);
    return await response.data;
}

