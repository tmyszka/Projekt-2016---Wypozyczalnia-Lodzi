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
    public partial class OknoDialogowe : Form
    {
        public string Wynik
        {
            get { return tekstWpisywany.Text; }
        }

        public OknoDialogowe (string tytul, string tekst)
        {
            InitializeComponent();

            this.Text = tytul;
            this.naglowek.Text = tekst;
        }

        private void przyciskWyjscia_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
