namespace WebApplication.Models
{
    public class ProductosQuejas
    {
        public int ProductoId { get; set; }
        public int QuejaId { get; set; }

        public Cat_Productos Producto { get; set; }
        public Quejas Queja { get; set; }
    }
}