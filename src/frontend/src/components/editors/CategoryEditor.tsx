import React, {useContext, useState, useEffect, useCallback} from "react";
import { useApiCall, Failed, Loaded } from "../../hooks/apiCallHooks";
import { useHandleChange } from "../../hooks/useEditorForm";
import {AppointmentDTO, CategoryDTO} from "../../logic/dtos";
import {Category, User} from "../../logic/entities";
import { DataContext } from "../contexts/DataProvider";
import { DIContext } from "../contexts/DIContext";
import UserAdder from "./UserAdder";

import './CategoryEditor.scss';
import { NotificationContext } from "../contexts/NotificationProvider";
import {ResultPromise} from "../../utilities/result";

interface CategoryEditdata {
    id: number;
    name: string;
    description: string;
    everyoneAllowed: boolean;
    maxAttendees: number;
    price: number;

    createAnother: boolean;
}

const CategoryEditorBase = ({ initialCategory, apiCall, owner, onClose, labels }: {
    initialCategory?: Category,
    apiCall: (_: CategoryDTO) => ResultPromise<Category, Error>,
    owner: User,
    onClose: () => void,
    labels: {
        createAnother: string,
        submit: string,
    },
}) => {
    const initialCategoryEditorState: CategoryEditdata = 
        initialCategory === undefined
            ? {
                id: 0,
                name: '',
                description: '',
                everyoneAllowed: true,
                maxAttendees: 1,
                price: 0,
                createAnother: false,
            }
            : {
                id: initialCategory.id,
                name: initialCategory.name,
                description: initialCategory.description,
                everyoneAllowed: initialCategory.everyoneAllowed,
                maxAttendees: initialCategory.maxAttendees,
                price: initialCategory.price,
                createAnother: false,
            }

    const { dataDispatch } = useContext(DataContext);
    const { notificationDispatch } = useContext(NotificationContext);

    const [closeAfterLoad, setCloseAfterLoad] = useState(false);
    const [users, setUsers] = useState<User[]>(
        initialCategory === undefined
            ? []
            : initialCategory.allowedUsers
    );
    const [state, setState] = useState(initialCategoryEditorState);

    const [createCategoryState, createCategory] = useApiCall((dto: CategoryDTO) => 
        apiCall(dto)
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
        });
        event.preventDefault();
    }

    useEffect(() => {
        if(createCategoryState instanceof Failed) {
            // console.error('Error in CategoryEditor: ', createCategoryState.error);
            notificationDispatch({ type: 'addError', message: `Error in CategoryEditor: ${createCategoryState.error}` })
        }
        if(createCategoryState instanceof Loaded && closeAfterLoad){
            onClose();
        }
    }, [createCategoryState, closeAfterLoad]);

    return (
        <>
        <form onSubmit={handleSubmit} className="category-editor-form">
            <div className="editor-inputs">
                <label htmlFor="name">Name</label>
                <input type="text" name="name" value={state.name} required={true} onChange={handleChange} />
                <label htmlFor="description">Description</label>
                <input type="text" name="description" value={state.description} required={true} onChange={handleChange} />
                <label htmlFor="everyoneAllowed">Everyone allowed</label>
                <input type="checkbox" name="everyoneAllowed" checked={state.everyoneAllowed} onChange={handleChange} />
                <label htmlFor="maxAttendees">MaxAttendees</label>
                <input type="number" name="maxAttendees" value={state.maxAttendees} min="1" onChange={handleChange} />
                <label htmlFor="price">Price</label>
                <input type="number" name="price" value={state.price} min="0" onChange={handleChange} />
            </div>
            
            <div className="editor-user-adder">
                <label htmlFor="allowedUsers">Allowed Users</label>
                <UserAdder users={users} setUsers={setUsers} />
            </div>

            <div className="editor-footer">
                <div className="editor-footer-checkbox">
                    <input type="checkbox" name="createAnother" checked={state.createAnother} onChange={handleChange} />
                    <label htmlFor="createAnother">{labels.createAnother}</label>
                </div>
                <input className="editor-footer-submit" type="submit" value={labels.submit} />
                <button onClick={e => {e.preventDefault(); onClose();}}>Mégse</button>
            </div>
        </form>
        </>
    );
}

export const CategoryEditorCreate = ({ owner, onClose }: {
    owner: User,
    onClose: () => void,
}) => {
    const { categoryRepo } = useContext(DIContext);
    
    return (
        <CategoryEditorBase 
            apiCall={categoryRepo.create}
            owner={owner}
            onClose={onClose}
            labels={{ createAnother: 'Több létrehozása', submit: 'Létrehozás' }}
        />
    );
}

export const CategoryEditorUpdate = ({ category, owner, onClose }: {
    category: Category,
    owner: User,
    onClose: () => void,
}) => {
    const { categoryRepo } = useContext(DIContext);

    const update = useCallback((dto: CategoryDTO) =>
            categoryRepo.update(dto)
            .andThen(_ => categoryRepo.getById(dto.id))
    , []);
    
    return (
        <CategoryEditorBase
            apiCall={update}
            initialCategory={category}
            owner={owner}
            onClose={onClose}
            labels={{ createAnother: 'Maradok szerkeszteni', submit: 'Mentés' }}
        />
    );
}