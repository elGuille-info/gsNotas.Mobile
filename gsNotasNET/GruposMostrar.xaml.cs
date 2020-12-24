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
    public partial class GruposMostrar : ContentPage
    {
        public static GruposMostrar Current;
        private static List<Grupo> _Grupos;
        public GruposMostrar()
        {
            InitializeComponent();
            Current = this;
            _Grupos = null;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (_Grupos is null || _Grupos.Count == 0)
                _Grupos = Grupo.Grupos(UsuarioSQL.UsuarioLogin);
            listView.ItemsSource = _Grupos;
            TituloNotas();
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }

        public static void TituloNotas()
        {
            Current.Title = $"{App.AppName} {App.AppVersion}";
            Current.LabelInfo.Text = $"Hay {_Grupos.Count()} grupos."; ;
        }

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

            //await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}