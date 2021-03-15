using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Thor.Models
{
    public class Factura
    {

        //DENUMIREA COLOANELOR DIN TABELUL SQL TREBUIE SA COINCIDA CU NUMELE DE AICI - Telefon - telefon

        public int Id { get; set; }
        
        public string Seria { get; set; }

        public string Numar_Document { get; set; }

        public string Nume_Firma { get; set; }

        public string CUI_Firma { get; set; }

        public string Tip_Factura { get; set; } //furnizor, client

        public DateTime Data_Emitere { get; set; }

        public DateTime Data_Scadenta { get; set; }

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

        public List<Produs> Produse { get; set; }
    }
}
