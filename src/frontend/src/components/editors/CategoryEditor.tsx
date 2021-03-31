import React, { useContext, useState, useEffect } from "react";
import { useApiCall, Failed, Loaded } from "../../hooks/apiCallHooks";
import { useHandleChange } from "../../hooks/useEditorForm";
import { CategoryDTO } from "../../logic/dtos";
import { User } from "../../logic/entities";
import { DataContext } from "../contexts/DataProvider";
import { DIContext } from "../contexts/DIContext";
import UserAdder from "./UserAdder";

import './CategoryEditor.scss';
import { NotificationContext } from "../contexts/NotificationProvider";

interface CategoryEditdata {
    name: string;
    description: string;
    everyoneAllowed: boolean;
    maxAttendees: number;
    price: number;

    createAnother: boolean;
}

export default ({ owner, onClose }: {
    owner: User,
    onClose: () => void,
}) => {
    const initialCategoryEditorState: CategoryEditdata = {
        name: '',
        description: '',
        everyoneAllowed: true,
        maxAttendees: 1,
        price: 0,
        createAnother: true,
    };

    const { dataDispatch } = useContext(DataContext);
    const { notificationDispatch } = useContext(NotificationContext);
    const { categoryRepo } = useContext(DIContext);

    const [closeAfterLoad, setCloseAfterLoad] = useState(false);
    const [users, setUsers] = useState<User[]>([]);
    const [state, setState] = useState(initialCategoryEditorState);

    const [createCategoryState, createCategory] = useApiCall((dto: CategoryDTO) => 
        categoryRepo.create(dto)
            .sideEffect(category => {
                console.log('Created', category);
                dataDispatch({ type: 'updateCategory', category });
            })
    , []);

    const handleChange = useHandleChange(setState);

    const handleSubmit = (event: React.ChangeEvent<HTMLFormElement>) => {
        setCloseAfterLoad(!state.createAnother);
        createCategory({ 
            ...state,
            allowedUserNames: users.map(u => u.userName),
            ownerUserName: owner.userName,
            id: 0,
        })
        event.preventDefault();
    }

    useEffect(() => {
        if(createCategoryState instanceof Failed) {
            // console.error('Error in CategoryEditor: ', createCategoryState.error);
            notificationDispatch({ type: 'addError', message: `Error in CategoryEditor: ${createCategoryState.error}` })
        }
        if(createCategoryState instanceof Loaded){
            if(closeAfterLoad) {
                onClose();
            } else {
                setState(prevState => ({
                    ...initialCategoryEditorState,
                    createAnother: prevState.createAnother,
                }));
                setUsers([]);
            }
        }
    }, [createCategoryState, closeAfterLoad]);

    return (
        <>
        <form onSubmit={handleSubmit} className="category-editor-form">
            <div className="editor-inputs">
                <label htmlFor="name">Name</label>
                <input type="text" name="name" value={state.name} onChange={handleChange} />
                <label htmlFor="description">Description</label>
                <input type="text" name="description" value={state.description} onChange={handleChange} />
                <label htmlFor="everyoneAllowed">Everyone allowed</label>
                <input type="checkbox" name="everyoneAllowed" checked={state.everyoneAllowed} onChange={handleChange} />
                <label htmlFor="maxAttendees">MaxAttendees</label>
                <input type="number" name="maxAttendees" value={state.maxAttendees} onChange={handleChange} />
                <label htmlFor="price">Price</label>
                <input type="number" name="price" value={state.price} onChange={handleChange} />
            </div>
            
            <div className="editor-user-adder">
                <label htmlFor="allowedUsers">Allowed Users</label>
                <UserAdder users={users} setUsers={setUsers} />
            </div>

            <div className="editor-footer">
                <div className="editor-footer-checkbox">
                    <label htmlFor="createAnother">Create another</label>
                    <input type="checkbox" name="createAnother" checked={state.createAnother} onChange={handleChange} />
                </div>
                <input className="editor-footer-submit" type="submit" value="Submit"/>
                <button onClick={e => {e.preventDefault(); onClose();}}>Cancel</button>
            </div>
        </form>
        </>
    );
}