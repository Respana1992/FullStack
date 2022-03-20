using System.Collections.Generic;

namespace Persistencia.DapperConexion.Paginacion
{
    public class PaginacionModel
    {
        public List<IDictionary<string, object>> ListaRecords {get;set;} 
        public int NueroPaginas {get;set;}
        public int TotalRecords {get;set;}
    }
}