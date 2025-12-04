import { useState, useEffect } from 'react';
import { CategoriesService, type CategoryDto, type PagedResponse } from '../../lib/api';
import { useToast } from '../../components/ui/Toast';
import { Modal } from '../../components/ui/Modal';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Image, Folder, Package, Upload } from 'lucide-react';

interface CategoryFormData {
    name: string;
    description: string;
    imageUrl: string;
    imageData: string;
}

export function CategoryManagementPage() {
    const [categories, setCategories] = useState<CategoryDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [editingCategory, setEditingCategory] = useState<CategoryDto | null>(null);
    const [formData, setFormData] = useState<CategoryFormData>({ name: '', description: '', imageUrl: '', imageData: '' });
    const [imageInputMode, setImageInputMode] = useState<'url' | 'upload'>('url');
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
        setFormData({ name: '', description: '', imageUrl: '', imageData: '' });
        setImageInputMode('url');
        setShowModal(true);
    };

    const handleEdit = (category: CategoryDto) => {
        setEditingCategory(category);
        setFormData({
            name: category.name || '',
            description: category.description || '',
            imageUrl: category.imageUrl || '',
            imageData: (category as any).imageData || ''
        });
        setImageInputMode((category as any).imageData ? 'upload' : 'url');
        setShowModal(true);
    };

    const handleSubmit = async () => {
        if (!formData.name.trim()) {
            addToast('Category name is required', 'error');
            return;
        }
        try {
            if (editingCategory) {
                await CategoriesService.putApiCategories({
                    id: editingCategory.id!,
                    requestBody: {
                        name: formData.name,
                        description: formData.description,
                        imageUrl: formData.imageUrl || undefined,
                        imageData: formData.imageData || undefined,
                        price: 0
                    }
                });
                addToast('Category updated successfully', 'success');
            } else {
                await CategoriesService.postApiCategories({
                    requestBody: {
                        name: formData.name,
                        description: formData.description,
                        imageUrl: formData.imageUrl || undefined,
                        imageData: formData.imageData || undefined,
                        price: 0
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
                                <div key={category.id} className="border border-slate-200 dark:border-slate-800 rounded-xl overflow-hidden hover:shadow-md transition-shadow bg-slate-50 dark:bg-slate-800/50">
                                    {/* Category Image */}
                                    {category.imageUrl ? (
                                        <div className="h-32 bg-slate-100 dark:bg-slate-800">
                                            <img
                                                src={category.imageUrl}
                                                alt={category.name || ''}
                                                className="w-full h-full object-cover"
                                                onError={(e) => (e.target as HTMLImageElement).style.display = 'none'}
                                            />
                                        </div>
                                    ) : (
                                        <div className="h-32 bg-gradient-to-br from-cyan-50 to-slate-100 dark:from-cyan-900/20 dark:to-slate-800 flex items-center justify-center">
                                            <Folder className="w-12 h-12 text-cyan-400/60" />
                                        </div>
                                    )}

                                    <div className="p-6">
                                        <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-2">{category.name}</h3>
                                        <p className="text-slate-600 dark:text-slate-400 text-sm mb-4 line-clamp-2">
                                            {category.description || 'No description available'}
                                        </p>
                                        <div className="flex items-center gap-2 text-sm text-slate-500 dark:text-slate-500 mb-4">
                                            <Package className="w-4 h-4" />
                                            <span>{category.serviceCount || 0} services</span>
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
                                </div>
                            ))}
                        </div>
                    )}
                </div>

                {/* Modal */}
                <Modal
                    isOpen={showModal}
                    onClose={() => setShowModal(false)}
                    title={editingCategory ? 'Edit Category' : 'Create Category'}
                    size="lg"
                >
                    <div className="space-y-4">
                        <div>
                            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                                Category Name *
                            </label>
                            <input
                                type="text"
                                value={formData.name}
                                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                                className="w-full px-4 py-2 bg-white dark:bg-slate-800 border border-slate-300 dark:border-slate-700 rounded-lg focus:ring-2 focus:ring-cyan-500 outline-none text-slate-900 dark:text-white"
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
                                className="w-full px-4 py-2 bg-white dark:bg-slate-800 border border-slate-300 dark:border-slate-700 rounded-lg focus:ring-2 focus:ring-cyan-500 outline-none text-slate-900 dark:text-white"
                                rows={3}
                                placeholder="Enter category description"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                                <Image className="w-4 h-4 inline mr-1" />
                                Image
                            </label>
                            {/* Image Input Mode Toggle */}
                            <div className="flex gap-2 mb-3">
                                <button
                                    type="button"
                                    onClick={() => {
                                        setImageInputMode('url');
                                        setFormData({ ...formData, imageData: '' });
                                    }}
                                    className={`flex-1 py-2 px-3 text-sm rounded-lg border transition-colors ${imageInputMode === 'url'
                                            ? 'bg-cyan-50 border-cyan-500 text-cyan-700 dark:bg-cyan-900/30 dark:text-cyan-400'
                                            : 'border-slate-300 dark:border-slate-700 text-slate-600 dark:text-slate-400 hover:border-slate-400'
                                        }`}
                                >
                                    <Image className="w-4 h-4 inline mr-1" />
                                    URL
                                </button>
                                <button
                                    type="button"
                                    onClick={() => {
                                        setImageInputMode('upload');
                                        setFormData({ ...formData, imageUrl: '' });
                                    }}
                                    className={`flex-1 py-2 px-3 text-sm rounded-lg border transition-colors ${imageInputMode === 'upload'
                                            ? 'bg-cyan-50 border-cyan-500 text-cyan-700 dark:bg-cyan-900/30 dark:text-cyan-400'
                                            : 'border-slate-300 dark:border-slate-700 text-slate-600 dark:text-slate-400 hover:border-slate-400'
                                        }`}
                                >
                                    <Upload className="w-4 h-4 inline mr-1" />
                                    Upload
                                </button>
                            </div>

                            {imageInputMode === 'url' ? (
                                <>
                                    <input
                                        type="url"
                                        value={formData.imageUrl}
                                        onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
                                        className="w-full px-4 py-2 bg-white dark:bg-slate-800 border border-slate-300 dark:border-slate-700 rounded-lg focus:ring-2 focus:ring-cyan-500 outline-none text-slate-900 dark:text-white"
                                        placeholder="https://example.com/image.jpg"
                                    />
                                    {formData.imageUrl && (
                                        <div className="mt-2 rounded-lg overflow-hidden h-24 w-full max-w-xs bg-slate-100 dark:bg-slate-800">
                                            <img
                                                src={formData.imageUrl}
                                                alt="Preview"
                                                className="w-full h-full object-cover"
                                                onError={(e) => (e.target as HTMLImageElement).style.display = 'none'}
                                            />
                                        </div>
                                    )}
                                </>
                            ) : (
                                <>
                                    <input
                                        type="file"
                                        accept="image/*"
                                        onChange={(e) => {
                                            const file = e.target.files?.[0];
                                            if (file) {
                                                const reader = new FileReader();
                                                reader.onloadend = () => {
                                                    const base64 = reader.result as string;
                                                    setFormData({ ...formData, imageData: base64 });
                                                };
                                                reader.readAsDataURL(file);
                                            }
                                        }}
                                        className="w-full px-4 py-2 bg-white dark:bg-slate-800 border border-slate-300 dark:border-slate-700 rounded-lg focus:ring-2 focus:ring-cyan-500 outline-none text-slate-900 dark:text-white file:mr-4 file:py-1 file:px-4 file:rounded-md file:border-0 file:text-sm file:font-medium file:bg-cyan-50 file:text-cyan-700 dark:file:bg-cyan-900/30 dark:file:text-cyan-400 hover:file:bg-cyan-100"
                                    />
                                    {formData.imageData && (
                                        <div className="mt-2 rounded-lg overflow-hidden h-24 w-full max-w-xs bg-slate-100 dark:bg-slate-800">
                                            <img src={formData.imageData} alt="Preview" className="w-full h-full object-cover" />
                                        </div>
                                    )}
                                </>
                            )}
                        </div>
                        <div className="flex gap-2 pt-4 border-t border-slate-200 dark:border-slate-700">
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
                </Modal>
            </div>
        </DashboardLayout>
    );
}
