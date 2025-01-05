import React from 'react';
import { History } from 'lucide-react';
import { useServiceChanges } from '../../context/ServiceChangeContext';
import { RecentChangeItem } from './RecentChangeItem';

export function RecentChanges() {
  const { recentChanges } = useServiceChanges();

  if (recentChanges.length === 0) {
    return null;
  }

  const visibleChanges = recentChanges.slice(0, 3);
  const additionalChanges = Math.max(0, recentChanges.length - 3);

  return (
    <div className="mb-8">
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-2">
          <History size={16} className="text-blue-600" />
          <h2 className="text-lg font-semibold">Recent Changes</h2>
        </div>
        {additionalChanges > 0 && (
          <span className="text-sm text-gray-500">
            +{additionalChanges} more changes
          </span>
        )}
      </div>
      <div className="space-y-4">
        {visibleChanges.map(change => (
          <RecentChangeItem key={change.id} change={change} />
        ))}
      </div>
    </div>
  );
}