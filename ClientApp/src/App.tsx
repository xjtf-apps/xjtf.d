import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ServicesList } from './components/ServicesList';
import { ServiceDetails } from './components/ServiceDetails';
import { ServiceProvider } from './context/ServiceContext';
import { ServiceChangeProvider } from './context/ServiceChangeContext';
import { Navbar } from './components/navbar/Navbar';

export function App() {
  return (
    <ServiceChangeProvider>
      <ServiceProvider>
        <BrowserRouter>
          <div className="min-h-screen bg-gray-50">
            <Navbar />
            <main className="max-w-7xl mx-auto px-4 py-8 pb-16">
              <Routes>
                <Route path="/" element={<ServicesList />} />
                <Route path="/service/:name" element={<ServiceDetails />} />
              </Routes>
            </main>
          </div>
        </BrowserRouter>
      </ServiceProvider>
    </ServiceChangeProvider>
  );
}