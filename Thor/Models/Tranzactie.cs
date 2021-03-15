using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thor.DataAccess;
using Thor.Models;

namespace Thor.Models
{
    public class Tranzactie
    {
        public int Id { get; set; }

        public string Tip_Operatiune { get; set; }

        public string Metoda { get; set; }

        public string Seria { get; set; }

        public string Numar_Document { get; set; }

        public DateTime Data_Tranzactie { get; set; }

        public string Seria_Document_Corespunzator { get; set; }

        public string Numar_Document_Corespunzator { get; set; }

        public DateTime Data_Document_Corespunzator { get; set; }

        public string Obiect_Tranzactie { get; set; }

        public decimal Valoare { get; set; }

        public decimal TVA { get; set; }

        public decimal Valoare_Cu_TVA
        {
            get
            {
                return Valoare + TVA;
            }
        }

        public string Moneda { get; set; }

        public decimal Curs { get; set; }
        
    }
}
