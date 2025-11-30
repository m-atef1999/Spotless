import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { CustomersService, type CustomerDto } from "../../lib/api";
import { DashboardLayout } from "../../layouts/DashboardLayout";
import { useToast } from "../../components/ui/Toast";
import { Search, RefreshCw } from "lucide-react";
import { Button } from "../../components/ui/Button";
import { Input } from "../../components/ui/Input";

export function CustomerManagementPage() {
  const [searchTerm, setSearchTerm] = useState("");
  const [emailFilter, setEmailFilter] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const { addToast } = useToast();

  const {
    data: response,
    isLoading,
    isError,
    refetch,
  } = useQuery<any, Error>({
    queryKey: ["customers", pageNumber, searchTerm, emailFilter],
    queryFn: async () => {
      const result = await CustomersService.getApiCustomers({
        nameFilter: searchTerm || undefined,
        emailFilter: emailFilter || undefined,
        pageNumber,
        pageSize: 25,
      });
      return result;
    },
    // Keep previous page data while fetching next to avoid layout shift
    keepPreviousData: true,
    // Cache slightly longer to reduce re-fetches while navigating pages
    staleTime: 10000,
  } as any);

  const customers = (response?.data as CustomerDto[]) || [];
  const totalPages = response?.totalPages || 1;

  const handleSearch = () => {
    setPageNumber(1);
    refetch();
  };

  const handleClearFilters = () => {
    setSearchTerm("");
    setEmailFilter("");
    setPageNumber(1);
  };

  if (isError) {
    addToast("Failed to load customers", "error");
  }

  return (
    <DashboardLayout role="Admin">
      <div className="min-h-screen bg-gray-50 dark:bg-slate-950 p-6">
        <div className="max-w-7xl mx-auto space-y-6">
          {/* Header */}
          <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
            <div>
              <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
                Customer Management
              </h1>
              <p className="text-gray-600 dark:text-slate-400 mt-2">
                Manage and view all customer accounts
              </p>
            </div>
            <Button onClick={() => refetch()} variant="outline" size="sm">
              <RefreshCw className="w-4 h-4 mr-2" />
              Refresh
            </Button>
          </div>

          {/* Filters */}
          <div className="bg-white dark:bg-slate-900 rounded-xl shadow-sm border border-slate-200 dark:border-slate-800 p-6">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-slate-300 mb-2">
                  Search by Name
                </label>
                <Input
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  placeholder="Enter customer name..."
                  icon={<Search className="w-4 h-4" />}
                  onKeyPress={(e) => e.key === "Enter" && handleSearch()}
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-slate-300 mb-2">
                  Search by Email
                </label>
                <Input
                  value={emailFilter}
                  onChange={(e) => setEmailFilter(e.target.value)}
                  placeholder="Enter email..."
                  icon={<Search className="w-4 h-4" />}
                  onKeyPress={(e) => e.key === "Enter" && handleSearch()}
                />
              </div>
              <div className="flex items-end gap-2">
                <Button onClick={handleSearch} className="flex-1">
                  Search
                </Button>
                <Button
                  onClick={handleClearFilters}
                  variant="secondary"
                  className="flex-1"
                >
                  Clear
                </Button>
              </div>
            </div>
          </div>

          {/* Customer Table */}
          <div className="bg-white dark:bg-slate-900 rounded-xl shadow-sm border border-slate-200 dark:border-slate-800 overflow-hidden">
            {isLoading ? (
              <div className="flex items-center justify-center h-64">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-cyan-600"></div>
              </div>
            ) : customers.length === 0 ? (
              <div className="text-center py-12">
                <p className="text-gray-500 dark:text-slate-400 text-lg">
                  No customers found
                </p>
              </div>
            ) : (
              <>
                <div className="overflow-x-auto">
                  <table className="min-w-full divide-y divide-gray-200 dark:divide-slate-800">
                    <thead className="bg-gray-50 dark:bg-slate-800/50">
                      <tr>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-slate-400 uppercase tracking-wider">
                          Name
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-slate-400 uppercase tracking-wider">
                          Email
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-slate-400 uppercase tracking-wider">
                          Phone
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-slate-400 uppercase tracking-wider">
                          Type
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-slate-400 uppercase tracking-wider">
                          Wallet Balance
                        </th>
                      </tr>
                    </thead>
                    <tbody className="bg-white dark:bg-slate-900 divide-y divide-gray-200 dark:divide-slate-800">
                      {customers.map((customer) => (
                        <tr
                          key={customer.id}
                          className="hover:bg-gray-50 dark:hover:bg-slate-800/50 cursor-pointer transition-colors"
                        >
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="text-sm font-medium text-gray-900 dark:text-white">
                              {customer.name}
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="text-sm text-gray-500 dark:text-slate-400">
                              {customer.email}
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="text-sm text-gray-500 dark:text-slate-400">
                              {customer.phone || "N/A"}
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-cyan-100 text-cyan-800 dark:bg-cyan-900/30 dark:text-cyan-400">
                              {customer.type}
                            </span>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="text-sm text-gray-900 dark:text-white">
                              {customer.walletBalance?.toFixed(2)} EGP
                            </div>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>

                {/* Pagination */}
                {totalPages > 1 && (
                  <div className="bg-white dark:bg-slate-900 px-4 py-3 flex items-center justify-between border-t border-gray-200 dark:border-slate-800 sm:px-6">
                    <div className="flex-1 flex justify-between sm:hidden">
                      <Button
                        onClick={() =>
                          setPageNumber(Math.max(1, pageNumber - 1))
                        }
                        disabled={pageNumber === 1}
                        variant="outline"
                        size="sm"
                      >
                        Previous
                      </Button>
                      <Button
                        onClick={() =>
                          setPageNumber(Math.min(totalPages, pageNumber + 1))
                        }
                        disabled={pageNumber === totalPages}
                        variant="outline"
                        size="sm"
                      >
                        Next
                      </Button>
                    </div>
                    <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
                      <div>
                        <p className="text-sm text-gray-700 dark:text-slate-400">
                          Page <span className="font-medium">{pageNumber}</span>{" "}
                          of <span className="font-medium">{totalPages}</span>
                        </p>
                      </div>
                      <div className="flex gap-2">
                        <Button
                          onClick={() =>
                            setPageNumber(Math.max(1, pageNumber - 1))
                          }
                          disabled={pageNumber === 1}
                          variant="outline"
                          size="sm"
                        >
                          Previous
                        </Button>
                        <Button
                          onClick={() =>
                            setPageNumber(Math.min(totalPages, pageNumber + 1))
                          }
                          disabled={pageNumber === totalPages}
                          variant="outline"
                          size="sm"
                        >
                          Next
                        </Button>
                      </div>
                    </div>
                  </div>
                )}
              </>
            )}
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
