using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccesoriosRelacionados",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_producto_padre = table.Column<int>(type: "int", nullable: false),
                    id_producto_recomendado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccesoriosRelacionados", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Canales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Accesorios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    actualizado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    actualizadopor = table.Column<long>(type: "bigint", nullable: false),
                    atributos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    descripcion_corta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    descripcion_larga = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    ficha_tecnica = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    modelo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    precio_con_iva = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    precio_sin_iva = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sku = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Accesorios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Actividad",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_actividad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Actividad", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Aplicaciones",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    App = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Aplicaciones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Area_Cobertura",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_cobertura = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Area_Cobertura", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cat_beneficios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    beneficio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_beneficios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_canales",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Canal_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Canal_es = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_canales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Categoria_Producto",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    codigo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Categoria_Producto", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Categoria_Servicio",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_categoria_servicio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_tipo_servicio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Categoria_Servicio", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Checklist_Producto",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_checklist_producto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Checklist_Producto", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_CondicionesComerciales",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    activa = table.Column<bool>(type: "bit", nullable: false),
                    condicion_comercial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    meses_credito = table.Column<float>(type: "real", nullable: false),
                    monto_descuento = table.Column<float>(type: "real", nullable: false),
                    num_meses_sinint = table.Column<float>(type: "real", nullable: false),
                    porcentaje_credito = table.Column<float>(type: "real", nullable: false),
                    porcentaje_descuento = table.Column<float>(type: "real", nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_CondicionesComerciales", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_CondicionesPago",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_Cat_Formas_Pago = table.Column<int>(type: "int", nullable: false),
                    id_cuenta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_CondicionesPago", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_distribuidor_autorizado",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_distribuidor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_distribuidor_autorizado", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Estado",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Estado", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Estatus_Compra",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Estatus_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estatus_es = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Estatus_Compra", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Estatus_Cotizacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Estatus_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estatus_es = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Estatus_Cotizacion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_estatus_servicio",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_estatus_servicio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    desc_estatus_servicio_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_tipo_servicio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_estatus_servicio", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cat_falla",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_falla_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    desc_falla_es = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_falla", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Formas_Pago",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FormaPago = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    comprobantes_obligatorios = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Formas_Pago", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Lista_Precios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    grupo_precio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    precio_sin_iva = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Lista_Precios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Materiales_Tecnico",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_material = table.Column<int>(type: "int", nullable: false),
                    id_tecnico = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Materiales_Tecnico", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cat_msi",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_msi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_msi", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Producto",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_producto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Producto", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Productos_Estatus_Troubleshooting",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    codigo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    desc_troubleshooting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Productos_Estatus_Troubleshooting", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cat_reparacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_reparacion_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    desc_reparacion_es = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_reparacion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Roles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    rol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    siglas = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_solicitado_por",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_solicitado_por = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_solicitado_por", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_solicitud_via",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_solicitud_via = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_solicitud_via", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Tecnicos_Tipo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_tipo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Tecnicos_Tipo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cat_tipo_condicion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    tipo_condicion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_tipo_condicion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cat_tipo_entidades",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    tipo_entidad = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_tipo_entidades", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cat_tipo_productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_tipo_productos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Tipo_Refaccion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_tipo_refaccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Tipo_Refaccion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_tipo_servicio",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_tipo_servicio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_tipo_servicio", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cat_tipos_herencia",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_tipos_herencia", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CatEstatus_Producto",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_estatus_producto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    desc_estatus_producto_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_tipo_servicio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatEstatus_Producto", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CatEstatus_Visita",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_estatus_visita = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    desc_estatus_visita_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_tipo_servicio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatEstatus_Visita", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Check_List_Categoria_Producto",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    categoria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    categoria_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sku = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Check_List_Categoria_Producto", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CondicionesComerciales_Cuenta",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Vigencia_final = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Vigencia_inicial = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_condicion = table.Column<int>(type: "int", nullable: false),
                    id_cuenta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondicionesComerciales_Cuenta", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "DatosFiscales_Canales",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ext_fact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Int_fact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    calle_numero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    colonia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_canal = table.Column<long>(type: "bigint", nullable: false),
                    id_estado = table.Column<int>(type: "int", nullable: false),
                    id_municipio = table.Column<int>(type: "int", nullable: false),
                    nombre_fact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    razon_social = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rfc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono_fact = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosFiscales_Canales", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "documentos_cotizacion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Id_Cotizacion = table.Column<long>(type: "bigint", nullable: false),
                    Id_foto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fecha_subida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_forma_pago = table.Column<int>(type: "int", nullable: false),
                    id_tipo_tipo_pago = table.Column<int>(type: "int", nullable: false),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    tipo_docto = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentos_cotizacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "formas_pago_tipos_comprobantes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_Cat_Formas_Pago = table.Column<int>(type: "int", nullable: false),
                    id_tipo_comprobantes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_formas_pago_tipos_comprobantes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Log_refacciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    almacen_entrada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    almacen_salida = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    id_refaccion = table.Column<int>(type: "int", nullable: false),
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log_refacciones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus_leido = table.Column<bool>(type: "bit", nullable: false),
                    evento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rol_notificado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Permisos_Flujo_Cotizacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_Canal = table.Column<long>(type: "bigint", nullable: false),
                    id_estatus_cotizacion = table.Column<int>(type: "int", nullable: false),
                    id_rol = table.Column<long>(type: "bigint", nullable: false),
                    id_tabla = table.Column<int>(type: "int", nullable: false),
                    inhabilitado = table.Column<bool>(type: "bit", nullable: false),
                    permiso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos_Flujo_Cotizacion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "productos_relacionados",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    id_producto_2 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos_relacionados", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "RegistroItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    Materno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Paterno = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rel_Imagen_Producto_Visita",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    actividad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    id_visita = table.Column<int>(type: "int", nullable: false),
                    path = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rel_Imagen_Producto_Visita", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TipoQueja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoQueja", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tipos_comprobantes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    es_liquidacion = table.Column<bool>(type: "bit", nullable: false),
                    tipo_pago = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_comprobantes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TokenItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Fecha = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Id_user = table.Column<long>(type: "bigint", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Accesorios_Relacionados",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_Accesorio = table.Column<int>(type: "int", nullable: false),
                    sku_sugerido = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Accesorios_Relacionados", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Accesorios_relacionados",
                        column: x => x.id_Accesorio,
                        principalTable: "Cat_Accesorios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Imagenes_Accesosrios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_Accesorio = table.Column<int>(type: "int", nullable: false),
                    url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Imagenes_Accesosrios", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Imagen_Accesorios",
                        column: x => x.id_Accesorio,
                        principalTable: "Cat_Accesorios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Cuentas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cuenta_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cuenta_es = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Id_Canal = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Cuentas", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_Canal_cuenta",
                        column: x => x.Id_Canal,
                        principalTable: "Cat_canales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Linea_Producto",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_categoriaid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Linea_Producto", x => x.id);
                    table.ForeignKey(
                        name: "FK_Cat_Linea_Producto_Cat_Categoria_Producto_id_categoriaid",
                        column: x => x.id_categoriaid,
                        principalTable: "Cat_Categoria_Producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Municipio",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    desc_municipio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estadoid = table.Column<int>(type: "int", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Municipio", x => x.id);
                    table.ForeignKey(
                        name: "FK_Cat_Municipio_Cat_Estado_estadoid",
                        column: x => x.estadoid,
                        principalTable: "Cat_Estado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Materiales",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cantidad = table.Column<int>(type: "int", nullable: true),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_grupo_precio = table.Column<int>(type: "int", nullable: false),
                    no_material = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Materiales", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Material_Grupo_Precio",
                        column: x => x.id_grupo_precio,
                        principalTable: "Cat_Lista_Precios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    actualizado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    actualizadopor = table.Column<long>(type: "bigint", nullable: false),
                    avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    fecha_nacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_Sucursales = table.Column<int>(type: "int", nullable: false),
                    id_app = table.Column<long>(type: "bigint", nullable: false),
                    id_canal = table.Column<int>(type: "int", nullable: false),
                    id_cuenta = table.Column<int>(type: "int", nullable: false),
                    id_rol = table.Column<long>(type: "bigint", nullable: false),
                    materno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nivel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    paterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono_movil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Users_App",
                        column: x => x.id_app,
                        principalTable: "Cat_Aplicaciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Users_Rol",
                        column: x => x.id_rol,
                        principalTable: "Cat_Roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rel_Categoria_Producto_Tipo_Producto",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    horas_tecnicos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_categoria = table.Column<int>(type: "int", nullable: false),
                    id_tipo_servicio = table.Column<int>(type: "int", nullable: false),
                    no_tecnicos = table.Column<int>(type: "int", nullable: false),
                    precio_hora_tecnico = table.Column<int>(type: "int", nullable: true),
                    precio_visita = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rel_Categoria_Producto_Tipo_Producto", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Categoria_Producto_Rel",
                        column: x => x.id_categoria,
                        principalTable: "Cat_Categoria_Producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Categoria_Producto_Rel_Tipo_Servicio",
                        column: x => x.id_tipo_servicio,
                        principalTable: "Cat_tipo_servicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sub_cat_tipo_servicio",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_tipo_servicio = table.Column<int>(type: "int", nullable: false),
                    sub_desc_tipo_servicio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_cat_tipo_servicio", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Servicio_SubTipoServicio",
                        column: x => x.id_tipo_servicio,
                        principalTable: "Cat_tipo_servicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "promocion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    beneficio_obligatorio = table.Column<bool>(type: "bit", nullable: false),
                    fecha_hora_fin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fecha_hora_inicio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_cat_tipo_condicion = table.Column<int>(type: "int", nullable: false),
                    id_tipo_beneficio = table.Column<int>(type: "int", nullable: false),
                    id_tipos_herencia_promo = table.Column<int>(type: "int", nullable: false),
                    incluir_desc_adic = table.Column<bool>(type: "bit", nullable: false),
                    monto_condicion = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vigencia_indefinida = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promocion", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_2promocion_condicion2",
                        column: x => x.id_cat_tipo_condicion,
                        principalTable: "cat_tipo_condicion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_1promocion_herencia2",
                        column: x => x.id_tipos_herencia_promo,
                        principalTable: "cat_tipos_herencia",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Check_List_Preguntas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_categoria = table.Column<int>(type: "int", nullable: false),
                    pregunta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pregunta_en = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Check_List_Preguntas", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Ckl_Categoria_Producto",
                        column: x => x.id_categoria,
                        principalTable: "Check_List_Categoria_Producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Sucursales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Id_Cuenta = table.Column<long>(type: "bigint", nullable: false),
                    Sucursal = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Sucursales", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_cuenta_Sucursal",
                        column: x => x.Id_Cuenta,
                        principalTable: "Cat_Cuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cat_SubLinea_Producto",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_linea_producto = table.Column<int>(type: "int", nullable: false),
                    id_lineaid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_SubLinea_Producto", x => x.id);
                    table.ForeignKey(
                        name: "FK_Cat_SubLinea_Producto_Cat_Linea_Producto_id_lineaid",
                        column: x => x.id_lineaid,
                        principalTable: "Cat_Linea_Producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Localidad",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cp = table.Column<long>(type: "bigint", nullable: false),
                    desc_localidad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    municipioid = table.Column<int>(type: "int", nullable: true),
                    zona = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Localidad", x => x.id);
                    table.ForeignKey(
                        name: "FK_Cat_Localidad_Cat_Municipio_municipioid",
                        column: x => x.municipioid,
                        principalTable: "Cat_Municipio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cat_Estadoid = table.Column<int>(type: "int", nullable: true),
                    Cat_Municipioid = table.Column<int>(type: "int", nullable: true),
                    Id_sucursal = table.Column<int>(type: "int", nullable: false),
                    actualizado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    actualizadopor = table.Column<long>(type: "bigint", nullable: false),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    folio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    materno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nombre_comercial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nombre_contacto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    paterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    referencias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    referidopor = table.Column<long>(type: "bigint", nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono_movil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tipo_persona = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vigencia_ref = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.id);
                    table.ForeignKey(
                        name: "FK_Clientes_Cat_Estado_Cat_Estadoid",
                        column: x => x.Cat_Estadoid,
                        principalTable: "Cat_Estado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Cat_Municipio_Cat_Municipioid",
                        column: x => x.Cat_Municipioid,
                        principalTable: "Cat_Municipio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vendedores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    actualizado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    actualizadopor = table.Column<long>(type: "bigint", nullable: false),
                    calle_numero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    colonia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_estado = table.Column<int>(type: "int", nullable: false),
                    id_municipio = table.Column<int>(type: "int", nullable: false),
                    materno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nombre_comercial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nombre_contacto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    paterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    referencias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono_movil = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendedores", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_Vendedores_Estado",
                        column: x => x.id_estado,
                        principalTable: "Cat_Estado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Vendedores_Mun",
                        column: x => x.id_municipio,
                        principalTable: "Cat_Municipio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tecnicos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    actualizado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    actualizadopor = table.Column<long>(type: "bigint", nullable: false),
                    color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    id_tipo_tecnico = table.Column<int>(type: "int", nullable: false),
                    noalmacen = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnicos", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Users_Tecnicos",
                        column: x => x.id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Users_Tecnicos_Tipo",
                        column: x => x.id_tipo_tecnico,
                        principalTable: "Cat_Tecnicos_Tipo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beneficio_desc",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    es_porcentaje = table.Column<bool>(type: "bit", nullable: false),
                    id_promocion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beneficio_desc", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_7beneficiodesc_promocion2",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beneficio_msi",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_cat_msi = table.Column<int>(type: "int", nullable: false),
                    id_promocion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beneficio_msi", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_beneficiomsi_catmsi2",
                        column: x => x.id_cat_msi,
                        principalTable: "cat_msi",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_beneficiomsi_promo52",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beneficio_productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    id_promocion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beneficio_productos", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_8beneficioproductos_promocion2",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beneficios_promocion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_cat_beneficios = table.Column<int>(type: "int", nullable: false),
                    id_promocion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beneficios_promocion", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_beneficiospromocion_cattipoentidades2",
                        column: x => x.id_cat_beneficios,
                        principalTable: "cat_beneficios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_6beneficiospromocion_promocion2",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entidades_excluidas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_entidad = table.Column<int>(type: "int", nullable: false),
                    id_promocion = table.Column<int>(type: "int", nullable: false),
                    id_tipo_entidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entidades_excluidas", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_4entidadesexcluidas_promocion2",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_entidades_excluidas_cattipoentidades2",
                        column: x => x.id_tipo_entidad,
                        principalTable: "cat_tipo_entidades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entidades_obligatorias",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_entidad = table.Column<int>(type: "int", nullable: false),
                    id_promocion = table.Column<int>(type: "int", nullable: false),
                    id_tipo_entidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entidades_obligatorias", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_4entidadesobli_promocion2",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_entidades_obligatorias_cattipoentidades2",
                        column: x => x.id_tipo_entidad,
                        principalTable: "cat_tipo_entidades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entidades_participantes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_entidad = table.Column<int>(type: "int", nullable: false),
                    id_promocion = table.Column<int>(type: "int", nullable: false),
                    id_tipo_entidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entidades_participantes", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_3entidadesparticipantes_promocion2",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_entidadesparticipantes_cattipoentidades2",
                        column: x => x.id_tipo_entidad,
                        principalTable: "cat_tipo_entidades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Producto_Producto",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_producto = table.Column<long>(type: "bigint", nullable: false),
                    id_promocion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto_Producto", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_producto_promocion_cat_productos",
                        column: x => x.id_producto,
                        principalTable: "Cat_Producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_producto_promocion_promociones",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "productos_condicion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    id_promocion = table.Column<int>(type: "int", nullable: false),
                    id_tipo_categoria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos_condicion", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_5productoscondicion_promocion2",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "productos_excluidos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    id_promocion = table.Column<int>(type: "int", nullable: false),
                    id_tipo_categoria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos_excluidos", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_5productos_excluidos_promocion2",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "promociones_compatibles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_promocion = table.Column<int>(type: "int", nullable: false),
                    id_promocion_2 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promociones_compatibles", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_5promocompatibles1_promocion2",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    actualizado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    actualizadopor = table.Column<long>(type: "bigint", nullable: false),
                    atributos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    descripcion_corta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    descripcion_larga = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    ficha_tecnica = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    horas_tecnico = table.Column<int>(type: "int", nullable: true),
                    id_categoria = table.Column<int>(type: "int", nullable: false),
                    id_linea = table.Column<int>(type: "int", nullable: true),
                    id_sublinea = table.Column<int>(type: "int", nullable: true),
                    modelo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    no_serie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    no_tecnico = table.Column<int>(type: "int", nullable: true),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    precio_con_iva = table.Column<float>(type: "real", nullable: false),
                    precio_hora = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    precio_sin_iva = table.Column<float>(type: "real", nullable: false),
                    requiere_instalacion = table.Column<bool>(type: "bit", nullable: false),
                    sku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tipo = table.Column<int>(type: "int", nullable: false),
                    url_guia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    url_manual = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible_partners = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Productos", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Categoria_Producto",
                        column: x => x.id_categoria,
                        principalTable: "Cat_Categoria_Producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Linea_Producto",
                        column: x => x.id_linea,
                        principalTable: "Cat_Linea_Producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ForeignKey_Sublinea_Producto",
                        column: x => x.id_sublinea,
                        principalTable: "Cat_SubLinea_Producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "condiones_comerciales_sucursal",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cat_SubLinea_Productoid = table.Column<int>(type: "int", nullable: true),
                    Cat_SucursalesId = table.Column<int>(type: "int", nullable: true),
                    id_Cat_SubLinea_Producto = table.Column<int>(type: "int", nullable: false),
                    id_Cat_Sucursales = table.Column<int>(type: "int", nullable: false),
                    margen = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_condiones_comerciales_sucursal", x => x.id);
                    table.ForeignKey(
                        name: "FK_condiones_comerciales_sucursal_Cat_SubLinea_Producto_Cat_SubLinea_Productoid",
                        column: x => x.Cat_SubLinea_Productoid,
                        principalTable: "Cat_SubLinea_Producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_condiones_comerciales_sucursal_Cat_Sucursales_Cat_SucursalesId",
                        column: x => x.Cat_SucursalesId,
                        principalTable: "Cat_Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Direccion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Fecha_Estimada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    actualizado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    actualizadopor = table.Column<long>(type: "bigint", nullable: false),
                    calle_numero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    colonia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_cliente = table.Column<long>(type: "bigint", nullable: false),
                    id_estado = table.Column<int>(type: "int", nullable: false),
                    id_localidad = table.Column<int>(type: "int", nullable: false),
                    id_municipio = table.Column<int>(type: "int", nullable: false),
                    id_prefijo_calle = table.Column<int>(type: "int", nullable: false),
                    nombrecontacto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    numExt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    numInt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono_movil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tipo_direccion = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Direccion", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Direccion_Cliente",
                        column: x => x.id_cliente,
                        principalTable: "Clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cotizaciones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Acciones = table.Column<int>(type: "int", nullable: false),
                    Estatus = table.Column<int>(type: "int", nullable: false),
                    Id_Canal = table.Column<long>(type: "bigint", nullable: false),
                    Id_Cliente = table.Column<long>(type: "bigint", nullable: false),
                    Id_Cuenta = table.Column<long>(type: "bigint", nullable: false),
                    Id_Estado_Instalacion = table.Column<int>(type: "int", nullable: false),
                    Id_Vendedor = table.Column<long>(type: "bigint", nullable: false),
                    Id_sucursal = table.Column<int>(type: "int", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    acepto_terminos_condiciones = table.Column<bool>(type: "bit", nullable: false),
                    cancelada = table.Column<bool>(type: "bit", nullable: false),
                    coment_cancel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creadopor = table.Column<int>(type: "int", nullable: false),
                    entrega_sol = table.Column<bool>(type: "bit", nullable: false),
                    fecha_cotiza = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ibs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_cotizacion_padre = table.Column<long>(type: "bigint", nullable: false),
                    id_formapago = table.Column<int>(type: "int", nullable: false),
                    id_user_entrega_sol = table.Column<bool>(type: "bit", nullable: false),
                    importe_condiciones_com = table.Column<float>(type: "real", nullable: false),
                    importe_precio_lista = table.Column<float>(type: "real", nullable: false),
                    importe_promociones = table.Column<float>(type: "real", nullable: false),
                    iva_condiciones_com = table.Column<float>(type: "real", nullable: false),
                    iva_precio_lista = table.Column<float>(type: "real", nullable: false),
                    iva_promociones = table.Column<float>(type: "real", nullable: false),
                    motivo_rechazo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    numero_productos = table.Column<int>(type: "int", nullable: false),
                    puede_solicitar_env = table.Column<int>(type: "int", nullable: false),
                    rechazada = table.Column<bool>(type: "bit", nullable: false),
                    referencia = table.Column<long>(type: "bigint", nullable: false),
                    requiere_fact = table.Column<bool>(type: "bit", nullable: false),
                    usr_modifico = table.Column<int>(type: "int", nullable: false),
                    vigenica_ref = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizaciones", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_Cotizacion_Canal",
                        column: x => x.Id_Canal,
                        principalTable: "Cat_canales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Cotizacion_Cliente",
                        column: x => x.Id_Cliente,
                        principalTable: "Clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatosFiscales",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ext_fact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Int_fact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    calle_numero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    colonia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_cliente = table.Column<long>(type: "bigint", nullable: false),
                    id_estado = table.Column<int>(type: "int", nullable: false),
                    id_municipio = table.Column<int>(type: "int", nullable: false),
                    id_prefijo_calle = table.Column<int>(type: "int", nullable: false),
                    nombre_fact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    razon_social = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rfc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono_fact = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosFiscales", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_DatosFiscales_Cliente",
                        column: x => x.id_cliente,
                        principalTable: "Clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quejas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Atendio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanalId = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<long>(type: "bigint", nullable: false),
                    DetalleReclamo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Folio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoQuejaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quejas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quejas_Canales_CanalId",
                        column: x => x.CanalId,
                        principalTable: "Canales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quejas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quejas_TipoQueja_TipoQuejaId",
                        column: x => x.TipoQuejaId,
                        principalTable: "TipoQueja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Servicio",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cat_Categoria_Servicioid = table.Column<int>(type: "int", nullable: true),
                    Cat_estatus_servicioid = table.Column<int>(type: "int", nullable: true),
                    IBS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    activar_credito = table.Column<bool>(type: "bit", nullable: true),
                    actualizado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    actualizadopor = table.Column<long>(type: "bigint", nullable: false),
                    contacto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    descripcion_actividades = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fecha_servicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_categoria_servicio = table.Column<int>(type: "int", nullable: false),
                    id_cliente = table.Column<long>(type: "bigint", nullable: false),
                    id_distribuidor_autorizado = table.Column<long>(type: "bigint", nullable: false),
                    id_estatus_servicio = table.Column<int>(type: "int", nullable: true),
                    id_motivo_cierre = table.Column<int>(type: "int", nullable: true),
                    id_solicitado_por = table.Column<int>(type: "int", nullable: false),
                    id_solicitud_via = table.Column<int>(type: "int", nullable: false),
                    id_sub_tipo_servicio = table.Column<int>(type: "int", nullable: false),
                    id_tipo_servicio = table.Column<int>(type: "int", nullable: false),
                    no_servicio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sub_tipo_servicioid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicio", x => x.id);
                    table.ForeignKey(
                        name: "FK_Servicio_Cat_Categoria_Servicio_Cat_Categoria_Servicioid",
                        column: x => x.Cat_Categoria_Servicioid,
                        principalTable: "Cat_Categoria_Servicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Servicio_Cat_estatus_servicio_Cat_estatus_servicioid",
                        column: x => x.Cat_estatus_servicioid,
                        principalTable: "Cat_estatus_servicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ForeignKey_Servicio_Cliente",
                        column: x => x.id_cliente,
                        principalTable: "Clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Servicio_Solicitado_Por",
                        column: x => x.id_solicitado_por,
                        principalTable: "Cat_solicitado_por",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Servicio_Solicitud_Via",
                        column: x => x.id_solicitud_via,
                        principalTable: "Cat_solicitud_via",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Servicio_TipoServicio",
                        column: x => x.id_tipo_servicio,
                        principalTable: "Cat_tipo_servicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Servicio_Sub_cat_tipo_servicio_sub_tipo_servicioid",
                        column: x => x.sub_tipo_servicioid,
                        principalTable: "Sub_cat_tipo_servicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tecnicos_Actividad",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_actividad = table.Column<long>(type: "bigint", nullable: false),
                    id_user = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnicos_Actividad", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Users_Tecnicos_Actividad",
                        column: x => x.id_user,
                        principalTable: "Tecnicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tecnicos_Cobertura",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_cobertura = table.Column<long>(type: "bigint", nullable: false),
                    id_user = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnicos_Cobertura", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Users_Tecnicos_Cobertura_Cat",
                        column: x => x.id_cobertura,
                        principalTable: "Cat_Area_Cobertura",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Users_Tecnicos_Cobertura",
                        column: x => x.id_user,
                        principalTable: "Tecnicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tecnicos_Producto",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_categoria_producto = table.Column<int>(type: "int", nullable: false),
                    id_user = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnicos_Producto", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Users_Tecnicos_Producto_Cat",
                        column: x => x.id_categoria_producto,
                        principalTable: "Cat_Categoria_Producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Users_Tecnicos_Productos",
                        column: x => x.id_user,
                        principalTable: "Tecnicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Imagenes_Producto",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Imagenes_Producto", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Imagen_Producto",
                        column: x => x.id_producto,
                        principalTable: "Cat_Productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Productos_Preguntas_Troubleshooting",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    actualizado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    actualizadopor = table.Column<long>(type: "bigint", nullable: false),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    pregunta = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Productos_Preguntas_Troubleshooting", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_cat_productos_preguntas_troubleshooting",
                        column: x => x.id_producto,
                        principalTable: "Cat_Productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Sugeridos_Producto",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    sku_sugerido = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Sugeridos_Producto", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Sugeridos_Producto",
                        column: x => x.id_producto,
                        principalTable: "Cat_Productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cliente_Productos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cat_Estatus_CompraId = table.Column<long>(type: "bigint", nullable: true),
                    Cat_Productosid = table.Column<int>(type: "int", nullable: true),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinGarantia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id_Cliente = table.Column<long>(type: "bigint", nullable: false),
                    Id_EsatusCompra = table.Column<long>(type: "bigint", nullable: false),
                    Id_Producto = table.Column<long>(type: "bigint", nullable: false),
                    NoOrdenCompra = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoPoliza = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    actualizado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    actualizadopor = table.Column<long>(type: "bigint", nullable: false),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cliente_Productos_Cat_Estatus_Compra_Cat_Estatus_CompraId",
                        column: x => x.Cat_Estatus_CompraId,
                        principalTable: "Cat_Estatus_Compra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cliente_Productos_Cat_Productos_Cat_Productosid",
                        column: x => x.Cat_Productosid,
                        principalTable: "Cat_Productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ForeignKey_Cliente_Producto",
                        column: x => x.Id_Cliente,
                        principalTable: "Clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Productos_Carrito",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Id_Producto = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos_Carrito", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_Carritos_productos",
                        column: x => x.Id_Producto,
                        principalTable: "Cat_Productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "afectacion_cc",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_condiones_comerciales_sucursal = table.Column<int>(type: "int", nullable: false),
                    id_promocion = table.Column<int>(type: "int", nullable: false),
                    margen = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_afectacion_cc", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_afectacioncc_condiones_comerciales_sucursal2",
                        column: x => x.id_condiones_comerciales_sucursal,
                        principalTable: "condiones_comerciales_sucursal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_9afectacion_cc_promocion2",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cotizacion_Producto",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Id_Cotizacion = table.Column<long>(type: "bigint", nullable: false),
                    Id_Producto = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    iva_cond_comerciales = table.Column<float>(type: "real", nullable: false),
                    iva_precio_descuento = table.Column<float>(type: "real", nullable: false),
                    iva_precio_lista = table.Column<float>(type: "real", nullable: false),
                    precio_condiciones_com = table.Column<float>(type: "real", nullable: false),
                    precio_descuento = table.Column<float>(type: "real", nullable: false),
                    precio_lista = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizacion_Producto", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_Cotizacion_Producto_cotiza",
                        column: x => x.Id_Cotizacion,
                        principalTable: "Cotizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Cotizacion_Producto_prod",
                        column: x => x.Id_Producto,
                        principalTable: "Cat_Productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cotizacion_Promocion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_cotizacion = table.Column<long>(type: "bigint", nullable: false),
                    id_promocion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizacion_Promocion", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_producto_promocion_cotizacion",
                        column: x => x.id_cotizacion,
                        principalTable: "Cotizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_producto_promocion_promocion",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductosQuejas",
                columns: table => new
                {
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    QuejaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductosQuejas", x => new { x.ProductoId, x.QuejaId });
                    table.ForeignKey(
                        name: "FK_ProductosQuejas_Cat_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Cat_Productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductosQuejas_Quejas",
                        column: x => x.QuejaId,
                        principalTable: "Quejas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Propuestas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DetalleCierre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<bool>(type: "bit", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuejaId = table.Column<int>(type: "int", nullable: false),
                    Solucion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propuestas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Propuestas_Quejas",
                        column: x => x.QuejaId,
                        principalTable: "Quejas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historial_estatus",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    estatus_final = table.Column<int>(type: "int", nullable: false),
                    estatus_inicial = table.Column<int>(type: "int", nullable: false),
                    id_servicio = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historial_estatus", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Historial_Servicio",
                        column: x => x.id_servicio,
                        principalTable: "Servicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Servicio_Troubleshooting",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    id_estatus_troubleshooting = table.Column<int>(type: "int", nullable: false),
                    id_servicio = table.Column<long>(type: "bigint", nullable: false),
                    observciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicio_Troubleshooting", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Servicio_Troubleshooting",
                        column: x => x.id_estatus_troubleshooting,
                        principalTable: "Cat_Productos_Estatus_Troubleshooting",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Servicio_Troubleshooting_Servivo",
                        column: x => x.id_servicio,
                        principalTable: "Servicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visita",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Tecnicosid = table.Column<long>(type: "bigint", nullable: true),
                    actividades_realizar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    asignacion_refacciones = table.Column<bool>(type: "bit", nullable: true),
                    cantidad = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    comprobante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    concepto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    entrega_refacciones = table.Column<bool>(type: "bit", nullable: true),
                    estatus = table.Column<int>(type: "int", nullable: true),
                    factura = table.Column<bool>(type: "bit", nullable: false),
                    fecha_deposito = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fecha_entrega_refaccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_fin_visita = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fecha_inicio_visita = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fecha_visita = table.Column<DateTime>(type: "datetime2", nullable: false),
                    garantia = table.Column<bool>(type: "bit", nullable: false),
                    hora = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hora_fin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_direccion = table.Column<int>(type: "int", nullable: false),
                    id_servicio = table.Column<long>(type: "bigint", nullable: false),
                    imagen_firma = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    imagen_pago_referenciado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    latitud_fin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    latitud_inicio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    longitud_fin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    longitud_inicio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    no_operacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pagado = table.Column<bool>(type: "bit", nullable: false),
                    pago_pendiente = table.Column<bool>(type: "bit", nullable: false),
                    persona_recibe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pre_diagnostico = table.Column<bool>(type: "bit", nullable: true),
                    si_acepto_tecnico_refaccion = table.Column<bool>(type: "bit", nullable: true),
                    terminos_condiciones = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visita", x => x.id);
                    table.ForeignKey(
                        name: "FK_Visita_Tecnicos_Tecnicosid",
                        column: x => x.Tecnicosid,
                        principalTable: "Tecnicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ForeignKey_Servicio_Visita",
                        column: x => x.id_servicio,
                        principalTable: "Servicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Productos_Respuestas_Troubleshooting",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    actualizado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    actualizadopor = table.Column<long>(type: "bigint", nullable: false),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    falla = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_troubleshooting = table.Column<int>(type: "int", nullable: false),
                    solucion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Productos_Respuestas_Troubleshooting", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Cat_Productos_Respuestas_Troubleshooting",
                        column: x => x.id_troubleshooting,
                        principalTable: "Cat_Productos_Preguntas_Troubleshooting",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prediagnostico",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_visita = table.Column<long>(type: "bigint", nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visitaid = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prediagnostico", x => x.id);
                    table.ForeignKey(
                        name: "FK_Prediagnostico_Visita_visitaid",
                        column: x => x.visitaid,
                        principalTable: "Visita",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Producto_Check_List_Respuestas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    estatus = table.Column<int>(type: "int", nullable: false),
                    id_producto = table.Column<long>(type: "bigint", nullable: false),
                    id_vista = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto_Check_List_Respuestas", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Producto_Check_List_Respuestas",
                        column: x => x.id_vista,
                        principalTable: "Visita",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rel_servicio_producto",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    descripcion_cierre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<int>(type: "int", nullable: false),
                    garantia = table.Column<bool>(type: "bit", nullable: false),
                    id_producto = table.Column<long>(type: "bigint", nullable: false),
                    id_vista = table.Column<long>(type: "bigint", nullable: false),
                    no_serie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    primera_visita = table.Column<bool>(type: "bit", nullable: false),
                    reparacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rel_servicio_producto", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Rel_Servicio_Producto",
                        column: x => x.id_vista,
                        principalTable: "Visita",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rel_servicio_Refaccion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    actividades = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estatus = table.Column<int>(type: "int", nullable: false),
                    fallas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_producto = table.Column<long>(type: "bigint", nullable: false),
                    id_vista = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rel_servicio_Refaccion", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Rel_Servicio_Refaccion",
                        column: x => x.id_vista,
                        principalTable: "Visita",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rel_tecnico_visita",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_tecnico = table.Column<long>(type: "bigint", nullable: false),
                    id_vista = table.Column<long>(type: "bigint", nullable: false),
                    tecnico_responsable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rel_tecnico_visita", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Tecnico_Visita",
                        column: x => x.id_tecnico,
                        principalTable: "Tecnicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Visita_Tecnico",
                        column: x => x.id_vista,
                        principalTable: "Visita",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prediagnostico_Refacciones",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    garantia = table.Column<bool>(type: "bit", nullable: false),
                    id_material = table.Column<long>(type: "bigint", nullable: false),
                    id_prediagnostico = table.Column<long>(type: "bigint", nullable: false),
                    numero_ir = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prediagnostico_Refacciones", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Prediagnostico_Refacciones",
                        column: x => x.id_prediagnostico,
                        principalTable: "Prediagnostico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Check_List_Respuestas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    comentarios = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    comentarios_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_pregunta = table.Column<int>(type: "int", nullable: false),
                    id_producto_check_list_respuestas = table.Column<int>(type: "int", nullable: false),
                    respuesta = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Check_List_Respuestas", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Check_List_Respuestas",
                        column: x => x.id_producto_check_list_respuestas,
                        principalTable: "Producto_Check_List_Respuestas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piezas_Repuesto",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    id_material = table.Column<long>(type: "bigint", nullable: false),
                    id_rel_servicio_refaccion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piezas_Repuesto", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Piezas_Repuesto_Rel_Servicio_Producto",
                        column: x => x.id_rel_servicio_refaccion,
                        principalTable: "Rel_servicio_Refaccion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piezas_Repuesto_Tecnico",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    id_material = table.Column<long>(type: "bigint", nullable: false),
                    id_rel_servicio_refaccion = table.Column<int>(type: "int", nullable: false),
                    tipo_refaccion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piezas_Repuesto_Tecnico", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Piezas_Repuesto_Tecnico_Rel_Servicio_Producto",
                        column: x => x.id_rel_servicio_refaccion,
                        principalTable: "Rel_servicio_Refaccion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_afectacion_cc_id_condiones_comerciales_sucursal",
                table: "afectacion_cc",
                column: "id_condiones_comerciales_sucursal");

            migrationBuilder.CreateIndex(
                name: "IX_afectacion_cc_id_promocion",
                table: "afectacion_cc",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_beneficio_desc_id_promocion",
                table: "beneficio_desc",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_beneficio_msi_id_cat_msi",
                table: "beneficio_msi",
                column: "id_cat_msi");

            migrationBuilder.CreateIndex(
                name: "IX_beneficio_msi_id_promocion",
                table: "beneficio_msi",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_beneficio_productos_id_promocion",
                table: "beneficio_productos",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_beneficios_promocion_id_cat_beneficios",
                table: "beneficios_promocion",
                column: "id_cat_beneficios");

            migrationBuilder.CreateIndex(
                name: "IX_beneficios_promocion_id_promocion",
                table: "beneficios_promocion",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Accesorios_Relacionados_id_Accesorio",
                table: "Cat_Accesorios_Relacionados",
                column: "id_Accesorio");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Cuentas_Id_Canal",
                table: "Cat_Cuentas",
                column: "Id_Canal");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Direccion_id_cliente",
                table: "Cat_Direccion",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Imagenes_Accesosrios_id_Accesorio",
                table: "Cat_Imagenes_Accesosrios",
                column: "id_Accesorio");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Imagenes_Producto_id_producto",
                table: "Cat_Imagenes_Producto",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Linea_Producto_id_categoriaid",
                table: "Cat_Linea_Producto",
                column: "id_categoriaid");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Localidad_municipioid",
                table: "Cat_Localidad",
                column: "municipioid");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Materiales_id_grupo_precio",
                table: "Cat_Materiales",
                column: "id_grupo_precio");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Municipio_estadoid",
                table: "Cat_Municipio",
                column: "estadoid");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Productos_id_categoria",
                table: "Cat_Productos",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Productos_id_linea",
                table: "Cat_Productos",
                column: "id_linea");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Productos_id_sublinea",
                table: "Cat_Productos",
                column: "id_sublinea");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Productos_Preguntas_Troubleshooting_id_producto",
                table: "Cat_Productos_Preguntas_Troubleshooting",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Productos_Respuestas_Troubleshooting_id_troubleshooting",
                table: "Cat_Productos_Respuestas_Troubleshooting",
                column: "id_troubleshooting");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_SubLinea_Producto_id_lineaid",
                table: "Cat_SubLinea_Producto",
                column: "id_lineaid");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Sucursales_Id_Cuenta",
                table: "Cat_Sucursales",
                column: "Id_Cuenta");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Sugeridos_Producto_id_producto",
                table: "Cat_Sugeridos_Producto",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_Check_List_Preguntas_id_categoria",
                table: "Check_List_Preguntas",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Check_List_Respuestas_id_producto_check_list_respuestas",
                table: "Check_List_Respuestas",
                column: "id_producto_check_list_respuestas");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Productos_Cat_Estatus_CompraId",
                table: "Cliente_Productos",
                column: "Cat_Estatus_CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Productos_Cat_Productosid",
                table: "Cliente_Productos",
                column: "Cat_Productosid");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Productos_Id_Cliente",
                table: "Cliente_Productos",
                column: "Id_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Cat_Estadoid",
                table: "Clientes",
                column: "Cat_Estadoid");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Cat_Municipioid",
                table: "Clientes",
                column: "Cat_Municipioid");

            migrationBuilder.CreateIndex(
                name: "IX_condiones_comerciales_sucursal_Cat_SubLinea_Productoid",
                table: "condiones_comerciales_sucursal",
                column: "Cat_SubLinea_Productoid");

            migrationBuilder.CreateIndex(
                name: "IX_condiones_comerciales_sucursal_Cat_SucursalesId",
                table: "condiones_comerciales_sucursal",
                column: "Cat_SucursalesId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_Producto_Id_Cotizacion",
                table: "Cotizacion_Producto",
                column: "Id_Cotizacion");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_Producto_Id_Producto",
                table: "Cotizacion_Producto",
                column: "Id_Producto");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_Promocion_id_cotizacion",
                table: "Cotizacion_Promocion",
                column: "id_cotizacion");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_Promocion_id_promocion",
                table: "Cotizacion_Promocion",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_Id_Canal",
                table: "Cotizaciones",
                column: "Id_Canal");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_Id_Cliente",
                table: "Cotizaciones",
                column: "Id_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_DatosFiscales_id_cliente",
                table: "DatosFiscales",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_entidades_excluidas_id_promocion",
                table: "entidades_excluidas",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_entidades_excluidas_id_tipo_entidad",
                table: "entidades_excluidas",
                column: "id_tipo_entidad");

            migrationBuilder.CreateIndex(
                name: "IX_entidades_obligatorias_id_promocion",
                table: "entidades_obligatorias",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_entidades_obligatorias_id_tipo_entidad",
                table: "entidades_obligatorias",
                column: "id_tipo_entidad");

            migrationBuilder.CreateIndex(
                name: "IX_entidades_participantes_id_promocion",
                table: "entidades_participantes",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_entidades_participantes_id_tipo_entidad",
                table: "entidades_participantes",
                column: "id_tipo_entidad");

            migrationBuilder.CreateIndex(
                name: "IX_historial_estatus_id_servicio",
                table: "historial_estatus",
                column: "id_servicio");

            migrationBuilder.CreateIndex(
                name: "IX_Piezas_Repuesto_id_rel_servicio_refaccion",
                table: "Piezas_Repuesto",
                column: "id_rel_servicio_refaccion");

            migrationBuilder.CreateIndex(
                name: "IX_Piezas_Repuesto_Tecnico_id_rel_servicio_refaccion",
                table: "Piezas_Repuesto_Tecnico",
                column: "id_rel_servicio_refaccion");

            migrationBuilder.CreateIndex(
                name: "IX_Prediagnostico_visitaid",
                table: "Prediagnostico",
                column: "visitaid");

            migrationBuilder.CreateIndex(
                name: "IX_Prediagnostico_Refacciones_id_prediagnostico",
                table: "Prediagnostico_Refacciones",
                column: "id_prediagnostico");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Check_List_Respuestas_id_vista",
                table: "Producto_Check_List_Respuestas",
                column: "id_vista");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Producto_id_producto",
                table: "Producto_Producto",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Producto_id_promocion",
                table: "Producto_Producto",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Carrito_Id_Producto",
                table: "Productos_Carrito",
                column: "Id_Producto");

            migrationBuilder.CreateIndex(
                name: "IX_productos_condicion_id_promocion",
                table: "productos_condicion",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_productos_excluidos_id_promocion",
                table: "productos_excluidos",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosQuejas_QuejaId",
                table: "ProductosQuejas",
                column: "QuejaId");

            migrationBuilder.CreateIndex(
                name: "IX_promocion_id_cat_tipo_condicion",
                table: "promocion",
                column: "id_cat_tipo_condicion");

            migrationBuilder.CreateIndex(
                name: "IX_promocion_id_tipos_herencia_promo",
                table: "promocion",
                column: "id_tipos_herencia_promo");

            migrationBuilder.CreateIndex(
                name: "IX_promociones_compatibles_id_promocion",
                table: "promociones_compatibles",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_Propuestas_QuejaId",
                table: "Propuestas",
                column: "QuejaId");

            migrationBuilder.CreateIndex(
                name: "IX_Quejas_CanalId",
                table: "Quejas",
                column: "CanalId");

            migrationBuilder.CreateIndex(
                name: "IX_Quejas_ClienteId",
                table: "Quejas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Quejas_TipoQuejaId",
                table: "Quejas",
                column: "TipoQuejaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rel_Categoria_Producto_Tipo_Producto_id_categoria",
                table: "Rel_Categoria_Producto_Tipo_Producto",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Rel_Categoria_Producto_Tipo_Producto_id_tipo_servicio",
                table: "Rel_Categoria_Producto_Tipo_Producto",
                column: "id_tipo_servicio");

            migrationBuilder.CreateIndex(
                name: "IX_Rel_servicio_producto_id_vista",
                table: "Rel_servicio_producto",
                column: "id_vista");

            migrationBuilder.CreateIndex(
                name: "IX_Rel_servicio_Refaccion_id_vista",
                table: "Rel_servicio_Refaccion",
                column: "id_vista");

            migrationBuilder.CreateIndex(
                name: "IX_rel_tecnico_visita_id_tecnico",
                table: "rel_tecnico_visita",
                column: "id_tecnico");

            migrationBuilder.CreateIndex(
                name: "IX_rel_tecnico_visita_id_vista",
                table: "rel_tecnico_visita",
                column: "id_vista");

            migrationBuilder.CreateIndex(
                name: "IX_Servicio_Cat_Categoria_Servicioid",
                table: "Servicio",
                column: "Cat_Categoria_Servicioid");

            migrationBuilder.CreateIndex(
                name: "IX_Servicio_Cat_estatus_servicioid",
                table: "Servicio",
                column: "Cat_estatus_servicioid");

            migrationBuilder.CreateIndex(
                name: "IX_Servicio_id_cliente",
                table: "Servicio",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Servicio_id_solicitado_por",
                table: "Servicio",
                column: "id_solicitado_por");

            migrationBuilder.CreateIndex(
                name: "IX_Servicio_id_solicitud_via",
                table: "Servicio",
                column: "id_solicitud_via");

            migrationBuilder.CreateIndex(
                name: "IX_Servicio_id_tipo_servicio",
                table: "Servicio",
                column: "id_tipo_servicio");

            migrationBuilder.CreateIndex(
                name: "IX_Servicio_sub_tipo_servicioid",
                table: "Servicio",
                column: "sub_tipo_servicioid");

            migrationBuilder.CreateIndex(
                name: "IX_Servicio_Troubleshooting_id_estatus_troubleshooting",
                table: "Servicio_Troubleshooting",
                column: "id_estatus_troubleshooting");

            migrationBuilder.CreateIndex(
                name: "IX_Servicio_Troubleshooting_id_servicio",
                table: "Servicio_Troubleshooting",
                column: "id_servicio");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_cat_tipo_servicio_id_tipo_servicio",
                table: "Sub_cat_tipo_servicio",
                column: "id_tipo_servicio");

            migrationBuilder.CreateIndex(
                name: "IX_Tecnicos_id_tipo_tecnico",
                table: "Tecnicos",
                column: "id_tipo_tecnico");

            migrationBuilder.CreateIndex(
                name: "IX_Tecnicos_Actividad_id_user",
                table: "Tecnicos_Actividad",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_Tecnicos_Cobertura_id_cobertura",
                table: "Tecnicos_Cobertura",
                column: "id_cobertura");

            migrationBuilder.CreateIndex(
                name: "IX_Tecnicos_Cobertura_id_user",
                table: "Tecnicos_Cobertura",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_Tecnicos_Producto_id_categoria_producto",
                table: "Tecnicos_Producto",
                column: "id_categoria_producto");

            migrationBuilder.CreateIndex(
                name: "IX_Tecnicos_Producto_id_user",
                table: "Tecnicos_Producto",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_Users_id_app",
                table: "Users",
                column: "id_app");

            migrationBuilder.CreateIndex(
                name: "IX_Users_id_rol",
                table: "Users",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "IX_Vendedores_id_estado",
                table: "Vendedores",
                column: "id_estado");

            migrationBuilder.CreateIndex(
                name: "IX_Vendedores_id_municipio",
                table: "Vendedores",
                column: "id_municipio");

            migrationBuilder.CreateIndex(
                name: "IX_Visita_Tecnicosid",
                table: "Visita",
                column: "Tecnicosid");

            migrationBuilder.CreateIndex(
                name: "IX_Visita_id_servicio",
                table: "Visita",
                column: "id_servicio");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccesoriosRelacionados");

            migrationBuilder.DropTable(
                name: "afectacion_cc");

            migrationBuilder.DropTable(
                name: "beneficio_desc");

            migrationBuilder.DropTable(
                name: "beneficio_msi");

            migrationBuilder.DropTable(
                name: "beneficio_productos");

            migrationBuilder.DropTable(
                name: "beneficios_promocion");

            migrationBuilder.DropTable(
                name: "Cat_Accesorios_Relacionados");

            migrationBuilder.DropTable(
                name: "Cat_Actividad");

            migrationBuilder.DropTable(
                name: "Cat_Checklist_Producto");

            migrationBuilder.DropTable(
                name: "Cat_CondicionesComerciales");

            migrationBuilder.DropTable(
                name: "Cat_CondicionesPago");

            migrationBuilder.DropTable(
                name: "Cat_Direccion");

            migrationBuilder.DropTable(
                name: "Cat_distribuidor_autorizado");

            migrationBuilder.DropTable(
                name: "Cat_Estatus_Cotizacion");

            migrationBuilder.DropTable(
                name: "cat_falla");

            migrationBuilder.DropTable(
                name: "Cat_Formas_Pago");

            migrationBuilder.DropTable(
                name: "Cat_Imagenes_Accesosrios");

            migrationBuilder.DropTable(
                name: "Cat_Imagenes_Producto");

            migrationBuilder.DropTable(
                name: "Cat_Localidad");

            migrationBuilder.DropTable(
                name: "Cat_Materiales");

            migrationBuilder.DropTable(
                name: "Cat_Materiales_Tecnico");

            migrationBuilder.DropTable(
                name: "Cat_Productos_Respuestas_Troubleshooting");

            migrationBuilder.DropTable(
                name: "cat_reparacion");

            migrationBuilder.DropTable(
                name: "Cat_Sugeridos_Producto");

            migrationBuilder.DropTable(
                name: "cat_tipo_productos");

            migrationBuilder.DropTable(
                name: "Cat_Tipo_Refaccion");

            migrationBuilder.DropTable(
                name: "CatEstatus_Producto");

            migrationBuilder.DropTable(
                name: "CatEstatus_Visita");

            migrationBuilder.DropTable(
                name: "Check_List_Preguntas");

            migrationBuilder.DropTable(
                name: "Check_List_Respuestas");

            migrationBuilder.DropTable(
                name: "Cliente_Productos");

            migrationBuilder.DropTable(
                name: "CondicionesComerciales_Cuenta");

            migrationBuilder.DropTable(
                name: "Cotizacion_Producto");

            migrationBuilder.DropTable(
                name: "Cotizacion_Promocion");

            migrationBuilder.DropTable(
                name: "DatosFiscales");

            migrationBuilder.DropTable(
                name: "DatosFiscales_Canales");

            migrationBuilder.DropTable(
                name: "documentos_cotizacion");

            migrationBuilder.DropTable(
                name: "entidades_excluidas");

            migrationBuilder.DropTable(
                name: "entidades_obligatorias");

            migrationBuilder.DropTable(
                name: "entidades_participantes");

            migrationBuilder.DropTable(
                name: "formas_pago_tipos_comprobantes");

            migrationBuilder.DropTable(
                name: "historial_estatus");

            migrationBuilder.DropTable(
                name: "Log_refacciones");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "Permisos_Flujo_Cotizacion");

            migrationBuilder.DropTable(
                name: "Piezas_Repuesto");

            migrationBuilder.DropTable(
                name: "Piezas_Repuesto_Tecnico");

            migrationBuilder.DropTable(
                name: "Prediagnostico_Refacciones");

            migrationBuilder.DropTable(
                name: "Producto_Producto");

            migrationBuilder.DropTable(
                name: "Productos_Carrito");

            migrationBuilder.DropTable(
                name: "productos_condicion");

            migrationBuilder.DropTable(
                name: "productos_excluidos");

            migrationBuilder.DropTable(
                name: "productos_relacionados");

            migrationBuilder.DropTable(
                name: "ProductosQuejas");

            migrationBuilder.DropTable(
                name: "promociones_compatibles");

            migrationBuilder.DropTable(
                name: "Propuestas");

            migrationBuilder.DropTable(
                name: "RegistroItems");

            migrationBuilder.DropTable(
                name: "Rel_Categoria_Producto_Tipo_Producto");

            migrationBuilder.DropTable(
                name: "Rel_Imagen_Producto_Visita");

            migrationBuilder.DropTable(
                name: "Rel_servicio_producto");

            migrationBuilder.DropTable(
                name: "rel_tecnico_visita");

            migrationBuilder.DropTable(
                name: "Servicio_Troubleshooting");

            migrationBuilder.DropTable(
                name: "Tecnicos_Actividad");

            migrationBuilder.DropTable(
                name: "Tecnicos_Cobertura");

            migrationBuilder.DropTable(
                name: "Tecnicos_Producto");

            migrationBuilder.DropTable(
                name: "tipos_comprobantes");

            migrationBuilder.DropTable(
                name: "TokenItems");

            migrationBuilder.DropTable(
                name: "Vendedores");

            migrationBuilder.DropTable(
                name: "condiones_comerciales_sucursal");

            migrationBuilder.DropTable(
                name: "cat_msi");

            migrationBuilder.DropTable(
                name: "cat_beneficios");

            migrationBuilder.DropTable(
                name: "Cat_Accesorios");

            migrationBuilder.DropTable(
                name: "Cat_Lista_Precios");

            migrationBuilder.DropTable(
                name: "Cat_Productos_Preguntas_Troubleshooting");

            migrationBuilder.DropTable(
                name: "Check_List_Categoria_Producto");

            migrationBuilder.DropTable(
                name: "Producto_Check_List_Respuestas");

            migrationBuilder.DropTable(
                name: "Cat_Estatus_Compra");

            migrationBuilder.DropTable(
                name: "Cotizaciones");

            migrationBuilder.DropTable(
                name: "cat_tipo_entidades");

            migrationBuilder.DropTable(
                name: "Rel_servicio_Refaccion");

            migrationBuilder.DropTable(
                name: "Prediagnostico");

            migrationBuilder.DropTable(
                name: "Cat_Producto");

            migrationBuilder.DropTable(
                name: "promocion");

            migrationBuilder.DropTable(
                name: "Quejas");

            migrationBuilder.DropTable(
                name: "Cat_Productos_Estatus_Troubleshooting");

            migrationBuilder.DropTable(
                name: "Cat_Area_Cobertura");

            migrationBuilder.DropTable(
                name: "Cat_Sucursales");

            migrationBuilder.DropTable(
                name: "Cat_Productos");

            migrationBuilder.DropTable(
                name: "Visita");

            migrationBuilder.DropTable(
                name: "cat_tipo_condicion");

            migrationBuilder.DropTable(
                name: "cat_tipos_herencia");

            migrationBuilder.DropTable(
                name: "Canales");

            migrationBuilder.DropTable(
                name: "TipoQueja");

            migrationBuilder.DropTable(
                name: "Cat_Cuentas");

            migrationBuilder.DropTable(
                name: "Cat_SubLinea_Producto");

            migrationBuilder.DropTable(
                name: "Tecnicos");

            migrationBuilder.DropTable(
                name: "Servicio");

            migrationBuilder.DropTable(
                name: "Cat_canales");

            migrationBuilder.DropTable(
                name: "Cat_Linea_Producto");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Cat_Tecnicos_Tipo");

            migrationBuilder.DropTable(
                name: "Cat_Categoria_Servicio");

            migrationBuilder.DropTable(
                name: "Cat_estatus_servicio");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Cat_solicitado_por");

            migrationBuilder.DropTable(
                name: "Cat_solicitud_via");

            migrationBuilder.DropTable(
                name: "Sub_cat_tipo_servicio");

            migrationBuilder.DropTable(
                name: "Cat_Categoria_Producto");

            migrationBuilder.DropTable(
                name: "Cat_Aplicaciones");

            migrationBuilder.DropTable(
                name: "Cat_Roles");

            migrationBuilder.DropTable(
                name: "Cat_Municipio");

            migrationBuilder.DropTable(
                name: "Cat_tipo_servicio");

            migrationBuilder.DropTable(
                name: "Cat_Estado");
        }
    }
}
