import React from 'react';
import { WindowsService } from '../types/service';
import { PinnedServiceCard } from './PinnedServiceCard';
import { Pin } from 'lucide-react';

interface PinnedServicesProps {
  services: WindowsService[];
}

export function PinnedServices({ services }: PinnedServicesProps) {
  if (services.length === 0) return null;

  return (
    <div className="mb-8">
      <div className="flex items-center gap-2 mb-4">
        <Pin size={16} className="text-blue-600" />
        <h2 className="text-lg font-semibold">Pinned Services</h2>
      </div>
      <div className="grid grid-cols-1 gap-4">
        {services.map((service) => (
          <PinnedServiceCard key={service.serviceName} service={service} />
        ))}
      </div>
    </div>
  );
}