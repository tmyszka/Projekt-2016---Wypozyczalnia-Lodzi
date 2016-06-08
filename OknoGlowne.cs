using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WypozyczalniaLodzi
{
    public partial class OknoGlowne : Form
    {
        public OknoGlowne()
        {
            InitializeComponent();
        }

        private void OknoGlowne_Load(object sender, EventArgs e)
        {
            this.IsMdiContainer = true;
        }

        private void OknoGlowne_Shown(object sender, EventArgs e)
        {
            Logowanie logowanie = new Logowanie();

            logowanie.StartPosition = FormStartPosition.Manual;
            logowanie.Location = new Point((this.ClientSize.Width - logowanie.Width) / 2, (this.ClientSize.Height + logowanie.Height) / 2);
            logowanie.ShowDialog();

            if (logowanie.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                Close();
            }
            else
            {
                this.Text += " - jesteś zalogowany jako " + DaneLogowania.Imie + " " + DaneLogowania.Nazwisko + " (" + (DaneLogowania.CzyPracownik ? "pracownik" : "klient") + ")";

                if (DaneLogowania.CzyPracownik)
                {
                    wyswietlInterfacePracownika();
                }
                else
                {
                    wyswietlInterfaceKlienta();
                }
            }
        }

        private void wyswietlInterfacePracownika()
        {
            InterfacePracownika pracownik = new InterfacePracownika();
            pracownik.MdiParent = this;
            pracownik.Show();
        }

        private void wyswietlInterfaceKlienta()
        {
            InterfaceKlienta klient = new InterfaceKlienta();
            klient.MdiParent = this;
            klient.Show();
        }
    }
}