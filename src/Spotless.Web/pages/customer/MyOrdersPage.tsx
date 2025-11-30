import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { format } from "date-fns";
import {
  Package,
  Calendar,
  MapPin,
  ChevronRight,
  Loader2,
  AlertCircle,
} from "lucide-react";
import { DashboardLayout } from "../../layouts/DashboardLayout";
import { Button } from "../../components/ui/Button";
import { useToast } from "../../components/ui/Toast";

import {
  OrdersService,
  type OrderDto,
  OrderStatus,
  getOrderStatusLabel,
  getOrderStatusColor,
} from "../../lib/api";
import { useQuery, useQueryClient } from "@tanstack/react-query";

export const MyOrdersPage: React.FC = () => {
  const [orders, setOrders] = useState<OrderDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { addToast } = useToast();
  const navigate = useNavigate();

  const [pageSize, setPageSize] = useState(20);
  const queryClient = useQueryClient();

  const { data: ordersResponse, isFetching } = useQuery<OrderDto[], Error>({
    queryKey: ["myOrders", pageSize],
    queryFn: async () => {
      const response = await OrdersService.getApiOrdersCustomer({
        pageNumber: 1,
        pageSize: pageSize,
      });
      const data = (response.data as unknown as OrderDto[]) || [];
      // Sort by date descending with safety check
      const sorted = data.sort((a, b) => {
        const dateA = a?.scheduledDate
          ? new Date(a.scheduledDate).getTime()
          : 0;
        const dateB = b?.scheduledDate
          ? new Date(b.scheduledDate).getTime()
          : 0;
        return dateB - dateA;
      });
      return sorted;
    },
    keepPreviousData: true,
    staleTime: 10000,
  } as any);

  useEffect(() => {
    setIsLoading(isFetching);
    setError(null);
    setOrders(ordersResponse || []);
  }, [ordersResponse, isFetching]);

  const handleCancelOrder = async (orderId: string) => {
    if (!confirm("Are you sure you want to cancel this order?")) return;

    try {
      await OrdersService.postApiOrdersCancel({ id: orderId });
      addToast("Order cancelled successfully", "success");
      // Refresh orders
      queryClient.invalidateQueries({ queryKey: ["myOrders"] });
    } catch (err) {
      console.error("Failed to cancel order", err);
      addToast("Failed to cancel order", "error");
    }
  };

  const canCancel = (status?: number) => {
    return status === OrderStatus.Requested || status === OrderStatus.Confirmed;
  };

  return (
    <DashboardLayout role="Customer">
      <div className="max-w-5xl mx-auto space-y-8">
        <div className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
            My Orders
          </h1>
          <div className="flex gap-4 items-center">
            <label className="sr-only" htmlFor="pageSizeSelect">
              Items per page
            </label>
            <select
              id="pageSizeSelect"
              value={pageSize}
              onChange={(e) => setPageSize(Number(e.target.value))}
              className="bg-white dark:bg-slate-900 text-slate-900 dark:text-white border border-slate-200 dark:border-slate-800 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-cyan-500 outline-none"
            >
              <option value={20}>Show 20</option>
              <option value={40}>Show 40</option>
              <option value={60}>Show 60</option>
              <option value={80}>Show 80</option>
              <option value={100}>Show 100</option>
            </select>
            <Link to="/customer/new-order">
              <Button>New Order</Button>
            </Link>
          </div>
        </div>

        {isLoading ? (
          <div className="flex justify-center py-12">
            <Loader2 className="w-8 h-8 animate-spin text-cyan-500" />
          </div>
        ) : error ? (
          <div className="p-4 bg-red-50 text-red-600 rounded-xl flex items-center gap-2">
            <AlertCircle className="w-5 h-5" />
            {error}
          </div>
        ) : orders.length === 0 ? (
          <div className="text-center py-16 bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800">
            <div className="w-16 h-16 bg-slate-100 dark:bg-slate-800 rounded-full flex items-center justify-center mx-auto mb-4">
              <Package className="w-8 h-8 text-slate-400" />
            </div>
            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-2">
              No orders yet
            </h3>
            <p className="text-slate-500 dark:text-slate-400 mb-6 max-w-sm mx-auto">
              You haven't placed any orders yet. Start by creating your first
              cleaning request.
            </p>
            <Link to="/customer/new-order">
              <Button>Create First Order</Button>
            </Link>
          </div>
        ) : (
          <div className="space-y-4">
            {orders.map((order) => (
              <div
                key={order.id}
                className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-4 sm:p-6 hover:border-cyan-300 dark:hover:border-cyan-700 transition-all group"
              >
                <div className="flex flex-col sm:flex-row justify-between gap-4">
                  <div className="space-y-1">
                    <div className="flex items-center gap-3">
                      <span className="font-mono text-sm text-slate-500">
                        #{order.id?.slice(0, 8)}
                      </span>
                      <span
                        className={`px-2.5 py-0.5 rounded-full text-xs font-medium ${getOrderStatusColor(
                          order.status || 0
                        )}`}
                      >
                        {getOrderStatusLabel(order.status || 0)}
                      </span>
                    </div>
                    <div className="flex items-center gap-2 text-slate-900 dark:text-white font-medium">
                      <Calendar className="w-4 h-4 text-slate-400" />
                      {order.scheduledDate ? (
                        <div className="flex flex-col">
                          {order.startTime ? (
                            <span className="text-sm text-slate-700 dark:text-slate-300 font-medium">
                              Pickup Time at{" "}
                              {format(new Date(order.scheduledDate), "MMM d")},{" "}
                              {format(
                                new Date(`2000-01-01T${order.startTime}`),
                                "h:mm a"
                              )}
                            </span>
                          ) : (
                            <span className="text-sm text-slate-500">
                              Pickup Time: Not Scheduled
                            </span>
                          )}
                          {order.estimatedDurationHours &&
                            order.estimatedDurationHours > 0 && (
                              <span className="text-xs text-slate-500 mt-0.5">
                                Est. Duration: {order.estimatedDurationHours}{" "}
                                hrs
                              </span>
                            )}
                        </div>
                      ) : (
                        "Not scheduled"
                      )}
                    </div>

                    <div className="flex flex-col gap-1 mt-1">
                      {order.items && order.items.length > 0 ? (
                        <>
                          {order.items.slice(0, 3).map((item, idx) => (
                            <div
                              key={idx}
                              className="flex items-center gap-2 text-sm text-slate-600 dark:text-slate-300 font-semibold"
                            >
                              <Package className="w-4 h-4 text-cyan-500" />
                              {item.serviceName || "Service"}
                            </div>
                          ))}
                          {order.items.length > 3 && (
                            <div className="text-xs text-slate-500 pl-6">
                              and {order.items.length - 3} more...
                            </div>
                          )}
                        </>
                      ) : (
                        <div className="flex items-center gap-2 text-sm text-slate-600 dark:text-slate-300 font-semibold">
                          <Package className="w-4 h-4 text-cyan-500" />
                          {order.serviceName || "Service"}
                        </div>
                      )}
                    </div>

                    {order.createdAt && (
                      <div className="text-xs text-slate-400 ml-6 mt-1">
                        Ordered at:{" "}
                        {format(
                          new Date(
                            order.createdAt.endsWith("Z")
                              ? order.createdAt
                              : order.createdAt + "Z"
                          ),
                          "PP p"
                        )}
                      </div>
                    )}
                    <div className="flex items-center gap-2 text-sm text-slate-500 dark:text-slate-400">
                      <MapPin className="w-4 h-4" />
                      <span
                        className="truncate max-w-[200px]"
                        title={order.deliveryAddress || ""}
                      >
                        {order.deliveryAddress || "No address"}
                      </span>
                    </div>
                  </div>

                  <div className="flex flex-col sm:items-end justify-between gap-4">
                    <div className="text-lg font-bold text-cyan-600 dark:text-cyan-400">
                      {order.totalPrice?.toFixed(2)} EGP
                    </div>
                    <div className="flex gap-2">
                      {canCancel(order.status) && order.id && (
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleCancelOrder(order.id!)}
                          className="text-red-600 hover:bg-red-50 hover:text-red-700 border-red-200"
                        >
                          Cancel
                        </Button>
                      )}
                      <Button
                        variant="outline"
                        size="sm"
                        className="group-hover:bg-cyan-50 dark:group-hover:bg-cyan-900/20 group-hover:text-cyan-700 dark:group-hover:text-cyan-300 group-hover:border-cyan-200 dark:group-hover:border-cyan-800"
                        onClick={() => navigate(`/customer/orders/${order.id}`)}
                      >
                        View Details
                        <ChevronRight className="w-4 h-4 ml-1" />
                      </Button>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </DashboardLayout>
  );
};
