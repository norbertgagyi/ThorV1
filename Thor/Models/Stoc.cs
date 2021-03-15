using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thor.Models
{
    public class Stoc
    {
        public int Id { get; set; }

        public string Denumire_Produs { get; set; }

        public decimal Pret_Stoc { get; set; }

        public decimal Cantitate { get; set; }

        public string Unitate_Masura { get; set; }

    }
}
