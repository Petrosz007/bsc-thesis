import { useCallback, useContext, useState } from "react";
import { DIContext } from "../components/contexts/DIContext";
import { LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { User } from "../logic/entities";
import { ResultPromise } from "../utilities/result";
import { DataContext } from "../components/contexts/DataProvider";
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

export const useApiCall = <T,E>(fn: (...args: any[]) => ResultPromise<T,E>, deps: React.DependencyList) => {
    const [status, setStatus] = useState<Status<T,Error>>(new Idle());

    const fetch = useCallback(async (...args: any[]) => {
        setStatus(new Loading());

        const result = await fn(...args).toPromise();
        result.match(
            value => setStatus(new Loaded(value)),
            error => setStatus(new Failed(error))
        );
    }, [fn, ...deps]);

    return [ status, fetch ] as const;
}

export const useLogin = (): [ Status<User,Error>, (userName: string, password: string) => Promise<void> ] => {
    const [status, setStatus] = useState<Status<User,Error>>(new Idle());
    const { loginDispatch } = useContext(LoginContext);
    const { userRepo, accountRepo } = useContext(DIContext);

    const login = useCallback(async (userName: string, password: string) => {
        setStatus(new Loading());

        await accountRepo.login(userName, password)
            .andThen(_ => userRepo.getSelfInfo())
            .match(
            user => {
                setStatus(new Loaded(user));     
                loginDispatch({ type: 'login', user });
            },
            error => setStatus(new Failed(error)));
    }, []);

    return [ status, login ];
}

export const useLogout = (): [ Status<boolean,Error>, () => void ] => {
    const [status, setStatus] = useState<Status<boolean,Error>>(new Idle());
    const { loginState, loginDispatch } = useContext(LoginContext);
    const { dataDispatch } = useContext(DataContext);
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
