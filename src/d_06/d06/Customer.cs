public class Customer
{
    private readonly string name;
    private readonly int number;
    private readonly int cartCapacity;
    private int numberOfGoodsInCart;

    public Customer(string name, int number, int cartCapacity)
    {
        this.name = name;
        this.number = number;
        this.cartCapacity = cartCapacity;
        this.numberOfGoodsInCart = 0;
    }

    public string GetName => name;
    public int GetNumber => number;
    public int GetCartCapacity => cartCapacity;
    public int GetNumberOfGoodsInCart => numberOfGoodsInCart;

    public void SetNumberOfGoods(int count)
    {
        numberOfGoodsInCart = count;
    }
    
    public void FillCart()
    {
        Random random = new Random();
        numberOfGoodsInCart = random.Next(1, cartCapacity + 1);
    }
    
    public override string ToString()
    {
        return $"{name}, customer #{number} ({numberOfGoodsInCart} items in cart)";
    }
    
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Customer otherCustomer = (Customer)obj;
        return name == otherCustomer.name && number == otherCustomer.number;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(name, number);
    }
    
    public static bool operator ==(Customer left, Customer right)
    {
        if (object.ReferenceEquals(left, null))
            return object.ReferenceEquals(right, null);

        return left.Equals(right);
    }

    public static bool operator !=(Customer left, Customer right)
    {
        return !(left == right);
    }
    
}