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
    public partial class OknoWyboruDaty : Form
    {
        public DateTime WybranaData
        {
            get { return kalendarz.SelectionRange.Start; }
            set 
            { 
                kalendarz.SelectionRange.Start = value;
                kalendarz.SelectionRange.End = value;

                ustawDateMinimalna(value);
            }
        }

        public OknoWyboruDaty()
        {
            InitializeComponent();

            kalendarz.MinDate = DateTime.Today.AddDays(-10);
            kalendarz.MaxDate = DateTime.Today.AddDays(365);
        }

        public void ustawDateMinimalna(DateTime dataMinimalna)
        {
            kalendarz.MinDate = dataMinimalna;
            ustawDateMaksymalna(dataMinimalna.AddDays(30));
        }

        private void ustawDateMaksymalna(DateTime dataMaksymalna)
        {
            kalendarz.MaxDate = dataMaksymalna;
        }

        private void zatwierdz_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void anuluj_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
