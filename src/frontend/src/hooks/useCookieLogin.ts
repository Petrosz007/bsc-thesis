import { useCallback, useContext, useState } from "react";
import { DIContext } from "../components/contexts/DIContext";
import { LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { Unit } from "../utilities/result";
import { Failed, Idle, Loaded, Loading, Status } from "./apiCallHooks";

export const useCookieLogin = () => {
    const { loginState, loginDispatch } = useContext(LoginContext);
    const { userRepo } = useContext(DIContext);
    const [status, setStatus] = useState<Status<Unit,Error>>(new Idle());

    const login = useCallback(async () => {
        setStatus(new Loading());

        if(loginState instanceof LoggedOut) {
            await userRepo.getSelf()
                .andThen(_ => userRepo.getSelfInfo())
                .match(
                    user => {
                        loginDispatch({ type: 'login', user });
                        setStatus(new Loaded({}));
                    },
                    error => {
                        if(error === 'NotLoggedIn') {
                            setStatus(new Loaded({}));
                        }
                        else {
                            setStatus(new Failed(error));
                            console.error('useCookieLogin', error);
                        }
                    }
                );
        } else {
            setStatus(new Loaded({}));
        }
    }, []);

    return [status, login] as const;
}
