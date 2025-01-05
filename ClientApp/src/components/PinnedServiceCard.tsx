import React, { useEffect, useState, useCallback } from 'react';
import { WindowsService, WindowsServiceWithUptime } from '../types/service';
import { Play, Square, Settings2 } from 'lucide-react';
import { Link } from 'react-router-dom';
import { UptimeGraph } from './UptimeGraph';
import { usePolling } from '../hooks/usePolling';
import { DetailedUptimeGraph } from './DetailedUptimeGraph';
import { serverApi } from '../api/serverApi';

interface PinnedServiceCardProps {
  service: WindowsService;
}

const UPDATE_INTERVAL = 5000;

export function PinnedServiceCard({ service }: PinnedServiceCardProps) {
  const [serviceWUptime, setServiceWUptime] = useState<WindowsServiceWithUptime | undefined>(undefined);

  const fetchServiceUptime = useCallback(async () => {
    serverApi.getServiceByName(service.serviceName)
      .then(data => setServiceWUptime(data));
  }, [service.serviceName]);

  usePolling(fetchServiceUptime, UPDATE_INTERVAL);

  return (
    <Link 
      to={`/service/${service.serviceName}`}
      className="block bg-white rounded-lg shadow-md p-4 hover:shadow-lg transition-shadow"
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
      <div className="flex items-center justify-between text-sm text-gray-500 mb-4">
        <span className="flex items-center">
          <Settings2 size={16} className="mr-1" />
          {service.startType}
        </span>
        <span className="text-gray-400">{service.serviceName}</span>
      </div>
      {serviceWUptime && serviceWUptime.detailedUptime && <div className="border-t pt-4">
        <DetailedUptimeGraph data={serviceWUptime.detailedUptime} compact />
      </div>}
    </Link>
  );
}