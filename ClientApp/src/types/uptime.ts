export interface UptimeRecord {
  timestamp: string;
  status: 'Running' | 'Stopped';
  duration: number; // in minutes
}