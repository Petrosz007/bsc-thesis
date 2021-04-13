import React, {useContext, useEffect, useState} from "react";
import {LoggedIn, LoggedOut, LoginContext} from "../components/contexts/LoginProvider";
import {Redirect} from "react-router";
import DataProvider, {DataContext} from "../components/contexts/DataProvider";
import {User, UserSelfInfo} from "../logic/entities";
import {Failed, Idle, Loaded, Loading, useApiCall, useLogout} from "../hooks/apiCallHooks";
import {DIContext} from "../components/contexts/DIContext";
import {NotificationContext} from "../components/contexts/NotificationProvider";
import Modal from "../components/Modal";
import {UserEditor} from "../components/editors/UserEditor";
import AvatarUploader from "../components/editors/AvatarUploader";

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
                <button className="buttonBase inverted" onClick={logout}>Kijelentkezés</button>
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

const ProfileInfo = ({ user }: { user: UserSelfInfo }) => {
    const [editorOpen, setEditorOpen] = useState(false);

    return (
        <>
            {editorOpen &&
                <Modal>
                    <UserEditor user={user} onClose={() => setEditorOpen(false)} />
                </Modal>
            }
            <button onClick={() => setEditorOpen(true)}>Szerkesztés</button>
            {user.contractorPage !== null &&
                <AvatarUploader />
            }
            <UserSelfInfoDisplay user={user} />
        </>
    );
}

export default () => {
    const { loginState } = useContext(LoginContext);

    if (loginState instanceof LoggedOut)
        return <Redirect to='/' />;

    return (
        <DataProvider>
            <ProfileInfo user={loginState.user}/>
        </DataProvider>
    )
}