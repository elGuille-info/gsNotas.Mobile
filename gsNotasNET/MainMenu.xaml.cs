using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gsNotasNET
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenu : ContentPage
    {
        public static MainMenu Current;
        public MainMenu()
        {
            InitializeComponent();
            Current = this;
            MyMenu();
        }

        public void MyMenu()
        {
            Title = $"Menú - gsNotasNET.Android {App.AppVersion}";
            Detail = new Feed();
            List<Menu> menu = new List<Menu>
            {
                new Menu{ Page= new Feed("Mi Perfil"), MenuTitle="Mi Perfil", MenuDetail="My Profile", Icon="Xperfil.png"},
                new Menu{ Page= new Feed("Usuarios"), MenuTitle="Usuarios", MenuDetail="Users", Icon="Xcontactos.png"},
                new Menu{ Page= new ListaNotas(), MenuTitle="Notas", MenuDetail="Notes", Icon="Xnota.png"},
                new Menu{ Page= new Feed("Grupos"), MenuTitle="Grupos", MenuDetail="Groups", Icon="XTag_16x.svg"},
                new Menu{ Page= new ListaNotas(), MenuTitle="Notas Archivadas", MenuDetail="Archived Notes", Icon="XGroupBox_16x.svg"},
                new Menu{ Page= new Feed("Configuración"), MenuTitle="Configuración", MenuDetail="Settings", Icon="XSettings.png"},
                new Menu{ Page= new Login(), MenuTitle="Cerrar Sesión", MenuDetail="Close Session", Icon="XClose_16x.svg"}
            };
            ListMenu.ItemsSource = menu;
        }
        private void ListMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var menu = e.SelectedItem as Menu;
            if (menu != null)
            {
                //IsPresented = false;
                //Detail = menu.Page;
                Current.Navigation.PushAsync(menu.Page);
            }
        }
        public class Menu
        {
            public string MenuTitle {get; set; }
            public string MenuDetail { get; set; }
            public ImageSource Icon { get; set; }
            public ContentPage Page { get; set; }
        }
    }
}