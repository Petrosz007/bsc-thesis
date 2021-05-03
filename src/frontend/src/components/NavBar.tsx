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
            <button className="buttonBase inverted" onClick={()=>history.push('/login')}>Bejelentkezés</button>
            <button className="buttonBase" onClick={()=>history.push('/register')}>Regisztráció</button>
        </>
    );
}

const LoggedInComponent = ({ user }: { user: User }) => {
    return (
        <>
            <NavLink to="/booked">Foglalások</NavLink>
            {user.contractorPage !== null &&
            <>
                <NavLink to="/own-appointments">Vállalkozói oldal</NavLink>
                <NavLink to="/reports">Számlázás</NavLink>
            </>
            }
            <NavLink to="/profile">{user.name}</NavLink>
        </>
    );
}

export default ({ className }: React.HTMLAttributes<HTMLDivElement>) => {
    const { loginState } = useContext(LoginContext);
    const history = useHistory();

    return (
        <nav className={`navbar ${className}`}>
            <p>Időpontfoglaló Webes Alkalmazás</p>
            <div className="navCenter">
                <NavLink to="/" exact={true}>Kezdőlap</NavLink>
                <NavLink to="/contractor">Vállalkozók</NavLink>
                {loginState instanceof LoggedIn &&
                <>
                    <NavLink to="/booked">Foglalások</NavLink>
                    {loginState.user.contractorPage !== null &&
                    <>
                        <NavLink to="/own-appointments">Vállalkozói oldal</NavLink>
                        <NavLink to="/reports">Számlázás</NavLink>
                    </>
                    }
                </>
                }
            </div>
            <div className="navRight">
                {loginState instanceof LoggedIn
                    ? <NavLink to="/profile">{loginState.user.name}</NavLink>
                    : <>
                        <button className="buttonBase inverted" onClick={()=>history.push('/login')}>Bejelentkezés</button>
                        <button className="buttonBase" onClick={()=>history.push('/register')}>Regisztráció</button>
                    </>
                }
            </div>
        </nav>
    );
}