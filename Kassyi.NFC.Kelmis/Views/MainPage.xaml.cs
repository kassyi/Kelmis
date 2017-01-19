using Kassyi.NFC.Kelmis.Views;
using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace Kassyi.NFC.Kelmis
{
    /// <summary>
    /// 各ページへの遷移をコントロールします
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //遷移先ページ名とメニューの｢インスタンス｣との対応
        Dictionary<string, RadioButton> menus;
        //ラジオボタン=メニューの後ろにつけたグループ名
        const string menuGropName = "RadioBtn";

        public MainPage()
        {
            this.InitializeComponent();
            menus = new Dictionary<string, RadioButton>{
                { nameof(ScanSFCardPage), ScanSFCardPageRadioBtn },
                { nameof(CalendarLogViewerPage),HistoryPageRadioBtn },
                { nameof(CalendarPage),CalendarPageRadioBtn },
                { nameof(SettingPage),SettingPageRadioBtn },
            };
        }

        //イベントハンドラ
        private void MenuTaped(object sender, RoutedEventArgs e)
        {
            var radioBtn = sender as RadioButton;
            //暫定的に
            var nameSpage = typeof(ScanSFCardPage).Namespace;
            var targetPageName = radioBtn.Name.Replace(menuGropName,"");
            MainContentFrame.Navigate(Type.GetType($"{nameSpage}.{targetPageName}"));
            Splitter.IsPaneOpen = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ScanSFCardPageRadioBtn.IsChecked = true;
            SystemNavigationManager.GetForCurrentView()
              .BackRequested += MainPage_BackRequested;

            MainContentFrame.Navigated += MainContentFrame_Navigated;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            MainContentFrame.Navigated -= MainContentFrame_Navigated;

            SystemNavigationManager.GetForCurrentView()
              .BackRequested -= MainPage_BackRequested;
        }

        // システムの［戻る］ボタンが押された時のイベントハンドラー 
        private void MainPage_BackRequested(object sender,BackRequestedEventArgs e)
        {
            if (MainContentFrame.CanGoBack)
            {
                MainContentFrame.GoBack();
                e.Handled = true;
            }
        }

        // コンテンツを表示するFrame内で画面遷移したときのイベントハンドラー 
        private void MainContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (var x in menus.Values)
            {
                x.IsChecked = false;
            }
            menus[e.Content.GetType().Name].IsChecked = true;

            if (e.Content.GetType() == typeof(ScanSFCardPage)) 
              MainContentFrame.BackStack.Clear(); 

            // ウィンドウ左上の [←] ボタンの表示を制御する 
            SystemNavigationManager.GetForCurrentView()
              .AppViewBackButtonVisibility
              = MainContentFrame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        }
    }
}
