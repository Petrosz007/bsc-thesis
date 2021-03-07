import React, { createContext, useReducer, useState } from "react";
import { User } from "src/logic/entities";

export class LoggedOut {}
export class LoggedIn {
    constructor(readonly user: User) {}
}
export type LoginState = LoggedOut | LoggedIn;

export interface LoginContextType {
    loginState: LoginState;
    loginDispatch: React.Dispatch<LoginAction>;
}

export const LoginContext = createContext<LoginContextType>({
    loginState: new LoggedOut(),
    loginDispatch: () => null,
});

export type LoginAction =
    | { type: 'login', user: User }
    | { type: 'logout' };

const reducer = (state: LoginState, action: LoginAction): LoginState => {
    switch(action.type) {
        case 'login':
            return new LoggedIn(action.user);
        case 'logout':
            return new LoggedOut();
        default:
            return state;
    };
};
    
export default ({ children }: { children: React.ReactNode }) => {
    const [loginState, loginDispatch] = useReducer(reducer, new LoggedOut());

    return (
        <LoginContext.Provider value={{ loginState, loginDispatch }}>
            {children}
        </LoginContext.Provider>   
    )
}
