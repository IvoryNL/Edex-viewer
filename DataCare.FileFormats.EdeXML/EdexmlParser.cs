using DataCare.EdeXML.Models;
using DataCare.EdeXML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Collections.ObjectModel;

namespace DataCare.EdeXML

{
    public static class EdexmlParser
    {
        public static EdexAdministratie ParseEdexfile(string importDocString)
        {
            XmlTextReader reader = new XmlTextReader(importDocString);
            XDocument importDoc = XDocument.Load(reader);
            return ParseEdexfile(importDoc);
        }

        public static EdexAdministratie ParseEdexfile(XDocument importDoc)
        {
            EdexAdministratie result;
            var versie = importDoc.XPathSelectElement("EDEX/school/xsdversie").Value;
            switch (versie)
            {
                case "2.1":
                    result = Helpers.Generic.ImportEdit(importDoc,
                            Helpers.Edex20.ParseerLeerling2_x,
                            Helpers.Edex20.ParseerLeerkracht2_x,
                            Helpers.Edex20.ParseerGroep2_x,
                            Helpers.Edex20.ParseerSchool2_x,
                            Helpers.Edex20.ParseerVestiging2_x,
                            Helpers.Edex20.ParseerSamengesteldegroep2_x);
                    break;

                case "2.0":
                    result = Helpers.Generic.ImportEdit(importDoc,
                            Helpers.Edex20.ParseerLeerling2_x,
                            Helpers.Edex20.ParseerLeerkracht2_x,
                            Helpers.Edex20.ParseerGroep2_x,
                            Helpers.Edex20.ParseerSchool2_x,
                            Helpers.Edex20.ParseerVestiging2_x,
                            Helpers.Edex20.ParseerSamengesteldegroep2_x);
                    break;

                case "1.03":
                    result = Helpers.Generic.ImportEdit(importDoc,
                            Helpers.Generic.ParseerLeerlingBasis,
                            Helpers.Generic.ParseerLeerkrachtBasis,
                            Helpers.Generic.ParseerGroepBasis,
                            Helpers.Generic.ParseerSchoolBasis, null, null);
                    break;

                default:
                    result = new EdexAdministratie();
                    break;
            }
            result.ValidateEdex();
            return result;
        }
    }
}