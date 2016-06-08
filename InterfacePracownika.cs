using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WypozyczalniaLodzi
{
    public partial class InterfacePracownika : Form
    {
        private DataTable tablicaLodzi;
        private DataTable tablicaKlientow;

        public InterfacePracownika()
        {
            InitializeComponent();
        }

        private void InterfacePracownika_Load(object sender, EventArgs e)
        {
            przygotujInformacjeOLodzi();

            wypelnijListeLodzi();
            wypelnijListeKlientow();

            wypelnijInformacjeOLodzi((int)listaLodzi.SelectedValue);
            wypelnijInformacjeOKliencie((int)listaKlientow.SelectedValue);
            wypelnijRezerwacje((int)listaKlientow.SelectedValue);
        }

        #region Wypełnianie i obsługa tabeli i comboboxów.

        private void przygotujInformacjeOLodzi()
        {
            informacjeOLodzi.AllowUserToAddRows = false;
            informacjeOLodzi.AllowUserToDeleteRows = false;
            informacjeOLodzi.ReadOnly = true;
        }

        private void wypelnijListeLodzi()
        {
            string zapytanieOListeLodzi =
                "Select L.IdLodzi, L.NazwaLodzi, L.Model, L.CenaWynajmu, L.Stan, M.NazwaMagazynu "
            + "From WypozyczalniaLodzi.dbo.Lodzie L "
            + " Inner Join WypozyczalniaLodzi.dbo.Magazyny M On L.MagazynyId = M.IdMagazynu";

            tablicaLodzi = ObslugaBazyDanych.pobierzTablice(zapytanieOListeLodzi);

            listaLodzi.SelectedIndexChanged -= listaLodzi_SelectedIndexChanged;
            listaLodzi.DataSource = tablicaLodzi;
            listaLodzi.ValueMember = "IdLodzi";
            listaLodzi.DisplayMember = "NazwaLodzi";

            listaLodzi.SelectedIndexChanged += listaLodzi_SelectedIndexChanged;
        }

        private void listaLodzi_SelectedIndexChanged(object sender, EventArgs e)
        {
            wypelnijInformacjeOLodzi((int)listaLodzi.SelectedValue);
        }

        private void wypelnijInformacjeOLodzi(int idLodzi)
        {
            DataTable tablicaInformacjiOLodzi = tablicaLodzi.Select("IdLodzi = " + idLodzi).CopyToDataTable();
            informacjeOLodzi.DataSource = tablicaInformacjiOLodzi;
        }

        private void wypelnijListeKlientow()
        {
            string zapytanieOListeKlientow =
                "Select L.IdKlienta, L.Imie, L.Nazwisko, L.Adres, L.Pesel, L.Telefon, L.Email, L.Imie + ' ' + L.Nazwisko As Nazwa "
            + "From WypozyczalniaLodzi.dbo.Klienci L ";

            tablicaKlientow = ObslugaBazyDanych.pobierzTablice(zapytanieOListeKlientow);

            listaKlientow.SelectedIndexChanged -= listaKlientow_SelectedIndexChanged;
            listaKlientow.DataSource = tablicaKlientow;
            listaKlientow.ValueMember = "IdKlienta";
            listaKlientow.DisplayMember = "Nazwa";

            listaKlientow.SelectedIndexChanged += listaKlientow_SelectedIndexChanged;
        }

        private void listaKlientow_SelectedIndexChanged(object sender, EventArgs e)
        {
            wypelnijInformacjeOKliencie((int)listaKlientow.SelectedValue);
            wypelnijRezerwacje((int)listaKlientow.SelectedValue);
        }

        private void wypelnijInformacjeOKliencie(int idKlienta)
        {
            DataTable tablicaInformacjiOKlientach = tablicaKlientow.Select("IdKlienta = " + idKlienta).CopyToDataTable();
            informacjeOKliencie.DataSource = tablicaInformacjiOKlientach;
        }

        private void wypelnijRezerwacje(int idKlienta)
        {
            string zapytanieRezerwacji =
                "Select R.IdRezerwacji, L.NazwaLodzi, R.DataWynajmu, R.DataZwrotu, L.CenaWynajmu, R.Zakonczona"
                + " From [WypozyczalniaLodzi].[dbo].[Rezerwacje] R Inner Join WypozyczalniaLodzi.dbo.Lodzie L ON R.LodzieId = L.IdLodzi"
                + " Where R.KlienciId = " + idKlienta;

            rezerwacje.DataSource = ObslugaBazyDanych.pobierzTablice(zapytanieRezerwacji);
        }

        #endregion

        #region Obsługa pracowników.
        private void dodajPracownika_Click(object sender, EventArgs e)
        {
            DodawanieOsoby oknoDodawania = new DodawanieOsoby(false);
            oknoDodawania.ShowDialog();
        }

        private void usunPracownika_Click(object sender, EventArgs e)
        {
            object czyAdministrator = ObslugaBazyDanych.pobierzWartosc("Select Count (*) From WypozyczalniaLodzi.dbo.Pracownicy Where UprawnieniaId = 3 And IdPracownika = " + DaneLogowania.Id);

            if (czyAdministrator == null || (int)czyAdministrator == 0)
            {
                MessageBox.Show("Do podglądu listy potrzebne są uprawnienia administratora.");

                return;
            }
    

            OknoDialogowe dialog = new OknoDialogowe("Usuwanie Pracownika", "Podaj Id pracownika do usunięcia.");
            dialog.ShowDialog();

            int idPracownika = 0;
            try
            {
                idPracownika = Int32.Parse(dialog.Wynik);

                string zapytanie = "Delete From WypozyczalniaLodzi.dbo.Pracownicy Where UprawnieniaId = 2 And IdPracownika = " + idPracownika;
                ObslugaBazyDanych.wykonajPolecenie(zapytanie);

                MessageBox.Show("Pracownik został usunięty.");
            }
            catch
            {
                MessageBox.Show("Id pracownika musi być liczbą naturalną i nie możesz usuwać administratorów!");
            }
        }

        private void edytujPracownika_Click(object sender, EventArgs e)
        {
            DodawanieOsoby oknoDodawania = new DodawanieOsoby(false, DaneLogowania.Id);
            oknoDodawania.ShowDialog();
        }

        private void listaPracownikow_Click(object sender, EventArgs e)
        {
            object czyAdministrator = ObslugaBazyDanych.pobierzWartosc("Select Count (*) From WypozyczalniaLodzi.dbo.Pracownicy Where UprawnieniaId = 3 And IdPracownika = " + DaneLogowania.Id);

            if (czyAdministrator == null || (int)czyAdministrator == 0)
            {
                MessageBox.Show("Do podglądu listy potrzebne są uprawnienia administratora.");
            }
            else
            {
                ListaPracownikow lista = new ListaPracownikow();
                lista.Show();
            }
        }
        
        #endregion

        #region Obsługa klientów.
        private void dodajKlienta_Click(object sender, EventArgs e)
        {
            DodawanieOsoby oknoDodawania = new DodawanieOsoby(true);
            oknoDodawania.ShowDialog();

            wypelnijListeKlientow();
        }

        private void usunKlienta_Click(object sender, EventArgs e)
        {
            OknoDialogowe dialog = new OknoDialogowe("Usuwanie Klienta", "Podaj Id klienta do usunięcia.");
            dialog.ShowDialog();

            int idKlienta = 0;
            try
            {
                try
                {
                    idKlienta = Int32.Parse(dialog.Wynik);
                }
                catch
                {
                    throw new Exception("Id klienta musi być liczbą naturalną.");
                }

                object czyKlientMaRezerwacjeNieoddane = ObslugaBazyDanych.pobierzWartosc("Select Count (*) From WypozyczalniaLodzi.dbo.Rezerwacje Where DataZwrotu Is Null And KlienciId = " + idKlienta);

                if (czyKlientMaRezerwacjeNieoddane == null || (int)czyKlientMaRezerwacjeNieoddane > 0)
                {
                    throw new Exception("Klient ma nie oddane rezerwacje.");
                }

                string zapytanie = "Delete From WypozyczalniaLodzi.dbo.Rezerwacje Where KlienciId = " + idKlienta + "; Delete From WypozyczalniaLodzi.dbo.Klienci Where IdKlienta = " + idKlienta;
                ObslugaBazyDanych.wykonajPolecenie(zapytanie);

                wypelnijListeKlientow();

                MessageBox.Show("Klient został usunięty.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void edytujKlienta_Click(object sender, EventArgs e)
        {
            DodawanieOsoby oknoDodawania = new DodawanieOsoby(true, (int)listaKlientow.SelectedValue);
            oknoDodawania.ShowDialog();
            wypelnijListeKlientow();
        }

        #endregion

        #region Obsługa rezerwacji.
        private void dodajRezerwacje_Click(object sender, EventArgs e)
        {
            OknoDialogowe dialogLodzi = new OknoDialogowe("Id Lodzi", "Podaj Id lodzi do rezerwacji:");
            dialogLodzi.ShowDialog();

            int idKlienta = (int)listaKlientow.SelectedValue;
            int idLodzi = 0;
            try
            {
                idLodzi = Int32.Parse(dialogLodzi.Wynik);

                object czyStatekIstnieje = ObslugaBazyDanych.pobierzWartosc("Select Count (*) From WypozyczalniaLodzi.dbo.Lodzie Where IdLodzi = " + idLodzi);

                if (czyStatekIstnieje == null || (int)czyStatekIstnieje == 0)
                {
                    throw new Exception("Brak statku.");
                }

                OknoWyboruDaty oknoWyboruDaty = new OknoWyboruDaty();
                DateTime dataPoczatku, dataKonca;

                if (oknoWyboruDaty.ShowDialog() == DialogResult.OK)
                {
                    dataPoczatku = oknoWyboruDaty.WybranaData;
                    oknoWyboruDaty.ustawDateMinimalna(dataPoczatku);

                    if (oknoWyboruDaty.ShowDialog() == DialogResult.OK)
                    {
                        dataKonca = oknoWyboruDaty.WybranaData;
                    }
                    else
                    {
                        throw new Exception("Nie ustalono daty poczatku rezerwacji.");
                    }
                }
                else
                {
                    throw new Exception("Nie ustalono daty końca rezerwacji.");
                }

                string dataOd, dataDo;
                dataOd = dataPoczatku.Year + "/" + dataPoczatku.Month + "/" + dataPoczatku.Day;
                dataDo = dataKonca.Year + "/" + dataKonca.Month + "/" + dataKonca.Day;

                string liczbaRezerwacji =
                    "Select Count (*) From [WypozyczalniaLodzi].[dbo].[Rezerwacje] " +
                    "Where " +
                    "( " +
                        "DataWynajmu Between '" + dataOd + "' And '" + dataDo + "' " +
                        "Or " +
                        "DataZwrotu Between '" + dataOd + "' And '" + dataDo + "' " +
                        "Or " +
                        "'" + dataOd + "' Between DataWynajmu And DataZwrotu " +
                        "Or " +
                        "'" + dataDo + "' Between DataWynajmu And DataZwrotu " +
                    ") " +
                    "And LodzieId = " + idLodzi;

                if ((int)ObslugaBazyDanych.pobierzWartosc(liczbaRezerwacji) > 0)
                {
                    throw new Exception("Dana łódź jest już zarezerwowana w tym terminie.");
                }

                string zapytanie = "Insert Into WypozyczalniaLodzi.dbo.Rezerwacje (LodzieId, PracownicyId, KlienciId, DataWynajmu, DataZwrotu) Values (" + idLodzi + ", " + DaneLogowania.Id + ", " + idKlienta + ", '" + dataOd + "', '" + dataDo + "')";
                ObslugaBazyDanych.wykonajPolecenie(zapytanie);

                //Przeładowanie rezerwacji.
                wypelnijRezerwacje((int)listaKlientow.SelectedValue);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Id lodzi musi być liczbą całkowitą.\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void zmianaRezerwacji_Click(object sender, EventArgs e)
        {
            OknoDialogowe dialogRezerwacji = new OknoDialogowe("Id Rezerwacji", "Podaj id rezerwcji do zmiany:");
            dialogRezerwacji.ShowDialog();

            int idKlienta = (int)listaKlientow.SelectedValue;
            int idRezerwacji = 0;
            try
            {
                idRezerwacji = Int32.Parse(dialogRezerwacji.Wynik);

                OknoWyboruDaty oknoWyboruDaty = new OknoWyboruDaty();
                DateTime dataPoczatku, dataKonca;

                if (oknoWyboruDaty.ShowDialog() == DialogResult.OK)
                {
                    dataPoczatku = oknoWyboruDaty.WybranaData;
                    oknoWyboruDaty.ustawDateMinimalna(dataPoczatku);

                    if (oknoWyboruDaty.ShowDialog() == DialogResult.OK)
                    {
                        dataKonca = oknoWyboruDaty.WybranaData;
                    }
                    else
                    {
                        throw new Exception("Nie ustalono daty poczatku rezerwacji.");
                    }
                }
                else
                {
                    throw new Exception("Nie ustalono daty końca rezerwacji.");
                }

                string dataOd, dataDo;
                dataOd = dataPoczatku.Year + "/" + dataPoczatku.Month + "/" + dataPoczatku.Day;
                dataDo = dataKonca.Year + "/" + dataKonca.Month + "/" + dataKonca.Day;

                string liczbaRezerwacji =
                    "Select Count (*) From [WypozyczalniaLodzi].[dbo].[Rezerwacje] " +
                    "Where " +
                    "( " +
                        "DataWynajmu Between '" + dataOd + "' And '" + dataDo + "' " +
                        "Or " +
                        "DataZwrotu Between '" + dataOd + "' And '" + dataDo + "' " +
                        "Or " +
                        "'" + dataOd + "' Between DataWynajmu And DataZwrotu " +
                        "Or " +
                        "'" + dataDo + "' Between DataWynajmu And DataZwrotu " +
                    ") " +
                    "And LodzieId = (Select LodzieId From [WypozyczalniaLodzi].[dbo].[Rezerwacje] Where IdRezerwacji = " + idRezerwacji + ")";

                if ((int)ObslugaBazyDanych.pobierzWartosc(liczbaRezerwacji) > 0)
                {
                    throw new Exception("Dana łódź jest już zarezerwowana w tym terminie.");
                }

                string zapytanie = "Update WypozyczalniaLodzi.dbo.Rezerwacje Set DataWynajmu = '" + dataOd + "', DataZwrotu = '" + dataDo + "' Where IdRezerwacji = " + idRezerwacji + " And KlienciId = " + idKlienta;
                ObslugaBazyDanych.wykonajPolecenie(zapytanie);

                //Przeładowanie rezerwacji.
                wypelnijRezerwacje((int)listaKlientow.SelectedValue);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Id rezerwacji musi być liczbą całkowitą.\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void usunRezerwacje_Click(object sender, EventArgs e)
        {
            OknoDialogowe dialogRezerwacji = new OknoDialogowe("Id Rezerwacji", "Podaj Id rezerwacji do rezygnacji:");
            dialogRezerwacji.ShowDialog();

            int idKlienta = (int)listaKlientow.SelectedValue;
            int idRezerwacji = 0;
            try
            {
                idRezerwacji = Int32.Parse(dialogRezerwacji.Wynik);

                object czyMoznaZrezygnowacZLodzi = ObslugaBazyDanych.pobierzWartosc("Select Count (*) From WypozyczalniaLodzi.dbo.Rezerwacje Where Zakonczona = 0 And IdRezerwacji = " + idRezerwacji + " And KlienciId = " + idKlienta);

                if (czyMoznaZrezygnowacZLodzi == null || (int)czyMoznaZrezygnowacZLodzi == 0)
                {
                    throw new Exception("Rezerwacja nie istnieje dla klienta o id: " + idKlienta + ", albo jest zakończona.");
                }

                string zapytanie = "Delete From WypozyczalniaLodzi.dbo.Rezerwacje Where Zakonczona = 0 And IdRezerwacji = " + idRezerwacji + " And KlienciId = " + idKlienta;
                ObslugaBazyDanych.wykonajPolecenie(zapytanie);

                //Przeładowanie rezerwacji.
                wypelnijRezerwacje((int)listaKlientow.SelectedValue);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Id rezerwacji musi być liczbą całkowitą.\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void zakonczRezerwacje_Click(object sender, EventArgs e)
        {
            OknoDialogowe dialogRezerwacji = new OknoDialogowe("Id Rezerwacji", "Podaj Id rezerwacji do rezygnacji:");
            dialogRezerwacji.ShowDialog();

            int idKlienta = (int)listaKlientow.SelectedValue;
            int idRezerwacji = 0;
            try
            {
                idRezerwacji = Int32.Parse(dialogRezerwacji.Wynik);

                string zapytanie = "Update WypozyczalniaLodzi.dbo.Rezerwacje Set Zakonczona = 1 Where Zakonczona = 0 And IdRezerwacji = " + idRezerwacji + " And KlienciId = " + idKlienta;
                ObslugaBazyDanych.wykonajPolecenie(zapytanie);

                //Przeładowanie rezerwacji.
                wypelnijRezerwacje((int)listaKlientow.SelectedValue);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Id rezerwacji musi być liczbą całkowitą.\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
