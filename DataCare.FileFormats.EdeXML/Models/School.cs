namespace DataCare.EdeXML.Models
{
    using DataCare.Model.Basisadministratie;
    using System;
    using NodaTime;

    public class EdexSchool
    {
        public Schooljaar Schooljaar { get; set; }
        public LocalDate? Peildatum { get; set; }
        public string Brincode { get; set; }
        public string Dependancecode { get; set; }
        public string Schoolkey { get; set; }
        public string Versie { get; set; }
    }
}