using DataCare.EdeXML.Models;
using DataCare.Infrastructure.EdeXMLImport;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DataCare.Tools.EdexViewer
{
    public class Vestiging
    {
        public ObservableCollection<Groep> Groepen { get; set; }
        public string Naam { get; set; }
        public string Key { get; set; }

        public Vestiging()
        {
            Groepen = new ObservableCollection<Groep>();
        }
    }
}