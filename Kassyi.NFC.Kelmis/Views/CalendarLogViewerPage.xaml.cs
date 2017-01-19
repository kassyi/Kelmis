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
using Reactive.Bindings;
using Kassyi.NFC.Kelmis.Models;
using Kassyi.NFC.Kelmis.Model;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Kassyi.NFC.Kelmis.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class CalendarLogViewerPage : Page
    {
        CalendarLogViewerPageViewModel model { get; } = new CalendarLogViewerPageViewModel();

        public CalendarLogViewerPage()
        {
            this.InitializeComponent();
            //todo: bind
            foreach (var idm in KelmisLogDb.Current.GetSaveedCardIdms())
            {
                cardSelecter.Items.Add(idm);
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
