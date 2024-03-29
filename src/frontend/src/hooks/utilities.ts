import React, {useEffect, useLayoutEffect} from "react";

export const useEffectAsync = (f: () => Promise<void>, deps?: React.DependencyList) => 
    useEffect(() => {f()}, deps);

export const useLayoutEffectAsync = (f: () => Promise<void>, deps?: React.DependencyList) => 
    useLayoutEffect(() => {f()}, deps);
