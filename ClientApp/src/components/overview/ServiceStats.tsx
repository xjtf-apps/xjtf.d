import React from 'react';
import { Pin, Play, List, Monitor } from 'lucide-react';
import { WindowsService } from '../../types/service';

interface ServiceStatsProps {
  services: WindowsService[];
  pinnedServices: string[];
}

export function ServiceStats({ services, pinnedServices }: ServiceStatsProps) {
  const runningServices = services.filter(service => service.status === 'Running').length;

  return (
    <div>
      <div className="flex items-center gap-2 mb-4">
        <Monitor size={16} className="text-blue-600" />
        <h2 className="text-lg font-semibold">Overview</h2>
      </div>
      
      <div className="bg-white rounded-lg shadow p-6 mb-8">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-blue-50 rounded-lg">
              <Pin className="text-blue-600" size={20} />
            </div>
            <div>
              <div className="text-2xl font-semibold">{pinnedServices.length}</div>
              <div className="text-sm text-gray-600">Pinned Services</div>
            </div>
          </div>

          <div className="flex items-center gap-3">
            <div className="p-2 bg-green-50 rounded-lg">
              <Play className="text-green-600" size={20} />
            </div>
            <div>
              <div className="text-2xl font-semibold">{runningServices}</div>
              <div className="text-sm text-gray-600">Running Services</div>
            </div>
          </div>

          <div className="flex items-center gap-3">
            <div className="p-2 bg-gray-50 rounded-lg">
              <List className="text-gray-600" size={20} />
            </div>
            <div>
              <div className="text-2xl font-semibold">{services.length}</div>
              <div className="text-sm text-gray-600">Total Services</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}