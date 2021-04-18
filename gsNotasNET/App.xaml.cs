using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Net.Mail;

using gsNotasNET.Models;
using gsNotasNET.Data;

using Xamarin.Forms;
using Xamarin.Essentials;


namespace gsNotasNET
{
    public partial class App : Application
    {
        /// <summary>
        /// La versión de la aplicación
        /// </summary>
        public static string AppVersion { get; } = "v2..34";

        public static string AppVersionFull { get; } = "2.0.0.34";

        /// <summary>
        /// El nombre de la aplicación
        /// </summary>
        public static string AppName = "gsNotasNET.Android";

        public App()
        {
            InitializeComponent();

            // Crear las propiedades de la aplicación
            CrearPropiedadesApp();

            // Si se está usando desde el IDE de VS
            if (System.Diagnostics.Debugger.IsAttached)
            {
                AppName = "gsNotasNET.Android.Debug";
            }

            // Iniciar con el usuario de prueba.
            var usuario = UsuarioSQL.Usuario("prueba");

            if(IniciarConUsuario)
            {
                var usuario2 = UsuarioSQL.Usuario(UltimoUsuario);
                if (usuario2.ID == 0)
                    UsuarioSQL.UsuarioLogin = usuario;
            }

            // Iniciar con la página Login que vuelva a MainMenu.
            //MainPage = new NavigationPage(new Login(new MainMenu()));

            MainPage = new NavigationPage(new MainMenu());
        }

        public static string TipoConexion { get; private set; }

        /// <summary>
        /// Comprobar si hay conexión a internet.
        /// </summary>
        /// <returns>Un valor verdadero si hay conexión a Internet.</returns>
        public static bool HayConexionInternet()
        {
            var current = Connectivity.NetworkAccess;

            //TipoConexion = current.ToString();
            var sb = new StringBuilder();
            sb.Append("(");
            foreach (var cp in Connectivity.ConnectionProfiles)
            {
                sb.Append($"{cp.ToString()}; ");
            }
            var cnnPro = sb.ToString().TrimEnd(new char[] { ';', ' ' }) + ")";

            if (current == NetworkAccess.Internet)
            {
                // Connection to internet is available
                TipoConexion = $"Hay conexión a Internet. {cnnPro}";
                return true;
            }
            else if (current == NetworkAccess.ConstrainedInternet)
            {
                TipoConexion = $"Sin conexión a Internet. {cnnPro}";
                return false;
            }
            else if (current == NetworkAccess.Local)
            {
                TipoConexion = $"Sin conexión a Internet. {cnnPro}";
                return false;
            }
            else if (current == NetworkAccess.Unknown)
            {
                TipoConexion = $"Sin conexión a Internet. {cnnPro}";
                return false;
            }
            else //if (current == NetworkAccess.None)
            {
                TipoConexion = $"Sin conexión a Internet. {cnnPro}";
                return false;
            }
        }

