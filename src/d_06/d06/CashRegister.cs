using System.Collections.Generic;

public class CashRegister
{
    private readonly string name;
    private readonly Queue<Customer> customers;
    private readonly TimeSpan itemProcessingTime;
    private readonly TimeSpan switchCustomerTime;
    private TimeSpan totalProcessingTime;

    public CashRegister(string name, TimeSpan itemProcessingTime, TimeSpan switchCustomerTime)
    {
        this.name = name;
        customers = new Queue<Customer>();
        this.itemProcessingTime = itemProcessingTime;
        this.switchCustomerTime = switchCustomerTime;
        totalProcessingTime = TimeSpan.Zero;
        _totalWaitTimeInSeconds = 0;
        _totalProcessedCustomers = 0;
    }
    
    public TimeSpan ItemProcessingTime => itemProcessingTime;
    public TimeSpan SwitchCustomerTime => switchCustomerTime;
    public TimeSpan TotalProcessingTime => totalProcessingTime;
    public string Name => name;
    public Queue<Customer> Customers => customers;

    public void EnqueueCustomer(Customer customer)
    {
        customers.Enqueue(customer);
    }

    public Customer? DequeueCustomer()
    {
        if (customers.Count > 0)
        {
            return customers.Dequeue();
        }

        return null;
    }

    public override string ToString()
    {
        return $"Register #{name}";
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        CashRegister otherCashRegister = (CashRegister)obj;
        return name == otherCashRegister.name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(name);
    }
    
    public static bool operator ==(CashRegister? register1, CashRegister? register2)
    {
        if (ReferenceEquals(register1, null) && ReferenceEquals(register2, null))
        {
            return true;
        }

        if (ReferenceEquals(register1, null) || ReferenceEquals(register2, null))
        {
            return false;
        }

        return register1.name == register2.name;
    }

    public static bool operator !=(CashRegister register1, CashRegister register2)
    {
        return !(register1 == register2);
    }
    
    private double _totalWaitTimeInSeconds;
    private int _totalProcessedCustomers;

    public Customer? Process()
    {
        Customer? customer = DequeueCustomer();
        {
            TimeSpan randomProcessingTime = TimeSpan.FromSeconds(new Random().Next(1, (int)ItemProcessingTime.TotalSeconds + 1));
            TimeSpan customerProcessingTime = randomProcessingTime * customer.GetNumberOfGoodsInCart + SwitchCustomerTime;

            System.Threading.Thread.Sleep(customerProcessingTime);

            _totalWaitTimeInSeconds += customerProcessingTime.TotalSeconds;
            _totalProcessedCustomers++;
            totalProcessingTime += customerProcessingTime;

            Console.WriteLine($"{customer} processed at {Name}. Processing time: {customerProcessingTime.TotalSeconds} seconds. Total time: {TotalProcessingTime.TotalSeconds} seconds.");
        }

        return customer;
    }
    
    public double GetAverageWaitTime()
    {
        return _totalWaitTimeInSeconds / (_totalProcessedCustomers > 0 ? _totalProcessedCustomers : 1);
    }
}