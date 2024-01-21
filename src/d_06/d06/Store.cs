using System;
using System.Collections.Generic;
using d06;

public class Store
{
    private readonly List<CashRegister> cashRegisters;
    private readonly Storage storage;
    private bool openStatus;
    private readonly TimeSpan globalItemProcessingTime;
    private readonly TimeSpan globalSwitchCustomerTime;

    public Store(int numberOfCashRegisters, Storage storage, TimeSpan globalItemProcessingTime, TimeSpan globalSwitchCustomerTime)
    {
        openStatus = false;
        this.openStatus = false;
        this.globalItemProcessingTime = globalItemProcessingTime;
        this.globalSwitchCustomerTime = globalSwitchCustomerTime;

        if (numberOfCashRegisters <= 0)
        {
            throw new ArgumentException("The number of cash registers must be greater than zero.");
        }

        this.cashRegisters = new List<CashRegister>();
        this.storage = storage;

        for (int i = 1; i <= numberOfCashRegisters; i++)
        {
            cashRegisters.Add(new CashRegister(i.ToString(), globalItemProcessingTime, globalSwitchCustomerTime));
        }
    }

    public List<CashRegister> CashRegisters => cashRegisters;

    public Storage Storage => storage;

    public void Open(List<Customer> customers, bool useLeastCustomers)
    {
        openStatus = true;
        Console.WriteLine("The store is now open!");
        ProcessCustomers(customers, useLeastCustomers);
    }

    public void Close()
    {
        openStatus = false;
        Console.WriteLine("The store is now closed.");
        foreach (var register in CashRegisters)
        {
            Console.WriteLine($"{register}: Average wait time: {register.GetAverageWaitTime()} seconds");
        }
    }

    public bool IsOpen => openStatus;
    public List<CashRegister> GetRegisters => cashRegisters;
    
    public void ProcessCustomers(List<Customer> customers, bool useLeastCustomers)
    {
        int[] peopleCount = new int[customers.Count];
        int[] itemsCount = new int[customers.Count];
        
        Parallel.ForEach(customers.Take(10), new ParallelOptions { MaxDegreeOfParallelism = 2 }, customer =>
        {
            customer.FillCart();

            var chosenRegister = useLeastCustomers
                ? customer.ChooseCheckoutWithLeastCustomers(cashRegisters)
                : customer.ChooseCheckoutWithLeastGoods(cashRegisters);

            int chosenRegisterIndex = CashRegisters.IndexOf(chosenRegister);

            lock (chosenRegister.Customers)
            {
                peopleCount[chosenRegisterIndex]++;
                itemsCount[chosenRegisterIndex] += customer.GetNumberOfGoodsInCart;
                chosenRegister.EnqueueCustomer(customer);
            }
            
        });
        
        Task.Run(() => ProcessNewCustomers(customers, useLeastCustomers, peopleCount, itemsCount));
        
        while (IsOpen && CashRegisters.Any(register => register.Customers.Count > 0))
        {
            Parallel.ForEach(CashRegisters, register =>
            {
                if (register.Customers.Count > 0)
                {
                    Customer? customer = register.Process();

                    if (customer != null)
                    {
                        try
                        {
                            Storage.TakeGoods(customer.GetNumberOfGoodsInCart);
                        }
                        catch (ArgumentException)
                        {
                            if (Storage.GetNumberOfGoods > 0)
                            {
                                customer.SetNumberOfGoods(customer.GetNumberOfGoodsInCart - Storage.GetNumberOfGoods);
                                Storage.TakeGoods(Storage.GetNumberOfGoods);
                            }

                            Console.WriteLine(customer);
                        }

                        lock (this)
                        {
                            int index = CashRegisters.IndexOf(register);
                            peopleCount[index]--;
                            if (peopleCount[index] == 0)
                                itemsCount[index] = 0;
                            else
                                itemsCount[index] -= customer.GetNumberOfGoodsInCart;

                            Console.WriteLine($"{customer} - {register} ({peopleCount[index]} people with {itemsCount[index]} items behind)");
                        }
                    }
                }
            });
        }

        Close();
    }
        
    private async void ProcessNewCustomers(List<Customer> customers, bool useLeastCustomers, int[] peopleCount, int[] itemsCount)
    {
        int totalCustomers = 10;

        while (totalCustomers < 20)
        {
            await Task.Delay(TimeSpan.FromSeconds(7));
            List<Customer> newCustomers = new List<Customer>(1);
            newCustomers.Add(customers[totalCustomers]);

            Parallel.ForEach(newCustomers, new ParallelOptions { MaxDegreeOfParallelism = 2 }, customer =>
            {
                customer.FillCart();

                var chosenRegister = useLeastCustomers
                    ? customer.ChooseCheckoutWithLeastCustomers(cashRegisters)
                    : customer.ChooseCheckoutWithLeastGoods(cashRegisters);

                int chosenRegisterIndex = CashRegisters.IndexOf(chosenRegister);

                lock (chosenRegister.Customers)
                {
                    peopleCount[chosenRegisterIndex]++;
                    itemsCount[chosenRegisterIndex] += customer.GetNumberOfGoodsInCart;
                    chosenRegister.EnqueueCustomer(customer);
                }

                totalCustomers++;
                Console.WriteLine($"New customer arrived: {customer} - Chose {chosenRegister} ({peopleCount[chosenRegisterIndex]} people with {itemsCount[chosenRegisterIndex]} items behind)");
            });
        }
    }
}