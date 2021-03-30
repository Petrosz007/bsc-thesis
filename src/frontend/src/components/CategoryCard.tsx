import React from "react";
import { Category } from "../logic/entities";
import UserCard from "./UserCard";

import './CategoryCard.scss';

export default ({ category }: { category: Category }) => {
    return (
        <div className="category-card">
            <p>Id: {category.id}</p>
            <p>Name: {category.name}</p>
            <p>Description: {category.description}</p>
            Users: {category.allowedUsers.map(user => 
                <UserCard user={user} />
            )}
            <p>{category.everyoneAllowed}</p>
            <p>{category.maxAttendees}</p>
            <p>Price: {category.price}</p>
            <UserCard user={category.owner} />
        </div>
    );
};