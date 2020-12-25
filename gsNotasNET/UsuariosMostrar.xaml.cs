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
    public partial class UsuariosMostrar : ContentPage
    {
        public static UsuariosMostrar Current;

        private static List<UsuarioSQL> _Usuarios;

        public UsuariosMostrar()
        {
            InitializeComponent();
            Current = this;
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (UsuarioSQL.UsuarioLogin is null ||
                    UsuarioSQL.UsuarioLogin.Email.ToLower().IndexOf("elguille.info@") == -1)
            {
                Navigation.PushAsync(new Login(Current));
                return;
            }
            if(_Usuarios is null || _Usuarios.Count() == 0)
            {
                _Usuarios = UsuarioSQL.Usuarios();
            }
            listView.ItemsSource = _Usuarios;

            TituloNotas();
        }

        public static void TituloNotas()
        {
            Current.Title = $"{App.AppName} {App.AppVersion}";
            if (!(_Usuarios is null))
                Current.LabelInfo.Text = $"Hay {_Usuarios.Count()} usuarios activos y {UsuarioSQL.CountDeBaja()} de baja o eliminados.";
            else
                Current.LabelInfo.Text = $"No se puede acceder al total de usuarios.";
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new UsuarioVer
                {
                    BindingContext = e.SelectedItem as UsuarioSQL
                });
            }
        }
    }
}
