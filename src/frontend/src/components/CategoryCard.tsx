import React, {useState} from "react";
import {Category, User} from "../logic/entities";

import './CategoryCard.scss';
import {CategoryEditorUpdate} from "./editors/CategoryEditor";
import Modal from "./Modal";
import CategoryViewer from "./CategoryViewer";
import {DownArrow, EditIcon, InfoIcon} from "../SVGs";

export const CategoryCardEditable = ({ category, onEdit, onView }: { 
    category: Category, 
    onEdit: (_: Category) => void, 
    onView: (_: Category) => void,
}) => {
    return (
        <div className="category-card">
            <span className="category-header clickable">{category.name}</span>
            <div className="category-description">
                <p>{category.description}</p>
                <p>{category.price} Ft</p>
            </div>
            <div className="category-methods">
                <button onClick={() => onView(category)}><InfoIcon className="infoIcon"/></button>
                <button onClick={() => onEdit(category)}><EditIcon className="editIcon"/></button>
            </div>
        </div>
    );
};

export const CategoriesEditable = ({ owner, categories }: { owner: User, categories: Category[] }) => {
    const [categoryToEdit, setCategoryToEdit] = useState<Category|undefined>(undefined);
    const [categoryToView, setCategoryToView] = useState<Category|undefined>(undefined);
    const [collapsed, setCollapsed] = useState(false);
    
    return (
        <>
        {categoryToEdit !== undefined &&
             <Modal>
                 <CategoryEditorUpdate category={categoryToEdit} owner={owner} onClose={() => setCategoryToEdit(undefined)}/>
             </Modal>
        }
        {categoryToView !== undefined &&
             <Modal>
                 <CategoryViewer category={categoryToView} onClose={() => setCategoryToView(undefined)}/>
             </Modal>
        }
        <div className={`categories ${collapsed ? 'collapsed' : ''}`}>
            <h2>Kategóriák <button onClick={() => setCollapsed(!collapsed)}><DownArrow className="downIcon"/></button></h2>
            <div className="categoryCards">
                {categories.map(category =>
                    <React.Fragment key={category.id}>
                        <CategoryCardEditable category={category} onEdit={c => setCategoryToEdit(c)} key={category.id} onView={c => setCategoryToView(c)}/>
                        <hr/>
                    </React.Fragment>
                )}
            </div>
        </div>
        </>
    );
}