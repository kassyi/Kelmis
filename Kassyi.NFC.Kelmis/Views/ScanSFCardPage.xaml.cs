using Kassyi.NFC.Kelmis.Models;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Kassyi.NFC.Kelmis.Views
{


    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class ScanSFCardPage : Page
    {
        public ScanSFCardPageModel ViewModel { get; private set; }


        public ScanSFCardPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = new ScanSFCardPageModel(Dispatcher);
            ViewModel.BeginScan();
            ViewModel.Status.PropertyChanged += GoToState;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.Status.PropertyChanged -= GoToState;
            ViewModel.EndScan();
            ViewModel.Dispose();
            ViewModel = null;
        }

        private void GoToState(object sender, PropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, $"{ViewModel.Status.Value.ToString()}State", true); 
        }
    }
}
