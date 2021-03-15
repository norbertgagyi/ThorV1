using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thor.Models
{
    public class ContBancar
    {
        public int Id { get; set; }

        public string IBAN { get; set; }

        public string Banca { get; set; }

        public string Moneda_Cont { get; set; }
    }
}
