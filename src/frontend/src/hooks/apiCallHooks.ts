import { useCallback, useContext, useState } from "react";
import { DIContext } from "../components/contexts/DIContext";
import { LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { User } from "src/logic/entities";
import { ResultPromise } from "src/utilities/result";

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

    const login = useCallback(async () => {
        setStatus(new Loading());

        const loginResult = await accountRepo.login(userName, password)
            .andThen(_ => userRepo.getByUserName(userName))
            .toPromise();

        loginResult.match(
            user => {
                loginDispatch({ type: 'login', user });
                setStatus(new Loaded(user));     
            },
            error => setStatus(new Failed(error)));
    }, [userName, password]);

    return [ status, login ];
}

export const useLogout = (): [ Status<boolean,Error>, () => void ] => {
    const [status, setStatus] = useState<Status<boolean,Error>>(new Idle());
    const { loginState, loginDispatch } = useContext(LoginContext);
    const { accountRepo } = useContext(DIContext);

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
                loginDispatch({ type: 'logout' });
                setStatus(new Loaded(true));     
            },
            error => setStatus(new Failed(error)));
    }, [status, loginState, loginDispatch, accountRepo]);

    return [ status, logout ];
}
