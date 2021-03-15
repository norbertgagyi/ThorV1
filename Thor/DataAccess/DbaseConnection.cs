using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using Thor.Models;

namespace Thor.DataAccess
{
    public static class DbaseConnection
    {
        public static string EscapeApostrophe(string s)
        {
            return s.Replace("'", "''");
        }

        public static List<Firma> DBFCitesteFirme(List<string> ani)
        {
            //metoda cu dictionar - lookup foarte rapid - pentru fiecare firma, daca nu are deja o cheie in dictionar, creez una,
            //daca are deja, se va sti foarte rapid, deci trece mai departe

            //am nevoie de asta pentru ca citesc toti anii pentru parteneri, apar sigur duplicate pe care le rezolv aici, nu selectand distinct,
            //care e mai incet cu limbaj sql

            Dictionary<string, bool> firme = new Dictionary<string, bool>();

            List<Firma> union = DBFCitesteFirmeParteneri(ani).Union(DBFCitesteFirmeReceptii(ani)).ToList();

            List<Firma> rez = new List<Firma>();
             
            foreach(Firma f in union)
            {
                if (!firme.ContainsKey(f.Nume + f.CUI))
                {
                    rez.Add(f);
                    firme.Add(f.Nume + f.CUI, true);
                }
            }

            for (int i = 0; i < rez.Count; i++)
                rez[i].Id = i + 1;

            return rez;
        }

        public static List<Firma> DBFCitesteFirmeParteneri(List<string> ani)
        {
            List<Firma> rezultat = new List<Firma>();

            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT" +
                        " CF_PAR, DEN_PAR, REG_PAR," +
                        " TARA_PAR, JUD_PAR, LOC_PAR, C_POST, ADRESA_PAR," +
                        "NR_TEL, NR_FAX, E_MAIL," +
                        "CONT_PAR, BANCA_PAR" +
                        " FROM [PARTENER.dbf]";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();

                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Firma f = new Firma();

                        f.CUI = r.ItemArray.ElementAt(0).ToString();
                        f.Nume = r.ItemArray.ElementAt(1).ToString();
                        f.Registrul_Comertului = r.ItemArray.ElementAt(2).ToString();
                        f.Tara = r.ItemArray.ElementAt(3).ToString();
                        f.Judet = r.ItemArray.ElementAt(4).ToString();
                        f.Localitate = r.ItemArray.ElementAt(5).ToString();
                        f.Cod_Postal = r.ItemArray.ElementAt(6).ToString();
                        f.Adresa = r.ItemArray.ElementAt(7).ToString();
                        f.Telefon = r.ItemArray.ElementAt(8).ToString();
                        f.Fax = r.ItemArray.ElementAt(9).ToString();
                        f.Email = r.ItemArray.ElementAt(10).ToString();

                        if (r.ItemArray.ElementAt(11).ToString() != "" && r.ItemArray.ElementAt(12).ToString() != "")
                        {
                            f.Conturi_Bancare = new List<ContBancar>();
                            ContBancar cont = new ContBancar();

                            cont.IBAN = r.ItemArray.ElementAt(11).ToString();
                            cont.Banca = r.ItemArray.ElementAt(12).ToString();

                            f.Conturi_Bancare.Add(cont);
                        }

                        rezultat.Add(f);
                    }

