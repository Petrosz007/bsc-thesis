import React, {useContext} from 'react';
import { User } from '../logic/entities';

import './UserCard.scss';

export default ({ user }: { user: User }) => {
    return (
        <div className="user-card">
            <p>{user.userName}</p>
            <p>{user.name}</p>
            {user.contractorPage && 
            <>
                <p>{user.contractorPage.title}</p>
                <p>{user.contractorPage.bio}</p>
            </>
            }
        </div>
    );
};