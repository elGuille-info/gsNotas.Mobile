using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using gsNotasNET.Models;
using System.Linq;
using System;

namespace gsNotasNET.Data
{
    public class NotasDatabase
    {
        readonly SQLiteAsyncConnection _database;

        public NotasDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Nota>().Wait();
        }

        /// <summary>
        /// Los grupos que hay asignados
        /// </summary>
        /// <returns>Una colección de tipo string</returns>
        public HashSet<string> Grupos()
        {
            var colNotas = GetNotesAsync().Result;
            var grupos = new HashSet<string>();
            foreach (var s in colNotas)
            {
                if (!grupos.Contains(s.Grupo))
                {
                    grupos.Add(s.Grupo);
                }
            }
            return grupos;
        }

        /// <summary>
        /// El número de elementos en la tabla
        /// </summary>
        /// <returns></returns>
        public Task<int> CountAsync()
        { 
            return _database.Table<Nota>().CountAsync();
        }

        /// <summary>
        /// Devuelve las notas del grupo indicado
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        public Task<List<Nota>> GetNotesAsync(string grupo)
        {
            return _database.Table<Nota>()
                                .Where(n => n.Grupo == grupo)
                                .ToListAsync();
        }

        /// <summary>
        /// Devuelve las notas, clasificadas (espero)
        /// </summary>
        /// <returns></returns>
        public Task<List<Nota>> GetNotesAsync()
        {
            //return _database.Table<Nota>().ToListAsync();

            // Clasificar el contenido de la lista
            var lista = _database.Table<Nota>().ToListAsync();
            var lista2 = new List<Nota>();
            foreach (var n in lista.Result)
                lista2.Add(n);

            // Código adaptado de un ejemplo en:
            // https://tinyurl.com/y9zvt22t
            // This shows calling the Sort(Comparison(T) overload using
            // an anonymous method for the Comparison delegate.
            // This method treats null as the lesser of two values.
            lista2.Sort(delegate (Nota x, Nota y)
            {
                if (x.Grupo == null && y.Grupo == null) return 0;
                else if (x.Grupo == null) return -1;
                else if (y.Grupo == null) return 1;
                else return x.Grupo.CompareTo(y.Grupo);
            });
            lista.Result.Clear(); // Borraba los de lista2
            foreach (var n in lista2)
                lista.Result.Add(n); // lo añadía a lista2

            return lista;

            
        }

        public Task<Nota> GetNoteAsync(int id)
        {
            return _database.Table<Nota>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveNoteAsync(Nota note)
        {
            if (note.ID != 0)
            {
                return _database.UpdateAsync(note);
            }
            else
            {
                return _database.InsertAsync(note);
            }
        }

        public Task<int> DeleteNoteAsync(Nota note)
        {
            return _database.DeleteAsync(note);
        }
    }
}