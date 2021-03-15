using System;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thor.Models;

namespace Thor.DataAccess
{
    public static class SqlConnection
    {
        /* public static void ReseteazaBazaDateSiRescrie()
         {
             List<Firma> firme;

             firme = DbaseConnection.DBFCitesteFirme();

             using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Parteneri")))
             {
                 connection.Execute("dbo.ReseteazaBazaDate", commandType: CommandType.StoredProcedure);
             }

             foreach (Firma f in firme)
             {
                 f.Facturi = DbaseConnection.DBFCitesteFacturiFirma(f);
                 f.Tranzactii = DbaseConnection.DBFCitesteTranzactiiFirma(f);

                 if (f.Facturi != null)
                     foreach (Factura fac in f.Facturi)
                     {
                         fac.Produse = DbaseConnection.DBFCitesteProduseFactura(fac);

                         foreach (Produs prod in fac.Produse)
                         {
                             CreeazaProdus(prod);
                         }

                         CreeazaFactura(fac);
                     }

                 if (f.Tranzactii != null)
                     foreach (Tranzactie tr in f.Tranzactii)
                     {
                         CreeazaTranzactie(tr);
                     }

                 if (f.Conturi_Bancare != null)
                     foreach (ContBancar cb in f.Conturi_Bancare)
                     {
                         CreeazaContBancar(cb);
                     }

                 CreeazaFirma(f);
             }
         }*/

        /* public static Firma CreeazaFirma(Firma firma)
         {
             using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Parteneri")))
             {
                 var p = new DynamicParameters();
                 p.Add("@nume", firma.Nume);
                 p.Add("@email", firma.Email);
                 p.Add("@telefon", firma.Telefon);
                 p.Add("@registrul_comertului", firma.Registrul_Comertului);
                 p.Add("@cui", firma.CUI);
                 p.Add("@rulaj", firma.Rulaj);
                 p.Add("@fax", firma.Fax);
                 p.Add("@tara", firma.Tara);
                 p.Add("@judet", firma.Judet);
                 p.Add("@localitate", firma.Localitate);
                 p.Add("@adresa", firma.Adresa);
                 p.Add("@cod_postal", firma.Cod_Postal);
                 p.Add("@datorie", firma.Datorie);

                 p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                 connection.Execute("dbo.CreeazaFirma", p, commandType: CommandType.StoredProcedure);

                 firma.Id = p.Get<int>("@id");


                 if (firma.Conturi_Bancare != null)
                     foreach (ContBancar cb in firma.Conturi_Bancare)
                     {
                         var pa = new DynamicParameters();
                         pa.Add("@idFirma", firma.Id);
                         pa.Add("@idCont", cb.Id);

                         connection.Execute("dbo.CreeazaFirmaContBancar", pa, commandType: CommandType.StoredProcedure);

                     }

                 if (firma.Facturi != null)
                     foreach (Factura fac in firma.Facturi)
                     {
                         var pa = new DynamicParameters();
                         pa.Add("@idFirma", firma.Id);
                         pa.Add("@idFactura", fac.Id);

                         connection.Execute("dbo.CreeazaFirmaFactura", pa, commandType: CommandType.StoredProcedure);

                     }

                 if (firma.Tranzactii != null)
                     foreach (Tranzactie tr in firma.Tranzactii)
                     {
                         var pa = new DynamicParameters();
                         pa.Add("@idFirma", firma.Id);
                         pa.Add("@idTranzactie", tr.Id);

                         connection.Execute("dbo.CreeazaFirmaTranzactie", pa, commandType: CommandType.StoredProcedure);

                     }


                 return firma;

             }
         } */

        public static Persoana Persoana_Adaugare(Persoana persoana, string cui)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Parteneri")))
            {
                var p = new DynamicParameters();
                p.Add("@nume_prenume", persoana.Nume_Prenume);
                p.Add("@telefon", persoana.Telefon);
                p.Add("@email", persoana.Email);

                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.Persoane_Adaugare", p, commandType: CommandType.StoredProcedure);

                persoana.Id = p.Get<int>("@id");

                var pa = new DynamicParameters();
                pa.Add("@id_persoana", persoana.Id);
                pa.Add("@cui_firma", cui);
                pa.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("dbo.FirmaPersoana_Adaugare", pa, commandType: CommandType.StoredProcedure);

                return persoana;
            }
        }

        public static List<Persoana> CitestePersoane_DupaCUI(string cui)
        {
            List<Persoana> persoane;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Parteneri")))
            {
               // firme = connection.Query<Firma>("SELECT * FROM dbo.Firme WHERE Nume LIKE '%" + nume + "%' ORDER BY Rulaj DESC").ToList();

                var p = new DynamicParameters();
                p.Add("@cui_firma", cui);
                persoane = connection.Query<Persoana>("dbo.Persoane_CitireDupaCUI", p, commandType: CommandType.StoredProcedure).ToList();
            }

            return persoane;
        }
        
        public static ContBancar ContBancar_Adaugare(ContBancar cont, string cui)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Parteneri")))
            {
                var p = new DynamicParameters();
                p.Add("@iban", cont.IBAN);
                p.Add("@banca", cont.Banca);
                p.Add("@moneda", cont.Moneda_Cont);

                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.ConturiBancare_Adaugare", p, commandType: CommandType.StoredProcedure);

                cont.Id = p.Get<int>("@id");

                var pa = new DynamicParameters();
                pa.Add("@id_cont", cont.Id);
                pa.Add("@cui_firma", cui);
                pa.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("dbo.FirmaContBancar_Adaugare", pa, commandType: CommandType.StoredProcedure);

                return cont;
            }
        }

        public static List<ContBancar> CitesteConturiBancare_DupaCUI(string cui)
        {
            List<ContBancar> conturi;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Parteneri")))
            {
                // firme = connection.Query<Firma>("SELECT * FROM dbo.Firme WHERE Nume LIKE '%" + nume + "%' ORDER BY Rulaj DESC").ToList();

                var p = new DynamicParameters();
                p.Add("@cui_firma", cui);
                conturi = connection.Query<ContBancar>("dbo.ConturiBancare_CitireDupaCUI", p, commandType: CommandType.StoredProcedure).ToList();
            }

            return conturi;
        }
    }
}
