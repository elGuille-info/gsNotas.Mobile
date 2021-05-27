using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Foundation;

using gsNotasNET;

using UIKit;

namespace gsNotasNET.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            var gsApp = new App();

            // Ponerlo después del new App()
            App.AppName = "gsNotas.iOS";

            // Leer los valores encriptados                         (27/May/21)
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


            //LoadApplication(new App());
            LoadApplication(gsApp);

            return base.FinishedLaunching(app, options);
        }
    }
}


