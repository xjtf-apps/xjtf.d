import { UptimeRecord } from "./uptime";

export interface WindowsService {
  displayName: string;
  status: 'Running' | 'Stopped' | 'StartPending' | 'ContinuePending' | 'PausePending' | 'Paused';
  startType: 'Automatic' | 'Manual' | 'Disabled' | 'Boot' | 'System';
  serviceName: string;
  description: string;
}

export interface WindowsServiceWithUptime extends WindowsService {
  uptime: UptimeRecord[];
  detailedUptime: UptimeRecord[];
}