        public static string UltimoUsuario
        {
            get 
            {
                if (Application.Current.Properties["UltimoUsuario"] is null)
                    return "";
                return Application.Current.Properties["UltimoUsuario"].ToString(); 
            }
            set { Application.Current.Properties["UltimoUsuario"] = value; }
        }
        public static string UltimoPassword
        {
            get 
            {
                if (Application.Current.Properties["UltimoPassword"] is null)
                    return "";
                return Application.Current.Properties["UltimoPassword"].ToString(); 
            }
            set { Application.Current.Properties["UltimoPassword"] = value; }
        }
        public static bool RecordarUsuario
        {
            get 
            {
                if (Application.Current.Properties["RecordarUsuario"] is null)
                    return true;
                return (bool)Application.Current.Properties["RecordarUsuario"]; 
            }
            set { Application.Current.Properties["RecordarUsuario"] = value; }
        }
        public static bool RecordarPassword 
        {
            get 
            {
                if (Application.Current.Properties["RecordarPassword"] is null)
                    return false;
                return (bool)Application.Current.Properties["RecordarPassword"]; 
            }
            set { Application.Current.Properties["RecordarPassword"] = value; } 
        }
        public static bool IniciarConUsuario
        {
            get 
            {
                if (Application.Current.Properties["IniciarConUsuario"] is null)
                    return false;
                return (bool)Application.Current.Properties["IniciarConUsuario"]; 
            }
            set { Application.Current.Properties["IniciarConUsuario"] = value; }
        }
        //
        public static string BuscarTexto
        {
            get 
            {
                if (Application.Current.Properties["BuscarTexto"] is null)
                    return "";
                return Application.Current.Properties["BuscarTexto"].ToString(); 
            }
            set { Application.Current.Properties["BuscarTexto"] = value; }
        }
        public static string BuscarGrupo
        {
            get
            {
                if (Application.Current.Properties["BuscarGrupo"] is null)
                    return "";
                return Application.Current.Properties["BuscarGrupo"].ToString();
            }
            set { Application.Current.Properties["BuscarGrupo"] = value; }
        }
        public static bool BuscarFavoritas
        {
            get 
            {
                if (Application.Current.Properties["BuscarFavoritas"] is null)
                    return false;
                return (bool)Application.Current.Properties["BuscarFavoritas"]; }
            set { Application.Current.Properties["BuscarFavoritas"] = value; }
        }
        public static bool BuscarArchivadas
        {
            get 
            {
                if (Application.Current.Properties["BuscarArchivadas"] is null)
                    return false;
                return (bool)Application.Current.Properties["BuscarArchivadas"]; 
            }
            set { Application.Current.Properties["BuscarArchivadas"] = value; }
        }
        public static bool BuscarEliminadas
        {
            get 
            {
                if (Application.Current.Properties["BuscarEliminadas"] is null)
                    return false;
                return (bool)Application.Current.Properties["BuscarEliminadas"]; 
            }
            set { Application.Current.Properties["BuscarEliminadas"] = value; }
        }
        public static bool BuscarNotificar
        {
            get 
            {
                if (Application.Current.Properties["BuscarNotificar"] is null)
                    return false;
                return (bool)Application.Current.Properties["BuscarNotificar"]; 
            }
            set { Application.Current.Properties["BuscarNotificar"] = value; }
        }
        // Sinconizar y usar base local
        public static bool SincronizarAuto
        {
            get 
            { 
                if(Application.Current.Properties["SincronizarAuto"] is null)
                        return true;
                return (bool)Application.Current.Properties["SincronizarAuto"];
            }
            set { Application.Current.Properties["SincronizarAuto"] = value; }
        }
        public static bool UsarNotasLocal
        {
            get 
            {
                if (Application.Current.Properties["UsarNotasLocal"] is null)
                    return false;
                return (bool)Application.Current.Properties["UsarNotasLocal"]; 
            }
            set { Application.Current.Properties["UsarNotasLocal"] = value; }
        }
        public static bool Notificar
        {
            get 
            {
                if (Application.Current.Properties["Notificar"] is null)
                    return true;
                return (bool)Application.Current.Properties["Notificar"]; 
            }
            set { Application.Current.Properties["Notificar"] = value; }
        }
        //
        public static string UltimoGrupo
        {
            get 
            {
                if (Application.Current.Properties["UltimoGrupo"] is null)
                    return "";
                return Application.Current.Properties["UltimoGrupo"].ToString(); 
            }
            set { Application.Current.Properties["UltimoGrupo"] = value; }
        }

