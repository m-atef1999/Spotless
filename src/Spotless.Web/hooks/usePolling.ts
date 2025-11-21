import { useEffect, useRef } from 'react';

export const usePolling = (callback: () => void, intervalMs: number = 30000) => {
    const savedCallback = useRef(callback);

    // Remember the latest callback.
    useEffect(() => {
        savedCallback.current = callback;
    }, [callback]);

    // Set up the interval.
    useEffect(() => {
        function tick() {
            savedCallback.current();
        }
        if (intervalMs !== null) {
            const id = setInterval(tick, intervalMs);
            return () => clearInterval(id);
        }
    }, [intervalMs]);
};
