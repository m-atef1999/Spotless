import React, { useEffect, useState } from "react";
import {
  Search,
  Clock,
  Sparkles,
  Loader2,
  AlertCircle,
  Zap,
} from "lucide-react";
import { DashboardLayout } from "../../layouts/DashboardLayout";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";

import {
  ServicesService,
  type ServiceDto,
  type PagedResponse,
} from "../../lib/api";
import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { getServiceImage } from "../../lib/imageUtils";

export const ServicesPage: React.FC = () => {
  const navigate = useNavigate();
  const [services, setServices] = useState<ServiceDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [debouncedSearchTerm, setDebouncedSearchTerm] = useState("");

  // Debounce search term
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearchTerm(searchTerm);
    }, 500);
    return () => clearTimeout(timer);
  }, [searchTerm]);

  const {
    data: servicesResponse,
    isFetching,
    refetch,
  } = useQuery<any, Error>({
    queryKey: ["services", debouncedSearchTerm],
    queryFn: async () => {
      const response: PagedResponse = await ServicesService.getApiServices({
        nameSearchTerm: debouncedSearchTerm,
        pageNumber: 1,
        pageSize: 50,
      });
      return response;
    },
    keepPreviousData: true,
    staleTime: 10000,
  } as any);

  useEffect(() => {
    setIsLoading(isFetching);
    setError(null);
    setServices(servicesResponse?.data || []);
  }, [servicesResponse, isFetching]);

  const handleBookNow = (serviceId?: string) => {
    navigate(
      `/customer/new-order${serviceId ? `?serviceId=${serviceId}` : ""}`
    );
  };

  return (
    <DashboardLayout role="Customer">
      <div className="max-w-6xl mx-auto space-y-8">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold text-slate-900 dark:text-white flex items-center gap-3">
              <Sparkles className="w-8 h-8 text-cyan-500" />
              Our Services
            </h1>
            <p className="text-slate-500 dark:text-slate-400 mt-1">
              Choose from our wide range of professional cleaning services.
            </p>
          </div>
          <div className="w-full md:w-96">
            <Input
              placeholder="Search services..."
              icon={<Search className="w-5 h-5" />}
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </div>

        {isLoading ? (
          <div className="flex justify-center py-20">
            <Loader2 className="w-10 h-10 animate-spin text-cyan-500" />
          </div>
        ) : error ? (
          <div className="flex flex-col items-center justify-center py-20 text-center">
            <div className="w-16 h-16 bg-red-100 text-red-600 rounded-full flex items-center justify-center mb-4">
              <AlertCircle className="w-8 h-8" />
            </div>
            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-2">
              Error Loading Services
            </h3>
            <p className="text-slate-500 dark:text-slate-400 mb-6 max-w-md">
              {error}
            </p>
            <Button onClick={() => refetch()} variant="outline">
              Try Again
            </Button>
          </div>
        ) : services.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-20 text-center">
            <div className="w-16 h-16 bg-slate-100 text-slate-400 rounded-full flex items-center justify-center mb-4">
              <Search className="w-8 h-8" />
            </div>
            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-2">
              No Services Found
            </h3>
            <p className="text-slate-500 dark:text-slate-400 max-w-md">
              We couldn't find any services matching "{searchTerm}". Try
              adjusting your search terms.
            </p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {services.map((service) => (
              <div
                key={service.id}
                className="group bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800 p-6 hover:shadow-xl hover:border-cyan-200 dark:hover:border-cyan-900/50 transition-all duration-300 flex flex-col"
              >
                <div
                  className="mb-4 cursor-pointer"
                  onClick={() => navigate(`/customer/services/${service.id}`)}
                >
                  <div className="relative h-48 mb-4 rounded-xl overflow-hidden group-hover:shadow-md transition-all duration-300">
                    <img
                      src={getServiceImage(service)}
                      alt={service.name || ""}
                      className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                    />
                    <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent opacity-60" />
                    <div className="absolute bottom-3 left-3 text-white">
                      <div className="p-1.5 bg-white/20 backdrop-blur-md rounded-lg inline-flex">
                        <Sparkles className="w-4 h-4 text-white" />
                      </div>
                    </div>
                  </div>
                  <h3 className="text-xl font-bold text-slate-900 dark:text-white mb-2 group-hover:text-cyan-600 dark:group-hover:text-cyan-400 transition-colors">
                    {service.name}
                  </h3>
                  <p className="text-slate-500 dark:text-slate-400 text-sm line-clamp-3">
                    {service.description || "No description available."}
                  </p>
                </div>

                <div className="mt-auto pt-6 border-t border-slate-100 dark:border-slate-800">
                  <div className="flex items-center justify-between mb-4">
                    <div className="flex items-center gap-1.5 text-sm text-slate-500 dark:text-slate-400">
                      <Clock className="w-4 h-4" />
                      {service.estimatedDurationHours}h
                    </div>
                    <div className="flex flex-col items-end gap-1">
                      <div className="flex items-center gap-1.5 text-lg font-bold text-slate-900 dark:text-white">
                        <span className="text-sm font-bold text-cyan-500">
                          EGP
                        </span>
                        {service.basePrice?.toFixed(2)}
                      </div>
                      {service.maxWeightKg !== undefined && (
                        <span className="text-xs font-medium text-slate-500 dark:text-slate-400">
                          Max {service.maxWeightKg} KG
                        </span>
                      )}
                    </div>
                  </div>

                  <Button
                    onClick={() => service.id && handleBookNow(service.id)}
                    className="w-full shadow-lg shadow-cyan-500/20"
                  >
                    <Zap className="w-4 h-4 mr-2" />
                    Book Now
                  </Button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </DashboardLayout>
  );
};
