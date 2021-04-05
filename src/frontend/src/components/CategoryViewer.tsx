import React from "react";
import {Appointment, Category} from "../logic/entities";

import './CategoryViewer.scss';
import UserName from "./UserName";
import {DateTime} from "luxon";

export default ({ category, onClose }: { category: Category, onClose: () => void }) => {
    return (
        <div className="category-viewer">
            <div className="viewer-content">
                <span className="appointment-header">{category.name}</span>
                <p>{category.description}</p>
                <p>{category.price} Ft</p>
                <p>Max résztvevők: {category.maxAttendees}</p>
                {category.everyoneAllowed
                    ? <p>Nyílt esemény</p>
                    :
                    <>
                        <p>Engedélyezett felhasználók: {category.allowedUsers.length} fő</p>
                        <ul>
                            {category.allowedUsers.map(user =>
                                <li key={user.userName}><UserName user={user} /></li>
                            )}
                        </ul>
                    </>
                }
            </div>
            <div className="viewer-footer">
                <button className="viewer-footer" onClick={e => {e.preventDefault(); onClose();}}>Mégse</button>
            </div>
        </div>
    );
}