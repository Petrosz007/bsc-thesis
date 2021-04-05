import {Appointment, Category, User} from "../../logic/entities";
import {AppointmentDTO} from "../../logic/dtos";
import {ResultPromise} from "../../utilities/result";
import {DateTime} from "luxon";
import React, {useCallback, useContext, useEffect, useState} from "react";
import {DataAction, DataContext} from "../contexts/DataProvider";
import {NotificationContext} from "../contexts/NotificationProvider";
import {Failed, Loaded, useApiCall} from "../../hooks/apiCallHooks";
import UserAdder from "./UserAdder";

export function EditorBase<TEditorState extends { createAnother: boolean },TEntity,TDto>({
    state, editorStateToDto, apiCall, onClose, handleChange, labels, children, dataDispatchAction
}: {
    state: TEditorState,
    editorStateToDto: (_: TEditorState) => TDto,
    apiCall: (_x: TDto) => ResultPromise<TEntity,Error>,
    onClose: () => void,
    handleChange: (event: React.ChangeEvent<HTMLInputElement>) => void
    labels: {
        createAnother: string,
        submit: string,
    },
    children: React.ReactNode,
    dataDispatchAction: (_: TEntity) => DataAction,
}) {
    const { notificationDispatch } = useContext(NotificationContext);
    const { dataDispatch } = useContext(DataContext);

    const [closeAfterLoad, setCloseAfterLoad] = useState(false);

    const [makeApiCallState, makeApiCall] = useApiCall((dto: TDto) =>
            apiCall(dto)
                .sideEffect(result => {
                    console.log('result', result);
                    dataDispatch(dataDispatchAction(result));
                })
        , [apiCall, dataDispatchAction]);
    
    const handleSubmit = useCallback((event: React.ChangeEvent<HTMLFormElement>) => {
        console.log(state);
        setCloseAfterLoad(!state.createAnother);
        makeApiCall(editorStateToDto(state));

        event.preventDefault();
    }, [state, setCloseAfterLoad, makeApiCall, editorStateToDto])

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
            <form onSubmit={handleSubmit} className="appointment-editor-form">
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