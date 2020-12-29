using System;
using System.IO;
using Xamarin.Forms;
using gsNotasNET.Models;
using System.Xml;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Linq;

namespace gsNotasNET
{
    public partial class NotaEditar : ContentPage
    {
        public NotasDatosMostrar DatosMostrar;

        public NotaEditar()
        {
            InitializeComponent();
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var nota = (NotaSQL)BindingContext;
            // no guardar notas en blanco
            if (string.IsNullOrEmpty(nota.Texto))
            {
                await Navigation.PopAsync();
                return;
            }

            nota.Modificada = DateTime.UtcNow;
            nota.Archivada = chkArchivada.IsToggled;
            nota.Favorita = chkFavorita.IsToggled;
            nota.Eliminada = chkEliminada.IsToggled;
            nota.Notificar = chkNotificar.IsToggled;
            nota.Grupo = txtGrupo.Text;

            // Guardar el último grupo usado
            App.UltimoGrupo = nota.Grupo;

            var note = nota.ComoNotaLocal();

            if (App.UsarNotasLocal || DatosMostrar == NotasDatosMostrar.Local)
            {
                note.ID = nota.ID;
                await App.Database.SaveNoteAsync(note);
            }
            else
            {
                NotaSQL.GuardarNota(nota);

                // Guardar los datos del usuario
                UsuarioSQL.UsuarioLogin.UltimoAcceso = DateTime.UtcNow;
                UsuarioSQL.UsuarioLogin.VersionPrograma = $"{App.AppName} {App.AppVersion}";
                UsuarioSQL.GuardarUsuario(UsuarioSQL.UsuarioLogin);
            }

            //
            // Por comprobar 
            //

            // Si la nota no es nueva, sincronizar el contenido

            // Si la nota es nueva, crear una nota en la otra base de datos

            if(App.HayConexionInternet())
            {
                // Sincronizar la nota
                if (App.UsarNotasLocal || DatosMostrar == NotasDatosMostrar.Local)
                {
                    // Se ha guardado como nota local
                    if (note.idNota != 0)
                    {
                        nota = NotaSQL.Nota(note.idNota);
                    }
                    else
                        nota = new NotaSQL();

                    if (nota.ID == 0)
                    {
                        nota = new NotaSQL
                        {
                            Archivada = note.Archivada,
                            Eliminada = note.Eliminada,
                            Favorita = note.Favorita,
                            Grupo = note.Grupo,
                            Modificada = note.Modificada,
                            Notificar = note.Notificar,
                            Sincronizada = note.Sincronizada,
                            Texto = note.Texto,
                            idNota = note.ID
                        };
                    }
                    else
                    {
                        nota = note.ComoNotaRemota();
                    }
                    nota.idNota = note.ID;
                    nota.Sincronizada = true;
                    NotaSQL.GuardarNota(nota);
                    note.idNota = nota.ID;
                    note.Sincronizada = true;
                    await App.Database.SaveNoteAsync(note);
                }
                else
                {
                    // Se ha guardado como nota remota
                    if (nota.idNota != 0)
                    {
                        note = await App.Database.GetNoteAsync(nota.idNota);
                    }
                    else
                        note = new Nota();

                    if (note.ID == 0)
                    {
                        note = new Nota
                        {
                            Archivada = nota.Archivada,
                            Eliminada = nota.Eliminada,
                            Favorita = nota.Favorita,
                            Grupo = nota.Grupo,
                            Modificada = nota.Modificada,
                            Notificar = nota.Notificar,
                            Sincronizada = nota.Sincronizada,
                            Texto = nota.Texto,
                            idNota = nota.ID
                        };
                    }
                    else
                    { 
                        note = nota.ComoNotaLocal();
                    }
                    note.idNota = nota.ID;
                    note.Sincronizada = true;
                    await App.Database.SaveNoteAsync(note);
                    nota.idNota = note.ID;
                    nota.Sincronizada = true;
                    NotaSQL.GuardarNota(nota);
                }
            }
            await Navigation.PopAsync();
            
            NotasActivas.TituloNotas();
        }

        /// <summary>
        /// Se produce cuando cambia el binding-context y por tanto la nota está asignada
        /// </summary>
        private void ContentPage_BindingContextChanged(object sender, EventArgs e)
        {
            var nota = (NotaSQL)BindingContext;
            if (nota is null)
                return;

            if (nota.Texto.Any())
            {
                char[] returns = { '\r', '\n' };
                if (nota.Texto.IndexOfAny(returns) > -1)
                {
                    var s = nota.Texto.Split(returns, StringSplitOptions.RemoveEmptyEntries);
                    
                    this.Title = $"#{nota.ID}, {nota.Modificada.ToString("dd/MM/yy HH:mm")}, {nota.Texto.Length} c. {s.Length} l.";
                }
                else
                {
                    this.Title = $"#{nota.ID}, {nota.Modificada.ToString("dd/MM/yy HH:mm")}, {nota.Texto.Length} c. 1 l.";
                }
            }
            chkArchivada.IsToggled = nota.Archivada;
            chkEliminada.IsToggled = nota.Eliminada;
            chkFavorita.IsToggled = nota.Favorita;
            chkNotificar.IsToggled = nota.Notificar;
            // No tener "bindado" el grupo de la nota
            if (string.IsNullOrEmpty(nota.Grupo))
                txtGrupo.Text = App.UltimoGrupo;
            else
                txtGrupo.Text = nota.Grupo;
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }
    }
}