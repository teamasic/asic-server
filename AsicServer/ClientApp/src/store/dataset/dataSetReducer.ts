import { Reducer, Action, AnyAction } from "redux";
import { DataSetState } from "./dataSetState";
import { ACTIONS } from "./dataSetActionCreator";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: DataSetState = {
    isLoading: false,
};

const reducers: Reducer<DataSetState> = (state: DataSetState | undefined, incomingAction: AnyAction): DataSetState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.CREATE_DATASET:
            return {
                ...state,
            };
    }

    return state;
};

export default reducers;