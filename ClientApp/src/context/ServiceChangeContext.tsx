import React, { createContext, useContext, useState, useCallback } from 'react';
import { WindowsService } from '../types/service';
import { ServiceChange, ServiceChangeStore } from '../types/recent-change';

const ServiceChangeContext = createContext<ServiceChangeStore | undefined>(undefined);

export function ServiceChangeProvider({ children }: { children: React.ReactNode }) {
  const [recentChanges, setRecentChanges] = useState<ServiceChange[]>([]);

  const addChange = useCallback((service: WindowsService, previousStatus: 'Running' | 'Stopped') => {
    const change: ServiceChange = {
      service,
      previousStatus,
      timestamp: Date.now(),
      id: `${service.serviceName}-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
    };

    setRecentChanges(prev => {
      const newChanges = [change, ...prev].slice(0, 5); // Keep last 5 changes
      return newChanges;
    });
  }, []);

  return (
    <ServiceChangeContext.Provider value={{ recentChanges, addChange }}>
      {children}
    </ServiceChangeContext.Provider>
  );
}

export function useServiceChanges() {
  const context = useContext(ServiceChangeContext);
  if (context === undefined) {
    throw new Error('useServiceChanges must be used within a ServiceChangeProvider');
  }
  return context;
}