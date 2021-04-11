import React, {useCallback, useContext, useEffect, useState} from "react"
import {safeApiFetchWithBodyAsUnit} from "../../repositories/utilities";
import {DIContext} from "../contexts/DIContext";
import {Failed, useApiCall} from "../../hooks/apiCallHooks";
import {NotificationContext} from "../contexts/NotificationProvider";

export default () => {
    const { userRepo } = useContext(DIContext);
    const { notificationDispatch } = useContext(NotificationContext);
    const [selectedFile, setSelectedFile] = useState<File|undefined>(undefined);
    
    const [updateState, updateAvatar] = useApiCall((formData: FormData) =>
        userRepo.updateAvatar(formData)
    , []);
    
    useEffect(() => {
        if(updateState instanceof Failed) {
            notificationDispatch({ type: 'addError', message: `Error updating Avatar: ${updateState.error.message}` });
        }
    }, [updateState]);
    
    const handleSubmit = useCallback(async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if(selectedFile === undefined)
            return;
        
        const formData = new FormData();
        formData.append('file', selectedFile);

        await updateAvatar(formData);
    }, [selectedFile]);
    
    return (
        <form onSubmit={handleSubmit}>
            <input type="file" onChange={e => setSelectedFile(e.target.files?.[0])}/>
            <input type="submit" value={"Upload avatar"} />
        </form>
    )
}