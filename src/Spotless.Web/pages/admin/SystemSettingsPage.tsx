import React, { useState, useEffect } from 'react';
import { SystemSettingsService, type SystemSettingDto } from '../../lib/api';

interface SystemSettings {
    general: {
        siteName: string;
        supportEmail: string;
        supportPhone: string;
        maintenanceMode: boolean;
    };
    business: {
        taxRate: number;
        serviceFee: number;
        currency: string;
        timezone: string;
    };
    notifications: {
        emailNotifications: boolean;
        smsNotifications: boolean;
        pushNotifications: boolean;
    };
    payment: {
        stripeEnabled: boolean;
        paypalEnabled: boolean;
        cashOnDelivery: boolean;
    };
}

const SystemSettingsPage: React.FC = () => {
    const [settings, setSettings] = useState<SystemSettings>({
        general: { siteName: '', supportEmail: '', supportPhone: '', maintenanceMode: false },
        business: { taxRate: 0, serviceFee: 0, currency: 'USD', timezone: 'America/New_York' },
        notifications: { emailNotifications: false, smsNotifications: false, pushNotifications: false },
        payment: { stripeEnabled: false, paypalEnabled: false, cashOnDelivery: false }
    });
    const [settingsMap, setSettingsMap] = useState<Map<string, SystemSettingDto>>(new Map());
    const [loading, setLoading] = useState(false);
    const [saveSuccess, setSaveSuccess] = useState(false);

    useEffect(() => {
        fetchSettings();
    }, []);

    const fetchSettings = async () => {
        setLoading(true);
        try {
            const settingsArray: SystemSettingDto[] = await SystemSettingsService.getApiSettings({ category: undefined });

            const map = new Map<string, SystemSettingDto>();
            const grouped: SystemSettings = {
                general: { siteName: '', supportEmail: '', supportPhone: '', maintenanceMode: false },
                business: { taxRate: 0, serviceFee: 0, currency: 'USD', timezone: 'America/New_York' },
                notifications: { emailNotifications: false, smsNotifications: false, pushNotifications: false },
                payment: { stripeEnabled: false, paypalEnabled: false, cashOnDelivery: false }
            };

            settingsArray.forEach(setting => {
                map.set(setting.key || '', setting);
                const category = (setting.category?.toLowerCase() || 'general') as keyof SystemSettings;
                const key = setting.key || '';
                const value = setting.value;

                if (grouped[category] && key in grouped[category]) {
                    const categorySettings = grouped[category] as Record<string, string | number | boolean>;
                    if (typeof categorySettings[key] === 'boolean') {
                        categorySettings[key] = value === 'true';
                    } else if (typeof categorySettings[key] === 'number') {
                        categorySettings[key] = parseFloat(value || '0');
                    } else {
                        categorySettings[key] = value || '';
                    }
                }
            });

            setSettingsMap(map);
            setSettings(grouped);
        } catch (error) {
            console.error('Failed to fetch settings:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleSave = async () => {
        setLoading(true);
        setSaveSuccess(false);
        try {
            const updates: Promise<void>[] = [];

            Object.entries(settings).forEach(([, categorySettings]) => {
                Object.entries(categorySettings).forEach(([key, value]) => {
                    const setting = settingsMap.get(key);
                    if (setting?.id) {
                        updates.push(
                            SystemSettingsService.putApiSettings({
                                id: setting.id,
                                requestBody: { value: String(value) }
                            })
                        );
                    }
                });
            });

            await Promise.all(updates);
            setSaveSuccess(true);
            setTimeout(() => setSaveSuccess(false), 3000);
        } catch (error) {
            console.error('Failed to save settings:', error);
        } finally {
            setLoading(false);
        }
    };

    const updateGeneralSetting = (key: keyof SystemSettings['general'], value: string | boolean) => {
        setSettings(prev => ({
            ...prev,
            general: { ...prev.general, [key]: value }
        }));
    };

    const updateBusinessSetting = (key: keyof SystemSettings['business'], value: string | number) => {
        setSettings(prev => ({
            ...prev,
            business: { ...prev.business, [key]: value }
        }));
    };

    const updateNotificationSetting = (key: keyof SystemSettings['notifications'], value: boolean) => {
        setSettings(prev => ({
            ...prev,
            notifications: { ...prev.notifications, [key]: value }
        }));
    };

    const updatePaymentSetting = (key: keyof SystemSettings['payment'], value: boolean) => {
        setSettings(prev => ({
            ...prev,
            payment: { ...prev.payment, [key]: value }
        }));
    };

    return (
        <div className="p-6 max-w-4xl mx-auto">
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-3xl font-bold text-gray-900">System Settings</h1>
                <button
                    onClick={handleSave}
                    disabled={loading}
                    className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 disabled:bg-gray-400 transition-colors"
                >
                    {loading ? 'Saving...' : 'Save Changes'}
                </button>
            </div>

            {saveSuccess && (
                <div className="mb-6 bg-green-50 border border-green-200 rounded-lg p-4">
                    <p className="text-green-800">Settings saved successfully!</p>
                </div>
            )}

            <div className="space-y-6">
                {/* General Settings */}
                <div className="bg-white rounded-lg shadow p-6">
                    <h2 className="text-xl font-semibold text-gray-900 mb-4">General Settings</h2>
                    <div className="space-y-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Site Name</label>
                            <input
                                type="text"
                                value={settings.general.siteName}
                                onChange={(e) => updateGeneralSetting('siteName', e.target.value)}
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Support Email</label>
                            <input
                                type="email"
                                value={settings.general.supportEmail}
                                onChange={(e) => updateGeneralSetting('supportEmail', e.target.value)}
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Support Phone</label>
                            <input
                                type="tel"
                                value={settings.general.supportPhone}
                                onChange={(e) => updateGeneralSetting('supportPhone', e.target.value)}
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                            />
                        </div>
                        <div className="flex items-center">
                            <input
                                type="checkbox"
                                id="maintenanceMode"
                                checked={settings.general.maintenanceMode}
                                onChange={(e) => updateGeneralSetting('maintenanceMode', e.target.checked)}
                                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                            />
                            <label htmlFor="maintenanceMode" className="ml-2 block text-sm text-gray-900">
                                Enable Maintenance Mode
                            </label>
                        </div>
                    </div>
                </div>

                {/* Business Settings */}
                <div className="bg-white rounded-lg shadow p-6">
                    <h2 className="text-xl font-semibold text-gray-900 mb-4">Business Settings</h2>
                    <div className="space-y-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Tax Rate (%)</label>
                            <input
                                type="number"
                                step="0.1"
                                value={settings.business.taxRate}
                                onChange={(e) => updateBusinessSetting('taxRate', parseFloat(e.target.value))}
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Service Fee (%)</label>
                            <input
                                type="number"
                                step="0.1"
                                value={settings.business.serviceFee}
                                onChange={(e) => updateBusinessSetting('serviceFee', parseFloat(e.target.value))}
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Currency</label>
                            <select
                                value={settings.business.currency}
                                onChange={(e) => updateBusinessSetting('currency', e.target.value)}
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                            >
                                <option value="USD">USD - US Dollar</option>
                                <option value="EUR">EUR - Euro</option>
                                <option value="GBP">GBP - British Pound</option>
                                <option value="CAD">CAD - Canadian Dollar</option>
                            </select>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Timezone</label>
                            <select
                                value={settings.business.timezone}
                                onChange={(e) => updateBusinessSetting('timezone', e.target.value)}
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                            >
                                <option value="America/New_York">Eastern Time</option>
                                <option value="America/Chicago">Central Time</option>
                                <option value="America/Denver">Mountain Time</option>
                                <option value="America/Los_Angeles">Pacific Time</option>
                            </select>
                        </div>
                    </div>
                </div>

                {/* Notification Settings */}
                <div className="bg-white rounded-lg shadow p-6">
                    <h2 className="text-xl font-semibold text-gray-900 mb-4">Notification Settings</h2>
                    <div className="space-y-3">
                        <div className="flex items-center">
                            <input
                                type="checkbox"
                                id="emailNotifications"
                                checked={settings.notifications.emailNotifications}
                                onChange={(e) => updateNotificationSetting('emailNotifications', e.target.checked)}
                                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                            />
                            <label htmlFor="emailNotifications" className="ml-2 block text-sm text-gray-900">
                                Enable Email Notifications
                            </label>
                        </div>
                        <div className="flex items-center">
                            <input
                                type="checkbox"
                                id="smsNotifications"
                                checked={settings.notifications.smsNotifications}
                                onChange={(e) => updateNotificationSetting('smsNotifications', e.target.checked)}
                                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                            />
                            <label htmlFor="smsNotifications" className="ml-2 block text-sm text-gray-900">
                                Enable SMS Notifications
                            </label>
                        </div>
                        <div className="flex items-center">
                            <input
                                type="checkbox"
                                id="pushNotifications"
                                checked={settings.notifications.pushNotifications}
                                onChange={(e) => updateNotificationSetting('pushNotifications', e.target.checked)}
                                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                            />
                            <label htmlFor="pushNotifications" className="ml-2 block text-sm text-gray-900">
                                Enable Push Notifications
                            </label>
                        </div>
                    </div>
                </div>

                {/* Payment Settings */}
                <div className="bg-white rounded-lg shadow p-6">
                    <h2 className="text-xl font-semibold text-gray-900 mb-4">Payment Settings</h2>
                    <div className="space-y-3">
                        <div className="flex items-center">
                            <input
                                type="checkbox"
                                id="stripeEnabled"
                                checked={settings.payment.stripeEnabled}
                                onChange={(e) => updatePaymentSetting('stripeEnabled', e.target.checked)}
                                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                            />
                            <label htmlFor="stripeEnabled" className="ml-2 block text-sm text-gray-900">
                                Enable Stripe Payments
                            </label>
                        </div>
                        <div className="flex items-center">
                            <input
                                type="checkbox"
                                id="paypalEnabled"
                                checked={settings.payment.paypalEnabled}
                                onChange={(e) => updatePaymentSetting('paypalEnabled', e.target.checked)}
                                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                            />
                            <label htmlFor="paypalEnabled" className="ml-2 block text-sm text-gray-900">
                                Enable PayPal Payments
                            </label>
                        </div>
                        <div className="flex items-center">
                            <input
                                type="checkbox"
                                id="cashOnDelivery"
                                checked={settings.payment.cashOnDelivery}
                                onChange={(e) => updatePaymentSetting('cashOnDelivery', e.target.checked)}
                                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                            />
                            <label htmlFor="cashOnDelivery" className="ml-2 block text-sm text-gray-900">
                                Enable Cash on Delivery
                            </label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default SystemSettingsPage;
