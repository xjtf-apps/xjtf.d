import React, { createContext, useContext, useEffect } from 'react';
import { ServiceStore } from '../types/service-store';
import { useServiceStore } from '../hooks/useServiceStore';

const ServiceContext = createContext<ServiceStore | undefined>(undefined);

export function ServiceProvider({ children }: { children: React.ReactNode }) {
  const serviceStore = useServiceStore();

  useEffect(() => {
    serviceStore.loadServices();
  }, [serviceStore.loadServices, serviceStore.updatingServices]);

  return (
    <ServiceContext.Provider value={serviceStore}>
      {children}
    </ServiceContext.Provider>
  );
}

export function useService() {
  const context = useContext(ServiceContext);
  if (context === undefined) {
    throw new Error('useService must be used within a ServiceProvider');
  }
  return context;
}