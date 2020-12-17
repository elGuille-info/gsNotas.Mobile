using System;

using SQLite;

namespace gsNotasNET.Models
{
    public class Nota
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string Grupo { get; set; }

        //public string Detalle()
        //{
        //    return $"{Grupo} - {Date}";
        //}
    }
}