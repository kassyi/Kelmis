using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace StrawhatNet.NFC.FelicaReader.Suica
{
    public enum TerminalIdEnum
    {
        精算機 = 3,
        携帯型端末 = 4,
        車載端末 = 5,
        券売機1 = 7,
        券売機2 = 8,
        入金機 = 9,
        券売機3 = 18,
        券売機等1 = 20,
        券売機等2 = 21,
        改札機 = 22,
        簡易改札機 = 23,
        窓口端末1 = 24,
        窓口端末2 = 25,
        改札端末 = 26,
        携帯電話 = 27,
        乗継精算機 = 28,
        連絡改札機 = 29,
        簡易入金機 = 31,
        ViewAltte1 = 70,
        ViewAltte2 = 72,
        物販端末 = 199,
        自販機 = 200,
    }

    public enum ProsessIdEnum
    {
        運賃支払_改札出場 = 1,
        チャージ = 2,
        券購_磁気券購入 = 3,
        精算 = 4,
        精算_入場精算 = 5,
        窓出_改札窓口処理 = 6,
        新規_新規発行 = 7,
        控除_窓口控除 = 8,
        バス_PiTaPa系 = 13,
        バス_IruCa系 = 15,
        再発_再発行処理 = 17,
        支払_新幹線利用 = 19,
        入A_入場時オートチャージ = 20,
        出A_出場時オートチャージ = 21,
        入金_バスチャージ = 31,
        券購_バス路面電車企画券購入 = 35,
        物販 = 70,
        ポイントチャージ = 72,
        入金_レジ入金 = 73,
        物販取消 = 74,
        入物_入場物販 = 75,
        物現_現金併用物販 = 198,
        入物_入場現金併用物販 = 203,
        精算_他社精算 = 132,
        精算_他社入場精算 = 133,
    }
    public class UsageCategoryString
    {
        public const string ProductSales = "物販";
        public const string Bus = "バス";
        public const string Train = "鉄道";
    }

    /// <summary>
    /// 各種IDから日本語表現を取得するクラス。DBも扱う
    /// </summary>
    public class StationStringDb : IDisposable
    {
        private readonly static Dictionary<int, string> TerminalCodes;
        private readonly static Dictionary<int, string> ProsessCodes;
        private readonly static Dictionary<string, string> BusCumLineCodes;
        private static SQLiteConnection DbConnection;
        private static readonly StationStringDb instance = new StationStringDb();
        public const string DefaultString = "不明";

        public static StationStringDb Instance
        {
            get
            {
                if (instance.disposedValue)
                    throw new ObjectDisposedException(typeof(StationStringDb).Name);
                return instance;
            }
        }

        private StationStringDb()
        {
            var asmName = GetType().GetTypeInfo().Assembly.GetName().Name;

            //DBオープン
            DbConnection = new SQLiteConnection(
                new SQLitePlatformWinRT(),
                Path.Combine(Package.Current.InstalledLocation.Path,
                    asmName,
                    "suica_stationcode.db"),
                SQLiteOpenFlags.ReadOnly);
        }

        public string GetOrDefaultTerminalIdString(int id)
        {
            return TerminalCodes.GetOrDefault(id, DefaultString);
        }

        public string GetOrDefaultProsessIdString(int id)
        {
            return ProsessCodes.GetOrDefault(id, DefaultString);
        }

        public string GetOrDerDefaultBusCompanyName(string id)
        {
            return BusCumLineCodes.GetOrDefault(id, DefaultString);
        }


        /// <summary>
        /// 駅のレコードを検索するためのエリアコードを算出します。
        /// </summary>
        /// <param name="lineCode"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public byte GetAreaCode(string lineCode, byte region)
        {
            return GetAreaCode(byte.Parse(lineCode, System.Globalization.NumberStyles.HexNumber), region);
        }

        /// <summary>
        /// 駅のレコードを検索するためのエリアコードを算出します。
        /// </summary>
        /// <param name="lineCode"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public static byte GetAreaCode(byte lineCode, byte region)
        {
            var areacodeSoruce = (byte)(0x3 & region >> 4);
            if ((lineCode & 0xFF) < 128)
                return 0;
            if (region >= 0 && region <= 3)
                return (byte)(region + 1);
            return 0;
        }

        /// <summary>
        /// 駅のレコード（複数）を取得します。
        /// </summary>
        /// <param name="lineCode">16進（小文字）の路線コード</param>
        /// <returns></returns>
        public TableQuery<Station> SearchStation(Expression<Func<Station, bool>> predExpr)
        {
            return DbConnection.Table<Station>().Where(predExpr);
        }

        /// <summary>
        /// 入線の駅を検索します。
        /// </summary>
        /// <param name="recode"></param>
        /// <returns></returns>
        public TableQuery<Station> SearchInStation(SuicaRecode recode)
        {
            return SearchStation(GetAreaCode(recode.InLineCode, recode.Region), recode.InLineCode, recode.InStationCode);
        }

        /// <summary>
        /// 出線の駅を検索します。
        /// </summary>
        /// <param name="recode"></param>
        /// <returns></returns>
        public TableQuery<Station> SearchOutStation(SuicaRecode recode)
        {
            return SearchStation(GetAreaCode(recode.OutLineCode, recode.Region), recode.OutLineCode, recode.OutStationCode);
        }

        public TableQuery<Station> SearchStation(byte areaCode, string lineCode, string stationCode)
        {
            return DbConnection.Table<Station>().Where(s => s.AreaCode == areaCode && s.LineCode == lineCode && s.StationCode == stationCode);
        }

        

        static StationStringDb()
        {

            TerminalCodes = new Dictionary<int, string>(){
{3 , "のりこし精算機"},
{4 , "携帯型端末"},
{5 , "車載端末"},
{7 , "カード発売機"},
{8 , "自動券売機"},
{9 , "入金機"},
{18 , "自動券売機"},
{20 , "駅務機器"},
{21 , "定期券発売機"},
{22 , "自動改札機"},
{23 , "簡易改札機"},
{24 , "窓口端末"},
{25 , "窓口端末（みどりの窓口）"},
{26 , "窓口端末（有人改札）"},
{27 , "モバイルSuica"},
{28 , "乗継精算機"},
{29 , "連絡改札機"},
{31 , "簡易入金機"},
{70 , "VIEW ALTTE"},
{72 , "VIEW ALTTE"},
{199 , "物販端末"},
{200 , "自販機"},
            };

            ProsessCodes = new Dictionary<int, string>() {
{1 , "運賃支払（改札出場）"},
{2 , "チャージ"},
{3 , "磁気券購入"},
{4 , "精算"},
{5 , "精算（入場精算）"},
{6 , "窓口出場（改札窓口処理）"},
{7 , "新規（新規発行)"},
{8 , "控除（窓口控除)"},
{13 , "バス等（均一運賃）"},
{15 , "バス等"},
{17 , "再発行  (再発行処理)"},
{19 , "支払 (新幹線利用)"},
{20 , "入場時オートチャージ"},
{21 , "出場時オートチャージ"},
{23 , "出場時オートチャージ"},
{27 , "バス等"},
{29 , "バス等"},
{31 , "バスチャージ"},
{35 , "バス路面電車企画券購入"},
{70 , "物販"},
{72 , "ポイントチャージ"},
{73 , "レジチャージ"},
{74 , "物販取消"},
{75 , "入場物販)"},
{198 , "現金併用物販"},
{203 , "入場現金併用物販"},
{132 , "精算 (他社精算)"},
{133 , "精算 (他社入場精算)"},
            };

            BusCumLineCodes = new Dictionary<string, string>()
            {
{"5D1","広島電鉄"},
{"5E1","岡山電気軌道"},
{"A01","西日本鉄道"},
{"C2A","京成バス"},
{"C2D","船橋新京成バス"},
{"C2E","習志野新京成バス"},
{"C2F","松戸新京成バス"},
{"C37","東京ベイシティ交通"},
{"C3C","ちばフラワーバス"},
{"C3D","ちばグリーンバス"},
{"C42","平和交通"},
{"C43","あすか交通"},
{"C4A","東京都交通局"},
{"C4B","東急バス"},
{"C4C","京王バス"},
{"C4D","関東バス"},
{"C4E","西武バス"},
{"C4F","国際興業バス"},
{"C50","小田急バス"},
{"C51","東武バス"},
{"C52","立川バス"},
{"C53","西東京バス"},
{"C57","フジエクスプレス"},
{"C5A","小田急箱根高速バス"},
{"C5B","ジェイアールバス関東"},
{"C5C","日立自動車交通"},
{"C62","多摩バス"},
{"C69","京浜急行バス"},
{"C6A","横浜市交通局"},
{"C6B","神奈川中央交通"},
{"C6C","川崎鶴見臨港バス"},
{"C6D","箱根登山バス"},
{"C6E","伊豆箱根バス"},
{"C6F","江ノ電バス"},
{"C70","川崎市バス"},
{"C71","相鉄バス"},
{"C72","富士急湘南バス"},
{"C74","江ノ電バス藤沢"},
{"C7C","相鉄バス"},
{"C7E","山梨交通"},
{"C80","富士急行"},
{"C81","富士急山梨バス"},
{"C82","富士急平和観光バス"},
{"C83","富士急シティバス"},
{"C84","富士急静岡バス"},
{"C85","東京急行電鉄"},
{"D18","しずてつジャストライン"},
{"E16","京阪バス"},
{"E1F","大阪市交通局"},
{"E20","高槻市交通局"},
{"E22","阪急バス"},
{"E29","大阪空港交通"},
{"E31","阪神電気鉄道"},
{"E33","神姫バス"},
{"E38","神戸市交通局"},
{"E51","奈良交通"},
{"F0C","両備バス"},
{"F0F","岡山電気軌道"},
{"F10","下津井電鉄"},
{"F14","広電バス"},
{"F15","広島バス"},
{"F16","広島交通"},
{"F18","芸陽バス"},
{"F1F","中国JRバス"},
{"F24","ボンバス"},
{"F47","コトデンバス"},

            };
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                }
                DbConnection.Dispose();

                disposedValue = true;
            }
        }

        ~StationStringDb()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(false);
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
