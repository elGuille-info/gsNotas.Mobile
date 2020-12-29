using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gsNotasNET.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gsNotasNET
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SincronizarBases : ContentPage
    {
        private ContentPage _pagina;
        public SincronizarBases(ContentPage pagina = null)
        {
            InitializeComponent();
            _pagina = pagina;
            Title = $"Sincronizar - {App.AppName} {App.AppVersion}";
        }

        private List<NotaSQL> colLocalNoSinc = null;
        private List<NotaSQL> colRemotaNoSinc = null;
        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0 || UsuarioSQL.UsuarioLogin.Email == "prueba")
            {
                VolverAMain();
                return;
            }

            // Esto es lo que hacía antes.
            //CopiarTodasLocal2Remoto();

            colLocalNoSinc = App.Database.NotasNoSincronizadas();
            var localNoSinc = colLocalNoSinc.Count();
            LabelLocalNoSinc.Text = localNoSinc.ToString();
            colRemotaNoSinc = NotaSQL.NotasNoSincronizadas(UsuarioSQL.UsuarioLogin.ID);
            var remotaNoSinc = colRemotaNoSinc.Count();
            LabelRemotaNoSinc.Text = remotaNoSinc.ToString();

            //chkSincronizarAuto.IsToggled = App.SincronizarAuto;
        }

        private void btnFinalizar_Clicked(object sender, EventArgs e)
        {
            VolverAMain();
        }

        private void VolverAMain()
        {
            if(_pagina is null)
                Application.Current.MainPage = new NavigationPage(new MainMenu());
            else
            {
                try
                {
                    Navigation.PushAsync(_pagina);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    //Navigation.PushAsync(MainMenu.Current);
                    _pagina = null;
                    VolverAMain();
                }
            }
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }

        async private void btnSincLocal_Clicked(object sender, EventArgs e)
        {
            // Sinconizar las notas no sincronizadas de la base local con la remota.
            if (colLocalNoSinc is null || colLocalNoSinc.Count() == 0)
            {
                LabelError.Text = "O no hay base local o no hay notas que sincronizar.";
                return;
            }
            var total = 0;
            foreach (var note in colLocalNoSinc)
            {
                var nota = new NotaSQL();

                // comprobar que la nota local no tenga asignado el ID de la nota remota
                if (note.idNota != 0)
                {
                    nota = NotaSQL.Nota(note.idNota);
                }
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
                        Sincronizada = true,
                        Texto = note.Texto,
                        idNota = note.ID
                    };
                }
                NotaSQL.GuardarNota(nota);
                note.idNota = nota.ID;
                note.Sincronizada = true;
                await App.Database.SaveNoteAsync(note);

                total++;
            }
            var plural = total == 1 ? "" : "s";
            LabelError.Text = $"Se han sicronizado {total} nota{plural} de la base local en la base remota.";
        }

        async private void btnSincRemoto_Clicked(object sender, EventArgs e)
        {
            // Sinconizar las notas no sincronizadas de la base remota con la local.
            if (colRemotaNoSinc is null || colRemotaNoSinc.Count() == 0)
            {
                LabelError.Text = "O no hay conexión a la base remota o no hay notas que sincronizar.";
                return;
            }
            var total = 0;
            foreach (var nota in colRemotaNoSinc)
            {
                var note = new Nota();

                // comprobar que la nota remota no tenga asignado el ID de la nota local
                if (nota.idNota != 0)
                {
                    note = await App.Database.GetNoteAsync(nota.idNota);
                }
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
                        Sincronizada = true,
                        Texto = nota.Texto,
                        idNota = nota.ID
                    };
                }
                await App.Database.SaveNoteAsync(note);
                nota.Sincronizada = true;
                nota.idNota = note.ID;
                NotaSQL.GuardarNota(nota);

                total++;
            }
            var plural = total == 1 ? "" : "s";
            LabelError.Text = $"Se han sicronizado {total} nota{plural} de la base remota en la base local.";
        }
    }
}