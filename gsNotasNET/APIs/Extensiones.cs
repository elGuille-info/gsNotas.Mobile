using System;
using System.Collections.Generic;
using System.Text;

namespace gsNotasNET.APIs
{
    public static class Extensiones
    {
        public static string Plural(this string singular, int valor, bool conES = false)
        {
            return Plural(valor, singular, conES);
        }

        public static string Plural(this int valor, string singular, bool conES = false)
        {
            var mayusculas = singular == singular.ToUpper();

            if (valor != 1)
            {
                if (conES)
                    singular += "es";
                else
                    singular += "s";
            }
            if (mayusculas)
                return singular.ToUpper();
            return singular;
        }

    }
}
