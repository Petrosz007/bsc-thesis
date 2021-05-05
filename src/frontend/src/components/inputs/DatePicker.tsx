import {InputHTMLAttributes, useCallback, useDebugValue, useEffect, useState} from "react";
import {DateTime, Interval} from "luxon";
import React from "react";

import './DatePicker.scss';
import {RightArrow} from "../../SVGs";

interface DatePickerProps extends InputHTMLAttributes<HTMLInputElement> {
    valueDate: DateTime,
    onChangeDate: (_: DateTime) => void,
    minDate?: DateTime,
    maxDate?: DateTime,
};

export const DatePicker = ({ valueDate, onChangeDate, minDate, maxDate, ...props}: DatePickerProps) => {
    const handleChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
        const date = DateTime.fromISO(e.target.value);
        if(!date.isValid) return;
        if(minDate !== undefined && date < minDate) return;
        if(maxDate !== undefined && date > maxDate) return;
        
        onChangeDate(date);
    }, [onChangeDate]);
    
    return (
        <input {...props} 
               type="date"
               value={valueDate.toISODate()}
               onChange={handleChange}
               min={minDate?.toISODate()} 
               max={maxDate?.toISODate()}
               required
        />
    );
}

export const DateTimePicker = ({ valueDate, onChangeDate, minDate, maxDate, ...props}: DatePickerProps) => {
    const handleDateChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
        const date = DateTime.fromISO(e.target.value).set({ hour: valueDate.hour, minute: valueDate.hour });
        if(!date.isValid) return;
        // if(minDate !== undefined && date < minDate) return;
        // if(maxDate !== undefined && date > maxDate) return;

        onChangeDate(date);
    }, [onChangeDate, minDate, maxDate]);
    
    const handleTimeChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
        const date = DateTime.fromFormat(`${valueDate.toFormat('yyyy.MM.dd')} ${e.target.value}`, 'yyyy.MM.dd HH:mm');
        if(!date.isValid) return;
        // if(minDate !== undefined && date < minDate) return;
        // if(maxDate !== undefined && date > maxDate) return;

        onChangeDate(date);
    }, [valueDate, onChangeDate, minDate, maxDate]);

    return (
        <div className="dateTimePicker">
            <input {...props}
                   type="date"
                   value={valueDate.toISODate()}
                   onChange={handleDateChange}
                   min={minDate?.toISODate()}
                   max={maxDate?.toISODate()}
                   required
            />
            <input {...props}
                   type="time"
                   value={valueDate.toFormat('HH:mm')}
                   onChange={handleTimeChange}
                   min={minDate?.toFormat('HH:mm')}
                   max={maxDate?.toFormat('HH:mm')}
                   required
            />
        </div>
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
        <div className="dateRange">
            <DatePicker valueDate={value.start} onChangeDate={onStartDateChange} maxDate={value.end} />
            <RightArrow className="rightArrow"/>
            <DatePicker valueDate={value.end} onChangeDate={onEndDateChange} minDate={value.start} />
        </div>
    );
}
