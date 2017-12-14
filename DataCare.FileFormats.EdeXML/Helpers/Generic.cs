using DataCare.EdeXML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using DataCare.Utilities;
using DataCare.Model.Basisadministratie;
using NodaTime;

namespace DataCare.EdeXML.Helpers
{
    internal static class Generic
    {
        public static EdexAdministratie ImportEdit(XDocument ededxDocument, Func<XElement, EdexLeerling> leerlingVersie, Func<XElement, EdexLeerkracht> leerkrachtVersie, Func<XElement, EdexGroep> groepVersie, Func<XElement, EdexSchool> schoolVersie, Func<XElement, EdexVestiging> vestigingVersie, Func<XElement, EdexSamengesteldegroep> samengesteldegroepVersie)
        {
            var administratie = new EdexAdministratie();
            foreach (var element in ededxDocument.Root.Elements())
            {
                switch (element.Name.ToString())
                {
                    case "school":
                        administratie.School = ParseerSchool(element, schoolVersie);
                        break;

                    case "vestigingen":
                        administratie.Vestigingen.AddRange(Edex20.ParseerVestigingen(element, vestigingVersie));
                        break;

                    case "groepen":
                        administratie.Samengesteldegroepen.AddRange(Edex20.ParseerSamengesteldegroepen(element, samengesteldegroepVersie));
                        administratie.Groepen.AddRange(ParseerGroepen(element, groepVersie));
                        break;

                    case "leerlingen":
                        administratie.Leerlingen.AddRange(ParseerLeerlingen(element, leerlingVersie));
                        break;

                    case "leerkrachten":
                        administratie.Leerkrachten.AddRange(ParseerLeerkrachten(element, leerkrachtVersie));
                        break;

                    default:
                        break;
                }
            }

            return administratie;
        }

        public static EdexSchool ParseerSchool(XElement element, Func<XElement, EdexSchool> parseerSchoolVersie)
        {
            return parseerSchoolVersie(element);
        }

        public static EdexSchool ParseerSchoolBasis(XElement schoolElement)
        {
            var school = new EdexSchool();
            school.Schooljaar = GetElementValue(schoolElement, "schooljaar");
            school.Brincode = GetElementValue(schoolElement, "brincode");
            school.Dependancecode = GetElementValue(schoolElement, "dependancecode");
            school.Schoolkey = GetElementValue(schoolElement, "schoolkey");
            school.Versie = GetElementValue(schoolElement, "xsdversie");
            return school;
        }

        public static Dictionary<string, EdexLeerling> ParseerLeerlingen(XElement element, Func<XElement, EdexLeerling> parseerLeerlingVersie) =>
                element.Elements().Select(leerlingElement => parseerLeerlingVersie(leerlingElement)).ToDictionary(l => l.Key, l => l);

        internal static EdexLeerling ParseerLeerlingBasis(XElement leerlingElement)
        {
            var leerling = new EdexLeerling();
            leerling.Key = leerlingElement.Attribute("key").Value;
            leerling.Roepnaam = GetElementValue(leerlingElement, "roepnaam");
            leerling.Achternaam = GetElementValue(leerlingElement, "achternaam");
            leerling.Voorvoegsel = GetElementValue(leerlingElement, "voorvoegsel");
            leerling.Voorletters = GetElementValue(leerlingElement, "voorletters-1");
            leerling.Geslacht = (Geslacht)Enum.Parse(typeof(Geslacht), GetElementValue(leerlingElement, "geslacht"), true);
            leerling.Jaargroep = GetElementValue(leerlingElement, "jaargroep");
            leerling.Land = GetElementValue(leerlingElement, "land");
            leerling.Gewicht_nieuw = GetElementValue(leerlingElement, "gewicht_nieuw");
            leerling.Postcodenl = GetElementValue(leerlingElement, "postcodenl");
            leerling.Geboortedatum = GetElementDate(leerlingElement, "geboortedatum");
            leerling.Instroomdatum = GetElementDate(leerlingElement, "instroomdatum");
            leerling.Uitstroomdatum = GetElementDate(leerlingElement, "uitstroomdatum");
            leerling.Mutatiedatum = GetElementDate(leerlingElement, "mutatiedatum");
            leerling.Land_vader = GetElementValue(leerlingElement, "land_vader");
            leerling.Land_moeder = GetElementValue(leerlingElement, "land_moeder");
            leerling.Onderwijsnummer = GetElementValue(leerlingElement, "onderwijsnummer");
            leerling.Bsn = GetElementValue(leerlingElement, "sofinummer");
            leerling.Groep = leerlingElement.Element("groep") != null ? leerlingElement.Element("groep").Attribute("key").Value : null;

            foreach (var item in leerlingElement.XPathSelectElements(".//toevoegingen/blok"))
            {
                Opmerking opmerking = new Opmerking();
                opmerking.Code = GetElementValue(item, "code");
                opmerking.Tekst = GetElementValue(item, "opmerking");
                leerling.Toevoegingen.Add(opmerking);
            }

            return leerling;
        }

        private static Dictionary<string, EdexLeerkracht> ParseerLeerkrachten(XElement element, Func<XElement, EdexLeerkracht> parseerLeerkrachtVersie) =>
                element.Elements().Select(leerkrachtElement => parseerLeerkrachtVersie(leerkrachtElement)).ToDictionary(lk => lk.Key, lk => lk);

        internal static EdexLeerkracht ParseerLeerkrachtBasis(XElement leerkrachtElement)
        {
            var leerkracht = new EdexLeerkracht();
            leerkracht.Key = leerkrachtElement.Attribute("key").Value;
            leerkracht.Roepnaam = GetElementValue(leerkrachtElement, "roepnaam");
            leerkracht.Achternaam = GetElementValue(leerkrachtElement, "achternaam");
            leerkracht.Voorvoegsel = GetElementValue(leerkrachtElement, "voorvoegsel");
            leerkracht.Voorletters = GetElementValue(leerkrachtElement, "voorletters-1");
            foreach (var item in leerkrachtElement.XPathSelectElements(".//groepen/groep"))
            {
                leerkracht.Groepen.Add(item.Attribute("key").Value);
            }
            return leerkracht;
        }

        private static Dictionary<string, EdexGroep> ParseerGroepen(XElement element, Func<XElement, EdexGroep> parseerGroepVersie) =>
                element.Elements().Where(e => e.Name == "groep").Select(groepElement => parseerGroepVersie(groepElement)).ToDictionary(g => g.Key, g => g);

        internal static EdexGroep ParseerGroepBasis(XElement groepElement)
        {
            var groep = new EdexGroep();
            groep.Key = groepElement.Attribute("key").Value;
            groep.Naam = GetElementValue(groepElement, "naam");
            groep.Jaargroep = GetElementValue(groepElement, "jaargroep");
            return groep;
        }

        internal static string GetElementValue(XElement containing, XName elementName)
        {
            if (containing.Element(elementName) != null)
            {
                return containing.Element(elementName).Value;
            }
            return string.Empty;
        }

        internal static LocalDate GetElementDate(XElement containing, XName elementName)
        {
            if (containing.Element(elementName) != null)
            {
                return DateTime.Parse(containing.Element(elementName).Value).ToLocalDate();
            }
            return default(LocalDate);
        }
    }
}