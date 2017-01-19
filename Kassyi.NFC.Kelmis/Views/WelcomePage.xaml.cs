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

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Kassyi.NFC.Kelmis.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            this.InitializeComponent();
        }

        private void LanchAuthPageBtn_Click(object sender, RoutedEventArgs e)
        {
            var rootFlame = Window.Current.Content as Frame;
            rootFlame.Navigate(typeof(ConnectWithDropboxWatingPage));
        }

        private void LancthMainPageBtn_Click(object sender, RoutedEventArgs e)
        {
            var rootFlame = Window.Current.Content as Frame;
            rootFlame.Navigate(typeof(MainPage));
        }
    }
}
