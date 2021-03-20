import { useCallback, useContext, useEffect, useState } from "react";
import { DIContext } from "../components/contexts/DIContext";
import { LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { User } from "../logic/entities";
import { ResultPromise } from "../utilities/result";
import { DataContext } from "../components/contexts/DataProvider";
import { useEffectAsync } from "./utilities";
import { useHistory } from "react-router";

export class Loading {} 
export class Idle {}
export class Loaded<T> {
    constructor(public readonly value: T) {}
}
export class Failed<T> {
    constructor(public readonly error: T) {}
}

export type Status<T,U> = Loading | Idle | Loaded<T> | Failed<U>;

export const useApiCall = <T,E>(fn: () => ResultPromise<T,E>, deps: React.DependencyList) => {
    const [status, setStatus] = useState<Status<T,Error>>(new Idle());

    const fetch = useCallback(async () => {
        setStatus(new Loading());

        const result = await fn().toPromise();
        result.match(
            value => setStatus(new Loaded(value)),
            error => setStatus(new Failed(error))
        );
    }, [fn, ...deps]);

    return [ status, fetch ] as const;
}

export const useLogin = (
    userName: string,
    password: string,
): [ Status<User,Error>, () => void ] => {
    const [status, setStatus] = useState<Status<User,Error>>(new Idle());
    const { loginState, loginDispatch } = useContext(LoginContext);
    const { userRepo, accountRepo } = useContext(DIContext);
    const history = useHistory();

    const login = useCallback(async () => {
        setStatus(new Loading());

        await accountRepo.login(userName, password)
            .andThen(_ => userRepo.getByUserName(userName))
            .match(
            user => {
                setStatus(new Loaded(user));     
                loginDispatch({ type: 'login', user });
                history.push('/');
            },
            error => setStatus(new Failed(error)));
    }, [userName, password]);

    return [ status, login ];
}

export const useLogout = (): [ Status<boolean,Error>, () => void ] => {
    const [status, setStatus] = useState<Status<boolean,Error>>(new Idle());
    const { loginState, loginDispatch } = useContext(LoginContext);
    const { dataState, dataDispatch } = useContext(DataContext);
    const { accountRepo } = useContext(DIContext);
    const history = useHistory();

    const logout = useCallback(async () => {
        if(loginState instanceof LoggedOut) {
            setStatus(new Failed(new Error('Cannot log out without being logged in')));
            return;
        }

        setStatus(new Loading());

        const logoutResult = await accountRepo.logout()
            .toPromise();

        logoutResult.match(
            _ => {
                setStatus(new Loaded(true));
                loginDispatch({ type: 'logout' });
                dataDispatch({ type: 'logout' });
                history.push('/'); 
            },
            error => setStatus(new Failed(error)));
    }, [status, loginState, loginDispatch, accountRepo]);

    return [ status, logout ];
}
