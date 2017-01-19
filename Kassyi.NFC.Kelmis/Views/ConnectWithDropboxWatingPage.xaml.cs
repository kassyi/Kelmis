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
using Kassyi.NFC.Kelmis.Models;
using Windows.System;


// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Kassyi.NFC.Kelmis.Views
{

    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class ConnectWithDropboxWatingPage : Page
    {
        ConnectWithDropboxPageViewModel viewModel = new ConnectWithDropboxPageViewModel();
        public ConnectWithDropboxWatingPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null && e.Parameter is string)
            {
                viewModel.AccessToken.Value = (string)e.Parameter;
                //TODO: 処理
            }
            else
            {
                await Launcher.LaunchUriAsync(viewModel.AuthorizeUri);
            }
        }
    }
}
