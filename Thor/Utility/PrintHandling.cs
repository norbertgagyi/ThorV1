using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thor.Models;
using Thor.DataAccess;
using System.Drawing;
using System.Drawing.Printing;

namespace Thor.Utility
{
    public static class PrintHandling
    {
        public static int i = 0;
        public static int j = 0; //pentru tabele, i - contorul general, j contorul pentru cate se vor afisa in tabel pe o pagina
        public static int p = 1; //pagina
        public static int k = 0; //alt contor

        public static void PrintListaFacturiSiIncasariPerioada(PrintPageEventArgs e, Firma f, DateTime inceput, DateTime sfarsit)
        {
            int leftMargin = 50;
            
            int inaltimeRand = 18;

            int maxRanduriPePagina = 52;

            int inceputTabel = 100;
            
            int yTabelCurent = 0; //inaltimea din tabel la care ma aflu - cand se face o pagina noua incepe din nou de la 0

            f.FacturiClient = DbaseConnection.DBFCitesteFacturiClientFirma(f, Settings.Ani_InUz);

            f.Tranzactii = DbaseConnection.DBFCitesteTranzactiiFirma(f, Settings.Ani_InUz);

            List<Factura> facPer = f.FacturiClientPePerioada(inceput, sfarsit);

            List<Tranzactie> incasari = f.IncasariPePerioada(inceput, sfarsit);

            decimal totalFara = 0;
            decimal totalTVA = 0;
            decimal totalCu = 0;

            int totalPagini = 1 + facPer.Count / maxRanduriPePagina;

            decimal totalIncasari = 0;

            foreach(Factura fa in facPer)
            {
                totalFara += fa.Valoare;
                totalTVA += fa.TVA;
                totalCu += fa.Valoare_Cu_TVA;
            }

            foreach(Tranzactie t in incasari)
            {
                totalIncasari += t.Valoare;
            }

            #region toatePaginile

            e.Graphics.DrawString(f.Nume, new Font("Calibri", 13, FontStyle.Bold), Brushes.Black, new Point(leftMargin, 40));
            e.Graphics.DrawString("Lista facturilor platite din perioada " + inceput.ToShortDateString() + " - " + sfarsit.ToShortDateString(), new Font("Calibri", 11, FontStyle.Italic), Brushes.Black, new Point(380, 42));

            e.Graphics.DrawString("Data tiparirii: " + DateTime.Now, new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(leftMargin, e.MarginBounds.Bottom + 20));
            e.Graphics.DrawString("pag. " + p + @"/" + totalPagini, new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(e.MarginBounds.Right, e.MarginBounds.Bottom + 20));


            #endregion

            #region tabel - pe fiecare pagina apare

            e.Graphics.DrawString("Serie doc.", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin, inceputTabel - 20));
            e.Graphics.DrawString("Data emit.", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 100, inceputTabel - 20));
            e.Graphics.DrawString("Data scad.", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 180, inceputTabel - 20));
            e.Graphics.DrawString("Valoare", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 260, inceputTabel - 20));
            e.Graphics.DrawString("TVA", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 350, inceputTabel - 20));
            e.Graphics.DrawString("Val. cu TVA", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 430, inceputTabel - 20));
           

            if (facPer.Count > 0)
            {
                while (i < facPer.Count)
                {
                    if (i % 2 == 0)
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(242, 242, 242)), 50, inceputTabel - 1 + yTabelCurent, 700, inaltimeRand);

                    e.Graphics.DrawString(facPer[i].Seria + facPer[i].Numar_Document, new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(facPer[i].Data_Emitere.ToShortDateString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 100, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(facPer[i].Data_Scadenta.ToShortDateString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 180, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(facPer[i].Valoare.ToString("0.00"), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 260, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(facPer[i].TVA.ToString("0.00"), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 350, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(facPer[i].Valoare_Cu_TVA.ToString("0.00"), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 430, inceputTabel + yTabelCurent));
                    
                   // e.Graphics.DrawString(facPer[i].Valoare_Cu_TVA.ToString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 520, inceputTabel + yTabelCurent));


                    i++; //incrementez contorul general in orice caz, cu asta se trece peste toate, neavand importanta cate intrari vreau in pagina
                    yTabelCurent += inaltimeRand; //trec pe urmatorul rand

                    if (j < maxRanduriPePagina) // verific daca am trecut cu contorul care merge pana la maximul de randuri din pagina
                    {
                        j++; //daca nu, il incrementez
                        e.HasMorePages = false; // si ii spun ca nu va fi o alta pagina
                    }
                    else // daca da
                    {
                        j = 0; //resetez contorul
                        e.HasMorePages = true; //si ii spun sa creeaze o pagina noua
                        p++;
                        return; //returnand aici, functia se va mai executa o data, cu variabilele globale in starea adecvata
                    }
                }
            }

            #endregion

            #region ultimaPagina - poate fi prima si ultima

            e.Graphics.DrawString("Totaluri", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 180, inceputTabel + yTabelCurent + inaltimeRand));
            e.Graphics.DrawString(totalFara.ToString("0.00"), new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 260, inceputTabel + yTabelCurent + inaltimeRand));
            e.Graphics.DrawString(totalTVA.ToString("0.00"), new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 350, inceputTabel + yTabelCurent + inaltimeRand));
            e.Graphics.DrawString(totalCu.ToString("0.00"), new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 430, inceputTabel + yTabelCurent + inaltimeRand));
            
            #endregion

        }

        public static void PrintFacturaClient(PrintPageEventArgs e, Firma firma, Factura f)
        {
            int leftMargin = 50;

            int inaltimeRand = 18;

            int maxRanduriPePagina = 46;

            int inceputTabel = 220;

            int yTabelCurent = 0; //inaltimea din tabel la care ma aflu - cand se face o pagina noua incepe din nou de la 0

            List<Produs> produse = DbaseConnection.DBFCitesteProduseFacturaClient(f, Settings.Ani_InUz);

            int totalPagini = 1 + produse.Count / maxRanduriPePagina;
            
            #region toatePaginile

            e.Graphics.DrawString("Societatea: " + firma.Nume, new Font("Calibri", 13, FontStyle.Bold), Brushes.Black, new Point(leftMargin, 40));
            e.Graphics.DrawString("Factura: " + f.Seria + f.Numar_Document, new Font("Calibri", 11, FontStyle.Bold), Brushes.Black, new Point(leftMargin, 70));
            e.Graphics.DrawString("Data emiterii: " + f.Data_Emitere.ToShortDateString(), new Font("Calibri", 11, FontStyle.Italic), Brushes.Black, new Point(leftMargin, 98));
            e.Graphics.DrawString("Data scadentei: " + f.Data_Scadenta.ToShortDateString(), new Font("Calibri", 11, FontStyle.Italic), Brushes.Black, new Point(leftMargin, 116));

            e.Graphics.DrawString("Total fara TVA: " + f.Valoare.ToString("0.00") + " " + f.Moneda, new Font("Calibri", 11, FontStyle.Italic), Brushes.Black, new Point(leftMargin, 148));
            e.Graphics.DrawString("TVA: " + f.TVA.ToString("0.00") + " " + f.Moneda, new Font("Calibri", 11, FontStyle.Italic), Brushes.Black, new Point(leftMargin, 166));

            e.Graphics.DrawString("Data tiparirii: " + DateTime.Now, new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(leftMargin, e.MarginBounds.Bottom + 20));
            e.Graphics.DrawString("pag. " + p + @"/" + totalPagini, new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(e.MarginBounds.Right, e.MarginBounds.Bottom + 20));


            #endregion

            #region tabel - pe fiecare pagina apare

            e.Graphics.DrawString("Denumire produs", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin, inceputTabel - 20));
            e.Graphics.DrawString("Pret/UM", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 400, inceputTabel - 20));
            e.Graphics.DrawString("Cantitate", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 470, inceputTabel - 20));
            e.Graphics.DrawString("Valoare", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 560, inceputTabel - 20));
            e.Graphics.DrawString("TVA", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 640, inceputTabel - 20));

            if (produse.Count > 0)
            {
                while (i < produse.Count)
                {
                    if (i % 2 == 0)
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(242, 242, 242)), 50, inceputTabel - 1 + yTabelCurent, 700, inaltimeRand);

                    e.Graphics.DrawString(produse[i].Denumire_Produs, new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(produse[i].Pret_Vanzare.ToString("0.00"), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 400, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(produse[i].Cantitate.ToString() + " " + produse[i].Unitate_Masura, new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 470, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(produse[i].Valoare.ToString("0.00"), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 560, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(produse[i].TVA.ToString("0.00"), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 640, inceputTabel + yTabelCurent));
                    
                    // e.Graphics.DrawString(facPer[i].Valoare_Cu_TVA.ToString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 520, inceputTabel + yTabelCurent));


                    i++; //incrementez contorul general in orice caz, cu asta se trece peste toate, neavand importanta cate intrari vreau in pagina
                    yTabelCurent += inaltimeRand; //trec pe urmatorul rand

                    if (j < maxRanduriPePagina) // verific daca am trecut cu contorul care merge pana la maximul de randuri din pagina
                    {
                        j++; //daca nu, il incrementez
                        e.HasMorePages = false; // si ii spun ca nu va fi o alta pagina
                    }
                    else // daca da
                    {
                        j = 0; //resetez contorul
                        e.HasMorePages = true; //si ii spun sa creeaze o pagina noua
                        p++;
                        return; //returnand aici, functia se va mai executa o data, cu variabilele globale in starea adecvata
                    }
                }
            }

            #endregion

            #region ultimaPagina - poate fi prima si ultima

            e.Graphics.DrawString(f.Valoare.ToString("0.00"), new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 560, inceputTabel + yTabelCurent + inaltimeRand));
            e.Graphics.DrawString(f.TVA.ToString("0.00"), new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 640, inceputTabel + yTabelCurent + inaltimeRand));
            
            #endregion

        }

        public static void PrintFacturaFurnizor(PrintPageEventArgs e, Firma firma, Factura f)
        {
            int leftMargin = 50;

            int inaltimeRand = 18;

            int maxRanduriPePagina = 46;

            int inceputTabel = 220;

            int yTabelCurent = 0; //inaltimea din tabel la care ma aflu - cand se face o pagina noua incepe din nou de la 0

            List<Produs> produse = DbaseConnection.DBFCitesteProduseFacturaFurnizor(f, Settings.Ani_InUz);

            int totalPagini = 1 + produse.Count / maxRanduriPePagina;

            #region toatePaginile

            e.Graphics.DrawString("Societatea: " + firma.Nume, new Font("Calibri", 13, FontStyle.Bold), Brushes.Black, new Point(leftMargin, 40));
            e.Graphics.DrawString("Factura: " + f.Seria + f.Numar_Document, new Font("Calibri", 11, FontStyle.Bold), Brushes.Black, new Point(leftMargin, 70));
            e.Graphics.DrawString("Data emiterii: " + f.Data_Emitere.ToShortDateString(), new Font("Calibri", 11, FontStyle.Italic), Brushes.Black, new Point(leftMargin, 98));
            e.Graphics.DrawString("Data scadentei: " + f.Data_Scadenta.ToShortDateString(), new Font("Calibri", 11, FontStyle.Italic), Brushes.Black, new Point(leftMargin, 116));

            e.Graphics.DrawString("Total fara TVA: " + f.Valoare.ToString("0.00") + " " + f.Moneda, new Font("Calibri", 11, FontStyle.Italic), Brushes.Black, new Point(leftMargin, 148));
            e.Graphics.DrawString("TVA: " + f.TVA.ToString("0.00") + " " + f.Moneda, new Font("Calibri", 11, FontStyle.Italic), Brushes.Black, new Point(leftMargin, 166));

            e.Graphics.DrawString("Data tiparirii: " + DateTime.Now, new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(leftMargin, e.MarginBounds.Bottom + 20));
            e.Graphics.DrawString("pag. " + p + @"/" + totalPagini, new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(e.MarginBounds.Right, e.MarginBounds.Bottom + 20));


            #endregion

            #region tabel - pe fiecare pagina apare

            e.Graphics.DrawString("Denumire produs", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin, inceputTabel - 20));
            e.Graphics.DrawString("Pret/UM", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 400, inceputTabel - 20));
            e.Graphics.DrawString("Cantitate", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 470, inceputTabel - 20));
            e.Graphics.DrawString("Valoare", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 560, inceputTabel - 20));
            e.Graphics.DrawString("TVA", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 640, inceputTabel - 20));

            if (produse.Count > 0)
            {
                while (i < produse.Count)
                {
                    if (i % 2 == 0)
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(242, 242, 242)), 50, inceputTabel - 1 + yTabelCurent, 700, inaltimeRand);

                    e.Graphics.DrawString(produse[i].Denumire_Produs, new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(produse[i].Pret_Vanzare.ToString("0.00"), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 400, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(produse[i].Cantitate.ToString() + " " + produse[i].Unitate_Masura, new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 470, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(produse[i].Valoare.ToString("0.00"), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 560, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(produse[i].TVA.ToString("0.00"), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 640, inceputTabel + yTabelCurent));

                    // e.Graphics.DrawString(facPer[i].Valoare_Cu_TVA.ToString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 520, inceputTabel + yTabelCurent));


                    i++; //incrementez contorul general in orice caz, cu asta se trece peste toate, neavand importanta cate intrari vreau in pagina
                    yTabelCurent += inaltimeRand; //trec pe urmatorul rand

                    if (j < maxRanduriPePagina) // verific daca am trecut cu contorul care merge pana la maximul de randuri din pagina
                    {
                        j++; //daca nu, il incrementez
                        e.HasMorePages = false; // si ii spun ca nu va fi o alta pagina
                    }
                    else // daca da
                    {
                        j = 0; //resetez contorul
                        e.HasMorePages = true; //si ii spun sa creeaze o pagina noua
                        p++;
                        return; //returnand aici, functia se va mai executa o data, cu variabilele globale in starea adecvata
                    }
                }
            }

            #endregion

            #region ultimaPagina - poate fi prima si ultima

            e.Graphics.DrawString(f.Valoare.ToString("0.00"), new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 560, inceputTabel + yTabelCurent + inaltimeRand));
            e.Graphics.DrawString(f.TVA.ToString("0.00"), new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 640, inceputTabel + yTabelCurent + inaltimeRand));

            #endregion

        }
        
        public static void PrintSituatieRulajIncasariClient(PrintPageEventArgs e, Firma firma, DateTime inceput, DateTime sfarsit)
        {
            List<decimal> rulajLuni = firma.RulajClientLuniDinPerioada(inceput, sfarsit);
            List<decimal> incasariLuni = firma.IncasariClientLuniDinPerioada(inceput, sfarsit);

            List<int> nrFacturiLuni = firma.NumarFacturiClientLuniDinPerioada(inceput, sfarsit);
            List<int> nrIncasariLuni = firma.NumarIncasariLuniDinPerioada(inceput, sfarsit);

            int leftMargin = 50;

            int inaltimeRand = 18;

            int maxRanduriPePagina = 33;

            int inceputTabel = 430;

            int yTabelCurent = 0; //inaltimea din tabel la care ma aflu - cand se face o pagina noua incepe din nou de la 0

            int totalPagini = 1 + rulajLuni.Count / maxRanduriPePagina;
            
            Pen solid = new Pen(Brushes.Black);
            Pen dashed = new Pen(Brushes.Black);


            dashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            dashed.DashPattern = new float[] { 2, 3 };

            #region toatePaginile

            e.Graphics.DrawString("Societatea: " + firma.Nume, new Font("Calibri", 13, FontStyle.Bold), Brushes.Black, new Point(leftMargin, 40));
            e.Graphics.DrawString("Situatie client rulaj - incasari lunare pentru " + inceput.ToShortDateString() + " - " + sfarsit.ToShortDateString(), new Font("Calibri", 11, FontStyle.Italic), Brushes.Black, new Point(380, 42));
            
            e.Graphics.DrawString("Data tiparirii: " + DateTime.Now, new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(leftMargin, e.MarginBounds.Bottom + 20));
            e.Graphics.DrawString("pag. " + p + @"/" + totalPagini, new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(e.MarginBounds.Right, e.MarginBounds.Bottom + 20));

            #endregion

            #region grafic

            Point startA = new Point(e.MarginBounds.Right - 40, 120);
            Point stopA = new Point(e.MarginBounds.Right - 20, 120);

            Point startB = new Point(e.MarginBounds.Right - 40, 140);
            Point stopB = new Point(e.MarginBounds.Right - 20, 140);

            e.Graphics.DrawLine(solid, startA, stopA);
            e.Graphics.DrawLine(dashed, startB, stopB);

            e.Graphics.DrawString("Rulaj", new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(e.MarginBounds.Right - 10, 110));
            e.Graphics.DrawString("Incasari", new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(e.MarginBounds.Right - 10, 130));
            
            GraphicLibrary.GraficLinie(e.Graphics, leftMargin + 10, 130, e.MarginBounds.Right, 200, rulajLuni, GraphicLibrary.LuniPrescurtatePerioada(inceput, sfarsit), Color.Black, false, "Incasari", "Luni", true);
            GraphicLibrary.GraficLinie(e.Graphics, leftMargin + 10, 130, e.MarginBounds.Right, 200, incasariLuni, GraphicLibrary.LuniPrescurtatePerioada(inceput, sfarsit), Color.Black, true, "Incasari", "Luni", false);

            #endregion

            #region statistici

            e.Graphics.DrawString("Luna", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin, inceputTabel - 20));
            e.Graphics.DrawString("Rulaj", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 100, inceputTabel - 20));
            e.Graphics.DrawString("Incasari", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 200, inceputTabel - 20));
            e.Graphics.DrawString("Numar facturi", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 300, inceputTabel - 20));
            e.Graphics.DrawString("Numar incasari", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 400, inceputTabel - 20));


            if (rulajLuni.Count > 0)
            {
                while (i < rulajLuni.Count)
                {
                    if (i % 2 == 0)
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(242, 242, 242)), 50, inceputTabel - 1 + yTabelCurent, e.MarginBounds.Right, inaltimeRand);

                    e.Graphics.DrawString(GraphicLibrary.LuniSiAnPrescurtatePerioada(inceput, sfarsit)[i].ToString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(rulajLuni[i].ToString("0.00"), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 100, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(incasariLuni[i].ToString("0.00"), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 200, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(nrFacturiLuni[i].ToString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 300, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(nrIncasariLuni[i].ToString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 400, inceputTabel + yTabelCurent));


                    // e.Graphics.DrawString(facPer[i].Valoare_Cu_TVA.ToString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 520, inceputTabel + yTabelCurent));


                    i++; //incrementez contorul general in orice caz, cu asta se trece peste toate, neavand importanta cate intrari vreau in pagina
                    yTabelCurent += inaltimeRand; //trec pe urmatorul rand

                    if (j < maxRanduriPePagina) // verific daca am trecut cu contorul care merge pana la maximul de randuri din pagina
                    {
                        j++; //daca nu, il incrementez
                        e.HasMorePages = false; // si ii spun ca nu va fi o alta pagina
                    }
                    else // daca da
                    {
                        j = 0; //resetez contorul
                        e.HasMorePages = true; //si ii spun sa creeaze o pagina noua
                        p++;
                        return; //returnand aici, functia se va mai executa o data, cu variabilele globale in starea adecvata
                    }
                }
            }


            #endregion

        }

        public static void PrintSituatieRulajPlatiFurnizor(PrintPageEventArgs e, Firma firma, DateTime inceput, DateTime sfarsit)
        {
            List<decimal> rulajLuni = firma.RulajFurnizorLuniDinPerioada(inceput, sfarsit);
            List<decimal> platiLuni = firma.PlatiFurnizorLuniDinPerioada(inceput, sfarsit);

            List<int> nrFacturiLuni = firma.NumarFacturiFurnizorLuniDinPerioada(inceput, sfarsit);
            List<int> nrPlatiLuni = firma.NumarPlatiLuniDinPerioada(inceput, sfarsit);

            int leftMargin = 50;

            int inaltimeRand = 18;

            int maxRanduriPePagina = 33;

            int inceputTabel = 430;

            int yTabelCurent = 0; //inaltimea din tabel la care ma aflu - cand se face o pagina noua incepe din nou de la 0

            int totalPagini = 1 + rulajLuni.Count / maxRanduriPePagina;

            Pen solid = new Pen(Brushes.Black);
            Pen dashed = new Pen(Brushes.Black);


            dashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            dashed.DashPattern = new float[] { 2, 3 };

            #region toatePaginile

            e.Graphics.DrawString("Societatea: " + firma.Nume, new Font("Calibri", 13, FontStyle.Bold), Brushes.Black, new Point(leftMargin, 40));
            e.Graphics.DrawString("Situatie furnizor rulaj - plati lunare pentru " + inceput.ToShortDateString() + " - " + sfarsit.ToShortDateString(), new Font("Calibri", 11, FontStyle.Italic), Brushes.Black, new Point(380, 42));

            e.Graphics.DrawString("Data tiparirii: " + DateTime.Now, new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(leftMargin, e.MarginBounds.Bottom + 20));
            e.Graphics.DrawString("pag. " + p + @"/" + totalPagini, new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(e.MarginBounds.Right, e.MarginBounds.Bottom + 20));

            #endregion

            #region grafic

            Point startA = new Point(e.MarginBounds.Right - 40, 120);
            Point stopA = new Point(e.MarginBounds.Right - 20, 120);

            Point startB = new Point(e.MarginBounds.Right - 40, 140);
            Point stopB = new Point(e.MarginBounds.Right - 20, 140);

            e.Graphics.DrawLine(solid, startA, stopA);
            e.Graphics.DrawLine(dashed, startB, stopB);

            e.Graphics.DrawString("Rulaj", new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(e.MarginBounds.Right - 10, 110));
            e.Graphics.DrawString("Plati", new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(e.MarginBounds.Right - 10, 130));

            GraphicLibrary.GraficLinie(e.Graphics, leftMargin + 10, 130, e.MarginBounds.Right, 200, rulajLuni, GraphicLibrary.LuniPrescurtatePerioada(inceput, sfarsit), Color.Black, false, "Plati", "Luni", true);
            GraphicLibrary.GraficLinie(e.Graphics, leftMargin + 10, 130, e.MarginBounds.Right, 200, platiLuni, GraphicLibrary.LuniPrescurtatePerioada(inceput, sfarsit), Color.Black, true, "Plati", "Luni", false);

            #endregion

            #region statistici

            e.Graphics.DrawString("Luna", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin, inceputTabel - 20));
            e.Graphics.DrawString("Rulaj", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 100, inceputTabel - 20));
            e.Graphics.DrawString("Plati", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 200, inceputTabel - 20));
            e.Graphics.DrawString("Numar facturi", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 300, inceputTabel - 20));
            e.Graphics.DrawString("Numar plati", new Font("Calibri", 10, FontStyle.Bold), Brushes.Black, new Point(leftMargin + 400, inceputTabel - 20));


            if (rulajLuni.Count > 0)
            {
                while (i < rulajLuni.Count)
                {
                    if (i % 2 == 0)
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(242, 242, 242)), 50, inceputTabel - 1 + yTabelCurent, e.MarginBounds.Right, inaltimeRand);

                    e.Graphics.DrawString(GraphicLibrary.LuniSiAnPrescurtatePerioada(inceput, sfarsit)[i].ToString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(rulajLuni[i].ToString("0.00"), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 100, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(platiLuni[i].ToString("0.00"), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 200, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(nrFacturiLuni[i].ToString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 300, inceputTabel + yTabelCurent));
                    e.Graphics.DrawString(nrPlatiLuni[i].ToString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 400, inceputTabel + yTabelCurent));


                    // e.Graphics.DrawString(facPer[i].Valoare_Cu_TVA.ToString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, new Point(leftMargin + 520, inceputTabel + yTabelCurent));


                    i++; //incrementez contorul general in orice caz, cu asta se trece peste toate, neavand importanta cate intrari vreau in pagina
                    yTabelCurent += inaltimeRand; //trec pe urmatorul rand

                    if (j < maxRanduriPePagina) // verific daca am trecut cu contorul care merge pana la maximul de randuri din pagina
                    {
                        j++; //daca nu, il incrementez
                        e.HasMorePages = false; // si ii spun ca nu va fi o alta pagina
                    }
                    else // daca da
                    {
                        j = 0; //resetez contorul
                        e.HasMorePages = true; //si ii spun sa creeaze o pagina noua
                        p++;
                        return; //returnand aici, functia se va mai executa o data, cu variabilele globale in starea adecvata
                    }
                }
            }


            #endregion

        }

        public static void PrintComanda(PrintPageEventArgs e, Firma firma, Comanda comanda)
        {

            int latimeTabel = e.MarginBounds.Width;

            int inaltimeTabel = e.MarginBounds.Height / 2;
            
            int inaltimeCapTabel = 15;

            int latimePrimaColoana = 30;

            Rectangle[,] rectangles = new Rectangle[4, 4];

            rectangles[0, 0] = new Rectangle(e.MarginBounds.Left, e.MarginBounds.Top, latimePrimaColoana, inaltimeCapTabel);

            e.Graphics.DrawRectangle(new Pen(Brushes.Black), rectangles[0, 0]);

            int latimeCelula = (latimeTabel - latimePrimaColoana) / 3;
            int inaltimeCelula = (inaltimeTabel - inaltimeCapTabel) / 3;

            int xTabelCurent = e.MarginBounds.Left + latimePrimaColoana;
            
            int yTabelCurent = e.MarginBounds.Top + inaltimeCapTabel;
            
            //cap tabel

            for(int j = 1; j < 4; j++)
            {
                rectangles[0, j] = new Rectangle(xTabelCurent, e.MarginBounds.Top, latimeCelula, inaltimeCapTabel);
                xTabelCurent += latimeCelula;

                e.Graphics.DrawRectangle(new Pen(Brushes.Black), rectangles[0, j]);
            }

            //margine tabel

            for(int i = 1; i < 4; i++)
            {
                rectangles[i, 0] = new Rectangle(e.MarginBounds.Left, yTabelCurent, latimePrimaColoana, inaltimeCelula);
                yTabelCurent += inaltimeCelula;

                e.Graphics.DrawRectangle(new Pen(Brushes.Black), rectangles[i, 0]);
            }
            
            xTabelCurent = e.MarginBounds.Left + latimePrimaColoana;

            yTabelCurent = e.MarginBounds.Top + inaltimeCapTabel;

            for (int i = 1; i < 4; i++)
            {
                xTabelCurent = e.MarginBounds.Left + latimePrimaColoana;

                for(int j = 1; j < 4; j++)
                {
                    rectangles[i, j] = new Rectangle(xTabelCurent, yTabelCurent, latimeCelula, inaltimeCelula);

                    xTabelCurent += latimeCelula;

                    e.Graphics.DrawRectangle(new Pen(Brushes.Black), rectangles[i, j]);
                }

                yTabelCurent += inaltimeCelula;
            }

            

            //  int totalPagini = 1 + rulajLuni.Count / maxRanduriPePagina;

            e.Graphics.DrawString("Data tiparirii: " + DateTime.Now, new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(e.MarginBounds.Left, e.MarginBounds.Bottom + 20));
            // e.Graphics.DrawString("pag. " + p + @"/" + totalPagini, new Font("Calibri", 10, FontStyle.Italic), Brushes.Black, new Point(e.MarginBounds.Right, e.MarginBounds.Bottom + 20));

            e.Graphics.DrawString("Nr", new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, rectangles[0, 0]);
            e.Graphics.DrawString("Client", new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, rectangles[0, 1]);
            e.Graphics.DrawString("Tip/Dimensiune", new Font("Calibri", 10, FontStyle.Regular), Brushes.Black,200,100);
            e.Graphics.DrawString("Observatii", new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, rectangles[0, 3]);

            e.Graphics.DrawString(comanda.Id.ToString(), new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, 20,20);
            e.Graphics.DrawString(comanda.Nume_Firma, new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, 200,200);
            e.Graphics.DrawString(comanda.Imprimeu, new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, 250,200);

            e.HasMorePages = false;

        }
    }
}
