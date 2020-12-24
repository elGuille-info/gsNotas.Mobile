using System;
using System.IO;
using Xamarin.Forms;
using gsNotasNET.Data;
using System.Threading.Tasks;
using Xamarin.Essentials;
using gsNotasNET.Models;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Net.Mail;

namespace gsNotasNET
{
    public partial class App : Application
    {
        /// <summary>
        /// La versión de la aplicación
        /// </summary>
        public static string AppVersion { get; } = "v2..24";

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

            // Iniciar con la página Login que vuelva a MainMenu.
            MainPage = new NavigationPage(new Login(new MainMenu()));
        }

        public static string UltimoUsuario
        {
            get { return Application.Current.Properties["UltimoUsuario"].ToString(); }
            set { Application.Current.Properties["UltimoUsuario"] = value; }
        }
        public static string UltimoPassword
        {
            get { return Application.Current.Properties["UltimoPassword"].ToString(); }
            set { Application.Current.Properties["UltimoPassword"] = value; }
        }
        public static bool RecordarUsuario
        {
            get { return (bool)Application.Current.Properties["RecordarUsuario"]; }
            set { Application.Current.Properties["RecordarUsuario"] = value; }
        }
        public static bool RecordarPassword 
        {
            get { return (bool)Application.Current.Properties["RecordarPassword"]; }
            set { Application.Current.Properties["RecordarPassword"] = value; } 
        }

        private void CrearPropiedadesApp()
        {
            if (!Application.Current.Properties.ContainsKey("UltimoUsuario"))
                Application.Current.Properties.Add("UltimoUsuario", "");
            if (!Application.Current.Properties.ContainsKey("UltimoPassword"))
                Application.Current.Properties.Add("UltimoPassword", "");
            if (!Application.Current.Properties.ContainsKey("RecordarUsuario"))
                Application.Current.Properties.Add("RecordarUsuario", false);
            if (!Application.Current.Properties.ContainsKey("RecordarPassword"))
                Application.Current.Properties.Add("RecordarPassword",false);
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
            sb.AppendLine($"Indicalo en la aplicación '{App.AppName} {App.AppVersion}'.{App.crlf}");
            sb.AppendLine($"Gracias.{App.crlf}Guillermo.");
            sb.AppendLine();
            sb.AppendLine($"Enviado a: {email}");
            sb.AppendLine();
            sb.AppendFormat("Para validar el correo puedes indicar el código de validación durante las {0} horas (UTC).", DateTime.UtcNow.Hour);
            sb.AppendLine();
            sb.AppendLine("Cualquier bug o duda sobre el programa, por favor responde a este email.");
            sb.AppendLine("Gracias.");
            sb.AppendLine();

            // Enviar el código de validación al email
            await SendEmail("Codigo de validacion", sb.ToString(), email);

            //return new Task<string>(() => hash);
            return hash;
        }

        // https://www.c-sharpcorner.com/article/xamarin-forms-send-email-using-smtp2/
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
                //DisplayAlert("Faild", ex.Message, "OK");
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

        // Esto lo manda desde la cuenta que esté definida en el dispositivo.
        //public static async Task SendEmail(string subject, string body, string emailTo, string emailCc)
        //{
        //    try
        //    {
        //        var message = new EmailMessage
        //        {
        //            Subject = subject,
        //            Body = body,
        //            To = new List<string>() { emailTo },
        //            BodyFormat = EmailBodyFormat.PlainText,
        //            Cc = new List<string>() { emailCc },
        //            //Bcc = bccRecipients
        //        };
        //        await Email.ComposeAsync(message);
        //    }
        //    catch (FeatureNotSupportedException fbsEx)
        //    {
        //        // Email is not supported on this device
        //        Debug.WriteLine(fbsEx.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Some other exception occurred
        //        Debug.WriteLine(ex.Message);
        //    }
        //}

        public static readonly string crlf = "\n\r";

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
                    database = new NotasDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gsNotasNET.db3"));
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