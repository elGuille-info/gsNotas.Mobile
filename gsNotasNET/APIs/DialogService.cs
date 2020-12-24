using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using gsNotasNET.Data;
using Xamarin.Essentials;
using gsNotasNET.Models;
using System.Linq;
using System.Diagnostics;

namespace gsNotasNET.APIs
{
    public class DialogService
    {
        /*
        Adaptado de:
        https://stackoverflow.com/a/61345151/14338047
        Allows you to provide a call back method that will be called after user dismiss the alert box
        - for example navigates back to the previous page.
        */
        public static async Task ShowErrorAsync(string title, string message, string buttonOK, Action CallBackAferHide)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, buttonOK, "-");
            CallBackAferHide?.Invoke();
        }
    }
}
