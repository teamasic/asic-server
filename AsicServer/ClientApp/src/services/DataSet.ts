import ApiResponse from "../models/ApiResponse";
import axios from "axios";
import { routes } from "../constants/routes";
import DataSet from "../models/DataSet";

const baseRoute  = routes.BASE_ROUTE + "dataset";
const apify = (path: string) => `${baseRoute}/${path}`;

export const create = async (dataSet: DataSet): Promise<ApiResponse> => {
    const response = await axios.post(baseRoute, dataSet);
    return await response.data;
};



