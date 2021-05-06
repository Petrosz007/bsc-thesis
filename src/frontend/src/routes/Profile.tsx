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
import {EditIcon, LogoutIcon} from "../SVGs";

import './Profile.scss';

const ProfileInfo = ({ user }: { user: UserSelfInfo }) => {
    const [editorOpen, setEditorOpen] = useState(false);
    const [avatarEditorOpen, setAvatarEditorOpen] = useState(false);
    const [logoutState, logout] = useLogout();
    const { notificationDispatch } = useContext(NotificationContext);

    useEffect(() => {
        if(logoutState instanceof Failed) {
            notificationDispatch({ type: 'addError', message: `Logout failed: ${logoutState.error}` })
        }
    }, [logoutState]);

    return (
        <div className="profilePage">
            {editorOpen &&
                <Modal>
                    <UserEditor user={user} onClose={() => setEditorOpen(false)} />
                </Modal>
            }
            {avatarEditorOpen && user.contractorPage !== null &&
                <Modal>
                    <AvatarUploader onClose={() => setAvatarEditorOpen(false)} user={user} />
                </Modal>
            }
            <div className="editButtons">
                <button onClick={() => setEditorOpen(true)}><EditIcon className="editIcon" />Szerkesztés</button>
                {user.contractorPage !== null &&
                    <button onClick={() => setAvatarEditorOpen(true)}><EditIcon className="editIcon" />Profilkép Frissítése</button>
                }
                <button onClick={logout}><LogoutIcon className="logoutIcon" />Kijelentkezés</button>
            </div>
            
            <div className="profileInfo">
                <h2>Személyes adatok</h2>
                <div className="profileInfoContent">
                    <div>
                        <span>Név</span>
                        <p>{user.name}</p>
                    </div>
                    <div>
                        <span>Felhasználónév</span>
                        <p>{user.userName}</p>
                    </div>
                    <div>
                        <span>Email</span>
                        <p>{user.email}</p>
                    </div>
                    {user.contractorPage !== null &&
                    <>
                        <div>
                            <span>Foglalkozás</span>
                            <p>{user.contractorPage.title}</p>
                        </div>
                        <div>
                            <span>Bio</span>
                            <p>{user.contractorPage.bio}</p>
                        </div>
                    </>
                    }
                </div>
            </div>
        </div>
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