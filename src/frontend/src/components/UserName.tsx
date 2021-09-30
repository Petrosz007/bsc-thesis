import React from "react";
import {User} from "../logic/entities";

import './UserName.scss';

export default ({ user, className }: { user: User, className?: string }) => {
    return (
        <span className={`username-display ${className}`}>{user.name} <i>@{user.userName}</i></span>
    )
}