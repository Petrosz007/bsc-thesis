import { useCallback, useContext, useState } from "react";
import { DIContext } from "../components/contexts/DIContext";
import { LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { User } from "src/logic/entities";

export class Loading {} 
export class Idle {}
export class Loaded<T> {
    constructor(public readonly value: T) {}
}
export class Failed<T> {
    constructor(public readonly error: T) {}
}

export type Status<T,U> = Loading | Idle | Loaded<T> | Failed<U>;

export const useApiCall = <T>(fn: () => Promise<T>, deps: React.DependencyList) => {
    const [status, setStatus] = useState<Status<T,Error>>(new Idle());

    const fetch = useCallback(async () => {
        setStatus(new Loading());
        try {
            const result = await fn();
            setStatus(new Loaded(result));
            return result;
        } catch(err) {
            const error = err instanceof Error ? err : new Error(err);
            setStatus(new Failed(error));
            throw error;
        }
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
        try {
            await accountRepo.login(userName, password);
            const user = await userRepo.getByUserName(userName);
            loginDispatch({ type: 'login', user });
            setStatus(new Loaded(user));
        }
        catch(err) {
            const error = err instanceof Error ? err : new Error(err);
            setStatus(new Failed(error));
        }
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
        try {
            await accountRepo.logout();
            loginDispatch({ type: 'logout' });
            setStatus(new Loaded(true));
        }
        catch(err) {
            const error = err instanceof Error ? err : new Error(err);
            setStatus(new Failed(error));
        }
    }, [status, loginState, loginDispatch, accountRepo]);

    return [ status, logout ];
}
