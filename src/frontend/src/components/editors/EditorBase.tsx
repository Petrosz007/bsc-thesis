import {ResultPromise} from "../../utilities/result";
import React, {useCallback, useContext, useEffect, useState} from "react";
import {DataAction, DataContext} from "../contexts/DataProvider";
import {NotificationContext} from "../contexts/NotificationProvider";
import {Failed, Loaded, useApiCall} from "../../hooks/apiCallHooks";

import './EditorBase.scss';

export function EditorBase<TEditorState extends { createAnother: boolean },TEntity,TDto>({
    state, editorStateToDto, apiCall, onClose, handleChange, labels, children, dataDispatchAction, validator
}: {
    state: TEditorState,
    editorStateToDto: (_: TEditorState) => TDto,
    apiCall: (_x: TDto) => ResultPromise<TEntity,Error>,
    onClose: () => void,
    handleChange: (event: React.ChangeEvent<HTMLInputElement & HTMLSelectElement & HTMLTextAreaElement>) => void
    labels: {
        header: string,
        createAnother: string,
        submit: string,
    },
    children: React.ReactNode,
    dataDispatchAction?: (_: TEntity) => DataAction,
    validator?: (_: TEditorState) => boolean,
}) {
    const { notificationDispatch } = useContext(NotificationContext);
    const { dataDispatch } = useContext(DataContext);

    const [closeAfterLoad, setCloseAfterLoad] = useState(false);

    const [makeApiCallState, makeApiCall] = useApiCall((dto: TDto) =>
            apiCall(dto)
                .sideEffect(result => {
                    if(dataDispatchAction !== undefined)
                        dataDispatch(dataDispatchAction(result));
                })
        , [apiCall, dataDispatchAction]);
    
    const handleSubmit = useCallback((event: React.ChangeEvent<HTMLFormElement>) => {
        event.preventDefault();
        
        if(validator !== undefined && !validator(state)) return;
            
        setCloseAfterLoad(!state.createAnother);
        makeApiCall(editorStateToDto(state));
    }, [state, setCloseAfterLoad, makeApiCall, editorStateToDto, validator])

    useEffect(() => {
        if(makeApiCallState instanceof Failed) {
            console.error('Error in EditorBase: ', makeApiCallState.error);
            notificationDispatch({ type: 'addError', message: `Error: ${makeApiCallState.error}` });
        }
        if(makeApiCallState instanceof Loaded && closeAfterLoad){
            onClose();
        }
    }, [makeApiCallState, closeAfterLoad]);

    return (
        <>
            <form onSubmit={handleSubmit} className="editor-form">
                <h2>{labels.header}</h2>
                <div className="editor-inputs">
                    {children}
                </div>

                <div className="editor-footer">
                    <div className="editor-footer-checkbox">
                        <input type="checkbox" name="createAnother" checked={state.createAnother} onChange={handleChange} />
                        <label htmlFor="createAnother">{labels.createAnother}</label>
                    </div>
                    <input className="editor-footer-submit" type="submit" value={labels.submit}/>
                    <button onClick={e => {e.preventDefault(); onClose();}}>Mégse</button>
                </div>
            </form>
        </>
    );
}