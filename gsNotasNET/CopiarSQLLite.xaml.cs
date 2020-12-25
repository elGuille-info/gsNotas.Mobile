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
    public partial class CopiarSQLLite : ContentPage
    {
        private ContentPage _pagina;
        public CopiarSQLLite(ContentPage pagina = null)
        {
            InitializeComponent();
            _pagina = pagina;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            Title = $"Copiar SQLLite - {App.AppName} {App.AppVersion}";

            if (UsuarioSQL.UsuarioLogin.NotasCopiadas)
            {
                VolverAMain();
                return;
            }

            // Comprobar si hay mensajes en la base de SQL Lite.
            var tn = App.Database.CountAsync();
            if (tn.Result == 0)
            {
                LabelInfo.Text = $"No hay datos anteriores." +
                                 $"{App.crlf}{App.crlf}Pulsa en el botón 'Finalizar' para ir a la aplicación.";
                UsuarioSQL.UsuarioLogin.NotasCopiadas = true;
                UsuarioSQL.GuardarUsuario(UsuarioSQL.UsuarioLogin);
                btnFinalizar.IsVisible = true;
                return;
            }
            LabelInfo.Text = $"Copiando {tn.Result} notas anteriores.";
            var total = 0;
            foreach(var note in App.Database.GetNotesAsync().Result)
            {
                if (!note.Text.Any())
                    continue;
                var nota = new NotaSQL();
                nota.idUsuario = UsuarioSQL.UsuarioLogin.ID;
                nota.Texto = note.Text;
                nota.Modificada = note.Date;
                nota.Grupo = note.Grupo;
                NotaSQL.GuardarNota(nota);
                total++;
            }
            
            LabelInfo.Text = $"Se han copiado {total} de {tn.Result} notas anteriores." + 
                             $"{App.crlf}{App.crlf}Pulsa en el botón 'Finalizar' para ir a la aplicación.";

            UsuarioSQL.UsuarioLogin.NotasCopiadas = true;
            UsuarioSQL.GuardarUsuario(UsuarioSQL.UsuarioLogin);
            btnFinalizar.IsVisible = true;
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
    }
}