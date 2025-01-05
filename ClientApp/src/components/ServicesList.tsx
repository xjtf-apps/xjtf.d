import React, { useState } from 'react';
import { ServiceCard } from './ServiceCard';
import { PinnedServices } from './PinnedServices';
import { RecentChanges } from './recent-changes/RecentChanges';
import { ServiceStats } from './overview/ServiceStats';
import { Search, ChevronDown, List } from 'lucide-react';
import { useService } from '../context/ServiceContext';

export function ServicesList() {
  const [filter, setFilter] = useState('');
  const { services, pinnedServices, updatingServices } = useService();
  const [statusFilter, setStatusFilter] = useState<'all' | 'running' | 'stopped'>('all');

  const pinnedServicesList = services.filter(service => 
    pinnedServices.includes(service.serviceName)
  );

  const unpinnedServices = services.filter(service => 
    !pinnedServices.includes(service.serviceName)
  );

  const filteredServices = unpinnedServices.filter(service => {
    const matchesSearch = service.displayName.toLowerCase().includes(filter.toLowerCase()) ||
                         service.serviceName.toLowerCase().includes(filter.toLowerCase());
    const matchesStatus = statusFilter === 'all' || 
                         (statusFilter === 'running' && service.status === 'Running') ||
                         (statusFilter === 'stopped' && service.status === 'Stopped');
    return matchesSearch && matchesStatus;
  });

  return (
    <div className="w-full max-w-4xl mx-auto">
      <ServiceStats services={services} pinnedServices={pinnedServices} />
      <PinnedServices services={pinnedServicesList} />
      <RecentChanges />
      
      <div className="flex items-center gap-2 mb-4">
        <List size={16} className="text-blue-600" />
        <h2 className="text-lg font-semibold">All Services</h2>
      </div>
      
      <div className="mb-6 flex flex-col sm:flex-row gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={20} />
          <input
            type="text"
            placeholder="Search services..."
            className="w-full pl-10 pr-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={filter}
            onChange={(e) => setFilter(e.target.value)}
          />
        </div>
        <div className="relative">
          <select
            className="appearance-none px-6 pr-10 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white text-gray-700 min-w-[160px]"
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value as 'all' | 'running' | 'stopped')}
          >
            <option value="all">All Services</option>
            <option value="running">Running</option>
            <option value="stopped">Stopped</option>
          </select>
          <ChevronDown size={16} className="absolute right-4 top-1/2 transform -translate-y-1/2 text-gray-500 pointer-events-none" />
        </div>
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {filteredServices.map((service) => (
          <ServiceCard 
            key={service.serviceName} 
            service={service}
            isUpdating={updatingServices.has(service.serviceName)}
          />
        ))}
      </div>
    </div>
  );
}