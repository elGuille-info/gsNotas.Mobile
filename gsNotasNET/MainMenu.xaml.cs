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
    public partial class MainMenu : ContentPage
    {
        public static MainMenu Current;
        public MainMenu()
        {
            InitializeComponent();
            Current = this;
            Title = $"Menú - {App.AppName} {App.AppVersion}";
        }

        public void MyMenu()
        {
            Title = $"Menú - {App.AppName} {App.AppVersion}";
            //Detail = new Configuracion();
            List<Menu> menu = new()
            {
                new Menu{ Page= new NotasActivas(), MenuTitle="Notas", MenuDetail="Muestra las notas no archivadas.", Icon="XNota_Azul.png"},
                new Menu{ Page= new NotasFavoritas(), MenuTitle="Notas Favoritas", MenuDetail="Muestra las notas favoritas.", Icon="XConfigurar_usuario_clip.png"},
                new Menu{ Page= new NotasArchivadas(), MenuTitle="Notas Archivadas", MenuDetail="Muestra las notas archivadas.", Icon="XGrupos.png"},
                new Menu{ Page= new GruposMostrar(), MenuTitle="Grupos", MenuDetail="Muestra los grupos creados con las notas.", Icon="XSeleccionar_opciones.png"}
            };
            if (!(UsuarioSQL.UsuarioLogin is null) && UsuarioSQL.UsuarioLogin.Email.ToLower() != "prueba")
            {
                menu.Add(new Menu { Page = new UsuarioPerfil(UsuarioSQL.UsuarioLogin), MenuTitle = "Perfil", MenuDetail = "Modifica el perfil del usuario actual", Icon = "XConfigurar_usuario.png" });
                menu.Add(new Menu { Page = new UsuarioValidar(), MenuTitle = "Validar Email", MenuDetail = "Validar el correo usado para entrar en la aplicación.", Icon = "XUsuarioValidar.png" });
            }
            if (!(UsuarioSQL.UsuarioLogin is null) &&  UsuarioSQL.UsuarioLogin.Email.ToLower().IndexOf("elguille.info@") > -1)
            {
                menu.Add(new Menu { Page = new UsuariosMostrar(), MenuTitle = "Usuarios", MenuDetail = "Muestra los usuarios activos.", Icon = "XUsuarios.png" });
            }
            menu.Add(new Menu { Page = new Configuracion(), MenuTitle = "Configuración", MenuDetail = "Configuración del programa.", Icon = "XConfigApp.png" });
            menu.Add(new Menu { Page = new Comentarios(), MenuTitle = "Comentarios", MenuDetail = "Enviar comentarios a elGuille.", Icon = "XEnviarComentarios.png" });
            menu.Add(new Menu { Page = new Login(), MenuTitle = "Cambiar de Usuario", MenuDetail = "Cambiar de usuario (ir a la página de Login).", Icon = "XLogin.png" });
            ListMenu.ItemsSource = menu;
        }
        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            // Al mostrarse, si el usuario no es elguille.info@
            // no mostrar el menú de mostrar usuarios.
            if (UsuarioSQL.UsuarioLogin is null)
            {
                LabelInfo.Text = "No hay usuario logueado.";
            }
            else
                LabelInfo.Text = $"Sesión iniciada con {UsuarioSQL.UsuarioLogin.Email}.";

            MyMenu();
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
            public string MenuTitle { get; set; }
            public string MenuDetail { get; set; }
            public ImageSource Icon { get; set; }
            public ContentPage Page { get; set; }
        }
    }
}