using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thor.DataAccess;
using Thor.Utility;
using System.IO;

namespace Thor.Models
{
    public class Firma
    {
        //DENUMIREA COLOANELOR DIN TABELUL SQL TREBUIE SA COINCIDA CU NUMELE DE AICI - Telefon - telefon

        public int Id { get; set; } //id de lista de firme

        public string Nume { get; set; } // numele firmei

        public string Registrul_Comertului { get; set; } //registrul comertului

        public string CUI { get; set; } //cod unic de inregistrare

        public string Email { get; set; } //email-ul firmei

        public string Telefon { get; set; } //telefonul firmei

        public string Fax { get; set; }

        public string Tara { get; set; }

        public string Judet { get; set; }

        public string Localitate { get; set; }

        public string Adresa { get; set; }

        public string Cod_Postal { get; set; }

        public bool Favorit { get; set; } = false;

        public bool Activ_An_Curent { get; set; } = false;

        public bool Afiseaza_Notificari { get; set; } = true;

        public bool Foloseste_Termen_Plata { get; set; } = false;

        public int Termen_Plata { get; set; } = 0; //zile

        public bool Foloseste_Plafon_Client { get; set; } = false;

        public decimal Plafon_Client { get; set; } = 0;

        public bool Foloseste_Plafon_Furnizor { get; set; } = false;

        public decimal Plafon_Furnizor { get; set; } = 0;
        

