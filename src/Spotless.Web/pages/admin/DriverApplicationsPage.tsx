import React, { useState, useEffect } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { DashboardLayout } from "../../layouts/DashboardLayout";
import { Button } from "../../components/ui/Button";
import { DriversService, OpenAPI, type DriverApplicationDto } from "../../lib/api";
import { useToast } from "../../components/ui/Toast";
import { Check, X, FileText, User, Truck } from "lucide-react";

// Helper to get token from OpenAPI config
const getToken = async () => {
  if (typeof OpenAPI.TOKEN === 'function') {
    return await OpenAPI.TOKEN({} as any);
  }
  return OpenAPI.TOKEN;
};

export const DriverApplicationsPage: React.FC = () => {
  const [status, setStatus] = useState<"Submitted" | "Approved" | "Rejected">(
    "Submitted"
  );
  const queryClient = useQueryClient();
  const { addToast } = useToast();

  // Prefetch all tabs on mount for instant switching
  useEffect(() => {
    const statuses: ("Submitted" | "Approved" | "Rejected")[] = ["Submitted", "Approved", "Rejected"];

    statuses.forEach((s) => {
      queryClient.prefetchQuery({
        queryKey: ["driver-applications", s],
        queryFn: async () => {
          const token = await getToken();
          const response = await fetch(
            `${OpenAPI.BASE}/api/Drivers/admin/applications?status=${encodeURIComponent(s)}`,
            {
              headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
              },
            }
          );
          if (!response.ok) return [];
          const result = await response.json();
          if (Array.isArray(result)) return result;
          if (result?.data && Array.isArray(result.data)) return result.data;
          return [];
        },
        staleTime: 60000,
      });
    });
  }, [queryClient]);


  const { data: applications = [], isLoading, isError, error } = useQuery<any[], Error>({
    queryKey: ["driver-applications", status],
    queryFn: async () => {
      const token = await getToken();
      const response = await fetch(
        `${OpenAPI.BASE}/api/Drivers/admin/applications?status=${encodeURIComponent(status)}`,
        {
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        }
      );
      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to fetch applications: ${response.status} - ${errorText}`);
      }
      const result = await response.json();
      // PagedResponse returns { data: [...], totalCount, pageNumber, pageSize }
      if (Array.isArray(result)) return result;
      if (result?.data && Array.isArray(result.data)) return result.data;
      return [];
    },
    staleTime: 60000,
    gcTime: 300000,
    retry: 1,
  } as any);

  const approveMutation = useMutation({
    mutationFn: async (applicationId: string) => {
      await DriversService.postApiDriversAdminApplicationsApprove({
        applicationId,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["driver-applications"] });
      addToast("Application approved successfully", "success");
    },
    onError: () => {
      addToast("Failed to approve application", "error");
    },
  });

  const rejectMutation = useMutation({
    mutationFn: async (applicationId: string) => {
      await DriversService.postApiDriversAdminRegistrationsReject({
        applicationId,
        requestBody: "Application rejected by admin", // Simple rejection for now
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["driver-applications"] });
      addToast("Application rejected", "info");
    },
    onError: () => {
      addToast("Failed to reject application", "error");
    },
  });

  const handleApprove = (id: string) => {
    if (window.confirm("Are you sure you want to approve this application?")) {
      approveMutation.mutate(id);
    }
  };

  const handleReject = (id: string) => {
    if (window.confirm("Are you sure you want to reject this application?")) {
      rejectMutation.mutate(id);
    }
  };

  if (isError) {
    return (
      <DashboardLayout role="Admin">
        <div className="p-6">
          <div className="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-6">
            <h3 className="text-lg font-semibold text-red-800 dark:text-red-400 mb-2">Failed to load applications</h3>
            <p className="text-red-600 dark:text-red-300 mb-4">{(error as any)?.message || 'Unknown error'}</p>
            <Button onClick={() => window.location.reload()}>Retry</Button>
          </div>
        </div>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout role="Admin">
      <div className="space-y-8">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
            Driver Applications
          </h1>
          <p className="text-slate-500 dark:text-slate-400 mt-1">
            Review and manage incoming driver registration requests.
          </p>
        </div>

        <div className="flex gap-2 border-b border-slate-200 dark:border-slate-800">
          {(["Submitted", "Approved", "Rejected"] as const).map((tab) => (
            <button
              key={tab}
              onClick={() => setStatus(tab)}
              className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${status === tab
                ? "border-cyan-500 text-cyan-600 dark:text-cyan-400"
                : "border-transparent text-slate-500 hover:text-slate-700 dark:text-slate-400 dark:hover:text-slate-300"
                }`}
            >
              {tab === "Submitted" ? "Pending" : tab}
            </button>
          ))}
        </div>

        {isLoading ? (
          <div className="flex items-center justify-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-cyan-600"></div>
          </div>
        ) : (
          <div className="space-y-4">
            {applications.length === 0 ? (
              <div className="text-center py-12 bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800">
                <FileText className="w-12 h-12 text-slate-300 mx-auto mb-4" />
                <h3 className="text-lg font-medium text-slate-900 dark:text-white">
                  No Pending Applications
                </h3>
                <p className="text-slate-500">
                  All caught up! There are no new driver applications to review.
                </p>
              </div>
            ) : (
              applications.map((app: DriverApplicationDto) => (
                <div
                  key={app.id}
                  className="bg-white dark:bg-slate-900 p-6 rounded-xl border border-slate-200 dark:border-slate-800 shadow-sm flex flex-col md:flex-row md:items-center justify-between gap-6"
                >
                  <div className="flex items-start gap-4">
                    <div className="w-12 h-12 bg-orange-100 dark:bg-orange-900/30 rounded-full flex items-center justify-center text-orange-600 dark:text-orange-400 shrink-0">
                      <User className="w-6 h-6" />
                    </div>
                    <div>
                      <h3 className="text-lg font-semibold text-slate-900 dark:text-white">
                        {app.customerName}
                      </h3>
                      <div className="space-y-1 mt-1">
                        <p className="text-sm text-slate-600 dark:text-slate-300 flex items-center gap-2">
                          <span className="font-medium">Email:</span>{" "}
                          {app.customerEmail}
                        </p>
                        <p className="text-sm text-slate-600 dark:text-slate-300 flex items-center gap-2">
                          <span className="font-medium">Phone:</span>{" "}
                          {app.customerPhone}
                        </p>
                        <p className="text-sm text-slate-600 dark:text-slate-300 flex items-center gap-2">
                          <Truck className="w-4 h-4 text-slate-400" />
                          {app.vehicleInfo}
                        </p>
                      </div>
                    </div>
                  </div>

                  <div className="flex items-center gap-3 md:self-center self-end">
                    {status === "Submitted" ? (
                      <>
                        <Button
                          variant="outline"
                          onClick={() => handleReject(app.id!)}
                          isLoading={rejectMutation.isPending}
                          className="text-red-600 hover:bg-red-50 border-red-200"
                        >
                          <X className="w-4 h-4 mr-2" />
                          Reject
                        </Button>
                        <Button
                          onClick={() => handleApprove(app.id!)}
                          isLoading={approveMutation.isPending}
                          className="bg-green-600 hover:bg-green-700 text-white"
                        >
                          <Check className="w-4 h-4 mr-2" />
                          Approve
                        </Button>
                      </>
                    ) : (
                      <span className={`px-3 py-1 rounded-full text-sm font-medium ${status === "Approved"
                        ? "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400"
                        : "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400"
                        }`}>
                        {status}
                      </span>
                    )}
                  </div>
                </div>
              ))
            )}
          </div>
        )}
      </div>
    </DashboardLayout>
  );
};
