import { StrictMode, useContext, useEffect } from 'react';
import React from 'react';
import ReactDOM from 'react-dom';
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
import { Failed, Idle, Loading, useLogin } from './hooks/apiCallHooks';
import Booked from './routes/Booked';
import OwnAppointments from './routes/OwnAppointments';
  
import './index.scss';
import { useCookieLogin } from './hooks/useCookieLogin';
import { useEffectAsync, useLayoutEffectAsync } from './hooks/utilities';
import Register from './routes/Register';
import Report from './routes/Reports';
import Reports from './routes/Reports';
import NotificationProvider, { NotificationContext } from './components/contexts/NotificationProvider';
import Profile from "./routes/Profile";
import NotFound from "./routes/NotFound";

const App = () => {
    return (
        <NotificationProvider>
            <LoginProvider>
                <Main />
            </LoginProvider>
        </NotificationProvider>
    );
};

const Main = () => {
    const [loginStatus, login] = useCookieLogin();
    const { notificationDispatch } = useContext(NotificationContext);

    useEffectAsync(login, []);
    useEffect(() => {
        if(loginStatus instanceof Failed) {
            console.error('Error communicating with the server', loginStatus.error);
            notificationDispatch({ type: 'addError', message: 'Error communication with the server' });
        }

    }, [loginStatus]);

    if(loginStatus instanceof Idle || loginStatus instanceof Loading) return <p>Loading...</p>;

    return (
        <Router>
            <div className="main-layout">
                <NavBar className="main-navbar"/>

                <div className="main-content">
                    <Switch>
                        <Route path="/contractor">          <Contractor />          </Route>
                        <Route path="/booked">              <Booked />              </Route>
                        <Route path="/own-appointments">    <OwnAppointments />     </Route>
                        <Route path="/reports">             <Reports />             </Route>
                        <Route path="/profile">             <Profile />             </Route>
                        <Route path="/login">               <Login />               </Route>
                        <Route path="/register">            <Register />            </Route>
                        <Route exact path="/">              <Home />                </Route>
                        <Route>                             <NotFound />            </Route>
                    </Switch>
                </div>
            </div>
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
