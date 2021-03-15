using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using Thor.Models;
using Thor.DataAccess;
using Thor.Utility;
using Thor.Modules;
using System.Reflection;
using System.Drawing.Printing;

namespace Thor
{
    public partial class mainForm : Form
    {

        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            //PRIMA OARA SE CITESC SETARILE DIN FISIER
            Settings.CitesteSetari();

            ActualizeazaPanelSetari();

            listViewFoldereDBF.Items.Clear();
            
            timerActualizareDate.Interval = Settings.Interval_Actualizare * 60000;
            // cand se porneste, se iau cele mai actuale baze de date, si inca cele care ma intereseaza
           // Thread t = new Thread(() => Settings.Actualizeaza_DBF(Settings.Ani_InUz));
           // t.Start();

           // Thread a = new Thread(() => MessageBox.Show("Se actualizeaza baza de date... \n\r Aceasta operatiune poate dura cateva secunde"));
          //  a.Start();


            Settings.Actualizeaza_DBF(Settings.Ani_InUz);

            SwitchPanel(panelListaFirme);

            //aici nu se filtreaza, se folosesc cele luate direct cum sunt
            firme = DbaseConnection.DBFCitesteFirme(Settings.Ani_InUz); //cel mai curent an, fisierul contine toate firmele cu care s-a lucrat vreodata
            firmeDisplay = firme;

            stocuri = DbaseConnection.DBFCitesteStocuri(Settings.Ani_InUz);
            stocuriDisplay = stocuri;

            ActualizeazaFirmeActive();

            evenimenteUrmatoare = Settings.Evenimente_NerezolvateInInterval(DateTime.Now, DateTime.Now.AddDays(Settings.NotificariZileAvans));
            evenimenteUrmatoareDisplay = evenimenteUrmatoare;

            GestioneazaNotificari(evenimenteUrmatoare);
            GestioneazaInterfataGeneral();

            //cand se schimba itemu, se refresheaza, aici cu toate firmele anyway, NU E BUNA METODA< DAR MERGE
            comboBoxScadenteAfisare.SelectedItem = comboBoxScadenteAfisare.Items[0];
            comboBoxFirmeAfisare.SelectedItem = comboBoxFirmeAfisare.Items[0];
            comboBoxFirmaCautareDupa.SelectedItem = comboBoxFirmaCautareDupa.Items[0];
            comboBoxEvenimenteAfisare.SelectedItem = comboBoxEvenimenteAfisare.Items[3];
            comboBoxFirmaEvenimentAfisare.SelectedItem = comboBoxFirmaEvenimentAfisare.Items[4];
            comboBoxFirmaTranzactiiAfisare.SelectedItem = comboBoxFirmaTranzactiiAfisare.Items[0];
            comboBoxSetariAni.SelectedItem = comboBoxSetariAni.Items[comboBoxSetariAni.FindString(Settings.An_Curent.ToString())];
            comboBoxComparatieAn1.SelectedItem = comboBoxComparatieAn1.Items[comboBoxComparatieAn1.FindString(Settings.An_Curent.ToString()) - 1];
            comboBoxComparatieAn2.SelectedItem = comboBoxComparatieAn2.Items[comboBoxComparatieAn2.FindString(Settings.An_Curent.ToString())];

            for (int i = 0; i < Settings.CaiFoldereDBF.Count; i++)
            {
                ListViewItem l = new ListViewItem();

                l.Text = Settings.CaiFoldereDBF.ToList()[i].Key;
                l.SubItems.Add(Settings.CaiFoldereDBF[Settings.CaiFoldereDBF.ToList()[i].Key]);

                listViewFoldereDBF.Items.Add(l);
            }

            Settings.CaiFoldereDBF.Clear();

            for(int i = 0; i < listViewFoldereDBF.Items.Count; i++)
            {
                try
                {
                    Settings.CaiFoldereDBF.Add(listViewFoldereDBF.Items[i].Text, listViewFoldereDBF.Items[i].SubItems[1].Text);
                }
                catch { };
            }

            Settings.ScrieSetari();

            AfiseazaTimpActualizare(Settings.Ultima_Actualizare_DBF, Settings.Urmatoare_Actualizare_DBF);

        }

        public List<Firma> firme;
        public List<Firma> firmeDisplay;
        public List<string> firmeCautate;
        public Firma firmaSelectata = null;
        
        
        public List<Factura> facturiClientDisplay;
        public Factura facturaClientSelectata = null;

        public List<Factura> facturiFurnizorDisplay;
        public Factura facturaFurnizorSelectata = null;
        
        public string modDisplayFacturiClient = "Normal";
        public string modDisplayFacturiFurnizor = "Normal";

        //Normal, Scadente, Platite, Neplatite

        public List<Factura> facturiScadenteDisplay;

        public List<Eveniment> evenimenteDisplay;
        public Eveniment evenimentSelectat = null;

        public List<Eveniment> evenimenteUrmatoare;
        public List<Eveniment> evenimenteUrmatoareDisplay;
        public Eveniment evenimentUrmatorSelectat = null;

        public List<Eveniment> evenimenteIstoric;
        public List<Eveniment> evenimenteIstoricDisplay;
        public Eveniment evenimentIstoricSelectat = null;

        public List<Comanda> comenziDisplay;
        public Comanda comandaSelectata = null;

        public List<Tranzactie> tranzactiiDisplay;
        public Tranzactie tranzactieSelectata = null;

        public List<Stoc> stocuri;
        public List<Stoc> stocuriDisplay;

        public Persoana persoanaSelectata = null;
        
        public ContBancar contSelectat = null;

        public string cheieCaleSelectata = null;

        public Panel panelCurent = null;

        public List<string> fisiereSelectateEveniment = new List<string>();
        public List<string> fisiereDisplay = new List<string>();

        public List<string> fisiereSelectateComanda = new List<string>();
        
        public bool _modModificarePersoana = false;

        public bool _modModificareContBancar = false;

        public bool _modModificareEveniment = false;

        public bool _modModificareComanda = false;
        

        #region functii

        public bool InfoPersoaneOK()
        {
          /*  bool rezultat = true;

            if(string.IsNullOrWhiteSpace(textBoxPersoanaNume.Text))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxPersoanaTelefon.Text))
            {
                return false;
            } */

            return true; //return rezultat;
        }

        public bool InfoEvenimentOK()
        {
            /*  bool rezultat = true;

              if (string.IsNullOrWhiteSpace(textBoxEvenimentDenumire.Text))
              {
                  return false;
              }

              if (string.IsNullOrWhiteSpace(comboBoxEvenimentTip.Text))
              {
                  return false;
              } 

              return rezultat;

              */

            return true;
        }
       
        public void AfiseazaTimpActualizare(DateTime ultima, DateTime urmatoarea)
        {
            toolStripStatusLabelUltimaActualizare.Text = "Ultima actualizare a bazei de date: " + ultima.ToShortTimeString();
            toolStripStatusLabelUrmatoareaActualizare.Text = "Urmatoarea actualizare: " + urmatoarea.ToShortTimeString();
        }

        public void ActualizeazaPanelSetari()
        {
            comboBoxSetariIntervalActualizare.Text = Settings.Interval_Actualizare.ToString();
        }

        public void ActualizeazaFirmeActive()
        {
            try
            {
                if (firme != null)
                {
                    foreach (string s in DbaseConnection.DBFFirmeActive(new List<string>() { Settings.An_Curent.ToString() }))
                    {
                        Firma f = firme.FindAll(x => x.CUI == s)[0];

                        f.CitesteInformatiiAditionale();
                        f.Activ_An_Curent = true;
                        f.ScrieInformatiiAditionale();
                    }
                }
            }
            catch { };
        }
        
        public void ReseteazaInfoPersoana()
        {
            persoanaSelectata = null;
            textBoxPersoanaNume.Clear();
            textBoxPersoanaTelefon.Clear();
            textBoxPersoanaEmail.Clear();
        }
        
        public void ReseteazaInfoEveniment()
        {
            evenimentSelectat = null;
            fisiereSelectateEveniment = new List<string>();
            textBoxEvenimentDenumire.Clear();
            dateTimePickerEvenimentData.Value = DateTime.Now;
            comboBoxEvenimentStare.SelectedItem = comboBoxEvenimentStare.Items[0];
            comboBoxEvenimentTip.SelectedItem = comboBoxEvenimentTip.Items[0];
            textBoxEvenimentNote.Clear();
            listViewEvenimentDocumente.Items.Clear();
        }

        public void ReseteazaInfoFirma()
        {
            firmaSelectata = null;
            textBoxFirmaDenumire.Text = "";
            textBoxFirmaCUI.Text = "";
            textBoxFirmaRegistru.Text = "";
            textBoxFirmaTelefon.Text = "";
            textBoxFirmaFax.Text = "";
            textBoxFirmaTara.Text = "";
            textBoxFirmaJudet.Text = "";
            textBoxFirmaLocalitate.Text = "";
            textBoxFirmaCodPostal.Text = "";
            textBoxFirmaAdresa.Text = "";

            checkBoxFirmaFavorit.Checked = false;
            checkBoxFirmaNotificari.Checked = false;
            textBoxFirmaSoldInitialClient.Text = "0";
            textBoxFirmaSoldInitialFurnizor.Text = "0";
            checkBoxFirmaTermenPlata.Checked = false;
            textBoxFirmaTermenPlata.Text = "0";
            checkBoxFirmaPlafonClient.Checked = false;
            textBoxFirmaPlafonClient.Text = "0";
            checkBoxFirmaPlafonFurnizor.Checked = false;
            textBoxFirmaPlafonFurnizor.Text = "0";


            ReseteazaInfoPersoana();
            ReseteazaInfoEveniment();

            GestioneazaInterfataGeneral();

            panelButoaneFirma.Size = new Size(150, 0);
            panelButoaneFirma.Enabled = false;

            listViewPersoane.Items.Clear();
            listViewConturi.Items.Clear();
        }
        
        public void CautareFirma(string criteriu, string valoare)
        {
            if(valoare == "")
            {
                firmeDisplay = FiltreazaFirme(comboBoxFirmeAfisare.SelectedItem.ToString(), firme);
                PopuleazaListaFirme(listViewFirme, firmeDisplay);
                return;
            }

            if (criteriu == "firma")
                firmeDisplay = firme.FindAll(x => x.Nume.Contains(valoare.ToUpper()));
            else if (criteriu == "factura")
            {
                firmeCautate = DbaseConnection.DBFCautaFirmeFacturi(Settings.Ani_InUz, valoare);
                firmeDisplay = firme.FindAll(x => firmeCautate.Contains(x.Nume));
            }
            else if(criteriu == "tranzactie")
            {
                firmeCautate = DbaseConnection.DBFCautaFirmeTranzactii(Settings.Ani_InUz, valoare);
                firmeDisplay = firme.FindAll(x => firmeCautate.Contains(x.Nume));
            }

            PopuleazaListaFirme(listViewFirme, firmeDisplay);
        }

        public List<Firma> FiltreazaFirme(string pentruDisplay, List<Firma> f)
        {
            if (f != null)
            {
                if (pentruDisplay == "Toate")
                {
                    return f;
                }
                else if (pentruDisplay == "Favorite")
                {
                    return f.FindAll(x => x.Favorit == true);
                }
                else if (pentruDisplay == "Active an curent")
                {
                    return f.FindAll(x => x.Activ_An_Curent == true);
                }
            }
            return new List<Firma>();
        }

        public void CautareStoc(string valoare)
        {
            if(valoare == "")
            {
                stocuriDisplay = stocuri;
                PopuleazaListaStocuri(listViewStocuri, stocuriDisplay);
            }
            else
            {
                stocuriDisplay = stocuri.FindAll(x => x.Denumire_Produs.Contains(valoare.ToUpper()));
                PopuleazaListaStocuri(listViewStocuri, stocuriDisplay);
            }
        }

        public List<Eveniment> FiltreazaEvenimente(string pentruDisplay, List<Eveniment> eve)
        {
            if (eve != null)
            {
                if (pentruDisplay == "Toate")
                {
                    return eve;
                }
                else
                {
                    return eve.FindAll(x => x.Tip == pentruDisplay);
                }
            }
            return new List<Eveniment>();
        }
        

        public List<Factura> FiltreazaFacturiPePerioada(List<Factura> fac, DateTime inceput, DateTime sfarsit)
        {
            List<Factura> rezultat = new List<Factura>();

            if (fac.Count > 0)
            {
                foreach (Factura f in fac)
                {
                    if (f.Data_Emitere >= inceput && f.Data_Emitere <= sfarsit)
                    {
                        rezultat.Add(f);
                    }
                }
            }

            return rezultat;
        }
        
        public List<Eveniment> FiltreazaEvenimenteFirma(string pentruDisplay, List<Eveniment> eve)
        {
            if (eve != null)
            {
                if (pentruDisplay == "Toate")
                {
                    return eve;
                }
                else
                {
                    return eve.FindAll(x => x.Tip == pentruDisplay);
                }
            }
            return new List<Eveniment>();
        }

        public List<Tranzactie> FiltreazaTranzactii(string pentruDisplay, List<Tranzactie> tran)
        {
            if (tran != null)
            {
                if (pentruDisplay == "Incasari")
                {
                    return tran.FindAll(x => x.Tip_Operatiune == "I");
                }
                else if(pentruDisplay == "Plati")
                {
                    return tran.FindAll(x => x.Tip_Operatiune == "P");
                }
            }
            return new List<Tranzactie>();
        }

        public List<Factura> FiltreazaFacturiScadente(string pentruDisplay, List<Factura> fac)
        {
            if (fac != null)
            {
                if(pentruDisplay == "Toate")
                {
                    return fac;
                }
                else if (pentruDisplay == "Client")
                {
                    return fac.FindAll(x => x.Tip_Factura == "Client");
                }
                else if (pentruDisplay == "Furnizor")
                {
                    return fac.FindAll(x => x.Tip_Factura == "Furnizor");
                }
            }
            return new List<Factura>();
        }



        public void PopuleazaListaFirme(ListView l, List<Firma> f)
        {
            l.Items.Clear();

            if (f.Count > 0)
            {
                for(int i = 0; i < f.Count; i++)
                {
                    //temporar, aici, atunci cand umple lista de firme, vreau sa stiu dinainte care firma e favorita pentru a gasi mai usor
                    f[i].CitesteInformatiiAditionale();

                    l.Items.Add(f[i].Nume);
                    l.Items[i].SubItems.Add(f[i].CUI);
                    l.Items[i].SubItems.Add(f[i].Registrul_Comertului);
                    l.Items[i].SubItems.Add(f[i].Telefon);
                }
            }
        }

        public void PopuleazaListaStocuri(ListView l, List<Stoc> s)
        {
            l.Items.Clear();

            if (s.Count > 0)
            {
                for (int i = 0; i < s.Count; i++)
                {

                    l.Items.Add(s[i].Denumire_Produs);
                    l.Items[i].SubItems.Add(s[i].Pret_Stoc.ToString("0.00"));
                    l.Items[i].SubItems.Add(s[i].Cantitate.ToString("0.00"));
                    l.Items[i].SubItems.Add(s[i].Unitate_Masura);
                }
            }
        }

        public void PopuleazaInformatieFirma(Firma f)
        {
            //favorit sau daca se afiseaza notificari si sold init
            
            textBoxFirmaDenumire.Text = f.Nume;
            textBoxFirmaCUI.Text = f.CUI;
            textBoxFirmaRegistru.Text = f.Registrul_Comertului;
            textBoxFirmaTelefon.Text = f.Telefon;
            textBoxFirmaFax.Text = f.Fax;
            textBoxFirmaTara.Text = f.Tara;
            textBoxFirmaJudet.Text = f.Judet;
            textBoxFirmaLocalitate.Text = f.Localitate;
            textBoxFirmaCodPostal.Text = f.Cod_Postal;
            textBoxFirmaAdresa.Text = f.Adresa;
            
            checkBoxFirmaFavorit.Checked = f.Favorit;
            checkBoxFirmaNotificari.Checked = f.Afiseaza_Notificari;
            textBoxFirmaSoldInitialClient.Text = f.SoldInitialClient.ToString();
            textBoxFirmaSoldInitialFurnizor.Text = f.SoldInitialFurnizor.ToString();
            checkBoxFirmaTermenPlata.Checked = f.Foloseste_Termen_Plata;
            textBoxFirmaTermenPlata.Text = f.Termen_Plata.ToString();
            checkBoxFirmaPlafonClient.Checked = f.Foloseste_Plafon_Client;
            textBoxFirmaPlafonClient.Text = f.Plafon_Client.ToString();
            checkBoxFirmaPlafonFurnizor.Checked = f.Foloseste_Plafon_Furnizor;
            textBoxFirmaPlafonFurnizor.Text = f.Plafon_Furnizor.ToString();

            if (f.Cai_Documente != null)
            {
                fisiereDisplay = f.Cai_Documente;

                listViewFirmaDocumente.Items.Clear();
                foreach (string s in fisiereDisplay)
                    listViewFirmaDocumente.Items.Add(Path.GetFileName(s));
            }
            

            ReseteazaInfoPersoana();

            PopuleazaListaPersoaneDeContact(listViewPersoane, f.Persoane_De_Contact);

            PopuleazaListaConturiBancare(listViewConturi, f.Conturi_Bancare);
        }

        public void PopuleazaInformatiePersoanaContact(Persoana p)
        {
            textBoxPersoanaNume.Text = p.Nume_Prenume;
            textBoxPersoanaTelefon.Text = p.Telefon;
            textBoxPersoanaEmail.Text = p.Email;
        }

        public void PopuleazaInformatieComanda(Comanda c)
        {
            comboBoxMaterialComanda.SelectedText = c.Material;
            comboBoxTipProdusComanda.SelectedText = c.Tip_Produs;
            textBoxImprimeuComanda.Text = c.Imprimeu;
            textBoxCantitateComanda.Text = c.Cantitate.ToString();
            textBoxPretUnitarComanda.Text = c.Pret_Unitar.ToString();
            dateTimePickerDataComanda.Value = c.Data_Lansare;
            textBoxObservatiiComanda.Text = c.Observatii;
            textBoxDimensiuniComanda.Text = c.Dimensiuni;

            

            if (c.Cai_Documente != null)
            {
                fisiereSelectateComanda = c.Cai_Documente;

                listViewGraficaComanda.Items.Clear();
                foreach (string s in fisiereSelectateComanda)
                    listViewGraficaComanda.Items.Add(Path.GetFileName(s));
            }
        }

