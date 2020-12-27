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

        public Task<List<Nota>> GetNotesAsync()
        {
            return _database.Table<Nota>().ToListAsync();
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
            // No eliminar la nota
            note.Eliminada = true;
            return SaveNoteAsync(note);
            //return _database.DeleteAsync(note);
        }
    }
}