        public string Identificator
        {
            get
            {
                return (Nume + CUI).Replace(@">", "-").Replace(@"<", "-").Replace(@"\", "-").Replace(@"/", "-").Replace('"'.ToString(), "-").Replace(@"?", "-");
            }
        }

        public string Cale
        {
            get
            {
                return Settings.CaleDate + @"\" + Identificator;
            }
        }

        public string CaleFisierInfo
        {
            get
            {
                return Cale + @"\" + "info.txt";
            }
        }



        public string CaleEvenimente
        {
            get
            {
                return Cale + @"\" + "Evenimente";
            }
        }

        public string CaleDocumente
        {
            get
            {
                return Cale + @"\" + "Documente";
            }
        }

        public string CaleComenzi
        {
            get
            {
                return Cale + @"\" + "Comenzi";
            }
        }

        public string CaleComanda(int id)
        {
            return CaleComenzi + @"\" + "C" + id;
        }

        public string CaleFisierInfoComanda(int id)
        {
            return CaleComanda(id) + @"\" + "info.txt";
        }

        public string CaleFisierInfoComanda(string caleComanda)
        {
            return caleComanda + @"\" + "info.txt";
        }

        public string CaleDocumenteComanda(int id)
        {
            return CaleComanda(id) + @"\" + "Documente";
        }

        public string CaleDocumenteComanda(string caleComanda)
        {
            return caleComanda + @"\" + "Documente";
        }


        public string CaleEveniment(int id)
        {
            return CaleEvenimente + @"\" + "EV" + id;
        }

        public string CaleFisierInfoEveniment(int id)
        {
            return CaleEveniment(id) + @"\" + "info.txt";
        }

        public string CaleFisierInfoEveniment(string caleEveniment)
        {
            return caleEveniment + @"\" + "info.txt";
        }

        public string CaleDocumenteEveniment(int id)
        {
            return CaleEveniment(id) + @"\" + "Documente";
        }

        public string CaleDocumenteEveniment(string caleEveniment)
        {
            return caleEveniment + @"\" + "Documente";
        }


        public string CalePersoaneContact
        {
            get
            {
                return Cale + @"\" + "PersoaneContact";
            }
        }

        public string CalePersoanaContact(int id)
        {
            return CalePersoaneContact + @"\" + "P" + id;
        }

        public string CaleFisierInfoPersoana(int id)
        {
            return CalePersoanaContact(id) + @"\" + "info.txt";
        }

        public string CaleFisierInfoPersoana(string calePersoana)
        {
            return calePersoana + @"\" + "info.txt";
        }


        public string CaleConturiBancare
        {
            get
            {
                return Cale + @"\" + "ConturiBancare";
            }
        }

        public string CaleContBancar(int id)
        {
            return CaleConturiBancare + @"\" + "C" + id;
        }

        public string CaleFisierInfoCont(int id)
        {
            return CaleContBancar(id) + @"\" + "info.txt";
        }

        public string CaleFisierInfoCont(string caleCont)
        {
            return caleCont + @"\" + "info.txt";
        }




        public List<ContBancar> Conturi_Bancare
        {
            get
            {
                List<ContBancar> rezultat = new List<ContBancar>();

                if (System.IO.Directory.Exists(CaleConturiBancare))
                {
                    foreach (string cale in Directory.GetDirectories(CaleConturiBancare))
                    {
                        rezultat.Add(CitesteContBancar(cale));
                    }
                }

                return rezultat;
            }
            set
            {

            }
        }

        public List<Persoana> Persoane_De_Contact
        {
            get
            {
                List<Persoana> rezultat = new List<Persoana>();

                if (System.IO.Directory.Exists(CalePersoaneContact))
                {
                    foreach (string cale in Directory.GetDirectories(CalePersoaneContact))
                    {
                        rezultat.Add(CitestePersoana(cale));
                    }
                }

                return rezultat;
            }
            set
            {

            }
        }

        public List<Comanda> Comenzi
        {
            get
            {
                List<Comanda> rezultat = new List<Comanda>();

                if(System.IO.Directory.Exists(CaleComenzi))
                {
                    foreach(string cale in Directory.GetDirectories(CaleComenzi))
                    {
                        rezultat.Add(CitesteComanda(cale));
                    }
                }

                return rezultat.OrderBy(x => x.Data_Lansare).ToList();

            } 
            set
            {

            }
        }

        public List<Eveniment> Evenimente
        {
            get
            {
                List<Eveniment> rezultat = new List<Eveniment>();

                if (System.IO.Directory.Exists(CaleEvenimente))
                {
                    foreach (string cale in Directory.GetDirectories(CaleEvenimente))
                    {
                        rezultat.Add(CitesteEveniment(cale));
                    }
                }

                return rezultat.OrderBy(x => x.Data_Eveniment).ToList();
            }
            set
            {

            }
        }



        public List<Factura> FacturiClient { get; set; }

        public List<Factura> FacturiClientPlatite(DateTime inceput, DateTime sfarsit)
        {
            List<Factura> facturiPerioada = FacturiClientPePerioada(inceput, sfarsit);

            List<Tranzactie> incasariPerioada = new List<Tranzactie>();
            List<Factura> facturiPlatite = new List<Factura>();

            //iau ultima factura de pe acea perioada, vad cand e scadenta, daca dupa scadenta am incasat atata cat ar trebui, totul e ok
            if (facturiPerioada.Count > 0)
                incasariPerioada = IncasariPePerioada(inceput, facturiPerioada[facturiPerioada.Count - 1].Data_Scadenta.AddDays(Termen_Plata));
            else
                return facturiPlatite;


            decimal totalIncasariPerioada = 0m;

            if (incasariPerioada.Count > 0)
                foreach (Tranzactie t in incasariPerioada)
                {
                    totalIncasariPerioada += t.Valoare;
                }

            totalIncasariPerioada = totalIncasariPerioada - SoldInitialClient;

            decimal totalFacturi = 0m;

            for (int i = 0; i < facturiPerioada.Count; i++)
            {
                if (totalFacturi + facturiPerioada[i].Valoare <= totalIncasariPerioada)
                {
                    totalFacturi = totalFacturi + facturiPerioada[i].Valoare;
                    facturiPlatite.Add(facturiPerioada[i]);
                }
            }

            return facturiPlatite.OrderBy(x => x.Data_Emitere).ToList();
        }

        public List<Factura> FacturiClientNeplatite(DateTime inceput, DateTime sfarsit)
        {
            List<Factura> facturiPerioada = FacturiClientPePerioada(inceput, sfarsit);

            List<Tranzactie> incasariPerioada = new List<Tranzactie>();
            List<Factura> facturiPlatite = new List<Factura>();

            //iau ultima factura de pe acea perioada, vad cand e scadenta, daca dupa scadenta am incasat atata cat ar trebui, totul e ok
            if (facturiPerioada.Count > 0)
                incasariPerioada = IncasariPePerioada(inceput, facturiPerioada[facturiPerioada.Count - 1].Data_Scadenta.AddDays(Termen_Plata));
            else
                return facturiPlatite;


            decimal totalIncasariPerioada = 0m;

            if (incasariPerioada.Count > 0)
                foreach (Tranzactie t in incasariPerioada)
                {
                    totalIncasariPerioada += t.Valoare;
                }

            totalIncasariPerioada = totalIncasariPerioada - SoldInitialClient;

            decimal totalFacturi = 0m;

            for (int i = 0; i < facturiPerioada.Count; i++)
            {
                if (totalFacturi + facturiPerioada[i].Valoare <= totalIncasariPerioada)
                {
                    totalFacturi = totalFacturi + facturiPerioada[i].Valoare;
                    facturiPlatite.Add(facturiPerioada[i]);
                }
            }

            return facturiPerioada.Except(facturiPlatite).OrderBy(x => x.Data_Emitere).ToList();
        }



        public List<Factura> FacturiFurnizor { get; set; }

        public List<Factura> FacturiFurnizorPlatite(DateTime inceput, DateTime sfarsit)
        {
            List<Factura> facturiPerioada = FacturiFurnizorPePerioada(inceput, sfarsit);

            List<Tranzactie> platiPerioada = new List<Tranzactie>();
            List<Factura> facturiPlatite = new List<Factura>();

            //iau ultima factura de pe acea perioada, vad cand e scadenta, daca dupa scadenta am incasat atata cat ar trebui, totul e ok
            if (facturiPerioada.Count > 0)
                platiPerioada = PlatiPePerioada(inceput, facturiPerioada[facturiPerioada.Count - 1].Data_Scadenta);
            else
                return facturiPlatite;


            decimal totalPlatiPerioada = 0m;

            if (platiPerioada.Count > 0)
                foreach (Tranzactie t in platiPerioada)
                {
                    totalPlatiPerioada += t.Valoare;
                }

            totalPlatiPerioada = totalPlatiPerioada - SoldInitialFurnizor;

            decimal totalFacturi = 0m;

            for (int i = 0; i < facturiPerioada.Count; i++)
            {
                if (totalFacturi + facturiPerioada[i].Valoare <= totalPlatiPerioada)
                {
                    totalFacturi = totalFacturi + facturiPerioada[i].Valoare;
                    facturiPlatite.Add(facturiPerioada[i]);
                }
            }

            return facturiPlatite.OrderBy(x => x.Data_Emitere).ToList();
        }

        public List<Factura> FacturiFurnizorNeplatite(DateTime inceput, DateTime sfarsit)
        {
            List<Factura> facturiPerioada = FacturiFurnizorPePerioada(inceput, sfarsit);

            List<Tranzactie> platiPerioada = new List<Tranzactie>();
            List<Factura> facturiPlatite = new List<Factura>();

            //iau ultima factura de pe acea perioada, vad cand e scadenta, daca dupa scadenta am incasat atata cat ar trebui, totul e ok
            if (facturiPerioada.Count > 0)
                platiPerioada = PlatiPePerioada(inceput, facturiPerioada[facturiPerioada.Count - 1].Data_Scadenta.AddDays(Termen_Plata));
            else
                return facturiPlatite;


            decimal totalPlatiPerioada = 0m;

            if (platiPerioada.Count > 0)
                foreach (Tranzactie t in platiPerioada)
                {
                    totalPlatiPerioada += t.Valoare;
                }

            totalPlatiPerioada = totalPlatiPerioada - SoldInitialFurnizor;

            decimal totalFacturi = 0m;

            for (int i = 0; i < facturiPerioada.Count; i++)
            {
                if (totalFacturi + facturiPerioada[i].Valoare <= totalPlatiPerioada)
                {
                    totalFacturi = totalFacturi + facturiPerioada[i].Valoare;
                    facturiPlatite.Add(facturiPerioada[i]);
                }
            }

            return facturiPerioada.Except(facturiPlatite).OrderBy(x => x.Data_Emitere).ToList();
        }



        public decimal RulajClientLuna(int luna, int an)
        {
            decimal rez = 0m;

            if (FacturiClient != null)
            {
                foreach (Factura f in FacturiClientPePerioada(new DateTime(an, luna, 1), new DateTime(an, luna, DateTime.DaysInMonth(an, luna))))
                    rez += f.Valoare_Cu_TVA;
            }

            return rez;
        }

        public int NumarFacturiClientLuna(int luna, int an)
        {
            int rez = 0;

            if (FacturiClient != null)
            {
                foreach (Factura f in FacturiClientPePerioada(new DateTime(an, luna, 1), new DateTime(an, luna, DateTime.DaysInMonth(an, luna))))
                    rez++;
            }

            return rez;
        }
        
        public decimal RulajFurnizorLuna(int luna, int an)
        {
            decimal rez = 0m;

            if (FacturiClient != null)
            {
                foreach (Factura f in FacturiFurnizorPePerioada(new DateTime(an, luna, 1), new DateTime(an, luna, DateTime.DaysInMonth(an, luna))))
                    rez += f.Valoare_Cu_TVA;
            }

            return rez;
        }

        public int NumarFacturiFurnizorLuna(int luna, int an)
        {
            int rez = 0;

            if (FacturiClient != null)
            {
                foreach (Factura f in FacturiFurnizorPePerioada(new DateTime(an, luna, 1), new DateTime(an, luna, DateTime.DaysInMonth(an, luna))))
                    rez++;
            }

            return rez;
        }



        public List<decimal> RulajClientLuniDinPerioada(DateTime inceput, DateTime sfarsit)
        {
            List<decimal> valoriPeLuni = new List<decimal>();

            DateTime cnt = inceput;
            
            while(cnt.Date <= sfarsit.Date)
            {
                valoriPeLuni.Add(RulajClientLuna(cnt.Month, cnt.Year));

                cnt = cnt.AddMonths(1);
            }

            return valoriPeLuni;
        }

        public List<decimal> IncasariClientLuniDinPerioada(DateTime inceput, DateTime sfarsit)
        {
            List<decimal> valoriPeLuni = new List<decimal>();

            DateTime cnt = inceput;

            while (cnt.Date <= sfarsit.Date)
            {
                valoriPeLuni.Add(IncasariLuna(cnt.Month, cnt.Year));

                cnt = cnt.AddMonths(1);
            }

            return valoriPeLuni;
        }

        public List<decimal> RulajFurnizorLuniDinPerioada(DateTime inceput, DateTime sfarsit)
        {
            List<decimal> valoriPeLuni = new List<decimal>();

            DateTime cnt = inceput;

            while (cnt.Date <= sfarsit.Date)
            {
                valoriPeLuni.Add(RulajFurnizorLuna(cnt.Month, cnt.Year));

                cnt = cnt.AddMonths(1);
            }

            return valoriPeLuni;
        }

        public List<decimal> PlatiFurnizorLuniDinPerioada(DateTime inceput, DateTime sfarsit)
        {
            List<decimal> valoriPeLuni = new List<decimal>();

            DateTime cnt = inceput;

            while (cnt.Date <= sfarsit.Date)
            {
                valoriPeLuni.Add(PlatiLuna(cnt.Month, cnt.Year));

                cnt = cnt.AddMonths(1);
            }

            return valoriPeLuni;
        }



        public List<int> NumarFacturiClientLuniDinPerioada(DateTime inceput, DateTime sfarsit)
        {
            List<int> valoriPeLuni = new List<int>();
            
            DateTime cnt = inceput;

            while (cnt.Date <= sfarsit.Date)
            {
                valoriPeLuni.Add(FacturiClientPePerioada(new DateTime(cnt.Year, cnt.Month, 1), new DateTime(cnt.Year, cnt.Month, DateTime.DaysInMonth(cnt.Year, cnt.Month))).Count);
                cnt = cnt.AddMonths(1);
            }

            return valoriPeLuni;
        }

        public List<int> NumarIncasariLuniDinPerioada(DateTime inceput, DateTime sfarsit)
        {
            List<int> valoriPeLuni = new List<int>();

            DateTime cnt = inceput;

            while (cnt.Date <= sfarsit.Date)
            {
                valoriPeLuni.Add(IncasariPePerioada(new DateTime(cnt.Year, cnt.Month, 1), new DateTime(cnt.Year, cnt.Month, DateTime.DaysInMonth(cnt.Year, cnt.Month))).Count);
                cnt = cnt.AddMonths(1);
            }

            return valoriPeLuni;
        }



        public List<int> NumarFacturiFurnizorLuniDinPerioada(DateTime inceput, DateTime sfarsit)
        {
            List<int> valoriPeLuni = new List<int>();

            DateTime cnt = inceput;

            while (cnt.Date <= sfarsit.Date)
            {
                valoriPeLuni.Add(FacturiFurnizorPePerioada(new DateTime(cnt.Year, cnt.Month, 1), new DateTime(cnt.Year, cnt.Month, DateTime.DaysInMonth(cnt.Year, cnt.Month))).Count);
                cnt = cnt.AddMonths(1);
            }

            return valoriPeLuni;
        }

        public List<int> NumarPlatiLuniDinPerioada(DateTime inceput, DateTime sfarsit)
        {
            List<int> valoriPeLuni = new List<int>();

            DateTime cnt = inceput;

            while (cnt.Date <= sfarsit.Date)
            {
                valoriPeLuni.Add(PlatiPePerioada(new DateTime(cnt.Year, cnt.Month, 1), new DateTime(cnt.Year, cnt.Month, DateTime.DaysInMonth(cnt.Year, cnt.Month))).Count);
                cnt = cnt.AddMonths(1);
            }

            return valoriPeLuni;
        }


        #region analiza general

        public decimal TotalFacturiClientPerioada(DateTime inceput, DateTime sfarsit)
        {
            decimal rezultat = 0;

            if (FacturiClient != null)
            {
                foreach (Factura f in FacturiClient)
                {
                    if (f.Data_Emitere >= inceput && f.Data_Emitere <= sfarsit)
                    {
                        rezultat += f.Valoare_Cu_TVA;
                    }
                }
            }

            return rezultat;
        }

        public decimal TotalFacturiClientPerioadaFaraTVA(DateTime inceput, DateTime sfarsit)
        {
            decimal rezultat = 0;

            if(FacturiClient != null)
            {
                foreach(Factura f in FacturiClient)
                {
                    if(f.Data_Emitere >= inceput && f.Data_Emitere <= sfarsit)
                    {
                        rezultat += f.Valoare;
                    }
                }
            }

            return rezultat;
        }

        public decimal TotalTVAFacturiClientPerioada(DateTime inceput, DateTime sfarsit)
        {
            decimal rezultat = 0;

            if (FacturiClient != null)
            {
                foreach (Factura f in FacturiClient)
                {
                    if (f.Data_Emitere >= inceput && f.Data_Emitere <= sfarsit)
                    {
                        rezultat += f.TVA;
                    }
                }
            }

            return rezultat;
        }

        public int NumarFacturiClientPerioada(DateTime inceput, DateTime sfarsit)
        {
            if (FacturiClient != null)
                return FacturiClient.FindAll(x => x.Data_Emitere >= inceput && x.Data_Emitere <= sfarsit).Count();
            else
                return 0;
        }

        public decimal TVAMediuClient(DateTime inceput, DateTime sfarsit)
        {
            decimal rez = 0;

            if (TotalFacturiClientPerioadaFaraTVA(inceput, sfarsit) != 0)
                rez = (100 * TotalTVAFacturiClientPerioada(inceput, sfarsit)) / TotalFacturiClientPerioadaFaraTVA(inceput, sfarsit);

            return rez;
        }



        public decimal TotalFacturiFurnizorPerioada(DateTime inceput, DateTime sfarsit)
        {
            decimal rezultat = 0;

            if (FacturiFurnizor != null)
            {
                foreach (Factura f in FacturiFurnizor)
                {
                    if (f.Data_Emitere >= inceput && f.Data_Emitere <= sfarsit)
                    {
                        rezultat += f.Valoare_Cu_TVA;
                    }
                }
            }

            return rezultat;
        }
        
        public decimal TotalFacturiFurnizorPerioadaFaraTVA(DateTime inceput, DateTime sfarsit)
        {
            decimal rezultat = 0;

            if (FacturiFurnizor != null)
            {
                foreach (Factura f in FacturiFurnizor)
                {
                    if (f.Data_Emitere >= inceput && f.Data_Emitere <= sfarsit)
                    {
                        rezultat += f.Valoare;
                    }
                }
            }

            return rezultat;
        }

        public decimal TotalTVAFacturiFurnizorPerioada(DateTime inceput, DateTime sfarsit)
        {
            decimal rezultat = 0;

            if (FacturiFurnizor != null)
            {
                foreach (Factura f in FacturiFurnizor)
                {
                    if (f.Data_Emitere >= inceput && f.Data_Emitere <= sfarsit)
                    {
                        rezultat += f.TVA;
                    }
                }
            }

            return rezultat;
        }

        public int NumarFacturiFurnizorPerioada(DateTime inceput, DateTime sfarsit)
        {
            if (FacturiFurnizor != null)
                return FacturiFurnizor.FindAll(x => x.Data_Emitere >= inceput && x.Data_Emitere <= sfarsit).Count();
            else
                return 0;
        }

        public decimal TVAMediuFurnizor(DateTime inceput, DateTime sfarsit)
        {
            decimal rez = 0;

            if (TotalFacturiFurnizorPerioadaFaraTVA(inceput, sfarsit) != 0)
                rez = (100 * TotalTVAFacturiFurnizorPerioada(inceput, sfarsit)) / TotalFacturiFurnizorPerioadaFaraTVA(inceput, sfarsit);

            return rez;
        }



        #endregion





        public decimal SoldInitialClient { get; set; } = 0;

        public decimal SoldInitialFurnizor { get; set; } = 0;



        public decimal RulajClient
        {
            get
            {
                decimal rezultat = 0;

                if (FacturiClient != null)
                {
                    foreach (Factura f in FacturiClient)
                    {
                        rezultat += f.Valoare_Cu_TVA;
                    }
                }

                return rezultat;
            }
        }

        public decimal RulajFurnizor
        {
            get
            {
                decimal rezultat = 0;

                if (FacturiFurnizor != null)
                {
                    foreach (Factura f in FacturiFurnizor)
                    {
                        rezultat += f.Valoare_Cu_TVA;
                    }
                }

                return rezultat;
            }
        }




        public List<Tranzactie> Tranzactii { get; set; }

        public List<Tranzactie> TranzactiiPePerioada(DateTime inceput, DateTime sfarsit)
        {
            List<Tranzactie> rezultat = new List<Tranzactie>();

            if (Tranzactii != null)
                foreach (Tranzactie t in Tranzactii)
                {
                    if (t.Data_Tranzactie >= inceput && t.Data_Tranzactie <= sfarsit)
                        rezultat.Add(t);
                }

            return rezultat.OrderBy(x => x.Data_Tranzactie).ToList();
        }
        


        public List<Tranzactie> IncasariPePerioada(DateTime inceput, DateTime sfarsit)
        {
            List<Tranzactie> rezultat = new List<Tranzactie>();

            if (Tranzactii != null)
                foreach (Tranzactie t in Tranzactii)
                {
                    if (t.Tip_Operatiune == "I")
                        if (t.Data_Tranzactie >= inceput && t.Data_Tranzactie <= sfarsit)
                            rezultat.Add(t);
                }

            return rezultat.OrderBy(x => x.Data_Tranzactie).ToList();
        }
        
        public List<Tranzactie> PlatiPePerioada(DateTime inceput, DateTime sfarsit)
        {
            List<Tranzactie> rezultat = new List<Tranzactie>();

            if (Tranzactii != null)
                foreach (Tranzactie t in Tranzactii)
                {
                    if (t.Tip_Operatiune == "P")
                        if (t.Data_Tranzactie >= inceput && t.Data_Tranzactie <= sfarsit)
                            rezultat.Add(t);
                }


            return rezultat.OrderBy(x => x.Data_Tranzactie).ToList();
        }



        public decimal IncasariLuna(int luna, int an)
        {
            decimal rez = 0m;

            if (FacturiClient != null)
            {
                foreach (Tranzactie t in IncasariPePerioada(new DateTime(an, luna, 1), new DateTime(an, luna, DateTime.DaysInMonth(an, luna))))
                    rez += t.Valoare_Cu_TVA;
            }

            return rez;
        }

        public decimal PlatiLuna(int luna, int an)
        {
            decimal rez = 0m;

            if (FacturiClient != null)
            {
                foreach (Tranzactie t in PlatiPePerioada(new DateTime(an, luna, 1), new DateTime(an, luna, DateTime.DaysInMonth(an, luna))))
                    rez += t.Valoare_Cu_TVA;
            }

            return rez;
        }



        public List<Factura> FacturiClientPePerioada(DateTime inceput, DateTime sfarsit)
        {
            List<Factura> rezultat = new List<Factura>();

            if (FacturiClient != null)
                foreach (Factura f in FacturiClient)
                {
                    if (f.Data_Emitere >= inceput && f.Data_Emitere <= sfarsit)
                        rezultat.Add(f);
                }
            

            return rezultat.OrderBy(x => x.Data_Emitere).ToList();
        }

        public List<Factura> FacturiFurnizorPePerioada(DateTime inceput, DateTime sfarsit)
        {
            List<Factura> rezultat = new List<Factura>();

            if (FacturiFurnizor != null)
                foreach (Factura f in FacturiFurnizor)
                {
                    if (f.Data_Emitere >= inceput && f.Data_Emitere <= sfarsit)
                        rezultat.Add(f);
                }

            return rezultat.OrderBy(x => x.Data_Emitere).ToList();
        }



        public List<Factura> FacturiClientScadenteInPerioada(DateTime inceput, DateTime sfarsit)
        {
            List<Factura> rezultat = new List<Factura>();

            if (FacturiClient != null)
                foreach (Factura f in FacturiClient)
                {
                    if (f.Data_Scadenta >= inceput && f.Data_Scadenta <= sfarsit)
                        rezultat.Add(f);
                }


            return rezultat.OrderBy(x => x.Data_Emitere).ToList();
        }

        public List<Factura> FacturiFurnizorScadenteInPerioada(DateTime inceput, DateTime sfarsit)
        {
            List<Factura> rezultat = new List<Factura>();

            if (FacturiFurnizor != null)
                foreach (Factura f in FacturiFurnizor)
                {
                    if (f.Data_Scadenta >= inceput && f.Data_Scadenta <= sfarsit)
                        rezultat.Add(f);
                }

            return rezultat.OrderBy(x => x.Data_Emitere).ToList();
        }



        public List<string> Cai_Documente
        {
            get
            {
                try
                {
                    List<string> rezultat = new List<string>();

                    rezultat = Directory.GetFiles(CaleDocumente).ToList();

                    return rezultat;
                }
                catch
                {
                    return new List<string>();
                }
            }
            set
            {

            }
        }

        
        public List<Eveniment> EvenimentePePerioada(DateTime inceput, DateTime sfarsit)
        {
            List<Eveniment> rezultat = new List<Eveniment>();

            if (Evenimente.Count > 0)
                foreach (Eveniment eve in Evenimente)
                {
                    if (eve.Data_Eveniment >= inceput && eve.Data_Eveniment <= sfarsit)
                        rezultat.Add(eve);
                }
            

            return rezultat;
        }



        public void ScrieInformatiiAditionale()
        {
            if (!System.IO.Directory.Exists(Cale))
            {
                System.IO.Directory.CreateDirectory(Cale);
            }

            if(!System.IO.Directory.Exists(CaleDocumente))
            {
                System.IO.Directory.CreateDirectory(CaleDocumente);
            }

            List<string> linii = new List<string>();

            linii.Add(Favorit ? "true" : "false");
            linii.Add(Afiseaza_Notificari ? "true" : "false");
            


            linii.Add(SoldInitialClient.ToString());
            linii.Add(SoldInitialFurnizor.ToString());

            linii.Add(Foloseste_Termen_Plata ? "true" : "false");
            linii.Add(Termen_Plata.ToString());

            linii.Add(Foloseste_Plafon_Client ? "true" : "false");
            linii.Add(Plafon_Client.ToString());

            linii.Add(Foloseste_Plafon_Furnizor ? "true" : "false");
            linii.Add(Plafon_Furnizor.ToString());

            linii.Add(Activ_An_Curent ? "true" : "false");


            System.IO.File.WriteAllLines(CaleFisierInfo, linii);
        }

        public void CitesteInformatiiAditionale()
        {
            if (File.Exists(CaleFisierInfo))
            {
                string[] linii = File.ReadAllLines(CaleFisierInfo);

                try
                {
                    if (linii[0] != "" || linii[0] != null)
                        Favorit = linii[0] == "true" ? true : false;

                    if (linii[1] != "" || linii[1] != null)
                        Afiseaza_Notificari = linii[1] == "true" ? true : false;

                    if (linii[2] != "" || linii[2] != null)
                        SoldInitialClient = decimal.Parse(linii[2]);

                    if (linii[3] != "" || linii[3] != null)
                        SoldInitialFurnizor = decimal.Parse(linii[3]);

                    if (linii[4] != "" || linii[4] != null)
                        Foloseste_Termen_Plata = linii[4] == "true" ? true : false;

                    if (linii[5] != "" || linii[5] != null)
                        Termen_Plata = Int32.Parse(linii[5]);

                    if (linii[6] != "" || linii[6] != null)
                        Foloseste_Plafon_Client = linii[6] == "true" ? true : false;

                    if (linii[7] != "" || linii[7] != null)
                        Plafon_Client = decimal.Parse(linii[7]);

                    if (linii[8] != "" || linii[8] != null)
                        Foloseste_Plafon_Furnizor = linii[8] == "true" ? true : false;

                    if (linii[9] != "" || linii[9] != null)
                        Plafon_Furnizor = decimal.Parse(linii[9]);

                    if(linii[10] != "" || linii[10] != null)
                        Activ_An_Curent = linii[10] == "true" ? true : false;
                }
                catch
                {
                    ScrieInformatiiAditionale();
                }
            }
        }


        public Eveniment CitesteEveniment(string caleEveniment)
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
                    if (i < linii.Length - 1)
                        e.Note += "\r\n";
                }
            }

