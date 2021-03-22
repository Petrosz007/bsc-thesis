import { Failed, Loaded, Loading, useLogout } from "../hooks/apiCallHooks";
import React, { useContext, useEffect } from "react";
import { NavLink, Redirect, useHistory } from "react-router-dom";
import { User } from "../logic/entities";
import { LoggedIn, LoggedOut, LoginContext } from "./contexts/LoginProvider";

import './NavBar.scss';

const LoggedOutConponent = () => {
    const history = useHistory();
    return (
        <>
            <button className="buttonBase inverted" onClick={()=>history.push('/login')}>Log In</button>
            <button className="buttonBase" onClick={()=>history.push('/register')}>Register</button>
        </>
    );
}

const LoggedInComponent = ({ user }: { user: User }) => {
    const [logoutState, logout] = useLogout();

    useEffect(() => {
        if(logoutState instanceof Failed) {
            console.error('Logout failed', logoutState.error);
        }
    }, [logoutState]);

    if(logoutState instanceof Loading) return <p>Logging out...</p>;

    return (
        <>
            <NavLink to="/booked">Booked</NavLink>
            {user.contractorPage !== null &&
            <>
                <NavLink to="/own-appointments">Own Appointments</NavLink>
                <NavLink to="/reports">Reports</NavLink>
            </>
            }
            <p>Hello {user.name}!</p>
            <button className="buttonBase inverted" onClick={logout}>Log Out</button>
        </>
    );
}

export default ({ className }: React.HTMLAttributes<HTMLDivElement>) => {
    const { loginState } = useContext(LoginContext);

    return (
        <nav className={`navbar ${className}`}>
            <p>Időpontfoglaló Webes Alkalmazás</p>
            <div className="navRight">
                <NavLink to="/" exact={true}>Home</NavLink>
                {loginState instanceof LoggedOut && 
                    <LoggedOutConponent /> 
                }
                {loginState instanceof LoggedIn && 
                    <LoggedInComponent user={loginState.user} /> 
                }
            </div>
        </nav>
    );
}