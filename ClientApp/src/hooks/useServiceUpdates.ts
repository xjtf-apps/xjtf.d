import { useState, useCallback, useRef } from 'react';
import { usePolling } from './usePolling';
import { WindowsService } from '../types/service';
import { useServiceChanges } from '../context/ServiceChangeContext';
import { serverApi } from '../api/serverApi';

const UPDATE_INTERVAL = 5000; // 5 seconds

export function useServiceUpdates() {
  const [lastUpdate, setLastUpdate] = useState<Date>(new Date());
  const [updatingServices, setUpdatingServices] = useState<Set<string>>(new Set());
  const previousServices = useRef<Map<string, WindowsService>>(new Map());
  const { addChange } = useServiceChanges();

  const updateServices = useCallback(async () => {
    try {
      const services = await serverApi.getServices();

      // Track status changes and update global state
      services.forEach(service => {
        const previousService = previousServices.current.get(service.serviceName);
        if (previousService && previousService.status !== service.status) {
          addChange(service, previousService.status);
        }
      });

      // Track which services are currently updating
      const changedServices = new Set(
        services
          .filter((service) => {
            const prev = previousServices.current.get(service.serviceName);
            return prev && prev.status !== service.status;
          })
          .map(service => service.serviceName)
      );

      setUpdatingServices(changedServices);
      setLastUpdate(new Date());
      
      // Update previous services reference
      previousServices.current = new Map(services.map(s => [s.serviceName, s]));

      return services;
    } catch (error) {
      console.error('Failed to update services:', error);
      return [];
    }
  }, [addChange]);

  usePolling(updateServices, UPDATE_INTERVAL);

  return {
    lastUpdate,
    updatingServices,
    updateServices
  };
}