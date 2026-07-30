using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegradorSAP.Data.Models
{
    public class MaterialesSapViewModel
    {
		public string Codigo { get; set; }
        public string Producto { get; set; }
        public string Unidad { get; set; }
        public string BodegaCodigo { get; set; }
        public string Bodega { get; set; }
        public string Grupo { get; set; }
        public string CatalogoFabricante { get; set; }
        public string Fabricante { get; set; }
        public string UnidadInventario { get; set; }
        public string UnidadCompra { get; set; }
        public decimal UltimoPrecioCmp { get; set; }
        public decimal Stock { get; set; }
        public decimal PMP { get; set; }
        public decimal Valorizado { get; set; }



    }
}
