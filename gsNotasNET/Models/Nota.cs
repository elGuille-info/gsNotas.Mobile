using System;

using SQLite;

namespace gsNotasNET.Models
{
    public class Nota //: IComparable<Nota>, IEquatable<Nota>
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string Grupo { get; set; }

        ///// <summary>
        ///// Con esta implementación no los ordena,
        ///// he cambiado el orden a ver si así.
        ///// </summary>
        ///// <param name="other"></param>
        ///// <returns></returns>
        //public int CompareTo(Nota other)
        //{
        //    return (other.Grupo + other.Date.ToString("yyyy-MM-dd HH:mm")).CompareTo(this.Grupo + this.Date.ToString("yyyy-MM-dd HH:mm"));
        //}

        ///// <summary>
        ///// A ver si con IEquatable.
        ///// Aquí no llega.
        ///// </summary>
        ///// <param name="other"></param>
        ///// <returns></returns>
        //public bool Equals(Nota other)
        //{
        //    return (Grupo + Date.ToString("yyyy-MM-dd HH:mm")).CompareTo(other.Grupo + other.Date.ToString("yyyy-MM-dd HH:mm")) == 0;
        //}

        //public string Detalle()
        //{
        //    return $"{Grupo} - {Date}";
        //}
    }
}