import React, { createContext, useReducer } from "react";
import Notifications from "../Notifications";
import { v4 as uuidv4 } from 'uuid';

type NotificationType = 'error' | 'warning' | 'success';

export interface NotificationData {
    id: string;
    message: string;
    notificationType: NotificationType;
}

export interface NotificationContextType {
    notifications: NotificationData[];
    notificationDispatch: React.Dispatch<NotificationAction>;
}

export const NotificationContext = createContext<NotificationContextType>({
    notifications: [],
    notificationDispatch: () => null,
});

export type NotificationAction =
    | { type: 'addError', message: string }
    | { type: 'addWarning', message: string }
    | { type: 'addSuccess', message: string }
    | { type: 'remove', id: string };

const reducer = (state: NotificationData[], action: NotificationAction): NotificationData[] => {
    switch(action.type) {
        case 'addError':
            return [...state, { id: uuidv4(), message: action.message, notificationType: 'error' }];
        case 'addWarning':
            return [...state, { id: uuidv4(), message: action.message, notificationType: 'warning' }];
        case 'addSuccess':
            return [...state, { id: uuidv4(), message: action.message, notificationType: 'success' }];
        case 'remove':
            return state.filter(n => n.id !== action.id);
        default:
            return state;
    };
};
    
export default ({ children }: { children: React.ReactNode }) => {
    const [notifications, notificationDispatch] = useReducer(reducer, []);

    return (
        <NotificationContext.Provider value={{ notifications, notificationDispatch }}>
            <Notifications notifications={notifications} />
            {children}
        </NotificationContext.Provider>   
    )
}
