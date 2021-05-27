using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gsNotasNET.APIs;
using gsNotasNET.Models;

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
            

            // si se deben mostrar las notas a notificar
            if (App.Notificar)
            {
                // Comprobar si hay notas a notificar
                var colNotificar = NotaSQL.Buscar(UsuarioSQL.UsuarioLogin.ID, "Notificar = 1 AND Eliminada = 0");
                if (colNotificar.Count() != 0)
                {
                    // Mostrar la ventana de las notas marcadas para notificar
                    Navigation.PushAsync(new NotasMostrar(NotasDatosMostrar.Notificar));
                }
            }
            MyMenu();
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            Title = $"Menú - {App.AppName} {App.AppVersion}";
            LabelStatus.Text = App.StatusInfo;

            // Al mostrarse, si el usuario no es elguille.info@
            // no mostrar el menú de mostrar usuarios.
            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0 || UsuarioSQL.UsuarioLogin.Email == "prueba")
                LabelInfo.Text = "No hay usuario logueado.";
            else
                LabelInfo.Text = $"Usuario: {UsuarioSQL.UsuarioLogin.Email} ({UsuarioSQL.UsuarioLogin.Nombre}).";

            MyMenu();
        }

        public void MyMenu()
        {
            var backColor = (Color)Application.Current.Resources["Accent"];
            var menu = new List<Menu>();

            // Solo asignar las cosas si el usuario está registrado.
            // Aunque siempre se iniciará con el usuario de prueba.
            if (!(UsuarioSQL.UsuarioLogin is null))
            {
                menu = new()
                {
                    new Menu { Page = new NotasActivas(), MenuTitle = "Notas", MenuDetail = "Muestra las notas no archivadas.", Icon = "XNotas.png" },
                    new Menu { Page = new NotasMostrar(NotasDatosMostrar.Favoritas), MenuTitle = "Notas Favoritas", MenuDetail = "Muestra las notas favoritas.", Icon = "XFavoritas.png" },
                    new Menu { Page = new NotasMostrar(NotasDatosMostrar.Notificar), MenuTitle = "Notas para Notificar", MenuDetail = "Muestra las notas indicadas para notificar.", Icon = "XNotificar.png" },
                    new MainMenu.Menu { Page = new GruposLista(), MenuTitle = "Grupos", MenuDetail = "Muestra los grupos creados con las notas.", Icon = "XGrupos.png" },
                    new Menu { Page = new NotasBuscar(), MenuTitle = "Buscar", MenuDetail = "Buscar texto en las notas.", Icon = "XBuscar.png" },
                    new Menu { MenuBackgroundColor = backColor, IconVisible = false, TitleVisible = false },
                    new Menu { Page = new OtrasOpciones(), MenuTitle = "Otras Opciones...", MenuDetail = "Más opciones de notas, Comentarios, Acerca de, etc.", Icon = "XConfigApp.png" },
                    new Menu { Page = new Login(), MenuTitle = "Cambiar de Usuario", MenuDetail = "Cambiar de usuario o iniciar sesión.", Icon = "XLogin.png" }
                };
            }
            ListMenu.ItemsSource = menu;
        }

        async private void ListMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Menu menu = e.SelectedItem as Menu;
            if (menu != null)
            {
                //IsPresented = false;
                //Detail = menu.Page;
                await Current.Navigation.PushAsync(menu.Page);
            }
        }
        public class Menu
        {
            public string MenuTitle { get; set; }
            public string MenuDetail { get; set; }
            public ImageSource Icon { get; set; }
            public ContentPage Page { get; set; }
            public bool TitleVisible { get; set; } = true;
            public Color MenuBackgroundColor { get; set; } = Color.Transparent;

            private bool _IconVisible = true;
            public bool IconVisible
            {
                get { return _IconVisible; }
                set
                {
                    _IconVisible = value;
                    LineVisible = !value;
                }
            }
            public double IconHeight { get; set; } = 48;
            public bool LineVisible { get; set; } = false;
        }
    }
}