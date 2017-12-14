namespace DataCare.EdeXML.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataCare.Model.Basisadministratie;
    using DataCare.Utilities;
    using NodaTime;

    public class EdexAdministratie
    {
        public Schooljaar Schooljaar
        {
            get
            {
                if (School != null)
                {
                    return School.Schooljaar;
                }
                return Schooljaar.Onbekend;
            }
        }

        public Dictionary<string, EdexGroep> Groepen { get; private set; }
        public Dictionary<string, EdexLeerling> Leerlingen { get; private set; }
        public Dictionary<string, EdexLeerkracht> Leerkrachten { get; private set; }
        public List<EdexVestiging> Vestigingen { get; private set; }
        public Dictionary<string, EdexSamengesteldegroep> Samengesteldegroepen { get; private set; }

        public EdexSchool School { get; set; }

        public Dictionary<string, EdexLeerling> LeerlingenMetDubbelSofinummer { get; private set; }
        public Dictionary<string, EdexLeerling> InCompleteLeerlingen { get; private set; }

        public EdexAdministratie()
        {
            Groepen = new Dictionary<string, EdexGroep>();
            Leerlingen = new Dictionary<string, EdexLeerling>();
            Leerkrachten = new Dictionary<string, EdexLeerkracht>();
            Vestigingen = new List<EdexVestiging>();
            Samengesteldegroepen = new Dictionary<string, EdexSamengesteldegroep>();
            LeerlingenMetDubbelSofinummer = new Dictionary<string, EdexLeerling>();
            InCompleteLeerlingen = new Dictionary<string, EdexLeerling>();
        }

        public bool IsValid { get; private set; }

        public void ValidateEdex()
        {
            this.FillNietCompleteLeerlingen();
            this.FillDubbeleSofinummers();
            IsValid =
                School != null &&
                !LeerlingenMetDubbelSofinummer.Any() &&
                Groepen.Any() &&
                Leerlingen.Any();
        }

        public IEnumerable<EdexLeerkracht> GetLeerkrachten(EdexGroep groep)
        {
            return Leerkrachten.Where(l => l.Value.Groepen.Contains(groep.Key)).Select(kvp => kvp.Value);
        }

        public IEnumerable<EdexLeerkracht> GetLeerkrachten(EdexSamengesteldegroep samengesteldeGroep)
        {
            return Leerkrachten.Where(l => l.Value.SamengesteldeGroepen.Contains(samengesteldeGroep.Key)).Select(kvp => kvp.Value);
        }

        /// <summary>
        /// Haal leerlingen zonder:
        /// Voornaam, Achternaam, geslacht of geboortedatum
        /// </summary>
        /// <returns></returns>
        public void FillNietCompleteLeerlingen()
        {
            this.InCompleteLeerlingen.AddRange(
                Leerlingen.Where
                (l => string.IsNullOrWhiteSpace(l.Value.Achternaam) ||
                string.IsNullOrWhiteSpace(l.Value.Naam) ||
                l.Value.Geboortedatum == null ||
                l.Value.Geboortedatum == default(LocalDate)).ToList());
            //this.Leerlingen.RemoveRange(InCompleteLeerlingen);
        }

        public void FillDubbeleSofinummers()
        {
            var pgnLeerlingen = Leerlingen.GroupJoin(
                Leerlingen,
                l => l.Value.Bsn,
                l2 => l2.Value.Bsn,
                (l, l2) =>
                {
                    return new { Pgn = l.Value.Bsn, Leerling = l2 };
                });

            LeerlingenMetDubbelSofinummer = pgnLeerlingen.Where(p => p.Leerling.Count() > 1 && !string.IsNullOrWhiteSpace(p.Pgn)).SelectMany(p => p.Leerling).Distinct().ToDictionary(p => p.Key, p => p.Value);
        }

        public IEnumerable<EdexGroep> GetGroepenFromVestiging(string naam)
        {
            var leerlingen = Leerlingen.Where(l => l.Value.Vestiging == naam);
            return leerlingen.Join(Groepen,
                l => l.Value.Groep,
                g => g.Key,
                (l, g) => g.Value);
        }

        public IEnumerable<EdexGroep> GetGroepenWithoutVestiging()
        {
            var leerlingen = Leerlingen.Where(l => string.IsNullOrEmpty(l.Value.Vestiging));
            return leerlingen.Join(Groepen,
                l => l.Value.Groep,
                g => g.Key,
                (l, g) => g.Value);
        }

        public IEnumerable<Tuple<EdexVestiging, EdexGroep>> GetGroepenWithVestiging()
        {
            return Leerlingen.Values.Where(l => l.Groep != null).Select(l => Tuple.Create(l.Vestiging != null ? Vestigingen.FirstOrDefault(v => v.Key == l.Vestiging) : null, Groepen[l.Groep])).Distinct();
        }

        public IEnumerable<Tuple<EdexVestiging, EdexSamengesteldegroep>> GetSamenGesteldeGroepenWithVestiging()
        {
            var samengesteldeGroepenMetVestiging = Leerlingen.SelectMany(l => l.Value.SamengesteldeGroepen.Select(sg => new { VestigingKey = l.Value.Vestiging, Samengesteldegroep = Samengesteldegroepen[sg] })).Distinct();
            return Samengesteldegroepen.Values.GroupJoin
                (samengesteldeGroepenMetVestiging,
                sg => sg.Key,
                vsg => vsg.Samengesteldegroep.Key,
                (sg, vsg) =>
                {
                    if (vsg.Count() == 1)
                    {
                        return vsg.Select(t => Tuple.Create(Vestigingen.FirstOrDefault(v => v.Key == t.VestigingKey), t.Samengesteldegroep)).Single();
                    }
                    return Tuple.Create(default(EdexVestiging), sg);
                });
        }

        public EdexVestiging GetVestiging(EdexSamengesteldegroep groep)
        {
            var vestigingKeys = Leerlingen
                .Where(l => l.Value.SamengesteldeGroepen.Contains(groep.Key))
                .Select(l => l.Value.Vestiging).Distinct();

            if (vestigingKeys.Count() == 1)
            {
                return Vestigingen.FirstOrDefault(v => v.Key == vestigingKeys.First());
            }
            return null;
        }
    }
}