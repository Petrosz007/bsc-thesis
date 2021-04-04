import React from "react";
import { createContext, useReducer } from "react";
import { Appointment, Category, User } from "../../logic/entities";
import { setValue } from "../../utilities/listExtensions";

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
    | { type: 'deleteAppointment', id: number }
    | { type: 'updateCategory', category: Category }
    | { type: 'setCategories', categories: Category[] }
    | { type: 'updateUser', user: User }
    | { type: 'setUsers', users: User[] }
    | { type: 'deleteUser', userName: string }
    | { type: 'logout'};

const reducer = (state: DataState, action: DataAction): DataState => {
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
        case 'deleteAppointment':
            return {
                ...state,
                appointments: state.appointments.filter(a => a.id !== action.id),
            };
        case 'updateCategory':
            return {
                ...state,
                categories: setValue(state.categories, action.category, c => c.id),
                appointments: state.appointments.map(a => ({
                    ...a,
                    category: a.category.id === action.category.id ? action.category : a.category,
                })),
            };
        case 'setCategories':
            return {
                ...state,
                categories: action.categories,
            };
        case 'updateUser':
            return {
                ...state,
                users: setValue(state.users, action.user, u => u.userName),
            };
        case 'setUsers':
            return {
                ...state,
                users: action.users,
            };
        case 'deleteUser':
            return {
                ...state,
                users: state.users.filter(user => user.userName !== action.userName),
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