import React, {useContext, useState, useCallback} from "react";
import { useHandleChange } from "../../hooks/useEditorForm";
import {CategoryDTO} from "../../logic/dtos";
import {Category, User} from "../../logic/entities";
import { DIContext } from "../contexts/DIContext";
import UserAdder from "./UserAdder";
import {ResultPromise} from "../../utilities/result";
import {EditorBase} from "./EditorBase";

// import './CategoryEditor.scss';

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
    initialCategory: Category,
    apiCall: (_: CategoryDTO) => ResultPromise<Category, Error>,
    owner: User,
    onClose: () => void,
    labels: {
        header: string,
        createAnother: string,
        submit: string,
    },
}) => {
    const initialCategoryEditorState: CategoryEditdata = {
        ...initialCategory,
        createAnother: false,
    };

    const [users, setUsers] = useState<User[]>(initialCategory.allowedUsers);
    const [state, setState] = useState(initialCategoryEditorState);

    const handleChange = useHandleChange(setState);

    const editorStateToDto = useCallback((editorState: CategoryEditdata) => ({
        ...editorState,
        allowedUserNames: editorState.everyoneAllowed ? [] : users.map(u => u.userName),
        ownerUserName: owner.userName,
    }), [users, owner]);

    return (
        <EditorBase
            state={state}
            apiCall={apiCall}
            handleChange={handleChange}
            editorStateToDto={editorStateToDto}
            labels={labels}
            onClose={onClose}
            dataDispatchAction={category => ({ type: 'updateCategory', category })}
        >
            <div className="editorGroup">
                <label htmlFor="name">Név</label>
                <input type="text" name="name" value={state.name} required={true} onChange={handleChange} />
            </div>
            <div className="editorGroup">
                <label htmlFor="description">Leírás</label>
                <input type="text" name="description" value={state.description} required={true} onChange={handleChange} />
            </div>
            <div className="editorGroup">
                <label htmlFor="everyoneAllowed">Nyílt esemény 
                    <input type="checkbox" name="everyoneAllowed" checked={state.everyoneAllowed} onChange={handleChange} />
                </label>
            </div>
            <div className="editorGroup">
                <label htmlFor="maxAttendees">Max résztvevők</label>
                <input type="number" name="maxAttendees" value={state.maxAttendees} min="1" onChange={handleChange} />
            </div>
            <div className="editorGroup">
                <label htmlFor="price">Ár</label>
                <input type="number" name="price" value={state.price} min="0" onChange={handleChange} />
            </div>
            {!state.everyoneAllowed && <>
                <label htmlFor="allowedUsers">Engedélyezett résztvevők</label>
                <UserAdder users={users} setUsers={setUsers} />
            </>}
        </EditorBase>
    );
}

export const CategoryEditorCreate = ({ owner, onClose }: {
    owner: User,
    onClose: () => void,
}) => {
    const { categoryRepo } = useContext(DIContext);
    const initialCategory: Category = {
        id: 0,
        name: '',
        description: '',
        everyoneAllowed: true,
        maxAttendees: 1,
        price: 3000,
        allowedUsers: [],
        owner,
    };
    
    return (
        <CategoryEditorBase 
            initialCategory={initialCategory}
            apiCall={categoryRepo.create}
            owner={owner}
            onClose={onClose}
            labels={{ header: 'Új Kategória', createAnother: 'Több létrehozása', submit: 'Létrehozás' }}
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
            labels={{ header: 'Kategória szerkesztése', createAnother: 'Maradok szerkeszteni', submit: 'Mentés' }}
        />
    );
}