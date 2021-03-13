export class Ok<T,E> {
    constructor(public readonly value: T) {}

    unwrapOr = (defaultValue: T) => this.value;
    isOk = (): this is Ok<T,E> => true;
    isErr = (): this is Err<T,E> => !this.isOk;

    map = <U>(fn: (_x: T) => U): Result<U,E> => Result.ok(fn(this.value));
    mapAsync = <U>(fn: (_x: T) => Promise<U>): ResultPromise<U,E> => Result.fromSafeAsync<[_x: T], U, E>(fn)(this.value);

    andThen = <U,F>(fn: (_x: T) => Result<U,F>): Result<U,E|F> => fn(this.value);
    andThenAsync = <U,F>(fn: (_x: T) => Promise<Result<U,F>>): ResultPromise<U,E|F> => ResultPromise.of(fn(this.value));

    sideEffect = (fn: (_x: T) => void): Result<T,E> => this.andThen(x => { fn(x); return Result.ok(x); });
    sideEffectAsync = (fn: (_x: T) => Promise<void>): ResultPromise<T,E> => 
        ResultPromise.ok<T,E>(this.value)
        .andThenAsync(async x => { await fn(x); return ResultPromise.ok(x); });

    match = <U>(
        okFn: (_x: T) => U,
        errFn: (_x: E) => U
    ): U => okFn(this.value);
}

export class Err<T,E> {
    constructor(public readonly error: E) {}

    unwrapOr = (defaultValue: T) => defaultValue;
    isOk = (): this is Ok<T,E> => false;
    isErr = (): this is Err<T,E> => !this.isOk;

    map = <U>(fn: (_x: T) => U): Result<U,E> => Result.err(this.error);
    mapAsync = <U>(fn: (_x: T) => Promise<U>): ResultPromise<U,E> => ResultPromise.err(this.error);

    andThen = <U,F>(fn: (_x: T) => Result<U,F>): Result<U,E|F> => Result.err(this.error);
    andThenAsync = <U,F>(fn: (_x: T) => Promise<Result<U,F>>): ResultPromise<U,E|F> => ResultPromise.err(this.error);

    sideEffect = (fn: (_x: T) => void): Result<T,E> => Result.err(this.error);
    sideEffectAsync = (fn: (_x: T) => Promise<void>): ResultPromise<T,E> => ResultPromise.err(this.error);

    match = <U>(
        okFn: (_x: T) => U,
        errFn: (_x: E) => U
    ): U => errFn(this.error);
}

export type Result<T,E> = Ok<T,E> | Err<T,E>;

export namespace Result {
    export const ok = <T,E>(value: T): Result<T,E> => new Ok(value);
    export const err = <T,E>(error: E): Result<T,E> => new Err(error);
    
    export const fromThrowable = <Args extends any[], T, E>(
        fn: (...args: Args) => T,
        onError?: (_err: any) => E
    ): (...args: Args) => Result<T,E> => 
        (...args) => {
            try {
                return ok(fn(...args));
            }
            catch(err) {
                return err(onError === undefined ? err : onError(err));
            }
        }

    export const fromSafeAsync = <Args extends any[], T, E>(
        fn: (...args: Args) => Promise<T>
    ): (...args: Args) => ResultPromise<T,E> => 
        (...args) => new ResultPromise(
            fn(...args)
            .then(res => Result.ok<T,E>(res)));

    export const fromThrowableAsync = <Args extends any[], T, E>(
        fn: (...args: Args) => Promise<T>,
        onError?: (_err: any) => E
    ): (...args: Args) => ResultPromise<T,E> => 
        (...args) => new ResultPromise(
            fn(...args)
            .then(res => Result.ok<T,E>(res))
            .catch(err => Result.err<T,E>(onError === undefined ? err : onError(err))));
}

export class ResultPromise<T,E> {
    constructor(public readonly promise: Promise<Result<T,E>>) {}

    static of = <T,E>(promise: Promise<Result<T,E>>) => new ResultPromise(promise);
    static lift = <T,E>(result: Result<T,E>) => ResultPromise.of(Promise.resolve(result));
    static ok = <T,E>(value: T):  ResultPromise<T,E> => ResultPromise.lift(Result.ok(value));
    static err = <T,E>(error: E): ResultPromise<T,E> => ResultPromise.lift(Result.err(error));

    toPromise = (): Promise<Result<T,E>> => this.promise;

    map = <U>(fn: (_x: T) => U): ResultPromise<U,E> => new ResultPromise(
        this.promise.then(result => result.map(fn))
    );
    mapAsync = <U>(fn: (_x: T) => Promise<U>): ResultPromise<U,E> => new ResultPromise(
        this.promise.then(result => result.mapAsync(fn).promise)
    );

    andThen = <U,F>(fn: (_x: T) => ResultPromise<U,F>): ResultPromise<U,E | F> => new ResultPromise(
        this.promise.then(result => 
            result
            .map(fn)
            .match(
                x => x.promise, 
                x => Promise.resolve(Result.err<U,E|F>(x))
            ))
    );

    static join = <T,E,F>(x: ResultPromise<ResultPromise<T,E>, F>): ResultPromise<T,E|F> => x.andThen(x => x);

    andThenAsync = <U,F>(fn: (_x: T) => Promise<ResultPromise<U,F>>): ResultPromise<U,E | F> => new ResultPromise(
        this.promise.then(result => 
            ResultPromise.join(result.mapAsync(fn)).promise
        )
    );

    sideEffect = (fn: (_x: T) => void): ResultPromise<T,E> => this.andThen(x => { fn(x); return ResultPromise.ok(x); });
    sideEffectAsync = (fn: (_x: T) => Promise<void>): ResultPromise<T,E> => this.andThenAsync(async x => { await fn(x); return ResultPromise.ok(x); });

    static compose = <T,E,U,F,Args extends any[]>(
        f: (...args: Args) => ResultPromise<T,E>, 
        g: (_x: ResultPromise<T,E>) => ResultPromise<U,F>
    ): (...args: Args) => ResultPromise<U,F> => 
        (...args) => g(f(...args));

    static all = <T,E>(values: readonly ResultPromise<T,E>[]): ResultPromise<T[],E> => ResultPromise.of(
        Promise.all(values.map(x => x.promise)).then(results =>
            results.reduce((acc, x) =>
                acc.isErr()
                    ? acc
                    : x.map(y => [...acc.value, y])
            , Result.ok([] as T[])))
    );

    match = <U>(
        okFn: (_x: T) => U,
        errFn: (_: E) => U
    ): Promise<U> => this.promise.then(result => result.match(okFn, errFn));
}

const fetchError = (err: unknown) => {
    if(err instanceof Error) {
        return err;
    }

    return new Error(`${err}`);
}

export const safeFetch: (input: RequestInfo, init?: RequestInit | undefined) => ResultPromise<Response, Error> = 
    ResultPromise.compose(
        Result.fromThrowableAsync(fetch, fetchError), 
        x => x.andThenAsync(async response => 
            !response.ok
                ? ResultPromise.err(new Error(`${response.status}: ${await response.text()}`))
                : ResultPromise.ok(response)
        )
    );
