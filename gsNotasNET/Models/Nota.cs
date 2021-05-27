//-----------------------------------------------------------------------------
// Nota                                                             (21/Dic/20)
// Clase para las notas locales.
//
// (c) Guillermo (elGuille) Som, 2020-2021
//-----------------------------------------------------------------------------
using System;

using SQLite;

namespace gsNotasNET.Models
{
    public class Nota : NotaSQL
    {
        [PrimaryKey, AutoIncrement]
        public override int ID { get; set; }

        public Nota()
        {
            ID = 0;
            idUsuario = UsuarioSQL.UsuarioLogin.ID;
            Texto = "";
            Modificada = DateTime.UtcNow;
            Grupo = "";
            Archivada = false;
            Eliminada = false;
            Favorita = false;
            Sincronizada = false;
            Notificar = false;
            idNota = 0;
        }

        /// <summary>
        /// Copiar esta nota de tipo <see cref="Nota"/> a una del tipo <see cref="NotaSQL"/>.
        /// </summary>
        /// <returns>Una copia del tipo <see cref="NotaSQL"/>.</returns>
        public NotaSQL ComoNotaRemota()
        {
            Nota note = this;

            //return note;
            var nota = new NotaSQL
            {
                //ID = note.ID,
                Archivada = note.Archivada,
                Eliminada = note.Eliminada,
                Favorita = note.Favorita,
                Grupo = note.Grupo,
                Modificada = note.Modificada,
                Notificar = note.Notificar,
                Sincronizada = note.Sincronizada,
                Texto = note.Texto,
                idNota = note.idNota
            };
            return nota;
        }
    }
}