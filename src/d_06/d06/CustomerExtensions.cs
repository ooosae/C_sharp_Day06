using System.Collections.Generic;
using System.Linq;

namespace d06;

public static class CustomerExtensions
{
    public static CashRegister? ChooseCheckoutWithLeastCustomers(this Customer customer, IEnumerable<CashRegister> cashRegisters)
    {
        return cashRegisters
            .AsParallel()
            .OrderBy(register => register.Customers.ToList().Count)
            .FirstOrDefault();
    }

    public static CashRegister? ChooseCheckoutWithLeastGoods(this Customer customer, IEnumerable<CashRegister> cashRegisters)
    {
        return cashRegisters
            .AsParallel()
            .OrderBy(register => register.Customers.ToList().Sum(c => c.GetNumberOfGoodsInCart))
            .FirstOrDefault();
    }
}