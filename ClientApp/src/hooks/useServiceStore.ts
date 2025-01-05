import { useState, useCallback } from 'react';
import { ServiceStore } from '../types/service-store';
import { useServiceUpdates } from './useServiceUpdates';
import { serverApi } from '../api/serverApi';
import { WindowsService } from '../types/service';

export function useServiceStore(): ServiceStore {
  const [state, setState] = useState({
    services: [],
    pinnedServices: [],
  } as { services: WindowsService[], pinnedServices: string[] });

  const { lastUpdate, updatingServices, updateServices } = useServiceUpdates();

  const loadServices = useCallback(async () => {
    const services = await serverApi.getServices();
    const pinnedServices = await serverApi.getPinnedServices();
    setState({
      pinnedServices: pinnedServices.pinnedServices,
      services: services
    });
  }, []);

  const togglePinned = useCallback(async (serviceName: string) => {
    const isPinned = state.pinnedServices.includes(serviceName);
    if (isPinned) {
      setState(current => ({ ...current, pinnedServices: current.pinnedServices.filter(name => name !== serviceName) }));
      await serverApi.unpinService(serviceName);
    } else {
      setState(current => ({ ...current, pinnedServices: [...current.pinnedServices, serviceName] }));
      await serverApi.pinService(serviceName);
    }
  }, []);

  const isPinned = useCallback((serviceName: string) => {
    return state.pinnedServices.includes(serviceName);
  }, [state.pinnedServices]);

  return {
    services: state.services,
    pinnedServices: state.pinnedServices,
    lastUpdate,
    updatingServices,
    togglePinned,
    isPinned,
    loadServices,
  };
}