import React, { useContext, useEffect, useState } from "react"
import { useApiCall, Loading, Failed } from "../../hooks/apiCallHooks";
import { User } from "../../logic/entities";
import { setValue } from "../../utilities/listExtensions";
import { DIContext } from "../contexts/DIContext";
import { NotificationContext } from "../contexts/NotificationProvider";

export default ({ users, setUsers }: { users: User[], setUsers: React.Dispatch<React.SetStateAction<User[]>> }) => {
    const { userRepo } = useContext(DIContext);
    const { notificationDispatch } = useContext(NotificationContext);

    const [userName, setUserName] = useState("");

    const [addState, add] = useApiCall(() =>
        userRepo.getByUserName(userName).sideEffect(user => {
            setUsers(prevState => setValue(prevState, user, u => u.userName));
        })
    , [userName]);

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
            <input type="text" value={userName} onChange={e => setUserName(e.target.value)} />
            {addState instanceof Loading
                ? <span>Loading...</span>
                : <button onClick={e => {add(); e.preventDefault()}}>Add</button>}<br/>
            {users.map(user => 
                <React.Fragment key={user.userName}>
                    <span>{user.name} ({user.userName})</span>
                    <button onClick={e => {remove(user.userName); e.preventDefault()}}>X</button>
                    <br/>
                </React.Fragment>
            )}     
        </div>
    )
}
