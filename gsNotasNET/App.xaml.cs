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
        public static string AppVersion { get; } = "v2.1..4";

        public static string AppVersionFull { get; } = "2.1.0.4";

        /// <summary>
        /// El nombre de la aplicación.
        /// </summary>
        public static string AppName = "gsNotas.Mobile";

        public App()
        {
            InitializeComponent();

            // Los colores a usar                                   (24/May/21)
            // No surten efecto, ya que se usan los valores "fijos"
            //Application.Current.Resources["ColorAzul1"] = Color.DarkGreen; // Application.Current.Resources["ColorAzul1"]; // #0073cf
            //Application.Current.Resources["ColorAzul2"] = Color.DarkOrange; // Application.Current.Resources["ColorAzul2"]; // #0063b1
            //Application.Current.Resources["ColorBlanco"] = Color.Wheat; // Application.Current.Resources["ColorBlanco"];
            ////Application.Current.Resources["ColorBlanco"] = Application.Current.Resources["ColorBlanco"];
            //Application.Current.Resources["ColorAmarillo"] = Color.Indigo; // Application.Current.Resources["ColorAmarillo"];

            FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            FicConfig = Path.Combine(FolderPath, $"{AppName}.txt");

            // Crear las propiedades de la aplicación
            //CrearPropiedadesApp();
            LeerConfig();

            //// Si se está usando desde el IDE de VS
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    AppName = "gsNotasNET.Debug";
            //}

            // Iniciar con el usuario de prueba.
            //var usuario = UsuarioSQL.Usuario("prueba");

            // Iniciar con el usuario local                         (22/May/21)
            var usuario = AsignarUsuarioLocal();

            if (IniciarConUsuario)
            {
                var usuario2 = UsuarioSQL.Usuario(UltimoUsuario);
                if (usuario2.ID == 0)
                    UsuarioSQL.UsuarioLogin = usuario;
                else
                {
                    App.UsarNotasLocal = false;
                }
            }

            // Iniciar con la página Login que vuelva a MainMenu.
            //MainPage = new NavigationPage(new Login(new MainMenu()));

            MainPage = new NavigationPage(new MainMenu());
        }

        /// <summary>
        /// El path local de la aplicación.
        /// </summary>
        public static string FolderPath { get; private set; }
        /// <summary>
        /// El fichero de configuración.
        /// </summary>
        public static string FicConfig { get; private set; }

        public static int MaxIDUsuario = 999999;

        /// <summary>
        /// Asignar el usuario local.
        /// </summary>
        public static UsuarioSQL AsignarUsuarioLocal()
        {
            var usuario = new UsuarioSQL();
            UsuarioSQL.UsuarioLogin = usuario;
            usuario.Email = "local";
            usuario.Nombre = "Usuario Local";
            usuario.ID = MaxIDUsuario;
            usuario.Validado = true;
            App.UsarNotasLocal = true;
            App.SincronizarAuto = false;
            //App.UltimoUsuario = usuario.Email;
            //App.RecordarPassword = false;
            //App.RecordarUsuario = false;
            
            return usuario;
        }

        public static string StatusInfo
        {
            get 
            {
                string msgVersion;

                if (!App.HayConexionInternet())
                {
                    msgVersion = $"ATENCIÓN: {App.TipoConexion}";
                    App.UsarNotasLocal = true;
                }
                else
                    msgVersion = $"{App.TipoConexion}";

                if (App.UsarNotasLocal)
                    msgVersion += ". - Usando las notas locales.";
                else
                {
                    string sNotas = "Notas (max. 2048)";
                    if (NotasNETSQLDatabase.UsarNotasMaxConfig)
                        sNotas = "NotasMax";
                    msgVersion += $". - Usando notas remotas: {sNotas}.";
                }
                return msgVersion;
            }
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

        public static string UltimoUsuario { get; set; }
        //{
        //    get 
        //    {
        //        if (Application.Current.Properties["UltimoUsuario"] is null)
        //            return "";
        //        return Application.Current.Properties["UltimoUsuario"].ToString(); 
        //    }
        //    set { Application.Current.Properties["UltimoUsuario"] = value; }
        //}
        public static string UltimoPassword { get; set; }
        //{
        //    get 
        //    {
        //        if (Application.Current.Properties["UltimoPassword"] is null)
        //            return "";
        //        return Application.Current.Properties["UltimoPassword"].ToString(); 
        //    }
        //    set { Application.Current.Properties["UltimoPassword"] = value; }
        //}
        public static bool RecordarUsuario { get; set; }
        //{
        //    get 
        //    {
        //        if (Application.Current.Properties["RecordarUsuario"] is null)
        //            return true;
        //        return (bool)Application.Current.Properties["RecordarUsuario"]; 
        //    }
        //    set { Application.Current.Properties["RecordarUsuario"] = value; }
        //}
        public static bool RecordarPassword { get; set; }
        //{
        //    get 
        //    {
        //        if (Application.Current.Properties["RecordarPassword"] is null)
        //            return false;
        //        return (bool)Application.Current.Properties["RecordarPassword"]; 
        //    }
        //    set { Application.Current.Properties["RecordarPassword"] = value; } 
        //}
        public static bool IniciarConUsuario { get; set; }
        //{
        //    get 
        //    {
        //        if (Application.Current.Properties["IniciarConUsuario"] is null)
        //            return false;
        //        return (bool)Application.Current.Properties["IniciarConUsuario"]; 
        //    }
        //    set { Application.Current.Properties["IniciarConUsuario"] = value; }
        //}
        //
        public static string BuscarTexto { get; set; }
        //{
        //    get 
        //    {
        //        if (Application.Current.Properties["BuscarTexto"] is null)
        //            return "";
        //        return Application.Current.Properties["BuscarTexto"].ToString(); 
        //    }
        //    set { Application.Current.Properties["BuscarTexto"] = value; }
        //}
        public static string BuscarGrupo { get; set; }
        //{
        //    get
        //    {
        //        if (Application.Current.Properties["BuscarGrupo"] is null)
        //            return "";
        //        return Application.Current.Properties["BuscarGrupo"].ToString();
        //    }
        //    set { Application.Current.Properties["BuscarGrupo"] = value; }
        //}
        public static bool BuscarFavoritas { get; set; }
        //{
        //    get 
        //    {
        //        if (Application.Current.Properties["BuscarFavoritas"] is null)
        //            return false;
        //        return (bool)Application.Current.Properties["BuscarFavoritas"]; }
        //    set { Application.Current.Properties["BuscarFavoritas"] = value; }
        //}
        public static bool BuscarArchivadas { get; set; }
        //{
        //    get 
        //    {
        //        if (Application.Current.Properties["BuscarArchivadas"] is null)
        //            return false;
        //        return (bool)Application.Current.Properties["BuscarArchivadas"]; 
        //    }
        //    set { Application.Current.Properties["BuscarArchivadas"] = value; }
        //}
        public static bool BuscarEliminadas { get; set; }
        //{
        //    get 
        //    {
        //        if (Application.Current.Properties["BuscarEliminadas"] is null)
        //            return false;
        //        return (bool)Application.Current.Properties["BuscarEliminadas"]; 
        //    }
        //    set { Application.Current.Properties["BuscarEliminadas"] = value; }
        //}
        public static bool BuscarNotificar { get; set; }
        //{
        //    get 
        //    {
        //        if (Application.Current.Properties["BuscarNotificar"] is null)
        //            return false;
        //        return (bool)Application.Current.Properties["BuscarNotificar"]; 
        //    }
        //    set { Application.Current.Properties["BuscarNotificar"] = value; }
        //}
        // Sinconizar y usar base local
        public static bool SincronizarAuto { get; set; }
        //{
        //    get 
        //    { 
        //        if(Application.Current.Properties["SincronizarAuto"] is null)
        //                return true;
        //        return (bool)Application.Current.Properties["SincronizarAuto"];
        //    }
        //    set { Application.Current.Properties["SincronizarAuto"] = value; }
        //}
        public static bool UsarNotasLocal { get; set; }
        //{
        //    get 
        //    {
        //        if (Application.Current.Properties["UsarNotasLocal"] is null)
        //            return false;
        //        return (bool)Application.Current.Properties["UsarNotasLocal"]; 
        //    }
        //    set { Application.Current.Properties["UsarNotasLocal"] = value; }
        //}
        public static bool Notificar { get; set; }
        //{
        //    get 
        //    {
        //        if (Application.Current.Properties["Notificar"] is null)
        //            return true;
        //        return (bool)Application.Current.Properties["Notificar"]; 
        //    }
        //    set { Application.Current.Properties["Notificar"] = value; }
        //}
        //
        public static string UltimoGrupo { get; set; }
        //{
        //    get 
        //    {
        //        if (Application.Current.Properties["UltimoGrupo"] is null)
        //            return "";
        //        return Application.Current.Properties["UltimoGrupo"].ToString(); 
        //    }
        //    set { Application.Current.Properties["UltimoGrupo"] = value; }
        //}

        //private static bool _UsarNotasMaxConfig;
        //public static bool UsarNotasMaxConfig 
        //{
        //    get 
        //    {
        //        return NotaSQL.UsarNotasMaxConfig;
        //        //return _UsarNotasMaxConfig; 
        //    }
        //    set 
        //    {
        //        //_UsarNotasMaxConfig = value;
        //        //NotaSQL.UsarNotasMaxConfig = _UsarNotasMaxConfig;
        //        NotaSQL.UsarNotasMaxConfig = value;
        //    } 
        //}

        /// <summary>
        /// Leer los datos de configuración.
        /// </summary>
        public static void LeerConfig()
        {
            if (!File.Exists(App.FicConfig))
                return;
            using (var sr = new StreamReader(App.FicConfig, System.Text.Encoding.Default, true))
            {
                string s;
                UltimoUsuario = sr.ReadLine();
                UltimoPassword = sr.ReadLine();
                s = sr.ReadLine();
                RecordarUsuario = s == "1";
                s = sr.ReadLine();
                RecordarPassword = s == "1";
                s = sr.ReadLine();
                IniciarConUsuario = s == "1";
                BuscarTexto = sr.ReadLine();
                BuscarGrupo = sr.ReadLine();
                s = sr.ReadLine();
                BuscarFavoritas = s == "1";
                s = sr.ReadLine();
                BuscarArchivadas = s == "1";
                s = sr.ReadLine();
                BuscarEliminadas = s == "1";
                s = sr.ReadLine();
                BuscarNotificar = s == "1";
                s = sr.ReadLine();
                SincronizarAuto = s == "1";
                s = sr.ReadLine();
                UsarNotasLocal = s == "1";
                s = sr.ReadLine();
                Notificar = s == "1";
                UltimoGrupo = sr.ReadLine();
                //if (!sr.EndOfStream)
                //{
                //    s = sr.ReadLine();
                //    UsarNotasMaxConfig = s == "1";
                //}
                //else
                //    UsarNotasMaxConfig = false;
            }
        }

        /// <summary>
        /// Guardar los datos de configuración.
        /// </summary>
        public static void GuardarConfig()
        {
            using (var sw = new StreamWriter(App.FicConfig, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(UltimoUsuario);
                sw.WriteLine(UltimoPassword);
                sw.WriteLine(RecordarUsuario ? "1": "0");
                sw.WriteLine(RecordarPassword ? "1" : "0");
                sw.WriteLine(IniciarConUsuario ? "1" : "0");
                sw.WriteLine(BuscarTexto);
                sw.WriteLine(BuscarGrupo);
                sw.WriteLine(BuscarFavoritas ? "1" : "0");
                sw.WriteLine(BuscarArchivadas ? "1" : "0");
                sw.WriteLine(BuscarEliminadas ? "1" : "0");
                sw.WriteLine(BuscarNotificar ? "1" : "0");
                sw.WriteLine(SincronizarAuto ? "1" : "0");
                sw.WriteLine(UsarNotasLocal ? "1" : "0");
                sw.WriteLine(Notificar ? "1" : "0");
                sw.WriteLine(UltimoGrupo);
                //sw.WriteLine(UsarNotasMaxConfig ? "1" : "0");
            }
        }

        ///// <summary>
        ///// Crear las propiedades en Application.Properties.
        ///// Pero parece que no siempre funciona, al menos a mi me da esa impresión.
        ///// </summary>
        ///// <remarks>Usar datos guardados en un fichero de texto.</remarks>
        //private void CrearPropiedadesApp()
        //{
        //    if (!Application.Current.Properties.ContainsKey("UltimoUsuario"))
        //        Application.Current.Properties.Add("UltimoUsuario", "");
        //    if (!Application.Current.Properties.ContainsKey("UltimoPassword"))
        //        Application.Current.Properties.Add("UltimoPassword", "");
        //    if (!Application.Current.Properties.ContainsKey("RecordarUsuario"))
        //        Application.Current.Properties.Add("RecordarUsuario", true);
        //    if (!Application.Current.Properties.ContainsKey("RecordarPassword"))
        //        Application.Current.Properties.Add("RecordarPassword",false);
        //    if (!Application.Current.Properties.ContainsKey("IniciarConUsuario"))
        //        Application.Current.Properties.Add("IniciarConUsuario", false);
        //    // Para la búsqueda
        //    if (!Application.Current.Properties.ContainsKey("BuscarTexto"))
        //        Application.Current.Properties.Add("BuscarTexto", "");
        //    if (!Application.Current.Properties.ContainsKey("BuscarGrupo"))
        //        Application.Current.Properties.Add("BuscarGrupo", "");
        //    if (!Application.Current.Properties.ContainsKey("BuscarFavoritas"))
        //        Application.Current.Properties.Add("BuscarFavoritas", false);
        //    if (!Application.Current.Properties.ContainsKey("BuscarArchivadas"))
        //        Application.Current.Properties.Add("BuscarArchivadas", false);
        //    if (!Application.Current.Properties.ContainsKey("BuscarEliminadas"))
        //        Application.Current.Properties.Add("BuscarEliminadas", false);
        //    if (!Application.Current.Properties.ContainsKey("BuscarNotificar"))
        //        Application.Current.Properties.Add("BuscarNotificar", false);
        //    // Para sincronizar las notas
        //    if (!Application.Current.Properties.ContainsKey("SincronizarAuto"))
        //        Application.Current.Properties.Add("SincronizarAuto", true);
        //    if (!Application.Current.Properties.ContainsKey("UsarNotasLocal"))
        //        Application.Current.Properties.Add("UsarNotasLocal", false);
        //    // Notificar las notas marcadas como notificar
        //    if (!Application.Current.Properties.ContainsKey("Notificar"))
        //        Application.Current.Properties.Add("Notificar", true);
        //    // El último grupo indicado en editar notas
        //    if (!Application.Current.Properties.ContainsKey("UltimoGrupo"))
        //        Application.Current.Properties.Add("UltimoGrupo", "");

        //}

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
        /// Mostrar el botón de Paypal si los donativos son menores de 50€. (26/May/21)
        /// </summary>
        public static bool MostrarPayPal
        {
            get { return UsuarioSQL.UsuarioLogin.Pagos < 50; }
        }

        private static string UrlDonativoPayPal = "https://www.paypal.com/donate?hosted_button_id=E48GY5YNX8AMS";

        /// <summary>
        /// Muestra la página para los donativos con PayPal.
        /// </summary>
        public static async Task MostrarDonativoPayPal()
        {
            var uri = new Uri(UrlDonativoPayPal);
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
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

        /// <summary>
        /// Acceder a las notas de SQLLite.
        /// </summary>
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