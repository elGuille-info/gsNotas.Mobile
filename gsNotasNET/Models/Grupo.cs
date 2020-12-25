using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;
using gsNotasNET.Data;

namespace gsNotasNET.Models
{
    public class Grupo
    {
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public int Notas { get; set; }
        public int NotasArchivadas { get; set; }
        public int NotasEliminadas { get; set; }
        public int NotasFavoritas { get; set; }
        //public string CrLf { get; } = "\r\n";

        /// <summary>
        /// Los grupos que hay asignados
        /// </summary>
        /// <returns>Una colección de tipo string</returns>
        public static List<Grupo> Grupos(UsuarioSQL usuario)
        {
            List<NotaSQL> colNotas;

            // Si el usuario es elGuille, mostrar todos los grupos
            // si no, solo los del usuario y todas las notas (estén o no archivadas o eliminadas)
            if (usuario.Email.ToLower().IndexOf("elguille.info@") > -1)
                colNotas = NotaSQL.NotasUsuario(0);
            else
                colNotas = NotaSQL.NotasUsuario(usuario.ID);

            // Primero añadir a la colección de tipo Dictionary
            // para evitar repetidos
            var gruposDict = new Dictionary<string, Grupo>();
            foreach (var s in colNotas)
            {
                if (!gruposDict.ContainsKey(s.Grupo))
                {
                    gruposDict.Add(s.Grupo, new Grupo());
                }
                var g = gruposDict[s.Grupo];
                g.Nombre = s.Grupo;
                g.Usuario = usuario.Email;
                g.Notas++;
                if (s.Archivada)
                    g.NotasArchivadas++;
                if (s.Eliminada)
                    g.NotasEliminadas++;
                if (s.Favorita)
                    g.NotasFavoritas++;
            }
            var grupos = new List<Grupo>();
            foreach (var gd in gruposDict.Keys)
            {
                grupos.Add(gruposDict[gd]);
            }
            return grupos;
        }
    }
}
