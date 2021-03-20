import React, { useCallback, useContext, useState } from "react"
import { Failed, Loading, useApiCall } from "../hooks/apiCallHooks";
import { DataContext } from "./contexts/DataProvider";
import { DIContext } from "./contexts/DIContext";

export default () => {
    const { userRepo } = useContext(DIContext);
    const { dataState, dataDispatch } = useContext(DataContext);
    const [userName, setUserName] = useState("");

    const [addState, add] = useApiCall(() =>
        userRepo.getByUserName(userName).sideEffect(user => {
            dataDispatch({ type: 'updateUser', user });
        })
    , [userName]);

    const remove = (userName: string) => {
        dataDispatch({ type: 'deleteUser', userName });
    };

    return (
        <div>
            <input type="text" value={userName} onChange={e => setUserName(e.target.value)} />
            {addState instanceof Loading
                ? <span>Loading...</span>
                : <button onClick={e => {add(); e.preventDefault()}}>Add</button>}<br/>
            {addState instanceof Failed && <p>Failed: {addState.error.message}</p>}
            {dataState.users.map(user => 
                <React.Fragment key={user.userName}>
                    <span>{user.name} ({user.userName})</span>
                    <button onClick={e => {remove(user.userName); e.preventDefault()}}>X</button>
                    <br/>
                </React.Fragment>
            )}     
        </div>
    )
}