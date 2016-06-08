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
    public partial class Logowanie : Form
    {
        public Logowanie()
        {
            InitializeComponent();

            zalogujJako.SelectedIndex = 0;
        }

        private void zaloguj_Click(object sender, EventArgs e)
        {
            if (!czyWypelnionoWszystkiePola())
            {
                MessageBox.Show("Nie podano wszystkich danych logowania.", "Brak danych", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            bool pracownik = zalogujJako.SelectedItem.Equals("Pracownik");
            string imie = daneLogowaniaImie.Text;
            string nazwisko = daneLogowaniaNazwisko.Text;
            string haslo = daneLogowaniaHaslo.Text;

            //imie = "Maciej";
            //nazwisko = "Zadziorny";
            //haslo = "krety123";
            //pracownik = true;

            string sprawdzLogowanie =
                " Select Count (*) " +
                " From WypozyczalniaLodzi.dbo." + (pracownik ? "Pracownicy" : "Klienci") +
                " Where Imie = '" + imie + "' And Nazwisko = '" + nazwisko + "' And haslo = '" + haslo + "'";

            bool czyPoprawneLogowanie = (int)ObslugaBazyDanych.pobierzWartosc(sprawdzLogowanie) == 1;

            if (!czyPoprawneLogowanie)
            {
                MessageBox.Show("Niepoprawne dane logowania, brak dostępu do aplikacji.", "Niepoprawne dane", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {

                string pobierzId = "Select " + (pracownik ? "IdPracownika" : "IdKlienta") + " From WypozyczalniaLodzi.dbo." + (pracownik ? "Pracownicy" : "Klienci") + " Where Imie = '" + imie + "' And Nazwisko = '" + nazwisko + "' And haslo = '" + haslo + "'";

                DaneLogowania.Imie = imie;
                DaneLogowania.Nazwisko = nazwisko;
                DaneLogowania.CzyPracownik = pracownik;
                DaneLogowania.Id = (int)ObslugaBazyDanych.pobierzWartosc(pobierzId);

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private bool czyWypelnionoWszystkiePola()
        {
            if (zalogujJako.SelectedItem == null)
                return false;

            if (!zalogujJako.SelectedItem.Equals("Klient") && !zalogujJako.SelectedItem.Equals("Pracownik"))
                return false;

            if (daneLogowaniaImie.Equals(String.Empty) || daneLogowaniaNazwisko.Equals(String.Empty))
                return false;

            if (daneLogowaniaHaslo.Equals(String.Empty))
                return false;

            return true;
        }

        private void daneLogowaniaImie_Enter(object sender, EventArgs e)
        {
            if (daneLogowaniaImie.Text.Equals("Imię"))
            {
                daneLogowaniaImie.Text = String.Empty;
            }
        }

        private void daneLogowaniaImie_Leave(object sender, EventArgs e)
        {
            if (daneLogowaniaImie.Text.Equals(String.Empty))
            {
                daneLogowaniaImie.Text = "Imię";
            }
        }

        private void daneLogowaniaNazwisko_Enter(object sender, EventArgs e)
        {
            if (daneLogowaniaNazwisko.Text.Equals("Nazwisko"))
            {
                daneLogowaniaNazwisko.Text = String.Empty;
            }
        }

        private void daneLogowaniaNazwisko_Leave(object sender, EventArgs e)
        {
            if (daneLogowaniaNazwisko.Text.Equals(String.Empty))
            {
                daneLogowaniaNazwisko.Text = "Nazwisko";
            }
        }

        private void zakoncz_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
