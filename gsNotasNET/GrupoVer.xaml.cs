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
    public partial class GrupoVer : ContentPage
    {
        public GrupoVer()
        {
            InitializeComponent();
            Title = $"{App.AppName} {App.AppVersion}";
        }

        /// <summary>
        /// Se produce cuando cambia el binding-context y por tanto el grupo está asignado.
        /// </summary>
        private void ContentPage_BindingContextChanged(object sender, EventArgs e)
        {
            var grupo = (Grupo)BindingContext;
            //if (grupo is null)
            //    return;

            LabelInfo.Text = grupo.Nombre;

            listView.ItemsSource = Grupo.NotasDelGrupo(grupo.Nombre);
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }

        async private void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Poder editar las notas, sean de uso local o no.
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new NotaEditar
                {
                    DatosMostrar = NotasDatosMostrar.Activas,
                    BindingContext = e.SelectedItem as NotaSQL
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