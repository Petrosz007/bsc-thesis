export class Dictionary<Value> {
    [key: string]: Value[];

    static keys = <T>(dict: Dictionary<T>): string[] => Object.keys(dict) as string[];
}


export const groupBy = <T>(list: T[], compare: (_x: T) => string): Dictionary<T> =>
    list.reduce((acc, x) => {
        const key = compare(x);
        return { ...acc, [key]: [...acc[key] ?? [], x] }
    }, {} as Dictionary<T>);


export const uniques = <T>(list: T[], compare: (_x: T) => string): T[] => 
    Object.values(groupBy(list, compare)).flatMap(x => x.length === 0 ? [] : x[0]);


export const setValue = <T,G>(values: T[], value: T, lens: (_x: T) => G): T[] => {
    if(!values.some(x => lens(x) === lens(value))) {
        return [...values, value];
    }

    return values.map(x =>
        lens(x) !== lens(value)
            ? x
            : value);
}
