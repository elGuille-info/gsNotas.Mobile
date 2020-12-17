using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Android.App;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("gsNotasNET.Android")]
[assembly: AssemblyDescription("Utilidad para tomar notas y guardarlas en el dispositivo. (revisión del 17-dic-2020)")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("elGuille")]
[assembly: AssemblyProduct("gsNotasNET.Android")]
[assembly: AssemblyCopyright("Copyright © Guillermo Som (elGuille), 2020")]
[assembly: AssemblyTrademark("Creado con Xamarin.Forms y .NET Standard 2.0")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
[assembly: AssemblyVersion("1.0.0.7")]
[assembly: AssemblyFileVersion("1.0.0.7")]

// Add some common permissions, these can be removed if not needed
[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]
