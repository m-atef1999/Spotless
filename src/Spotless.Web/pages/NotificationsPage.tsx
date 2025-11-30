import React, { useState } from "react";
import {
  Bell,
  Check,
  Trash2,
  RefreshCw,
  ChevronLeft,
  ChevronRight,
} from "lucide-react";
import { DashboardLayout } from "../layouts/DashboardLayout";
import { Button } from "../components/ui/Button";
import { NotificationsService } from "../lib/api";
import { NotificationType } from "../lib/constants";
import { useAuthStore } from "../store/authStore";
import { useQuery, useQueryClient } from "@tanstack/react-query";

export const NotificationsPage: React.FC = () => {
  const { role } = useAuthStore();
  const [filter, setFilter] = useState<"all" | "unread">("all");
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const queryClient = useQueryClient();

  const {
    data: notifications = [],
    isFetching,
    refetch,
  } = useQuery<any[], Error>({
    queryKey: ["notifications", filter, pageNumber],
    queryFn: async () => {
      const data = await NotificationsService.getApiNotifications({
        unreadOnly: filter === "unread",
        page: pageNumber,
        pageSize: pageSize,
      });
      return (Array.isArray(data) ? data : []) as any[];
    },
    keepPreviousData: true,
    staleTime: 10000,
  } as any);

  const hasMore = (notifications?.length || 0) === pageSize;

  const markAsRead = async (id: string) => {
    try {
      await NotificationsService.putApiNotificationsRead({ id });
      queryClient.invalidateQueries({ queryKey: ["notifications"] });
    } catch (error) {
      console.error("Failed to mark as read:", error);
    }
  };

  const deleteNotification = async (id: string) => {
    try {
      await NotificationsService.deleteApiNotifications({ id });
      queryClient.invalidateQueries({ queryKey: ["notifications"] });
    } catch (error) {
      console.error("Failed to delete notification:", error);
    }
  };

  const getNotificationTypeColor = (type: number | undefined) => {
    switch (type) {
      case NotificationType.OrderCreated:
      case NotificationType.OrderConfirmed:
      case NotificationType.OrderInProgress:
      case NotificationType.OrderCompleted:
      case NotificationType.OrderAssigned:
        return "bg-blue-100 text-blue-800 border-blue-200 dark:bg-blue-900/30 dark:text-blue-400 dark:border-blue-800";
      case NotificationType.OrderCancelled:
      case NotificationType.PaymentFailed:
      case NotificationType.DriverApplicationRejected:
        return "bg-red-100 text-red-800 border-red-200 dark:bg-red-900/30 dark:text-red-400 dark:border-red-800";
      case NotificationType.PaymentReceived:
      case NotificationType.DriverApplicationApproved:
        return "bg-green-100 text-green-800 border-green-200 dark:bg-green-900/30 dark:text-green-400 dark:border-green-800";
      case NotificationType.System:
      case NotificationType.Promotion:
        return "bg-gray-100 text-gray-800 border-gray-200 dark:bg-slate-800 dark:text-slate-400 dark:border-slate-700";
      default:
        return "bg-gray-100 text-gray-800 border-gray-200 dark:bg-slate-800 dark:text-slate-400 dark:border-slate-700";
    }
  };

  const getNotificationTypeLabel = (type: number | undefined) => {
    switch (type) {
      case NotificationType.OrderCreated:
        return "Order Created";
      case NotificationType.OrderConfirmed:
        return "Order Confirmed";
      case NotificationType.OrderAssigned:
        return "Order Assigned";
      case NotificationType.OrderInProgress:
        return "Order In Progress";
      case NotificationType.OrderCompleted:
        return "Order Completed";
      case NotificationType.OrderCancelled:
        return "Order Cancelled";
      case NotificationType.PaymentReceived:
        return "Payment Received";
      case NotificationType.PaymentFailed:
        return "Payment Failed";
      case NotificationType.DriverApplicationApproved:
        return "Driver Approved";
      case NotificationType.DriverApplicationRejected:
        return "Driver Rejected";
      case NotificationType.System:
        return "System";
      case NotificationType.Promotion:
        return "Promotion";
      default:
        return "Notification";
    }
  };

  return (
    <DashboardLayout role={role || "Customer"}>
      <div className="p-6 space-y-6">
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
          <h1 className="text-3xl font-bold text-slate-900 dark:text-white flex items-center gap-3">
            <Bell className="w-8 h-8 text-cyan-500" />
            Notifications
          </h1>
          <div className="flex gap-2 w-full sm:w-auto">
            <Button onClick={() => refetch()} variant="secondary" size="sm">
              <RefreshCw className="w-4 h-4 mr-2" />
              Refresh
            </Button>
            <div className="flex bg-slate-100 dark:bg-slate-800 rounded-lg p-1">
              <button
                onClick={() => setFilter("all")}
                className={`px-4 py-1.5 rounded-md text-sm font-medium transition-all ${
                  filter === "all"
                    ? "bg-white dark:bg-slate-700 text-slate-900 dark:text-white shadow-sm"
                    : "text-slate-500 dark:text-slate-400 hover:text-slate-900 dark:hover:text-white"
                }`}
              >
                All
              </button>
              <button
                onClick={() => setFilter("unread")}
                className={`px-4 py-1.5 rounded-md text-sm font-medium transition-all ${
                  filter === "unread"
                    ? "bg-white dark:bg-slate-700 text-slate-900 dark:text-white shadow-sm"
                    : "text-slate-500 dark:text-slate-400 hover:text-slate-900 dark:hover:text-white"
                }`}
              >
                Unread
              </button>
            </div>
          </div>
        </div>

        {isFetching && notifications.length === 0 ? (
          <div className="flex items-center justify-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-cyan-600"></div>
          </div>
        ) : notifications.length === 0 ? (
          <div className="text-center py-12 bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800">
            <Bell className="w-16 h-16 text-slate-300 dark:text-slate-700 mx-auto mb-4" />
            <p className="text-slate-500 dark:text-slate-400 text-lg">
              No notifications found.
            </p>
          </div>
        ) : (
          <div className="space-y-4">
            {notifications.map((notification) => (
              <div
                key={notification.id}
                className={`p-4 rounded-xl border-2 transition-all ${
                  notification.isRead
                    ? "bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800"
                    : "bg-cyan-50 dark:bg-cyan-900/10 border-cyan-200 dark:border-cyan-800 shadow-sm"
                }`}
              >
                <div className="flex justify-between items-start gap-4">
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-2">
                      <span
                        className={`px-2 py-0.5 rounded text-xs font-semibold border ${getNotificationTypeColor(
                          notification.type
                        )}`}
                      >
                        {getNotificationTypeLabel(notification.type)}
                      </span>
                      {!notification.isRead && (
                        <span className="px-2 py-0.5 rounded bg-cyan-500 text-white text-xs font-semibold">
                          NEW
                        </span>
                      )}
                      <span className="text-xs text-slate-400">
                        {notification.createdAt
                          ? new Date(notification.createdAt).toLocaleString()
                          : ""}
                      </span>
                    </div>
                    <h3 className="font-semibold text-slate-900 dark:text-white text-lg">
                      {notification.title}
                    </h3>
                    <p className="text-slate-600 dark:text-slate-300 mt-1">
                      {notification.message}
                    </p>
                  </div>
                  <div className="flex gap-2 shrink-0">
                    {!notification.isRead && (
                      <button
                        onClick={() => markAsRead(notification.id!)}
                        className="p-2 text-cyan-600 hover:bg-cyan-100 dark:text-cyan-400 dark:hover:bg-cyan-900/30 rounded-lg transition-colors"
                        title="Mark as read"
                      >
                        <Check className="w-5 h-5" />
                      </button>
                    )}
                    <button
                      onClick={() => deleteNotification(notification.id!)}
                      className="p-2 text-red-600 hover:bg-red-100 dark:text-red-400 dark:hover:bg-red-900/30 rounded-lg transition-colors"
                      title="Delete"
                    >
                      <Trash2 className="w-5 h-5" />
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Pagination */}
        <div className="flex items-center justify-between pt-4 border-t border-slate-200 dark:border-slate-800">
          <Button
            variant="outline"
            onClick={() => setPageNumber((p) => Math.max(1, p - 1))}
            disabled={pageNumber === 1 || isFetching}
          >
            <ChevronLeft className="w-4 h-4 mr-2" />
            Previous
          </Button>
          <span className="text-sm text-slate-500 dark:text-slate-400">
            Page {pageNumber}
          </span>
          <Button
            variant="outline"
            onClick={() => setPageNumber((p) => p + 1)}
            disabled={!hasMore || isFetching}
          >
            Next
            <ChevronRight className="w-4 h-4 ml-2" />
          </Button>
        </div>
      </div>
    </DashboardLayout>
  );
};
