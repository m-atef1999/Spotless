using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DryCleaningProject.Models
{
    public enum OrderStatus
    {
        PendingPayment,
        AcceptedByDriver,
        PickedUp,
        InProcessing,
        ReadyForDelivery,
        Delivered,
        Cancelled
    }

    public enum PaymentMethod
    {
        CreditCard,
        WalletBalance,
        CashOnDelivery,
        PayPal
    }

    public enum PaymentStatus
    {
        Pending,
        Accepted,
        Denied
    }

    public class Service
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PricePerUnit { get; set; }
    }


    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal WalletBalance { get; set; } = 0.00m;
    }

    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public bool IsWalletDeduction { get; set; }
    }


    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int? DriverId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? PickUpDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string PickUpAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public List<Service> services { get; set; } = new List<Service>();
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;
        [JsonIgnore]
        public bool IsReadyForDriverAcceptance => Status == OrderStatus.PendingPayment;
    }
    
    public class Driver
    {
        public int DriverId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string VehicleDetails { get; set; }
        public bool IsAvailable { get; set; } = true;
    }

    public class Admin
    {
        public int AdminId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}

namespace DryCleaningProject.Data
{
    public interface IDbManager
    {
        List<Models.Customer> GetCustomers();
        void AddCustomer(Models.Customer customer);
        void UpdateCustomer(Models.Customer customer);
        void DeleteCustomer(int customerId);

        List<Models.Service> GetServices();

        List<Models.Order> GetOrders();
        void AddOrder(Models.Order order);
        void UpdateOrder(Models.Order order);
        void DeleteOrder(int orderId);

        List<Models.Payment> GetPayments();
        void AddPayment(Models.Payment payment);
        void UpdatePayment(Models.Payment payment);

        List<Models.Driver> GetDrivers();
        void AddDriver(Models.Driver driver);
        void UpdateDriver(Models.Driver driver);

        List<Models.Admin> GetAdmins();
        void AddAdmin(Models.Admin admin);

        Models.Service GetMostOrderedService();
        List<Models.Order> GetOrdersByCustomerId(int customerId);
        List<Models.Order> GetOrdersByStatus(Models.OrderStatus status);
        decimal GetTotalRevenue();
        Models.Driver GetDriverByEmail(string email);
        Models.Admin GetAdminByEmail(string email);
    }
    
    public class JsonDbManager : IDbManager
    {
        private readonly string CustomerFilePath = "customers.json";
        private readonly string ServiceFilePath = "services.json";
        private readonly string OrderFilePath = "orders.json";
        private readonly string PaymentFilePath = "payments.json";
        private readonly string DriverFilePath = "drivers.json";
        private readonly string AdminFilePath = "admins.json";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };
        private void SaveData<T>(List<T> data, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(data, JsonOptions);
            File.WriteAllText(filePath, jsonString);
        }

