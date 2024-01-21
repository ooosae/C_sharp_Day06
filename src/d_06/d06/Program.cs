using System;
using System.Collections.Generic;

namespace d06;
using Microsoft.Extensions.Configuration;

class Program
{
    static void ProcessCustomers(bool useLeastCustomers)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "../../../appsettings.json");

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(path, optional: false, reloadOnChange: true)
            .Build();

        TimeSpan itemProcessingTime = TimeSpan.Parse(configuration.GetSection("CashRegisterConfig")["ItemProcessingTime"]);
        TimeSpan switchCustomerTime = TimeSpan.Parse(configuration.GetSection("CashRegisterConfig")["SwitchCustomerTime"]);

        var myStorage = new Storage(50);
        int numberOfCashRegisters = 4;

        Store myStore = new Store(numberOfCashRegisters, myStorage, itemProcessingTime, switchCustomerTime);

        var customers = new List<Customer>
        {
            new Customer("Anna", 1, 7),
            new Customer("Pavel", 2, 7),
            new Customer("Viktor", 3, 7),
            new Customer("Andrew", 4, 7),
            new Customer("Josh", 5, 7),
            new Customer("Tina", 6, 7),
            new Customer("Katya", 7, 7),
            new Customer("Max", 8, 7),
            new Customer("Bob", 9, 7),
            new Customer("Rose", 10, 7),
            new Customer("Petya", 11, 7),
            new Customer("Leo", 12, 7),
            new Customer("Jimmy", 13, 7),
            new Customer("Harry", 14, 7),
            new Customer("Josef", 15, 7),
            new Customer("Sergey", 16, 7),
            new Customer("Vilatiy", 17, 7),
            new Customer("Kostya", 18, 7),
            new Customer("Sonya", 19, 7),
            new Customer("Tanya", 20, 7)
        };

        myStore.Open(customers, useLeastCustomers);
        
    }

    static void Main()
    {
        Console.WriteLine("Method 1: Least Customers:\n");
        ProcessCustomers(true);
        Console.WriteLine("\n\nMethod 2: Least Items:\n");
        ProcessCustomers(false);
    }
    }