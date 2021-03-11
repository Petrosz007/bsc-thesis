import React, { useContext } from "react";
import { User } from "src/logic/entities";
import { LoggedIn, LoggedOut, LoginContext } from "./contexts/LoginProvider";

import './NavBar.scss';

const LoggedOutConponent = () => 
    <div className="loginDiv">
        <button className="buttonBase inverted">Log In</button>
        <button className="buttonBase">Register</button>
    </div>;

const LoggedInComponent = ({ user }: { user: User }) => 
    <div className="loginDiv">
        <p>Hello {user.name}!</p>
        <button className="buttonBase">Log Out</button>
    </div>;

export default () => {
    const { loginState } = useContext(LoginContext);

    return (
        <nav className="navbar">
            <p>Időpontfoglaló Webes Alkalmazás</p>
            {loginState instanceof LoggedOut && 
                <LoggedOutConponent /> 
            }
            {loginState instanceof LoggedIn && 
                <LoggedInComponent user={loginState.user} /> 
            }
        </nav>
    );
}