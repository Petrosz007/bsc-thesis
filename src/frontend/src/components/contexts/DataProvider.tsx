import React from "react";
import { createContext, useReducer } from "react";
import { Appointment, Category, User } from "../../logic/entities";

export interface DataState {
    appointments: Appointment[];
    categories: Category[];
    users: User[];
}

export interface DataContextType {
    dataState: DataState;
    dataDispatch: React.Dispatch<DataAction>;
}

const initialState: DataState = {
    appointments: [],
    categories: [],
    users: [],
}

export const DataContext = createContext<DataContextType>({
    dataState: initialState,
    dataDispatch: () => null,
});

export type DataAction =
    | { type: 'updateAppointment', appointment: Appointment }
    | { type: 'setAppointments', appointments: Appointment[] }
    | { type: 'updateCategory', category: Category }
    | { type: 'updateUser', user: User }
    | { type: 'logout'};

const setValue = <T,G>(values: T[], value: T, lens: (_x: T) => G): T[] => {
    if(!values.some(x => lens(x) === lens(value))) {
        return [...values, value];
    }

    return values.map(x =>
        lens(x) !== lens(value)
            ? x
            : value);
}

const reducer = (state: DataState, action: DataAction): DataState => {
    console.log(state, action);
    
    switch(action.type) {
        case 'updateAppointment':
            return {
                ...state,
                appointments: setValue(state.appointments, action.appointment, a => a.id),
            };
        case 'setAppointments':
            return {
                ...state,
                appointments: action.appointments,
            };
        case 'updateCategory':
            return {
                ...state,
                categories: setValue(state.categories, action.category, c => c.id),
            };
        case 'updateUser':
            return {
                ...state,
                users: setValue(state.users, action.user, u => u.userName),
            };
        case 'logout':
            return initialState;
        default:
            return state;
    }
};

export default ({ children }: { children: React.ReactNode }) => {
    const [dataState, dataDispatch] = useReducer(reducer, initialState);

    return (
        <DataContext.Provider value={{ dataState, dataDispatch }}>
            {children}
        </DataContext.Provider>
    );
};