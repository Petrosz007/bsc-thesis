import React from "react";
import { useCallback, useContext, useState } from "react";
import { Redirect } from "react-router";
import { Failed, Idle, Loaded, Loading, useApiCall, useLogin, useLogout } from "../hooks/apiCallHooks";
import { LoggedIn, LoggedOut, LoginContext } from "./contexts/LoginProvider";

export default () => {
    const [userName, setUserName] = useState('customer1');
    const [password, setPassword] = useState('kebab');

    const { loginState } = useContext(LoginContext);

    const [loginStatus, login] = useLogin();
    const [logoutStatus, logout] = useLogout();

    if(loginState instanceof LoggedIn && loginStatus instanceof Loaded)
        return <Redirect to="/" />;

    if(loginStatus instanceof Loading || logoutStatus instanceof Loading) return <div>Logging in ...</div>

    return (
        <div>
            {loginStatus instanceof Failed && <div>Error: {loginStatus.error.message}</div>}
            {logoutStatus instanceof Failed && <div>Error: {logoutStatus.error.message}</div>}

            {loginState instanceof LoggedIn && <div>Logged in</div>}
            {loginState instanceof LoggedOut && <div>Logged out</div>}
            
            UserName: <input type="text" value={userName} onChange={e => setUserName(e.target.value)}/><br/>
            Password: <input type="password" value={password} onChange={e => setPassword(e.target.value)}/><br/>
            <button onClick={() => login(userName, password)}>Login</button>
            <button onClick={() => logout()}>Logout</button>
        </div>
    );
};