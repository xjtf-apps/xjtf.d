import { useEffect, useRef, useCallback } from 'react';

export function usePolling(callback: () => void, interval: number) {
  const savedCallback = useRef(callback);

  useEffect(() => {
    savedCallback.current = callback;
  }, [callback]);

  const executeCallback = useCallback(() => {
    savedCallback.current();
  }, []);

  useEffect(() => {
    const id = setInterval(executeCallback, interval);
    return () => clearInterval(id);
  }, [interval, executeCallback]);
}