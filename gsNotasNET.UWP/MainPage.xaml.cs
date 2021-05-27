using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace gsNotasNET.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            var gsApp = new gsNotasNET.App();
            
            // Ponerlo después de crear la instancia
            gsNotasNET.App.AppName = "gsNotas.UWP";

            // Leer los valores encriptados                         (22/May/21)
            StreamReader str;
            using (str = new StreamReader("encrypted-string-client_secret_50463492690.json.txt"))
            {
                gsNotasNET.App.ClientSecretJson = str.BaseStream;
            }
            using (str = new StreamReader("encrypted-string.txt"))
            {
                gsNotasNET.App.CredencialesSQL = str.BaseStream;
            }
            using (str = new StreamReader("encrypted-string-guille.txt"))
            {
                gsNotasNET.App.CredencialesGuille = str.BaseStream;
            }
            using (str = new StreamReader("encrypted-string-correos-elguille.txt"))
            {
                gsNotasNET.App.CredencialesCorreosGuille = str.BaseStream;
            }

            LoadApplication(gsApp);
        }
    }
}
