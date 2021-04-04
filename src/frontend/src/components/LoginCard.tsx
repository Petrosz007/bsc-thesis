import React, { useEffect } from "react";
import { useCallback, useContext, useState } from "react";
import { Redirect } from "react-router";
import { Failed, Idle, Loaded, Loading, useApiCall, useLogin, useLogout } from "../hooks/apiCallHooks";
import { LoggedIn, LoggedOut, LoginContext } from "./contexts/LoginProvider";
import { NotificationContext } from "./contexts/NotificationProvider";

export default () => {
    const { loginState } = useContext(LoginContext);
    const { notificationDispatch } = useContext(NotificationContext);
    
    const [userName, setUserName] = useState('customer1');
    const [password, setPassword] = useState('kebab');

    const [loginStatus, login] = useLogin();

    useEffect(() => {
        if(loginStatus instanceof Failed) {
            console.error('Error in LoginCard: ', loginStatus.error);
            notificationDispatch({ type: 'addError', message: `Error logging in: ${loginStatus.error.message}` });
        }
    }, [loginStatus]);

    if(loginState instanceof LoggedIn && loginStatus instanceof Loaded)
        return <Redirect to="/" />;

    if(loginStatus instanceof Loading) return <div>Logging in ...</div>

    return (
        <form onSubmit={() => login(userName, password)}>
            UserName: <input type="text" value={userName} required={true} onChange={e => setUserName(e.target.value)}/><br/>
            Password: <input type="password" value={password} required={true} autoComplete="current-password" onChange={e => setPassword(e.target.value)}/><br/>
            <input type="submit" value="Login" />
        </form>
    );
};