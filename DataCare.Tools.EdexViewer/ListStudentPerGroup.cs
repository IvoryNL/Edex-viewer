using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCare.EdeXML;
using DataCare.EdeXML.Models;

namespace DataCare.Tools.EdexViewer
{
    class ListStudentPerGroup
    {
        public ObservableCollection<List<EdexLeerling>> KlassenLijst { get; set; }

        public ListStudentPerGroup()
        {
            KlassenLijst = new ObservableCollection<List<EdexLeerling>>();
        }

        public void voegLeerlingenToeAanKlassenLijst(EdexAdministratie EdexAdministratie)
        {
            foreach (var groep in EdexAdministratie.Groepen)
            {
                List<EdexLeerling> leerlingen = new List<EdexLeerling>();
                foreach (var leerling in EdexAdministratie.Leerlingen.Where(l => l.Value.Groep == groep.Value.Key)) {
                    leerlingen.Add(leerling.Value);
                }
                if (leerlingen.Count != 0) KlassenLijst.Add(leerlingen);
            }
        }
    }
}
