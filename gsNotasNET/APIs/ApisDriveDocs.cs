//-----------------------------------------------------------------------------
// Biblioteca para acceder a las Notas en Google Drive              (14/Dic/20)
//
// Para usarla con gsNotasNETF                                      (15/Dic/20)
// Utiliza las credenciales de correos.elguille.info@gmail.com
//
// Instalo paquete de Google Docs API:
// Install-Package Google.Apis.Docs.v1 -Version 1.49.0.2170
// Install-Package Google.Apis.Drive.v3 -Version 1.49.0.2166
//
// (c) Guillermo (elGuille) Som, 2020
//-----------------------------------------------------------------------------

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data; // Para el tipo File

using System;
using System.Collections.Generic;
//using System.IO;
using System.Threading;
using System.Text;

namespace gsNotasNET.APIs //gsGoogleDriveDocsAPINET
{
    /// <summary>
    /// Delegado para enviar mensajes de esta clase.
    /// </summary>
    /// <param name="mensaje">El texto a devolver en el evento.</param>
    public delegate void MensajeDelegate(string mensaje);

    /// <summary>
    /// Delegado para avisar de que se inicia o finaliza la operación de guardar documentos.
    /// </summary>
    public delegate void AvisoGuardarNotasDelegate();

    public class ApisDriveDocs
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/docs.googleapis.com-dotnet-quickstart.json
        //static string[] Scopes = { DocsService.Scope.Documents, DocsService.Scope.DocumentsReadonly };
        static string[] Scopes = { DocsService.Scope.Documents, DocsService.Scope.DriveFile };
        static string ApplicationName = App.AppName;

        static DocsService docService;
        static DriveService driveService;

        /// <summary>
        /// Este evento avisará cuando se hayan guardado las notas/documentos en Google Drive.
        /// </summary>
        public static event AvisoGuardarNotasDelegate FinalizadoGuardarNotasEnDrive;

        protected static void OnFinalizadoGuardarNotasEnDrive()
        {
            FinalizadoGuardarNotasEnDrive?.Invoke();
        }

        /// <summary>
        /// Este evento se producirá cuando se inicie la creación de documentos en Google Drive.
        /// </summary>
        public static event AvisoGuardarNotasDelegate IniciadoGuardarNotasEnDrive;

        protected static void OnIniciadoGuardarNotasEnDrive()
        {
            IniciadoGuardarNotasEnDrive?.Invoke();
        }

        /// <summary>
        /// Evento para avisar de los documentos que se van guardando.
        /// </summary>
        public static event MensajeDelegate GuardandoNotas;
        
        protected static void OnGuardandoNotas(string mensaje)
        {
            GuardandoNotas?.Invoke(mensaje);
        }

