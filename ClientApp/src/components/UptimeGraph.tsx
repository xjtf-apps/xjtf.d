import React from 'react';
import { UptimeRecord } from '../types/uptime';

interface UptimeGraphProps {
  data: UptimeRecord[];
  compact?: boolean;
}

export function UptimeGraph({ data, compact = false }: UptimeGraphProps) {
  const totalMinutes = 24 * 60;
  
  return (
    <div className={compact ? "" : "bg-white p-4 rounded-lg shadow"}>
      <h3 className={`font-semibold mb-4 ${compact ? "text-sm" : "text-lg"}`}>
        7-Day Uptime History
      </h3>
      <div className="space-y-2">
        {data.sort((a, b) => new Date(a.timestamp).valueOf() - new Date(b.timestamp).valueOf()).reverse().map((record) => (
          <div key={record.timestamp} className="flex items-center gap-2">
            <span className={`text-gray-500 w-24 ${compact ? "text-xs" : "text-sm"}`}>
              {new Date(record.timestamp).toLocaleDateString()}
            </span>
            <div className={`flex-1 ${compact ? "h-4" : "h-6"} bg-gray-100 rounded`}>
              <div
                className={`h-full rounded ${
                  record.status === 'Running' ? 'bg-green-500' : 'bg-red-500'
                }`}
                style={{ width: `${(record.duration / totalMinutes) * 100}%` }}
              />
            </div>
            <span className={`text-gray-500 w-16 ${compact ? "text-xs" : "text-sm"}`}>
              {Math.round((record.duration / totalMinutes) * 100)}%
            </span>
          </div>
        ))}
      </div>
    </div>
  );
}