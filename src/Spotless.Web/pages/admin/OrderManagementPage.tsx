import React, { useState } from "react";
import { Search, Filter, Eye, Calendar, User, RefreshCw, MoreVertical, X } from "lucide-react";
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

  const updateStatusMutation = useMutation({
    mutationFn: async ({ orderId, status }: { orderId: string; status: number }) => {
      await OrdersService.putApiOrdersStatus({ id: orderId, status });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["orders"] });
      addToast("Order status updated!", "success");
    },
    onError: () => {
      addToast("Failed to update order status.", "error");
    },
  });

  const cancelOrderMutation = useMutation({
    mutationFn: async (orderId: string) => {
      await OrdersService.postApiOrdersCancel({ id: orderId });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["orders"] });
      addToast("Order cancelled.", "info");
    },
    onError: () => {
      addToast("Failed to cancel order.", "error");
    },
  });

  const [showAssignModal, setShowAssignModal] = useState(false);
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
  const [actionMenuOrderId, setActionMenuOrderId] = useState<string | null>(null);

  const handleAssignDriver = (orderId: string) => {
    setSelectedOrderId(orderId);
    setShowAssignModal(true);
  };

  const handleDriverSelected = (driverId: string) => {
    if (!selectedOrderId) return;
    assignDriverMutation.mutate({ orderId: selectedOrderId, driverId });
  };

  const handleStatusChange = (orderId: string, newStatus: number) => {
    updateStatusMutation.mutate({ orderId, status: newStatus });
    setActionMenuOrderId(null);
  };

  const handleCancelOrder = (orderId: string) => {
    if (window.confirm("Are you sure you want to cancel this order?")) {
      cancelOrderMutation.mutate(orderId);
    }
    setActionMenuOrderId(null);
  };

  const getStatusBadge = (status: number) => {
    switch (status) {
      case OrderStatus.PaymentFailed:
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400">
            Payment Failed
          </span>
        );
      case OrderStatus.Requested:
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400">
            Pending
          </span>
        );
      case OrderStatus.Confirmed:
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400">
            Confirmed
          </span>
        );
      case OrderStatus.DriverAssigned:
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-indigo-100 text-indigo-800 dark:bg-indigo-900/30 dark:text-indigo-400">
            Driver Assigned
          </span>
        );
      case OrderStatus.PickedUp:
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-cyan-100 text-cyan-800 dark:bg-cyan-900/30 dark:text-cyan-400">
            Picked Up
          </span>
        );
      case OrderStatus.InCleaning:
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400">
            In Progress
          </span>
        );
      case OrderStatus.OutForDelivery:
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400">
            Out for Delivery
          </span>
        );
      case OrderStatus.Delivered:
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">
            Completed
          </span>
        );
      case OrderStatus.Cancelled:
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400">
            Cancelled
          </span>
        );
      default:
        return (
          <span className="px-2 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-800 dark:bg-slate-800 dark:text-slate-400">
            Status {status}
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
                        <div className="flex justify-end gap-2 items-center relative">
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
                          {order.status !== OrderStatus.Delivered &&
                            order.status !== OrderStatus.Cancelled && (
                              <div className="relative">
                                <Button
                                  size="sm"
                                  variant="ghost"
                                  onClick={() =>
                                    setActionMenuOrderId(
                                      actionMenuOrderId === order.id ? null : order.id
                                    )
                                  }
                                >
                                  <MoreVertical className="w-4 h-4" />
                                </Button>
                                {actionMenuOrderId === order.id && (
                                  <div className="absolute right-0 top-8 z-50 bg-white dark:bg-slate-800 rounded-lg shadow-lg border border-slate-200 dark:border-slate-700 py-1 w-48">
                                    <div className="px-3 py-2 text-xs font-semibold text-slate-400 uppercase">
                                      Change Status
                                    </div>
                                    {order.status !== OrderStatus.Confirmed && (
                                      <button
                                        className="w-full px-3 py-2 text-left text-sm text-slate-700 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-700"
                                        onClick={() => handleStatusChange(order.id, OrderStatus.Confirmed)}
                                      >
                                        Mark as Confirmed
                                      </button>
                                    )}
                                    {order.status !== OrderStatus.InCleaning && (
                                      <button
                                        className="w-full px-3 py-2 text-left text-sm text-slate-700 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-700"
                                        onClick={() => handleStatusChange(order.id, OrderStatus.InCleaning)}
                                      >
                                        Mark as In Progress
                                      </button>
                                    )}
                                    <button
                                      className="w-full px-3 py-2 text-left text-sm text-slate-700 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-700"
                                      onClick={() => handleStatusChange(order.id, OrderStatus.Delivered)}
                                    >
                                      Mark as Completed
                                    </button>
                                    <div className="border-t border-slate-200 dark:border-slate-700 my-1"></div>
                                    <button
                                      className="w-full px-3 py-2 text-left text-sm text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20 flex items-center gap-2"
                                      onClick={() => handleCancelOrder(order.id)}
                                    >
                                      <X className="w-4 h-4" />
                                      Cancel Order
                                    </button>
                                  </div>
                                )}
                              </div>
                            )}
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
