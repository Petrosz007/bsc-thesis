import React from "react";
import {Category} from "../logic/entities";
import UserName from "./UserName";

import './DetailViewer.scss';

export default ({ category, onClose }: { category: Category, onClose: () => void }) => {
    return (
        <div className="detailViewer">
            <span className="viewer-header">{category.name}</span>
            <div className="viewer-content">
                <div>
                    <span>Leírás</span>
                    <p>{category.description}</p>
                </div>
                <div>
                    <span>Ár</span>
                    <p>{category.price} Ft</p>
                </div>
                <div>
                    <span>Ajánlott max résztvevők</span>
                    <p>{category.maxAttendees} fő</p>
                </div>
                <div>
                {category.everyoneAllowed
                    ? <span>Nyílt esemény</span>
                    : <>
                        <span>Engedélyezett ügyfelek</span>
                        <p>{category.allowedUsers.length} fő</p>
                        <ul className="allowedUsersList">
                            {category.allowedUsers.map(user =>
                                <li key={user.userName}><UserName user={user} /></li>
                            )}
                        </ul>
                    </>
                }
                </div>
            </div>
            <div className="viewer-footer">
                <button className="viewer-footer" onClick={e => {e.preventDefault(); onClose();}}>Bezárás</button>
            </div>
        </div>
    );
}