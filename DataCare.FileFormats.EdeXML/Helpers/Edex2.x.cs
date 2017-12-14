using DataCare.EdeXML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DataCare.EdeXML.Helpers
{
    internal class Edex20
    {
        internal static EdexSchool ParseerSchool2_x(XElement schoolElement)
        {
            var school = Generic.ParseerSchoolBasis(schoolElement);
            school.Peildatum = Generic.GetElementDate(schoolElement, "peildatum");
            return school;
        }

        internal static EdexLeerling ParseerLeerling2_x(XElement leerlingElement)
        {
            var leerling = Generic.ParseerLeerlingBasis(leerlingElement);
            leerling.Voornamen = Generic.GetElementValue(leerlingElement, "voornamen");
            leerling.Onderwijsnummer = Generic.GetElementValue(leerlingElement, "onderwijsnummer");
            leerling.BSN_Ondwnr_4 = Generic.GetElementValue(leerlingElement, "bsn_ondwnr-4");
            leerling.Bsn = Generic.GetElementValue(leerlingElement, "bsn");
            leerling.PostnummerBe = Generic.GetElementValue(leerlingElement, "postnummerbe");
            leerling.Rijksregistratienummer = Generic.GetElementValue(leerlingElement, "rijksregisternummer");
            leerling.FotoUrl = Generic.GetElementValue(leerlingElement, "fotourl");
            leerling.Vestiging = leerlingElement.Element("vestiging") != null ? leerlingElement.Element("vestiging").Attribute("key").Value : null;

            foreach (var item in leerlingElement.XPathSelectElements(".//samengestelde_groepen/samengestelde_groep"))
            {
                leerling.SamengesteldeGroepen.Add(item.Attribute("key").Value);
            }

            return leerling;
        }

        internal static EdexLeerkracht ParseerLeerkracht2_x(XElement leerkrachtElement)
        {
            var Leerkracht = Generic.ParseerLeerkrachtBasis(leerkrachtElement);
            Leerkracht.Rol = Generic.GetElementValue(leerkrachtElement, "rol");
            Leerkracht.Rolomschrijving = Generic.GetElementValue(leerkrachtElement, "rolomschrijving");
            Leerkracht.Voornamen = Generic.GetElementValue(leerkrachtElement, "voornamen");
            Leerkracht.Gebruikersnaam = Generic.GetElementValue(leerkrachtElement, "gebruikersnaam");
            Leerkracht.Emailadres = Generic.GetElementValue(leerkrachtElement, "emailadres");

            foreach (var item in leerkrachtElement.XPathSelectElements(".//groepen/samengestelde_groep"))
            {
                Leerkracht.SamengesteldeGroepen.Add(item.Attribute("key").Value);
            }

            return Leerkracht;
        }

        internal static EdexGroep ParseerGroep2_x(XElement groepElement)
        {
            var groep = Generic.ParseerGroepBasis(groepElement);
            groep.Omschrijving = Generic.GetElementValue(groepElement, "omschrijving");
            groep.Mutatiedatum = Generic.GetElementDate(groepElement, "mutatiedatum");
            if (groepElement.Element("toevoegingen") != null)
            {
                foreach (var item in groepElement.Element("toevoegingen").Elements())
                {
                    Opmerking opmerking = new Opmerking();
                    opmerking.Code = item.Element("code").Value;
                    opmerking.Tekst = Generic.GetElementValue(item, "opmerking");
                    groep.Toevoegingen.Add(opmerking);
                }
            }
            return groep;
        }

        internal static EdexVestiging ParseerVestiging2_x(XElement vestigingElement)
        {
            return ParseerVestigingBasis(vestigingElement);
        }

        internal static EdexSamengesteldegroep ParseerSamengesteldegroep2_x(XElement samengesteldegroepElement)
        {
            var samengesteldegroep = ParseerSamengesteldegroepBasis(samengesteldegroepElement);
            samengesteldegroep.Omschrijving = Generic.GetElementValue(samengesteldegroepElement, "omschrijving");
            return samengesteldegroep;
        }

        internal static List<EdexVestiging> ParseerVestigingen(XElement element, Func<XElement, EdexVestiging> parseerVestigingVersie) =>
        element.Elements().Where(e => e.Name == "vestiging").Select(vestigingElement => parseerVestigingVersie(vestigingElement)).ToList();

        internal static EdexVestiging ParseerVestigingBasis(XElement vestigingElement)
        {
            var vestiging = new EdexVestiging();
            vestiging.Key = vestigingElement.Attribute("key").Value;
            vestiging.Naam = Generic.GetElementValue(vestigingElement, "naam");
            return vestiging;
        }

        internal static Dictionary<string, EdexSamengesteldegroep> ParseerSamengesteldegroepen(XElement element, Func<XElement, EdexSamengesteldegroep> parseerSamengesteldegroepVersie) =>
                element.Elements().Where(e => e.Name == "samengestelde_groep").Select(samengesteldegroepElement => parseerSamengesteldegroepVersie(samengesteldegroepElement)).ToDictionary(sg => sg.Key, sg => sg);

        private static EdexSamengesteldegroep ParseerSamengesteldegroepBasis(XElement samengesteldegroepElement)
        {
            var samengesteldegroep = new EdexSamengesteldegroep();
            samengesteldegroep.Key = samengesteldegroepElement.Attribute("key").Value;
            samengesteldegroep.Naam = Generic.GetElementValue(samengesteldegroepElement, "naam");
            return samengesteldegroep;
        }
    }
}