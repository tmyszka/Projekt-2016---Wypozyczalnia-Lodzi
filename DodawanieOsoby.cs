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
    public partial class DodawanieOsoby : Form
    {
        private bool oknoDodawaniaKlienta;
        private int idOsoby;

        private DodawanieOsoby()
        {
            InitializeComponent();
        }

        public DodawanieOsoby(bool klient)
            : this()
        {
            this.oknoDodawaniaKlienta = klient;
            this.idOsoby = -1;
        }

        public DodawanieOsoby(bool klient, int idOsoby)
            : this ()
        {
            this.oknoDodawaniaKlienta = klient;
            this.idOsoby = idOsoby;
        }

        private void DodawanieOsoby_Load(object sender, EventArgs e)
        {
            if (oknoDodawaniaKlienta)
            {
                this.Text = "Dodawanie nowego klienta";
            }
            else
            {
                this.Text = "Dodawania nowego pracownika";
                emailPole.Enabled = false;
            }

            if (idOsoby != -1)
            {
                string zapytanie = "Select * From WypozyczalniaLodzi.dbo." + (oknoDodawaniaKlienta ? "Klienci" : "Pracownicy") + " Where " + (oknoDodawaniaKlienta ? "IdKlienta" : "IdPracownika") + " = " + idOsoby;
                DataTable osoba = ObslugaBazyDanych.pobierzTablice(zapytanie);

                imiePole.Text = osoba.Rows[0]["Imie"].ToString();
                nazwiskoPole.Text = osoba.Rows[0]["Nazwisko"].ToString();
                adresPole.Text = osoba.Rows[0]["Adres"].ToString();

                peselPole.Text = osoba.Rows[0]["Pesel"].ToString();
                peselPole.Enabled = false;

                telefonPole.Text = osoba.Rows[0]["Telefon"].ToString();

                if (oknoDodawaniaKlienta)
                {
                    emailPole.Text = osoba.Rows[0]["Email"].ToString();
                }

                hasloPole.Text = osoba.Rows[0]["Haslo"].ToString();
            }
        }

        private void stworz_Click(object sender, EventArgs e)
        {
            string blad = string.Empty;

            if (imiePole.Text.Length == 0)
                blad += "Wprowadź imię.\n";

            if (nazwiskoPole.Text.Length == 0)
                blad += "Wprowadź nazwisko.\n";

            if (adresPole.Text.Length == 0)
                blad += "Wprowadź adres.\n";

            if (peselPole.Text.Length != 11)
                blad += "Wprowadź poprawny pesel (11 cyfr).\n";

            if (telefonPole.Text.Length != 9)
                blad += "Wprowadź poprawny telefon (9 cyfr).\n";

            if (((emailPole.Text.Length == 0) || !emailPole.Text.Contains("@")) && (oknoDodawaniaKlienta))
                blad += "Wprowadź poprawny email.\n";

            if (hasloPole.Text.Length == 0)
                blad += "Wprowadź hasło (do 20 znaków).\n";

            if (blad.Length > 0)
            {
                MessageBox.Show(blad);
            }
            else
            {
                try
                {
                    if (idOsoby == -1)
                    {
                        zapiszOsobe();
                        MessageBox.Show("Nowy użytkownik stworzony.");
                    }
                    else
                    {
                        aktualizujOsobe();
                        MessageBox.Show("Użytkownik zedytowany.");
                    }
                    
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.Message);
                }                
            }
        }

        private void zapiszOsobe()
        {
            object czyUzytkownikIstnieje = ObslugaBazyDanych.pobierzWartosc("Select Count (*) From WypozyczalniaLodzi.dbo." + (oknoDodawaniaKlienta ? "Klienci" : "Pracownicy") + " Where Pesel = " + peselPole.Text);

            if (czyUzytkownikIstnieje != null && (int)czyUzytkownikIstnieje > 0)
            {
                throw new Exception("Użytkownik o takim numerze pesel już istnieje.");
            }

            string zapisDoBazy = "Insert Into WypozyczalniaLodzi.dbo." + (oknoDodawaniaKlienta ? "Klienci" : "Pracownicy")
                + " Values ('" + imiePole.Text + "', '" + nazwiskoPole.Text + "', '" + adresPole.Text + "', '" + peselPole.Text + "', '" + telefonPole.Text + "', "
                + (oknoDodawaniaKlienta ? "'" + emailPole.Text + "', '" : "") + (oknoDodawaniaKlienta ? "1'" : "'2'") + ", '"
                + hasloPole.Text + "')";

            ObslugaBazyDanych.wykonajPolecenie(zapisDoBazy);
        }

        private void aktualizujOsobe()
        {
            string zapisDoBazy = "Update WypozyczalniaLodzi.dbo." + (oknoDodawaniaKlienta ? "Klienci" : "Pracownicy")
                + " Set Imie = '" + imiePole.Text + "', Nazwisko = '" + nazwiskoPole.Text + "', Adres = '" + adresPole.Text + "', Telefon = '" + telefonPole.Text + "', "
                + (oknoDodawaniaKlienta ? "Email = '" + emailPole.Text + "', " : "") + " Haslo = '" + hasloPole.Text + "' Where " + (oknoDodawaniaKlienta ? "IdKlienta" : "IdPracownika") + " = " + idOsoby;

            ObslugaBazyDanych.wykonajPolecenie(zapisDoBazy);
        }

        private void anuluj_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
