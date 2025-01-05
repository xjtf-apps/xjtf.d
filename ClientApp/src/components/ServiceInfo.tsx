import React from 'react';
import { Play, Square, Settings2 } from 'lucide-react';
import { WindowsService } from '../types/service';

interface ServiceInfoProps {
  service: WindowsService;
}

export function ServiceInfo({ service }: ServiceInfoProps) {
  return (
    <div className="bg-white rounded-lg shadow p-6">
      <div className="flex items-center justify-between mb-6">
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
        <div className="flex items-center text-gray-500">
          <Settings2 size={16} className="mr-1" />
          {service.startType}
        </div>
      </div>
      
      <div className="space-y-4">
        <div>
          <h3 className="text-sm font-medium text-gray-500 mb-1">Service Name</h3>
          <p className="text-gray-900">{service.serviceName}</p>
        </div>
        
        <div>
          <h3 className="text-sm font-medium text-gray-500 mb-1">Description</h3>
          <p className="text-gray-900">{service.description}</p>
        </div>
      </div>
    </div>
  );
}