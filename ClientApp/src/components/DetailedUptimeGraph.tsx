import React from 'react';
import { UptimeRecord } from '../types/uptime';

interface DetailedUptimeGraphProps {
  data: UptimeRecord[];
  compact?: boolean;
}

export function DetailedUptimeGraph({ data, compact = false }: DetailedUptimeGraphProps) {
  return (
    <div className={compact ? "" : "bg-white p-4 rounded-lg shadow"}>
      <h3 className={`font-semibold mb-4 ${compact ? "text-sm" : "text-lg"}`}>
        Last 30 Minutes Uptime
      </h3>
      <div className="h-24 relative">
        <div className="absolute inset-0 flex items-end">
          {data.map((record, index) => (
            <div
              key={record.timestamp}
              className="flex-1 h-full flex items-end mx-px"
            >
              <div
                className={`w-full transition-all duration-200 ${
                  record.status === 'Running' ? 'bg-green-500' : 'bg-red-500'
                }`}
                style={{ height: record.status === 'Running' ? '100%' : '20%' }}
              />
            </div>
          ))}
        </div>
      </div>
      <div className="flex justify-between mt-2 text-xs text-gray-500">
        <span>{data.length * 5 / 60} min ago</span>
        <span>now</span>
      </div>
    </div>
  );
}