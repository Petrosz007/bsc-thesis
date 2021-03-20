import { StrictMode, useEffect } from 'react';
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
import { useCookieLogin, useLogin } from './hooks/apiCallHooks';
import Booked from './routes/Booked';
import OwnAppointments from './routes/OwnAppointments';
  
import './index.scss';

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
            <div className="main-layout">
                <NavBar className="main-navbar"/>

                <div className="main-content">
                    <Switch>
                        <Route path="/contractor">          <Contractor />          </Route>
                        <Route path ="/booked">             <Booked />              </Route>
                        <Route path ="/own-appointments">   <OwnAppointments />     </Route>
                        <Route path ="/login">              <Login />       </Route>
                        <Route path="/">                    <Home />        </Route>
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
