import React, {useCallback, useState} from "react"
import {safeApiFetchWithBodyAsUnit} from "../../repositories/utilities";

export default () => {
    const [selectedFile, setSelectedFile] = useState<File|undefined>(undefined);
    
    const handleSubmit = useCallback(async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if(selectedFile === undefined)
            return;
        
        const formData = new FormData();
        formData.append('file', selectedFile);
        
        await fetch(`https://localhost:5001/User/Avatar`, {
            credentials: 'include',
            mode: 'cors',
            method: 'POST',
            body: formData,
        });
    }, [selectedFile]);
    
    return (
        <form onSubmit={handleSubmit}>
            <input type="file" onChange={e => setSelectedFile(e.target.files?.[0])}/>
            <input type="submit" value={"Upload avatar"} />
        </form>
    )
}