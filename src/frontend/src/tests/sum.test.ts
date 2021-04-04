import { largeFunction, sum } from "../logic";


test('Adds 1 + 2 to equal 3', () => {
    expect(sum(1,2)).toBe(3);
});

test('test what?', () => {
    expect(largeFunction("Hello There2")).toBe(5);
})
