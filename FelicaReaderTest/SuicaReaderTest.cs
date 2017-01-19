using System;
using static StrawhatNet.NFC.FelicaReader.FelicaCardUtil;
using StrawhatNet.NFC.FelicaReader.Suica;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace FelicaReaderTest
{
    
    [TestClass]
    public class SuicaReaderTest
    {
        //TODO: バス、オートチャージ、物販の仕様も作る。
        //TODO: 20件読みきれなかった時のテスト。
        //TODO: 本家DB読み込み＆テスト
        [TestMethod]
        public void SuicaUsageHistoryRecodeTest1()
        {
            //石神井公園→秋津
            var case1Expected = "1601000220F0BA0EBA166E090000DA00";
            var case1Actual = new SuicaCardPaser()
            {
                TerminalId = (int)TerminalIdEnum.改札機,
                ProssesId = (int)ProsessIdEnum.運賃支払_改札出場,
                Date = new DateTime(2016, 7, 16),
                InLineCode = "ba",
                InStationCode = "e",
                OutLineCode = "ba",
                OutStationCode = "16",
                Balance = 2414,
                SequenceNumber = 218,
                Region = 0
            };
            SuicaUsageHistoryRecodeTestActual(case1Expected, case1Actual, "石神井公園", "秋津");
        }
        [TestMethod]
        public void SuicaUsageHistoryRecodeTest2()
        {
            //新秋津→国立
            var case2Expected = "1601000220F02911032096080000DC00";
            var case2Actual = new SuicaCardPaser()
            {
                TerminalId = (int)TerminalIdEnum.改札機,
                ProssesId = (int)ProsessIdEnum.運賃支払_改札出場,
                Date = new DateTime(2016, 7, 16),
                InLineCode = "29",
                InStationCode = "11",
                OutLineCode = "3",
                OutStationCode = "20",
                Balance = 2198,
                SequenceNumber = 220,
                Region = 0
            };
            SuicaUsageHistoryRecodeTestActual(case2Expected, case2Actual, "新秋津", "国立");
        }

        /// <summary>
        /// レコードから正しくパースして、駅名も持ってこれることの確認
        /// </summary>
        public void SuicaUsageHistoryRecodeTestActual(string bytes, SuicaCardPaser recode, string inStaionName, string outStationName)
        {
            var expected = new SuicaCardPaser(ToArraySegmentBytes(bytes));
            Assert.AreEqual(expected.TerminalId, recode.TerminalId);
            Assert.AreEqual(expected.ProssesId, recode.ProssesId);
            Assert.AreEqual(expected.Date, recode.Date);
            Assert.AreEqual(expected.UsageCategory, recode.UsageCategory);
            Assert.AreEqual(expected.InLineCode, recode.InLineCode);
            Assert.AreEqual(expected.InStationCode, recode.InStationCode);
            Assert.AreEqual(expected.OutLineCode, recode.OutLineCode);
            Assert.AreEqual(expected.OutStationCode, recode.OutStationCode);
            Assert.AreEqual(expected.Balance, recode.Balance);
            Assert.AreEqual(expected.SequenceNumber, recode.SequenceNumber);
            Assert.AreEqual(expected.Region, recode.Region);

            var db = StationStringDb.Instance;
            var inStation =  db.SearchInStation(recode).Single();
            var outStation = db.SearchOutStation(recode).Single();
            Assert.AreEqual(inStation.StaionName, inStaionName);
            Assert.AreEqual(outStation.StaionName, outStationName);
        }

        //20件の履歴を例外発生せずに検索できるか
        [TestMethod]
        public void ParseAndSearch20Test()
        {
            var db = StationStringDb.Instance;
            foreach (var recode in Enum20Log())
            {
                var areacode = db.GetAreaCode(recode.InLineCode, recode.Region);
                var inStation = db.SearchInStation(recode).Single();
                inStation.ToString();
                if (recode.ProssesId == (int)ProsessIdEnum.入A_入場時オートチャージ || recode.ProssesId == (int)ProsessIdEnum.出A_出場時オートチャージ)
                    continue;
                var outStation = db.SearchOutStation(recode).Single();
                outStation.ToString();
            }
        }

        [TestMethod]
        //展開したあと再パックした時に元に戻せるか。
        public void Pack20Test()
        {
            foreach (var recode in Enum20Log())
            {
                Assert.IsTrue(SuicaRecode.EqualsRawData(recode.GeneratePackedBytes(), recode.RawData));
            }
        }

        [TestMethod]
        public void ToBytePassTest()
        {
            var input = "1601000220FD0322032064030000F000";
            var actual = new byte[] { 0x16, 0x01, 0x00, 0x02, 0x20, 0xFD, 0x03, 0x22, 0x03, 0x20, 0x64, 0x03, 0x00, 0x00, 0xF0, 0x00 };
            CollectionAssert.AreEqual(ToBytes(input), actual);
        }

        [TestMethod]
        public void ToByteFailTest()
        {
            var input = "1122334455F";
            AssertEx.Throws<ArgumentOutOfRangeException>(() =>ToBytes(input));
        }

        private IEnumerable<SuicaCardPaser> Enum20Log()
        {
            var rawBytes = ToArraySegmentBytes("16010002210503220320CC08000101001601000221050320032251090000FF0016010002210303220320D6090000FD00160100022103032003225B0A0000FB00160100022101180C0320E00A0000F900160100022101D01ED027640C0000F700160100022101D027D01EFE0C0000F5001601000221010320180C980D0000F300161402012101032000001C0F0000F1001601000220FD0322032064030000F0001601000220FD03200322E9030000EE001601000220FB032203206E040000EC001601000220FB03200322F3040000EA001601000220F90322032078050000E8001601000220F903200322FD050000E6001601000220F60322032082060000E4001601000220F60320032207070000E2001601000220F4032203208C070000E0001601000220F40320032211080000DE001601000220F02911032096080000DC00");

            var recodes = new List<ArraySegment<byte>>(Enumerable.Range(0, 20)
                .Select(i => new ArraySegment<byte>(rawBytes.Array, i * 16, 16)))
                .Select(b => new SuicaCardPaser(b));
            return recodes;
        }


    }
}