                    conn.Close();
                }
                catch { }
            }

            return rezultat;
            
        }

        public static List<Firma> DBFCitesteFirmeReceptii(List<string> ani)
        {
            List<Firma> rezultat = new List<Firma>();

            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DISTINCT CF_PAR,DEN_PAR FROM [RECEPTII.dbf]";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();




                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Firma f = new Firma();

                        f.CUI = r.ItemArray.ElementAt(0).ToString();
                        f.Nume = r.ItemArray.ElementAt(1).ToString();

                        f.Conturi_Bancare = new List<ContBancar>();

                        rezultat.Add(f);
                    }

                    conn.Close();
                }
                catch { }
            }

            return rezultat;
        }


        
        public static List<Factura> DBFCitesteFacturiClientFirma(Firma firma, List<string> ani)
        {
            List<Factura> facturi = new List<Factura>();
            //valoarea e valoarea fara tva
            int cnt = 0;
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT SERIA,NR_DOC,DATA_DOC,DATA_SC,VALOARE,TOT_TVA,MON,CURS FROM [FACTURI.dbf] WHERE CF_PAR = '" + firma.CUI + "'";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Factura fac = new Factura();

                        fac.Id = cnt++;
                        fac.Seria = r.ItemArray.ElementAt(0).ToString();
                        fac.Numar_Document = r.ItemArray.ElementAt(1).ToString();
                        fac.Nume_Firma = firma.Nume;
                        fac.CUI_Firma = firma.CUI;
                        fac.Tip_Factura = "Client";

                        if (r.ItemArray.ElementAt(2).ToString() != "")
                            fac.Data_Emitere = DateTime.Parse(r.ItemArray.ElementAt(2).ToString());

                        if (r.ItemArray.ElementAt(3).ToString() != "")
                            fac.Data_Scadenta = DateTime.Parse(r.ItemArray.ElementAt(3).ToString());


                        fac.Valoare = (decimal)(double)(r.ItemArray.ElementAt(4));
                        fac.TVA = (decimal)(double)(r.ItemArray.ElementAt(5));

                        if (r.ItemArray.ElementAt(6).ToString() != "" && r.ItemArray.ElementAt(7).ToString() != "")
                        {
                            fac.Moneda = r.ItemArray.ElementAt(6).ToString();
                            fac.Curs = (decimal)(double)(r.ItemArray.ElementAt(7));
                        }
                        else
                        {
                            fac.Moneda = "RON";
                            fac.Curs = 1;
                        }

                        facturi.Add(fac);
                    }

                    conn.Close();
                }
                catch { }
            }

            return facturi.OrderBy(x => x.Data_Emitere).ToList();

        }

        public static List<Factura> DBFCitesteFacturiFurnizorFirma(Firma firma, List<string> ani)

        {
            List<Factura> facturi = new List<Factura>();
            //valoarea din receptie e valoare cu tva
            int cnt = 0;
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT SERIA,NR_DOC,DATA_DOC,DATA_SC,VAL_PROD,TOT_TVA,MON,CURS FROM [RECEPTII.dbf] WHERE CF_PAR = '" + firma.CUI + "'";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Factura fac = new Factura();

                        fac.Id = cnt++;
                        fac.Seria = r.ItemArray.ElementAt(0).ToString();
                        fac.Numar_Document = r.ItemArray.ElementAt(1).ToString();
                        fac.Nume_Firma = firma.Nume;
                        fac.CUI_Firma = firma.CUI;
                        fac.Tip_Factura = "Furnizor";

                        if (r.ItemArray.ElementAt(2).ToString() != "")
                            fac.Data_Emitere = DateTime.Parse(r.ItemArray.ElementAt(2).ToString());

                        if (r.ItemArray.ElementAt(3).ToString() != "")
                            fac.Data_Scadenta = DateTime.Parse(r.ItemArray.ElementAt(3).ToString());


                        fac.Valoare = (decimal)(double)(r.ItemArray.ElementAt(4));
                        fac.TVA = (decimal)(double)(r.ItemArray.ElementAt(5));

                        if (r.ItemArray.ElementAt(6).ToString() != "" && r.ItemArray.ElementAt(7).ToString() != "")
                        {
                            fac.Moneda = r.ItemArray.ElementAt(6).ToString();
                            fac.Curs = (decimal)(double)(r.ItemArray.ElementAt(7));
                        }
                        else
                        {
                            fac.Moneda = "RON";
                            fac.Curs = 1;
                        }

                        facturi.Add(fac);
                    }

                    conn.Close();
                }
                catch { }
            }

            return facturi.OrderBy(x => x.Data_Emitere).ToList();

        }


        public static List<Factura> DBFFacturi(List<string> ani, DateTime dataInceput, DateTime dataSfarsit)
        {
            List<Factura> f = DBFFacturiClient(ani).Union(DBFFacturiFurnizor(ani)).OrderBy(x => x.Data_Emitere).ToList();

            return f.FindAll(x => x.Data_Scadenta.Date >= dataInceput && x.Data_Scadenta.Date <= dataSfarsit);
        }

        public static List<Factura> DBFFacturiClient(List<string> ani)
        {
            List<Factura> facturi = new List<Factura>();

            int cnt = 0;
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT SERIA,NR_DOC,DATA_DOC,DATA_SC,VALOARE,TOT_TVA,MON,CURS,DEN_PAR,CF_PAR FROM [FACTURI.dbf]";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Factura fac = new Factura();

                        fac.Id = cnt++;
                        fac.Seria = r.ItemArray.ElementAt(0).ToString();
                        fac.Numar_Document = r.ItemArray.ElementAt(1).ToString();
                        fac.Tip_Factura = "Client";

                        if (r.ItemArray.ElementAt(2).ToString() != "")
                            fac.Data_Emitere = DateTime.Parse(r.ItemArray.ElementAt(2).ToString());

                        if (r.ItemArray.ElementAt(3).ToString() != "")
                            fac.Data_Scadenta = DateTime.Parse(r.ItemArray.ElementAt(3).ToString());


                        fac.Valoare = (decimal)(double)(r.ItemArray.ElementAt(4));
                        fac.TVA = (decimal)(double)(r.ItemArray.ElementAt(5));
                        fac.Nume_Firma = r.ItemArray.ElementAt(8).ToString();
                        fac.CUI_Firma = r.ItemArray.ElementAt(9).ToString();

                        if (r.ItemArray.ElementAt(6).ToString() != "" && r.ItemArray.ElementAt(7).ToString() != "")
                        {
                            fac.Moneda = r.ItemArray.ElementAt(6).ToString();
                            fac.Curs = (decimal)(double)(r.ItemArray.ElementAt(7));
                        }
                        else
                        {
                            fac.Moneda = "RON";
                            fac.Curs = 1;
                        }

                        facturi.Add(fac);
                    }

                    conn.Close();
                }
                catch { }
            }

            return facturi;

        }

        public static List<Factura> DBFFacturiFurnizor(List<string> ani)
        {
            List<Factura> facturi = new List<Factura>();
            //valoarea din receptie e valoare cu tva
            int cnt = 0;
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT SERIA,NR_DOC,DATA_DOC,DATA_SC,VAL_PROD,TOT_TVA,MON,CURS,DEN_PAR,CF_PAR FROM [RECEPTII.dbf]";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Factura fac = new Factura();
                        
                        fac.Id = cnt++;
                        fac.Seria = r.ItemArray.ElementAt(0).ToString();
                        fac.Numar_Document = r.ItemArray.ElementAt(1).ToString();
                        fac.Tip_Factura = "Furnizor";

                        if (r.ItemArray.ElementAt(2).ToString() != "")
                            fac.Data_Emitere = DateTime.Parse(r.ItemArray.ElementAt(2).ToString());

                        if (r.ItemArray.ElementAt(3).ToString() != "")
                            fac.Data_Scadenta = DateTime.Parse(r.ItemArray.ElementAt(3).ToString());


                        fac.Valoare = (decimal)(double)(r.ItemArray.ElementAt(4));
                        fac.TVA = (decimal)(double)(r.ItemArray.ElementAt(5));
                        fac.Nume_Firma = r.ItemArray.ElementAt(8).ToString();
                        fac.CUI_Firma = r.ItemArray.ElementAt(9).ToString();

                        if (r.ItemArray.ElementAt(6).ToString() != "" && r.ItemArray.ElementAt(7).ToString() != "")
                        {
                            fac.Moneda = r.ItemArray.ElementAt(6).ToString();
                            fac.Curs = (decimal)(double)(r.ItemArray.ElementAt(7));
                        }
                        else
                        {
                            fac.Moneda = "RON";
                            fac.Curs = 1;
                        }

                        facturi.Add(fac);
                    }

                    conn.Close();
                }
                catch { }
            }

            return facturi;

        }


        public static List<string> DBFFirmeActive(List<string> ani)
        {
            return DBFFirmeClientActive(ani).Union(DBFFirmeFurnizorActive(ani)).ToList();
        }

        public static List<string> DBFFirmeClientActive(List<string> ani)
        {
            List<string> firme = new List<string>();

            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DISTINCT CF_PAR FROM [FACTURI.dbf]";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        firme.Add(r.ItemArray.ElementAt(0).ToString());
                    }

                    conn.Close();
                }
                catch { }
            }
            return firme;
        }

        public static List<string> DBFFirmeFurnizorActive(List<string> ani)
        {
            List<string> firme = new List<string>();

            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DISTINCT CF_PAR FROM [RECEPTII.dbf]";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        firme.Add(r.ItemArray.ElementAt(0).ToString());
                    }

                    conn.Close();
                }
                catch { }
            }
            return firme;
        }


        public static List<Stoc> DBFCitesteStocuri(List<string> ani)
        {
            List<Stoc> stocuri = new List<Stoc>();

            int cnt = 0;

            for(int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DEN_PROD, PRET_STOC, CANTITATE," +
                        "U_M FROM [STOCINV.dbf]";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();
                    
                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Stoc s = new Stoc();

                        s.Id = cnt++;

                        s.Denumire_Produs = r.ItemArray.ElementAt(0).ToString();
                        s.Pret_Stoc = (decimal)(double)(r.ItemArray.ElementAt(1));
                        s.Cantitate = (decimal)(double)(r.ItemArray.ElementAt(2));
                        s.Unitate_Masura = r.ItemArray.ElementAt(3).ToString();

                        stocuri.Add(s);
                    }

                    conn.Close();
                }
                catch { }
            }

            return stocuri;
        }




        public static List<Tranzactie> DBFCitesteTranzactiiBancareFirma(Firma firma, List<string> ani)
        {
            List<Tranzactie> tranzactii = new List<Tranzactie>();

            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT TIP_OP," +
                        "SERIA,NR_DOC,DATA_DOC," +
                        "SERDC,NR_D_COR,DATA_D_COR," +
                        "VALOARE,TOT_TVA," +
                        "OB_PLATA,MON,CURS FROM [BANCA.dbf] WHERE CF_PAR = '" + firma.CUI + "' AND DEN_PAR LIKE '%" + EscapeApostrophe(firma.Nume) + "%'";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();



                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Tranzactie t = new Tranzactie();

                        t.Tip_Operatiune = r.ItemArray.ElementAt(0).ToString();
                        t.Seria = r.ItemArray.ElementAt(1).ToString();
                        t.Numar_Document = r.ItemArray.ElementAt(2).ToString();
                        t.Seria_Document_Corespunzator = r.ItemArray.ElementAt(4).ToString();
                        t.Numar_Document_Corespunzator = r.ItemArray.ElementAt(5).ToString();
                        t.Valoare = (decimal)(double)(r.ItemArray.ElementAt(7));
                        t.TVA = (decimal)(double)(r.ItemArray.ElementAt(8));
                        t.Obiect_Tranzactie = r.ItemArray.ElementAt(9).ToString();
                        t.Metoda = "BANCA";

                        if (r.ItemArray.ElementAt(3).ToString() != "" && r.ItemArray.ElementAt(6).ToString() != "")
                        {
                            t.Data_Tranzactie = DateTime.Parse(r.ItemArray.ElementAt(3).ToString());
                            t.Data_Document_Corespunzator = DateTime.Parse(r.ItemArray.ElementAt(6).ToString());
                        }

                        if (r.ItemArray.ElementAt(10).ToString() != "" && r.ItemArray.ElementAt(11).ToString() != "")
                        {
                            t.Moneda = r.ItemArray.ElementAt(10).ToString();
                            t.Curs = (decimal)(double)(r.ItemArray.ElementAt(11));
                        }
                        else
                        {
                            t.Moneda = "RON";
                            t.Curs = 1;
                        }

                        tranzactii.Add(t);
                    }

                    conn.Close();
                }
                catch { }
            }

            return tranzactii.OrderBy(x => x.Data_Tranzactie).ToList();

        }

        public static List<Tranzactie> DBFCitesteTranzactiiMonetareFirma(Firma firma, List<string> ani)
        {
            List<Tranzactie> tranzactii = new List<Tranzactie>();

            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT TIP_OP," +
                        "SERIA,NR_DOC,DATA_DOC," +
                        "SERDC,NR_D_COR,DATA_D_COR," +
                        "VALOARE,TOT_TVA," +
                        "OB_PLATA,MON,CURS FROM [CASA.dbf] WHERE CF_PAR = '" + firma.CUI + "' AND DEN_PAR LIKE '%" + EscapeApostrophe(firma.Nume) + "%'";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();




                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Tranzactie t = new Tranzactie();

                        t.Tip_Operatiune = r.ItemArray.ElementAt(0).ToString();
                        t.Seria = r.ItemArray.ElementAt(1).ToString();
                        t.Numar_Document = r.ItemArray.ElementAt(2).ToString();
                        t.Seria_Document_Corespunzator = r.ItemArray.ElementAt(4).ToString();
                        t.Numar_Document_Corespunzator = r.ItemArray.ElementAt(5).ToString();
                        t.Valoare = (decimal)(double)(r.ItemArray.ElementAt(7));
                        t.TVA = (decimal)(double)(r.ItemArray.ElementAt(8));
                        t.Obiect_Tranzactie = r.ItemArray.ElementAt(9).ToString();
                        t.Metoda = "CASA";

                        if (r.ItemArray.ElementAt(3).ToString() != "" && r.ItemArray.ElementAt(6).ToString() != "")
                        {
                            t.Data_Tranzactie = DateTime.Parse(r.ItemArray.ElementAt(3).ToString());
                            t.Data_Document_Corespunzator = DateTime.Parse(r.ItemArray.ElementAt(6).ToString());
                        }

                        if (r.ItemArray.ElementAt(10).ToString() != "" && r.ItemArray.ElementAt(11).ToString() != "")
                        {
                            t.Moneda = r.ItemArray.ElementAt(10).ToString();
                            t.Curs = (decimal)(double)(r.ItemArray.ElementAt(11));
                        }
                        else
                        {
                            t.Moneda = "RON";
                            t.Curs = 1;
                        }

                        tranzactii.Add(t);
                    }

                    conn.Close();
                }
                catch { }
            }

            return tranzactii.OrderBy(x => x.Data_Tranzactie).ToList();

        }

        public static List<Tranzactie> DBFCitesteTranzactiiCompensareFirma(Firma firma, List<string> ani)
        {
            List<Tranzactie> tranzactii = new List<Tranzactie>();

            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT TIP_OP," +
                        "SERIA,NR_DOC,DATA_DOC," +
                        "SERDC,NR_D_COR,DATA_D_COR," +
                        "VALOARE,MON,CURS " +
                        "FROM [COMPENS.dbf] WHERE CF_PAR = '" + firma.CUI + "' AND DEN_PAR LIKE '%" + EscapeApostrophe(firma.Nume) + "%'";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Tranzactie t = new Tranzactie();

                        t.Tip_Operatiune = r.ItemArray.ElementAt(0).ToString();
                        t.Seria = r.ItemArray.ElementAt(1).ToString();
                        t.Numar_Document = r.ItemArray.ElementAt(2).ToString();
                        t.Seria_Document_Corespunzator = r.ItemArray.ElementAt(4).ToString();
                        t.Numar_Document_Corespunzator = r.ItemArray.ElementAt(5).ToString();
                        t.Valoare = (decimal)(double)(r.ItemArray.ElementAt(7));
                        t.TVA = 0;
                        t.Obiect_Tranzactie = "";
                        t.Metoda = "COMPENSARE";

                        if (r.ItemArray.ElementAt(3).ToString() != "" && r.ItemArray.ElementAt(6).ToString() != "")
                        {
                            t.Data_Tranzactie = DateTime.Parse(r.ItemArray.ElementAt(3).ToString());
                            t.Data_Document_Corespunzator = DateTime.Parse(r.ItemArray.ElementAt(6).ToString());
                        }

                        if (r.ItemArray.ElementAt(8).ToString() != "" && r.ItemArray.ElementAt(9).ToString() != "")
                        {
                            t.Moneda = r.ItemArray.ElementAt(8).ToString();
                            t.Curs = (decimal)(double)(r.ItemArray.ElementAt(9));
                        }
                        else
                        {
                            t.Moneda = "RON";
                            t.Curs = 1;
                        }

                        tranzactii.Add(t);
                    }

                    conn.Close();
                }
                catch { }
            }

            return tranzactii.OrderBy(x => x.Data_Tranzactie).ToList();

        }

        public static List<Tranzactie> DBFCitesteTranzactiiFirma(Firma firma, List<string> ani)
        {
            List<Tranzactie> BanMon = DBFCitesteTranzactiiBancareFirma(firma, ani).Union(DBFCitesteTranzactiiMonetareFirma(firma, ani)).ToList();

            return BanMon.Union(DBFCitesteTranzactiiCompensareFirma(firma, ani)).OrderBy(x => x.Data_Tranzactie).ToList();
        }




        public static List<Produs> DBFCitesteProduseFacturaClient(Factura fac, List<string> ani)
        {
            List<Produs> produse = new List<Produs>();

            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DEN_PROD,U_M,CANTITATE,PRET_CUMP,PRET_VINZ,TOT_TVA FROM [MARFAFAC.dbf] WHERE " + (fac.Seria == "" ? "SERIA IS NULL" : "SERIA = '" + fac.Seria + "'") + " AND NR_DOC = " + fac.Numar_Document;

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Produs prod = new Produs();

                        prod.Denumire_Produs = r.ItemArray.ElementAt(0).ToString();

                        if (r.ItemArray.ElementAt(1).ToString() != "" && r.ItemArray.ElementAt(2).ToString() != ""
                            && r.ItemArray.ElementAt(3).ToString() != "" && r.ItemArray.ElementAt(4).ToString() != ""
                            && r.ItemArray.ElementAt(5).ToString() != "")
                        {
                            prod.Unitate_Masura = r.ItemArray.ElementAt(1).ToString();
                            prod.Cantitate = (decimal)(double)(r.ItemArray.ElementAt(2));
                            prod.Pret_Cumparare = (decimal)(double)(r.ItemArray.ElementAt(3));
                            prod.Pret_Vanzare = (decimal)(double)(r.ItemArray.ElementAt(4));
                            prod.TVA = (decimal)(double)(r.ItemArray.ElementAt(5));
                        }

                        produse.Add(prod);
                    }

                    conn.Close();
                }
                catch { }
            }

            return produse;

        }

        public static List<Produs> DBFCitesteProduseFacturaFurnizor(Factura fac, List<string> ani)
        {
            List<Produs> produse = new List<Produs>();

            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DEN_PROD,U_M,CANTITATE,PRET_CUMP,PRET_VINZ,TOT_TVA FROM [MARFAREC.dbf] WHERE " + (fac.Seria == "" ? "SERIA IS NULL" : "SERIA = '" + fac.Seria + "'") + " AND NR_DOC = " + fac.Numar_Document;

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();



                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Produs prod = new Produs();

                        prod.Denumire_Produs = r.ItemArray.ElementAt(0).ToString();

                        if (r.ItemArray.ElementAt(1).ToString() != "" && r.ItemArray.ElementAt(2).ToString() != ""
                            && r.ItemArray.ElementAt(3).ToString() != "" && r.ItemArray.ElementAt(4).ToString() != ""
                            && r.ItemArray.ElementAt(5).ToString() != "")
                        {
                            prod.Unitate_Masura = r.ItemArray.ElementAt(1).ToString();
                            prod.Cantitate = (decimal)(double)(r.ItemArray.ElementAt(2));
                            prod.Pret_Cumparare = (decimal)(double)(r.ItemArray.ElementAt(3));
                            prod.Pret_Vanzare = (decimal)(double)(r.ItemArray.ElementAt(4));
                            prod.TVA = (decimal)(double)(r.ItemArray.ElementAt(5));
                        }

                        produse.Add(prod);
                    }

                    conn.Close();
                }
                catch { }
            }

            return produse;

        }


        public static List<object> DBFTotalFacturiClientPerioada(List<string> ani, DateTime inceput, DateTime sfarsit)
        {
            //valoarea e valoarea fara tva
            List<object> rez = new List<object>();

            decimal valCuTVA = 0m;
            decimal valFaraTVA = 0m;
            decimal valTVA = 0m;
            int cnt = 0;
            decimal tvaMediu = 0m;

            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DATA_DOC,VALOARE,TOT_TVA,CURS FROM [FACTURI.dbf]";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        if (r.ItemArray.ElementAt(0).ToString() != "")
                        {
                            if (inceput <= DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) && DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) <= sfarsit)
                            {

                                valCuTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m) +
                            decimal.Parse(r.ItemArray.ElementAt(2).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                valFaraTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                valTVA += decimal.Parse(r.ItemArray.ElementAt(2).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                cnt++;

                            }
                        }
                    }

                    if (valFaraTVA != 0)
                        tvaMediu = (100 * valTVA) / valFaraTVA;

                    conn.Close();


                }
                catch { }
            }

            rez.Add(valCuTVA);
            rez.Add(valFaraTVA);
            rez.Add(valTVA);
            rez.Add(cnt);
            rez.Add(tvaMediu);

            return rez;

        }

        public static List<object> DBFTotalFacturiFurnizorPerioada(List<string> ani, DateTime inceput, DateTime sfarsit)
        {
            List<object> rez = new List<object>();

            decimal valCuTVA = 0m;
            decimal valFaraTVA = 0m;
            decimal valTVA = 0m;
            int cnt = 0;
            decimal tvaMediu = 0m;
            //valoarea e valoarea cu tva
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DATA_DOC,VALOARE,TOT_TVA,CURS FROM [RECEPTII.dbf]";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        if (r.ItemArray.ElementAt(0).ToString() != "")
                        {
                            if (inceput <= DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) && DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) <= sfarsit)
                            {
                                valCuTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);


                                valFaraTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m) -
                            decimal.Parse(r.ItemArray.ElementAt(2).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);


                                valTVA += decimal.Parse(r.ItemArray.ElementAt(2).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                cnt++;

                            }
                        }
                    }

                    if (valFaraTVA != 0)
                        tvaMediu = (100 * valTVA) / valFaraTVA;

                    conn.Close();


                }
                catch { }
            }

            rez.Add(valCuTVA);
            rez.Add(valFaraTVA);
            rez.Add(valTVA);
            rez.Add(cnt);
            rez.Add(tvaMediu);

            return rez;

        }



        public static List<object> DBFTotalIncasariBancarePerioada(List<string> ani, DateTime inceput, DateTime sfarsit)
        {
            List<object> rez = new List<object>();

            decimal valCuTVA = 0m;
            decimal valFaraTVA = 0m;
            decimal valTVA = 0m;
            int cnt = 0;
            decimal tvaMediu = 0m;
            //valoarea e valoarea fara tva
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DATA_EMIT,VALOARE,TOT_TVA,CURS FROM [BANCA.dbf] WHERE TIP_OP = 'I' AND CF_PAR IS NOT NULL";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        if (r.ItemArray.ElementAt(0).ToString() != "")
                        {
                            if (inceput <= DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) && DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) <= sfarsit)
                            {
                                valCuTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m) +
                            decimal.Parse(r.ItemArray.ElementAt(2).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                valFaraTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                valTVA += decimal.Parse(r.ItemArray.ElementAt(2).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                cnt++;

                            }
                        }
                    }

                    if (valFaraTVA != 0)
                        tvaMediu = (100 * valTVA) / valFaraTVA;

                    conn.Close();

                }
                catch { }
            }

            rez.Add(valCuTVA);
            rez.Add(valFaraTVA);
            rez.Add(valTVA);
            rez.Add(cnt);
            rez.Add(tvaMediu);

            return rez;

        }

        public static List<object> DBFTotalIncasariMonetarePerioada(List<string> ani, DateTime inceput, DateTime sfarsit)
        {
            List<object> rez = new List<object>();

            decimal valCuTVA = 0m;
            decimal valFaraTVA = 0m;
            decimal valTVA = 0m;
            int cnt = 0;
            decimal tvaMediu = 0m;
            //valoarea e valoarea fara tva
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DATA_EMIT,VALOARE,TOT_TVA,CURS FROM [CASA.dbf] WHERE TIP_OP = 'I' AND CF_PAR IS NOT NULL";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        if (r.ItemArray.ElementAt(0).ToString() != "")
                        {
                            if (inceput <= DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) && DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) <= sfarsit)
                            {
                                valCuTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m) +
                            decimal.Parse(r.ItemArray.ElementAt(2).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                valFaraTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                valTVA += decimal.Parse(r.ItemArray.ElementAt(2).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                cnt++;

                            }
                        }
                    }

                    if (valFaraTVA != 0)
                        tvaMediu = (100 * valTVA) / valFaraTVA;

                    conn.Close();

                }
                catch { }

            }

            rez.Add(valCuTVA);
            rez.Add(valFaraTVA);
            rez.Add(valTVA);
            rez.Add(cnt);
            rez.Add(tvaMediu);

            return rez;

        }

        public static List<object> DBFTotalIncasariCompensarePerioada(List<string> ani, DateTime inceput, DateTime sfarsit)
        {
            List<object> rez = new List<object>();

            decimal valCuTVA = 0m;
            decimal valFaraTVA = 0m;
            decimal valTVA = 0m;
            int cnt = 0;
            decimal tvaMediu = 0m;
            //valoarea e valoarea fara tva
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DATA_EMIT,VALOARE,CURS FROM [COMPENS.dbf] WHERE TIP_OP = 'I' AND CF_PAR IS NOT NULL";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        if (r.ItemArray.ElementAt(0).ToString() != "")
                        {
                            if (inceput <= DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) && DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) <= sfarsit)
                            {

                                valFaraTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(2).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(2).ToString()) : 1m);

                                cnt++;

                            }
                        }
                    }

                    valCuTVA = valFaraTVA;

                    if (valFaraTVA != 0)
                        tvaMediu = (100 * valTVA) / valFaraTVA;

                    conn.Close();


                }
                catch { }
            }

            rez.Add(valCuTVA);
            rez.Add(valFaraTVA);
            rez.Add(valTVA);
            rez.Add(cnt);
            rez.Add(tvaMediu);

            return rez;

        }

        public static List<object> DBFTotalIncasari(List<string> ani, DateTime inceput, DateTime sfarsit)
        {
            List<object> rez = new List<object>();

            decimal valCuTVA = 0m;
            decimal valFaraTVA = 0m;
            decimal valTVA = 0m;
            int cnt = 0;
            decimal tvaMediu = 0m;

            try
            {
                valCuTVA += (decimal)DBFTotalIncasariBancarePerioada(ani, inceput, sfarsit)[0];
                valCuTVA += (decimal)DBFTotalIncasariMonetarePerioada(ani, inceput, sfarsit)[0];
                valCuTVA += (decimal)DBFTotalIncasariCompensarePerioada(ani, inceput, sfarsit)[0];
                
                valFaraTVA += (decimal)DBFTotalIncasariBancarePerioada(ani, inceput, sfarsit)[1];
                valFaraTVA += (decimal)DBFTotalIncasariMonetarePerioada(ani, inceput, sfarsit)[1];
                valFaraTVA += (decimal)DBFTotalIncasariCompensarePerioada(ani, inceput, sfarsit)[1];
                
                valTVA += (decimal)DBFTotalIncasariBancarePerioada(ani, inceput, sfarsit)[2];
                valTVA += (decimal)DBFTotalIncasariMonetarePerioada(ani, inceput, sfarsit)[2];
                valTVA += (decimal)DBFTotalIncasariCompensarePerioada(ani, inceput, sfarsit)[2];
                
                cnt += (int)DBFTotalIncasariBancarePerioada(ani, inceput, sfarsit)[3];
                cnt += (int)DBFTotalIncasariMonetarePerioada(ani, inceput, sfarsit)[3];
                cnt += (int)DBFTotalIncasariCompensarePerioada(ani, inceput, sfarsit)[3];
                
                if (valFaraTVA != 0)
                    tvaMediu = (100 * valTVA) / valFaraTVA;
                
            }
            catch { }

            rez.Add(valCuTVA);
            rez.Add(valFaraTVA);
            rez.Add(valTVA);
            rez.Add(cnt);
            rez.Add(tvaMediu);

            return rez;

        }


        public static List<object> DBFTotalPlatiBancarePerioada(List<string> ani, DateTime inceput, DateTime sfarsit)
        {
            List<object> rez = new List<object>();

            decimal valCuTVA = 0m;
            decimal valFaraTVA = 0m;
            decimal valTVA = 0m;
            int cnt = 0;
            decimal tvaMediu = 0m;
            //valoarea e valoarea fara tva
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DATA_EMIT,VALOARE,TOT_TVA,CURS FROM [BANCA.dbf] WHERE TIP_OP = 'P' AND CF_PAR IS NOT NULL";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        if (r.ItemArray.ElementAt(0).ToString() != "")
                        {
                            if (inceput <= DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) && DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) <= sfarsit)
                            {
                                valCuTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m) +
                            decimal.Parse(r.ItemArray.ElementAt(2).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                valFaraTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                valTVA += decimal.Parse(r.ItemArray.ElementAt(2).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                cnt++;

                            }
                        }
                    }

                    if (valFaraTVA != 0)
                        tvaMediu = (100 * valTVA) / valFaraTVA;

                    conn.Close();

                }
                catch { }
            }

            rez.Add(valCuTVA);
            rez.Add(valFaraTVA);
            rez.Add(valTVA);
            rez.Add(cnt);
            rez.Add(tvaMediu);

            return rez;

        }

        public static List<object> DBFTotalPlatiMonetarePerioada(List<string> ani, DateTime inceput, DateTime sfarsit)
        {
            List<object> rez = new List<object>();

            decimal valCuTVA = 0m;
            decimal valFaraTVA = 0m;
            decimal valTVA = 0m;
            int cnt = 0;
            decimal tvaMediu = 0m;
            //valoarea e valoarea fara tva
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DATA_EMIT,VALOARE,TOT_TVA,CURS FROM [CASA.dbf] WHERE TIP_OP = 'P' AND CF_PAR IS NOT NULL";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        if (r.ItemArray.ElementAt(0).ToString() != "")
                        {
                            if (inceput <= DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) && DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) <= sfarsit)
                            {
                                valCuTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m) +
                            decimal.Parse(r.ItemArray.ElementAt(2).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                valFaraTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                valTVA += decimal.Parse(r.ItemArray.ElementAt(2).ToString()) * (r.ItemArray.ElementAt(3).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(3).ToString()) : 1m);

                                cnt++;

                            }
                        }
                    }

                    if (valFaraTVA != 0)
                        tvaMediu = (100 * valTVA) / valFaraTVA;

                    conn.Close();

                }
                catch { }

            }

            rez.Add(valCuTVA);
            rez.Add(valFaraTVA);
            rez.Add(valTVA);
            rez.Add(cnt);
            rez.Add(tvaMediu);

            return rez;

        }

        public static List<object> DBFTotalPlatiCompensarePerioada(List<string> ani, DateTime inceput, DateTime sfarsit)
        {
            List<object> rez = new List<object>();

            decimal valCuTVA = 0m;
            decimal valFaraTVA = 0m;
            decimal valTVA = 0m;
            int cnt = 0;
            decimal tvaMediu = 0m;
            //valoarea e valoarea fara tva
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DATA_EMIT,VALOARE,CURS FROM [COMPENS.dbf] WHERE TIP_OP = 'P' AND CF_PAR IS NOT NULL";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        if (r.ItemArray.ElementAt(0).ToString() != "")
                        {
                            if (inceput <= DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) && DateTime.Parse(r.ItemArray.ElementAt(0).ToString()) <= sfarsit)
                            {

                                valFaraTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(2).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(2).ToString()) : 1m);

                                cnt++;

                            }
                        }
                    }

                    valCuTVA = valFaraTVA;

                    if (valFaraTVA != 0)
                        tvaMediu = (100 * valTVA) / valFaraTVA;

                    conn.Close();


                }
                catch { }
            }

            rez.Add(valCuTVA);
            rez.Add(valFaraTVA);
            rez.Add(valTVA);
            rez.Add(cnt);
            rez.Add(tvaMediu);

            return rez;

        }

        public static List<object> DBFTotalPlati(List<string> ani, DateTime inceput, DateTime sfarsit)
        {
            List<object> rez = new List<object>();

            decimal valCuTVA = 0m;
            decimal valFaraTVA = 0m;
            decimal valTVA = 0m;
            int cnt = 0;
            decimal tvaMediu = 0m;

            try
            {
                valCuTVA += (decimal)DBFTotalPlatiBancarePerioada(ani, inceput, sfarsit)[0];
                valCuTVA += (decimal)DBFTotalPlatiMonetarePerioada(ani, inceput, sfarsit)[0];
                valCuTVA += (decimal)DBFTotalPlatiCompensarePerioada(ani, inceput, sfarsit)[0];

                valFaraTVA += (decimal)DBFTotalPlatiBancarePerioada(ani, inceput, sfarsit)[1];
                valFaraTVA += (decimal)DBFTotalPlatiMonetarePerioada(ani, inceput, sfarsit)[1];
                valFaraTVA += (decimal)DBFTotalPlatiCompensarePerioada(ani, inceput, sfarsit)[1];

                valTVA += (decimal)DBFTotalPlatiBancarePerioada(ani, inceput, sfarsit)[2];
                valTVA += (decimal)DBFTotalPlatiMonetarePerioada(ani, inceput, sfarsit)[2];
                valTVA += (decimal)DBFTotalPlatiCompensarePerioada(ani, inceput, sfarsit)[2];

                cnt += (int)DBFTotalPlatiBancarePerioada(ani, inceput, sfarsit)[3];
                cnt += (int)DBFTotalPlatiMonetarePerioada(ani, inceput, sfarsit)[3];
                cnt += (int)DBFTotalPlatiCompensarePerioada(ani, inceput, sfarsit)[3];

                if (valFaraTVA != 0)
                    tvaMediu = (100 * valTVA) / valFaraTVA;

            }
            catch { }

            rez.Add(valCuTVA);
            rez.Add(valFaraTVA);
            rez.Add(valTVA);
            rez.Add(cnt);
            rez.Add(tvaMediu);

            return rez;

        }


        
        public static List<string> DBFCautaFirmeFacturi(List<string> ani, string numarFactura)
        {
            return DBFCautaFirmaFacturiiClient(ani, numarFactura).Union(DBFCautaFirmaFacturiiFurnizor(ani, numarFactura)).ToList();
        }

        public static List<string> DBFCautaFirmaFacturiiClient(List<string> ani, string numarFactura)
        {
            List<string> firme = new List<string>();
            //valoarea e valoarea fara tva
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DEN_PAR FROM [FACTURI.dbf] WHERE NR_DOC LIKE '" + numarFactura + "%'";


                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        firme.Add(r.ItemArray.ElementAt(0).ToString());
                    }

                    conn.Close();
                }
                catch { }
            }

            return firme;
        }
        
        public static List<string> DBFCautaFirmaFacturiiFurnizor(List<string> ani, string numarFactura)
        {
            List<string> firme = new List<string>();
            //valoarea e valoarea fara tva
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DEN_PAR FROM [RECEPTII.dbf] WHERE NR_DOC LIKE '" + numarFactura + "%'";


                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        firme.Add(r.ItemArray.ElementAt(0).ToString());
                    }

                    conn.Close();
                }
                catch { }
            }

            return firme;
        }



        public static List<string> DBFCautaFirmeTranzactii(List<string> ani, string numarTranzactie)
        {
            return DBFCautaFirmeTranzactiiBancare(ani, numarTranzactie).Union(DBFCautaFirmeTranzactiiMonetare(ani, numarTranzactie)).Union(DBFCautaFirmeTranzactiiCompensare(ani, numarTranzactie)).ToList();
        }
        
        public static List<string> DBFCautaFirmeTranzactiiBancare(List<string> ani, string numarTranzactie)
        {
            List<string> firme = new List<string>();
            //valoarea e valoarea fara tva
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DEN_PAR FROM [BANCA.dbf] WHERE NR_DOC LIKE '" + numarTranzactie + "%'";


                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        firme.Add(r.ItemArray.ElementAt(0).ToString());
                    }

                    conn.Close();
                }
                catch { }
            }

            return firme;
        }

        public static List<string> DBFCautaFirmeTranzactiiMonetare(List<string> ani, string numarTranzactie)
        {
            List<string> firme = new List<string>();
            //valoarea e valoarea fara tva
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DEN_PAR FROM [CASA.dbf] WHERE NR_DOC LIKE '" + numarTranzactie + "%'";


                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        firme.Add(r.ItemArray.ElementAt(0).ToString());
                    }

                    conn.Close();
                }
                catch { }
            }

            return firme;
        }

        public static List<string> DBFCautaFirmeTranzactiiCompensare(List<string> ani, string numarTranzactie)
        {
            List<string> firme = new List<string>();
            //valoarea e valoarea fara tva
            for (int i = 0; i < ani.Count; i++)
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Settings.ConnectionStringDBFAn(ani[i]));

                    conn.Open();

                    string strQuery = "SELECT DEN_PAR FROM [COMPENS.dbf] WHERE NR_DOC LIKE '" + numarTranzactie + "%'";


                    OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

                    System.Data.DataSet ds = new System.Data.DataSet();


                    adapter.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        firme.Add(r.ItemArray.ElementAt(0).ToString());
                    }

                    conn.Close();
                }
                catch { }
            }

            return firme;
        }



        

        /*
          totalValoareFaraTVA += decimal.Parse(r.ItemArray.ElementAt(0).ToString()) * (r.ItemArray.ElementAt(2).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(2).ToString()) : 1m);

                totalValoareCuTVA += decimal.Parse(r.ItemArray.ElementAt(0).ToString()) * (r.ItemArray.ElementAt(2).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(2).ToString()) : 1m) + 
                    decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(2).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(2).ToString()) : 1m);

                totalTVA += decimal.Parse(r.ItemArray.ElementAt(1).ToString()) * (r.ItemArray.ElementAt(2).ToString() != "" ? decimal.Parse(r.ItemArray.ElementAt(2).ToString()) : 1m);
                
         */


        /*public static DataTable GetDataTableDBF()
        {

            // OleDbConnection conn = new OleDbConnection(@"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = d:\csc\nextquar;
            // Extended Properties = dBASE IV; User ID = Admin; Password =;");

            OleDbConnection conn = new OleDbConnection(GlobalConfig.CnnString("Dbase"));

            conn.Open();

            string strQuery = "SELECT DISTINCT DEN_PAR,CF_PAR FROM [FACTURI.dbf]";

            OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);

            System.Data.DataSet ds = new System.Data.DataSet();

            adapter.Fill(ds);

            return ds.Tables[0];

        }*/
    }
}