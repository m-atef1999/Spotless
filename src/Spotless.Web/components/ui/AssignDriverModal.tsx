import React, { useState } from "react";
import { Search, User, Truck } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import { Modal } from "./Modal";
import { Input } from "./Input";
import { Button } from "./Button";
import { DriversService } from "../../lib/api";
import { useToast } from "./Toast";

interface AssignDriverModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSelect: (driverId: string) => void;
}

export const AssignDriverModal: React.FC<AssignDriverModalProps> = ({
  isOpen,
  onClose,
  onSelect,
}) => {
  const [searchTerm, setSearchTerm] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 10;

  const { addToast } = useToast();

  const {
    data: drivers = [],
    isLoading,
    refetch,
  } = useQuery<any[], Error>({
    queryKey: ["drivers", pageNumber, searchTerm],
    queryFn: async () => {
      try {
        const res = await DriversService.getApiDrivers({
          pageNumber,
          pageSize,
          searchTerm: searchTerm || undefined,
        });
        return (res.data as any[]) || [];
      } catch (err) {
        console.error("Failed to load drivers for assign modal", err);
        addToast("Failed to load drivers. Server error.", "error");
        return [];
      }
    },
    keepPreviousData: true,
    staleTime: 10000,
  } as any);

  const handleSelect = (driverId: string) => {
    onSelect(driverId);
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Assign Driver" size="lg">
      <div className="space-y-4">
        <div className="flex gap-2 items-center">
          <div className="flex-1">
            <Input
              placeholder="Search drivers by name, phone or vehicle..."
              icon={<Search className="w-4 h-4" />}
              value={searchTerm}
              onChange={(e) => {
                setSearchTerm(e.target.value);
                setPageNumber(1);
              }}
            />
          </div>
          <Button variant="outline" onClick={() => refetch()}>
            Refresh
          </Button>
        </div>

        <div className="bg-white dark:bg-slate-900 rounded-lg border border-slate-200 dark:border-slate-800 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-800">
                  <th className="px-4 py-2 text-xs font-semibold text-slate-500">
                    Driver
                  </th>
                  <th className="px-4 py-2 text-xs font-semibold text-slate-500">
                    Phone
                  </th>
                  <th className="px-4 py-2 text-xs font-semibold text-slate-500">
                    Vehicle
                  </th>
                  <th className="px-4 py-2 text-xs font-semibold text-slate-500 text-right">
                    Action
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200 dark:divide-slate-800">
                {isLoading ? (
                  <tr>
                    <td
                      colSpan={4}
                      className="px-4 py-6 text-center text-slate-500"
                    >
                      Loading drivers...
                    </td>
                  </tr>
                ) : drivers.length === 0 ? (
                  <tr>
                    <td
                      colSpan={4}
                      className="px-4 py-6 text-center text-slate-500"
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
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-2">
                          <User className="w-4 h-4 text-slate-400" />
                          <div>
                            <div className="font-medium text-slate-900 dark:text-white">
                              {driver.name}
                            </div>
                            <div className="text-xs text-slate-500">
                              {driver.email}
                            </div>
                          </div>
                        </div>
                      </td>
                      <td className="px-4 py-3 text-sm text-slate-600 dark:text-slate-300">
                        {driver.phone}
                      </td>
                      <td className="px-4 py-3 text-sm text-slate-600 dark:text-slate-300">
                        <div className="flex items-center gap-2">
                          <Truck className="w-4 h-4 text-slate-400" />
                          <span>{driver.vehicleInfo}</span>
                        </div>
                      </td>
                      <td className="px-4 py-3 text-right">
                        <Button
                          size="sm"
                          onClick={() => handleSelect(driver.id)}
                        >
                          Assign
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
            disabled={isLoading || (drivers && drivers.length < pageSize)}
          >
            Next
          </Button>
        </div>
      </div>
    </Modal>
  );
};

export default AssignDriverModal;
