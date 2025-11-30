import React, { useState } from "react";
import {
  Search,
  Filter,
  User,
  AlertTriangle,
  Truck,
  RefreshCw,
} from "lucide-react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { DashboardLayout } from "../../layouts/DashboardLayout";
import { Button } from "../../components/ui/Button";
import { Input } from "../../components/ui/Input";
import { DriversService } from "../../lib/api";
import { useToast } from "../../components/ui/Toast";

import { useDebounce } from "../../hooks/useDebounce";

export const DriverManagementPage: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const debouncedSearch = useDebounce(searchTerm, 500);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const queryClient = useQueryClient();
  const { addToast } = useToast();

  const {
    data: drivers = [],
    isLoading,
    refetch,
  } = useQuery<any[], Error>({
    queryKey: ["drivers", pageNumber, debouncedSearch],
    queryFn: async () => {
      try {
        const response = await DriversService.getApiDrivers({
          pageNumber,
          pageSize,
          searchTerm: debouncedSearch || undefined,
        });
        const data = response.data;
        return Array.isArray(data) ? data : (data?.data || []);
      } catch (err) {
        console.error("Failed to load drivers", err);
        addToast("Failed to load drivers. Server error.", "error");
        return [];
      }
    },
    staleTime: 10000,
  });

  const revokeAccessMutation = useMutation({
    mutationFn: async (driverId: string) => {
      await DriversService.postApiDriversAdminRevoke({ driverId });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["drivers"] });
      addToast("Driver access revoked successfully", "success");
    },
    onError: () => {
      addToast("Failed to revoke access. Please try again.", "error");
    },
  });

  const handleRevokeAccess = (driverId: string, driverName: string) => {
    if (
      !window.confirm(
        `Are you sure you want to revoke access for driver ${driverName}? This action cannot be undone easily.`
      )
    ) {
      return;
    }
    revokeAccessMutation.mutate(driverId);
  };

  const getStatusBadge = (status: string) => {
    switch (status) {
      case "Online":
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">
            Online
          </span>
        );
      case "Offline":
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-800 dark:bg-slate-800 dark:text-slate-400">
            Offline
          </span>
        );
      case "Busy":
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400">
            Busy
          </span>
        );
      default:
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-800 dark:bg-slate-800 dark:text-slate-400">
            {status}
          </span>
        );
    }
  };

  return (
    <DashboardLayout role="Admin">
      <div className="space-y-8">
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
              Driver Management
            </h1>
            <p className="text-slate-500 dark:text-slate-400 mt-1">
              Manage active drivers and their access.
            </p>
          </div>
          <div className="flex gap-2 w-full sm:w-auto">
            <div className="w-full sm:w-64">
              <Input
                placeholder="Search drivers..."
                icon={<Search className="w-5 h-5" />}
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
            </div>
            <Button variant="outline" className="shrink-0">
              <Filter className="w-4 h-4 mr-2" />
              Filter
            </Button>
            <Button onClick={() => refetch()} variant="outline" size="icon">
              <RefreshCw className="w-4 h-4" />
            </Button>
          </div>
        </div>

        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-800">
                  <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                    Driver
                  </th>
                  <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                    Email
                  </th>
                  <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                    Phone
                  </th>
                  <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                    Vehicle
                  </th>
                  <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider text-right">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200 dark:divide-slate-800">
                {isLoading ? (
                  <tr>
                    <td
                      colSpan={6}
                      className="px-6 py-8 text-center text-slate-500"
                    >
                      Loading drivers...
                    </td>
                  </tr>
                ) : drivers.length === 0 ? (
                  <tr>
                    <td
                      colSpan={6}
                      className="px-6 py-8 text-center text-slate-500"
                    >
                      No drivers found.
                    </td>
                  </tr>
                ) : (
                  drivers.map((driver: any) => (
                    <tr
                      key={driver.id}
                      className="hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors"
                    >
                      <td className="px-6 py-4">
                        <div className="flex items-center gap-2">
                          <User className="w-4 h-4 text-slate-400" />
                          <span className="font-medium text-slate-900 dark:text-white">
                            {driver.name}
                          </span>
                        </div>
                      </td>
                      <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-300">
                        {driver.email}
                      </td>
                      <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-300">
                        {driver.phone}
                      </td>
                      <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-300">
                        <div className="flex items-center gap-2">
                          <Truck className="w-4 h-4 text-slate-400" />
                          {driver.vehicleInfo}
                        </div>
                      </td>
                      <td className="px-6 py-4">
                        {getStatusBadge(driver.status)}
                      </td>
                      <td className="px-6 py-4 text-right">
                        <Button
                          size="sm"
                          variant="ghost"
                          className="text-red-600 hover:text-red-700 hover:bg-red-50 dark:hover:bg-red-900/20"
                          onClick={() =>
                            handleRevokeAccess(driver.id, driver.name)
                          }
                          isLoading={
                            revokeAccessMutation.isPending &&
                            revokeAccessMutation.variables === driver.id
                          }
                        >
                          <AlertTriangle className="w-4 h-4 mr-1" />
                          Revoke
                        </Button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>

        {/* Pagination */}
        <div className="flex items-center justify-between">
          <Button
            variant="outline"
            onClick={() => setPageNumber((p) => Math.max(1, p - 1))}
            disabled={pageNumber === 1 || isLoading}
          >
            Previous
          </Button>
          <span className="text-sm text-slate-500">Page {pageNumber}</span>
          <Button
            variant="outline"
            onClick={() => setPageNumber((p) => p + 1)}
            disabled={drivers.length < pageSize || isLoading}
          >
            Next
          </Button>
        </div>
      </div>
    </DashboardLayout>
  );
};
