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
    return (
        <>
            <NavLink to="/booked">Booked</NavLink>
            {user.contractorPage !== null &&
            <>
                <NavLink to="/own-appointments">Own Appointments</NavLink>
                <NavLink to="/reports">Reports</NavLink>
            </>
            }
            <NavLink to="/profile">{user.name}</NavLink>
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
                <NavLink to="/contractor">Contractors</NavLink>
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