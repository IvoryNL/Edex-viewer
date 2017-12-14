using System;
using System.Collections.Generic;
using NodaTime;

namespace DataCare.EdeXML.Models
{
    public class EdexGroep
    {
        public EdexGroep()
        {
            Toevoegingen = new List<Opmerking>();
        }

        public string Key { get; set; }
        public string Naam { get; set; }
        public string Jaargroep { get; set; }
        public List<Opmerking> Toevoegingen { get; set; }
        public LocalDate? Mutatiedatum { get; set; }
        public string Omschrijving { get; set; }
    }
}