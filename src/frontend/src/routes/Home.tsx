import React, {useContext} from "react";
import { Link } from "react-router-dom";

import './Home.scss';
import {LoggedIn, LoggedOut, LoginContext} from "../components/contexts/LoginProvider";

export default () => {
    const { loginState } = useContext(LoginContext);
    
    return (
        <div className="homePage">
            <div className="introText">
                <h1>Időpont foglaló webes alkalmazás</h1>
                <p>Hogy az időpont egyeztetés egyszerű és gyors legyen!</p>
                {loginState instanceof LoggedOut &&
                    <Link to="/register" className="registerButton">Regisztrálok</Link>}
            </div>
            <img src="./calendar-person.svg" alt="Időpontot foglaló személy" />
        </div>
    );
}