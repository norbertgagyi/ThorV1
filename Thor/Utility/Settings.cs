using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thor.DataAccess;
using System.IO;
using System.Configuration;
using System.Reflection;
using Thor.Models;

namespace Thor.Utility
{
    public static class Settings
    {

        private static string[] fisiere = { "FACTURI.dbf", "PARTENER.dbf", "BANCA.dbf", "CASA.dbf", "COMPENS.dbf", "MARFAFAC.dbf", "RECEPTII.dbf", "MARFAREC.dbf" };

        public static string CaleProgram { get; set; } = @"c:\@thor";
        
        public static string CaleDate
        {
            get
            {
                return CaleProgram + @"\data";
            }
        }
        
        public static string CaleFisierSetari
        {
            get
            {
                return CaleProgram + @"\" + "setari.txt";
            }
            //Assembly.GetExecutingAssembly().Location; PENTRU a detecta de unde se porneste programul
        }

        //FAC CHEIA STRING PENTRU CA AM INTRARI CU 2018s - 2018SURSA, DECI TREBUIE CITITA
        public static Dictionary<string, string> CaiFoldereDBF { get; set; }

        public static string ConnectionStringDBFAn(string an)
        {
            if (CaiFoldereDBF.ContainsKey(an))
                return @"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = " + CaiFoldereDBF[an] + "; Extended Properties = dBASE IV; User ID = Admin; Password =;";

            return @"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = " + CaleProgram + "; Extended Properties = dBASE IV; User ID = Admin; Password =;";
        }

        public static List<string> Ani_InUz
        {
            get
            {
                List<string> rez = new List<string>();

                foreach (KeyValuePair<string, string> k in CaiFoldereDBF)
                {
                    if (k.Key.All(char.IsDigit))
                        rez.Add(k.Key);
                }

                return rez;
            }
        }
        
        public static int An_Curent
        {
            get
            {
                return DateTime.Now.Year;
            }
        }
            
        public static DateTime Ultima_Actualizare_DBF { get; set; } //timpul zilei

        public static DateTime Urmatoare_Actualizare_DBF
        {
            get
            {
                return Ultima_Actualizare_DBF.AddMinutes(Interval_Actualizare);
            }
        }
        
        public static int Interval_Actualizare { get; set; } = 30; //in minute
        


        public static void Actualizeaza_DBF(List<string> ani)
        {
            CitesteSetari();

            for (int i = 0; i < ani.Count; i++)
            {
                if (CaiFoldereDBF.ContainsKey(ani[i] + "s") && CaiFoldereDBF.ContainsKey(ani[i]))
                {
                    FileHandling.CopiazaFisiere(CaiFoldereDBF[ani[i] + "s"], CaiFoldereDBF[ani[i]], fisiere);

                    Ultima_Actualizare_DBF = DateTime.Now;
                }
            }
            
        }

        public static void ScrieSetari()
        {
            GenereazaIerarhieFoldere();

            List<string> liniiSetari = new List<string>();
            
            liniiSetari.Add(Interval_Actualizare.ToString());

            if (CaiFoldereDBF.Count > 0)
            {
                foreach (KeyValuePair<string, string> k in CaiFoldereDBF)
                {
                    liniiSetari.Add(k.Key + "*" + k.Value + "*");
                }
            }      
            
            System.IO.File.WriteAllLines(CaleFisierSetari, liniiSetari);
        }

        public static void CitesteSetari()
        {
            CaiFoldereDBF = new Dictionary<string, string>();
            //IMPORTANT - PENTRU O MAI MARE PROBABILA EFICIENTA, SE SCRIE ANUL IN CURS PRIMUL IN LISTA
            if (System.IO.File.Exists(CaleFisierSetari))
            {
                string[] linii = File.ReadAllLines(CaleFisierSetari);

                if (linii.Length > 0)
                {
                    Interval_Actualizare = int.Parse(linii[0]);

                    for (int i = 1; i < linii.Length; i++)
                    {
                        CaiFoldereDBF.Add(linii[i].Split("*".ToCharArray())[0], linii[i].Split("*".ToCharArray())[1]);
                    }
                }
            }
            else
                ScrieSetari();
        }

