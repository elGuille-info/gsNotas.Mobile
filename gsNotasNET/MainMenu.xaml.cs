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
                new Menu{ Page= new Feed("Mi Perfil"), MenuTitle="Mi Perfil", MenuDetail="My Profile", Icon="XConfigurar_usuario_clip.png"},
                new Menu{ Page= new MostrarUsuarios(), MenuTitle="Usuarios", MenuDetail="Users", Icon="XUsuarios.png"},
                new Menu{ Page= new ListaNotas(), MenuTitle="Notas", MenuDetail="Notes", Icon="XNota_Azul.png"},
                new Menu{ Page= new NotasArchivadas(), MenuTitle="Notas Archivadas", MenuDetail="Archived Notes", Icon="XGrupos.png"},
                new Menu{ Page= new Feed("Grupos"), MenuTitle="Grupos", MenuDetail="Groups", Icon="XSeleccionar_opciones.png"},
                new Menu{ Page= new Feed("Configuración"), MenuTitle="Configuración", MenuDetail="Settings", Icon="XConfiguracion.png"},
                new Menu{ Page= new Login(), MenuTitle="Cambiar de Usuario", MenuDetail="User Change", Icon="XLogin.png"}
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