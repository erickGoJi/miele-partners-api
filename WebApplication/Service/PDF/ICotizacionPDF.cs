using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Service
{
    public interface ICotizacionPDF
    {
        string CrearDocumento(long id, bool cliente);
    }
}
