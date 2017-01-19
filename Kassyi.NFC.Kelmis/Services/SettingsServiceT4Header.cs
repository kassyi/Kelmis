
//
//T4 Genelated
//
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using Windows.Storage;
using System.Linq;

namespace Kassyi.NFC.Kelmis.Services
{
    partial class SettingsService
    {
			public ReactiveProperty<System.Boolean> BatchMode => SettingStorage.Current.ToReactivePropertyAsSynchronized(x => x.BatchMode);
    		public ReactiveProperty<System.Boolean> PlaySe => SettingStorage.Current.ToReactivePropertyAsSynchronized(x => x.PlaySe);
    		public ReactiveProperty<System.String> DropboxAccessToken => SettingStorage.Current.ToReactivePropertyAsSynchronized(x => x.DropboxAccessToken);
    		public ReactiveProperty<System.Boolean> IsInitializedApp => SettingStorage.Current.ToReactivePropertyAsSynchronized(x => x.IsInitializedApp);
    			public System.String AppKey {get;} = "vdji8m5sqnrpp94";
    		public System.String RedirctUrl {get;} = "https://kelmis-app.blogspot.jp/redirect.html";
    		public System.String DatabaseBackupRemotePath {get;} = "/Backup/SuicaHistory.db";
    
        /// <summary>
        /// INotifyPropertyChangedを実装したApplicationDataContainerのラッパー
        /// </summary>
        class SettingStorage : INotifyPropertyChanged
        {
            public static SettingStorage Current { get; } = new SettingStorage();
            public event PropertyChangedEventHandler PropertyChanged;

            private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings.CreateContainer("kelmis", ApplicationDataCreateDisposition.Always);
		            public System.Boolean BatchMode
            {
			                get
				{ 
							if(!settings.Values.ContainsKey(nameof(BatchMode)))
						return false;
					var v = settings.Values[nameof(BatchMode)];
					return v == null ? false : (System.Boolean)v;
						}
		                set
                {
                    var key = nameof(BatchMode);
                    if (settings.Values.ContainsKey(key) && (System.Boolean)(settings.Values[key]) == value)
                        return;
                    settings.Values[key] = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(key));
                }
		            }
                     public System.Boolean PlaySe
            {
			                get
				{ 
							if(!settings.Values.ContainsKey(nameof(PlaySe)))
						return true;
					var v = settings.Values[nameof(PlaySe)];
					return v == null ? true : (System.Boolean)v;
						}
		                set
                {
                    var key = nameof(PlaySe);
                    if (settings.Values.ContainsKey(key) && (System.Boolean)(settings.Values[key]) == value)
                        return;
                    settings.Values[key] = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(key));
                }
		            }
                     public System.String DropboxAccessToken
            {
			                get
				{ 
							if(!settings.Values.ContainsKey(nameof(DropboxAccessToken)))
						return "";
					var v = settings.Values[nameof(DropboxAccessToken)];
					return v == null ? "" : (System.String)v;
						}
		                set
                {
                    var key = nameof(DropboxAccessToken);
                    if (settings.Values.ContainsKey(key) && (System.String)(settings.Values[key]) == value)
                        return;
                    settings.Values[key] = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(key));
                }
		            }
                     public System.String AppKey
            {
			                get
				{ 
							return "vdji8m5sqnrpp94";
						}
		            }
                     public System.String RedirctUrl
            {
			                get
				{ 
							return "https://kelmis-app.blogspot.jp/redirect.html";
						}
		            }
                     public System.Boolean IsInitializedApp
            {
			                get
				{ 
							if(!settings.Values.ContainsKey(nameof(IsInitializedApp)))
						return false;
					var v = settings.Values[nameof(IsInitializedApp)];
					return v == null ? false : (System.Boolean)v;
						}
		                set
                {
                    var key = nameof(IsInitializedApp);
                    if (settings.Values.ContainsKey(key) && (System.Boolean)(settings.Values[key]) == value)
                        return;
                    settings.Values[key] = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(key));
                }
		            }
                     public System.String DatabaseBackupRemotePath
            {
			                get
				{ 
							return "/Backup/SuicaHistory.db";
						}
		            }
                     

            private SettingStorage() { }
        }
    }
}

