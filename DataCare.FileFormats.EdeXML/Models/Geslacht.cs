namespace DataCare.Model.Basisadministratie
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public enum Geslacht
    {
        Onbekend,
        Man,
        Vrouw
    }

    public static class GeslachtHelper
    {
        public static IEnumerable<Geslacht> AlleGeslachten
        {
            get { return Enum.GetValues(typeof(Geslacht)).Cast<Geslacht>(); }
        }

        public static string Value(this Geslacht geslacht)
        {
            return Enum.GetName(typeof(Geslacht), geslacht);
        }

        public static Geslacht Create(string geslacht)
        {
            Geslacht result;
            if (Enum.TryParse(geslacht, out result))
            {
                return result;
            }

            string validValues = string.Join(", ", AlleGeslachten.Select(g => g.Value()));
            throw new ArgumentOutOfRangeException(
                "geslacht",
                geslacht,
                string.Format(CultureInfo.InvariantCulture, "Waarde moet één waarde uit de volgende lijst zijn: {0}.", validValues));
        }

        public static string Titel(this Geslacht geslacht)
        {
            switch (geslacht)
            {
                case Geslacht.Man:
                    return "Dhr.";

                case Geslacht.Vrouw:
                    return "Mevr.";

                case Geslacht.Onbekend:
                default:
                    return string.Empty;
            }
        }
    }
}