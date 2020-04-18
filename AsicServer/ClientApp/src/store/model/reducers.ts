import { Reducer, Action, AnyAction } from "redux";
import { ModelState } from "./state";
import { ACTIONS } from "./actionCreators";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: ModelState = {
    isTraining: false
};

const reducers: Reducer<ModelState> = (state: ModelState | undefined, incomingAction: AnyAction): ModelState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.START_TRAINING:
            return {
                ...state,
                isTraining: true
            };
        case ACTIONS.STOP_TRAINING:
            return {
                ...state,
                isTraining: false
            };
        case ACTIONS.RECEIVE_TRAIN_RESULT:
            return {
                ...state,
                lastTrainResult: action.result,
                isTraining: false
            };
        case ACTIONS.RECEIVE_IS_TRAINING:
            return {
                ...state,
                isTraining: action.isTraining
            };
    }

    return state;
};

export default reducers;