        /// <summary>
        /// Guarda el contenido del fichero de notas de gsNotasNET en Drive como documentos.
        /// </summary>
        /// <param name="ficNotas">El path de las notas usadas por la aplicación.</param>
        /// <param name="borrarAnt">SI para borrar PERMANENTEMENTE los ficheros existentes o NO para no eliminarlos.</param>
        /// <param name="lasNotas">
        /// Si se indica, será la colección de grupos y notas.
        /// En ese caso, no se leen las notas del fichero.
        /// </param>
        /// <returns>Devuelve el número de documentos (notas) creados.</returns>
        /// <remarks>Si se indica SI, los documentos se eliminarán permanentemente (no van a la papelera).</remarks>
        public static int GuardarNotasDrive(string ficNotas,
                                            string borrarAnt = "NO",
                                            Dictionary<string, List<string>> lasNotas = null)
        {
            OnIniciadoGuardarNotasEnDrive();

            FicNotas = ficNotas;

            UserCredential credential;
            //var credPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            // En Android usar:
            //Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gsNotasNET.db3"
            //var credDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //var credDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //var credDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //var credPath = System.IO.Path.Combine(credDir, ".credentials/gsNotasNET-2.0");
            //// Eliminar la carpeta y el contenido ya que se ve que sigue estando
            //// esto solo para la que había antes
            // Dice que no encuentra parte del path
            // /data/user/0/com.elguille.gsnotasnet/files/.local/share/.credentials/gsNotasNET-2.0
            //System.IO.Directory.Delete(credPath, true);

            // La nueva carpeta para las credenciales, a partir de v1.0.0.10
            var credDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var credPath = System.IO.Path.Combine(credDir, ".credentials/gsNotasNET.Android");

            using (var stream = App.ClientSecretJson)
            {
                //string credPath = "token.json";

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Google Docs API service.
            docService = new DocsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var nombreFolderPred = "gsNotasNET";
            var folderId = ExisteFolder(nombreFolderPred);
            if (string.IsNullOrEmpty(folderId))
                folderId = CrearFolder(nombreFolderPred);

            var colNotas = new Dictionary<string, List<string>>();

            // Si no se indican las notas a guardar en Drive
            // se leerán las del fichero.
            if (lasNotas is null)
                colNotas = LeerNotas();
            else
                colNotas = lasNotas;

            var total = 0;
            foreach (var folderNotas in colNotas.Keys)
            {
                Console.WriteLine();
                var subFolderId = ExisteFolder(folderNotas);
                if (string.IsNullOrEmpty(subFolderId))
                {
                    subFolderId = CrearSubFolder(folderId, folderNotas);
                    //OnGuardandoNotas($"Se ha creado la subcarpeta con el ID: {subFolderId}");
                    OnGuardandoNotas($"Se ha creado la subcarpeta {folderNotas}");
                }
                //else
                //    OnGuardandoNotas($"Ya existe la subcarpeta {folderNotas}, tiene el ID: {subFolderId}");
                //Console.WriteLine();
                if (borrarAnt == "SI")
                {
                    OnGuardandoNotas($"Borrando los documentos de la carpeta '{folderNotas}'...");
                    //Console.WriteLine();
                    var docBorrados = EliminarDocumentos(folderNotas);
                    OnGuardandoNotas($"Se han eliminado {docBorrados} de '{folderNotas}'.");
                    //Console.WriteLine();
                }
                OnGuardandoNotas($"Guardando {colNotas[folderNotas].Count} notas en '{nombreFolderPred}\\{folderNotas}'...");
                var t = 0;
                foreach (var nota in colNotas[folderNotas])
                {
                    t++;
                    total++;
                    var sDoc = $"{folderNotas}_{t:00000}.doc";
                    var fileId = CrearFile(subFolderId, sDoc, "application/vnd.google-apps.document");

                    Document gDoc = new Document();
                    gDoc.DocumentId = fileId;
                    gDoc.Title = sDoc;

                    //Console.WriteLine();
                    //Console.WriteLine($"El documento se creará con el nombre: {gDoc.Title}");
                    //Console.WriteLine();

                    // Añadir el contenido
                    List<Request> requests = new List<Request>();
                    var req = new Request
                    {
                        InsertText = new InsertTextRequest
                        {
                            Location = new Location { Index = 1 },
                            Text = nota
                        }
                    };

                    requests.Add(req);
                    req = new Request
                    {
                        // Asignar el tipo de fuente a Roboto Mono (o el que se indique)
                        UpdateTextStyle = new UpdateTextStyleRequest
                        {
                            TextStyle = new TextStyle
                            {
                                WeightedFontFamily = new WeightedFontFamily
                                {
                                    FontFamily = "Roboto Mono"
                                }
                            },
                            Fields = "*",
                            Range = new Range { StartIndex = 1, EndIndex = nota.Length }
                        }
                    };

                    requests.Add(req);

                    BatchUpdateDocumentRequest body = new BatchUpdateDocumentRequest();
                    body.Requests = requests;
                    var response = docService.Documents.BatchUpdate(body, gDoc.DocumentId).Execute();

                    //OnGuardandoNotas($"Documento creado con el ID: {response.DocumentId}");
                    OnGuardandoNotas($"Documento {gDoc.Title} creado.");
                }
            }
            OnGuardandoNotas($"Se han guardado {total} notas/documentos en Google Drive.");
            OnFinalizadoGuardarNotasEnDrive();

            return total;
        }

        /// <summary>
        /// Comprueba si existe la carpeta indicada.
        /// Si existe devuelve el ID, si no, una cadena vacía.
        /// </summary>
        /// <param name="folderName">El nombre de la carpeta a buscar.</param>
        /// <returns>El ID de la carpeta o una cadena vacía si no existe.</returns>
        public static string ExisteFolder(string folderName)
        {
            var fr = new FilesResource(driveService);
            fr.List().Q = "application/vnd.google-apps.folder";
            var folders = fr.List().Execute();
            foreach (var f in folders.Files)
            {
                // Saltarse los documentos
                if (f.MimeType == "application/vnd.google-apps.document")
                    continue;

                if (f.Name == folderName)
                    return f.Id;
            }
            return "";
        }

        /// <summary>
        /// Crea un directorio en el raíz de Drive.
        /// </summary>
        /// <param name="nombre">El nombre de la carpeta a crear.</param>
        /// <returns>El ID de la carpeta creada.</returns>
        public static string CrearFolder(string nombre)
        {
            File fileMetadata = new File();
            fileMetadata.Name = nombre;
            fileMetadata.MimeType = "application/vnd.google-apps.folder";

            var request = driveService.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();

            return file.Id;
        }

        /// <summary>
        /// Crea un directorio dentro del directorio con el ID indicado.
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="nombre"></param>
        /// <returns></returns>
        public static string CrearSubFolder(string folderId, string nombre)
        {
            File fileMetadata = new File();
            fileMetadata.Name = nombre;
            fileMetadata.MimeType = "application/vnd.google-apps.folder";
            fileMetadata.Parents = new List<string>() { folderId };

            var request = driveService.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();
            return file.Id;
        }

        /// <summary>
        /// Crear un fichero en la carpeta indicada.
        /// </summary>
        /// <param name="folderId">El ID de la carpeta donde se creará el fichero.</param>
        /// <param name="nombre">Nombre del fichero a crear.</param>
        /// <param name="mimeType">El tipo MIME del fichero a crear.</param>
        /// <returns>El ID del fichero creado</returns>
        /// <remarks>Los tipos MIME: https://developers.google.com/drive/api/v3/mime-types</remarks>
        public static string CrearFile(string folderId, string nombre, string mimeType)
        {
            File fileMetadata = new File();
            fileMetadata.Name = nombre;
            fileMetadata.MimeType = mimeType;
            fileMetadata.Parents = new List<string>() { folderId };
            var req = driveService.Files.Create(fileMetadata);
            req.Fields = "id, parents";
            var file = req.Execute();
            return file.Id;
        }

        /// <summary>
        /// Elimina los documentos que haya en la carpeta indicada.
        /// Los nombres de los documentos a eliminar deben empezar con el nombre de la carpeta.
        /// folderName_nombre.doc
        /// </summary>
        /// <param name="folderName">La carpeta con los documentos a eliminar.</param>
        /// <returns>Devuelve el número de documentos eliminados.</returns>
        public static int EliminarDocumentos(string folderName)
        {
            var t = 0;
            var fr = new FilesResource(driveService);
            fr.List().Q = "application/vnd.google-apps.folder";
            var folders = fr.List().Execute();
            var prefix = folderName + "_";
            foreach (var f in folders.Files)
            {
                if (f.MimeType == "application/vnd.google-apps.document" && f.Name.StartsWith(prefix))
                    if (deleteFile(f.Id))
                        t++;
            }
            return t;
        }

        private static bool deleteFile(string fileId)
        {
            try
            {
                driveService.Files.Delete(fileId).Execute();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al eliminar: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Array con los caracteres especiales que se sustituirán al guardar.
        /// </summary>
        private static string[] EspCaracteres { get; set; } = { "\n\r", "\n", "\r", "\"", "<", ">", "&" };

        /// <summary>
        /// Array con las marcas a reemplazar según el caracter especial.
        /// </summary>
        private static string[] EspMarcas { get; set; } = { "|NL|", "|CR|", "|LF|", "|quot|", "|lt|", "|gt|", "|A|" };

        /// <summary>
        /// Cambia las marcas especiales por los caracteres normales.
        /// </summary>
        /// <param name="s">La cadena donde se harán los cambios.</param>
        /// <returns>Una nueva cadena con las marcas sustituidas por los caracteres.</returns>
        /// <remarks>Usar esta función al leer del fichero y mostrarlos correctamente.</remarks>
        private static string espPoner(string s)
        {
            // restablecer los caracteres especiales
            // usar esta función para mostrarlos correctamente
            if (s.StartsWith("|sp|"))
                s = s.Substring(4);
            for (int j = 0; j < EspCaracteres.Length; j++)
                s = s.Replace(EspMarcas[j], EspCaracteres[j]);
            return s;
        }

        private static string _FicNotas;

        /// <summary>
        /// Ubicación del fichero de notas usado por gsNotsNETF.
        /// </summary>
        /// <returns>La ubicación del fichero con las notas.</returns>
        /// <remarks>
        /// Si no está asignado se usa el valor:
        /// DirDocumentos\gsNotasNETF.notasUC.txt
        /// </remarks>
        public static string FicNotas 
        {
            get { return _FicNotas; }
            private set 
            {
                if (string.IsNullOrEmpty(value))
                {
                    var DirDocumentos = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    _FicNotas = System.IO.Path.Combine(DirDocumentos, "gsNotasNETF.notasUC.txt");
                }
                else
                    _FicNotas = value;
            }
        }

        /// <summary>
        /// Lee las notas del programa gsNotasNETF.
        /// </summary>
        /// <returns>Devuelve una colección con los grupos y notas de cada grupo.</returns>
        public static Dictionary<string, List<string>> LeerNotas()
        {
            var path = FicNotas;

            var colNotas = new Dictionary<string, List<string>>();

            using (var sr = new System.IO.StreamReader(path, Encoding.UTF8, true))
            {
                while (!sr.EndOfStream)
                {
                    var s = sr.ReadLine();
                    if (!string.IsNullOrEmpty(s))
                    {
                        var s1 = s.TrimStart();
                        // Ignorar los comentarios #
                        if (s1.StartsWith("#"))
                            continue;

                        // Si es un grupo
                        if (s1.StartsWith("g:", StringComparison.OrdinalIgnoreCase))
                        {
                            var i = s.IndexOf("g:", StringComparison.OrdinalIgnoreCase);
                            if (i == -1) continue;

                            var g = s.Substring(i + 2).Trim();
                            // Es un grupo
                            // si ya existe, se ignora, pero se leen las notas y se continúa
                            var existe = false;
                            if (colNotas.ContainsKey(g))
                                existe = true;
                            else
                                colNotas.Add(g, new List<string>());

                            // Leer las notas
                            while (!sr.EndOfStream)
                            {
                                s = sr.ReadLine();
                                if (string.IsNullOrEmpty(s))
                                    continue;

                                s1 = s.TrimStart();
                                if (!s1.StartsWith("gfin:", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Al guardar en documentos no es necesario
                                    // cambiar el contenido
                                    if (!existe)
                                        colNotas[g].Add(espPoner(s));
                                }
                                else
                                    break;
                            }
                        }
                    }
                }
                sr.Close();
            };
            return colNotas;
        }
    }
}
#if ESDLL
namespace System.Runtime.CompilerServices
{
    public class IsExternalInit { }
}
#endif
