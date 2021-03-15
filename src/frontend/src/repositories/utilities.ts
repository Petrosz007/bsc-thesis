import { Result, ResultPromise, Unit } from '../utilities/result';


const errorHandler = (err: unknown) => {
    if(err instanceof Error) {
        return err;
    }

    return new Error(`${err}`);
}

export const safeFetch: (input: RequestInfo, init?: RequestInit | undefined) => ResultPromise<Response, Error> = 
    ResultPromise.compose(
        Result.fromThrowableAsync(fetch, errorHandler), 
        x => x.andThenAsync(async response => 
            !response.ok
                ? ResultPromise.err(new Error(`${response.status}: ${await response.text()}`))
                : ResultPromise.ok(response)
        )
    );

export const parseResponseAs = async <T>(response: Response): Promise<T> =>
    await response.json() as T;

export const safeParseResponseAs = <T>(response: Response) => Result.fromThrowableAsync(x => parseResponseAs<T>(x), errorHandler)(response);

const fetchOptions: RequestInit = {
    credentials: 'include',
    mode: 'cors',
};


export const safeApiFetchAs = <T>(url: string, method: 'GET' | 'DELETE'): ResultPromise<T,Error> =>
    safeFetch(url, {
        ...fetchOptions,
        method,
    })
    .andThen(r => safeParseResponseAs<T>(r));

export const safeApiFetchWithBodyAs = <T>(url: string, method: 'POST' | 'PUT', data?: any): ResultPromise<T,Error> => 
    safeFetch(url, {
        ...fetchOptions,
        headers: {
            'Content-Type': 'application/json',
        },
        method,
        body: data === undefined ? '' : JSON.stringify(data),
    })
    .andThen(r => safeParseResponseAs<T>(r));

export const safeApiFetchAsUnit = (url: string, method: 'GET' | 'DELETE'): ResultPromise<Unit,Error> => 
    safeFetch(url, {
        ...fetchOptions,
        method,
    })
    .map(_ => ({} as Unit));

export const safeApiFetchWithBodyAsUnit = (url: string, method: 'POST' | 'PUT', data?: any): ResultPromise<Unit,Error> => 
    safeFetch(url, {
        ...fetchOptions,
        headers: {
            'Content-Type': 'application/json',
        },
        method,
        body: data === undefined ? '' : JSON.stringify(data),
    })
    .map(_ => ({} as Unit));
