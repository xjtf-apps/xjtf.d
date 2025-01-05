import { WindowsService } from './service';

export interface ServiceChange {
  service: WindowsService;
  previousStatus: 'Running' | 'Stopped' | 'StartPending' | 'ContinuePending' | 'PausePending' | 'Paused';
  timestamp: number;
  id: string;
}

export interface ServiceChangeStore {
  recentChanges: ServiceChange[];
  addChange: (service: WindowsService, previousStatus: 'Running' | 'Stopped' | 'StartPending' | 'ContinuePending' | 'PausePending' | 'Paused') => void;
}