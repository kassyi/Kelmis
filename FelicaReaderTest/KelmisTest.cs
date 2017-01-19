using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Kassyi.NFC.Kelmis.Models;
using System.IO;
using Windows.Storage;
using System.Reflection;
using Windows.ApplicationModel;
using StrawhatNet.NFC.FelicaReader;

namespace FelicaReaderTest
{
    [TestClass]
    public class KelmisTest
    {
        KelmisLogDb dbConeccter;
        readonly string testDbFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "testdb.db");
        
        
        [TestMethod]
        public void AddLogFormEleMocaDbTest()
        {
            Reset();

            var elemocaDbPath = Path.Combine(Package.Current.InstalledLocation.Path, "SF_0101-0114-2416-4E11.DB");
            dbConeccter.AddLogFormEleMocaDb(elemocaDbPath, FelicaCardUtil.ToBytes("0101011424164E11"));
            Assert.AreEqual(dbConeccter.DbConnection.Table<KelmisLogRecode>().Count(), 63);
            var hoge = dbConeccter.DbConnection.Table<KelmisLogRecode>().ToArray();
        }

        //[TestInitialize]
        void Reset()
        {
            if (dbConeccter != null)
                dbConeccter.Dispose();
            File.Delete(testDbFilePath);
            //dbConeccter = new KelmisLogDb(testDbFilePath);

            //未実装
            Assert.Fail();
        }
        
    }
}

