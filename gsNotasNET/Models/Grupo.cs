//-----------------------------------------------------------------------------
// Grupo                                                            (23/Dic/20)
// Clase para los grupos de las notas.
//
// (c) Guillermo (elGuille) Som, 2020-2021
//-----------------------------------------------------------------------------
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

        /// <summary>
        /// Las notas obtenidas al llamar al método Grupos.
        /// </summary>
        public static List<NotaSQL> NotasGrupos;

        /// <summary>
        /// Los grupos que hay asignados.
        /// </summary>
        /// <returns>Una colección de tipo string</returns>
        /// <remarks>Aquí se tiene en cuenta si se están usando las notas locales o las remotas.</remarks>
        public static List<Grupo> Grupos(UsuarioSQL usuario)
        {
            if (App.UsarNotasLocal)
            {
                NotasGrupos = App.Database.Notas(App.Database.GetNotesAsync());
            }
            else
            {
                //// Si el usuario es elGuille, mostrar todos los grupos
                //// si no, solo los del usuario y todas las notas (estén o no archivadas o eliminadas)
                //if (usuario.Email.ToLower().IndexOf("elguille.info@") > -1)
                //    colNotas = NotaSQL.NotasUsuario(0);
                //else
                //    colNotas = NotaSQL.NotasUsuario(usuario.ID);

                // Mostrar solo las notas del usuario logueado.
                NotasGrupos = NotaSQL.NotasUsuario(usuario.ID);
            }

            // Primero añadir a la colección de tipo Dictionary
            // para evitar repetidos
            var gruposDict = new Dictionary<string, Grupo>();
            foreach (var s in NotasGrupos)
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

        /// <summary>
        /// Devuelve las notas del grupo indicado.
        /// </summary>
        /// <param name="grupo">El nombre del grupo del que se devolverán las notas.</param>
        /// <returns>Una colección de tipo <see cref="List{NotaSQL}"/>.</returns>
        public static List<NotaSQL> NotasDelGrupo(string grupo)
        {
            if (NotasGrupos is null)
                Grupos(UsuarioSQL.UsuarioLogin);

            return NotasGrupos.Where((n) => n.Grupo == grupo).ToList<NotaSQL>();
        }
    }
}
