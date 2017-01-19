using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;
using StrawhatNet.NFC.FelicaReader.Suica;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Kassyi.NFC.Kelmis.Models
{
    public sealed class KelmisLogDb : IDisposable
    {
        public SQLiteConnection DbConnection { get; private set; }
        public static KelmisLogDb Current { get; } = new KelmisLogDb();
        public static string DbFilePath
            => Path.Combine(ApplicationData.Current.LocalFolder.Path, nameof(KelmisLogDb) + ".db");

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private KelmisLogDb()
        {
            DbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DbFilePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            if (!IsLogTableExit())
                DbConnection.CreateTable<KelmisLogRecode>();
        }

        /// <summary>
        /// ログ記録用のテーブルが存在しているか
        /// </summary>
        /// <returns></returns>
        bool IsLogTableExit()
        {
            var query = $"select count(*) from sqlite_master where type = 'table' and name = '{nameof(KelmisLogRecode)}';";
            var cmd = DbConnection.CreateCommand(query);

            return cmd.ExecuteScalar<int>() == 1;
        }

        /// <summary>
        /// EleMocaClasicのDBからこのアプリにデータを取り込みます。
        /// </summary>
        /// <param name="dbFilePath">EleMocaClasicのDBへのフルパス</param>
        public void AddLogFormEleMocaDb(string dbFilePath, byte[] idm)
        {
            //TODO: 時間かかるかも。非同期化？
            using (var elemocaDb = new SQLiteConnection(new SQLitePlatformWinRT(), dbFilePath))
            {
                //TODO:不正な値を突っ込んだ時のテスト
                DbConnection.InsertOrIgnoreAll(elemocaDb.Table<Rireki>()
                                                .Select(elemocaRecode => new KelmisLogRecode(elemocaRecode, idm)));
            }
        }

        public IEnumerable<string> GetSaveedCardIdms()
        {
            return DbConnection.Table<KelmisLogRecode>()
                .Select(l => l.Idm)
                .Distinct();
        }

        public void AddLog(IEnumerable<KelmisLogRecode> kelmisLog)
        {
            DbConnection.InsertOrIgnoreAll(kelmisLog);
        }
        public void AddLog(IEnumerable<SuicaRecode> log, byte[] idm)
        {
            AddLog(log.Select(l => new KelmisLogRecode(l, idm)));
            //TODO: 必要あるの？？
            //DbConnection.Commit();
        }

        public TableQuery<KelmisLogRecode> GetLog(string idm, DateBlock block)
        {
            return DbConnection.Table<KelmisLogRecode>()
                .Where(log => log.Idm == idm && log.Date <= block.StartDate && log.Date >= block.EndDate);
        }



        public void Dispose()
        {
            try
            {
                DbConnection.Dispose();
            }
            finally
            {
                GC.SuppressFinalize(DbConnection);
                DbConnection = null;
            }
        }
    }
}
