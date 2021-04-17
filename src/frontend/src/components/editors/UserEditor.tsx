import React, {useContext, useState, useCallback} from "react";
import { useHandleChange } from "../../hooks/useEditorForm";
import {UserEditDTO} from "../../logic/dtos";
import {UserSelfInfo} from "../../logic/entities";
import { DIContext } from "../contexts/DIContext";
import {ResultPromise} from "../../utilities/result";
import {EditorBase} from "./EditorBase";

import './CategoryEditor.scss';
import {LoginContext} from "../contexts/LoginProvider";

interface UserEditData {
    email: string;
    name: string;
    contractorPage__title?: string;
    contractorPage__bio?: string;

    createAnother: boolean;
}

const UserEditorBase = ({ initialUser, apiCall, onClose, labels }: {
    initialUser: UserSelfInfo,
    apiCall: (_: UserEditDTO) => ResultPromise<UserSelfInfo, Error>,
    onClose: () => void,
    labels: {
        createAnother: string,
        submit: string,
    },
}) => {
    const initialUserEditorState: UserEditData = {
        ...initialUser,
        contractorPage__title: initialUser.contractorPage?.title,
        contractorPage__bio: initialUser.contractorPage?.bio,
        createAnother: false,
    };

    const [state, setState] = useState(initialUserEditorState);

    const handleChange = useHandleChange(setState);

    const editorStateToDto = useCallback((editorState: UserEditData): UserEditDTO => ({
        ...editorState,
        contractorPage: initialUser.contractorPage !== null
            ? {
                title: editorState.contractorPage__title!,
                bio: editorState.contractorPage__bio!
            }
            : null,
    }), [initialUser]);

    return (
        <EditorBase
            state={state}
            apiCall={apiCall}
            handleChange={handleChange}
            editorStateToDto={editorStateToDto}
            labels={labels}
            onClose={onClose}
            dataDispatchAction={user => ({ type: 'updateUser', user })}
        >
            <label htmlFor="name">Név</label>
            <input type="text" name="name" value={state.name} required={true} onChange={handleChange} />
            
            <label htmlFor="description">Email</label>
            <input type="email" name="email" value={state.email} required={true} onChange={handleChange} />
            
            {initialUser.contractorPage !== null && <>
                <label htmlFor="allowedUsers">Foglalkozás</label>
                <input type="text" name="contractorPage__title" value={state.contractorPage__title} required={true} onChange={handleChange} />
                
                <label htmlFor="allowedUsers">Magamról</label>
                <textarea name="contractorPage__bio" value={state.contractorPage__bio} required={true} onChange={handleChange} rows={3} />
            </>}
        </EditorBase>
    );
}

export const UserEditor = ({ user, onClose }: {
    user: UserSelfInfo,
    onClose: () => void,
}) => {
    const { userRepo } = useContext(DIContext);
    const { loginDispatch } = useContext(LoginContext);

    const update = useCallback((dto: UserEditDTO) =>
            userRepo.update(dto)
                .andThen(_ => userRepo.getSelfInfo())
                .sideEffect(user => {
                    loginDispatch({ type: 'login', user });
                })
        , []);

    return (
        <UserEditorBase
            apiCall={update}
            initialUser={user}
            onClose={onClose}
            labels={{ createAnother: 'Maradok szerkeszteni', submit: 'Mentés' }}
        />
    );
}