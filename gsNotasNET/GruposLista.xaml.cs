using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gsNotasNET.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gsNotasNET
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GruposLista : ContentPage
    {
        public static GruposLista Current;
        private List<Grupo> _Grupos;
        public GruposLista()
        {
            InitializeComponent();
            Current = this;
            _Grupos = null;
            //Title = $"{App.AppName} {App.AppVersion}";
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

            if (_Grupos is null || _Grupos.Count == 0)
            {
                _Grupos = Grupo.Grupos(UsuarioSQL.UsuarioLogin);
            }
            listView.ItemsSource = _Grupos;

            var plural = _Grupos.Count() == 1 ? "" : "s";
            LabelInfo.Text = $"Hay {_Grupos.Count()} grupo{plural}.";
        }

        //private void btnPrivacidad_Clicked(object sender, EventArgs e)
        //{
        //    _ = App.MostrarPoliticaPrivacidad();
        //}

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new GrupoVer
                {
                    BindingContext = e.SelectedItem as Grupo
                });
            }
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}