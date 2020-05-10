import * as signalR from "@microsoft/signalr";
import { MiddlewareAPI, Dispatch, Middleware } from "redux";
import { ApplicationState } from "../store";
import createSignalRConnection from './SignalRConnection';
import { success } from '../utils';

const ACTIONS = {
};

let connection: signalR.HubConnection;

function createSignalRMiddleware() {
    const middleware: Middleware = ({ dispatch, getState }: MiddlewareAPI) => (
        next: Dispatch
    ) => action => {
        switch (action.type) {
        }
        return next(action);
    };

    return middleware;
}

export function attachEvents(connection: signalR.HubConnection, dispatch: any) {
    const interval = setInterval(() => {
        if (connection.state === signalR.HubConnectionState.Connected) {
            connection.send('heartbeat');
            // console.log('heartbeat');
        }
    }, 10000);

    connection.onclose(() => {
        clearInterval(interval);
    });

    connection.on("attendeeUnknownBatch", (imageString: string) => {
        const images = imageString.split(",");
    });

    connection.on("keepAlive", () => {
        // console.log('keep alive');
    });
}

export default createSignalRMiddleware;