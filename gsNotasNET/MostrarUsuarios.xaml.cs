using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using gsNotasNET.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gsNotasNET
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MostrarUsuarios : ContentPage
    {
        public static MostrarUsuarios Current;

        private static List<UsuarioSQL> _Usuarios;

        public MostrarUsuarios()
        {
            InitializeComponent();
            Current = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (UsuarioSQL.UsuarioLogin is null || 
                    UsuarioSQL.UsuarioLogin.Email.ToLower().IndexOf("elguille.info@") == -1)
            {
                Navigation.PushAsync(new Login(Current));
                return;
            }
            if (_Usuarios is null || _Usuarios.Count() == 0)
                _Usuarios = UsuarioSQL.Usuarios();
            listView.ItemsSource = _Usuarios;
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //await DisplayAlert("Item Tapped", "An item was tapped.", "OK");
            
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (UsuarioSQL.UsuarioLogin is null ||
                    UsuarioSQL.UsuarioLogin.Email.ToLower().IndexOf("elguille.info@") == -1)
            {
                Navigation.PushAsync(new Login(Current));
            }
            if(_Usuarios is null || _Usuarios.Count() == 0)
            {
                _Usuarios = UsuarioSQL.Usuarios();
                listView.ItemsSource = _Usuarios;
            }
            TituloNotas();
        }

        public static void TituloNotas()
        {
            Current.Title = $"{App.AppName} {App.AppVersion}";
            Current.LabelInfo.Text = $"Hay {_Usuarios.Count()} usuarios activos y {UsuarioSQL.CountDeBaja()} de baja o eliminados."; ;
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }
    }
}
