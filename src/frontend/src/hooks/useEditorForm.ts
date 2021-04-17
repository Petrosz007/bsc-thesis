import React from "react";

export const useHandleChange = <T>(setState: React.Dispatch<React.SetStateAction<T>>) => {
    const handleChange = (event: React.ChangeEvent<HTMLInputElement & HTMLSelectElement & HTMLTextAreaElement>) => {
        const value = event.target.type === 'checkbox' 
            ? event.target.checked
            : event.target.value;

        setState(prevState => ({
            ...prevState,
            [event.target.name]: value,
        }));
    };

    return handleChange;
}
