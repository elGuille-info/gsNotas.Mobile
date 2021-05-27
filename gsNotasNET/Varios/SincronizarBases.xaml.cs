using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gsNotasNET.Models;
using static gsNotasNET.APIs.Extensiones;

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
            //Title = $"Sincronizar - {App.AppName} {App.AppVersion}";
        }

        private List<NotaSQL> colLocalNoSinc = null;
        private List<NotaSQL> colRemotaNoSinc = null;
        private bool conUsuarioRemoto = false;
        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            LabelStatus.Text = App.StatusInfo;

            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0 || UsuarioSQL.UsuarioLogin.Email == "prueba")
            {
                VolverAMain();
                return;
            }

            MostrarNotasSincronizadas();
        }

        private void MostrarNotasSincronizadas()
        {
            if (UsuarioSQL.UsuarioLogin.ID > 0 && UsuarioSQL.UsuarioLogin.ID < App.MaxIDUsuario)
                conUsuarioRemoto = true;
            else
                conUsuarioRemoto = false;

            // Esto es lo que hacía antes.
            //CopiarTodasLocal2Remoto();

            colLocalNoSinc = App.Database.NotasNoSincronizadas();
            var localNoSinc = colLocalNoSinc.Count();
            LabelLocalNoSinc.Text = localNoSinc.ToString();
            if (conUsuarioRemoto)
                colRemotaNoSinc = NotaSQL.NotasNoSincronizadas(UsuarioSQL.UsuarioLogin.ID);
            else
            {
                colRemotaNoSinc = new List<NotaSQL>();
                LabelError.Text = "No hay usuario remoto logueado.";
                LabelError.IsVisible = true;
            }
            var remotaNoSinc = colRemotaNoSinc.Count();
            LabelRemotaNoSinc.Text = remotaNoSinc.ToString();
        }

        //private void btnFinalizar_Clicked(object sender, EventArgs e)
        //{
        //    VolverAMain();
        //}

        async private void VolverAMain()
        {
            if (_pagina is null)
            {
                //Application.Current.MainPage = new NavigationPage(new MainMenu());
                await Navigation.PopToRootAsync();
            }
            else
            {
                try
                {
                    await Navigation.PushAsync(_pagina);
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

        //private void btnPrivacidad_Clicked(object sender, EventArgs e)
        //{
        //    _ = App.MostrarPoliticaPrivacidad();
        //}

        async private void btnSincLocal_Clicked(object sender, EventArgs e)
        {
            // Sinconizar las notas no sincronizadas de la base local con la remota.
            if (colLocalNoSinc is null || colLocalNoSinc.Count() == 0)
            {
                LabelError.Text = "O no hay base local o no hay notas que sincronizar.";
                LabelError.IsVisible = true;
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
            LabelError.BackgroundColor = Color.Green;
            LabelError.IsVisible = true;

            MostrarNotasSincronizadas();
        }

        async private void btnSincRemoto_Clicked(object sender, EventArgs e)
        {
            // Sinconizar las notas no sincronizadas de la base remota con la local.
            if (colRemotaNoSinc is null || colRemotaNoSinc.Count() == 0)
            {
                LabelError.Text = "O no hay conexión a la base remota o no hay notas que sincronizar.";
                LabelError.IsVisible = true;
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
            LabelError.BackgroundColor = Color.Green;
            LabelError.IsVisible = true;

            MostrarNotasSincronizadas();
        }

        async private void btnResetSinc_Clicked(object sender, EventArgs e)
        {
            var colLocalSinc = App.Database.NotasSincronizadas();
            LabelError.Text = $"Notas sincronizadas: locales: {colLocalSinc.Count()}";
            LabelError.BackgroundColor = Color.Green;
            List<NotaSQL> colRemotaSinc = new List<NotaSQL>();
            if (conUsuarioRemoto)
            {
                colRemotaSinc = NotaSQL.NotasSincronizadas(UsuarioSQL.UsuarioLogin.ID);
                LabelError.Text += $", remotas: {colRemotaSinc.Count()}.";
            }
            else
            {
                LabelError.Text += ". No hay usuario logueado, por tanto, no se resetean las notas remotas.";
                LabelError.BackgroundColor = Color.Red;
            }
            LabelError.Text += "\n\rSincronizando...";
            LabelError.IsVisible = true;

            int tLocal = 0, tRemoto = 0;

            for(var i = 0; i < colLocalSinc.Count; i++)
            {
                tLocal++;
                colLocalSinc[i].Sincronizada = false;
                colLocalSinc[i].idNota = 0;
                await App.Database.SaveNoteAsync(colLocalSinc[i]);
            }
            for (var i = 0; i < colRemotaSinc.Count; i++)
            {
                tRemoto++;
                colRemotaSinc[i].Sincronizada = false;
                colRemotaSinc[i].idNota = 0;
                NotaSQL.GuardarNota(colRemotaSinc[i]);
            }
            LabelError.Text = $"Marcadas como no sincronizadas: {tLocal} {tLocal.Plural("local", true)} y {tRemoto} {tRemoto.Plural("remota")}.";
            LabelError.BackgroundColor = Color.Green;
            LabelError.IsVisible = true;

            MostrarNotasSincronizadas();
        }
    }
}