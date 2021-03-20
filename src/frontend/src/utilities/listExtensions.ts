export class Dictionary<Value> {
    [key: string]: Value[];

    static keys = <T>(dict: Dictionary<T>): string[] => Object.keys(dict) as string[];
}

export const groupBy = <T>(list: T[], compare: (_x: T) => string): Dictionary<T> =>
    list.reduce((acc, x) => {
        const key = compare(x);
        return { ...acc, [key]: [...acc[key] ?? [], x] }
    }, {} as Dictionary<T>);
