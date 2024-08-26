using AutoMapper;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ProductosQuejas, ProductosQuejasDto>()
                .ReverseMap();
            CreateMap<Propuestas, PropuestasDto>()
                .ReverseMap();
            CreateMap<Propuestas, PropuestasUpdateDto>()
                .ReverseMap();
            CreateMap<Quejas, QuejasDto>()
                .ReverseMap();
            CreateMap<Cat_Linea_Producto, ProductLinesDto>()
                .ReverseMap();
            CreateMap<Cat_SubLinea_Producto, ProductSubLinesDto>()
                .ReverseMap();
            CreateMap<Cat_Imagenes_Producto, ProductImagesDto>()
                .ReverseMap();
            CreateMap<Cat_Productos, ProductsDto>()
                .ReverseMap();
            CreateMap<Cat_SubLinea_Producto, ProductSubLinesUpdateDto>()
                .ReverseMap();
            CreateMap<Cat_Productos, ProductsUpdateDto>()
                .ReverseMap();
            CreateMap<Clientes, ClientsDto>()
                .ReverseMap();
            CreateMap<Clientes, ClientsUpdateDto>()
                .ReverseMap();

            CreateMap<Cat_Productos, ProductCreateDto>().ReverseMap();
            CreateMap<Cat_SubLinea_Producto, ProductSubLineCreateDto>().ReverseMap();
            CreateMap<Cat_SubLinea_Producto, ProductSubLinesDto>().ReverseMap();
            CreateMap<Cat_SubLinea_Producto, ProductSubLinesUpdateDto>().ReverseMap();
            CreateMap<Cat_SubLinea_Producto, ProductSubLineDto>().ReverseMap();
            CreateMap<ProductSubLineDto, ProductSubLinesUpdateDto>().ReverseMap();
            CreateMap<productos_relacionados, ProductsRelatedDto>().ReverseMap();
            CreateMap<Rel_Categoria_Producto_Tipo_Producto, ProductSubLineTypeDto>().ReverseMap();

        }
    }
}
