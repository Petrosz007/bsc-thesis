import React, {useState} from "react";
import {Category, User} from "../logic/entities";

import './CategoryCard.scss';
import {CategoryEditorUpdate} from "./editors/CategoryEditor";
import Modal from "./Modal";

export const CategoryCardEditable = ({ category, onEdit }: { 
    category: Category, 
    onEdit: (_: Category) => void, 
}) => {
    return (
        <div className="category-card">
            <span className="category-header">{category.name}</span>
            <div className="category-description">
                <p>{category.description}</p>
                <p>{category.price} Ft</p>
            </div>
            <div className="category-methods">
                <button onClick={() => onEdit(category)}>Edit</button>
            </div>
        </div>
    );
};

export const CategoriesEditable = ({ owner, categories }: { owner: User, categories: Category[] }) => {
    const [categoryToEdit, setCategoryToEdit] = useState<Category|undefined>(undefined);
    
    return (
        <>
        {categoryToEdit !== undefined &&
             <Modal>
                 <CategoryEditorUpdate category={categoryToEdit} owner={owner} onClose={() => setCategoryToEdit(undefined)}/>
             </Modal>
        }
        <div>
            {categories.map(category =>
                <CategoryCardEditable category={category} onEdit={c => setCategoryToEdit(c)} key={category.id} />
            )}
        </div>
        </>
    );
}