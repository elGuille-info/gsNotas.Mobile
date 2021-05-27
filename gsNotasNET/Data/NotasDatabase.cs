//-----------------------------------------------------------------------------
// NotasDatabase                                                    (21/Dic/20)
// Para las notas locales guardadas con SQLLite.
//
// (c) Guillermo (elGuille) Som, 2020-2021
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using SQLite;
using gsNotasNET.Models;


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

        public List<NotaSQL> Notas(Task<List<Nota>> colNotes)
        {
            var colNotas = new List<NotaSQL>();

            //var colNotes = App.Database.GetNotesAsync().Result;
            foreach (var note in colNotes.Result)
            {
                colNotas.Add(note);
            }
            return colNotas;
        }


        /// <summary>
        /// Devuelve las notas marcadas para notificar que no estén eliminadas.
        /// </summary>
        /// <returns>Una colección con las notas marcadas para notificar.</returns>
        public List<NotaSQL> NotasNotificar()
        {
            return Notas(_database.Table<Nota>()
                                .Where(n => n.Notificar == true && n.Eliminada == false)
                                .ToListAsync());
        }

        /// <summary>
        /// Devuelve las notas favoritas.
        /// </summary>
        /// <returns>Una colección con las notas favoritas.</returns>
        public List<NotaSQL> NotasFavoritas()
        {
            return Notas( _database.Table<Nota>()
                                .Where(n => n.Favorita == true)
                                .ToListAsync());
        }

        /// <summary>
        /// Devuelve las notas eliminadas.
        /// </summary>
        /// <returns>Una colección con las notas eliminadas.</returns>
        public List<NotaSQL> NotasEliminadas()
        {
            return Notas( _database.Table<Nota>()
                                .Where(n => n.Eliminada == true)
                                .ToListAsync());
        }

        /// <summary>
        /// Devuelve las notas archivadas y no eliminadas.
        /// </summary>
        /// <returns>Una colección con las notas archivadas.</returns>
        public List<NotaSQL> NotasArchivadas()
        {
            return Notas(_database.Table<Nota>()
                                .Where(n => n.Archivada == true && n.Eliminada == false)
                                .ToListAsync());
        }

        /// <summary>
        /// Devuelve las notas no sincronizadas.
        /// </summary>
        /// <returns>Una colección con las notas no sincronizadas.</returns>
        public List<NotaSQL> NotasNoSincronizadas()
        {
            return Notas(_database.Table<Nota>()
                                .Where(n => n.Sincronizada == false)
                                .ToListAsync());
        }

        /// <summary>
        /// Devuelve las notas sincronizadas.
        /// </summary>
        /// <returns>Una colección con las notas no sincronizadas.</returns>
        public List<NotaSQL> NotasSincronizadas()
        {
            return Notas(_database.Table<Nota>()
                                .Where(n => n.Sincronizada == true)
                                .ToListAsync());
        }

        /// <summary>
        /// Devuelve todas las notas.
        /// </summary>
        /// <returns>Una colección con todas las notas.</returns>
        public List<NotaSQL> NotasTodas()
        {
            return Notas(_database.Table<Nota>().ToListAsync());
        }

        /// <summary>
        /// Devuelve la lista de notas a mostrar en la página NotasActivas.
        /// </summary>
        /// <param name="archivadas"></param>
        /// <param name="eliminadas"></param>
        /// <returns></returns>
        public Task<List<Nota>> NotasUsuarioAsync(bool? archivadas = null, bool? eliminadas = null)
        {
            if( archivadas is null && eliminadas is null)
            {
                // return GetNotesAsync(); 
                return _database.Table<Nota>().ToListAsync();
            }
            else if(archivadas is null)
            {
                return _database.Table<Nota>()
                    .Where(n => n.Eliminada == eliminadas)
                    .ToListAsync();
            }
            else
            {
                return _database.Table<Nota>()
                    .Where(n => n.Eliminada == eliminadas && n.Archivada == archivadas)
                    .ToListAsync();
            }
        }

        public List<string> Grupos()
        {
            // todas las notas estén o no archivadas
            var colNotas = GetNotesAsync().Result;
            var grupos = new List<string>();
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

        ///// <summary>
        ///// Devuelve las notas del grupo indicado
        ///// </summary>
        ///// <param name="grupo"></param>
        ///// <returns></returns>
        //public Task<List<Nota>> GetNotesAsync(string grupo)
        //{
        //    return _database.Table<Nota>()
        //                        .Where(n => n.Grupo == grupo)
        //                        .ToListAsync();
        //}

        /// <summary>
        /// Busca en todas las notas el texto indicado en las propiedades Texto y/o Grupo.
        /// </summary>
        /// <param name="texto">El texto a buscar en Texto.</param>
        /// <param name="grupo">El texto a buscar en Grupo.</param>
        /// <returns></returns>
        public List<NotaSQL> BuscarNotas(string texto, string grupo)
        {
            if (texto.Any())
                texto = texto.ToLower();
            if(grupo.Any())
                grupo = grupo.ToLower();

            if (texto.Any() && grupo.Any())
                return Notas(_database.Table<Nota>()
                    .Where(n => n.Texto.ToLower().IndexOf(texto) > -1 &&
                                n.Grupo.ToLower().IndexOf(grupo) > -1)
                    .ToListAsync());
            else if(grupo.Any())
                return Notas(_database.Table<Nota>()
                    .Where(n => n.Grupo.ToLower().IndexOf(grupo) > -1)
                    .ToListAsync());
            else
                return Notas(_database.Table<Nota>()
                    .Where(n => n.Texto.ToLower().IndexOf(texto) > -1)
                    .ToListAsync());
        }

        /// <summary>
        /// Devuelve TODAS las notas.
        /// </summary>
        /// <returns>Una colección con todas las notas.</returns>
        public Task<List<Nota>> GetNotesAsync()
        {
            return _database.Table<Nota>().ToListAsync();
        }

        public Task<Nota> GetNoteAsync(int id)
        {
            return _database.Table<Nota>()
                            .Where(n => n.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveNoteAsync(NotaSQL note)
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

        public Task<int> DeleteNoteAsync(NotaSQL note)
        {
            // No eliminar la nota
            note.Eliminada = true;
            return SaveNoteAsync(note);
            //return _database.DeleteAsync(note);
        }
    }
}