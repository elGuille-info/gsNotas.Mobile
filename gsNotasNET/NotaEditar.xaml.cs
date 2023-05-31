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

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            LabelStatus.Text = App.StatusInfo;
            if (UsuarioSQL.UsuarioLogin.Pagos < 25)
                txtTexto.Placeholder = "Escribe la nota (máximo 2048 caracteres)";
            else
                txtTexto.Placeholder = "Escribe la nota (sin límite de caracteres)";
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            NotaSQL nota = (NotaSQL)BindingContext;
            // no guardar notas en blanco
            if (string.IsNullOrEmpty(nota.Texto))
            {
                await Navigation.PopAsync();
                return;
            }

            // No reemplazar nada.                          (31/may/23 19.29)
            // Para guardar, que siempre tenga crLf.
            //ReemplazarCrLf(ref nota, ponerCrLf: true);

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
        /// Reemplazar los cambios de línea según la plataforma.
        /// </summary>
        /// <param name="nota"></param>
        private void ReemplazarCrLf(ref NotaSQL nota, bool ponerCrLf = false)
        {
            if (nota == null) return;

            if (nota.Texto.Any())
            {
                int cr = nota.Texto.IndexOf("\r");
                int lf = nota.Texto.IndexOf("\n");

                // Comprobar el tipo de retorno de carro. v2.2.0.0 (15/Oct/21)
                if (DeviceInfo.Platform == DevicePlatform.UWP)
                {
                    // Comprobar si estaba en el formato de este dispositivo
                    if (cr > -1 && lf > -1)
                    {
                        nota.Texto = nota.Texto.Replace("\n\r", "\r");
                        nota.Texto = nota.Texto.Replace("\r\n", "\r");
                        nota.Texto = nota.Texto.Replace("\n", "\r");
                        if(ponerCrLf)
                        {
                            // Guardar siempre con \n\r
                            nota.Texto = nota.Texto.Replace("\r", "\n\r");
                        }
                    }
                }
                else
                {
                    if (cr == -1 || lf == -1)
                    {
                        if (cr > -1)
                        {
                            nota.Texto = nota.Texto.Replace("\r", "\n\r");
                        }
                        else if (lf > -1)
                        {
                            nota.Texto = nota.Texto.Replace("\n", "\n\r");
                        }
                    }
                }
            }
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
                // No reemplazar nada.                      (31/may/23 19.30)
                // No ponerlo al mostrar.
                //ReemplazarCrLf(ref nota, ponerCrLf: false);

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

        //private void btnPrivacidad_Clicked(object sender, EventArgs e)
        //{
        //    _ = App.MostrarPoliticaPrivacidad();
        //}
    }
}