import { useState, useEffect } from 'react';
import { CategoriesService, type CategoryDto, type PagedResponse } from '../../lib/api';
import { useToast } from '../../components/ui/Toast';
import { Modal } from '../../components/ui/Modal';

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
        <div className="min-h-screen bg-gray-50 p-6">
            <div className="max-w-7xl mx-auto">
                {/* Header */}
                <div className="mb-8 flex justify-between items-center">
                    <div>
                        <h1 className="text-3xl font-bold text-gray-900">Category Management</h1>
                        <p className="text-gray-600 mt-2">Manage service categories</p>
                    </div>
                    <button
                        onClick={handleCreate}
                        className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors"
                    >
                        + Add Category
                    </button>
                </div>

                {/* Categories Grid */}
                <div className="bg-white rounded-lg shadow-sm overflow-hidden">
                    {loading ? (
                        <div className="flex items-center justify-center h-64">
                            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                        </div>
                    ) : categories.length === 0 ? (
                        <div className="text-center py-12">
                            <p className="text-gray-500 text-lg">No categories found</p>
                        </div>
                    ) : (
                        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 p-6">
                            {categories.map((category) => (
                                <div key={category.id} className="border border-gray-200 rounded-lg p-6 hover:shadow-md transition-shadow">
                                    <h3 className="text-lg font-semibold text-gray-900 mb-2">{category.name}</h3>
                                    <p className="text-gray-600 text-sm mb-4">{category.description || 'No description'}</p>
                                    <div className="text-sm text-gray-500 mb-4">
                                        Services: {category.serviceCount || 0}
                                    </div>
                                    <div className="flex gap-2">
                                        <button
                                            onClick={() => handleEdit(category)}
                                            className="flex-1 bg-blue-100 text-blue-700 px-4 py-2 rounded-lg hover:bg-blue-200 transition-colors"
                                        >
                                            Edit
                                        </button>
                                        <button
                                            onClick={() => handleDelete(category.id!)}
                                            className="flex-1 bg-red-100 text-red-700 px-4 py-2 rounded-lg hover:bg-red-200 transition-colors"
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
                            <div className="p-6">
                                <h2 className="text-2xl font-bold mb-4">
                                    {editingCategory ? 'Edit Category' : 'Create Category'}
                                </h2>
                                <div className="space-y-4">
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Category Name
                                        </label>
                                        <input
                                            type="text"
                                            value={formData.name}
                                            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                            placeholder="Enter category name"
                                        />
                                    </div>
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Description
                                        </label>
                                        <textarea
                                            value={formData.description}
                                            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                            rows={4}
                                            placeholder="Enter category description"
                                        />
                                    </div>
                                    <div className="flex gap-2 pt-4">
                                        <button
                                            onClick={handleSubmit}
                                            className="flex-1 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
                                        >
                                            {editingCategory ? 'Update' : 'Create'}
                                        </button>
                                        <button
                                            onClick={() => setShowModal(false)}
                                            className="flex-1 bg-gray-200 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-300 transition-colors"
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
        </div >
    );
}

