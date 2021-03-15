using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thor.Models
{
    public class Comanda
    {
        public int Id { get; set; }

        public string Nume_Firma { get; set; }

        public DateTime Data_Lansare { get; set; }

        public string Material { get; set; }

        public string Tip_Produs { get; set; }

        public string Imprimeu { get; set; }

        public string Dimensiuni { get; set; }

        public List<string> Cai_Documente { get; set; } //caile catre fisierele relevante comenzii

        public int Cantitate { get; set; }

        public decimal Pret_Unitar { get; set; }

        public string Observatii { get; set; }
    }
}
