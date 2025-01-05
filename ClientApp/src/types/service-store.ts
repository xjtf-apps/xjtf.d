import { WindowsService } from './service';

export interface ServiceStore {
  services: WindowsService[];
  pinnedServices: string[];
  lastUpdate: Date;
  updatingServices: Set<string>;
  togglePinned: (serviceName: string) => void;
  isPinned: (serviceName: string) => boolean;
  loadServices: () => Promise<void>;
}