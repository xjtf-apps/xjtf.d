import React from 'react';
import { Pin } from 'lucide-react';
import { useService } from '../context/ServiceContext';

interface PinButtonProps {
  serviceName: string;
}

export function PinButton({ serviceName }: PinButtonProps) {
  const { isPinned, togglePinned } = useService();
  const pinned = isPinned(serviceName);

  return (
    <button
      onClick={(e) => {
        e.preventDefault();
        togglePinned(serviceName);
      }}
      className={`p-2 rounded-full transition-colors ${
        pinned 
          ? 'bg-blue-100 text-blue-600 hover:bg-blue-200' 
          : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
      }`}
      title={pinned ? 'Unpin service' : 'Pin service'}
    >
      <Pin size={20} className={pinned ? 'fill-current' : ''} />
    </button>
  );
}