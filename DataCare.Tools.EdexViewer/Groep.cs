using DataCare.EdeXML.Models;
using System.Collections.ObjectModel;

namespace DataCare.Infrastructure.EdeXMLImport
{
    public class Groep
    {
        public string Key { get; set; }
        public string Naam { get; set; }
        public ObservableCollection<EdexLeerling> Leerlingen { get; set; }
        public ObservableCollection<EdexLeerkracht> Leerkrachten { get; set; }

        public Groep()
        {
            Leerlingen = new ObservableCollection<EdexLeerling>();
            Leerkrachten = new ObservableCollection<EdexLeerkracht>();
        }
    }
}