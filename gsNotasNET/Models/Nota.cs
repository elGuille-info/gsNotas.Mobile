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
        /// Copiar una nota de tipo <see cref="Nota"/> a una del tipo <see cref="NotaSQL"/>.
        /// </summary>
        /// <param name="note">La nota de tipo <see cref="Nota"/>.</param>
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

        //public string Texto { get; set; }
        //public DateTime Modificada { get; set; }
        //public string Grupo { get; set; }
        //public bool Archivada { get; set; } = false;
        //public bool Eliminada { get; set; } = false;
        //public bool Favorita { get; set; } = false;
        //public bool Sincronizada { get; set; } = false;
        //public bool Notificar { get; set; } = false;


        //public string Detalle()
        //{
        //    return $"{Grupo} - {Date}";
        //}
    }
}