        private List<T> LoadData<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                SaveData(new List<T>(), filePath);
                return new List<T>();
            }

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(jsonString, JsonOptions) ?? new List<T>();
        }

        public List<Models.Customer> GetCustomers() => LoadData<Models.Customer>(CustomerFilePath);
        public void AddCustomer(Models.Customer customer)
        {
            var customers = GetCustomers();
            customer.CustomerId = customers.Any() ? customers.Max(c => c.CustomerId) + 1 : 1;
            customers.Add(customer);
            SaveData(customers, CustomerFilePath);
        }
        public void UpdateCustomer(Models.Customer customer)
        {
            var customers = GetCustomers();
            var existingCustomer = customers.FirstOrDefault(c => c.CustomerId == customer.CustomerId);
            if (existingCustomer != null)
            {
                customers.Remove(existingCustomer);
                customers.Add(customer);
                SaveData(customers, CustomerFilePath);
            }
        }
        public void DeleteCustomer(int customerId)
        {
            var customers = GetCustomers();
            customers.RemoveAll(c => c.CustomerId == customerId);
            SaveData(customers, CustomerFilePath);
        }

        public List<Models.Service> GetServices()
        {
            var services = LoadData<Models.Service>(ServiceFilePath);
            if (!services.Any())
            {
                services.Add(new Models.Service { ServiceId = 1, Name = "Shirt Dry Cleaning", Description = "Professional dry cleaning for shirts.", PricePerUnit = 5.50m });
                services.Add(new Models.Service { ServiceId = 2, Name = "Suit Dry Cleaning", Description = "Full dry cleaning for a suit.", PricePerUnit = 25.00m });
                services.Add(new Models.Service { ServiceId = 3, Name = "Pants Dry Cleaning", Description = "Professional dry cleaning for pants.", PricePerUnit = 10.00m });
                SaveData(services, ServiceFilePath);
            }
            return services;
        }

        public List<Models.Order> GetOrders() => LoadData<Models.Order>(OrderFilePath);
        public void AddOrder(Models.Order order)
        {
            var orders = GetOrders();
            order.OrderId = orders.Any() ? orders.Max(o => o.OrderId) + 1 : 1;
            orders.Add(order);
            SaveData(orders, OrderFilePath);
        }
        public void UpdateOrder(Models.Order order)
        {
            var orders = GetOrders();
            var existingOrder = orders.FirstOrDefault(o => o.OrderId == order.OrderId);
            if (existingOrder != null)
            {
                orders.Remove(existingOrder);
                orders.Add(order);
                SaveData(orders, OrderFilePath);
            }
        }
        public void DeleteOrder(int orderId)
        {
            var orders = GetOrders();
            orders.RemoveAll(o => o.OrderId == orderId);
            SaveData(orders, OrderFilePath);
        }

        public List<Models.Payment> GetPayments() => LoadData<Models.Payment>(PaymentFilePath);
        public void AddPayment(Models.Payment payment)
        {
            var payments = GetPayments();
            payment.PaymentId = payments.Any() ? payments.Max(p => p.PaymentId) + 1 : 1;
            payments.Add(payment);
            SaveData(payments, PaymentFilePath);
        }
        public void UpdatePayment(Models.Payment payment)
        {
            var payments = GetPayments();
            var existingPayment = payments.FirstOrDefault(p => p.PaymentId == payment.PaymentId);
            if (existingPayment != null)
            {
                payments.Remove(existingPayment);
                payments.Add(payment);
                SaveData(payments, PaymentFilePath);
            }
        }
        
        public List<Models.Driver> GetDrivers() => LoadData<Models.Driver>(DriverFilePath);
        public void AddDriver(Models.Driver driver)
        {
            var drivers = GetDrivers();
            driver.DriverId = drivers.Any() ? drivers.Max(d => d.DriverId) + 1 : 1;
            drivers.Add(driver);
            SaveData(drivers, DriverFilePath);
        }
        public void UpdateDriver(Models.Driver driver)
        {
            var drivers = GetDrivers();
            var existingDriver = drivers.FirstOrDefault(d => d.DriverId == driver.DriverId);
            if (existingDriver != null)
            {
                drivers.Remove(existingDriver);
                drivers.Add(driver);
                SaveData(drivers, DriverFilePath);
            }
        }
        
        public List<Models.Admin> GetAdmins() => LoadData<Models.Admin>(AdminFilePath);
        public void AddAdmin(Models.Admin admin)
        {
            var admins = GetAdmins();
            admin.AdminId = admins.Any() ? admins.Max(a => a.AdminId) + 1 : 1;
            admins.Add(admin);
            SaveData(admins, AdminFilePath);
        }
        
        public Models.Service GetMostOrderedService()
        {
            var orders = GetOrders(); 
            var allOrderedServices = orders.SelectMany(o => o.services);

            var mostOrdered = allOrderedServices
                .GroupBy(s => s.ServiceId)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (mostOrdered != null)
            {
                var services = GetServices();
                return services.FirstOrDefault(s => s.ServiceId == mostOrdered.Key);
            }

            return null;
        }
        
        public List<Models.Order> GetOrdersByCustomerId(int customerId)
        {
            var orders = GetOrders();
            return orders.Where(o => o.CustomerId == customerId).ToList();
        }
        
        public List<Models.Order> GetOrdersByStatus(Models.OrderStatus status)
        {
            var orders = GetOrders();
            return orders.Where(o => o.Status == status).ToList();
        }
        
        public decimal GetTotalRevenue()
        {
            var orders = GetOrders();
            return orders.Where(o => o.Status == Models.OrderStatus.Delivered).Sum(o => o.TotalPrice);
        }
        
        public Models.Driver GetDriverByEmail(string email)
        {
            var drivers = GetDrivers();
            return drivers.FirstOrDefault(d => d.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public Models.Admin GetAdminByEmail(string email)
        {
            var admins = GetAdmins();
            return admins.FirstOrDefault(a => a.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
    }
}

namespace DryCleaningProject
{
    public static class Program
    {
        private static readonly Data.IDbManager dbManager = new Data.JsonDbManager();

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Dry Cleaning Service!");
            RunCustomerAndOrderCycle();
            
            Console.WriteLine("\n--- Demonstrating New Functions ---");
            var mostOrderedService = dbManager.GetMostOrderedService();
            if (mostOrderedService != null)
            {
                Console.WriteLine($"The most ordered service is: {mostOrderedService.Name}");
            }
            
            var pendingOrders = dbManager.GetOrdersByStatus(DryCleaningProject.Models.OrderStatus.AcceptedByDriver);
            Console.WriteLine($"\nThere are {pendingOrders.Count} orders ready to be picked up.");
            
            var totalRevenue = dbManager.GetTotalRevenue();
            Console.WriteLine($"\nTotal revenue from delivered orders is: ${totalRevenue}");
        }

        public static void RunCustomerAndOrderCycle()
        {
            Console.WriteLine("\n--- Customer Registration Cycle ---");
            Console.Write("Enter your first name: ");
            string firstName = Console.ReadLine();
            Console.Write("Enter your last name: ");
            string lastName = Console.ReadLine();
            Console.Write("Enter your email: ");
            string email = Console.ReadLine();
            Console.Write("Enter a password: ");
            string password = Console.ReadLine();
            Console.Write("Enter your phone number: ");
            string phoneNumber = Console.ReadLine();
            Console.Write("Enter your address: ");
            string address = Console.ReadLine();

            var newCustomer = new Models.Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = HashPassword(password),
                PhoneNumber = phoneNumber,
                Address = address,
                WalletBalance = 50.00m
            };

            dbManager.AddCustomer(newCustomer);
            Console.WriteLine($"\nCustomer registered successfully! Your Customer ID is: {newCustomer.CustomerId}");

            Console.WriteLine("\n--- Placing a New Order ---");
            var services = dbManager.GetServices();
            var newOrder = new Models.Order
            {
                CustomerId = newCustomer.CustomerId,
                PickUpAddress = newCustomer.Address,
                DeliveryAddress = newCustomer.Address
            };

            Console.WriteLine("Available Services:");
            foreach (var service in services)
            {
                Console.WriteLine($"[{service.ServiceId}] {service.Name} - {service.Description} - ${service.PricePerUnit}");
            }

            Console.Write("Enter the Service IDs you want (comma-separated): ");
            var selectedIds = Console.ReadLine().Split(',').Select(int.Parse).ToList();

            foreach (var id in selectedIds)
            {
                var selectedService = services.FirstOrDefault(s => s.ServiceId == id);
                if (selectedService != null)
                {
                    newOrder.services.Add(selectedService);
                    newOrder.TotalPrice += selectedService.PricePerUnit;
                }
            }

            Console.WriteLine($"\nOrder created. Your total price is: ${newOrder.TotalPrice}");
            dbManager.AddOrder(newOrder);

            // --- Payment Cycle ---
            Console.WriteLine("\n--- Payment Process ---");
            Console.WriteLine("Choose a payment method:");
            Console.WriteLine($"[1] CreditCard\n[2] WalletBalance (Current balance: ${newCustomer.WalletBalance})\n[3] CashOnDelivery\n[4] PayPal");
            Console.Write("Enter your choice: ");
            string paymentChoice = Console.ReadLine();

            var payment = new Models.Payment
            {
                OrderId = newOrder.OrderId,
                PaymentDate = DateTime.Now,
                Amount = newOrder.TotalPrice,
                IsWalletDeduction = false,
                Status = Models.PaymentStatus.Pending
            };

            switch (paymentChoice)
            {
                case "1":
                    payment.Method = Models.PaymentMethod.CreditCard;
                    payment.Status = Models.PaymentStatus.Accepted;
                    break;
                case "2":
                    payment.Method = Models.PaymentMethod.WalletBalance;
                    if (newCustomer.WalletBalance >= newOrder.TotalPrice)
                    {
                        newCustomer.WalletBalance -= newOrder.TotalPrice;
                        payment.Status = Models.PaymentStatus.Accepted;
                        payment.IsWalletDeduction = true;
                        dbManager.UpdateCustomer(newCustomer);
                        Console.WriteLine($"Deducted ${newOrder.TotalPrice} from your wallet. New balance: ${newCustomer.WalletBalance}");
                    }
                    else
                    {
                        payment.Status = Models.PaymentStatus.Denied;
                        Console.WriteLine("Insufficient wallet balance. Payment denied.");
                    }
                    break;
                case "3":
                    payment.Method = Models.PaymentMethod.CashOnDelivery;
                    payment.Status = Models.PaymentStatus.Accepted;
                    break;
                case "4":
                    payment.Method = Models.PaymentMethod.PayPal;
                    payment.Status = Models.PaymentStatus.Accepted;
                    break;
                default:
                    Console.WriteLine("Invalid payment choice. Payment denied.");
                    payment.Status = Models.PaymentStatus.Denied;
                    break;
            }

            dbManager.AddPayment(payment);

            if (payment.Status == Models.PaymentStatus.Accepted)
            {
                newOrder.Status = Models.OrderStatus.AcceptedByDriver;
                dbManager.UpdateOrder(newOrder);
                Console.WriteLine("\nPayment accepted. Your order is now ready to be accepted by a driver.");
                Console.WriteLine($"Order Status updated to: {newOrder.Status}");
            }
            else
            {
                Console.WriteLine("Payment was denied. Order cancelled.");
                newOrder.Status = Models.OrderStatus.Cancelled;
                dbManager.UpdateOrder(newOrder);
            }

            Console.WriteLine("\n--- All Operations Complete. ---");
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
