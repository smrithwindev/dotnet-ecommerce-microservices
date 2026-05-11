namespace OrderApi.Domain.Entities
{
    public class Order
    {
        public int Id { get; private set; }

        public int ProductId { get; private set; }

        public int ClientId { get; private set; }

        public int PurchaseQuantity { get; private set; }

        public DateTime OrderedDate { get; private set; }

        // Main constructor
        public Order(int productId, int clientId, int purchaseQuantity)
        {
            if (productId <= 0)
                throw new ArgumentException("Invalid product id");

            if (clientId <= 0)
                throw new ArgumentException("Invalid client id");

            if (purchaseQuantity <= 0)
                throw new ArgumentException("Purchase quantity must be greater than zero");

            ProductId = productId;
            ClientId = clientId;
            PurchaseQuantity = purchaseQuantity;
            OrderedDate = DateTime.UtcNow;
        }

        // Required by EF Core
        private Order()
        {
        }
    }
}