        public bool InfoComandaOK()
        {
            bool rezultat = true;

            if (string.IsNullOrWhiteSpace(comboBoxMaterialComanda.Text))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(comboBoxTipProdusComanda.Text))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxDimensiuniComanda.Text))
            {
                return false;
            }

            decimal a;

            if (!decimal.TryParse(textBoxCantitateComanda.Text, out a))
            {
                return false;
            }

            if (!decimal.TryParse(textBoxPretUnitarComanda.Text, out a))
            {
                return false;
            }

            return rezultat;

        }

        public void ReseteazaInfoComanda()
        {
            comandaSelectata = null;

            comboBoxMaterialComanda.Text = "";
            comboBoxTipProdusComanda.Text = "";
            textBoxImprimeuComanda.Text = "";
            textBoxCantitateComanda.Text = "";
            textBoxPretUnitarComanda.Text = "";
            dateTimePickerDataComanda.Value = DateTime.Today;
            textBoxObservatiiComanda.Text = "";
            textBoxDimensiuniComanda.Text = "";

            fisiereSelectateComanda = new List<string>();
            listViewGraficaComanda.Clear();
        }

        public void PopuleazaListaPersoaneDeContact(ListView l, List<Persoana> persoane)
        {
            l.Items.Clear();
            if (persoane.Count > 0)
            {
                for (int i = 0; i < persoane.Count; i++)
                {
                    listViewPersoane.Items.Add(persoane[i].Nume_Prenume);
                }
            }
        }

        public void PopuleazaInformatieContBancar(ContBancar c)
        {
            textBoxContIBAN.Text = c.IBAN;
            textBoxContBanca.Text = c.Banca;
            textBoxContMoneda.Text = c.Moneda_Cont;
        }

        public void PopuleazaListaConturiBancare(ListView l, List<ContBancar> conturi)
        {
            l.Items.Clear();
            if (conturi.Count > 0)
            {
                for (int i = 0; i < conturi.Count; i++)
                {
                    listViewConturi.Items.Add(conturi[i].IBAN);
                }
            }
        }

        public void PopuleazaListaFacturi(ListView l, List<Factura> fac, TextBox valTotal, Label nrFacturi)
        {
            l.Items.Clear();
            valTotal.Text = "0";
            nrFacturi.Text = fac.Count.ToString();

            if (fac.Count > 0 && fac != null)
            {
                decimal totalFacturi = 0;

                for (int i = 0; i < fac.Count; i++)
                {
                    l.Items.Add(fac[i].Seria);
                    l.Items[i].SubItems.Add(fac[i].Numar_Document);
                    l.Items[i].SubItems.Add(fac[i].Data_Emitere.ToLongDateString());
                    l.Items[i].SubItems.Add(fac[i].Data_Scadenta.ToLongDateString());
                    l.Items[i].SubItems.Add(fac[i].Valoare.ToString());
                    l.Items[i].SubItems.Add(fac[i].TVA.ToString());
                    l.Items[i].SubItems.Add(fac[i].Valoare_Cu_TVA.ToString());
                    l.Items[i].SubItems.Add(fac[i].Moneda);
                    l.Items[i].SubItems.Add(fac[i].Curs.ToString());

                    totalFacturi += fac[i].Valoare;


                    valTotal.Text = totalFacturi.ToString();
                }
            }
        }

        public void PopuleazaListaFacturiScadente(ListView l, List<Factura> fac)
        {
            l.Items.Clear();

            if (fac.Count > 0)
            {
                for (int i = 0; i < fac.Count; i++)
                {
                    l.Items.Add(fac[i].Nume_Firma);
                    l.Items[i].SubItems.Add(fac[i].Seria);
                    l.Items[i].SubItems.Add(fac[i].Numar_Document);
                    l.Items[i].SubItems.Add(fac[i].Data_Emitere.ToLongDateString());
                    l.Items[i].SubItems.Add(fac[i].Data_Scadenta.ToLongDateString());
                    l.Items[i].SubItems.Add(fac[i].Valoare.ToString());
                    l.Items[i].SubItems.Add(fac[i].TVA.ToString());
                    l.Items[i].SubItems.Add(fac[i].Valoare_Cu_TVA.ToString());
                    l.Items[i].SubItems.Add(fac[i].Moneda);
                    l.Items[i].SubItems.Add(fac[i].Curs.ToString());
                }
            }
        }

        public void PopuleazaInformatieFacturaClient(Factura fac)
        {
            fac.Produse = DbaseConnection.DBFCitesteProduseFacturaClient(fac, Settings.Ani_InUz);

            PopuleazaListaProduse(listViewProduseFacClient, fac.Produse);
        }

        public void PopuleazaInformatieFacturaFurnizor(Factura fac)
        {
            fac.Produse = DbaseConnection.DBFCitesteProduseFacturaFurnizor(fac, Settings.Ani_InUz);

            PopuleazaListaProduse(listViewProduseFacFurnizor, fac.Produse);
        }

        public void PopuleazaListaProduse(ListView l, List<Produs> prod)
        {
            l.Items.Clear();

            if (prod.Count > 0)
            {
                for (int i = 0; i < prod.Count; i++)
                {
                    l.Items.Add(prod[i].Denumire_Produs);
                    l.Items[i].SubItems.Add(prod[i].Unitate_Masura);
                    l.Items[i].SubItems.Add(prod[i].Cantitate.ToString());
                    l.Items[i].SubItems.Add(prod[i].Pret_Cumparare.ToString());
                    l.Items[i].SubItems.Add(prod[i].Pret_Vanzare.ToString());
                    l.Items[i].SubItems.Add(prod[i].Valoare.ToString());
                    l.Items[i].SubItems.Add(prod[i].Valoare_Cu_Tva.ToString());
                }
            }
        }

        public void PopuleazaListaEvenimenteFirma(ListView l, List<Eveniment> eve)
        {
            l.Items.Clear();

            if (eve.Count > 0)
            {
                for (int i = 0; i < eve.Count; i++)
                {
                    l.Items.Add(eve[i].Denumire);
                    l.Items[i].SubItems.Add(eve[i].Tip);
                    l.Items[i].SubItems.Add(eve[i].Stare);
                    l.Items[i].SubItems.Add(eve[i].Data_Eveniment.ToLongDateString());
                    l.Items[i].SubItems.Add(eve[i].Note);

                    l.Items[i].ForeColor = Color.White;

                    if (eve[i].Stare == "Nerezolvat")
                        l.Items[i].BackColor = Color.Red;
                    else
                        l.Items[i].BackColor = Color.Green;
                }
            }
        }

        public void PopuleazaListaComenziFirma(ListView l, List<Comanda> com)
        {
            l.Items.Clear();

            if(com.Count > 0)
            {
                for(int i = 0; i < com.Count; i++)
                {
                    l.Items.Add(com[i].Id.ToString());
                    l.Items[i].SubItems.Add(com[i].Data_Lansare.ToShortDateString());
                    l.Items[i].SubItems.Add(com[i].Material);
                    l.Items[i].SubItems.Add(com[i].Imprimeu);
                }
            }
        }

        public void PopuleazaListaEvenimente(ListView l, List<Eveniment> eve, bool color)
        {
            l.Items.Clear();

            if (eve.Count > 0)
            {
                for (int i = 0; i < eve.Count; i++)
                {
                    l.Items.Add(eve[i].Nume_Firma);
                    l.Items[i].SubItems.Add(eve[i].Denumire);
                    l.Items[i].SubItems.Add(eve[i].Tip);
                    l.Items[i].SubItems.Add(eve[i].Data_Eveniment.ToLongDateString());
                    l.Items[i].SubItems.Add(eve[i].Note);
                    
                    if (color)
                    {
                        l.Items[i].ForeColor = Color.White;

                        if (eve[i].Stare == "Nerezolvat")
                            l.Items[i].BackColor = Color.Red;
                        else
                            l.Items[i].BackColor = Color.Green;
                    }
                    
                }
            }
        }

        public void PopuleazaInformatieEveniment(Eveniment eve)
        {
            textBoxEvenimentDenumire.Text = eve.Denumire;
            comboBoxEvenimentTip.Text = eve.Tip;
            comboBoxEvenimentStare.SelectedItem = eve.Stare == "Rezolvat" ? comboBoxEvenimentStare.Items[1] : comboBoxEvenimentStare.Items[0];
            dateTimePickerEvenimentData.Value = eve.Data_Eveniment;
            textBoxEvenimentNote.Text = eve.Note;

            if (eve.Cai_Documente != null)
            {
                fisiereSelectateEveniment = eve.Cai_Documente;

                listViewEvenimentDocumente.Items.Clear();
                foreach (string s in fisiereSelectateEveniment)
                    listViewEvenimentDocumente.Items.Add(Path.GetFileName(s));
            }
        }

        public void PopuleazaListaTranzactii(ListView l, List<Tranzactie> tran, TextBox valTotal)
        {
            l.Items.Clear();
            valTotal.Text = "0";
            if (tran.Count > 0)
            {
                decimal total = 0;

                for (int i = 0; i < tran.Count; i++)
                {
                    l.Items.Add(tran[i].Tip_Operatiune);
                    l.Items[i].SubItems.Add(tran[i].Metoda);
                    l.Items[i].SubItems.Add(tran[i].Seria);
                    l.Items[i].SubItems.Add(tran[i].Numar_Document);
                    l.Items[i].SubItems.Add(tran[i].Data_Tranzactie.ToLongDateString());
                    l.Items[i].SubItems.Add(tran[i].Seria_Document_Corespunzator);
                    l.Items[i].SubItems.Add(tran[i].Numar_Document_Corespunzator);
                    l.Items[i].SubItems.Add(tran[i].Data_Document_Corespunzator.ToLongDateString());
                    l.Items[i].SubItems.Add(tran[i].Obiect_Tranzactie);
                    l.Items[i].SubItems.Add(tran[i].Valoare.ToString());
                    l.Items[i].SubItems.Add(tran[i].TVA.ToString());
                    l.Items[i].SubItems.Add(tran[i].Valoare_Cu_TVA.ToString());
                    l.Items[i].SubItems.Add(tran[i].Moneda);
                    l.Items[i].SubItems.Add(tran[i].Curs.ToString());

                    total += tran[i].Valoare;
                }

                valTotal.Text = total.ToString();
            }
        }
        
        public void SwitchPanel(Panel panel)
        {
            panel.BringToFront();
            panel.Enabled = true;
            panel.Visible = true;
            panel.Dock = DockStyle.Fill;

            
            panelCurent = panel;
            /*
            if (panelCurent == panelWebsiteUtile)
            {
                panelButoaneFirma.Size = new Size(150, 0);
                panelButoaneFirma.Enabled = false;
            }

            if (panelCurent == panelEvenimenteUrmatoare)
            {
                panelButoaneFirma.Size = new Size(150, 0);
                panelButoaneFirma.Enabled = false;
            }

            if (panelCurent == panelComparatii)
            {
                panelButoaneFirma.Size = new Size(150, 0);
                panelButoaneFirma.Enabled = false;
            }

            if (panelCurent == panelInformatiiGenerale)
            {
                panelButoaneFirma.Size = new Size(150, 0);
                panelButoaneFirma.Enabled = false;
            }

            if (panelCurent == panelListaFirme && firmaSelectata != null)
            {
                panelButoaneFirma.Size = new Size(150, 181);
                panelButoaneFirma.Enabled = true;
            }*/
        }

        public void GestioneazaNotificari(List<Eveniment> eve)
        {
            if (eve.Count > 0)
            {
                panelNotificare.Size = new Size(1761, 28);
            }
            else
                panelNotificare.Size = new Size(1761, 0);
        }

        public void GestioneazaInterfataGeneral()
        {
            GestioneazaInterfataPersoaneContact();

            if (firmaSelectata != null)
            {
                listViewPersoane.Enabled = true;
                textBoxPersoanaNume.Enabled = true;
                textBoxPersoanaTelefon.Enabled = true;
                textBoxPersoanaEmail.Enabled = true;

                buttonPersoanaCreare.Enabled = true;

                listViewConturi.Enabled = true;
                textBoxContIBAN.Enabled = true;
                textBoxContBanca.Enabled = true;
                textBoxContMoneda.Enabled = true;

                buttonContCreare.Enabled = true;

                checkBoxFirmaFavorit.Enabled = true;
                checkBoxFirmaNotificari.Enabled = true;
                textBoxFirmaSoldInitialClient.ReadOnly = false;
                textBoxFirmaSoldInitialFurnizor.ReadOnly = false;

                checkBoxFirmaTermenPlata.Enabled = true;
                if (firmaSelectata.Foloseste_Termen_Plata)
                    textBoxFirmaTermenPlata.ReadOnly = false;
                else
                    textBoxFirmaTermenPlata.ReadOnly = true;

                checkBoxFirmaPlafonClient.Enabled = true;
                if (firmaSelectata.Foloseste_Plafon_Client)
                    textBoxFirmaPlafonClient.ReadOnly = false;
                else
                    textBoxFirmaPlafonClient.ReadOnly = true;

                checkBoxFirmaPlafonFurnizor.Enabled = true;
                if (firmaSelectata.Foloseste_Plafon_Furnizor)
                    textBoxFirmaPlafonFurnizor.ReadOnly = false;
                else
                    textBoxFirmaPlafonFurnizor.ReadOnly = true;
            }
            else
            {
                listViewPersoane.Enabled = false;
                textBoxPersoanaNume.Enabled = false;
                textBoxPersoanaTelefon.Enabled = false;
                textBoxPersoanaEmail.Enabled = false;

                buttonPersoanaCreare.Enabled = false;
                buttonPersoanaStergere.Enabled = false;

                listViewConturi.Enabled = false;
                textBoxContIBAN.Enabled = false;
                textBoxContBanca.Enabled = false;
                textBoxContMoneda.Enabled = false;

                buttonContCreare.Enabled = false;
                buttonContStergere.Enabled = false;

                checkBoxFirmaFavorit.Enabled = false;
                checkBoxFirmaNotificari.Enabled = false;
                textBoxFirmaSoldInitialClient.ReadOnly = true;
                textBoxFirmaSoldInitialFurnizor.ReadOnly = true;
                checkBoxFirmaTermenPlata.Enabled = false;
                textBoxFirmaTermenPlata.ReadOnly = true;
                checkBoxFirmaPlafonClient.Enabled = false;
                textBoxFirmaPlafonClient.ReadOnly = true;
                checkBoxFirmaPlafonFurnizor.Enabled = false;
                textBoxFirmaPlafonFurnizor.ReadOnly = true;
            }
        }

        public void GestioneazaInterfataFacturiClient()
        {
            if (firmaSelectata != null)
            {
                if(modDisplayFacturiClient == "Normal")
                {
                    labelFacturiClient.Text = "Facturi client";
                }
                else if(modDisplayFacturiClient == "Scadente")
                {
                    labelFacturiClient.Text = "Facturi client scadente";
                }
                else if (modDisplayFacturiClient == "Platite")
                {
                    labelFacturiClient.Text = "Facturi client platite";
                }
                else if (modDisplayFacturiClient == "Neplatite")
                {
                    labelFacturiClient.Text = "Facturi client neplatite";
                }
            }
        }

        public void GestioneazaInterfataFacturiFurnizor()
        {
            if (firmaSelectata != null)
            {
                if (modDisplayFacturiFurnizor == "Normal")
                {
                    labelFacturiFurnizor.Text = "Facturi furnizor";
                }
                else if (modDisplayFacturiFurnizor == "Scadente")
                {
                    labelFacturiFurnizor.Text = "Facturi furnizor scadente";
                }
                else if (modDisplayFacturiFurnizor == "Platite")
                {
                    labelFacturiFurnizor.Text = "Facturi furnizor platite";
                }
                else if (modDisplayFacturiFurnizor == "Neplatite")
                {
                    labelFacturiFurnizor.Text = "Facturi furnizor neplatite";
                }
            }
        }

        public void GestioneazaInterfataComenzi()
        {
            if(firmaSelectata != null)
            {
                if(comandaSelectata != null)
                {
                    if(_modModificareComanda)
                    {
                        buttonCreereComanda.Text = "Confirmare";


                        comboBoxMaterialComanda.Enabled = true;
                        comboBoxTipProdusComanda.Enabled = true;
                        textBoxCantitateComanda.ReadOnly = false;
                        textBoxObservatiiComanda.ReadOnly = false;
                        textBoxPretUnitarComanda.ReadOnly = false;
                        textBoxDimensiuniComanda.ReadOnly = false;
                        buttonAdaugareFisiereComanda.Enabled = true;
                        dateTimePickerDataComanda.Enabled = true;
                        toolStripMenuItemComandaSterge.Enabled = true;
                        textBoxImprimeuComanda.ReadOnly = false;
                        
                    }

                    else if(!_modModificareComanda)
                    {
                        buttonCreereComanda.Text = "Modificare";

                        comboBoxMaterialComanda.Enabled = false;
                        comboBoxTipProdusComanda.Enabled = false;
                        textBoxCantitateComanda.ReadOnly = true;
                        textBoxObservatiiComanda.ReadOnly = true;
                        textBoxPretUnitarComanda.ReadOnly = true;
                        textBoxDimensiuniComanda.ReadOnly = true;
                        buttonAdaugareFisiereComanda.Enabled = false;
                        toolStripMenuItemComandaSterge.Enabled = false;
                        dateTimePickerDataComanda.Enabled = false;
                        textBoxImprimeuComanda.ReadOnly = true;
                    }
                }
                else
                {
                    buttonCreereComanda.Text = "Creere";

                    comboBoxMaterialComanda.Enabled = true;
                    comboBoxTipProdusComanda.Enabled = true;
                    textBoxCantitateComanda.ReadOnly = false;
                    textBoxObservatiiComanda.ReadOnly = false;
                    textBoxPretUnitarComanda.ReadOnly = false;
                    textBoxDimensiuniComanda.ReadOnly = false;
                    buttonAdaugareFisiereComanda.Enabled = true;
                    toolStripMenuItemComandaSterge.Enabled = true;
                    dateTimePickerDataComanda.Enabled = true;
                    textBoxImprimeuComanda.ReadOnly = false;
                }
            }
        }

        public void GestioneazaInterfataEvenimente()
        {
            if (firmaSelectata != null)
            {
                if(!(evenimentSelectat is null))
                {
                    if(_modModificareEveniment)
                    {
                        buttonFirmaEvenimentCreare.Text = "Confirmare";

                        textBoxEvenimentDenumire.ReadOnly = false;
                        comboBoxEvenimentTip.Enabled = true;
                        comboBoxEvenimentStare.Enabled = true;
                        dateTimePickerEvenimentData.Enabled = true;
                        textBoxEvenimentNote.ReadOnly = false;
                        buttonFirmaEvenimentAdaugareDocument.Enabled = true;
                        stergeToolStripMenuItem.Enabled = true;
                        buttonFirmaEvenimentStergere.Enabled = true;
                        buttonFirmaGoogleCalendar.Enabled = true;

                    }
                    else if(!_modModificareEveniment)
                    {
                        buttonFirmaEvenimentCreare.Text = "Modificare";

                        textBoxEvenimentDenumire.ReadOnly = true;
                        comboBoxEvenimentTip.Enabled = false;
                        comboBoxEvenimentStare.Enabled = false;
                        dateTimePickerEvenimentData.Enabled = false;
                        textBoxEvenimentNote.ReadOnly = true;
                        buttonFirmaEvenimentAdaugareDocument.Enabled = false;
                        stergeToolStripMenuItem.Enabled = false;
                        buttonFirmaEvenimentStergere.Enabled = true;
                        buttonFirmaGoogleCalendar.Enabled = true;
                    }
                }
                else
                {
                    buttonFirmaEvenimentCreare.Text = "Creare";

                    textBoxEvenimentDenumire.ReadOnly = false;
                    comboBoxEvenimentTip.Enabled = true;
                    comboBoxEvenimentStare.Enabled = true;
                    dateTimePickerEvenimentData.Enabled = true;
                    textBoxEvenimentNote.ReadOnly = false;
                    buttonFirmaEvenimentAdaugareDocument.Enabled = true;
                    stergeToolStripMenuItem.Enabled = true;
                    buttonFirmaEvenimentStergere.Enabled = false;
                    buttonFirmaGoogleCalendar.Enabled = false;
                }
            }
        }

        public void GestioneazaInterfataPersoaneContact()
        {
            if (firmaSelectata != null)
            {
                if (persoanaSelectata != null)
                {
                    buttonPersoanaStergere.Enabled = true;

                    if (_modModificarePersoana)
                    {
                        buttonPersoanaCreare.Text = "Confirmare";

                        textBoxPersoanaNume.ReadOnly = false;
                        textBoxPersoanaTelefon.ReadOnly = false;
                        textBoxPersoanaEmail.ReadOnly = false;

                        
                    }
                    else if (!_modModificarePersoana)
                    {
                        buttonPersoanaCreare.Text = "Modificare";

                        textBoxPersoanaNume.ReadOnly = true;
                        textBoxPersoanaTelefon.ReadOnly = true;
                        textBoxPersoanaEmail.ReadOnly = true;
                    }  
                }
                else if(persoanaSelectata == null)
                {
                    buttonPersoanaCreare.Text = "Creare";
                    buttonPersoanaStergere.Enabled = false;

                    textBoxPersoanaNume.ReadOnly = false;
                    textBoxPersoanaTelefon.ReadOnly = false;
                    textBoxPersoanaEmail.ReadOnly = false;
                }
            }
        }

        private void comboBoxCriteriuComparatie_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                ComparaFirmaPePerioada(panelComparatie, firmaSelectata, comboBoxCriteriuComparatie.Text, new DateTime(int.Parse((string)comboBoxComparatieAn1.SelectedItem), 1, 1), new DateTime(int.Parse((string)comboBoxComparatieAn1.SelectedItem), 12, 31), new DateTime(int.Parse((string)comboBoxComparatieAn2.SelectedItem), 1, 1), new DateTime(int.Parse((string)comboBoxComparatieAn2.SelectedItem), 12, 31));
            }
        }

        private void comboBoxComparatieAn1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                ComparaFirmaPePerioada(panelComparatie, firmaSelectata, comboBoxCriteriuComparatie.Text, new DateTime(int.Parse((string)comboBoxComparatieAn1.SelectedItem), 1, 1), new DateTime(int.Parse((string)comboBoxComparatieAn1.SelectedItem), 12, 31), new DateTime(int.Parse((string)comboBoxComparatieAn2.SelectedItem), 1, 1), new DateTime(int.Parse((string)comboBoxComparatieAn2.SelectedItem), 12, 31));
            }
        }

        private void comboBoxComparatieAn2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                ComparaFirmaPePerioada(panelComparatie, firmaSelectata, comboBoxCriteriuComparatie.Text, new DateTime(int.Parse((string)comboBoxComparatieAn1.SelectedItem), 1, 1), new DateTime(int.Parse((string)comboBoxComparatieAn1.SelectedItem), 12, 31), new DateTime(int.Parse((string)comboBoxComparatieAn2.SelectedItem), 1, 1), new DateTime(int.Parse((string)comboBoxComparatieAn2.SelectedItem), 12, 31));
            }
        }

        void PopuleazaStatisticiLuni(ListView l, List<object> luni, List<decimal> rulaj, List<decimal> incasari, List<int> nrfacturi, List<int> nrincasari)
        {
            l.Items.Clear();

            if(luni.Count > 0)
            {
                for(int i = 0; i < luni.Count; i++)
                {
                    l.Items.Add(luni[i].ToString());
                    l.Items[i].SubItems.Add(rulaj[i].ToString("0.00"));
                    l.Items[i].SubItems.Add(incasari[i].ToString("0.00"));
                    l.Items[i].SubItems.Add(nrfacturi[i].ToString());
                    l.Items[i].SubItems.Add(nrincasari[i].ToString());

                    l.Items[i].UseItemStyleForSubItems = false;

                    if (i == 0)
                    {
                        l.Items[i].SubItems.Add("0.00 %");
                    }
                    if (i > 0)
                    {
                        decimal dif = 0;

                        dif = rulaj[i] - rulaj[i - 1];

                        decimal procentCrestere = 0;

                        if (rulaj[i - 1] == 0 && rulaj[i] >= 0)
                        {
                            procentCrestere = rulaj[i];
                        }
                        else
                        {
                            procentCrestere = (100.0m * dif) / rulaj[i - 1];
                        }

                        l.Items[i].SubItems.Add(procentCrestere.ToString("0.00") + " %");

                        if (procentCrestere == 0)
                            l.Items[i].SubItems[5].ForeColor = Color.Black;
                        else
                            l.Items[i].SubItems[5].ForeColor = procentCrestere > 0 ? Color.Green : Color.Red;

                        
                    }

                }
            }
        }

        void PopuleazaStatisticiComparareLuni(ListView l, List<object> luni, List<decimal> rulajPer1, List<decimal> rulajPer2, List<int> nrfacturi1, List<int> nrfacturi2)
        {
            l.Items.Clear();

            if (luni.Count > 0)
            {
                for (int i = 0; i < luni.Count; i++)
                {
                    l.Items.Add(luni[i].ToString());
                    l.Items[i].SubItems.Add(rulajPer1[i].ToString("0.00"));
                    l.Items[i].SubItems.Add(nrfacturi1[i].ToString());
                    l.Items[i].SubItems.Add(rulajPer2[i].ToString("0.00"));
                    l.Items[i].SubItems.Add(nrfacturi2[i].ToString());

                    l.Items[i].UseItemStyleForSubItems = false;

                    decimal dif = 0;

                    dif = rulajPer2[i] - rulajPer1[i];

                    decimal procentCrestere = 0;

                    if (rulajPer1[i] == 0 && rulajPer2[i] >= 0)
                    {
                        procentCrestere = rulajPer2[i];
                    }
                    else
                    {
                        procentCrestere = (100.0m * dif) / rulajPer1[i];
                    }

                    l.Items[i].SubItems.Add(procentCrestere.ToString("0.00") + " %");

                    if (procentCrestere == 0)
                    {
                        l.Items[i].SubItems[5].ForeColor = Color.Black;
                        l.Items[i].SubItems[5].BackColor = Color.White;
                    }
                    else
                    {
                        l.Items[i].SubItems[5].ForeColor = Color.White;
                        l.Items[i].SubItems[5].BackColor = procentCrestere > 0 ? Color.Green : Color.Red;
                    }
                }
            }
        }

        public void ComparaFirmaPePerioada(Panel panel, Firma firma, string dupa, DateTime inceput1, DateTime sfarsit1, DateTime inceput2, DateTime sfarsit2)
        {
            List<decimal> valori1 = new List<decimal>();
            List<decimal> valori2 = new List<decimal>();

            decimal max = 0;

            if (firma != null)
            {
                firma.FacturiClient = DbaseConnection.DBFCitesteFacturiClientFirma(firma, Settings.Ani_InUz);

                firma.FacturiFurnizor = DbaseConnection.DBFCitesteFacturiFurnizorFirma(firma, Settings.Ani_InUz);

                firma.Tranzactii = DbaseConnection.DBFCitesteTranzactiiFirma(firma, Settings.Ani_InUz);

                


                switch (dupa)
                {
                    case "Grafic comparatie rulaje client":
                        valori1 = firma.RulajClientLuniDinPerioada(inceput1, sfarsit1);
                        valori2 = firma.RulajClientLuniDinPerioada(inceput2, sfarsit2);

                        PopuleazaStatisticiLuni(listViewStatisticiPer1, GraphicLibrary.LuniSiAnPrescurtatePerioada(inceput1, sfarsit1),
                          firma.RulajClientLuniDinPerioada(inceput1, sfarsit1), firma.IncasariClientLuniDinPerioada(inceput1, sfarsit1),
                          firma.NumarFacturiClientLuniDinPerioada(inceput1, sfarsit1), firma.NumarIncasariLuniDinPerioada(inceput1, sfarsit1));

                        PopuleazaStatisticiLuni(listViewStatisticiPer2, GraphicLibrary.LuniSiAnPrescurtatePerioada(inceput2, sfarsit2),
                            firma.RulajClientLuniDinPerioada(inceput2, sfarsit2), firma.IncasariClientLuniDinPerioada(inceput2, sfarsit2),
                            firma.NumarFacturiClientLuniDinPerioada(inceput2, sfarsit2), firma.NumarIncasariLuniDinPerioada(inceput2, sfarsit2));

                        PopuleazaStatisticiComparareLuni(listViewComparatiePerioade, GraphicLibrary.LuniPrescurtate(), firma.RulajClientLuniDinPerioada(inceput1, sfarsit1), 
                            firma.RulajClientLuniDinPerioada(inceput2, sfarsit2),
                            firma.NumarFacturiClientLuniDinPerioada(inceput1, sfarsit1), 
                            firma.NumarFacturiClientLuniDinPerioada(inceput2, sfarsit2));


                        columnHeaderComparatie1Per1.Text = "Rulaj " + (string)comboBoxComparatieAn1.SelectedItem;
                        columnHeaderComparatie2Per1.Text = "Nr. facturi " + (string)comboBoxComparatieAn1.SelectedItem;

                        columnHeaderComparatie1Per2.Text = "Rulaj " + (string)comboBoxComparatieAn2.SelectedItem;
                        columnHeaderComparatie2Per2.Text = "Nr. facturi " + (string)comboBoxComparatieAn2.SelectedItem;

                        columnHeaderStat2Per1.Text = "Incasari";
                        columnHeaderStat2Per2.Text = "Incasari";

                        tabControlPer1.SelectTab(0);
                        tabControlPer2.SelectTab(0);


                        max = valori1.Union(valori2).OrderByDescending(x => x).First();
                        
                        break;
                    case "Grafic comparatie incasari client":
                        valori1 = firma.IncasariClientLuniDinPerioada(inceput1, sfarsit1);
                        valori2 = firma.IncasariClientLuniDinPerioada(inceput2, sfarsit2);
                        
                        PopuleazaStatisticiLuni(listViewStatisticiPer1, GraphicLibrary.LuniSiAnPrescurtatePerioada(inceput1, sfarsit1),
                          firma.IncasariClientLuniDinPerioada(inceput1, sfarsit1), firma.IncasariClientLuniDinPerioada(inceput1, sfarsit1),
                          firma.NumarFacturiClientLuniDinPerioada(inceput1, sfarsit1), firma.NumarIncasariLuniDinPerioada(inceput1, sfarsit1));

                        PopuleazaStatisticiLuni(listViewStatisticiPer2, GraphicLibrary.LuniSiAnPrescurtatePerioada(inceput2, sfarsit2),
                            firma.IncasariClientLuniDinPerioada(inceput2, sfarsit2), firma.IncasariClientLuniDinPerioada(inceput2, sfarsit2),
                            firma.NumarFacturiClientLuniDinPerioada(inceput2, sfarsit2), firma.NumarIncasariLuniDinPerioada(inceput2, sfarsit2));

                        PopuleazaStatisticiComparareLuni(listViewComparatiePerioade, GraphicLibrary.LuniPrescurtate(), firma.IncasariClientLuniDinPerioada(inceput1, sfarsit1),
                            firma.IncasariClientLuniDinPerioada(inceput2, sfarsit2),
                            firma.NumarIncasariLuniDinPerioada(inceput1, sfarsit1),
                            firma.NumarIncasariLuniDinPerioada(inceput2, sfarsit2));


                        columnHeaderComparatie1Per1.Text = "Incasari " + (string)comboBoxComparatieAn1.SelectedItem;
                        columnHeaderComparatie2Per1.Text = "Nr. tranzactii " + (string)comboBoxComparatieAn1.SelectedItem;

                        columnHeaderComparatie1Per2.Text = "Incasari " + (string)comboBoxComparatieAn2.SelectedItem;
                        columnHeaderComparatie2Per2.Text = "Nr. tranzactii " + (string)comboBoxComparatieAn2.SelectedItem;

                        columnHeaderStat2Per1.Text = "Incasari";
                        columnHeaderStat2Per2.Text = "Incasari";

                        tabControlPer1.SelectTab(0);
                        tabControlPer2.SelectTab(0);

                        max = valori1.Union(valori2).OrderByDescending(x => x).First();

                        break;
                    case "Grafic comparatie rulaje furnizor":

                        valori1 = firma.RulajFurnizorLuniDinPerioada(inceput1, sfarsit1);
                        valori2 = firma.RulajFurnizorLuniDinPerioada(inceput2, sfarsit2);
                        
                        PopuleazaStatisticiLuni(listViewStatisticiPer1, GraphicLibrary.LuniSiAnPrescurtatePerioada(inceput1, sfarsit1),
                          firma.RulajFurnizorLuniDinPerioada(inceput1, sfarsit1), firma.PlatiFurnizorLuniDinPerioada(inceput1, sfarsit1),
                          firma.NumarFacturiFurnizorLuniDinPerioada(inceput1, sfarsit1), firma.NumarPlatiLuniDinPerioada(inceput1, sfarsit1));

                        PopuleazaStatisticiLuni(listViewStatisticiPer2, GraphicLibrary.LuniSiAnPrescurtatePerioada(inceput2, sfarsit2),
                            firma.RulajFurnizorLuniDinPerioada(inceput2, sfarsit2), firma.PlatiFurnizorLuniDinPerioada(inceput2, sfarsit2),
                            firma.NumarFacturiFurnizorLuniDinPerioada(inceput2, sfarsit2), firma.NumarPlatiLuniDinPerioada(inceput2, sfarsit2));

                        PopuleazaStatisticiComparareLuni(listViewComparatiePerioade, GraphicLibrary.LuniPrescurtate(), firma.RulajFurnizorLuniDinPerioada(inceput1, sfarsit1),
                            firma.RulajFurnizorLuniDinPerioada(inceput2, sfarsit2),
                            firma.NumarFacturiFurnizorLuniDinPerioada(inceput1, sfarsit1),
                            firma.NumarFacturiFurnizorLuniDinPerioada(inceput2, sfarsit2));


                        columnHeaderComparatie1Per1.Text = "Rulaj " + (string)comboBoxComparatieAn1.SelectedItem;
                        columnHeaderComparatie2Per1.Text = "Nr. facturi " + (string)comboBoxComparatieAn1.SelectedItem;

                        columnHeaderComparatie1Per2.Text = "Rulaj " + (string)comboBoxComparatieAn2.SelectedItem;
                        columnHeaderComparatie2Per2.Text = "Nr. facturi " + (string)comboBoxComparatieAn2.SelectedItem;

                        columnHeaderStat2Per1.Text = "Plati";
                        columnHeaderStat2Per2.Text = "Plati";

                        tabControlPer1.SelectTab(1);
                        tabControlPer2.SelectTab(1);


                        max = valori1.Union(valori2).OrderByDescending(x => x).First();

                        break;
                    case "Grafic comparatie plati furnizor":
                        valori1 = firma.PlatiFurnizorLuniDinPerioada(inceput1, sfarsit1);
                        valori2 = firma.PlatiFurnizorLuniDinPerioada(inceput2, sfarsit2);
                        

                        PopuleazaStatisticiLuni(listViewStatisticiPer1, GraphicLibrary.LuniSiAnPrescurtatePerioada(inceput1, sfarsit1),
                          firma.PlatiFurnizorLuniDinPerioada(inceput1, sfarsit1), firma.PlatiFurnizorLuniDinPerioada(inceput1, sfarsit1),
                          firma.NumarFacturiFurnizorLuniDinPerioada(inceput1, sfarsit1), firma.NumarPlatiLuniDinPerioada(inceput1, sfarsit1));

                        PopuleazaStatisticiLuni(listViewStatisticiPer2, GraphicLibrary.LuniSiAnPrescurtatePerioada(inceput2, sfarsit2),
                            firma.PlatiFurnizorLuniDinPerioada(inceput2, sfarsit2), firma.PlatiFurnizorLuniDinPerioada(inceput2, sfarsit2),
                            firma.NumarFacturiFurnizorLuniDinPerioada(inceput2, sfarsit2), firma.NumarPlatiLuniDinPerioada(inceput2, sfarsit2));

                        PopuleazaStatisticiComparareLuni(listViewComparatiePerioade, GraphicLibrary.LuniPrescurtate(), firma.PlatiFurnizorLuniDinPerioada(inceput1, sfarsit1),
                            firma.PlatiFurnizorLuniDinPerioada(inceput2, sfarsit2),
                            firma.NumarPlatiLuniDinPerioada(inceput1, sfarsit1),
                            firma.NumarPlatiLuniDinPerioada(inceput2, sfarsit2));


                        columnHeaderComparatie1Per1.Text = "Plati " + (string)comboBoxComparatieAn1.SelectedItem;
                        columnHeaderComparatie2Per1.Text = "Nr. tranzactii " + (string)comboBoxComparatieAn1.SelectedItem;

                        columnHeaderComparatie1Per2.Text = "Plati " + (string)comboBoxComparatieAn2.SelectedItem;
                        columnHeaderComparatie2Per2.Text = "Nr. tranzactii " + (string)comboBoxComparatieAn2.SelectedItem;

                        columnHeaderStat2Per1.Text = "Plati";
                        columnHeaderStat2Per2.Text = "Plati";

                        tabControlPer1.SelectTab(1);
                        tabControlPer2.SelectTab(1);

                        max = valori1.Union(valori2).OrderByDescending(x => x).First();

                        break;
                    default:

                        valori1 = firma.RulajClientLuniDinPerioada(inceput1, sfarsit1);
                        valori2 = firma.RulajClientLuniDinPerioada(inceput2, sfarsit2);

                        PopuleazaStatisticiLuni(listViewStatisticiPer1, GraphicLibrary.LuniSiAnPrescurtatePerioada(inceput1, sfarsit1),
                          firma.RulajClientLuniDinPerioada(inceput1, sfarsit1), firma.IncasariClientLuniDinPerioada(inceput1, sfarsit1),
                          firma.NumarFacturiClientLuniDinPerioada(inceput1, sfarsit1), firma.NumarIncasariLuniDinPerioada(inceput1, sfarsit1));

                        PopuleazaStatisticiLuni(listViewStatisticiPer2, GraphicLibrary.LuniSiAnPrescurtatePerioada(inceput2, sfarsit2),
                            firma.RulajClientLuniDinPerioada(inceput2, sfarsit2), firma.IncasariClientLuniDinPerioada(inceput2, sfarsit2),
                            firma.NumarFacturiClientLuniDinPerioada(inceput2, sfarsit2), firma.NumarIncasariLuniDinPerioada(inceput2, sfarsit2));

                        PopuleazaStatisticiComparareLuni(listViewComparatiePerioade, GraphicLibrary.LuniPrescurtate(), firma.RulajClientLuniDinPerioada(inceput1, sfarsit1),
                            firma.RulajClientLuniDinPerioada(inceput2, sfarsit2),
                            firma.NumarFacturiClientLuniDinPerioada(inceput1, sfarsit1),
                            firma.NumarFacturiClientLuniDinPerioada(inceput2, sfarsit2));


                        columnHeaderComparatie1Per1.Text = "Rulaj " + (string)comboBoxComparatieAn1.SelectedItem;
                        columnHeaderComparatie2Per1.Text = "Nr. facturi " + (string)comboBoxComparatieAn1.SelectedItem;

                        columnHeaderComparatie1Per2.Text = "Rulaj " + (string)comboBoxComparatieAn2.SelectedItem;
                        columnHeaderComparatie2Per2.Text = "Nr. facturi " + (string)comboBoxComparatieAn2.SelectedItem;

                        columnHeaderStat2Per1.Text = "Incasari";
                        columnHeaderStat2Per2.Text = "Incasari";

                        tabControlPer1.SelectTab(0);
                        tabControlPer2.SelectTab(0);

                        max = valori1.Union(valori2).OrderByDescending(x => x).First();

                        break;

                }
                
                textBoxPer1TotalFacturiClient.Text = firma.TotalFacturiClientPerioada(inceput1, sfarsit1).ToString("0.00");
                textBoxPer1TotalFacturiClientFaraTVA.Text = firma.TotalFacturiClientPerioadaFaraTVA(inceput1, sfarsit1).ToString("0.00");
                textBoxPer1TotalTVAFacturiClient.Text = firma.TotalTVAFacturiClientPerioada(inceput1, sfarsit1).ToString("0.00");
                textBoxPer1NumarFacturiClient.Text = firma.NumarFacturiClientPerioada(inceput1, sfarsit1).ToString();

                textBoxPer2TotalFacturiClient.Text = firma.TotalFacturiClientPerioada(inceput2, sfarsit2).ToString("0.00");
                textBoxPer2TotalFacturiClientFaraTVA.Text = firma.TotalFacturiClientPerioadaFaraTVA(inceput2, sfarsit2).ToString("0.00");
                textBoxPer2TotalTVAFacturiClient.Text = firma.TotalTVAFacturiClientPerioada(inceput2, sfarsit2).ToString("0.00");
                textBoxPer2NumarFacturiClient.Text = firma.NumarFacturiClientPerioada(inceput2, sfarsit2).ToString();


                textBoxFirma1TotalFacturiFurnizor.Text = firma.TotalFacturiFurnizorPerioada(inceput1, sfarsit1).ToString("0.00");
                textBoxFirma1TotalFacturiFurnizorFaraTVA.Text = firma.TotalFacturiFurnizorPerioadaFaraTVA(inceput1, sfarsit1).ToString("0.00");
                textBoxFirma1TotalTVAFacturiFurnizor.Text = firma.TotalTVAFacturiFurnizorPerioada(inceput1, sfarsit1).ToString("0.00");
                textBoxFirma1NumarFacturiFurnizor.Text = firma.NumarFacturiFurnizorPerioada(inceput1, sfarsit1).ToString();

                textBoxFirma2TotalFacturiFurnizor.Text = firma.TotalFacturiFurnizorPerioada(inceput2, sfarsit2).ToString("0.00");
                textBoxFirma2TotalFacturiFurnizorFaraTVA.Text = firma.TotalFacturiFurnizorPerioadaFaraTVA(inceput2, sfarsit2).ToString("0.00");
                textBoxFirma2TotalTVAFacturiFurnizor.Text = firma.TotalTVAFacturiFurnizorPerioada(inceput2, sfarsit2).ToString("0.00");
                textBoxFirma2NumarFacturiFurnizor.Text = firma.NumarFacturiFurnizorPerioada(inceput2, sfarsit2).ToString();

                labelAnPer1.Text = (string)comboBoxComparatieAn1.SelectedItem;
                labelAnPer2.Text = (string)comboBoxComparatieAn2.SelectedItem;

                panel.CreateGraphics().Clear(Color.White);

                GraphicLibrary.GraficLinie(panel.CreateGraphics(), 557, 450, listViewStatisticiPer2.Width - 9, listViewComparatiePerioade.Height - 26, valori1, GraphicLibrary.LuniPrescurtatePerioada(inceput1, sfarsit1), Color.Red, false, "Total", "Luni", true, max);
                GraphicLibrary.GraficLinie(panel.CreateGraphics(), 557, 450, listViewStatisticiPer2.Width - 9, listViewComparatiePerioade.Height - 26, valori2, Color.Blue, false, "Total", "Luni", true, max);
                
            }
        }

        private void textBoxCautareStoc_TextChanged(object sender, EventArgs e)
        {
            CautareStoc(textBoxCautareStoc.Text);
        }


        public string FormatWhiteSpace(string text)
        {
            return text.Replace(" ", "%20");
        }

        public string FormatDate(string date)
        {
            return date.Replace(".", "");
        }

        public string EvenimentFormatatGoogleCalendar(Eveniment eve)
        {
            string _base = @"http://www.google.com/calendar/event?action=TEMPLATE";

            string den = @"&text=" + FormatWhiteSpace(eve.Denumire);

            string date = @"&dates=" + FormatDate(eve.Data_Eveniment.ToString("yyyy/MM/dd")) + @"/" + FormatDate(eve.Data_Eveniment.AddDays(1).ToString("yyyy/MM/dd"));

            string details = @"&details=" + FormatWhiteSpace(eve.Note);

            return _base + den + date + details;
            
        }

        #endregion

        #region butoaneNavigare

        private void buttonGeneralFirma_Click(object sender, EventArgs e)
        {
            if(panelCurent != panelFirma)
            {
                if (firmaSelectata != null)
                {
                    SwitchPanel(panelFirma);
                }
            }
        }

        private void buttonFacturiFirma_Click(object sender, EventArgs e)
        {
            if (panelCurent != panelFacturiFirma)
            {
                if (firmaSelectata != null)
                {
                    firmaSelectata.FacturiClient = DbaseConnection.DBFCitesteFacturiClientFirma(firmaSelectata, Settings.Ani_InUz);

                    // firmaSelectata.FacturiClient = DbaseConnection.DBFCitesteFacturiClientFirma(firmaSelectata, "2017").Union(DbaseConnection.DBFCitesteFacturiClientFirma(firmaSelectata, "2018")).ToList();

                    firmaSelectata.FacturiFurnizor = DbaseConnection.DBFCitesteFacturiFurnizorFirma(firmaSelectata, Settings.Ani_InUz);

                    firmaSelectata.Tranzactii = DbaseConnection.DBFCitesteTranzactiiFirma(firmaSelectata, Settings.Ani_InUz);

                    facturiClientDisplay = firmaSelectata.FacturiClient;
                    facturiFurnizorDisplay = firmaSelectata.FacturiFurnizor;

                    modDisplayFacturiClient = "Normal";
                    GestioneazaInterfataFacturiClient();

                    SwitchPanel(panelFacturiFirma);

                    if (facturiClientDisplay.Count > 0)
                    {

                        dateTimePickerClientInceput.Value = firmaSelectata.FacturiClient[0].Data_Emitere;
                        dateTimePickerClientSfarsit.Value = DateTime.Today;
                    }

                    if (facturiFurnizorDisplay.Count > 0)
                    {

                        dateTimePickerFurnizorInceput.Value = firmaSelectata.FacturiFurnizor[0].Data_Emitere;
                        dateTimePickerFurnizorSfarsit.Value = DateTime.Today;
                    }

                    listViewProduseFacClient.Items.Clear();
                    listViewProduseFacFurnizor.Items.Clear();

                    PopuleazaListaFacturi(listViewFacturiClient, facturiClientDisplay, textBoxFacturiClientPerioada, labelNumarFacturiClient);
                    PopuleazaListaFacturi(listViewFacturiFurnizor, facturiFurnizorDisplay, textBoxFacturiFurnizorPerioada, labelNumarFacturiFurnizor);
                }

            }

        }

        private void buttonTranzactiiFirma_Click(object sender, EventArgs e)
        {
            if (panelCurent != panelTranzactiiFirma)
            {
                if (firmaSelectata != null)
                {
                    firmaSelectata.Tranzactii = DbaseConnection.DBFCitesteTranzactiiFirma(firmaSelectata, Settings.Ani_InUz);

                    //firmaSelectata.Tranzactii = DbaseConnection.DBFCitesteTranzactiiFirma(firmaSelectata, "2017").Union(DbaseConnection.DBFCitesteTranzactiiFirma(firmaSelectata, "2018")).ToList();

                    SwitchPanel(panelTranzactiiFirma);

                    tranzactiiDisplay = FiltreazaTranzactii(comboBoxFirmaTranzactiiAfisare.SelectedItem.ToString(), firmaSelectata.Tranzactii);

                    if (tranzactiiDisplay.Count > 0)
                    {
                        if (tranzactiiDisplay[0].Data_Tranzactie >= dateTimePickerTranzactiiInceput.MinDate)
                        {
                            dateTimePickerTranzactiiInceput.Value = tranzactiiDisplay[0].Data_Tranzactie;
                            dateTimePickerTranzactiiSfarsit.Value = DateTime.Today;
                        }
                    }
                    else
                    {
                        dateTimePickerTranzactiiInceput.Value = DateTime.Today;
                        dateTimePickerTranzactiiSfarsit.Value = DateTime.Today;
                    }

                    ReseteazaInfoEveniment();

                    PopuleazaListaTranzactii(listViewTranzactii, tranzactiiDisplay, textBoxTotalTranzactii);
                    
                }
            }
        }

        private void buttonEvenimenteFirma_Click(object sender, EventArgs e)
        {
            if (panelCurent != panelEvenimenteFirma)
            {
                if (firmaSelectata != null)
                {
                    //firmaSelectata.Evenimente = TextConnection.CitesteEvenimenteFirma(firmaSelectata);

                    SwitchPanel(panelEvenimenteFirma);

                    firmaSelectata.GenereazaEvenimentScadentaFacturaClient();

                    evenimenteDisplay = FiltreazaEvenimenteFirma(comboBoxFirmaEvenimentAfisare.SelectedItem.ToString(), firmaSelectata.Evenimente);

                    if (evenimenteDisplay.Count > 0)
                    {
                        dateTimePickerFirmaEvenimenteInceput.Value = firmaSelectata.Evenimente[0].Data_Eveniment;
                        dateTimePickerFirmaEvenimenteSfarsit.Value = firmaSelectata.Evenimente[firmaSelectata.Evenimente.Count - 1].Data_Eveniment;
                    }
                    else
                    {
                        dateTimePickerFirmaEvenimenteInceput.Value = DateTime.Today;
                        dateTimePickerFirmaEvenimenteSfarsit.Value = DateTime.Today;
                    }


                    PopuleazaListaEvenimenteFirma(listViewFirmaEvenimente, evenimenteDisplay);

                    evenimentSelectat = null;
                    GestioneazaInterfataEvenimente();
                    ReseteazaInfoEveniment();
                }
            }
        }

        private void buttonListaFirme_Click(object sender, EventArgs e)
        {
            if (panelCurent != panelListaFirme)
            {
                SwitchPanel(panelListaFirme);
               // ReseteazaInfoFirma();
            }
        }

        private void buttonWebsiteUtile_Click(object sender, EventArgs e)
        {
            if (panelCurent != panelWebsiteUtile)
                SwitchPanel(panelWebsiteUtile);
        }

        private void buttonNotificari_Click(object sender, EventArgs e)
        {
            if (panelCurent != panelEvenimenteUrmatoare)
            {
                SwitchPanel(panelEvenimenteUrmatoare);

                //reseteaza afisarea
                comboBoxEvenimenteAfisare.SelectedItem = comboBoxEvenimenteAfisare.Items[3];
                
                evenimenteUrmatoare = Settings.Evenimente_NerezolvateInInterval(DateTime.Now, DateTime.Now.AddDays(Settings.NotificariZileAvans));
                
                //adu la zi firmele pentru care se vor face notificari
                evenimenteUrmatoareDisplay = FiltreazaEvenimente(comboBoxEvenimenteAfisare.SelectedItem.ToString(), evenimenteUrmatoare);
                PopuleazaListaEvenimente(listViewEvenimenteUrmatoare, evenimenteUrmatoareDisplay, false);
                
                GestioneazaNotificari(evenimenteUrmatoare);
            }
        }

        private void buttonIstoric_Click(object sender, EventArgs e)
        {
            if(panelCurent != panelIstoric)
            {
                SwitchPanel(panelIstoric);

                comboBoxEvenimenteIstoricAfisare.SelectedItem = comboBoxEvenimenteIstoricAfisare.Items[3];

                evenimenteIstoric = Settings.Evenimente_Toate_Istoric();

                evenimenteIstoricDisplay = FiltreazaEvenimente(comboBoxEvenimenteIstoricAfisare.SelectedItem.ToString(), evenimenteIstoric);

                PopuleazaListaEvenimente(listViewEvenimenteIstoric, evenimenteIstoricDisplay, true);

                GestioneazaNotificari(evenimenteUrmatoare);
            }
        }


        private void buttonSetari_Click(object sender, EventArgs e)
        {
            if(panelCurent != panelSetariGenerale)
            {
                SwitchPanel(panelSetariGenerale);
            }
        }

        private void buttonComparatie_Click(object sender, EventArgs e)
        {
            if (panelCurent != panelComparatie)
            {
                SwitchPanel(panelComparatie);

                if (firmaSelectata != null)
                {
                    comboBoxCriteriuComparatie.SelectedItem = comboBoxCriteriuComparatie.Items[0]; //se executa automat, este event
                   // ComparaFirmaPePerioada(panelComparatie, firmaSelectata, "Grafic comparatie rulaje client", new DateTime(int.Parse((string)comboBoxComparatieAn1.SelectedItem), 1, 1), new DateTime(int.Parse((string)comboBoxComparatieAn1.SelectedItem), 12, 31), new DateTime(int.Parse((string)comboBoxComparatieAn2.SelectedItem), 1, 1), new DateTime(int.Parse((string)comboBoxComparatieAn2.SelectedItem), 12, 31));

                }
            }
        }

        private void buttonStocuri_Click(object sender, EventArgs e)
        {
            if(panelCurent != panelStocuri)
            {
                SwitchPanel(panelStocuri);

                PopuleazaListaStocuri(listViewStocuri, stocuriDisplay);
            }
        }

        private void buttonInformatiiGenerale_Click(object sender, EventArgs e)
        {
            if (panelCurent != panelInformatiiGenerale)
            {
                var watch = Stopwatch.StartNew();

                List<object> rez = DbaseConnection.DBFTotalFacturiClientPerioada(Settings.Ani_InUz, new DateTime(2017, 1,1), new DateTime(2018, 12,31));
                List<object> rez2 = DbaseConnection.DBFTotalFacturiFurnizorPerioada(Settings.Ani_InUz, new DateTime(2017, 1, 1), new DateTime(2018, 12, 31));
                List<object> rez3 = DbaseConnection.DBFTotalIncasari(Settings.Ani_InUz, new DateTime(2017, 1, 1), new DateTime(2018, 12, 31));
                List<object> rez4 = DbaseConnection.DBFTotalPlati(Settings.Ani_InUz, new DateTime(2017, 1, 1), new DateTime(2018, 12, 31));

                watch.Stop();

                MessageBox.Show(watch.ElapsedMilliseconds.ToString());

                textBox2.Text = Math.Round((decimal)rez[0], 3).ToString() + " RON";
                textBox3.Text = Math.Round((decimal)rez[1], 3).ToString() + " RON";
                textBox4.Text = Math.Round((decimal)rez[2], 3).ToString() + " RON";
                textBox5.Text = rez[3].ToString();
                textBox6.Text = Math.Round((decimal)rez[4], 3).ToString() + @"%";


                textBox7.Text = Math.Round((decimal)rez2[0], 3).ToString() + " RON";
                textBox8.Text = Math.Round((decimal)rez2[1], 3).ToString() + " RON";
                textBox9.Text = Math.Round((decimal)rez2[2], 3).ToString() + " RON";
                textBox10.Text = rez2[3].ToString();
                textBox11.Text = Math.Round((decimal)rez2[4], 3).ToString() + @"%";

                

                textBox12.Text = Math.Round((decimal)rez3[0], 3).ToString() + " RON";
                textBox13.Text = Math.Round((decimal)rez3[1], 3).ToString() + " RON";
                textBox14.Text = Math.Round((decimal)rez3[2], 3).ToString() + " RON";
                textBox15.Text = rez3[3].ToString();
                textBox16.Text = Math.Round((decimal)rez3[4], 3).ToString() + @"%";



                textBox17.Text = Math.Round((decimal)rez4[0], 3).ToString() + " RON";
                textBox18.Text = Math.Round((decimal)rez4[1], 3).ToString() + " RON";
                textBox19.Text = Math.Round((decimal)rez4[2], 3).ToString() + " RON";
                textBox20.Text = rez4[3].ToString();
                textBox21.Text = Math.Round((decimal)rez4[4], 3).ToString() + @"%";

                SwitchPanel(panelInformatiiGenerale);

                List<decimal> a = new List<decimal>();
                List<decimal> b = new List<decimal>();

                for(int i = 1; i <= 12; i++)
                {
                    a.Add((decimal)DbaseConnection.DBFTotalFacturiClientPerioada(Settings.Ani_InUz, new DateTime(2018, i, 1), new DateTime(2018, i, DateTime.DaysInMonth(2018, i)))[0]);
                    b.Add((decimal)DbaseConnection.DBFTotalFacturiClientPerioada(Settings.Ani_InUz, new DateTime(2017, i, 1), new DateTime(2017, i, DateTime.DaysInMonth(2017, i)))[0]);
                }

                GraphicLibrary.GraficLinie(panelInformatiiGenerale.CreateGraphics(), 850, 27, 200, 291, a, Color.Red, false, "Total", "Luni", true);
                GraphicLibrary.GraficLinie(panelInformatiiGenerale.CreateGraphics(), 850, 27, 200, 291, b, Color.Indigo, false, "Total", "Luni", false);

            }
        }

        private void buttonScadente_Click(object sender, EventArgs e)
        {
            if (panelCurent != panelScadente)
            {
                SwitchPanel(panelScadente);

                listViewScadente.Items.Clear();

                dateTimePickerScadenteInceput.Value = DateTime.Today;
                dateTimePickerScadenteSfarsit.Value = DateTime.Today.AddDays(7);

                PopuleazaListaFacturiScadente(listViewScadente, FiltreazaFacturiScadente(comboBoxScadenteAfisare.SelectedItem.ToString(), DbaseConnection.DBFFacturi(Settings.Ani_InUz, dateTimePickerScadenteInceput.Value, dateTimePickerScadenteSfarsit.Value)));


                // se executa cand ^ se executa PopuleazaListaFacturiScadente(listViewScadente, DbaseConnection.DBFFacturi(Settings.Ani_InUz, dateTimePickerScadenteInceput.Value, dateTimePickerScadenteSfarsit.Value));
            }
        }

        private void buttonLansareComenziFirma_Click(object sender, EventArgs e)
        {
            if (panelCurent != panelLansareComenzi)
            {
                if (firmaSelectata != null)
                {
                    SwitchPanel(panelLansareComenzi);

                    comenziDisplay = firmaSelectata.Comenzi.OrderBy(x => x.Data_Lansare).ToList();

                    PopuleazaListaComenziFirma(listViewComenziFirma, comenziDisplay);
                }
            }
        }
        

        #endregion

        #region Timer

        private void timerActualizareDate_Tick(object sender, EventArgs e)
        {
            Thread t = new Thread(() => Settings.Actualizeaza_DBF(Settings.Ani_InUz));
            t.Start();

            Thread a = new Thread(() => MessageBox.Show("Se actualizeaza baza de date... \n\r Aceasta operatiune poate dura cateva secunde"));
            a.Start();
            // Settings.Actualizeaza_DBF(Settings.Ani_InUz);


            AfiseazaTimpActualizare(Settings.Ultima_Actualizare_DBF, Settings.Urmatoare_Actualizare_DBF);

            firme = DbaseConnection.DBFCitesteFirme(Settings.Ani_InUz); //cel mai curent an, fisierul contine toate firmele cu care s-a lucrat vreodata
            firmeDisplay = firme;
            

            ActualizeazaFirmeActive();

            Settings.ScrieSetari();

            comboBoxFirmeAfisare.SelectedItem = comboBoxFirmeAfisare.Items[2];

            GestioneazaInterfataGeneral();

          //  ReseteazaInfoFirma();
            
            evenimenteUrmatoare = Settings.Evenimente_NerezolvateInInterval(DateTime.Now, DateTime.Now.AddDays(5));
            evenimenteUrmatoareDisplay = evenimenteUrmatoare;
            
            GestioneazaNotificari(evenimenteUrmatoare);
        }

        #endregion

        #region Firma

        private void textBoxCautare_TextChanged(object sender, EventArgs e)
        {
            CautareFirma(comboBoxFirmaCautareDupa.Text, textBoxCautareFirma.Text);
        }

        private void listViewFirmaDocumente_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (fisiereDisplay != null && fisiereDisplay.Count > 0)
                {
                    if (listViewFirmaDocumente.FocusedItem.Bounds.Contains(e.Location))
                    {
                        try
                        {
                            Process.Start(fisiereDisplay[listViewFirmaDocumente.FocusedItem.Index]);
                        }
                        catch
                        {
                            MessageBox.Show("Nu s-a putut deschide fisierul.");
                        }
                    }
                }
                
            }
        }

        private void listViewFirmaDocumente_MouseClick(object sender, MouseEventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (fisiereDisplay != null && fisiereDisplay.Count > 0)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        if (listViewFirmaDocumente.FocusedItem.Bounds.Contains(e.Location))
                        {
                            contextMenuStripFirmaDocumente.Show(Cursor.Position);
                        }
                    }
                }
            }
        }

        private void listViewFirme_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (listViewFirme.FocusedItem.Bounds.Contains(e.Location))
                {
                    if (firmaSelectata != null)
                    {
                        if (panelCurent != panelFacturiFirma)
                        {
                            /* firmaSelectata.FacturiClient = DbaseConnection.DBFCitesteFacturiClientFirma(firmaSelectata, Settings.Ani_InUz);

                             // firmaSelectata.FacturiClient = DbaseConnection.DBFCitesteFacturiClientFirma(firmaSelectata, "2017").Union(DbaseConnection.DBFCitesteFacturiClientFirma(firmaSelectata, "2018")).ToList();

                             firmaSelectata.FacturiFurnizor = DbaseConnection.DBFCitesteFacturiFurnizorFirma(firmaSelectata, Settings.Ani_InUz);
                             firmaSelectata.Tranzactii = DbaseConnection.DBFCitesteTranzactiiFirma(firmaSelectata, Settings.Ani_InUz);

                             facturiClientDisplay = firmaSelectata.FacturiClient;
                             facturiFurnizorDisplay = firmaSelectata.FacturiFurnizor;

                             SwitchPanel(panelFacturiFirma);

                             if (facturiClientDisplay.Count > 0)
                             {

                                 dateTimePickerClientInceput.Value = firmaSelectata.FacturiClient[0].Data_Emitere;
                                 dateTimePickerClientSfarsit.Value = firmaSelectata.FacturiClient[firmaSelectata.FacturiClient.Count - 1].Data_Emitere;
                             }

                             if (facturiFurnizorDisplay.Count > 0)
                             {

                                 dateTimePickerFurnizorInceput.Value = firmaSelectata.FacturiFurnizor[0].Data_Emitere;
                                 dateTimePickerFurnizorSfarsit.Value = firmaSelectata.FacturiFurnizor[firmaSelectata.FacturiFurnizor.Count - 1].Data_Emitere;
                             }

                             listViewProduseFacClient.Items.Clear();
                             listViewProduseFacFurnizor.Items.Clear();

                             PopuleazaListaFacturi(listViewFacturiClient, facturiClientDisplay, textBoxFacturiClientPerioada);
                             PopuleazaListaFacturi(listViewFacturiFurnizor, facturiFurnizorDisplay, textBoxFacturiFurnizorPerioada);

     */

                            SwitchPanel(panelFirma);
                        }
                    }
                }
            }
        }

        private void listViewFirme_SelectedIndexChanged(object sender, EventArgs e)
        {
            firmaSelectata = null;

            if (listViewFirme.SelectedItems.Count > 0)
            {
                if (firmeDisplay.Count > 0)
                {
                    if (panelButoaneFirma.Enabled == false && panelButoaneFirma.Size != new Size(150, 216))
                    {
                        panelButoaneFirma.Enabled = true;
                        panelButoaneFirma.Size = new Size(150, 216);
                    }

                    firmaSelectata = firmeDisplay[listViewFirme.SelectedIndices[0]];

                    PopuleazaInformatieFirma(firmaSelectata);

                    labelFirmaSelectata.Text = firmaSelectata.Nume;

                    GestioneazaInterfataGeneral();
                    
                    return;
                }
            }

            panelButoaneFirma.Enabled = false;
            panelButoaneFirma.Size = new Size(150, 0);
            ReseteazaInfoFirma();
            
        }


        private void textBoxFirmaSoldInitialClient_Leave(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (textBoxFirmaSoldInitialClient.Text != "")
                    firmaSelectata.SoldInitialClient = decimal.Parse(textBoxFirmaSoldInitialClient.Text);

                firmaSelectata.ScrieInformatiiAditionale();
            }
        }

        private void textBoxFirmaSoldInitialFurnizor_Leave(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (textBoxFirmaSoldInitialFurnizor.Text != "")
                    firmaSelectata.SoldInitialFurnizor = decimal.Parse(textBoxFirmaSoldInitialFurnizor.Text);

                firmaSelectata.ScrieInformatiiAditionale();
            }
        }

        private void textBoxFirmaTermenPlata_Leave(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (textBoxFirmaTermenPlata.Text != "")
                    firmaSelectata.Termen_Plata = int.Parse(textBoxFirmaTermenPlata.Text);

                firmaSelectata.ScrieInformatiiAditionale();
            }
        }

        private void textBoxFirmaPlafonClient_Leave(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (textBoxFirmaPlafonClient.Text != "")
                    firmaSelectata.Plafon_Client = decimal.Parse(textBoxFirmaPlafonClient.Text);

                firmaSelectata.ScrieInformatiiAditionale();
            }
        }

        private void textBoxFirmaPlafonFurnizor_Leave(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (textBoxFirmaPlafonFurnizor.Text != "")
                    firmaSelectata.Plafon_Furnizor = decimal.Parse(textBoxFirmaPlafonFurnizor.Text);

                firmaSelectata.ScrieInformatiiAditionale();
            }
        }

       
        private void checkBoxFirmaFavorit_CheckedChanged(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                firmaSelectata.Favorit = checkBoxFirmaFavorit.Checked;
                
                firmaSelectata.ScrieInformatiiAditionale();
            }
        }

        private void checkBoxFirmaNotificari_CheckedChanged(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                firmaSelectata.Afiseaza_Notificari = checkBoxFirmaNotificari.Checked;

                firmaSelectata.ScrieInformatiiAditionale();
            }
        }

        private void checkBoxFirmaTermenPlata_CheckedChanged(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                firmaSelectata.Foloseste_Termen_Plata = checkBoxFirmaTermenPlata.Checked;

                textBoxFirmaTermenPlata.ReadOnly = !checkBoxFirmaTermenPlata.Checked;

                firmaSelectata.ScrieInformatiiAditionale();
            }
        }

        private void checkBoxFirmaPlafonClient_CheckedChanged(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                firmaSelectata.Foloseste_Plafon_Client = checkBoxFirmaPlafonClient.Checked;

                textBoxFirmaPlafonClient.ReadOnly = !checkBoxFirmaPlafonClient.Checked;

                firmaSelectata.ScrieInformatiiAditionale();
            }
        }

        private void checkBoxFirmaPlafonFurnizor_CheckedChanged(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                firmaSelectata.Foloseste_Plafon_Furnizor = checkBoxFirmaPlafonFurnizor.Checked;

                textBoxFirmaPlafonFurnizor.ReadOnly = !checkBoxFirmaPlafonFurnizor.Checked;

                firmaSelectata.ScrieInformatiiAditionale();
            }
        }

        private void comboBoxFirmeAfisare_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReseteazaInfoFirma();

            firmeDisplay = FiltreazaFirme(comboBoxFirmeAfisare.SelectedItem.ToString(), firme);
            PopuleazaListaFirme(listViewFirme, firmeDisplay);
        }

        #endregion

        #region EvenimenteFirma

        private void listViewEvenimentDocumente_MouseClick(object sender, MouseEventArgs e)
        {
            //SUPER NICE - DESCHIDE CONTEXT MENU DOAR PENTRU ITEMELE HOVERATE
            if (firmaSelectata != null)
            {
                if (!(evenimentSelectat is null))
                {
                    if (fisiereSelectateEveniment != null && fisiereSelectateEveniment.Count > 0)
                    {
                        if (e.Button == MouseButtons.Right)
                        {
                            if (listViewEvenimentDocumente.FocusedItem.Bounds.Contains(e.Location))
                            {
                                contextMenuStripEvenimenteDocumente.Show(Cursor.Position);
                            }
                        }
                    }
                }

            }
        }

        private void listViewEvenimentDocumente_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (!(evenimentSelectat is null))
                {
                    if (fisiereSelectateEveniment != null && fisiereSelectateEveniment.Count > 0)
                    {
                        if (listViewEvenimentDocumente.FocusedItem.Bounds.Contains(e.Location))
                        {
                            try
                            {
                                Process.Start(fisiereSelectateEveniment[listViewEvenimentDocumente.FocusedItem.Index]);
                            }
                            catch
                            {
                                MessageBox.Show("Nu s-a putut deschide fisierul.");
                            }
                        }

                    }
                }
            }
        }

        private void listViewFirmaEvenimente_SelectedIndexChanged(object sender, EventArgs e)
        {
            evenimentSelectat = null;

            if (listViewFirmaEvenimente.SelectedItems.Count > 0)
            {
                if (evenimenteDisplay.Count > 0)
                {
                    evenimentSelectat = evenimenteDisplay[listViewFirmaEvenimente.SelectedIndices[0]];
                    
                    GestioneazaInterfataEvenimente();

                    PopuleazaInformatieEveniment(evenimentSelectat);

                    return;//nu il las sa treaca mai departe daca e selectata o factura pentru a nu reseta lista de produse
                }
            }

            _modModificareEveniment = false;
            GestioneazaInterfataEvenimente();
            ReseteazaInfoEveniment();
        }

        private void dateTimePickerFirmaEvenimenteInceput_ValueChanged(object sender, EventArgs e)
        {
            listViewFirmaEvenimente.Items.Clear();
            ReseteazaInfoEveniment();

            evenimenteDisplay = FiltreazaEvenimenteFirma(comboBoxFirmaEvenimentAfisare.SelectedItem.ToString(), firmaSelectata.EvenimentePePerioada(dateTimePickerFirmaEvenimenteInceput.Value, dateTimePickerFirmaEvenimenteSfarsit.Value));

            PopuleazaListaEvenimenteFirma(listViewFirmaEvenimente, evenimenteDisplay);
        }

        private void dateTimePickerFirmaEvenimenteSfarsit_ValueChanged(object sender, EventArgs e)
        {
            listViewFirmaEvenimente.Items.Clear();
            ReseteazaInfoEveniment();

            evenimenteDisplay = FiltreazaEvenimenteFirma(comboBoxFirmaEvenimentAfisare.SelectedItem.ToString(), firmaSelectata.EvenimentePePerioada(dateTimePickerFirmaEvenimenteInceput.Value, dateTimePickerFirmaEvenimenteSfarsit.Value));

            PopuleazaListaEvenimenteFirma(listViewFirmaEvenimente, evenimenteDisplay);
        }

        private void comboBoxFirmaEvenimentAfisare_SelectedIndexChanged(object sender, EventArgs e)
        {
            //resetez in cazul in care nu mai este in lista cel selectat
            ReseteazaInfoEveniment();

            if (firmaSelectata != null)
            {
                evenimenteDisplay = FiltreazaEvenimenteFirma(comboBoxFirmaEvenimentAfisare.SelectedItem.ToString(), firmaSelectata.EvenimentePePerioada(dateTimePickerFirmaEvenimenteInceput.Value, dateTimePickerFirmaEvenimenteSfarsit.Value));
                PopuleazaListaEvenimenteFirma(listViewFirmaEvenimente, evenimenteDisplay);
            }
        }

        private void buttonCreereComanda_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (comandaSelectata == null) // NU E COMANDA SELECTATA, NU SE PUNE PROBLEMA DE MODIFICARE
                {
                    if (InfoComandaOK())
                    {
                        Comanda com = new Comanda();

                        com.Id = -1;
                        com.Material = comboBoxMaterialComanda.Text;
                        com.Imprimeu = textBoxImprimeuComanda.Text;
                        com.Nume_Firma = firmaSelectata.Nume;
                        com.Pret_Unitar = decimal.Parse(textBoxPretUnitarComanda.Text);
                        com.Tip_Produs = comboBoxTipProdusComanda.Text;
                        com.Observatii = textBoxObservatiiComanda.Text;
                        com.Data_Lansare = dateTimePickerDataComanda.Value;
                        com.Cantitate = int.Parse(textBoxCantitateComanda.Text);
                        com.Dimensiuni = textBoxDimensiuniComanda.Text;

                        com.Cai_Documente = fisiereSelectateComanda;

                        firmaSelectata.SalveazaComanda(com, -1);

                        comenziDisplay = firmaSelectata.Comenzi.OrderBy(x => x.Data_Lansare).ToList();
                        PopuleazaListaComenziFirma(listViewComenziFirma, comenziDisplay);

                        _modModificareComanda = false;

                        ReseteazaInfoComanda();
                        GestioneazaInterfataComenzi();
                        
                    }
                    else
                    {
                        MessageBox.Show("Nu s-a putut creea comanda. Verificati datele introduse!");
                        return;
                    } //ceva mesaj calumea, un manager de erori
                }
                else if (comandaSelectata != null) //ESTE SELECTAT O COMANDA, BUTONUL S_AR NUMI MODIFICARE AICI
                {
                    if (_modModificareComanda == false)
                    {
                        _modModificareComanda = true;

                        GestioneazaInterfataComenzi();

                        return;
                    }
                    else if (_modModificareComanda == true) //AM DAT MODIFICARE, ACUMA BUTONUL SE NUMESTE CONFIMARE
                    {
                        if (InfoComandaOK())
                        {
                            Comanda com = new Comanda();

                            com.Id = -1;
                            com.Material = comboBoxMaterialComanda.Text;
                            com.Imprimeu = textBoxImprimeuComanda.Text;
                            com.Nume_Firma = firmaSelectata.Nume;
                            com.Pret_Unitar = decimal.Parse(textBoxPretUnitarComanda.Text);
                            com.Tip_Produs = comboBoxTipProdusComanda.Text;
                            com.Observatii = textBoxObservatiiComanda.Text;
                            com.Data_Lansare = dateTimePickerDataComanda.Value;
                            com.Cantitate = int.Parse(textBoxCantitateComanda.Text);
                            com.Dimensiuni = textBoxDimensiuniComanda.Text;

                            com.Cai_Documente = fisiereSelectateComanda;

                            firmaSelectata.SalveazaComanda(com, comandaSelectata.Id);

                            comenziDisplay = firmaSelectata.Comenzi.OrderBy(x => x.Data_Lansare).ToList();
                            PopuleazaListaComenziFirma(listViewComenziFirma, comenziDisplay);

                            _modModificareComanda = false;

                            ReseteazaInfoComanda();
                            GestioneazaInterfataComenzi();
                            
                        }
                        else
                        {
                            MessageBox.Show("Nu s-a putut creea comanda. Verificati datele introduse!");
                            return;
                        }

                        GestioneazaInterfataComenzi();

                        return;
                    }
                }

            }
        }

        private void buttonStergereComanda_Click(object sender, EventArgs e)
        {
            if(firmaSelectata != null)
            {
                if (comandaSelectata != null)
                {
                    DialogResult dr;

                    dr = MessageBox.Show("Sigur doriti sa stergeti aceasta comanda?", "Stergere", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        Directory.Delete(firmaSelectata.CaleComanda(comandaSelectata.Id), true);

                        comandaSelectata = null;

                        comenziDisplay = firmaSelectata.Comenzi;
                        PopuleazaListaComenziFirma(listViewComenziFirma, comenziDisplay);

                        ReseteazaInfoComanda();
                        GestioneazaInterfataComenzi();
                    }
                }
            }
            
        }

        private void buttonFirmaEvenimentCreare_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (evenimentSelectat is null) //NU ESTE EVENIMENT SELECTAT, DECI NU SE PUNE PROBLEMA DE MODURI DE MODIFICARE
                {
                    if (InfoEvenimentOK())
                    {
                        Eveniment ev = new Eveniment();
                        ev.Id = -1;
                        ev.Denumire = textBoxEvenimentDenumire.Text;
                        ev.Tip = comboBoxEvenimentTip.Text;
                        ev.Stare = comboBoxEvenimentStare.SelectedItem.ToString();
                        ev.Data_Eveniment = dateTimePickerEvenimentData.Value;
                        ev.Note = textBoxEvenimentNote.Text;
                        ev.Cai_Documente = fisiereSelectateEveniment;

                        firmaSelectata.SalveazaEveniment(ev, -1);

                        ReseteazaInfoEveniment();

                        evenimenteDisplay = FiltreazaEvenimenteFirma(comboBoxFirmaEvenimentAfisare.SelectedItem.ToString(), firmaSelectata.Evenimente);

                        PopuleazaListaEvenimenteFirma(listViewFirmaEvenimente, evenimenteDisplay);
                    }
                    else
                    {
                        MessageBox.Show("Nu s-a putut creea evenimentul. Verificati datele introduse!");
                        return;
                    } //ceva mesaj calumea, un manager de erori
                }
                else if(!(evenimentSelectat is null)) //ESTE SELECTAT UN EVENIMENT, AICI BUTONUl S-AR NUMI "MODIFICARE"
                {
                    if (_modModificareEveniment == false) //CAND APAS BUTONUL CA SA MODIFIC
                    {
                        _modModificareEveniment = true;

                        GestioneazaInterfataEvenimente();

                        return;
                    }
                    else if (_modModificareEveniment == true) //CAND APAS BUTONUL CA SA CONFIRM
                    {
                        if (InfoEvenimentOK())
                        {
                            Eveniment ev = new Eveniment();
                            ev.Id = evenimentSelectat.Id;
                            ev.Denumire = textBoxEvenimentDenumire.Text;
                            ev.Tip = comboBoxEvenimentTip.Text;
                            ev.Stare = comboBoxEvenimentStare.SelectedItem.ToString();
                            ev.Data_Eveniment = dateTimePickerEvenimentData.Value;
                            ev.Note = textBoxEvenimentNote.Text;
                            ev.Cai_Documente = fisiereSelectateEveniment;

                            firmaSelectata.SalveazaEveniment(ev, evenimentSelectat.Id);

                            ReseteazaInfoEveniment();

                            evenimenteDisplay = FiltreazaEvenimenteFirma(comboBoxFirmaEvenimentAfisare.SelectedItem.ToString(), firmaSelectata.Evenimente);

                            PopuleazaListaEvenimenteFirma(listViewFirmaEvenimente, evenimenteDisplay);

                            _modModificareEveniment = false;
                        }
                        else
                        {
                            MessageBox.Show("Nu s-a putut creea evenimentul. Verificati datele introduse!");
                            return;
                        } //ceva mesaj calumea, un manager de erori

                        GestioneazaInterfataEvenimente();

                        return;
                    }
                }
            }
        }


        private void buttonFirmaEvenimentAdaugareDocument_Click(object sender, EventArgs e)
        {
            string[] fileNames = null;

            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Multiselect = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    fileNames = openFileDialog1.FileNames;
                }
            };

            if (fileNames != null)
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    listViewEvenimentDocumente.Items.Add(Path.GetFileName(fileNames[i]));
                    fisiereSelectateEveniment.Add(fileNames[i]);
                }
            }
        }

        private void buttonFirmaGoogleCalendar_Click(object sender, EventArgs e)
        {
            if (InfoEvenimentOK())
            {
                Eveniment ev = new Eveniment();

                ev.Id = 0;
                ev.Denumire = textBoxEvenimentDenumire.Text;
                ev.Tip = comboBoxEvenimentTip.Text;
                ev.Stare = comboBoxEvenimentStare.SelectedItem.ToString();
                ev.Data_Eveniment = dateTimePickerEvenimentData.Value;
                ev.Note = textBoxEvenimentNote.Text;
                ev.Cai_Documente = fisiereSelectateEveniment;

                try
                {
                    Process.Start(EvenimentFormatatGoogleCalendar(ev));
                }
                catch { }
            }
        }

        private void buttonFirmaEvenimentStergere_Click(object sender, EventArgs e)
        {
            if(firmaSelectata != null)
            {
                if (!(evenimentSelectat is null))
                {
                    DialogResult dr;

                    dr = MessageBox.Show("Sigur doriti sa stergeti acest eveniment?", "Stergere", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        Directory.Delete(firmaSelectata.CaleEveniment(evenimentSelectat.Id), true);

                        evenimentSelectat = null;

                        evenimenteDisplay = FiltreazaEvenimenteFirma(comboBoxFirmaEvenimentAfisare.SelectedItem.ToString(), firmaSelectata.Evenimente);
                        PopuleazaListaEvenimenteFirma(listViewFirmaEvenimente, evenimenteDisplay);

                        GestioneazaInterfataEvenimente();
                        ReseteazaInfoEveniment();
                    }
                }
            }
        }

        private void deschideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //SUPER NICE
            if (firmaSelectata != null)
            {
                if (fisiereSelectateEveniment != null && fisiereSelectateEveniment.Count > 0)
                {
                    try
                    {
                        Process.Start(fisiereSelectateEveniment[listViewEvenimentDocumente.FocusedItem.Index]);
                    }
                    catch
                    {
                        MessageBox.Show("Nu s-a putut deschide fisierul.");
                    }
                }
            }
        }

        private void stergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (fisiereSelectateEveniment != null && fisiereSelectateEveniment.Count > 0)
                {
                    DialogResult dr;

                    dr = MessageBox.Show("Sigur doriti sa stergeti acest fisier?", "Stergere", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {

                        File.Delete(fisiereSelectateEveniment[listViewEvenimentDocumente.FocusedItem.Index]);

                        fisiereSelectateEveniment.RemoveAt(listViewEvenimentDocumente.FocusedItem.Index);

                        listViewEvenimentDocumente.Items.Clear();

                        foreach (string s in fisiereSelectateEveniment)
                            listViewEvenimentDocumente.Items.Add(Path.GetFileName(s));
                    }
                }
            }
        }

        #endregion

        #region Evenimente

        private void comboBoxEvenimenteAfisare_SelectedIndexChanged(object sender, EventArgs e)
        {
            evenimenteUrmatoare = Settings.Evenimente_NerezolvateInInterval(DateTime.Now, DateTime.Now.AddDays(Settings.NotificariZileAvans));
            evenimenteUrmatoareDisplay = FiltreazaEvenimente(comboBoxEvenimenteAfisare.SelectedItem.ToString(), evenimenteUrmatoare);
            PopuleazaListaEvenimente(listViewEvenimenteUrmatoare, evenimenteUrmatoareDisplay, false);

        }

        private void textBoxEvenimenteUrmatoareZile_TextChanged(object sender, EventArgs e)
        {
            if (textBoxEvenimenteUrmatoareZile.Text == "")
                textBoxEvenimenteUrmatoareZile.Text = Settings.NotificariZileAvans.ToString();

            if (int.TryParse(textBoxEvenimenteUrmatoareZile.Text, out int n))
            {
                Settings.NotificariZileAvans = int.Parse(textBoxEvenimenteUrmatoareZile.Text);

                evenimenteUrmatoare = Settings.Evenimente_NerezolvateInInterval(DateTime.Now, DateTime.Now.AddDays(Settings.NotificariZileAvans));
                evenimenteUrmatoareDisplay = FiltreazaEvenimente(comboBoxEvenimenteAfisare.SelectedItem.ToString(), evenimenteUrmatoare);
                PopuleazaListaEvenimente(listViewEvenimenteUrmatoare, evenimenteUrmatoareDisplay, false);
                PopuleazaListaEvenimente(listViewEvenimenteUrmatoare, evenimenteUrmatoareDisplay, false);
            }
        }

        private void listViewEvenimenteUrmatoare_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (listViewEvenimenteUrmatoare.FocusedItem.Bounds.Contains(e.Location))
                {
                    firmaSelectata = firme.Find(x => x.Nume == evenimenteUrmatoareDisplay[listViewEvenimenteUrmatoare.SelectedIndices[0]].Nume_Firma);
                    
                    SwitchPanel(panelEvenimenteFirma);
                    
                    evenimenteDisplay = FiltreazaEvenimenteFirma(comboBoxFirmaEvenimentAfisare.SelectedItem.ToString(), firmaSelectata.Evenimente);

                    if (evenimenteDisplay.Count > 0)
                    {
                        dateTimePickerFirmaEvenimenteInceput.Value = firmaSelectata.Evenimente[0].Data_Eveniment;
                        dateTimePickerFirmaEvenimenteSfarsit.Value = DateTime.Today;
                    }
                    else
                    {
                        dateTimePickerFirmaEvenimenteInceput.Value = DateTime.Today;
                        dateTimePickerFirmaEvenimenteSfarsit.Value = DateTime.Today;
                    }
                    
                    PopuleazaListaEvenimenteFirma(listViewFirmaEvenimente, evenimenteDisplay);

                    if (firmaSelectata != null)
                    {
                        evenimentSelectat = firmaSelectata.Evenimente.Find(x => x.Id == evenimenteUrmatoareDisplay[listViewEvenimenteUrmatoare.SelectedIndices[0]].Id);
                    }

                    PopuleazaInformatieEveniment(evenimentSelectat);

                    if (panelButoaneFirma.Enabled == false && panelButoaneFirma.Size != new Size(150, 216))
                    {
                        panelButoaneFirma.Enabled = true;
                        panelButoaneFirma.Size = new Size(150, 216);
                    }
                    
                    PopuleazaInformatieFirma(firmaSelectata);

                    labelFirmaSelectata.Text = firmaSelectata.Nume;

                    GestioneazaInterfataGeneral();
                    GestioneazaInterfataEvenimente();

                }
            }
        }

        #endregion

        #region IstoricEvenimente

        private void comboBoxEvenimenteIstoricAfisare_SelectedIndexChanged(object sender, EventArgs e)
        {
            evenimenteIstoric = Settings.Evenimente_Toate_Istoric();
            evenimenteIstoricDisplay = FiltreazaEvenimente(comboBoxEvenimenteIstoricAfisare.SelectedItem.ToString(), evenimenteIstoric);
            PopuleazaListaEvenimente(listViewEvenimenteIstoric, evenimenteIstoricDisplay, true);
        }

        private void listViewEvenimenteIstoric_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (listViewEvenimenteIstoric.FocusedItem.Bounds.Contains(e.Location))
                {
                    firmaSelectata = firme.Find(x => x.Nume == evenimenteIstoricDisplay[listViewEvenimenteIstoric.SelectedIndices[0]].Nume_Firma);

                    SwitchPanel(panelEvenimenteFirma);

                    evenimenteDisplay = FiltreazaEvenimenteFirma(comboBoxFirmaEvenimentAfisare.SelectedItem.ToString(), firmaSelectata.Evenimente);

                    if (evenimenteDisplay.Count > 0)
                    {
                        dateTimePickerFirmaEvenimenteInceput.Value = firmaSelectata.Evenimente[0].Data_Eveniment;
                        dateTimePickerFirmaEvenimenteSfarsit.Value = DateTime.Today;
                    }
                    else
                    {
                        dateTimePickerFirmaEvenimenteInceput.Value = DateTime.Today;
                        dateTimePickerFirmaEvenimenteSfarsit.Value = DateTime.Today;
                    }

                    PopuleazaListaEvenimenteFirma(listViewFirmaEvenimente, evenimenteDisplay);

                    if (firmaSelectata != null)
                    {
                        evenimentSelectat = firmaSelectata.Evenimente.Find(x => x.Id == evenimenteIstoricDisplay[listViewEvenimenteIstoric.SelectedIndices[0]].Id);
                    }

                    PopuleazaInformatieEveniment(evenimentSelectat);

                    if (panelButoaneFirma.Enabled == false && panelButoaneFirma.Size != new Size(150, 216))
                    {
                        panelButoaneFirma.Enabled = true;
                        panelButoaneFirma.Size = new Size(150, 216);
                    }

                    PopuleazaInformatieFirma(firmaSelectata);

                    labelFirmaSelectata.Text = firmaSelectata.Nume;

                    GestioneazaInterfataGeneral();
                    GestioneazaInterfataEvenimente();

                }
            }
        }

        #endregion

        #region Scadente

        private void dateTimePickerScadenteInceput_ValueChanged(object sender, EventArgs e)
        {
            listViewScadente.Items.Clear();

            facturiScadenteDisplay = FiltreazaFacturiScadente(comboBoxScadenteAfisare.SelectedItem.ToString(), DbaseConnection.DBFFacturi(Settings.Ani_InUz, dateTimePickerScadenteInceput.Value, dateTimePickerScadenteSfarsit.Value));

            PopuleazaListaFacturiScadente(listViewScadente, facturiScadenteDisplay);
        }

        private void dateTimePickerScadenteSfarsit_ValueChanged(object sender, EventArgs e)
        {
            listViewScadente.Items.Clear();

            facturiScadenteDisplay = FiltreazaFacturiScadente(comboBoxScadenteAfisare.SelectedItem.ToString(), DbaseConnection.DBFFacturi(Settings.Ani_InUz, dateTimePickerScadenteInceput.Value, dateTimePickerScadenteSfarsit.Value));

            PopuleazaListaFacturiScadente(listViewScadente, facturiScadenteDisplay);

        }

        private void comboBoxScadenteAfisare_SelectedIndexChanged(object sender, EventArgs e)
        {
            listViewScadente.Items.Clear();

            facturiScadenteDisplay = FiltreazaFacturiScadente(comboBoxScadenteAfisare.SelectedItem.ToString(), DbaseConnection.DBFFacturi(Settings.Ani_InUz, dateTimePickerScadenteInceput.Value, dateTimePickerScadenteSfarsit.Value));

            PopuleazaListaFacturiScadente(listViewScadente, facturiScadenteDisplay);
        }

        private void listViewScadente_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (listViewScadente.FocusedItem.Bounds.Contains(e.Location))
                {
                    try
                    {
                        firmaSelectata = firme.Find(x => x.CUI == facturiScadenteDisplay[listViewScadente.SelectedIndices[0]].CUI_Firma);

                        SwitchPanel(panelEvenimenteFirma);

                        firmaSelectata.FacturiClient = DbaseConnection.DBFCitesteFacturiClientFirma(firmaSelectata, Settings.Ani_InUz);

                        firmaSelectata.FacturiFurnizor = DbaseConnection.DBFCitesteFacturiFurnizorFirma(firmaSelectata, Settings.Ani_InUz);

                        firmaSelectata.Tranzactii = DbaseConnection.DBFCitesteTranzactiiFirma(firmaSelectata, Settings.Ani_InUz);

                        facturiClientDisplay = firmaSelectata.FacturiClient;

                        facturiFurnizorDisplay = firmaSelectata.FacturiFurnizor;

                        modDisplayFacturiClient = "Scadente";
                        GestioneazaInterfataFacturiClient();

                        SwitchPanel(panelFacturiFirma);

                        if (facturiClientDisplay.Count > 0)
                        {
                            dateTimePickerClientInceput.Value = firmaSelectata.FacturiClient[0].Data_Emitere;
                            dateTimePickerClientSfarsit.Value = firmaSelectata.FacturiClient[firmaSelectata.FacturiClient.Count - 1].Data_Scadenta;
                        }

                        if (facturiFurnizorDisplay.Count > 0)
                        {
                            dateTimePickerFurnizorInceput.Value = firmaSelectata.FacturiFurnizor[0].Data_Emitere;
                            dateTimePickerFurnizorSfarsit.Value = firmaSelectata.FacturiFurnizor[firmaSelectata.FacturiFurnizor.Count - 1].Data_Scadenta;
                        }

                        listViewProduseFacClient.Items.Clear();
                        listViewProduseFacFurnizor.Items.Clear();

                        PopuleazaListaFacturi(listViewFacturiClient, facturiClientDisplay, textBoxFacturiClientPerioada, labelNumarFacturiClient);
                        PopuleazaListaFacturi(listViewFacturiFurnizor, facturiFurnizorDisplay, textBoxFacturiFurnizorPerioada, labelNumarFacturiFurnizor);

                        if (firmaSelectata != null)
                        {
                            if (facturiScadenteDisplay[listViewScadente.SelectedIndices[0]].Tip_Factura == "Client")
                            {
                                facturaClientSelectata = firmaSelectata.FacturiClient.Find(x => x.Seria == facturiScadenteDisplay[listViewScadente.SelectedIndices[0]].Seria &&
                                                                                                x.Numar_Document == facturiScadenteDisplay[listViewScadente.SelectedIndices[0]].Numar_Document);

                                tabControlClientFurnizor.SelectTab(0);
                                PopuleazaInformatieFacturaClient(facturaClientSelectata);
                            }
                            else if (facturiScadenteDisplay[listViewScadente.SelectedIndices[0]].Tip_Factura == "Furnizor")
                            {
                                facturaFurnizorSelectata = firmaSelectata.FacturiFurnizor.Find(x => x.Seria == facturiScadenteDisplay[listViewScadente.SelectedIndices[0]].Seria &&
                                                                                                x.Numar_Document == facturiScadenteDisplay[listViewScadente.SelectedIndices[0]].Numar_Document &&
                                                                                                x.Valoare == facturiScadenteDisplay[listViewScadente.SelectedIndices[0]].Valoare);

                                tabControlClientFurnizor.SelectTab(1);
                                PopuleazaInformatieFacturaFurnizor(facturaFurnizorSelectata);
                            }
                        }

                        if (panelButoaneFirma.Enabled == false && panelButoaneFirma.Size != new Size(150, 216))
                        {
                            panelButoaneFirma.Enabled = true;
                            panelButoaneFirma.Size = new Size(150, 216);
                        }

                        PopuleazaInformatieFirma(firmaSelectata);

                        labelFirmaSelectata.Text = firmaSelectata.Nume;

                        GestioneazaInterfataGeneral();

                    }
                    catch
                    {
                        MessageBox.Show("Firma cautata nu poate fi gasita.");
                    }
                }
            }
        }

        #endregion

        #region Tranzactii

        private void dateTimePickerTranzactiiInceput_ValueChanged(object sender, EventArgs e)
        {
            listViewTranzactii.Items.Clear();

            tranzactiiDisplay = FiltreazaTranzactii(comboBoxFirmaTranzactiiAfisare.SelectedItem.ToString(), firmaSelectata.TranzactiiPePerioada(dateTimePickerTranzactiiInceput.Value, dateTimePickerTranzactiiSfarsit.Value));

            PopuleazaListaTranzactii(listViewTranzactii, tranzactiiDisplay, textBoxTotalTranzactii);
        }

        private void dateTimePickerTranzactiiSfarsit_ValueChanged(object sender, EventArgs e)
        {
            listViewTranzactii.Items.Clear();

            tranzactiiDisplay = FiltreazaTranzactii(comboBoxFirmaTranzactiiAfisare.SelectedItem.ToString(), firmaSelectata.TranzactiiPePerioada(dateTimePickerTranzactiiInceput.Value, dateTimePickerTranzactiiSfarsit.Value));

            PopuleazaListaTranzactii(listViewTranzactii, tranzactiiDisplay, textBoxTotalTranzactii);
        }

        private void comboBoxFirmaTranzactiiAfisare_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                tranzactiiDisplay = FiltreazaTranzactii(comboBoxFirmaTranzactiiAfisare.SelectedItem.ToString(), firmaSelectata.Tranzactii);

                if (tranzactiiDisplay.Count > 0)
                {
                    if (tranzactiiDisplay[0].Data_Tranzactie >= dateTimePickerTranzactiiInceput.MinDate)
                    {
                        dateTimePickerTranzactiiInceput.Value = tranzactiiDisplay[0].Data_Tranzactie;
                        dateTimePickerTranzactiiSfarsit.Value = DateTime.Today;
                    }
                }
                else
                {
                    dateTimePickerTranzactiiInceput.Value = DateTime.Today;
                    dateTimePickerTranzactiiSfarsit.Value = DateTime.Today;
                }

                //O CAUTARE DUPA SERIE SI NUMAR - CAUT IN DBF - DUPA ALA CAUT IN LISTA DE FIRME

                PopuleazaListaTranzactii(listViewTranzactii, tranzactiiDisplay, textBoxTotalTranzactii);
            }
        }

        #endregion

      
        #region Persoane

        private void tabControlPersoaneConturi_SelectedIndexChanged(object sender, EventArgs e)
        {
            persoanaSelectata = null;

            GestioneazaInterfataPersoaneContact();
            ReseteazaInfoPersoana();


            //la fel conturile bancare
        }

        private void buttonPersoanaCreare_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (persoanaSelectata == null) //NU ESTE O PERSOANA SELECTATA => CREARE
                {
                    if (InfoPersoaneOK())
                    {
                        Persoana p = new Persoana();
                        p.Id = 0;
                        p.Nume_Prenume = textBoxPersoanaNume.Text;
                        p.Telefon = textBoxPersoanaTelefon.Text;
                        p.Email = textBoxPersoanaEmail.Text;

                        firmaSelectata.SalveazaPersoana(p, -1);

                        ReseteazaInfoPersoana();

                        PopuleazaListaPersoaneDeContact(listViewPersoane, firmaSelectata.Persoane_De_Contact);
                    }
                    else
                    {
                        MessageBox.Show("Gresit");
                        return;
                    }
                }
                else if (persoanaSelectata != null) //E SELECTATA O PERSOANA
                {
                    if(!_modModificarePersoana) //SELECTAT DAR NU IN MODUL DE MODIFICARE
                    {
                        _modModificarePersoana = true; //APASAND BUTONUL IL FAC SA FIE MOD MODIFICARE

                        GestioneazaInterfataPersoaneContact();

                        return;
                    }

                    else if(_modModificarePersoana) // SELECTAT SI IN MODUL DE MODIFICARE
                    {
                        if (InfoPersoaneOK())
                        {
                            Persoana p = new Persoana();
                            p.Id = 0;
                            p.Nume_Prenume = textBoxPersoanaNume.Text;
                            p.Telefon = textBoxPersoanaTelefon.Text;
                            p.Email = textBoxPersoanaEmail.Text;

                            firmaSelectata.SalveazaPersoana(p, persoanaSelectata.Id);

                            ReseteazaInfoPersoana();
                            
                            PopuleazaListaPersoaneDeContact(listViewPersoane, firmaSelectata.Persoane_De_Contact);

                            _modModificarePersoana = false; //AM TERMINAT, AM CONFIRMAT
                        }
                        else
                        {
                            MessageBox.Show("Gresit");
                            return;
                        }

                        GestioneazaInterfataPersoaneContact();

                        return;
                    }
                }
            }
        }

        private void listViewPersoane_SelectedIndexChanged(object sender, EventArgs e)
        {
            persoanaSelectata = null;

            if (listViewPersoane.SelectedItems.Count > 0)
            {
                persoanaSelectata = firmaSelectata.Persoane_De_Contact[listViewPersoane.SelectedIndices[0]];

                PopuleazaInformatiePersoanaContact(persoanaSelectata);

                GestioneazaInterfataPersoaneContact();

                return;//nu il las sa treaca mai departe daca e selectata o factura pentru a nu reseta lista de produse

            }

            _modModificarePersoana = false;
            GestioneazaInterfataPersoaneContact();
            ReseteazaInfoPersoana();
        }

        private void buttonPersoanaStergere_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (persoanaSelectata != null)
                {
                    DialogResult dr;

                    dr = MessageBox.Show("Sigur doriti sa stergeti aceasta persoana de contact?", "Stergere", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        Directory.Delete(firmaSelectata.CalePersoanaContact(persoanaSelectata.Id), true);

                        persoanaSelectata = null;

                        PopuleazaListaPersoaneDeContact(listViewPersoane, firmaSelectata.Persoane_De_Contact);

                        GestioneazaInterfataPersoaneContact();
                        ReseteazaInfoPersoana();
                    }
                }
            }
        }

        #endregion

        #region ConturiBancare

        private void buttonContCreare_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                ContBancar p = new ContBancar();
                p.Id = 0;
                p.IBAN = textBoxContIBAN.Text;
                p.Banca = textBoxContBanca.Text;
                p.Moneda_Cont = textBoxContMoneda.Text;

                firmaSelectata.SalveazaContBancar(p, -1);
            }
        }

        #endregion

        #region FacturiClient

        private void listViewFacturiClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            facturaClientSelectata = null;

            if (listViewFacturiClient.SelectedItems.Count > 0)
            {
                if (facturiClientDisplay.Count > 0)
                {
                    facturaClientSelectata = facturiClientDisplay[listViewFacturiClient.SelectedIndices[0]];

                    PopuleazaInformatieFacturaClient(facturaClientSelectata);

                    return;//nu il las sa treaca mai departe daca e selectata o factura pentru a nu reseta lista de produse
                }
            }

            listViewProduseFacClient.Items.Clear();
        }

        private void dateTimePickerClientInceput_ValueChanged(object sender, EventArgs e)
        {
            listViewProduseFacClient.Items.Clear();

            if (modDisplayFacturiClient == "Normal")
            {
                facturiClientDisplay = firmaSelectata.FacturiClientPePerioada(dateTimePickerClientInceput.Value, dateTimePickerClientSfarsit.Value);

                PopuleazaListaFacturi(listViewFacturiClient, facturiClientDisplay, textBoxFacturiClientPerioada, labelNumarFacturiClient);
            }
            else if(modDisplayFacturiClient == "Scadente")
            {
                facturiClientDisplay = firmaSelectata.FacturiClientScadenteInPerioada(dateTimePickerClientInceput.Value, dateTimePickerClientSfarsit.Value);

                PopuleazaListaFacturi(listViewFacturiClient, facturiClientDisplay, textBoxFacturiClientPerioada, labelNumarFacturiClient);
            }
        }

        private void dateTimePickerClientSfarsit_ValueChanged(object sender, EventArgs e)
        {
            listViewProduseFacClient.Items.Clear();

            if (modDisplayFacturiClient == "Normal")
            {
                facturiClientDisplay = firmaSelectata.FacturiClientPePerioada(dateTimePickerClientInceput.Value, dateTimePickerClientSfarsit.Value);

                PopuleazaListaFacturi(listViewFacturiClient, facturiClientDisplay, textBoxFacturiClientPerioada, labelNumarFacturiClient);
            }
            else if (modDisplayFacturiClient == "Scadente")
            {
                facturiClientDisplay = firmaSelectata.FacturiClientScadenteInPerioada(dateTimePickerClientInceput.Value, dateTimePickerClientSfarsit.Value);

                PopuleazaListaFacturi(listViewFacturiClient, facturiClientDisplay, textBoxFacturiClientPerioada, labelNumarFacturiClient);
            }
        }

        private void buttonFacturiClientTiparire_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (facturaClientSelectata != null)
                {
                    PrintHandling.i = 0;
                    PrintHandling.j = 0;
                    PrintHandling.p = 1;
                    PrintHandling.k = 0;

                    System.Drawing.Printing.PrintDocument tiparesteFactura = new System.Drawing.Printing.PrintDocument();
                    tiparesteFactura.PrintPage += tiparesteFacturaClient_PrintPage;

                    // printPreviewDialog1.Document = p;
                    // printPreviewDialog1.ShowDialog();
                    tiparesteFactura.Print();
                }
            }
        }

        private void buttonFacturiClientActiuni_Click(object sender, EventArgs e)
        {
            contextMenuStripFacturiClientActiuni.Show(Cursor.Position);            
        }
        
        private void tiparesteFacturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (facturaClientSelectata != null)
                {
                    PrintHandling.i = 0;
                    PrintHandling.j = 0;
                    PrintHandling.p = 1;
                    PrintHandling.k = 0;

                    System.Drawing.Printing.PrintDocument tiparesteFactura = new System.Drawing.Printing.PrintDocument();
                    tiparesteFactura.PrintPage += tiparesteFacturaClient_PrintPage;

                    // printPreviewDialog1.Document = p;
                    // printPreviewDialog1.ShowDialog();
                    tiparesteFactura.Print();
                }
            }
        }

        private void tiparesteFacturaClient_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            PrintHandling.PrintFacturaClient(e, firmaSelectata, facturaClientSelectata);
        }
        
        private void graficRulajPlatiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                DbaseConnection.DBFCitesteFacturiClientFirma(firmaSelectata, Settings.Ani_InUz);
                DbaseConnection.DBFCitesteTranzactiiFirma(firmaSelectata, Settings.Ani_InUz);

                Form gform = new graficForm();

                gform.Text = firmaSelectata.Nume + " - Grafic rulaj-incasari";

                gform.Size = new Size(1200, 800);

                List<decimal> facturiLuni = firmaSelectata.RulajClientLuniDinPerioada(dateTimePickerClientInceput.Value, dateTimePickerClientSfarsit.Value);

                //  for(int i = 1; i <= 12; i++)
                //  {
                //      facturiLuni.Add(firmaSelectata.RulajClientLuna(i, 2018));
                //  }

                List<decimal> incasariLuni = firmaSelectata.IncasariClientLuniDinPerioada(dateTimePickerClientInceput.Value, dateTimePickerClientSfarsit.Value);

                // for(int i = 1; i <= 12; i++)
                //  {
                //      incasariLuni.Add(firmaSelectata.IncasariLuna(i, 2018));
                //  }

                gform.Show();

                Graphics g = gform.CreateGraphics();

                GraphicLibrary.GraficLinie(g, 70, 20, 1000, 200, facturiLuni, GraphicLibrary.LuniPrescurtatePerioada(dateTimePickerClientInceput.Value, dateTimePickerClientSfarsit.Value), Color.Red, false, "Incasari", "Luni", true);
                GraphicLibrary.GraficLinie(g, 70, 20, 1000, 200, incasariLuni, GraphicLibrary.LuniPrescurtatePerioada(dateTimePickerClientInceput.Value, dateTimePickerClientSfarsit.Value), Color.Blue, false, "Incasari", "Luni", false);

                g.DrawString("Rulaj client", new Font("Calibri", 11, FontStyle.Italic), Brushes.Red, new Point(980, 20));
                g.DrawString("Incasari", new Font("Calibri", 11, FontStyle.Italic), Brushes.Blue, new Point(980, 40));
            }
        }

        private void tiparesteGraficRulajPlatiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                PrintHandling.i = 0;
                PrintHandling.j = 0;
                PrintHandling.p = 1;
                PrintHandling.k = 0;

                System.Drawing.Printing.PrintDocument printGrafic = new System.Drawing.Printing.PrintDocument();
                printGrafic.PrintPage += printGraficClient_PrintPage;

                // printPreviewDialog1.Document = p;
                // printPreviewDialog1.ShowDialog();
                printGrafic.Print();
            }
        }

        private void printGraficClient_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (firmaSelectata != null)
            {
                PrintHandling.PrintSituatieRulajIncasariClient(e, firmaSelectata, dateTimePickerClientInceput.Value, dateTimePickerClientSfarsit.Value);
            }
        }

        private void neplatiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                facturiClientDisplay = firmaSelectata.FacturiClientNeplatite(new DateTime(2018, 1, 1), DateTime.Now);

                modDisplayFacturiClient = "Neplatite";
                GestioneazaInterfataFacturiClient();

                facturaClientSelectata = null;
                listViewProduseFacClient.Items.Clear();
                listViewProduseFacFurnizor.Items.Clear();

                PopuleazaListaFacturi(listViewFacturiClient, facturiClientDisplay, textBoxFacturiClientPerioada, labelNumarFacturiClient);
            }
        }

        private void platiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                facturiClientDisplay = firmaSelectata.FacturiClientPlatite(new DateTime(2018, 1, 1), DateTime.Now);

                modDisplayFacturiClient = "Platite";
                GestioneazaInterfataFacturiClient();

                facturaClientSelectata = null;
                listViewProduseFacClient.Items.Clear();
                listViewProduseFacFurnizor.Items.Clear();

                PopuleazaListaFacturi(listViewFacturiClient, facturiClientDisplay, textBoxFacturiClientPerioada, labelNumarFacturiClient);
            }
        }

        private void scadenteInPerioadaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                facturiClientDisplay = firmaSelectata.FacturiClientScadenteInPerioada(dateTimePickerClientInceput.Value, dateTimePickerClientSfarsit.Value);

                modDisplayFacturiClient = "Scadente";
                GestioneazaInterfataFacturiClient();

                facturaClientSelectata = null;
                listViewProduseFacClient.Items.Clear();
                listViewProduseFacFurnizor.Items.Clear();

                PopuleazaListaFacturi(listViewFacturiClient, facturiClientDisplay, textBoxFacturiClientPerioada, labelNumarFacturiClient);
            }
        }

        private void toateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                facturiClientDisplay = firmaSelectata.FacturiClient;

                if (facturiClientDisplay.Count > 0)
                {

                    dateTimePickerClientInceput.Value = facturiClientDisplay[0].Data_Emitere;
                    dateTimePickerClientSfarsit.Value = DateTime.Today;
                }

                if (facturiFurnizorDisplay.Count > 0)
                {

                    dateTimePickerFurnizorInceput.Value = facturiFurnizorDisplay[0].Data_Emitere;
                    dateTimePickerFurnizorSfarsit.Value = DateTime.Today;
                }

                facturaClientSelectata = null;
                listViewProduseFacClient.Items.Clear();
                listViewProduseFacFurnizor.Items.Clear();

                modDisplayFacturiClient = "Normal";
                GestioneazaInterfataFacturiClient();

                PopuleazaListaFacturi(listViewFacturiClient, facturiClientDisplay, textBoxFacturiClientPerioada, labelNumarFacturiClient);
            }
        }


        #endregion

        #region FacturiFurnizor

        private void listViewFacturiFurnizor_SelectedIndexChanged(object sender, EventArgs e)
        {
            facturaFurnizorSelectata = null;

            if (listViewFacturiFurnizor.SelectedItems.Count > 0)
            {
                if (facturiFurnizorDisplay.Count > 0)
                {
                    facturaFurnizorSelectata = facturiFurnizorDisplay[listViewFacturiFurnizor.SelectedIndices[0]];

                    PopuleazaInformatieFacturaFurnizor(facturaFurnizorSelectata);

                    return;//nu il las sa treaca mai departe daca e selectata o factura pentru a nu reseta lista de produse
                }
            }

            listViewProduseFacFurnizor.Items.Clear();
        }

        private void tiparesteFacturaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (facturaFurnizorSelectata != null)
                {
                    PrintHandling.i = 0;
                    PrintHandling.j = 0;
                    PrintHandling.p = 1;
                    PrintHandling.k = 0;

                    System.Drawing.Printing.PrintDocument tiparesteFactura = new System.Drawing.Printing.PrintDocument();
                    tiparesteFactura.PrintPage += tiparesteFacturaFurnizor_PrintPage;

                    // printPreviewDialog1.Document = p;
                    // printPreviewDialog1.ShowDialog();
                    tiparesteFactura.Print();
                }
            }
        }

        private void tiparesteFacturaFurnizor_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            PrintHandling.PrintFacturaFurnizor(e, firmaSelectata, facturaFurnizorSelectata);
        }

        private void graficRulajPlatiToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                DbaseConnection.DBFCitesteFacturiFurnizorFirma(firmaSelectata, Settings.Ani_InUz);
                DbaseConnection.DBFCitesteTranzactiiFirma(firmaSelectata, Settings.Ani_InUz);

                Form gform = new graficForm();

                gform.Text = firmaSelectata.Nume + " - Grafic rulaj-plati";

                gform.Size = new Size(1200, 800);

                List<decimal> facturiLuni = firmaSelectata.RulajFurnizorLuniDinPerioada(dateTimePickerFurnizorInceput.Value, dateTimePickerFurnizorSfarsit.Value);

                //  for(int i = 1; i <= 12; i++)
                //  {
                //      facturiLuni.Add(firmaSelectata.RulajClientLuna(i, 2018));
                //  }

                List<decimal> platiLuni = firmaSelectata.PlatiFurnizorLuniDinPerioada(dateTimePickerFurnizorInceput.Value, dateTimePickerFurnizorSfarsit.Value);

                // for(int i = 1; i <= 12; i++)
                //  {
                //      incasariLuni.Add(firmaSelectata.IncasariLuna(i, 2018));
                //  }

                gform.Show();

                Graphics g = gform.CreateGraphics();

                GraphicLibrary.GraficLinie(g, 70, 20, 1000, 200, facturiLuni, GraphicLibrary.LuniPrescurtatePerioada(dateTimePickerFurnizorInceput.Value, dateTimePickerFurnizorSfarsit.Value), Color.Red, false, "Plati", "Luni", true);
                GraphicLibrary.GraficLinie(g, 70, 20, 1000, 200, platiLuni, GraphicLibrary.LuniPrescurtatePerioada(dateTimePickerFurnizorInceput.Value, dateTimePickerFurnizorSfarsit.Value), Color.Blue, false, "Plati", "Luni", false);

                g.DrawString("Rulaj furnizor", new Font("Calibri", 11, FontStyle.Italic), Brushes.Red, new Point(980, 20));
                g.DrawString("Plati", new Font("Calibri", 11, FontStyle.Italic), Brushes.Blue, new Point(980, 40));
            }
        }

        private void tiparesteGraficRulajPlatiToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                PrintHandling.i = 0;
                PrintHandling.j = 0;
                PrintHandling.p = 1;
                PrintHandling.k = 0;

                System.Drawing.Printing.PrintDocument printGrafic = new System.Drawing.Printing.PrintDocument();
                printGrafic.PrintPage += printGraficFurnizor_PrintPage;

                // printPreviewDialog1.Document = p;
                // printPreviewDialog1.ShowDialog();
                printGrafic.Print();
            }
        }

        private void printGraficFurnizor_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (firmaSelectata != null)
            {
                PrintHandling.PrintSituatieRulajPlatiFurnizor(e, firmaSelectata, dateTimePickerFurnizorInceput.Value, dateTimePickerFurnizorSfarsit.Value);
            }
        }

        private void toateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                facturiFurnizorDisplay = firmaSelectata.FacturiFurnizor;

                if (facturiClientDisplay.Count > 0)
                {
                    dateTimePickerClientInceput.Value = facturiClientDisplay[0].Data_Emitere;
                    dateTimePickerClientSfarsit.Value = DateTime.Today;
                }

                if (facturiFurnizorDisplay.Count > 0)
                {

                    dateTimePickerFurnizorInceput.Value = facturiFurnizorDisplay[0].Data_Emitere;
                    dateTimePickerFurnizorSfarsit.Value = DateTime.Today;
                }

                facturaFurnizorSelectata = null;
                listViewProduseFacClient.Items.Clear();
                listViewProduseFacFurnizor.Items.Clear();

                modDisplayFacturiFurnizor = "Normal";
                GestioneazaInterfataFacturiFurnizor();

                PopuleazaListaFacturi(listViewFacturiFurnizor, facturiFurnizorDisplay, textBoxFacturiFurnizorPerioada, labelNumarFacturiFurnizor);
            }
        }

        private void neplatiteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                facturiFurnizorDisplay = firmaSelectata.FacturiFurnizorNeplatite(new DateTime(2018, 1, 1), DateTime.Now);

                modDisplayFacturiFurnizor = "Neplatite";
                GestioneazaInterfataFacturiFurnizor();

                facturaFurnizorSelectata = null;
                listViewProduseFacClient.Items.Clear();
                listViewProduseFacFurnizor.Items.Clear();

                PopuleazaListaFacturi(listViewFacturiFurnizor, facturiFurnizorDisplay, textBoxFacturiFurnizorPerioada, labelNumarFacturiFurnizor);
            }
        }

        private void platiteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                facturiFurnizorDisplay = firmaSelectata.FacturiFurnizorPlatite(new DateTime(2018, 1, 1), DateTime.Now);

                modDisplayFacturiFurnizor = "Platite";
                GestioneazaInterfataFacturiFurnizor();

                facturaFurnizorSelectata = null;
                listViewProduseFacClient.Items.Clear();
                listViewProduseFacFurnizor.Items.Clear();

                PopuleazaListaFacturi(listViewFacturiFurnizor, facturiFurnizorDisplay, textBoxFacturiFurnizorPerioada, labelNumarFacturiFurnizor);
            }
        }

        private void scadenteInPerioadaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                facturiFurnizorDisplay = firmaSelectata.FacturiFurnizorScadenteInPerioada(dateTimePickerFurnizorInceput.Value, dateTimePickerFurnizorSfarsit.Value);

                modDisplayFacturiFurnizor = "Scadente";
                GestioneazaInterfataFacturiFurnizor();

                facturaFurnizorSelectata = null;
                listViewProduseFacClient.Items.Clear();
                listViewProduseFacFurnizor.Items.Clear();

                PopuleazaListaFacturi(listViewFacturiFurnizor, facturiFurnizorDisplay, textBoxFacturiFurnizorPerioada, labelNumarFacturiFurnizor);
            }
        }

        private void dateTimePickerFurnizorInceput_ValueChanged(object sender, EventArgs e)
        {
            listViewProduseFacFurnizor.Items.Clear();

            if (modDisplayFacturiFurnizor == "Normal")
            {
                facturiFurnizorDisplay = firmaSelectata.FacturiFurnizorPePerioada(dateTimePickerFurnizorInceput.Value, dateTimePickerFurnizorSfarsit.Value);

                PopuleazaListaFacturi(listViewFacturiFurnizor, facturiFurnizorDisplay, textBoxFacturiFurnizorPerioada, labelNumarFacturiFurnizor);
            }
            else if (modDisplayFacturiFurnizor == "Scadente")
            {
                facturiFurnizorDisplay = firmaSelectata.FacturiFurnizorScadenteInPerioada(dateTimePickerFurnizorInceput.Value, dateTimePickerFurnizorSfarsit.Value);

                PopuleazaListaFacturi(listViewFacturiFurnizor, facturiFurnizorDisplay, textBoxFacturiFurnizorPerioada, labelNumarFacturiFurnizor);
            }
        }

        private void dateTimePickerFurnizorSfarsit_ValueChanged(object sender, EventArgs e)
        {
            listViewProduseFacFurnizor.Items.Clear();

            if (modDisplayFacturiFurnizor == "Normal")
            {
                facturiFurnizorDisplay = firmaSelectata.FacturiFurnizorPePerioada(dateTimePickerFurnizorInceput.Value, dateTimePickerFurnizorSfarsit.Value);

                PopuleazaListaFacturi(listViewFacturiFurnizor, facturiFurnizorDisplay, textBoxFacturiFurnizorPerioada, labelNumarFacturiFurnizor);
            }
            else if (modDisplayFacturiFurnizor == "Scadente")
            {
                facturiFurnizorDisplay = firmaSelectata.FacturiFurnizorScadenteInPerioada(dateTimePickerFurnizorInceput.Value, dateTimePickerFurnizorSfarsit.Value);

                PopuleazaListaFacturi(listViewFacturiFurnizor, facturiFurnizorDisplay, textBoxFacturiFurnizorPerioada, labelNumarFacturiFurnizor);
            }
        }

        private void buttonFacturiFurnizorActiuni_Click(object sender, EventArgs e)
        {
            contextMenuStripFacturiFurnizorActiuni.Show(Cursor.Position);
        }

        #endregion

        #region WebsiteUtile

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.listafirme.ro");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.mfinante.gov.ro/agenticod.html?pagina=domenii");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.risco.ro/");
        }

        #endregion

        #region Setari

        private void comboBoxSetariIntervalActualizare_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Interval_Actualizare = int.Parse(comboBoxSetariIntervalActualizare.Text);
            
            AfiseazaTimpActualizare(Settings.Ultima_Actualizare_DBF, Settings.Urmatoare_Actualizare_DBF);

            Settings.ScrieSetari();

            Thread t = new Thread(() => Settings.Actualizeaza_DBF(Settings.Ani_InUz));
            t.Start();

            Thread a = new Thread(() => MessageBox.Show("Se actualizeaza baza de date... \n\r Aceasta operatiune poate dura cateva secunde"));
            a.Start();
            
        }

        #endregion

        #region Print

        private void buttonTiparesteComanda_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                PrintHandling.i = 0;
                PrintHandling.j = 0;
                PrintHandling.p = 1;
                PrintHandling.k = 0;

                System.Drawing.Printing.PrintDocument printCom = new System.Drawing.Printing.PrintDocument();
                printCom.PrintPage += printCom_PrintPage;
             //   printCom.PrinterSettings.PrinterName = PrinterSettings.InstalledPrinters
         //   for(int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
           //     {
            //        MessageBox.Show(PrinterSettings.InstalledPrinters[i]);
            //    }
                // printPreviewDialog1.Document = p;
                // printPreviewDialog1.ShowDialog();
                printCom.Print();
            }
        }

        private void printCom_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (comandaSelectata != null)
                {
                    PrintHandling.PrintComanda(e, firmaSelectata, comandaSelectata);
                }
            }
        }

        private void buttonTiparesteSituatie_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                PrintHandling.i = 0;
                PrintHandling.j = 0;
                PrintHandling.p = 1;
                PrintHandling.k = 0;

                System.Drawing.Printing.PrintDocument printSit = new System.Drawing.Printing.PrintDocument();
                printSit.PrintPage += printSit_PrintPage;

                // printPreviewDialog1.Document = p;
                // printPreviewDialog1.ShowDialog();
                printSit.Print();
            }
        }

        private void printSit_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (firmaSelectata != null)
            {
                List<decimal> valori1 = new List<decimal>();
                List<decimal> valori2 = new List<decimal>();

                for (int i = 1; i <= 8; i++)
                {
                    valori1.Add(firmaSelectata.RulajClientLuna(i, 2018));
                }
                for (int i = 1; i <= 12; i++)
                {
                    valori2.Add(firmaSelectata.RulajClientLuna(i, 2018));
                }
                
                PrintHandling.PrintListaFacturiSiIncasariPerioada(e, firmaSelectata, new DateTime(2018, 1, 1), new DateTime(2018, 8, 31));
              //  GraphicLibrary.GraficLinie(e.Graphics, 100, 700, 300, 300, valori1, Color.Red, "Total", "Luni");
               // GraphicLibrary.GraficFelieDeTort(e.Graphics);
            }
        }

        #endregion

        #region firmaGeneral

        private void firmaDeschidetoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (fisiereDisplay != null && fisiereDisplay.Count > 0)
                {
                    try
                    {
                        Process.Start(fisiereDisplay[listViewFirmaDocumente.FocusedItem.Index]);
                    }
                    catch
                    {
                        MessageBox.Show("Nu s-a putut deschide fisierul.");
                    }
                }
            }
        }

        private void firmaStergetoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (fisiereDisplay != null && fisiereDisplay.Count > 0)
                {
                    DialogResult dr;

                    dr = MessageBox.Show("Sigur doriti sa stergeti acest fisier?", "Stergere", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        File.Delete(fisiereDisplay[listViewFirmaDocumente.FocusedItem.Index]);

                        fisiereDisplay.RemoveAt(listViewFirmaDocumente.FocusedItem.Index);

                        listViewFirmaDocumente.Items.Clear();

                        foreach (string s in fisiereDisplay)
                            listViewFirmaDocumente.Items.Add(Path.GetFileName(s));
                    }
                }
            }
        }

        private void buttonFirmaAdaugareDocument_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                string[] fileNames = null;

                using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
                {
                    openFileDialog1.InitialDirectory = "c:\\";
                    openFileDialog1.Multiselect = true;

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        fileNames = openFileDialog1.FileNames;
                    }
                };

                if (fileNames != null)
                {
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        listViewFirmaDocumente.Items.Add(Path.GetFileName(fileNames[i]));
                        fisiereDisplay.Add(fileNames[i]);
                    }

                    if (fisiereDisplay != null)
                        foreach (string s in fisiereDisplay)
                        {
                            FileHandling.CopiazaFisier(s, firmaSelectata.CaleDocumente);
                        }
                }

                if (firmaSelectata.Cai_Documente != null)
                {
                    fisiereDisplay = firmaSelectata.Cai_Documente;

                    listViewFirmaDocumente.Items.Clear();
                    foreach (string s in fisiereDisplay)
                        listViewFirmaDocumente.Items.Add(Path.GetFileName(s));
                }
            }
        }
        

        #endregion

        private void buttonAdaugareCale_Click(object sender, EventArgs e)
        {
            string cale = "";

            using (FolderBrowserDialog fb = new FolderBrowserDialog())
            {
                if (fb.ShowDialog() == DialogResult.OK)
                {
                    cale = fb.SelectedPath;

                    if(!Settings.CaiFoldereDBF.ContainsKey(comboBoxSetariAni.Text) && !Settings.CaiFoldereDBF.ContainsValue(cale))
                    {
                        try
                        {
                            Settings.CaiFoldereDBF.Add(comboBoxSetariAni.Text + "s", cale);
                            
                            Settings.CaiFoldereDBF.Add(comboBoxSetariAni.Text, Settings.CaleProgram + @"\" + "dbf" + comboBoxSetariAni.Text);
                            
                            listViewFoldereDBF.Items.Clear();

                            for (int i = 0; i < Settings.CaiFoldereDBF.Count; i++)
                            {
                                ListViewItem l = new ListViewItem();

                                l.Text = Settings.CaiFoldereDBF.ToList()[i].Key;
                                l.SubItems.Add(Settings.CaiFoldereDBF[Settings.CaiFoldereDBF.ToList()[i].Key]);

                                listViewFoldereDBF.Items.Add(l);

                            }
                            Settings.ScrieSetari();
                        }
                        catch
                        {
                            MessageBox.Show("Nu s-a putut adauga calea.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nu s-a putut adauga calea pentru anul selectat.");
                    }
                }
            };

            Settings.Actualizeaza_DBF(Settings.Ani_InUz);

        }
        

        private void buttonAdaugareFisiereComanda_Click(object sender, EventArgs e)
        {
            string[] fileNames = null;

            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Multiselect = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    fileNames = openFileDialog1.FileNames;
                }
            };

            if (fileNames != null)
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    listViewGraficaComanda.Items.Add(Path.GetFileName(fileNames[i]));
                    fisiereSelectateComanda.Add(fileNames[i]);
                }
            }
        }

        private void buttonStergereCale_Click(object sender, EventArgs e)
        {
            DialogResult dr;

            dr = MessageBox.Show("Sigur doriti sa stergeti aceasta cale?", "Stergere", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                Settings.CaiFoldereDBF.Remove(cheieCaleSelectata);

                listViewFoldereDBF.Items.RemoveAt(listViewFoldereDBF.SelectedIndices[0]);

                Settings.ScrieSetari();
            }


        }

        private void listViewFoldereDBF_SelectedIndexChanged(object sender, EventArgs e)
        {
            cheieCaleSelectata = null;

            if (listViewFoldereDBF.SelectedItems.Count > 0)
            {
                if (Settings.CaiFoldereDBF.Count > 0)
                {
                    cheieCaleSelectata = listViewFoldereDBF.SelectedItems[0].Text;

                    //cheieCaleSelectata = Settings.CaiFoldereDBF[listViewFoldereDBF.SelectedItems[0].Text];
                    
                    return;

                }
            }
        }

        private void listViewComenziFirma_SelectedIndexChanged(object sender, EventArgs e)
        {
            comandaSelectata = null;

            ReseteazaInfoComanda();

            if (listViewComenziFirma.SelectedItems.Count > 0)
            {
                if (comenziDisplay.Count > 0)
                {
                    comandaSelectata = comenziDisplay[listViewComenziFirma.SelectedIndices[0]];
                    
                    PopuleazaInformatieComanda(comandaSelectata);
                    _modModificareComanda = false;
                    GestioneazaInterfataComenzi();
                    

                    return;//nu il las sa treaca mai departe daca e selectata o factura pentru a nu reseta lista de produse
                }
            }

            _modModificareComanda = false;
            GestioneazaInterfataComenzi();
            ReseteazaInfoComanda();
        }

        private void listViewGraficaComanda_MouseClick(object sender, MouseEventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (fisiereSelectateComanda != null && fisiereSelectateComanda.Count > 0)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        if (listViewGraficaComanda.FocusedItem.Bounds.Contains(e.Location))
                        {
                            contextMenuStripComenziDocumente.Show(Cursor.Position);
                        }
                    }
                }
            }
        }

        private void listViewGraficaComanda_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (fisiereSelectateComanda != null && fisiereSelectateComanda.Count > 0)
                {
                    if (listViewGraficaComanda.FocusedItem.Bounds.Contains(e.Location))
                    {
                        try
                        {
                            Process.Start(fisiereSelectateComanda[listViewGraficaComanda.FocusedItem.Index]);
                        }
                        catch
                        {
                            MessageBox.Show("Nu s-a putut deschide fisierul.");
                        }
                    }
                }

            }
        }

        private void toolStripMenuItemComandaStergere_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (fisiereSelectateComanda != null && fisiereSelectateComanda.Count > 0)
                {
                    DialogResult dr;

                    dr = MessageBox.Show("Sigur doriti sa stergeti acest fisier?", "Stergere", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        File.Delete(fisiereSelectateComanda[listViewGraficaComanda.FocusedItem.Index]);

                        fisiereSelectateComanda.RemoveAt(listViewGraficaComanda.FocusedItem.Index);

                        listViewGraficaComanda.Items.Clear();

                        foreach (string s in fisiereSelectateComanda)
                            listViewGraficaComanda.Items.Add(Path.GetFileName(s));
                    }
                }
            }
        }

        private void toolStripMenuItemComandaDeschide_Click(object sender, EventArgs e)
        {
            if (firmaSelectata != null)
            {
                if (fisiereSelectateComanda != null && fisiereSelectateComanda.Count > 0)
                {
                    try
                    {
                        Process.Start(fisiereSelectateComanda[listViewGraficaComanda.FocusedItem.Index]);
                    }
                    catch
                    {
                        MessageBox.Show("Nu s-a putut deschide fisierul.");
                    }
                }
            }
        }

        
    }
}

//TODO CONTURI BANCARE FUNCTIONALITATE
//TODO TIPARIRE SITUATIE - ADAUGA NR COMENZI, PERIOADA DE LA PRIMA FACTURA EVER LA ULTIMA, TOTALURI TRANZACTII