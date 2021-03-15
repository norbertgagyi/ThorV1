using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thor.Models
{
    public class Eveniment
    {
        public int Id { get; set; }

        public string Stare { get; set; }

        public string Nume_Firma { get; set; }

        public string Denumire { get; set; } //convorbire telefonica, contract incheiat bla

        public string Tip { get; set; }

        public DateTime Data_Eveniment { get; set; } //data ori din trecut, ori pentru programarea acestui eveniment - suna pe x la ora n

        public List<string> Cai_Documente { get; set; } //caile catre fisierele relevante evenimentului

        public string Note { get; set; } //note despre eveniment
        

        public static bool operator ==(Eveniment a, Eveniment b)
        {
            if (!(b is Eveniment))
                return false;

            Eveniment other = b as Eveniment;

            if (a.Nume_Firma != other.Nume_Firma)
                return false;

            if (a.Denumire != other.Denumire)
                return false;

         //   if (a.Stare != other.Stare)
              //  return false;

            if (a.Tip != other.Tip)
                return false;
            
            if (a.Note != other.Note)
                return false;

            return true;
        }

        public static bool operator !=(Eveniment a, Eveniment b)
        {
            return !(a == b);
        }
    }
}
