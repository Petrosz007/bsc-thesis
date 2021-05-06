import LoginCard from "../components/LoginCard";
import React, {useContext, useEffect, useState} from "react";
import {LoggedIn, LoginContext} from "../components/contexts/LoginProvider";
import {NotificationContext} from "../components/contexts/NotificationProvider";
import {Failed, Loaded, Loading, useLogin} from "../hooks/apiCallHooks";
import {Redirect} from "react-router";

import './Login.scss'

export default () => {
    const { loginState } = useContext(LoginContext);
    const { notificationDispatch } = useContext(NotificationContext);

    const [userName, setUserName] = useState('');
    const [password, setPassword] = useState('');

    const [loginStatus, login] = useLogin();

    useEffect(() => {
        if(loginStatus instanceof Failed) {
            notificationDispatch({ type: 'addError', message: `Hiba bejelentkezésnél: ${loginStatus.error.message}` });
        }
    }, [loginStatus]);

    if(loginState instanceof LoggedIn && loginStatus instanceof Loaded)
        return <Redirect to="/" />;

    if(loginStatus instanceof Loading) return <div>Bejelentkezés...</div>

    return (
        <div className="loginPage">
            <h2>Bejelentkezés</h2>
            <form className="loginForm" onSubmit={() => login(userName, password)}>
                <div>
                    <label htmlFor="username">Felhasználónév</label>
                    <input type="text" name="username" value={userName} required={true} onChange={e => setUserName(e.target.value)}/>
                </div>
                <div>
                    <label htmlFor="password">Jelszó</label>
                    <input type="password" name="password" value={password} required={true} autoComplete="current-password" onChange={e => setPassword(e.target.value)}/><br/>
                </div>
                <input type="submit" value="Bejelentkezés" />
            </form>
        </div>
    );
}
