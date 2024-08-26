﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class ProductCreateDto
    {
        public ProductCreateDto()
        {
            cat_imagenes_producto = new List<ProductImagesDto>();
            relacionados = new List<productos_relacionados>();
        }

        
        public int id { get; set; }
        public string sku { get; set; }
        public string no_serie { get; set; }
        public string modelo { get; set; }
        public string nombre { get; set; }
        public string descripcion_corta { get; set; }
        public string descripcion_larga { get; set; }
        public string atributos { get; set; }
        public float precio_sin_iva { get; set; }
        public float precio_con_iva { get; set; }
        public int id_categoria { get; set; }
        public int? id_linea { get; set; }
        public int? id_sublinea { get; set; }
        public int id_caracteristica_base { get; set; }
        public string ficha_tecnica { get; set; }
        public int? horas_tecnico { get; set; }
        public int? no_tecnico { get; set; }
        public decimal? precio_hora { get; set; }
        public bool estatus { get; set; }
        public long actualizadopor { get; set; }
        public int tipo { get; set; } //  1 es producto, 2 es accesorio, 3 es accesorio, 4 es "no instalable" !!! HAY QUE HACER CATALGO  
        public string url_guia { get; set; }
        public string url_manual { get; set; }
        public string url_impresion { get; set; }
        public Boolean requiere_instalacion { get; set; }
        public Boolean visible_partners { get; set; }

        public List<ProductImagesDto> cat_imagenes_producto { get; set; }
        public List<productos_relacionados> relacionados { get; set; }

    }
}