        public static void GenereazaIerarhieFoldere()
        {
            if (!Directory.Exists(CaleProgram))
                Directory.CreateDirectory(CaleProgram);

            if (!Directory.Exists(CaleDate))
                Directory.CreateDirectory(CaleDate);
        }

        #region cai

        private static string CaleFisierInfoFirma(string caleFirma)
        {
            return caleFirma + @"\" + "info.txt";
        }

        private static string CaleFisierInfoEveniment(string caleEveniment)
        {
            return caleEveniment + @"\" + "info.txt";
        }

        private static string CaleDocumenteEveniment(string caleEveniment)
        {
            return caleEveniment + @"\" + "Documente";
        }

        private static string CaleEvenimenteFirma(string caleFirma)
        {
            return caleFirma + @"\" + "Evenimente";
        }

        private static bool Firma_NotificariActive(string caleFirma)
        {
            if (File.Exists(CaleFisierInfoFirma(caleFirma)))
            {
                string[] linii = File.ReadAllLines(CaleFisierInfoFirma(caleFirma));

                if (linii[1] == "true")
                    return true;
            }

            return false;
        }

        #endregion

        public static List<Eveniment> Evenimente_NerezolvateInInterval(DateTime dataInceput, int intervalZile)
        {
            //pot fi toate, asa am dat acuma
            return Evenimente_FirmeNotificariActive().FindAll(x => x.Stare == "Nerezolvat" && (x.Data_Eveniment.Date <= dataInceput.AddDays(intervalZile).Date));
        }

        public static List<Eveniment> Evenimente_Toate()
        {
            List<Eveniment> evenimente = new List<Eveniment>();

            if (Directory.Exists(CaleDate))
                foreach (string dr in Directory.GetDirectories(CaleDate))
                {
                    evenimente.AddRange(EvenimenteFirma(dr));
                }

            return evenimente.OrderBy(x => x.Data_Eveniment).ToList();
        }

        public static List<Eveniment> Evenimente_FirmeNotificariActive()
        {
            List<Eveniment> evenimente = new List<Eveniment>();

            if (Directory.Exists(CaleDate))
                foreach (string dr in Directory.GetDirectories(CaleDate))
                {
                    if (Firma_NotificariActive(dr))
                        evenimente.AddRange(EvenimenteFirma(dr));
                }

            return evenimente.OrderBy(x => x.Data_Eveniment).ToList();
        }

        public static List<Eveniment> EvenimenteFirma(string caleFirma)
        {
            List<Eveniment> evenimente = new List<Eveniment>();

            if (Directory.Exists(CaleEvenimenteFirma(caleFirma)))
                foreach (string dr in Directory.GetDirectories(CaleEvenimenteFirma(caleFirma)))
                {
                    evenimente.Add(Eveniment(dr));
                }

            return evenimente.OrderBy(x => x.Data_Eveniment).ToList();
        }

        public static Eveniment Eveniment(string caleEveniment)
        {
            Eveniment e = new Eveniment();

            if (File.Exists(CaleFisierInfoEveniment(caleEveniment)))
            {
                string[] linii = File.ReadAllLines(CaleFisierInfoEveniment(caleEveniment));

                e.Id = int.Parse(linii[0]);
                e.Stare = linii[1];
                e.Nume_Firma = linii[2];
                e.Denumire = linii[3];
                e.Tip = linii[4];
                e.Data_Eveniment = DateTime.Parse(linii[5]);

                e.Note = "";

                for (int i = 6; i < linii.Length; i++)
                {
                    e.Note += linii[i];
                    e.Note += "\r\n";
                }
            }

            e.Cai_Documente = new List<string>();

            e.Cai_Documente = Directory.GetFiles(CaleDocumenteEveniment(caleEveniment)).ToList();

            return e;
        }


    }
}
