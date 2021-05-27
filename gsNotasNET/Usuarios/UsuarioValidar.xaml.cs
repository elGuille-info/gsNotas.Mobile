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

        public UsuarioValidar(string codigoValidar = "", ContentPage pagina = null)
        {
            InitializeComponent();
            _pagina = pagina;
            Current = this;
            //Title = $"{App.AppName} {App.AppVersion}";
            
            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0 || UsuarioSQL.UsuarioLogin.Email == "prueba")
                return;

            // si no se ha indicado, generar un código de validación
            if (string.IsNullOrEmpty(codigoValidar))
            {
                if(!UsuarioSQL.UsuarioLogin.Validado)
                    CodigoValidar = App.CodigoValidación(UsuarioSQL.UsuarioLogin.Email).Result;
            }
            else
                CodigoValidar = codigoValidar.ToUpper();
        }

        async private void ContentPage_Appearing(object sender, EventArgs e)
        {
            LabelStatus.Text = App.StatusInfo;

            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0 || UsuarioSQL.UsuarioLogin.Email == "prueba")
            {
                await Navigation.PushAsync(new Login(Current));
                LabelInfo.Text = "No hay usuario logueado.";
                return;
            }
            LabelInfo.Text = "Indica el código de validación y pulsa en VALIDAR.";
        }

        private void btnValidar_Clicked(object sender, EventArgs e)
        {
            bool validado = Current.txtValidar.Text.ToUpper() == CodigoValidar;

            UsuarioSQL.UsuarioLogin.Validado = validado;
            UsuarioSQL.GuardarUsuario(UsuarioSQL.UsuarioLogin);

            string sino = validado ? "SI" : "NO";
            LabelInfo.Text = $"El código de validación {sino} es correcto.";
            txtValidar.Focus();
        }
    }
}