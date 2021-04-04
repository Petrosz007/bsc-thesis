import {InputHTMLAttributes, useCallback, useState} from "react";
import {DateTime} from "luxon";
import React from "react";

interface DatePickerProps extends InputHTMLAttributes<HTMLInputElement> {
    valueDate: DateTime,
    onChangeDate: (_: DateTime) => void,
    minDate?: DateTime,
    maxDate?: DateTime,
};

export default ({ valueDate, onChangeDate, minDate, maxDate, ...props}: DatePickerProps) => {
    const [state, setState] = useState(valueDate.toISODate());
    
    const handleChange = useCallback((e:  React.ChangeEvent<HTMLInputElement>) => {
        setState(e.target.value);
        const date = DateTime.fromISO(e.target.value);
        onChangeDate(date);
    }, []);
    
    return (
        <input {...props} 
               type="date"
               value={state}
               onChange={handleChange}
               min={minDate?.toISODate()} 
               max={maxDate?.toISODate()} 
        />
    );
}