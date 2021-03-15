import { StrictMode, useEffect } from 'react';
import React from 'react';
import ReactDOM from 'react-dom';
import './App.scss';
import LoginProvider from './components/contexts/LoginProvider';
import NavBar from './components/NavBar';
import DataProvider from './components/contexts/DataProvider';
import {
    BrowserRouter as Router,
    Switch,
    Route,
    Link
  } from "react-router-dom";
import Home from './routes/Home';
import Contractor from './routes/Contractor';
import Login from './routes/Login';
import { useCookieLogin, useLogin } from './hooks/apiCallHooks';
  

const App = () => {
    return (
        <LoginProvider>
            <Main />
        </LoginProvider>
    );
};

const Main = () => {
    const cookieLogin = useCookieLogin();

    useEffect(() => {
        cookieLogin();
    }, []);

    return (
        <Router>
            <NavBar />

            <Switch>
                <Route path="/contractor">
                    <Contractor />
                </Route>
                <Route path ="/login">
                    <Login />
                </Route>
                <Route path="/">
                    <Home />
                </Route>
            </Switch>
        </Router>
    );
};

ReactDOM.render(
    <StrictMode>
        <App />
    </StrictMode>,
    document.getElementById('root')
);

 // Hot Module Replacement (HMR) - Remove this snippet to remove HMR.
 // Learn more: https://www.snowpack.dev/concepts/hot-module-replacement
if (import.meta.hot) {
    import.meta.hot.accept();
}
