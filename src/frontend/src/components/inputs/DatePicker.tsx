import {InputHTMLAttributes, useCallback, useDebugValue, useEffect, useState} from "react";
import {DateTime, Interval} from "luxon";
import React from "react";

interface DatePickerProps extends InputHTMLAttributes<HTMLInputElement> {
    valueDate: DateTime,
    onChangeDate: (_: DateTime) => void,
    minDate?: DateTime,
    maxDate?: DateTime,
};

export const DatePicker = ({ valueDate, onChangeDate, minDate, maxDate, ...props}: DatePickerProps) => {
    const handleChange = useCallback((e:  React.ChangeEvent<HTMLInputElement>) => {
        const date = DateTime.fromISO(e.target.value);
        onChangeDate(date);
    }, [onChangeDate]);
    
    return (
        <input {...props} 
               type="date"
               value={valueDate.toISODate()}
               onChange={handleChange}
               min={minDate?.toISODate()} 
               max={maxDate?.toISODate()} 
        />
    );
}

export const DateRangePicker = ({ value, onChange }: { value: Interval, onChange: (_: Interval) => void }) => {
    const onStartDateChange = useCallback((date: DateTime) => {
        const start = date.set({ hour: 0, minute: 0, second: 0, millisecond: 0 });
        onChange(Interval.fromDateTimes(start, value.end));
    }, [value, onChange]);
    
    const onEndDateChange = useCallback((date: DateTime) => {
        const end = date.set({ hour: 23, minute: 59, second: 59, millisecond: 59 })
        onChange(Interval.fromDateTimes(value.start, end));
    }, [value, onChange]);
    
    return (
        <>
        Start:
        <DatePicker valueDate={value.start} onChangeDate={onStartDateChange} maxDate={value.end} />
        End:
        <DatePicker valueDate={value.end} onChangeDate={onEndDateChange} minDate={value.start} />
        </>
    );
}
