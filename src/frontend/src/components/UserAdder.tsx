import React, { useContext, useState } from "react"
import { Failed, Loading, useApiCall } from "../hooks/apiCallHooks";
import { User } from "../logic/entities";
import { setValue } from "../utilities/listExtensions";
import { DIContext } from "./contexts/DIContext";

export default ({ users, setUsers }: { users: User[], setUsers: React.Dispatch<React.SetStateAction<User[]>> }) => {
    const { userRepo } = useContext(DIContext);
    const [userName, setUserName] = useState("");

    const [addState, add] = useApiCall(() =>
        userRepo.getByUserName(userName).sideEffect(user => {
            setUsers(prevState => setValue(prevState, user, u => u.userName));
        })
    , [userName]);

    const remove = (userName: string) => {
        setUsers(prevState => prevState.filter(u => u.userName !== userName));
    };

    return (
        <div>
            <input type="text" value={userName} onChange={e => setUserName(e.target.value)} />
            {addState instanceof Loading
                ? <span>Loading...</span>
                : <button onClick={e => {add(); e.preventDefault()}}>Add</button>}<br/>
            {addState instanceof Failed && <p>Failed: {addState.error.message}</p>}
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
