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
            .sideEffect(_ => {
                notificationDispatch({ type: 'addSuccess', message: `Profilkép sikeresen frissítve!` });
            })
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
        
        if(selectedFile.size > 2_000_000) {
            notificationDispatch({ type: 'addError', message: `Maximum 2Mb lehet a profilkép, a feltöltött fájl ${(selectedFile.size / 1_000_000).toFixed(2)}Mb!` });
            return;
        }
        
        if(!['image/png','image/jpeg'].includes(selectedFile.type)) {
            notificationDispatch({ type: 'addError', message: `Csak PNG és JPEG típusú lehet a profilkép, '${selectedFile.type}' nem!` });
            return;
        }
        
        const formData = new FormData();
        formData.append('file', selectedFile);

        await updateAvatar(formData);
    }, [selectedFile]);
    
    return (
        <div>
            <form onSubmit={handleSubmit}>
                <input type="file"
                       accept=".png, .jpg, .jpeg"
                       onChange={e => setSelectedFile(e.target.files?.[0])}/>
                <input type="submit" value={"Upload avatar"} />
            </form>
            {selectedFile !== undefined && <img src={URL.createObjectURL(selectedFile)} alt="Profilkép megtekintő"/>}
        </div>
    )
}