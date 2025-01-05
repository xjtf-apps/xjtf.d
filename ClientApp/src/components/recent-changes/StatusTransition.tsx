import React from 'react';

interface StatusTransitionProps {
  previousStatus: 'Running' | 'Stopped';
  currentStatus: 'Running' | 'Stopped';
}

export function StatusTransition({ previousStatus, currentStatus }: StatusTransitionProps) {
  return (
    <div className="text-sm">
      <span className={previousStatus === 'Running' ? 'text-green-600' : 'text-red-600'}>
        {previousStatus}
      </span>
      <span className="mx-2 text-gray-400">→</span>
      <span className={currentStatus === 'Running' ? 'text-green-600' : 'text-red-600'}>
        {currentStatus}
      </span>
    </div>
  );
}