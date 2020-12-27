using System;

using SQLite;

namespace gsNotasNET.Models
{
    public class Nota
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Texto { get; set; }
        public DateTime Modificada { get; set; }
        public string Grupo { get; set; }
        public bool Archivada { get; set; } = false;
        public bool Eliminada { get; set; } = false;
        public bool Favorita { get; set; } = false;
        public bool Sincronizada { get; set; } = false;


        //public string Detalle()
        //{
        //    return $"{Grupo} - {Date}";
        //}
    }
}