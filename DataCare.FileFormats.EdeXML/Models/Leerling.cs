namespace DataCare.EdeXML.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using DataCare.Model.Basisadministratie;
    using NodaTime;

    public class EdexLeerling
    {
        public EdexLeerling()
        {
            Toevoegingen = new List<Opmerking>();
            SamengesteldeGroepen = new List<string>();
        }

        public string Achternaam { get; set; }
        public string Voorvoegsel { get; set; }
        public string Voornamen { get; set; }
        public string Voorletters { get; set; }
        public LocalDate? Geboortedatum { get; set; }
        public Geslacht Geslacht { get; set; }
        public LocalDate? Start_ondw_jgr3 { get; set; }
        public string Jaargroep { get; set; }

        public string Groep { get; set; }
        public List<string> SamengesteldeGroepen { get; private set; }
        public string Vestiging { get; set; }
        public string Gebruikersnaam { get; set; }
        public string Emailadres { get; set; }
        public string FotoUrl { get; set; }
        public Etniciteit Etniciteit { get; set; }

        public string Land { get; set; }
        public string Land_vader { get; set; }
        public string Land_moeder { get; set; }
        public string Bsn { get; set; }
        public string BSN_Ondwnr_4 { get; set; }
        public string Onderwijsnummer { get; set; }
        public string Rijksregistratienummer { get; set; }
        public string Gewicht { get; set; }
        public string Gewicht_nieuw { get; set; }
        public string Postcodenl { get; set; }
        public string PostnummerBe { get; set; }
        public string PostcodeOverig { get; set; }

        public LocalDate? Instroomdatum { get; set; }
        public LocalDate? Uitstroomdatum { get; set; }
        public List<Opmerking> Toevoegingen { get; private set; }
        public LocalDate? Mutatiedatum { get; set; }

        public string Key { get; set; }
        public string Roepnaam { get; set; }

        public string Naam
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append(Achternaam);

                if (!string.IsNullOrEmpty(Voorvoegsel))
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(", ");
                    }

                    sb.Append(Voorvoegsel);
                }

                if (!string.IsNullOrEmpty(Roepnaam))
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(", ");
                    }

                    sb.Append(Roepnaam);
                }

                return sb.ToString();
            }
        }
    }
}