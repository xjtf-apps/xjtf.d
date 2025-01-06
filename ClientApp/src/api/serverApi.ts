import { PinnedServices } from '../types/pinned-services';
import { WindowsService, WindowsServiceWithUptime } from '../types/service';

// hostname
const hostname = 'localhost:7631';

export const serverApi = {
  async getServices(): Promise<WindowsService[]> {
    return fetch(`https://${hostname}/api/windowsservices`)
    .then(response => response.json())
    .then(data => data as WindowsService[])
  },

  async getServiceByName(serviceName: string): Promise<WindowsServiceWithUptime | undefined> {
    return fetch(`https://${hostname}/api/windowsservices/${serviceName}`)
    .then(response => response.json())
    .then(data => data as WindowsServiceWithUptime);
  },

  async pinService(serviceName: string): Promise<void> {
    fetch(`https://${hostname}/api/pinnedservices/pin/${serviceName}`, { method: 'PUT' });
  },

  async unpinService(serviceName: string): Promise<void> {
    fetch(`https://${hostname}/api/pinnedservices/unpin/${serviceName}`, { method: 'PUT' });
  },

  async getPinnedServices(): Promise<PinnedServices> {
    return fetch(`https://${hostname}/api/pinnedservices`)
    .then(response => response.json())
    .then(data => data as PinnedServices);
  }
};