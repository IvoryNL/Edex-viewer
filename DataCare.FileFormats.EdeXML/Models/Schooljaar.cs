// -----------------------------------------------------------------------
// <copyright file="Schooljaar.cs" company="DataCare BV">
// </copyright>
// -----------------------------------------------------------------------

namespace DataCare.Model.Basisadministratie
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using DataCare.Utilities;
    using NodaTime;

    /// <summary>
    /// Schooljaar type with helper methods.
    /// </summary>
    public struct Schooljaar : IComparable<Schooljaar>, IEquatable<Schooljaar>
    {
        private const int StartMaand = 8;
        private const int StartDag = 1;

        private const int EindMaand = 7;
        private const int EindDag = 31;

        private readonly int startjaar;

        internal enum SchooljaarType
        {
            Gewoon,
            Historisch,
            Onbekend
        }

        private readonly SchooljaarType type;

        private Schooljaar(int startjaar, SchooljaarType type)
        {
            Contract.Requires(startjaar > 0);

            this.startjaar = startjaar;
            this.type = type;
        }

        private Schooljaar(int startjaar)
        {
            Contract.Requires(startjaar > 0);

            this.startjaar = startjaar;
            this.type = SchooljaarType.Gewoon;
        }

        public static Schooljaar Create(int startjaar)
        {
            Contract.Requires(startjaar > 0);

            return new Schooljaar(startjaar);
        }

        public static Schooljaar Create(string naam)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(naam));

            string[] jaren;
            int jaar;

            if (string.IsNullOrWhiteSpace(naam))
            {
                return Schooljaar.Onbekend; // leeg is óók onbekend (TODO -> GroepBindingModel r.55(prop Leerlingtoewijzingen) eindigt hier met een null value. deze regel hier is een workaround voor iets dat niet klopt
                // throw new ArgumentOutOfRangeException("naam", naam, string.Format(CultureInfo.InvariantCulture, "Waarde moet gevuld zijn."));
            }

            if (naam.ToUpperInvariant() == "Historisch".ToUpperInvariant())
            {
                return new Schooljaar(1990, SchooljaarType.Historisch);
            }

            if (naam.ToUpperInvariant() == "{Onbekend}".ToUpperInvariant())
            {
                return Schooljaar.Onbekend;
            }

            if (((jaren = naam.Split(new[] { '-' }, 2)).Length == 2) && int.TryParse(jaren[0], out jaar))
            {
                return new Schooljaar(jaar);
            }

            if (int.TryParse(naam, out jaar))
            {
                Debug.WriteLine("Schooljaar in onjuist formaat ingelezen");
                return new Schooljaar(jaar);
            }

            var now = DateTimeExtensions.Now.Year;
            string validValues = string.Join(", ", Enumerable.Range(now - 20, 40).Select(j => new Schooljaar(j).Naam));
            throw new ArgumentOutOfRangeException(
                "naam",
                naam,
                string.Format(CultureInfo.InvariantCulture, "Waarde is {0} maar moet één waarde uit de volgende lijst zijn: {1}.", naam, validValues));
        }

        public static implicit operator Schooljaar(int startjaar)
        {
            Contract.Requires(startjaar > 0);

            return Create(startjaar);
        }

        public static implicit operator int(Schooljaar jaar)
        {
            return jaar.Startjaar;
        }

        public static implicit operator Schooljaar(string naam)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(naam));

            return Create(naam);
        }

        public static implicit operator string(Schooljaar jaar)
        {
            return jaar.Naam;
        }

        public int Startjaar
        {
            get { return this.startjaar; }
        }

        public int Eindjaar
        {
            get { return this.startjaar + 1; }
        }

        public LocalDate Startdatum
        {
            get { return new LocalDate(Startjaar, StartMaand, StartDag); }
        }

        public LocalDate Einddatum
        {
            get { return new LocalDate(Eindjaar, EindMaand, EindDag); }
        }

        public static Schooljaar Huidig
        {
            get { return LocalDateExtensions.ToDay().GetSchooljaar(); }
        }

        public static Schooljaar Onbekend
        {
            get { return new Schooljaar(1990, SchooljaarType.Onbekend); }
        }

        #region Equality

        public static bool operator !=(Schooljaar x, Schooljaar y)
        {
            return !(x == y);
        }

        public static bool operator ==(Schooljaar left, Schooljaar right)
        {
            return left.startjaar == right.startjaar;
        }

        public override bool Equals(object obj)
        {
            return Equality.Of(this, obj);
        }

        public bool Equals(Schooljaar other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return this.startjaar.GetHashCode();
        }

        #endregion Equality

        public string Naam
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                switch (this.type)
                {
                    case SchooljaarType.Gewoon:
                        return string.Format(CultureInfo.InvariantCulture, "{0}-{1}", Startjaar, Eindjaar);

                    case SchooljaarType.Historisch:
                        return "Historisch";

                    case SchooljaarType.Onbekend:
                    default:
                        return string.Empty;
                }
            }
        }

        public override string ToString()
        {
            return Naam;
        }

        public int CompareTo(Schooljaar other)
        {
            return this.startjaar - other.startjaar;
        }

        internal LocalDate DatumInSchooljaar(int maand, int dag)
        {
            return maand >= this.Startdatum.Month
                ? new LocalDate(this.Startjaar, maand, dag)
                : new LocalDate(this.Eindjaar, maand, dag);
        }
    }

    public static class SchooljaarHelpers
    {
        private const int Januari = 1;
        private const int Maart = 3;
        private const int April = 4;
        private const int Juli = 7;
        private const int Augustus = 8;
        private const int November = 11;
        private const int December = 12;

        public static DateInterval Afnameperiode(char periode)
        {
            DateInterval returnValue = null;
            switch (periode)
            {
                case 'B':
                    returnValue = new DateInterval(new LocalDate(0, Augustus, 1), new LocalDate(0, November, 30));
                    break;

                case 'M':
                    returnValue = new DateInterval(new LocalDate(0, December, 1), new LocalDate(0, Maart, 31));
                    break;

                case 'E':
                    returnValue = new DateInterval(new LocalDate(0, April, 1), new LocalDate(0, Juli, 31));
                    break;
            }

            return returnValue;
        }

        public static DateInterval Afnameperiode(LocalDate datum)
        {
            DateInterval returnvalue = null;

            // Maanden gebuikt omdat er geen jaren beschikbaar zijn
            if (datum.Month == December)
            {
                returnvalue = new DateInterval(new LocalDate(datum.Year, December, 1), new LocalDate(datum.Year + 1, Maart, 31));
            }

            if (datum.Month >= Januari && datum.Month <= Maart)
            {
                returnvalue = new DateInterval(new LocalDate(datum.Year - 1, December, 1), new LocalDate(datum.Year, Maart, 31));
            }

            if (datum.Month >= April && datum.Month <= Juli)
            {
                returnvalue = new DateInterval(new LocalDate(datum.Year, April, 1), new LocalDate(datum.Year, Juli, 31));
            }

            if (datum.Month >= Augustus && datum.Month <= November)
            {
                returnvalue = new DateInterval(new LocalDate(datum.Year, Augustus, 1), new LocalDate(datum.Year, November, 30));
            }

            return returnvalue;
        }

        [Pure]
        public static bool IsInSchooljaar(this LocalDate datum, Schooljaar jaar)
        {
            if (jaar != default(Schooljaar))
            {
                return jaar.Startdatum <= datum && datum <= jaar.Einddatum;
            }

            return false;
        }

        public static readonly int JaarBijStart = DateTimeExtensions.Now.Year;
        public static readonly int JaarOnderGrens = JaarBijStart - 30;
        public static readonly int JaarBovenGrens = JaarBijStart + 70;

        [Pure]
        public static Schooljaar GetSchooljaar(this LocalDate datum)
        {
            Contract.Requires(JaarOnderGrens >= 1);
            Contract.Requires(JaarBovenGrens <= int.MaxValue);
            Contract.Requires(JaarBovenGrens - JaarOnderGrens == 100);

            Schooljaar jaar = datum.Year;
            Schooljaar vorigJaar = datum.Year - 1;

            // Sanity check
            // Betreft eeuw correctie
            if (jaar < JaarOnderGrens || jaar > JaarBovenGrens)
            {
                Debug.WriteLine(
                    "SANITY CHECK TRIGGERED: datum voor Schooljaar voor {0} of na {1}, gok dat datum er binnen hoort",
                    JaarOnderGrens,
                    JaarBovenGrens);

                int eeuwJaar = datum.Year % 100;

                Func<int, int, int> adjust = (eeuwjaar, eeuw) => eeuwjaar + ((eeuw / 100) * 100);

                if (eeuwJaar < JaarBovenGrens % 100)
                {
                    eeuwJaar = adjust(eeuwJaar, JaarBovenGrens);
                }
                else
                {
                    eeuwJaar = adjust(eeuwJaar, JaarOnderGrens);
                }

                return GetSchooljaar(new LocalDate(eeuwJaar, datum.Month, datum.Day));
            }

            return datum.IsInSchooljaar(jaar) ? jaar : vorigJaar;
        }

        public static DateInterval GetPeriod(this Schooljaar schooljaar)
        {
            return new DateInterval(schooljaar.Startdatum, schooljaar.Einddatum);
        }

        [Pure]
        public static Schooljaar GetVorigSchooljaar(this Schooljaar schooljaar)
        {
            return Schooljaar.Create(schooljaar.Startjaar - 1);
        }

        [Pure]
        public static Schooljaar GetVolgendSchooljaar(this Schooljaar schooljaar)
        {
            return Schooljaar.Create(schooljaar.Startjaar + 1);
        }
    }
}