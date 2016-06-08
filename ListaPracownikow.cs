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
    public partial class ListaPracownikow : Form
    {
        public ListaPracownikow()
        {
            InitializeComponent();
        }

        private void ListaPracownikow_Load(object sender, EventArgs e)
        {
            pracownicy.DataSource = ObslugaBazyDanych.pobierzTablice("Select * From WypozyczalniaLodzi.dbo.Pracownicy");
        }
    }
}
