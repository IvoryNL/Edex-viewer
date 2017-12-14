using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NodaTime;

namespace DataCare.EdeXML.Models
{
    public class EdexLeerkracht
    {
        public string naam
        {
            get
            {   
                string volledigeNaam = Roepnaam + " " + Voorvoegsel + " " + Achternaam;
                return volledigeNaam;
            }
        }
        public string Key { get; set; }
        public string Achternaam { get; set; }
        public string Voorvoegsel { get; set; }
        public string Voornamen { get; set; }
        public string Voorletters { get; set; }
        public string Roepnaam { get; set; }
        public string Gebruikersnaam { get; set; }
        public string Emailadres { get; set; }
        public string Rol { get; set; }
        public string Rolomschrijving { get; set; }
        public List<string> Groepen { get; set; }
        public List<string> SamengesteldeGroepen { get; set; }
        public List<Opmerking> Toevoegingen { get; set; }

        public LocalDate? MutatieDatum { get; set; }

        public EdexLeerkracht()
        {
            Groepen = new List<string>();
            SamengesteldeGroepen = new List<string>();
        }

        public string Naam()
        {
            string format;

            string _voornaam = this.Roepnaam.Trim();
            string _tussenvoegsel = this.Voorvoegsel.Trim();
            string _achternaam = this.Achternaam.Trim();

            if (string.IsNullOrEmpty(_voornaam))
            {
                format = string.IsNullOrEmpty(_tussenvoegsel) ? "{2}" : "{1} {2}";
            }
            else
            {
                format = string.IsNullOrEmpty(_tussenvoegsel) ? "{0} {2}" : "{0} {1} {2}";
            }

            return string.Format(format, _voornaam, _tussenvoegsel, _achternaam).ToUpper();
        }
    }
}