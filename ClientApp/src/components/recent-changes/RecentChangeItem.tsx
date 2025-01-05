import React from 'react';
import { Link } from 'react-router-dom';
import { FadeTransition } from '../transitions/FadeTransition';
import { ServiceChange } from '../../types/recent-change';
import { StatusTransition } from './StatusTransition';

interface RecentChangeItemProps {
  change: ServiceChange;
}

export function RecentChangeItem({ change }: RecentChangeItemProps) {
  const { service, previousStatus, id } = change;
  
  return (
    <FadeTransition key={id} show={true}>
      <Link 
        to={`/service/${service.serviceName}`}
        className="block bg-white rounded-lg shadow-md p-4 hover:shadow-lg transition-shadow"
      >
        <div className="flex items-center justify-between mb-2">
          <h3 className="font-semibold text-lg">{service.displayName}</h3>
          <StatusTransition 
            previousStatus={previousStatus}
            currentStatus={service.status}
          />
        </div>
        <p className="text-sm text-gray-600">{service.description}</p>
      </Link>
    </FadeTransition>
  );
}