        private void CrearPropiedadesApp()
        {
            if (!Application.Current.Properties.ContainsKey("UltimoUsuario"))
                Application.Current.Properties.Add("UltimoUsuario", "");
            if (!Application.Current.Properties.ContainsKey("UltimoPassword"))
                Application.Current.Properties.Add("UltimoPassword", "");
            if (!Application.Current.Properties.ContainsKey("RecordarUsuario"))
                Application.Current.Properties.Add("RecordarUsuario", true);
            if (!Application.Current.Properties.ContainsKey("RecordarPassword"))
                Application.Current.Properties.Add("RecordarPassword",false);
            if (!Application.Current.Properties.ContainsKey("IniciarConUsuario"))
                Application.Current.Properties.Add("IniciarConUsuario", false);
            // Para la búsqueda
            if (!Application.Current.Properties.ContainsKey("BuscarTexto"))
                Application.Current.Properties.Add("BuscarTexto", "");
            if (!Application.Current.Properties.ContainsKey("BuscarGrupo"))
                Application.Current.Properties.Add("BuscarGrupo", "");
            if (!Application.Current.Properties.ContainsKey("BuscarFavoritas"))
                Application.Current.Properties.Add("BuscarFavoritas", false);
            if (!Application.Current.Properties.ContainsKey("BuscarArchivadas"))
                Application.Current.Properties.Add("BuscarArchivadas", false);
            if (!Application.Current.Properties.ContainsKey("BuscarEliminadas"))
                Application.Current.Properties.Add("BuscarEliminadas", false);
            if (!Application.Current.Properties.ContainsKey("BuscarNotificar"))
                Application.Current.Properties.Add("BuscarNotificar", false);
            // Para sincronizar las notas
            if (!Application.Current.Properties.ContainsKey("SincronizarAuto"))
                Application.Current.Properties.Add("SincronizarAuto", true);
            if (!Application.Current.Properties.ContainsKey("UsarNotasLocal"))
                Application.Current.Properties.Add("UsarNotasLocal", false);
            // Notificar las notas marcadas como notificar
            if (!Application.Current.Properties.ContainsKey("Notificar"))
                Application.Current.Properties.Add("Notificar", true);
            // El último grupo indicado en editar notas
            if (!Application.Current.Properties.ContainsKey("UltimoGrupo"))
                Application.Current.Properties.Add("UltimoGrupo", "");

        }

        /// <summary>
        /// Generar un código de validación y enviarlo al email.
        /// </summary>
        /// <returns></returns>
        async public static Task<string> CodigoValidación(string email)
        {
            // Generar el hash de validación.
            var hash = UsuarioSQL.ValidarHash(email);

            var fecha = DateTime.UtcNow;

            var sb = new StringBuilder();
            sb.AppendLine($"El código de validación para {App.AppName} es:");
            sb.AppendLine($"{App.crlf}{App.crlf}{hash}{App.crlf}{App.crlf}");
            sb.AppendLine($"Indicalo en la aplicación '{App.AppName} {App.AppVersion}'.");
            sb.AppendLine();
            sb.AppendLine("Gracias.");
            sb.AppendLine("Guillermo.");
            sb.AppendLine();
            sb.AppendLine($"Enviado a: '{email}'");
            sb.AppendLine();
            sb.AppendFormat("Para validar el correo puedes indicar el código de validación durante las {0} horas (UTC).", DateTime.UtcNow.Hour);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Cualquier bug o duda sobre el programa, por favor usa la página de comentarios del programa.");
            sb.AppendLine("Gracias.");
            sb.AppendLine();

            // Enviar el código de validación al email
            await SendEmail("Codigo de validacion", sb.ToString(), email);

            //return new Task<string>(() => hash);
            return hash;
        }

        // https://www.c-sharpcorner.com/article/xamarin-forms-send-email-using-smtp2/
        /// <summary>
        /// Enviar un email usando una cuenta de gmail.
        /// Usando la cuenta configurada en la aplicación.
        /// </summary>
        /// <param name="subject">El asunto del mensaje.</param>
        /// <param name="body">El texto del mensaje.</param>
        /// <param name="emailTo">El correo a quien se lo manda.</param>
        /// <returns></returns>
        async public static Task SendEmail(string subject, string body, string emailTo)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(CorreoUsuario);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.CC.Add(CorreoUsuario);

                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(CorreoUsuario, CorreoPassword);

