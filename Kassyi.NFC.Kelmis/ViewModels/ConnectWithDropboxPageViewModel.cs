using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Dropbox.Api;
using Kassyi.NFC.Kelmis.Services;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kassyi.NFC.Kelmis.Models
{

    class ConnectWithDropboxPageViewModel
    {
        public ReactiveProperty<string> AccessToken { get; } = new ReactiveProperty<string>();

        public ReactiveCommand CancelCommand { get; private set; } = new ReactiveCommand();


        public ConnectWithDropboxPageViewModel()
        {
            CancelCommand.Subscribe(i =>
            {
                var rootFlame = Window.Current.Content as Frame;
                rootFlame.Navigate(typeof(MainPage));
            });
        }

        public Uri AuthorizeUri => DropboxOAuth2Helper.GetAuthorizeUri(
                OAuthResponseType.Token,
                SettingsService.Current.AppKey,
                SettingsService.Current.RedirctUrl);


    }
}
