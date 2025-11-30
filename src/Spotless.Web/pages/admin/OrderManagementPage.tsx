import React, { useState } from "react";
import { Search, Filter, Eye, Calendar, User, RefreshCw } from "lucide-react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { DashboardLayout } from "../../layouts/DashboardLayout";
import { Button } from "../../components/ui/Button";
import { Input } from "../../components/ui/Input";
import { OrderStatus, DriversService, OrdersService } from "../../lib/api";
import { useToast } from "../../components/ui/Toast";
import AssignDriverModal from "../../components/ui/AssignDriverModal";

import { useDebounce } from "../../hooks/useDebounce";

export const OrderManagementPage: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const debouncedSearch = useDebounce(searchTerm, 500);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const queryClient = useQueryClient();
  const { addToast } = useToast();

  const {
    data: orders = [],
    isLoading,
    refetch,
  } = useQuery<any[], Error>({
    queryKey: ["orders", pageNumber, debouncedSearch],
    queryFn: async () => {
      const response = await OrdersService.getApiOrdersAdmin({
        pageNumber,
        pageSize,
        searchTerm: debouncedSearch || undefined,
      });
      return (response.data as any[]) || [];
    },
    // keep previous page data during fetch to avoid UI flashing
    keepPreviousData: true,
    staleTime: 10000,
    // keep server refresh but make it a bit less aggressive to reduce load
    refetchInterval: 30000,
  } as any);

  const assignDriverMutation = useMutation({
    mutationFn: async ({
      orderId,
      driverId,
    }: {
      orderId: string;
      driverId: string;
    }) => {
      await DriversService.postApiDriversAdminAssign({
        requestBody: {
          orderId,
          driverId,
        },
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["orders"] });
      addToast("Driver assigned successfully!", "success");
    },
    onError: () => {
      addToast("Failed to assign driver. Please try again.", "error");
    },
  });

  const [showAssignModal, setShowAssignModal] = useState(false);
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);

  const handleAssignDriver = (orderId: string) => {
    setSelectedOrderId(orderId);
    setShowAssignModal(true);
  };

  const handleDriverSelected = (driverId: string) => {
    if (!selectedOrderId) return;
    assignDriverMutation.mutate({ orderId: selectedOrderId, driverId });
  };

  const getStatusBadge = (status: number) => {
    switch (status) {
      case OrderStatus.Requested: // Pending
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400">
            Pending
          </span>
        );
      case OrderStatus.Confirmed: // Confirmed
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400">
            Confirmed
          </span>
        );
      case OrderStatus.InCleaning: // InProgress
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400">
            In Progress
          </span>
        );
      case OrderStatus.Delivered: // Completed
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">
            Completed
          </span>
        );
      case OrderStatus.Cancelled: // Cancelled
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400">
            Cancelled
          </span>
        );
      default:
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-800 dark:bg-slate-800 dark:text-slate-400">
            Unknown
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
              Order Management
            </h1>
            <p className="text-slate-500 dark:text-slate-400 mt-1">
              View and manage customer bookings.
            </p>
          </div>
          <div className="flex gap-2 w-full sm:w-auto">
            <div className="w-full sm:w-64">
              <Input
                placeholder="Search orders..."
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
                    Order ID
                  </th>
                  <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                    Customer
                  </th>
                  <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                    Service
                  </th>
                  <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                    Date
                  </th>
                  <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                    Assigned To
                  </th>
                  <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                    Amount
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
                      colSpan={8}
                      className="px-6 py-8 text-center text-slate-500"
                    >
                      Loading orders...
                    </td>
                  </tr>
                ) : orders.length === 0 ? (
                  <tr>
                    <td
                      colSpan={8}
                      className="px-6 py-8 text-center text-slate-500"
                    >
                      No orders found.
                    </td>
                  </tr>
                ) : (
                  orders.map((order: any) => (
                    <tr
                      key={order.id}
                      className="hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors"
                    >
                      <td className="px-6 py-4 font-mono text-sm text-slate-500">
                        #{order.id?.substring(0, 8)}
                      </td>
                      <td className="px-6 py-4">
                        <div className="flex items-center gap-2">
                          <User className="w-4 h-4 text-slate-400" />
                          <span className="font-medium text-slate-900 dark:text-white">
                            {order.customerName}
                          </span>
                        </div>
                      </td>
                      <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-300">
                        {order.items?.[0]?.serviceName || "Multiple Items"}
                      </td>
                      <td className="px-6 py-4">
                        <div className="flex items-center gap-2 text-sm text-slate-500">
                          <Calendar className="w-4 h-4" />
                          {new Date(order.orderDate).toLocaleDateString()}
                        </div>
                      </td>
                      <td className="px-6 py-4">
                        {getStatusBadge(order.status)}
                      </td>
                      <td className="px-6 py-4 text-sm text-slate-500">
                        {order.driverName || (
                          <span className="text-slate-400 italic">
                            Unassigned
                          </span>
                        )}
                      </td>
                      <td className="px-6 py-4 font-medium text-slate-900 dark:text-white">
                        {order.totalAmount?.currency}{" "}
                        {order.totalAmount?.amount?.toFixed(2)}
                      </td>
                      <td className="px-6 py-4 text-right">
                        <div className="flex justify-end gap-2">
                          {!order.driverName &&
                            order.status !== OrderStatus.Delivered &&
                            order.status !== OrderStatus.Cancelled && (
                              <Button
                                size="sm"
                                variant="outline"
                                onClick={() => handleAssignDriver(order.id)}
                              >
                                Assign
                              </Button>
                            )}
                          <Button size="sm" variant="ghost">
                            <Eye className="w-4 h-4" />
                          </Button>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>

        {/* Assign Driver Modal */}
        <AssignDriverModal
          isOpen={showAssignModal}
          onClose={() => {
            setShowAssignModal(false);
            setSelectedOrderId(null);
          }}
          onSelect={handleDriverSelected}
        />

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
            disabled={orders.length < pageSize || isLoading}
          >
            Next
          </Button>
        </div>
      </div>
    </DashboardLayout>
  );
};
