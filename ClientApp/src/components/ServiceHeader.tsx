import React from 'react';
import { Link } from 'react-router-dom';
import { ArrowLeft } from 'lucide-react';
import { PinButton } from './PinButton';

interface ServiceHeaderProps {
  serviceName: string;
  displayName: string;
}

export function ServiceHeader({ serviceName, displayName }: ServiceHeaderProps) {
  return (
    <div className="flex items-center justify-between">
      <div className="flex items-center gap-2">
        <Link 
          to="/"
          className="text-gray-500 hover:text-gray-700 transition-colors"
        >
          <ArrowLeft size={20} />
        </Link>
        <h2 className="text-2xl font-semibold">{displayName}</h2>
      </div>
      <PinButton serviceName={serviceName} />
    </div>
  );
}