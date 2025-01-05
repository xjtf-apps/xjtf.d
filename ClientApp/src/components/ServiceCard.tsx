import React from 'react';
import { WindowsService } from '../types/service';
import { Play, Square, Settings2 } from 'lucide-react';
import { Link } from 'react-router-dom';

interface ServiceCardProps {
  service: WindowsService;
  isUpdating?: boolean;
}

export function ServiceCard({ service, isUpdating }: ServiceCardProps) {
  return (
    <Link 
      to={`/service/${service.serviceName}`}
      className={`block bg-white rounded-lg shadow-md p-4 hover:shadow-lg transition-all ${
        isUpdating ? 'animate-pulse bg-blue-50' : ''
      }`}
    >
      <div className="flex items-center justify-between mb-2">
        <h3 className="font-semibold text-lg">{service.displayName}</h3>
        <div className="flex items-center gap-2">
          {service.status === 'Running' ? (
            <span className="flex items-center text-green-600">
              <Play size={16} className="mr-1" />
              Running
            </span>
          ) : (
            <span className="flex items-center text-red-600">
              <Square size={16} className="mr-1" />
              Stopped
            </span>
          )}
        </div>
      </div>
      <p className="text-sm text-gray-600 mb-2">{service.description}</p>
      <div className="flex items-center justify-between text-sm text-gray-500">
        <span className="flex items-center">
          <Settings2 size={16} className="mr-1" />
          {service.startType}
        </span>
        <span className="text-gray-400">{service.serviceName}</span>
      </div>
    </Link>
  );
}