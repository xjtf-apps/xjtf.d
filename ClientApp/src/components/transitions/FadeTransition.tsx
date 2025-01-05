import React from 'react';
import { useRef, useEffect } from 'react';

interface FadeTransitionProps {
  children: React.ReactNode;
  show: boolean;
}

export function FadeTransition({ children, show }: FadeTransitionProps) {
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const element = ref.current;
    if (!element) return;

    element.style.opacity = show ? '1' : '0';
    element.style.transform = show ? 'translateY(0)' : 'translateY(-10px)';
  }, [show]);

  return (
    <div
      ref={ref}
      className="transition-all duration-300"
      style={{
        opacity: 0,
        transform: 'translateY(-10px)'
      }}
    >
      {children}
    </div>
  );
}