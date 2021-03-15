using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thor.DataAccess;
using Thor.Models;
using System.Drawing;


namespace Thor.Utility
{
    public static class GraphicLibrary
    {
        public static List<object> LuniPrescurtatePerioada(DateTime inceput, DateTime sfarsit)
        {
            List<object> rez = new List<object>();

            DateTime cnt = inceput;

            while(cnt.Date <= sfarsit.Date)
            {
                rez.Add(cnt.Month.ToString());
                cnt = cnt.AddMonths(1);
            }

            return rez;
        }

        public static List<object> LuniSiAnPrescurtatePerioada(DateTime inceput, DateTime sfarsit)
        {
            List<object> rez = new List<object>();

            DateTime cnt = inceput;
            
            while (cnt.Date <= sfarsit.Date)
            {

                rez.Add(cnt.Month.ToString() + ((cnt.Month == 1) ? "    " + cnt.Year.ToString() : "").ToString());

                cnt = cnt.AddMonths(1);
            }

            return rez;
        }

        public static List<object> LuniPrescurtate()
        {
            List<object> rez = new List<object>();

            DateTime cnt = new DateTime(2018,1,1);

            while (cnt.Date <= new DateTime(2018, 12, 31))
            {
                rez.Add(cnt.Month.ToString());

                cnt = cnt.AddMonths(1);
            }

            return rez;
        }

        //grafic tip linie caz general
        public static void GraficLinie(Graphics g, int x, int y, int lungime, int inaltime, List<decimal> valoriVerticale, List<object> valoriOrizontale, Color culoareGrafic, bool dashed, string denumireOy, string denumireOx, bool scrieValori)
        {
            Pen black = new Pen(Brushes.Black);
            
            Pen grafic = new Pen(culoareGrafic);
            Pen ggrafic = new Pen(culoareGrafic, 5);

            if (dashed)
            {
                grafic.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                grafic.DashPattern = new float[] { 2, 4 };
            }


            g.DrawLine(black, new Point(x, y + inaltime), new Point(x + lungime, y + inaltime)); //orizontala
            g.DrawLine(black, new Point(x, y), new Point(x, y + inaltime)); //verticala

            g.DrawLine(black, new Point(x + lungime, y + inaltime - 5), new Point(x + lungime, y + inaltime + 5)); //capat orizontala
            g.DrawLine(black, new Point(x - 5, y), new Point(x + 5, y)); //capat verticala

            g.DrawString(denumireOy, new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x - 10, y - 20));
            g.DrawString(denumireOx, new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x + lungime - 15, y + inaltime + 5));

          //  g.DrawString("0", new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x - 5, y + inaltime + 5));

            decimal max = 0;

            for (int i = 0; i < valoriVerticale.Count; i++)
            {
                if (valoriVerticale[i] > max)
                    max = valoriVerticale[i];
            }

            decimal factorDeScalareVertical = (max != 0) ? (decimal)(inaltime - 30) / max : 1;
            decimal factorDeScalareOrizontal = (valoriVerticale.Count != 0) ? (decimal)lungime / valoriVerticale.Count : 1;

            Point lastPoint = new Point(x, y + inaltime - (int)(valoriVerticale[0] * factorDeScalareVertical));

