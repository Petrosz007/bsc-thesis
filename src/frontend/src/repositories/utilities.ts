import { ResultPromise, safeFetch } from '../utilities/result';

const fetchOptions: RequestInit = {
    credentials: 'include',
    mode: 'cors',
};

export const safeApiFetch = (url: string, method: 'GET' | 'DELETE'): ResultPromise<Response,Error> =>
    safeFetch(url, {
        ...fetchOptions,
        method,
    });

export const safeApiFetchWithBody = <T>(url: string, method: 'POST' | 'PUT', data?: T): ResultPromise<Response,Error> => 
    safeFetch(url, {
        ...fetchOptions,
        headers: {
            'Content-Type': 'application/json',
        },
        method,
        body: data === undefined ? '' : JSON.stringify(data),
    });
