const fetchOptions: RequestInit = {
    credentials: 'include',
    mode: 'cors',
};

export const apiFetchWithBody = async <T>(url: string, method: 'POST' | 'PUT', data?: T): Promise<Response> => 
    fetch(url, {
        ...fetchOptions,
        headers: {
            'Content-Type': 'application/json',
        },
        method,
        body: data === undefined ? '' : JSON.stringify(data),
    });

export const apiFetch = async (url: string, method: 'GET' | 'DELETE'): Promise<Response> =>
    fetch(url, {
        ...fetchOptions,
        method,
    });
