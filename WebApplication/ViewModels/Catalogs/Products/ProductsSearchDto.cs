namespace WebApplication.ViewModels
{
    public class ProductsSearchDto
    {
        public string text { get; set; }
        public int id_categoria { get; set; }
        public int? id_linea { get; set; }
        public int? id_sublinea { get; set; }
        public bool estatus { get; set; }
    }
}
