import React, {useContext, useEffect} from "react";
import {LoggedIn, LoggedOut, LoginContext} from "../components/contexts/LoginProvider";
import {Redirect} from "react-router";
import DataProvider from "../components/contexts/DataProvider";
import {User, UserSelfInfo} from "../logic/entities";
import {Failed, Idle, Loaded, Loading, useApiCall, useLogout} from "../hooks/apiCallHooks";
import {DIContext} from "../components/contexts/DIContext";
import {NotificationContext} from "../components/contexts/NotificationProvider";

const UserSelfInfoDisplay = ({ user }: { user: UserSelfInfo }) => {
    const [logoutState, logout] = useLogout();
    const { notificationDispatch } = useContext(NotificationContext);

    useEffect(() => {
        if(logoutState instanceof Failed) {
            notificationDispatch({ type: 'addError', message: `Logout failed: ${logoutState.error}` })
        }
    }, [logoutState]);

    return (
        <div>
            {logoutState instanceof Loading && <p>Logging out...</p>}
            {logoutState instanceof Idle &&
                <button className="buttonBase inverted" onClick={logout}>Log Out</button>
            }

            <p>Név: {user.name}</p>
            <p>Felhasználónév: {user.userName}</p>
            <p>Email: {user.email}</p>
            {user.contractorPage !== null &&
            <>
                <p>Foglalkozás: {user.contractorPage.title}</p>
                <p>Bio: {user.contractorPage.bio}</p>
            </>
            }
        </div>
    )
}

const ProfileInfo = () => {
    const { userRepo } = useContext(DIContext);
    const { notificationDispatch } = useContext(NotificationContext);
    
    const [state, refreshData] = useApiCall(() => 
        userRepo.getSelfInfo()
        , []);

    useEffect(() => {
        if(state instanceof Failed) {
            console.error("Error in Profile.tsx", state.error);
            notificationDispatch({ type: 'addError', message: `Error in Profile: ${state.error}` });
        }
        else if(state instanceof Idle) {
            refreshData();
        }
    }, [state]);
    
    return (
        <>
        {state instanceof Loading && <p>Loading...</p>}
        {state instanceof Loaded &&
            <UserSelfInfoDisplay user={state.value} />
        }
        </>
    );
}

export default () => {
    const { loginState } = useContext(LoginContext);

    if (loginState instanceof LoggedOut)
        return <Redirect to='/' />;

    return (
        <DataProvider>
            <ProfileInfo />
        </DataProvider>
    )
}