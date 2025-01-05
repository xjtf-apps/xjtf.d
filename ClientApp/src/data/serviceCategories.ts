export type ServiceCategory = {
  id: string;
  name: string;
  description: string;
};

export const serviceCategories: ServiceCategory[] = [
  {
    id: 'system',
    name: 'System Services',
    description: 'Core Windows system services'
  },
  {
    id: 'network',
    name: 'Network Services',
    description: 'Network-related services'
  },
  {
    id: 'security',
    name: 'Security Services',
    description: 'Security and authentication services'
  },
  {
    id: 'hardware',
    name: 'Hardware Services',
    description: 'Device and hardware management services'
  }
];