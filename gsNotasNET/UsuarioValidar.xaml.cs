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
    public partial class UsuarioValidar : ContentPage
    {
        public static string CodigoValidar;
        public static UsuarioValidar Current;
        private static ContentPage _pagina;
        public UsuarioValidar(string codigoValidar, ContentPage pagina)
        {
            InitializeComponent();
            CodigoValidar = codigoValidar;
            _pagina = pagina;
            Current = this;
        }

        private void btnValidar_Clicked(object sender, EventArgs e)
        {
            ComprobarCodigoValidar();
        }

        async public static void ComprobarCodigoValidar()
        {
            if (Current.txtValidar.Text.ToUpper() != CodigoValidar)
            {
                UsuarioPerfil.Validado = false;
                await Current.Navigation.PopAsync();
            }
            else
            {
                UsuarioPerfil.Validado = true;
                // Volver a la página que llamó a esta
                VolverAMain();
            }
        }
        private static void VolverAMain()
        {
            if (_pagina is null)
                Application.Current.MainPage = new NavigationPage(new MainMenu());
            else
            {
                try
                {
                    Current.Navigation.PushAsync(_pagina);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    _pagina = null;
                    VolverAMain();
                }
            }
        }

        /// <summary>
        /// Volver a la página de UsuarioValidar después de mostrar la alerta.
        /// </summary>
        async public static void CallBackAfertHide()
        {
            var uv = new UsuarioValidar(App.CodigoValidación(UsuarioSQL.UsuarioLogin.Email).Result, Current);
            await Current.Navigation.PushAsync(uv);
        }

    }
}