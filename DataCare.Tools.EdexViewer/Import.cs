using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DataCare.EdeXML;
using DataCare.EdeXML.Models;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using DataCare.Infrastructure.EdeXMLImport;

namespace DataCare.Tools.EdexViewer
{
    public class ImportViewModel : INotifyPropertyChanged
    {
        public EdexAdministratie EdexAdministratie { get; set; }

        public ObservableCollection<Vestiging> Vestigingen { get; set; }

        public ObservableCollection<EdexLeerling> EdexLeerlingenLijst { get; set; }
        
        public string PathOfFile;
        public string FileOrPathName
        {
            get
            {
                return PathOfFile;
            }
            set   
            {
                PathOfFile = value;
                importFile();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ImportViewModel()
        {
            Vestigingen = new ObservableCollection<Vestiging>();
            EdexLeerlingenLijst = new ObservableCollection<EdexLeerling>();
        }

        public void importFile()
        {
            XmlTextReader reader = new XmlTextReader(PathOfFile);
            XDocument xDoc = XDocument.Load(reader);
            EdexAdministratie = EdexmlParser.ParseEdexfile(xDoc);
            var groepenMetVestiging = EdexAdministratie.GetGroepenWithVestiging();
            
            foreach (var groepMetVestiging in groepenMetVestiging)
            {
                if (groepMetVestiging.Item1 != null)
                {
                    Vestiging vestiging = new Vestiging();
                    vestiging.Key = groepMetVestiging.Item1.Key;
                    vestiging.Naam = groepMetVestiging.Item1.Naam;
                    AddVestiging(vestiging);
                }
                else
                {
                    var bestaandeVestiging = Vestigingen.FirstOrDefault(v => v.Key == null);
                    if (bestaandeVestiging == null)
                    {
                        bestaandeVestiging = new Vestiging();
                        Vestigingen.Add(bestaandeVestiging);
                    }
                }
            }

            AddGroepen();
            
            foreach (var leerling in EdexAdministratie.Leerlingen.Values)
            {
                EdexLeerlingenLijst.Add(leerling);
            }

            NotifyPropertyChanged("EdexAdministratie");
            NotifyPropertyChanged("Vestigingen");
        }

        private void AddVestiging(Vestiging edexVestiging)
        {
            var Vestiging = new Vestiging
            {
                Key = edexVestiging.Key,
                Naam = edexVestiging.Naam
            };
            Vestigingen.Add(Vestiging);
        }

        private void AddGroepen()
        {
            var groepen = EdexAdministratie.GetGroepenWithVestiging();
            foreach (var vestigingMetGroep in groepen)
            {

                var groep = CreateGroep(vestigingMetGroep);

                if (vestigingMetGroep.Item1 != null)
                {
                    
                    var vestiging = Vestigingen.FirstOrDefault(v => v.Key == vestigingMetGroep.Item1.Key);
                    vestiging.Groepen.Add(groep);
                }
                else
                {
                    foreach (var vestiging in Vestigingen.Where(v => v.Key == null))
                    {
                        vestiging.Groepen.Add(groep);
                    }
                }
            }
        }

        private Groep CreateGroep(Tuple<EdexVestiging, EdexGroep> groepMetVestiging)
        {
            EdexGroep edexGroep = groepMetVestiging.Item2;
            Groep groep = new Groep();
            groep.Key = edexGroep.Key;
            groep.Naam = edexGroep.Naam;

           if (groepMetVestiging.Item1 != null)
           {
                foreach (var leerling in this.EdexAdministratie.Leerlingen.Values.Where(l => l.Groep == edexGroep.Key && l.Vestiging == groepMetVestiging.Item1.Key))
                {
                    groep.Leerlingen.Add(leerling);
                }
           }
           else
           {
                foreach (var leerling in EdexAdministratie.Leerlingen.Values.Where(l => l.Groep == edexGroep.Key && l.Vestiging == null))
                {
                    groep.Leerlingen.Add(leerling);
                }
           }

           foreach (var leerkracht in EdexAdministratie.Leerkrachten.Values.Where(l => l.Groepen.Contains(edexGroep.Key)))
           {
                groep.Leerkrachten.Add(leerkracht);
           }

           return groep;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
