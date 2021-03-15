using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Thor.Models;
using Thor.DataAccess;
using System.Configuration;

namespace Thor.Utility
{
    public static class FileHandling
    {
        //se cer caile continatoare si fisierele cu care se vrea a lucra, ele vor aparea asa: "nume.extensie"
        public static void CopiazaFisiere(string caleSursa, string caleTinta, string[] fisiere)
        {
            if (!System.IO.Directory.Exists(caleTinta))
            {
                System.IO.Directory.CreateDirectory(caleTinta);
            }

            if (fisiere != null)
                foreach (string s in fisiere)
                {
                    long l1;
                    long l2;

                    if (!File.Exists(caleSursa + @"\" + s))
                        return;
                    else
                    {
                        FileInfo f1 = new FileInfo(caleSursa + @"\" + s);
                        l1 = f1.Length;
                    }

                    if (!File.Exists(caleTinta + @"\" + s))
                    {
                        l2 = 0;
                    }
                    else
                    {
                        FileInfo f2 = new FileInfo(caleTinta + @"\" + s);
                        l2 = f2.Length;
                    }


                        
                    

                    if (l1 != l2)
                        System.IO.File.Copy(caleSursa + @"\" + s, caleTinta + @"\" + s, true);
                    
                }

        }
        //se cere calea intreaga, calea tinta, numele fisierului se determina din calea sursa intreaga
        public static void CopiazaFisier(string caleSursa, string caleTinta)
        {
            if (!System.IO.Directory.Exists(caleTinta))
            {
                System.IO.Directory.CreateDirectory(caleTinta);
            }
            
            long l1;
            long l2;

            if (!File.Exists(caleSursa))
                return;
            else
            {
                FileInfo f1 = new FileInfo(caleSursa);
                l1 = f1.Length;
            }

            if (!File.Exists(caleTinta))
            {
                l2 = 0;
            }
            else
            {
                FileInfo f2 = new FileInfo(caleTinta);
                l2 = f2.Length;
            }
            

            if (l1 != l2)
            {
                if (caleSursa != null && caleSursa != caleTinta + @"\" + Path.GetFileName(caleSursa))
                    System.IO.File.Copy(caleSursa, caleTinta + @"\" + Path.GetFileName(caleSursa), true);
            }

        }
        

      /*  public static List<string> IncarcaInformatieFisier(string cale)
        {
            if(!System.IO.File.Exists(cale))
            {
                return new List<string>();
            }

            return System.IO.File.ReadAllLines(cale).ToList();
        }*/

       /* public static List<Eveniment> ConvertesteInListaEvenimente(List<string> linii)
        {
            List<Eveniment> output = new List<Eveniment>();

            foreach(string linie in linii)
            {
                string[] coloane = linie.Split(separator, System.StringSplitOptions.None);

                Eveniment e = new Eveniment();

                e.Id = int.Parse(coloane[0]);
                e.Denumire = coloane[1];
                e.Data_Eveniment = DateTime.Parse(coloane[2]);
                e.Note = coloane[3];

            }
        }*/
    }
}
