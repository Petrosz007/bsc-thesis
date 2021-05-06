import React, {useCallback, useContext, useEffect, useState} from "react"
import {safeApiFetchWithBodyAsUnit} from "../../repositories/utilities";
import {DIContext} from "../contexts/DIContext";
import {Failed, useApiCall} from "../../hooks/apiCallHooks";
import {NotificationContext} from "../contexts/NotificationProvider";
import {EditorBase} from "./EditorBase";
import {useHandleChange} from "../../hooks/useEditorForm";
import {User} from "../../logic/entities";

import './AvatarUploader.scss';

interface AvatarEditData {
    avatarFile?: File|undefined,
    createAnother: boolean
}

export default ({ onClose, user }: {
    onClose: () => void,
    user: User,
}) => {
    const { userRepo, config } = useContext(DIContext);
    const { notificationDispatch } = useContext(NotificationContext);
    const [state, setState] = useState<AvatarEditData>({
        avatarFile: undefined,
        createAnother: false,
    });
    
    const updateAvatar = useCallback((formData: FormData) =>
        userRepo.updateAvatar(formData)
    , [userRepo]);
    
    const editorStateToDto = useCallback((editData: AvatarEditData) => {
        const formData = new FormData();
        formData.append('file', editData.avatarFile ?? new Blob());
        return formData;
    }, []);
    
    const validator = useCallback(({ avatarFile: selectedFile }: AvatarEditData) => {
        if(selectedFile === undefined) {
            notificationDispatch({ type: 'addError', message: `Nincs kiválasztva fájl!` });
            return false;
        }
        
        if(selectedFile.size > 2_000_000) {
            notificationDispatch({ 
                type: 'addError', 
                message: `Maximum 2MB lehet a profilkép, a feltöltött fájl ${(selectedFile.size / 1_000_000).toFixed(2)}MB!` 
            });
            return false;
        }
        
        if(!['image/png','image/jpeg'].includes(selectedFile.type)) {
            notificationDispatch({ type: 'addError', message: `Csak PNG és JPEG típusú lehet a profilkép, '${selectedFile.type}' nem!` });
            return false;
        }
        
        return true;
    }, [notificationDispatch]);
    
    const handleChange = useHandleChange<AvatarEditData>(setState);
    
    return (
        <EditorBase state={state}
                    editorStateToDto={editorStateToDto}
                    apiCall={updateAvatar}
                    onClose={onClose}
                    handleChange={handleChange}
                    labels={{
                        header: 'Profilkép szerkesztése',
                        createAnother: 'Maradok szerkeszteni',
                        submit: 'Profilkép frissítése',
                    }}
                    validator={validator}
        >
            <input type="file"
                   accept=".png, .jpg, .jpeg"
                   name="avatarFile"
                   onChange={e => setState(prevState => ({ ...prevState, avatarFile: e.target.files?.[0] }))}/>
            <img className="avatarEditorPreview" 
                 src={state.avatarFile !== undefined
                    ? URL.createObjectURL(state.avatarFile)
                    : `${config.apiUrl}/User/Avatar/${user.userName}`}
                 alt="Profilkép megtekintő"/>
        </EditorBase>
    )
}