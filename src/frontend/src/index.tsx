import { useEffect, useState, StrictMode } from 'react';
import React from 'react';
import ReactDOM from 'react-dom';
import './App.scss';

const App = () => {
    const [count, setCount] = useState(0);
    useEffect(() => {
        const timer = setTimeout(() => setCount(count + 1), 1000);
        return () => clearTimeout(timer);
    }, [count, setCount]);
    return (
        <>
            <h1>my first snowpack+react app</h1>
            <h2>hello ❄️Snowpack❄️</h2>
            <p>Is this really that fast? Holy moly</p>
            <h1>Time is ticking awaaay {count} awaay</h1>
            <div id="target0">
                <p>Lorem ipsum dolor sit, amet consectetur adipisicing elit. Similique a quo maiores dolore sequi. Vitae nam facere labore, expedita totam, eligendi dolor eum veniam accusamus assumenda enim eos. Impedit, sint.</p>
            </div>
        </>
    );
};

ReactDOM.render(
    <StrictMode>
        <App />
    </StrictMode>,
    document.getElementById('root')
);

 // Hot Module Replacement (HMR) - Remove this snippet to remove HMR.
 // Learn more: https://www.snowpack.dev/concepts/hot-module-replacement
if (import.meta.hot) {
    import.meta.hot.accept();
}
