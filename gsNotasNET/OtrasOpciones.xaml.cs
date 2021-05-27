using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using gsNotasNET.Models;
using Xamarin.Essentials;

namespace gsNotasNET
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtrasOpciones : ContentPage
    {
        public static OtrasOpciones Current;
        public OtrasOpciones()
        {
            InitializeComponent();
            Current = this;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            Title = $"Otras Opciones - {App.AppName}";
            LabelStatus.Text = App.StatusInfo;
            btnInicio.IsVisible = (DeviceInfo.Platform == DevicePlatform.UWP);

            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0 || UsuarioSQL.UsuarioLogin.Email == "prueba")
                LabelInfo.Text = "No hay usuario logueado.";
            else
                LabelInfo.Text = $"Usuario: {UsuarioSQL.UsuarioLogin.Email} ({UsuarioSQL.UsuarioLogin.Nombre}).";

            MyMenu();
        }

        public void MyMenu()
        {
            var backColor = (Color)Application.Current.Resources["Accent"];
            var menu = new List<MainMenu.Menu>();

            // Solo asignar las cosas si el usuario está registrado.
            // Aunque siempre se iniciará con el usuario de prueba.
            if (!(UsuarioSQL.UsuarioLogin is null))
            {
                menu = new()
                {
                    new MainMenu.Menu { Page = new NotasMostrar(NotasDatosMostrar.Archivadas), MenuTitle = "Notas Archivadas", MenuDetail = "Muestra las notas archivadas.", Icon = "XArchivadas.png" },
                    new MainMenu.Menu { Page = new NotasMostrar(NotasDatosMostrar.Eliminadas), MenuTitle = "Notas Eliminadas", MenuDetail = "Muestra las notas eliminadas.", Icon = "XNotasEliminadas.png" },
                    new MainMenu.Menu { Page = new NotasMostrar(NotasDatosMostrar.Local), MenuTitle = "Notas Locales (solo ver)", MenuDetail = "Muestra todas las notas de la base local.", Icon = "XLocal.png" },
                    //new MainMenu.Menu { Page = new GruposLista(), MenuTitle = "Grupos", MenuDetail = "Muestra los grupos creados con las notas.", Icon = "XGrupos.png" },
                };
                if(UsuarioSQL.UsuarioLogin.Pagos >= 25)
                {
                    var id = UsuarioSQL.UsuarioLogin.ID;
                    if (NotaSQL.CountNotas2048(id) >0 && NotaSQL.CountNotasMax(id) == 0)
                    {
                        menu.Add(new MainMenu.Menu { MenuBackgroundColor = backColor, IconVisible = false, TitleVisible = false });
                        menu.Add(new MainMenu.Menu { Page = new CopiarNotas(), MenuTitle = "Copiar Notas a NotasMax", MenuDetail = "Copia las notas limitadas a 2048 caracteres a notas sin límite de texto.", Icon = "XCopiarNotas.png" });
                    }
                }
                menu.Add(new MainMenu.Menu { MenuBackgroundColor = backColor, IconVisible = false, TitleVisible = false });
                menu.Add(new MainMenu.Menu { Page = new Configuracion(), MenuTitle = "Configuración", MenuDetail = "Configuración del programa.", Icon = "XConfigApp.png" });
                if (UsuarioSQL.UsuarioLogin.Email.ToLower() != "prueba" && UsuarioSQL.UsuarioLogin.Email.ToLower() != "local")
                {
                    menu.Add(new MainMenu.Menu { Page = new UsuarioPerfil(UsuarioSQL.UsuarioLogin), MenuTitle = "Perfil", MenuDetail = "Modifica el perfil del usuario actual", Icon = "XConfigurar_usuario.png" });
                }
                if (UsuarioSQL.UsuarioLogin.Email.ToLower().IndexOf("elguille.info@") > -1)
                {
                    menu.Add(new MainMenu.Menu { Page = new UsuariosMostrar(), MenuTitle = "Usuarios", MenuDetail = "Muestra los usuarios activos.", Icon = "XUsuarios.png" });
                }
                if (UsuarioSQL.UsuarioLogin.Email.ToLower() != "prueba" && UsuarioSQL.UsuarioLogin.Email.ToLower() != "local")
                {
                    menu.Add(new MainMenu.Menu { Page = new UsuarioValidar(), MenuTitle = "Validar Email", MenuDetail = "Validar el correo usado para entrar en la aplicación.", Icon = "XUsuarioValidar.png" });
                    menu.Add(new MainMenu.Menu { Page = new SincronizarBases(), MenuTitle = "Sincronizar", MenuDetail = "Sincronizar las bases de datos local y remota.", Icon = "XSincronizar.png" });
                }
            }
            menu.Add(new MainMenu.Menu { MenuBackgroundColor = backColor, IconVisible = false, TitleVisible = false });
            menu.Add(new MainMenu.Menu { Page = new Comentarios(), MenuTitle = "Comentarios", MenuDetail = "Enviar comentarios a elGuille.", Icon = "XEnviarComentarios.png" });
            menu.Add(new MainMenu.Menu { Page = new AcercaDegsNotasNET(), MenuTitle = "Acerca de...", MenuDetail = "Información breve de la aplicación.", Icon = "XAppInfo.png" });
            ListMenu.ItemsSource = menu;
        }
        private void ListMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is MainMenu.Menu menu)
            {
                //IsPresented = false;
                //Detail = menu.Page;
                Current.Navigation.PushAsync(menu.Page);
            }
        }
        async private void btnInicio_Clicked(object sender, EventArgs e)
        {
            // Volver a la anterior
            await Current.Navigation.PopAsync();
            // Volver a la principal
            //await Navigation.PopToRootAsync();
        }
        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }
    }
}