            e.Cai_Documente = new List<string>();

            e.Cai_Documente = Directory.GetFiles(CaleDocumenteEveniment(caleEveniment)).ToList();

            return e;
        }

        public int IdUltimaComandaInregistrata()
        {
            int rez = 0;

            if (Comenzi.Count > 0)
            {
                rez = Comenzi.OrderByDescending(x => x.Id).ToList()[0].Id;
            }

            return rez;
        }

        public int IdUltimaPersoanaInregistrata()
        {
            int rez = 0;

            if(Persoane_De_Contact.Count > 0)
            {
                rez = Persoane_De_Contact.OrderByDescending(x => x.Id).ToList()[0].Id;
            }

            return rez;
        }

        public Eveniment SalveazaEveniment(Eveniment eveniment, int idEve)
        {
            if (!System.IO.Directory.Exists(CaleEvenimente))
            {
                System.IO.Directory.CreateDirectory(CaleEvenimente);
            }

            int GetId = idEve;

            if (idEve == -1)
            {
                if (Settings.Evenimente_Toate().Count > 0)
                {
                    GetId = Settings.Evenimente_Toate().OrderByDescending(x => x.Id).ToList()[0].Id + 1;
                }
                else
                    GetId = 1;
            }
            //idul eveniment pe care vreau sa o salvez \ -1 inseamna ca vreau sa salvez una noua, daca id-ul e dat, va overwritea evenimentul cu acel id


            System.IO.Directory.CreateDirectory(CaleEveniment(GetId));
            System.IO.Directory.CreateDirectory(CaleDocumenteEveniment(GetId));

            List<string> linii = new List<string>();

            eveniment.Id = GetId;

            linii.Add(eveniment.Id.ToString());
            linii.Add(eveniment.Stare);
            linii.Add(Nume);
            linii.Add(eveniment.Denumire);
            linii.Add(eveniment.Tip);
            linii.Add(eveniment.Data_Eveniment.ToShortDateString());
            linii.Add(eveniment.Note);

            System.IO.File.WriteAllLines(CaleFisierInfoEveniment(GetId), linii);



            if (eveniment.Cai_Documente != null)
                foreach (string s in eveniment.Cai_Documente)
                {
                    FileHandling.CopiazaFisier(s, CaleDocumenteEveniment(GetId));
                }
           return eveniment;
        }

        public Comanda CitesteComanda(string caleComanda)
        {
            Comanda c = new Comanda();

            if (File.Exists(CaleFisierInfoComanda(caleComanda)))
            {
                string[] linii = File.ReadAllLines(CaleFisierInfoComanda(caleComanda));

                c.Id = int.Parse(linii[0]);
                c.Nume_Firma = linii[1];
                c.Material = linii[2];
                c.Tip_Produs = linii[3];
                c.Imprimeu = linii[4];
                c.Cantitate = int.Parse(linii[5]);
                c.Pret_Unitar = decimal.Parse(linii[6]);
                c.Data_Lansare = DateTime.Parse(linii[7]);
                c.Dimensiuni = linii[8];

                c.Observatii = "";

                for (int i = 9; i < linii.Length; i++)
                {
                    c.Observatii += linii[i];
                    if (i < linii.Length - 1)
                        c.Observatii += "\r\n";
                }
            }

            c.Cai_Documente = new List<string>();

            c.Cai_Documente = Directory.GetFiles(CaleDocumenteComanda(caleComanda)).ToList();

            return c;
        }

        public Comanda SalveazaComanda(Comanda comanda, int idCom)
        {
            if (!System.IO.Directory.Exists(CaleComenzi))
            {
                System.IO.Directory.CreateDirectory(CaleComenzi);
            }

            int GetId = idCom;

            if (idCom == -1)
                GetId = IdUltimaComandaInregistrata() + 1;
            //idul eveniment pe care vreau sa o salvez \ -1 inseamna ca vreau sa salvez una noua, daca id-ul e dat, va overwritea evenimentul cu acel id
            

            System.IO.Directory.CreateDirectory(CaleComanda(GetId));
            System.IO.Directory.CreateDirectory(CaleDocumenteComanda(GetId));

            List<string> linii = new List<string>();

            comanda.Id = GetId;

            linii.Add(comanda.Id.ToString());
            linii.Add(Nume);
            linii.Add(comanda.Material);
            linii.Add(comanda.Tip_Produs);
            linii.Add(comanda.Imprimeu);
            linii.Add(comanda.Cantitate.ToString());
            linii.Add(comanda.Pret_Unitar.ToString());
            linii.Add(comanda.Data_Lansare.ToShortDateString());
            linii.Add(comanda.Dimensiuni);
            linii.Add(comanda.Observatii);
            

            System.IO.File.WriteAllLines(CaleFisierInfoComanda(GetId), linii);



            if (comanda.Cai_Documente != null)
                foreach (string s in comanda.Cai_Documente)
                {
                    FileHandling.CopiazaFisier(s, CaleDocumenteComanda(GetId));
                }
            return comanda;
        }


        public Persoana CitestePersoana(string calePersoana)
        {
            Persoana pers = new Persoana();

            if (File.Exists(CaleFisierInfoPersoana(calePersoana)))
            {
                string[] linii = File.ReadAllLines(CaleFisierInfoPersoana(calePersoana));

                pers.Id = int.Parse(linii[1]);
                pers.Nume_Prenume = linii[2];
                pers.Telefon = linii[3];
                pers.Email = linii[4];

            }

            return pers;
        }

        public Persoana SalveazaPersoana(Persoana persoana, int idPers)
        {
            if (!System.IO.Directory.Exists(CalePersoaneContact))
            {
                System.IO.Directory.CreateDirectory(CalePersoaneContact);
            }

            int GetId = idPers;
            //idul pers pe care vreau sa o salvez \ -1 inseamna ca vreau sa salvez una noua, daca id-ul e dat, va overwritea evenimentul cu acel id
            if (idPers == -1)
                GetId = IdUltimaPersoanaInregistrata() + 1;
            

            System.IO.Directory.CreateDirectory(CalePersoanaContact(GetId));

            List<string> linii = new List<string>();

            persoana.Id = GetId;

            linii.Add(Nume);
            linii.Add(persoana.Id.ToString());
            linii.Add(persoana.Nume_Prenume);
            linii.Add(persoana.Telefon);
            linii.Add(persoana.Email);

            System.IO.File.WriteAllLines(CaleFisierInfoPersoana(GetId), linii);

            return persoana;
        }
        

        public ContBancar CitesteContBancar(string caleCont)
        {
            ContBancar cont = new ContBancar();

            if (File.Exists(CaleFisierInfoCont(caleCont)))
            {
                string[] linii = File.ReadAllLines(CaleFisierInfoCont(caleCont));

                cont.Id = int.Parse(linii[1]);
                cont.IBAN = linii[2];
                cont.Banca = linii[3];
                cont.Moneda_Cont = linii[4];

            }

            return cont;
        }
        
        public ContBancar SalveazaContBancar(ContBancar cont, int idCont)
        {
            if (!System.IO.Directory.Exists(CaleConturiBancare))
            {
                System.IO.Directory.CreateDirectory(CaleConturiBancare);
            }

            int GetId = idCont;
            //idul cont pe care vreau sa o salvez \ -1 inseamna ca vreau sa salvez una noua, daca id-ul e dat, va overwritea evenimentul cu acel id
            if (idCont == -1)
                GetId = Directory.GetDirectories(CaleConturiBancare).Count() + 1;


            System.IO.Directory.CreateDirectory(CaleContBancar(GetId));

            List<string> linii = new List<string>();

            cont.Id = GetId;

            linii.Add(Nume);
            linii.Add(cont.Id.ToString());
            linii.Add(cont.IBAN);
            linii.Add(cont.Banca);
            linii.Add(cont.Moneda_Cont);

            System.IO.File.WriteAllLines(CaleFisierInfoCont(GetId), linii);

            return cont;
        }



   /*     public void GenereazaEvenimentScadentaFacturaClient()
        {
            List<Factura> facclplatite = FacturiClientPlatite(new DateTime(Settings.An_Curent, 1, 1), DateTime.Today);
            List<Factura> facclperioada = FacturiClientPePerioada(new DateTime(Settings.An_Curent, 1, 1), DateTime.Today);

            if (facclplatite.Count != facclperioada.Count)
            {
                Eveniment ev = new Eveniment();
                ev.Id = -1;
                ev.Denumire = "Factura n";
                ev.Nume_Firma = Nume;
                ev.Tip = "Scadenta factura";
                ev.Stare = "Nerezolvat";
                ev.Data_Eveniment = DateTime.Now;
                ev.Note = "Facturi platite:";

                for(int i = 0; i < facclplatite.Count; i++)
                {
                    ev.Note += facclplatite[i].Numar_Document + " ";
                }
                
                ev.Cai_Documente = new List<string>() { };

                bool maiExista = false;

                if (Evenimente.Count > 0)
                {
                    for (int i = 0; i < Evenimente.Count; i++)
                    {
                        if(ev == Evenimente[i])
                        {
                            maiExista = true;
                            break;
                        }
                    }
                }

                if (!maiExista)
                    SalveazaEveniment(ev, -1);

            }
        } */

        public void GenereazaEvenimentScadentaFacturaClient()
        {
            List<Factura> facclneplatite = FacturiClientNeplatite(new DateTime(Settings.An_Curent, 1, 1), DateTime.Today);
            List<Factura> facclperioada = FacturiClientPePerioada(new DateTime(Settings.An_Curent, 1, 1), DateTime.Today);

            if (facclneplatite.Count > 0)
            {
                Eveniment ev = new Eveniment();
                ev.Id = -1;
                ev.Denumire = "Factura n";
                ev.Nume_Firma = Nume;
                ev.Tip = "Scadenta factura";
                ev.Stare = "Nerezolvat";
                ev.Data_Eveniment = DateTime.Now;
                ev.Note = "Facturi neplatite:";

                for (int i = 0; i < facclneplatite.Count; i++)
                {
                    ev.Note += facclneplatite[i].Numar_Document + " ";
                }

                ev.Cai_Documente = new List<string>() { };

                bool maiExista = false;

                if (Evenimente.Count > 0)
                {
                    for (int i = 0; i < Evenimente.Count; i++)
                    {
                        if (ev == Evenimente[i])
                        {
                            maiExista = true;
                            break;
                        }
                    }
                }

                if (!maiExista)
                    SalveazaEveniment(ev, -1);

            }
        }
    }
}
