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
    public partial class InterfaceKlienta : Form
    {
        private DataTable tablicaKlientow;

        public InterfaceKlienta ()
        {
            InitializeComponent();
        }

        private void InterfaceKlienta_Load (object sender, EventArgs e)
        {
            wypelnijListeKlientow();
            wypelnijInformacjeOKliencie(DaneLogowania.Id);
            wypelnijRezerwacje(DaneLogowania.Id);
        }

        private void wypelnijInformacjeOKliencie (int idKlienta)
        {
            DataTable tablicaInformacjiOKlientach = tablicaKlientow.Select("IdKlienta = " + idKlienta).CopyToDataTable();
            informacjeOKliencie.DataSource = tablicaInformacjiOKlientach;
        }

        private void wypelnijListeKlientow()
        {
            string zapytanieOListeKlientow =
                "Select L.IdKlienta, L.Imie, L.Nazwisko, L.Adres, L.Pesel, L.Telefon, L.Email, L.Imie + ' ' + L.Nazwisko As Nazwa "
            + "From WypozyczalniaLodzi.dbo.Klienci L ";

            tablicaKlientow = ObslugaBazyDanych.pobierzTablice(zapytanieOListeKlientow);

        }

        private void wypelnijRezerwacje (int idKlienta)
        {
            string zapytanieRezerwacji =
                "Select L.NazwaLodzi, R.DataWynajmu, R.DataZwrotu, L.CenaWynajmu"
                + " From [WypozyczalniaLodzi].[dbo].[Rezerwacje] R Inner Join WypozyczalniaLodzi.dbo.Lodzie L ON R.LodzieId = L.IdLodzi"
                + " Where R.KlienciId = " + idKlienta;

            rezerwacje.DataSource = ObslugaBazyDanych.pobierzTablice(zapytanieRezerwacji);
        }
    }
}
