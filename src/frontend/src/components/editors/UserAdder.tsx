import React, {useCallback, useContext, useEffect, useState} from "react"
import {useApiCall, Loading, Failed, Idle} from "../../hooks/apiCallHooks";
import { User } from "../../logic/entities";
import { setValue } from "../../utilities/listExtensions";
import { DIContext } from "../contexts/DIContext";
import { NotificationContext } from "../contexts/NotificationProvider";
import {ResultPromise} from "../../utilities/result";
import UserName from "../UserName";
import UserSelector from "../inputs/UserSelector";

import './UserAdder.scss';
import {DeleteIcon} from "../../SVGs";

const UserAdder = ({ usersToSelectFrom, users, setUsers, allowedUsers, max }: {
    usersToSelectFrom: User[],
    users: User[],
    setUsers: React.Dispatch<React.SetStateAction<User[]>>,
    allowedUsers?: User[],
    max?: number,
}) => {
    // const { userRepo } = useContext(DIContext);
    // const { notificationDispatch } = useContext(NotificationContext);

    const [selectedUser, setSelectedUser] = useState(usersToSelectFrom[0]);

    // const [addState, add] = useApiCall(() =>
    //         userRepo.getByUserName(selectedUser.userName)
    //             .andThen(user =>
    //                 allowedUsers === undefined || allowedUsers.some(u => u.userName === user.userName)
    //                     ? ResultPromise.ok<User,Error>(user)
    //                     : ResultPromise.err<User,Error>(new Error(`${user.name} nem engedélyezett résztvevő a kategórián. Szekeszd a kategóriát, ha hozzá szeretnéd adni.`))
    //             )
    //             .sideEffect(user => {
    //                 setUsers(prevState => setValue(prevState, user, u => u.userName));
    //             })
    //     , [selectedUser]);
    
    const add = useCallback((user: User) => {
        setUsers(prevState => setValue(prevState, user, u => u.userName))
    }, [setUsers]);

    // useEffect(() => {
    //     if(addState instanceof Failed) {
    //         notificationDispatch({ type: 'addError', message: `${addState.error}` });
    //     }
    // }, [addState]);

    const remove = useCallback((userName: string) => {
        setUsers(prevState => prevState.filter(u => u.userName !== userName));
    }, [setUsers]);

    return (
        <div className="userAdder">
            <div className="userAdderInputs">
                <UserSelector className="userSelector" selectedUser={selectedUser} setSelectedUser={setSelectedUser} users={usersToSelectFrom} />
                <button className="addButton" 
                        onClick={e => {add(selectedUser); e.preventDefault()}} 
                        disabled={max !== undefined && users.length >= max}
                >
                    {max !== undefined && users.length >= max ? 'Betelt' : 'Felvétel'}
                </button>
            </div>
            <ul className="userAdderUsers">
            {users.map(user =>
                <li key={user.userName}>
                    <UserName user={user} />
                    <button onClick={e => {remove(user.userName); e.preventDefault()}}><DeleteIcon className="deleteIcon"/></button>
                </li>
            )}
            </ul>
        </div>
    )
}


export default ({ users, setUsers, allowedUsers, max }: { 
    users: User[],
    setUsers: React.Dispatch<React.SetStateAction<User[]>>,
    allowedUsers?: User[],
    max?: number,
}) => {
    const { userRepo } = useContext(DIContext);
    const { notificationDispatch } = useContext(NotificationContext);
    
    const [allUsersState, getAllUsers] = useApiCall(() => 
        userRepo.getAllUsers()
        , []);
    
    useEffect(() => {
        if(allUsersState instanceof Failed) {
            notificationDispatch({ type: 'addError', message: `Hiba az összes felhasználó betöltése közben: ${allUsersState.error.message}` });
        }
        if(allUsersState instanceof Idle) {
            getAllUsers();
        }
    }, [allUsersState, getAllUsers, notificationDispatch]);
    
    if(allUsersState instanceof Loading || allUsersState instanceof Idle)
        return <div>Loading...</div>;
    
    if(allUsersState instanceof Failed)
        return null;
    
    return (
        <UserAdder 
            usersToSelectFrom={allowedUsers ?? allUsersState.value}
            users={users}
            setUsers={setUsers}
            allowedUsers={allowedUsers}
            max={max}
        />
    )
}
