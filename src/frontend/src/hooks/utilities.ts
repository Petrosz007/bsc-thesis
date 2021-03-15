import React, { useEffect } from "react";
export const useEffectAsync = (f: () => Promise<void>, deps?: React.DependencyList) => useEffect(() => {f()}, deps);
