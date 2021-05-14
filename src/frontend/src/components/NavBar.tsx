import React, { useContext } from "react";
import { NavLink } from "react-router-dom";
import { LoggedIn, LoginContext } from "./contexts/LoginProvider";

import './NavBar.scss';

export default ({ className }: React.HTMLAttributes<HTMLDivElement>) => {
    const { loginState } = useContext(LoginContext);

    return (
        <nav className={`navbar ${className}`}>
            <div className="navLeft">
                <p>Időpont foglaló Webes Alkalmazás</p>
                <div className="navLinks">
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
            </div>
            <div className="navRight">
                {loginState instanceof LoggedIn
                    ? <NavLink to="/profile">{loginState.user.name}</NavLink>
                    : <>
                        <NavLink to="/login">Bejelentkezés</NavLink>
                        <NavLink to="/register">Regisztráció</NavLink>
                    </>
                }
            </div>
        </nav>
    );
}