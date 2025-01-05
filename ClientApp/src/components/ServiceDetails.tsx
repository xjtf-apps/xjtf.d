import React, { useEffect } from 'react';
import { WindowsServiceWithUptime } from '../types/service';
import { UptimeGraph } from './UptimeGraph';
import { DetailedUptimeGraph } from './DetailedUptimeGraph';
import { useParams } from 'react-router-dom';
import { ServiceHeader } from './ServiceHeader';
import { ServiceInfo } from './ServiceInfo';
import { serverApi } from '../api/serverApi';

export function ServiceDetails() {
  const { name } = useParams<{ name: string }>();
  const [service, setService] = React.useState<WindowsServiceWithUptime | undefined>(undefined);
  
  useEffect(() => {
    if (name !== undefined) {
      serverApi
      .getServiceByName(name)
      .then(svc => setService(svc));
    }
  }, [name]);

  return (
    <div className="space-y-6">
      <ServiceHeader 
        serviceName={service?.serviceName ?? ''} 
        displayName={service?.displayName ?? ''} 
      />
      {service && service.uptime && service.detailedUptime && <>
        <ServiceInfo service={service} />
        <DetailedUptimeGraph data={service.detailedUptime} />
        <UptimeGraph data={service.uptime} />
      </>}
    </div>
  );
}