            if (scrieValori)
                g.DrawString(max.ToString(), new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x, y + inaltime - 15 - (int)(max * factorDeScalareVertical)));

            for (int i = 0; i < valoriVerticale.Count; i++)
            {
                Point punctConectare = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical));


                Point punctGros1 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical - 2));
                Point punctGros2 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical + 2));
                

                Point punctLinieOriz1 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - 3); //linii de pe axa Ox
                Point punctLinieOriz2 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime + 3);

                if (scrieValori)
                    g.DrawString(valoriOrizontale[i] as string, new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x + (int)(i * factorDeScalareOrizontal) - 5, y + inaltime));

                Point punctLinieVert1 = new Point(x - 3, y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical)); //linii de pe axa Oy
                Point punctLinieVert2 = new Point(x + 3, y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical));

                

                g.DrawLine(black, punctLinieOriz1, punctLinieOriz2);
                g.DrawLine(black, punctLinieVert1, punctLinieVert2);

                g.DrawLine(ggrafic, punctGros1, punctGros2);
                
                g.DrawLine(grafic, lastPoint, punctConectare);

                lastPoint = punctConectare;

            }
        }

        public static void GraficLinie(Graphics g, int x, int y, int lungime, int inaltime, List<decimal> valoriVerticale, Color culoareGrafic, bool dashed, string denumireOy, string denumireOx, bool scrieValori)
        {
            Pen black = new Pen(Brushes.Black);

            Pen grafic = new Pen(culoareGrafic);
            Pen ggrafic = new Pen(culoareGrafic, 5);

            if (dashed)
            {
                grafic.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                grafic.DashPattern = new float[] { 2, 4 };
            }


            g.DrawLine(black, new Point(x, y + inaltime), new Point(x + lungime, y + inaltime)); //orizontala
            g.DrawLine(black, new Point(x, y), new Point(x, y + inaltime)); //verticala

            g.DrawLine(black, new Point(x + lungime, y + inaltime - 5), new Point(x + lungime, y + inaltime + 5)); //capat orizontala
            g.DrawLine(black, new Point(x - 5, y), new Point(x + 5, y)); //capat verticala

            g.DrawString(denumireOy, new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x - 10, y - 20));
            g.DrawString(denumireOx, new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x + lungime - 15, y + inaltime + 5));

          // g.DrawString("0", new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x - 5, y + inaltime + 5));

            decimal max = 0;

            for (int i = 0; i < valoriVerticale.Count; i++)
            {
                if (valoriVerticale[i] > max)
                    max = valoriVerticale[i];
            }

            decimal factorDeScalareVertical = (max != 0) ? (decimal)(inaltime - 30) / max : 1;
            decimal factorDeScalareOrizontal = (valoriVerticale.Count != 0) ? (decimal)lungime / valoriVerticale.Count : 1;

            Point lastPoint = new Point(x, y + inaltime - (int)((valoriVerticale.Count != 0) ? valoriVerticale[0] : 1 * factorDeScalareVertical));

            if (scrieValori)
                g.DrawString(max.ToString(), new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x, y + inaltime - 15 - (int)(max * factorDeScalareVertical + 10)));

            for (int i = 0; i < valoriVerticale.Count; i++)
            {
                Point punctConectare = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical));


                Point punctGros1 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical - 2));
                Point punctGros2 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical + 2));
                

                Point punctLinieOriz1 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - 3); //linii de pe axa Ox
                Point punctLinieOriz2 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime + 3);

                Point punctLinieVert1 = new Point(x - 3, y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical)); //linii de pe axa Oy
                Point punctLinieVert2 = new Point(x + 3, y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical));

                g.DrawLine(black, punctLinieOriz1, punctLinieOriz2);
                g.DrawLine(black, punctLinieVert1, punctLinieVert2);

                g.DrawLine(ggrafic, punctGros1, punctGros2);

                g.DrawLine(grafic, lastPoint, punctConectare);

                lastPoint = punctConectare;

            }
        }

        public static void GraficLinie(Graphics g, int x, int y, int lungime, int inaltime, List<decimal> valoriVerticale, Color culoareGrafic, bool dashed, string denumireOy, string denumireOx, bool scrieValori, decimal maxPentruScara)
        {
            Pen black = new Pen(Brushes.Black);

            Pen grafic = new Pen(culoareGrafic);
            Pen ggrafic = new Pen(culoareGrafic, 5);

            if (dashed)
            {
                grafic.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                grafic.DashPattern = new float[] { 2, 4 };
            }


            g.DrawLine(black, new Point(x, y + inaltime), new Point(x + lungime, y + inaltime)); //orizontala
            g.DrawLine(black, new Point(x, y), new Point(x, y + inaltime)); //verticala

            g.DrawLine(black, new Point(x + lungime, y + inaltime - 5), new Point(x + lungime, y + inaltime + 5)); //capat orizontala
            g.DrawLine(black, new Point(x - 5, y), new Point(x + 5, y)); //capat verticala

            g.DrawString(denumireOy, new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x - 10, y - 20));
            g.DrawString(denumireOx, new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x + lungime - 15, y + inaltime + 5));

         //   g.DrawString("0", new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x - 5, y + inaltime + 5));

            decimal max = 0;

            for (int i = 0; i < valoriVerticale.Count; i++)
            {
                if (valoriVerticale[i] > max)
                    max = valoriVerticale[i];
            }

            decimal factorDeScalareVertical = (maxPentruScara != 0) ? (decimal)(inaltime - 30) / maxPentruScara : 1;
            decimal factorDeScalareOrizontal = (valoriVerticale.Count != 0) ? (decimal)lungime / valoriVerticale.Count : 1;



            Point lastPoint = new Point(x, y + inaltime - (int)((valoriVerticale.Count != 0) ? valoriVerticale[0] : 1 * factorDeScalareVertical));

            if (scrieValori)
                if (max != 0)
                    g.DrawString(max.ToString(), new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x, y + inaltime - 15 - (int)(max * factorDeScalareVertical + 10)));
               

            for (int i = 0; i < valoriVerticale.Count; i++)
            {
                Point punctConectare = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical));


                Point punctGros1 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical - 2));
                Point punctGros2 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical + 2));


                Point punctLinieOriz1 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - 3); //linii de pe axa Ox
                Point punctLinieOriz2 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime + 3);

                Point punctLinieVert1 = new Point(x - 3, y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical)); //linii de pe axa Oy
                Point punctLinieVert2 = new Point(x + 3, y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical));

                g.DrawLine(black, punctLinieOriz1, punctLinieOriz2);
                g.DrawLine(black, punctLinieVert1, punctLinieVert2);

                g.DrawLine(ggrafic, punctGros1, punctGros2);

                g.DrawLine(grafic, lastPoint, punctConectare);

                lastPoint = punctConectare;

            }
        }
        
        public static void GraficLinie(Graphics g, int x, int y, int lungime, int inaltime, List<decimal> valoriVerticale, List<object> valoriOrizontale, Color culoareGrafic, bool dashed, string denumireOy, string denumireOx, bool scrieValori, decimal maxPentruScara)
        {
            Pen black = new Pen(Brushes.Black);

            Pen grafic = new Pen(culoareGrafic);
            Pen ggrafic = new Pen(culoareGrafic, 5);

            if (dashed)
            {
                grafic.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                grafic.DashPattern = new float[] { 2, 4 };
            }


            g.DrawLine(black, new Point(x, y + inaltime), new Point(x + lungime, y + inaltime)); //orizontala
            g.DrawLine(black, new Point(x, y), new Point(x, y + inaltime)); //verticala

            g.DrawLine(black, new Point(x + lungime, y + inaltime - 5), new Point(x + lungime, y + inaltime + 5)); //capat orizontala
            g.DrawLine(black, new Point(x - 5, y), new Point(x + 5, y)); //capat verticala

            g.DrawString(denumireOy, new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x - 10, y - 20));
            g.DrawString(denumireOx, new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x + lungime - 15, y + inaltime + 5));

        //    g.DrawString("0", new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x - 5, y + inaltime + 5));

            decimal max = 0;

            for (int i = 0; i < valoriVerticale.Count; i++)
            {
                if (valoriVerticale[i] > max)
                    max = valoriVerticale[i];
            }

            decimal factorDeScalareVertical = (maxPentruScara != 0) ? (decimal)(inaltime - 30) / maxPentruScara : 1;
            decimal factorDeScalareOrizontal = (valoriVerticale.Count != 0) ? (decimal)lungime / valoriVerticale.Count : 1;



            Point lastPoint = new Point(x, y + inaltime - (int)((valoriVerticale.Count != 0) ? valoriVerticale[0] : 1 * factorDeScalareVertical));

            if (scrieValori)
                if (max != 0)
                    g.DrawString(max.ToString(), new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x, y + inaltime - 15 - (int)(max * factorDeScalareVertical + 10)));


            for (int i = 0; i < valoriVerticale.Count; i++)
            {
                Point punctConectare = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical));


                Point punctGros1 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical - 2));
                Point punctGros2 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical + 2));


                Point punctLinieOriz1 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime - 3); //linii de pe axa Ox
                Point punctLinieOriz2 = new Point(x + (int)(i * factorDeScalareOrizontal), y + inaltime + 3);

                if (scrieValori)
                    g.DrawString(valoriOrizontale[i] as string, new Font("Calibri", 9, FontStyle.Regular), Brushes.Black, new Point(x + (int)(i * factorDeScalareOrizontal) - 5, y + inaltime));


                Point punctLinieVert1 = new Point(x - 3, y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical)); //linii de pe axa Oy
                Point punctLinieVert2 = new Point(x + 3, y + inaltime - (int)(valoriVerticale[i] * factorDeScalareVertical));

                g.DrawLine(black, punctLinieOriz1, punctLinieOriz2);
                g.DrawLine(black, punctLinieVert1, punctLinieVert2);

                g.DrawLine(ggrafic, punctGros1, punctGros2);

                g.DrawLine(grafic, lastPoint, punctConectare);

                lastPoint = punctConectare;

            }
        }


        public static void GraficFelieDeTort(Graphics g)
        {
            g.DrawArc(new Pen(Brushes.Black), 10, 10, 100, 100, 0, 15);
            g.FillPie(Brushes.Black, 10, 10, 500, 500, 0, 15);
        }

    }
}
