using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thor.Models
{
    public class Produs
    {
        //DENUMIREA COLOANELOR DIN TABELUL SQL TREBUIE SA COINCIDA CU NUMELE DE AICI - Telefon - telefon

        public int Id { get; set; }

        public string Denumire_Produs { get; set; }

        public string Unitate_Masura { get; set;}

        public decimal Cantitate { get; set; }

        public decimal Pret_Cumparare { get; set; }

        public decimal Pret_Vanzare { get; set; }

        public decimal Valoare
        {
            get
            {
                return Pret_Vanzare * Cantitate;
            }
        }

        public decimal TVA { get; set; }
        
        public decimal Valoare_Cu_Tva
        {
            get
            {
                return Valoare + TVA;
            }
        }
    }
}
