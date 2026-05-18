namespace ProductApi.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        //Constructor with validation for creating a new product
        public Product(string name, decimal price, int quantity)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty");
            }

            if(price < 0)
            {
                throw new ArgumentException("Price cannot be negative");
            }
            
            if(quantity < 0)
            {
                throw new ArgumentException("Quantity must be greater than 0");
            }

            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }
}
