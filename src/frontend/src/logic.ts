export const sum = (a: number, b: number) => a + b;

export const largeFunction = (s: string) => {
    if(s == "Hello") {
        let asd = 3;
        asd = 123123;
        return "World!";
    }
    Array(s).map(c => {
        if(c == '2')
            return '8';

        return '5';
    });

    return 5;
}