import LoginCard from "../components/LoginCard";
import React, {useContext, useEffect, useState} from "react";
import {LoggedIn, LoggedOut, LoginContext} from "../components/contexts/LoginProvider";
import {NotificationContext} from "../components/contexts/NotificationProvider";
import {Failed, Loaded, Loading, useLogin} from "../hooks/apiCallHooks";
import {Redirect} from "react-router";

import './Login.scss'

const LoginPage = () => {
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
                    <input type="text" name="username" value={userName} required onChange={e => setUserName(e.target.value)}
                           placeholder="felhasznalo42"
                           pattern="[a-zA-Z0-9_]{3,25}"
                           title="3-25 karakter, a-z kisebetű, A-Z nagybetű, 0-9 szám"
                    />
                </div>
                <div>
                    <label htmlFor="password">Jelszó</label>
                    <input type="password" name="password" value={password} required 
                           autoComplete="current-password" onChange={e => setPassword(e.target.value)}
                           placeholder=" "
                           minLength={5}
                    />
                </div>
                <input type="submit" value="Bejelentkezés" />
            </form>
        </div>
    );
}

export default () => {
    const { loginState } = useContext(LoginContext);

    if (loginState instanceof LoggedIn)
        return <Redirect to='/' />;
    
    return <LoginPage />;
}
