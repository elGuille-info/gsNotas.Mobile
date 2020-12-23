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
    public partial class MostrarGrupos : ContentPage
    {
        public static MostrarGrupos Current;
        private static List<Grupo> _Grupos;
        public MostrarGrupos()
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
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }
    }
}