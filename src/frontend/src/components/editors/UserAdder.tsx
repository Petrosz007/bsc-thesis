import React, { useContext, useEffect, useState } from "react"
import {useApiCall, Loading, Failed, Idle} from "../../hooks/apiCallHooks";
import { User } from "../../logic/entities";
import { setValue } from "../../utilities/listExtensions";
import { DIContext } from "../contexts/DIContext";
import { NotificationContext } from "../contexts/NotificationProvider";
import {ResultPromise} from "../../utilities/result";
import UserName from "../UserName";
import UserSelector from "../inputs/UserSelector";

const UserAdder = ({ usersToSelectFrom, users, setUsers, allowedUsers, max }: {
    usersToSelectFrom: User[],
    users: User[],
    setUsers: React.Dispatch<React.SetStateAction<User[]>>,
    allowedUsers?: User[],
    max?: number,
}) => {
    const { userRepo } = useContext(DIContext);
    const { notificationDispatch } = useContext(NotificationContext);

    const [selectedUser, setSelectedUser] = useState(usersToSelectFrom[0]);

    const [addState, add] = useApiCall(() =>
            userRepo.getByUserName(selectedUser.userName)
                .andThen(user =>
                    allowedUsers === undefined || allowedUsers.some(u => u.userName === user.userName)
                        ? ResultPromise.ok<User,Error>(user)
                        : ResultPromise.err<User,Error>(new Error(`${user.userName} is not allowed on this category. Edit the category accordingly.`))
                )
                .sideEffect(user => {
                    setUsers(prevState => setValue(prevState, user, u => u.userName));
                })
        , [selectedUser]);

    useEffect(() => {
        if(addState instanceof Failed) {
            notificationDispatch({ type: 'addError', message: `Error in UserAdder ${addState.error}` });
        }
    }, [addState]);

    const remove = (userName: string) => {
        setUsers(prevState => prevState.filter(u => u.userName !== userName));
    };

    return (
        <div>
            <UserSelector selectedUser={selectedUser} setSelectedUser={setSelectedUser} users={usersToSelectFrom} />
            {addState instanceof Loading
                ? <span>Loading...</span>
                : <button onClick={e => {add(); e.preventDefault()}} disabled={max !== undefined && users.length >= max}>Add</button>}<br/>
            {users.map(user =>
                <React.Fragment key={user.userName}>
                    <UserName user={user} />
                    <button onClick={e => {remove(user.userName); e.preventDefault()}}>X</button>
                    <br/>
                </React.Fragment>
            )}
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
            usersToSelectFrom={allUsersState.value}
            users={users}
            setUsers={setUsers}
            allowedUsers={allowedUsers}
            max={max}
        />
    )
}
