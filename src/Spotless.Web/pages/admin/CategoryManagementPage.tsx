import { useState, useEffect } from 'react';
import { CategoriesService, type CategoryDto, type PagedResponse } from '../../lib/api';
import { useToast } from '../../components/ui/Toast';
import { Modal } from '../../components/ui/Modal';
import { DashboardLayout } from '../../layouts/DashboardLayout';

export function CategoryManagementPage() {
    const [categories, setCategories] = useState<CategoryDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [editingCategory, setEditingCategory] = useState<CategoryDto | null>(null);
    const [formData, setFormData] = useState({ name: '', description: '' });
    const { addToast } = useToast();

    useEffect(() => {
        fetchCategories();
    }, []);

    const fetchCategories = async () => {
        try {
            setLoading(true);
            const response: PagedResponse = await CategoriesService.getApiCategories({
                pageNumber: 1,
                pageSize: 100
            });
            setCategories((response.data as CategoryDto[]) || []);
        } catch (error) {
            addToast('Failed to load categories', 'error');
            console.error('Error fetching categories:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleCreate = () => {
        setEditingCategory(null);
        setFormData({ name: '', description: '' });
        setShowModal(true);
    };

    const handleEdit = (category: CategoryDto) => {
        setEditingCategory(category);
        setFormData({ name: category.name || '', description: category.description || '' });
        setShowModal(true);
    };

    const handleSubmit = async () => {
        try {
            if (editingCategory) {
                await CategoriesService.putApiCategories({
                    id: editingCategory.id!,
                    requestBody: {
                        name: formData.name,
                        description: formData.description,
                        price: 0 // Default price as it's not in the form yet, or add to form if needed
                    }
                });
                addToast('Category updated successfully', 'success');
            } else {
                await CategoriesService.postApiCategories({
                    requestBody: {
                        name: formData.name,
                        description: formData.description,
                        price: 0 // Default price
                    }
                });
                addToast('Category created successfully', 'success');
            }
            setShowModal(false);
            fetchCategories();
        } catch (error) {
            addToast('Failed to save category', 'error');
            console.error('Error saving category:', error);
        }
    };

    const handleDelete = async (id: string) => {
        if (!confirm('Are you sure you want to delete this category?')) return;

        try {
            await CategoriesService.deleteApiCategories({ id });
            addToast('Category deleted successfully', 'success');
            fetchCategories();
        } catch (error) {
            addToast('Failed to delete category', 'error');
            console.error('Error deleting category:', error);
        }
    };

    return (
        <DashboardLayout role="Admin">
            <div className="space-y-8">
                {/* Header */}
                <div className="flex justify-between items-center">
                    <div>
                        <h1 className="text-2xl font-bold text-slate-900 dark:text-white">Category Management</h1>
                        <p className="text-slate-500 dark:text-slate-400 mt-1">Manage service categories</p>
                    </div>
                    <button
                        onClick={handleCreate}
                        className="bg-cyan-600 text-white px-6 py-2 rounded-xl hover:bg-cyan-700 transition-colors font-medium"
                    >
                        + Add Category
                    </button>
                </div>

                {/* Categories Grid */}
                <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden shadow-sm">
                    {loading ? (
                        <div className="flex items-center justify-center h-64">
                            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-cyan-600"></div>
                        </div>
                    ) : categories.length === 0 ? (
                        <div className="text-center py-12">
                            <p className="text-slate-500 text-lg">No categories found</p>
                        </div>
                    ) : (
                        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 p-6">
                            {categories.map((category) => (
                                <div key={category.id} className="border border-slate-200 dark:border-slate-800 rounded-xl p-6 hover:shadow-md transition-shadow bg-slate-50 dark:bg-slate-800/50">
                                    <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-2">{category.name}</h3>
                                    <p className="text-slate-600 dark:text-slate-400 text-sm mb-4 line-clamp-2">{category.description || 'No description'}</p>
                                    <div className="text-sm text-slate-500 dark:text-slate-500 mb-4">
                                        Services: {category.serviceCount || 0}
                                    </div>
                                    <div className="flex gap-2">
                                        <button
                                            onClick={() => handleEdit(category)}
                                            className="flex-1 bg-cyan-50 dark:bg-cyan-900/20 text-cyan-700 dark:text-cyan-400 px-4 py-2 rounded-lg hover:bg-cyan-100 dark:hover:bg-cyan-900/30 transition-colors font-medium text-sm"
                                        >
                                            Edit
                                        </button>
                                        <button
                                            onClick={() => handleDelete(category.id!)}
                                            className="flex-1 bg-red-50 dark:bg-red-900/20 text-red-700 dark:text-red-400 px-4 py-2 rounded-lg hover:bg-red-100 dark:hover:bg-red-900/30 transition-colors font-medium text-sm"
                                        >
                                            Delete
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>

                {/* Modal */}
                {
                    showModal && (
                        <Modal isOpen={showModal} onClose={() => setShowModal(false)}>
                            <div className="p-6 bg-white dark:bg-slate-900">
                                <h2 className="text-2xl font-bold mb-4 text-slate-900 dark:text-white">
                                    {editingCategory ? 'Edit Category' : 'Create Category'}
                                </h2>
                                <div className="space-y-4">
                                    <div>
                                        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                                            Category Name
                                        </label>
                                        <input
                                            type="text"
                                            value={formData.name}
                                            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                                            className="w-full px-4 py-2 bg-white dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-lg focus:ring-2 focus:ring-cyan-500 outline-none text-slate-900 dark:text-white"
                                            placeholder="Enter category name"
                                        />
                                    </div>
                                    <div>
                                        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                                            Description
                                        </label>
                                        <textarea
                                            value={formData.description}
                                            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                                            className="w-full px-4 py-2 bg-white dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-lg focus:ring-2 focus:ring-cyan-500 outline-none text-slate-900 dark:text-white"
                                            rows={4}
                                            placeholder="Enter category description"
                                        />
                                    </div>
                                    <div className="flex gap-2 pt-4">
                                        <button
                                            onClick={handleSubmit}
                                            className="flex-1 bg-cyan-600 text-white px-4 py-2 rounded-lg hover:bg-cyan-700 transition-colors font-medium"
                                        >
                                            {editingCategory ? 'Update' : 'Create'}
                                        </button>
                                        <button
                                            onClick={() => setShowModal(false)}
                                            className="flex-1 bg-slate-100 dark:bg-slate-800 text-slate-700 dark:text-slate-300 px-4 py-2 rounded-lg hover:bg-slate-200 dark:hover:bg-slate-700 transition-colors font-medium"
                                        >
                                            Cancel
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </Modal>
                    )
                }
            </div >
        </DashboardLayout>
    );
}

