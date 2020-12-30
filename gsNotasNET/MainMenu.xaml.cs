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
            Title = $"Menú - {App.AppName} {App.AppVersion}";

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

        public void MyMenu()
        {
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
                    new Menu { Page = new NotasMostrar(NotasDatosMostrar.Archivadas), MenuTitle = "Notas Archivadas", MenuDetail = "Muestra las notas archivadas.", Icon = "XArchivadas.png" },
                    new Menu { Page = new NotasMostrar(NotasDatosMostrar.Eliminadas), MenuTitle = "Notas Eliminadas", MenuDetail = "Muestra las notas eliminadas.", Icon = "XNotasEliminadas.png" },
                    new Menu { Page = new NotasMostrar(NotasDatosMostrar.Local), MenuTitle = "Notas Locales (solo ver)", MenuDetail = "Muestra todas las notas de la base local.", Icon = "XLocal.png" },
                    new Menu { Page = new GruposMostrar(), MenuTitle = "Grupos", MenuDetail = "Muestra los grupos creados con las notas.", Icon = "XGrupos.png" },
                    new Menu { Page = new NotasBuscar(), MenuTitle = "Buscar", MenuDetail = "Buscar texto en las notas.", Icon = "XBuscar.png" }
                };
                if (UsuarioSQL.UsuarioLogin.Email.ToLower() != "prueba")
                {
                    menu.Add(new Menu { Page = new UsuarioPerfil(UsuarioSQL.UsuarioLogin), MenuTitle = "Perfil", MenuDetail = "Modifica el perfil del usuario actual", Icon = "XConfigurar_usuario.png" });
                }
                if (UsuarioSQL.UsuarioLogin.Email.ToLower().IndexOf("elguille.info@") > -1)
                {
                    menu.Add(new Menu { Page = new UsuariosMostrar(), MenuTitle = "Usuarios", MenuDetail = "Muestra los usuarios activos.", Icon = "XUsuarios.png" });
                }
                menu.Add(new Menu { Page = new Configuracion(), MenuTitle = "Configuración", MenuDetail = "Configuración del programa.", Icon = "XConfigApp.png" });
                if (UsuarioSQL.UsuarioLogin.Email.ToLower() != "prueba")
                {
                    menu.Add(new Menu { Page = new UsuarioValidar(), MenuTitle = "Validar Email", MenuDetail = "Validar el correo usado para entrar en la aplicación.", Icon = "XUsuarioValidar.png" });
                    menu.Add(new Menu { Page = new SincronizarBases(), MenuTitle = "Sincronizar", MenuDetail = "Sincronizar las bases de datos local y remota.", Icon = "XSincronizar.png" });
                }
            }
            menu.Add(new Menu { Page = new Comentarios(), MenuTitle = "Comentarios", MenuDetail = "Enviar comentarios a elGuille.", Icon = "XEnviarComentarios.png" });
            menu.Add(new Menu { Page = new AcercaDegsNotasNET(), MenuTitle = "Acerca de...", MenuDetail = "Información breve de la aplicación.", Icon = "XAppInfo.png" });
            menu.Add(new Menu { Page = new Login(), MenuTitle = "Cambiar de Usuario", MenuDetail = "Cambiar de usuario o iniciar sesión.", Icon = "XLogin.png" });
            ListMenu.ItemsSource = menu;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            // Al mostrarse, si el usuario no es elguille.info@
            // no mostrar el menú de mostrar usuarios.
            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0 || UsuarioSQL.UsuarioLogin.Email == "prueba")
            {
                LabelInfo.Text = "No hay usuario logueado.";
            }
            else
                LabelInfo.Text = $"Usuario: {UsuarioSQL.UsuarioLogin.Email} ({UsuarioSQL.UsuarioLogin.Nombre}).";

            string msgVersion = "";

            try
            {
                //System.Reflection.Assembly ensamblado = typeof(AcercaDegsNotasNET).Assembly;
                var ensamblado = System.Reflection.Assembly.GetExecutingAssembly();

                var versionWeb = "xx";


                //var cualVersion = VersionUtilidades.CompararVersionWeb(ensamblado, ref versionWeb);
                var cualVersion = VersionUtilidades.CompararVersionWeb(App.AppName, App.AppVersionFull, ref versionWeb);

                if (cualVersion == 1)
                    msgVersion = $"Hay una versión más reciente: v{versionWeb}.";
                else //if (cualVersion == -1)
                    msgVersion = $"Esta versión es la más reciente.";
            }
            catch (Exception ex)
            {
                var nota = new NotaSQL();
                nota.Texto = $"Error:\n\r{ex.Message}";
                nota.Grupo = "Error";
                Navigation.PushAsync(new NotaEditar
                {
                    DatosMostrar = NotasDatosMostrar.Activas,
                    BindingContext = nota
                });
            }

            if (!App.HayConexionInternet())
                LabelInternet.Text = $"{App.TipoConexion} Deberías usar la base local.";
            else
                LabelInternet.Text = $"{App.TipoConexion}";

            LabelInternet.Text += $" {msgVersion}";

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