                SmtpServer.Send(mail);

                await Nada();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Para cuando se necesesite usar await llamar a este método con: await App.Nada();
        /// </summary>
        /// <returns></returns>
        async public static Task Nada()
        {
            await Task.Run(() => true);
        }

        /// <summary>
        /// Manda un email desde una de las cuentas configuradas en el dispostivo.
        /// </summary>
        /// <param name="subject">El asunto del mensaje.</param>
        /// <param name="body">El texto del mensaje.</param>
        /// <param name="emailTo">A quién se le manda el mensaje.</param>
        /// <param name="emailCc">Por si se manda copia de este mensaje.</param>
        /// <returns></returns>
        public static async Task SendEmailUserLocal(string subject, string body, string emailTo, string emailCc = "")
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = new List<string>() { emailTo },
                    BodyFormat = EmailBodyFormat.PlainText,
                    //Bcc = bccRecipients
                };
                if (emailCc.Any())
                    message.Cc = new List<string>() { emailCc };

                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
                Debug.WriteLine(fbsEx.Message);
            }
            catch (Exception ex)
            {
                // Some other exception occurred
                Debug.WriteLine(ex.Message);
            }
        }

        public static string crlf { get { return "\n\r"; } }

        /// <summary>
        /// Los datos de las credenciales obtenidas del directorio Assets
        /// </summary>
        public static System.IO.Stream ClientSecretJson { get; set; }

        private static Stream _CredencialesSQL;
        /// <summary>
        /// Los datos para la conexión a la base de datos.
        /// No se incluye en el código fuente.
        /// </summary>
        /// <remarks>
        /// Tienes que crear un fichero llamado encrypted-string.txt,
        /// guardarlo en la carpeta Assets de la aplicación de Android,
        /// y guardar el usuario y el password, cada uno en una línea del fichero.
        /// </remarks>
        public static System.IO.Stream CredencialesSQL 
        {
            get { return _CredencialesSQL; }
            set 
            {
                // Al asignar el stream llamar a la asignación de la cadena de conexión.
                // De esta forma se usa el stream abierto una vez.
                _CredencialesSQL = value;
                NotasNETSQLDatabase.CadenaConexion = "";
            }
        }

        public static System.IO.Stream CredencialesGuille
        {
            set
            {
                using (var passwStream = new StreamReader(value))
                {
                    var s = passwStream.ReadLine();
                    Login.Usuario = s;
                    s = passwStream.ReadLine();
                    Login.Password = s;
                }
            }
        }

        private static string CorreoUsuario;
        private static string CorreoPassword;

        //encrypted-string-correos-elguille.txt
        public static System.IO.Stream CredencialesCorreosGuille
        {
            set
            {
                using (var passwStream = new StreamReader(value))
                {
                    var s = passwStream.ReadLine();
                    CorreoUsuario = s;
                    s = passwStream.ReadLine();
                    CorreoPassword = s;
                }
            }
        }

        /// <summary>
        /// Muestra la página de la política de privacidad en el navegador predeterminado.
        /// </summary>
        /// <returns></returns>
        public static async Task MostrarPoliticaPrivacidad()
        {
            var uri = new Uri("https://www.elguillemola.com/politica-de-privacidad/");
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }

        //
        // Para acceder a la base de datos SQLLite
        //

        static NotasDatabase database;

        public static NotasDatabase Database
        {
            get
            {
                if (database == null)
                {
                    if (App.AppName.Contains("Debug"))
                        database = new NotasDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gsNotasNET.db3"));
                    else
                        database = new NotasDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gsNotasNETLocal.db3"));
                    //database = new NotasDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gsNotasNET.db3"));
                }
                return database;
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}