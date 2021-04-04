import React from "react";
import {User} from "../logic/entities";

import './UserName.scss';

export default ({ user }: { user: User }) => {
    return (
        <span className="username-display">{user.name} <i>@{user.userName}</i></span>
    )
}