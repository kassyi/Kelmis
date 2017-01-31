using Dropbox.Api;
using Dropbox.Api.Files;
using Kassyi.NFC.Kelmis.Services;
using Kassyi.NFC.Kelmis.Strings;
using Pcsc;
using Pcsc.Common;
using Reactive.Bindings;
using StrawhatNet.NFC.FelicaReader.Suica;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.SmartCards;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Kassyi.NFC.Kelmis.Models
{
    public enum ScaningStatus
    {
        Disabled,
        Scaning,
        Error
    }

    public sealed partial class ScanSFCardPageModel : IDisposable
    {
        public ReactiveProperty<ScaningStatus> Status { get; } = new ReactiveProperty<ScaningStatus>();
        public ReactiveProperty<string> StatusMessage { get; } = new ReactiveProperty<string>() { Value = Resources.Hyphen };
        public bool IsPlaySe { get; set; } = true;

        private SmartCardReader cardReader;
        private CoreDispatcher dispatcher;
        private MemoryRandomAccessStream succsesSeStream;
        private MemoryRandomAccessStream errorSeStream;
        //ステートマシン。ネストしたクラスとして閉じ込めて定義してるのでこのクラスのprivateメソッドにもアクセスできる。
        //変数には一切触っていない。
        private State stateMachine = State.BeforeInitializeState;

        public ScanSFCardPageModel(CoreDispatcher d)
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(ScanSFCardPageModel));

            dispatcher = d;
            //初回時のみcontextからキックする。ステートマシン内に閉じ込められなかった
            stateMachine.Enter(this);
        }

        public async void BeginScan()
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(ScanSFCardPageModel));

            stateMachine = await stateMachine.Handle(new InitializeEvent(), this);
        }


        public void EndScan()
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(ScanSFCardPageModel));

            if (cardReader != null)
            {
                cardReader.CardAdded -= CardAddedHandler;
                cardReader.CardRemoved -= CardRemovedHandler;
                cardReader = null;
            }
            StatusMessage.Value = Resources.Disabled;
            Status.Value = ScaningStatus.Disabled;
        }

        struct ActionResult
        {
            public bool Success;
            public string Message;
        }


        #region ステートマシンからコールバックされるアクション内容本体

        /// <summary>
        /// 初期化し、読み取りを開始します。
        /// </summary>
        /// <returns>初期化に成功したか</returns>
        async Task<ActionResult> InitializeAsync()
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(ScanSFCardPageModel));

            //チェック
            var deviceInfo = await SmartCardReaderUtils.GetFirstSmartCardReaderInfo(SmartCardReaderKind.Nfc)
                ?? await SmartCardReaderUtils.GetFirstSmartCardReaderInfo(SmartCardReaderKind.Any); ;

            if (deviceInfo == null || !deviceInfo.IsEnabled)
            {
                return new ActionResult() { Success = false, Message = Resources.CanNotUseReader };
            }

            if (cardReader == null)
            {
                cardReader = await SmartCardReader.FromIdAsync(deviceInfo.Id);
                cardReader.CardAdded += CardAddedHandler;
                cardReader.CardRemoved += CardRemovedHandler;
            }
            succsesSeStream = await LoadSound(new Uri("ms-appx:///Assets/Success.wav"));
            errorSeStream = await LoadSound(new Uri("ms-appx:///Assets/Error.wav"));
            return new ActionResult() { Success = true };
        }

        async Task<ActionResult> ReadSuicayAsync(CardAddedEventArgs args)
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(ScanSFCardPageModel));

            var smartCard = args.SmartCard;
            SuicaCard suica = null;

            try
            {
                using (SmartCardConnection connection = await smartCard.ConnectAsync())
                {
                    IccDetection cardIdentification = new IccDetection(smartCard, connection);
                    await cardIdentification.DetectCardTypeAync();

                    if (cardIdentification.PcscDeviceClass == DeviceClass.StorageClass
                        && cardIdentification.PcscCardName == CardName.FeliCa)
                    {
                        var reader = new SuicaReader(connection);
                        suica = await reader.ReadSuicaAsync(connection);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return new ActionResult() { Success = false, Message = Resources.TouchIcCardAgain };
            }
            //履歴4件未満
            if (suica == null)
                return new ActionResult() { Success = false, Message = Resources.HistoryIsNotEnough };

            //DBに追加
            KelmisLogDb.Current.AddLog(suica.Recodes.Select(r => new KelmisLogRecode(r, suica.Idm)));
            await BackupDatabaseToDropbox();

            return new ActionResult() { Success = true };
        }

        private static async Task BackupDatabaseToDropbox()
        {
            if (string.IsNullOrEmpty(SettingsService.Current.DropboxAccessToken.Value))
                return;

            var client = new DropboxClient(SettingsService.Current.DropboxAccessToken.Value);
            using (var stream = System.IO.File.Open(KelmisLogDb.DbFilePath, System.IO.FileMode.Open))
            {
                await client.Files.UploadAsync(SettingsService.Current.DatabaseBackupRemotePath, WriteMode.Overwrite.Instance, body: stream);
            }
        }

        void PlaySuccessSe()
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(ScanSFCardPageModel));

            if (IsPlaySe)
            {
                PlaySeAsync(succsesSeStream);
            }

        }

        void PlayErrorSe()
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(ScanSFCardPageModel));

            if (IsPlaySe)
            {
                PlaySeAsync(errorSeStream);
            }
        }

        #endregion ステートマシンからコールバックされるアクション内容本体

        private async Task<MemoryRandomAccessStream> LoadSound(Uri uri)
        {
            // wav ファイルを先読みしてメモリ上に置いておく方法 (MemoryRandomAccessStream クラスを使用)
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

            var buff = new byte[stream.Size];
            var ibuffer = buff.AsBuffer();
            await stream.ReadAsync(ibuffer, (uint)stream.Size, Windows.Storage.Streams.InputStreamOptions.None);
            return new MemoryRandomAccessStream(buff);
        }

        private async void PlaySeAsync(MemoryRandomAccessStream buff)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var media = new MediaElement();
                media.SetSource(buff, "sound/wav");
                media.Play();
            });
        }

        private void CardAddedHandler(SmartCardReader sender, CardAddedEventArgs args)
            => Task.Run(async () => { stateMachine = await stateMachine.Handle(new CardAddedEvent(args), this); });
        private void CardRemovedHandler(SmartCardReader sender, CardRemovedEventArgs args)
            => Task.Run(async () => { stateMachine = await stateMachine.Handle(new CardRemovedEvent(args), this); });



        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    EndScan();
                }

                if (succsesSeStream != null)
                    succsesSeStream.Dispose();
                if (errorSeStream != null)
                    errorSeStream.Dispose();
                disposedValue = true;

            }
        }

        ~ScanSFCardPageModel()
        {
            Dispose(false);
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


    }
}
