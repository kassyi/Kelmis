using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrawhatNet.NFC.FelicaReader.Suica
{
    /// <summary>
    /// Suicaの利用履歴を表します。
    /// </summary>
    public class SuicaRecode
    {
        /// <summary>
        /// 端末区分
        /// </summary>
        public int TerminalId { get; set; }
        /// <summary>
        /// 処理区分
        /// </summary>
        public int ProssesId { get; set; }
        /// <summary>
        /// 日時　YYYYMMDDまで
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 利用区分
        /// </summary>
        public string UsageCategory => IsProductSales() ? UsageCategoryString.ProductSales
                                        : IsBus() ? UsageCategoryString.Bus
                                        : UsageCategoryString.Train;
        /// <summary>
        /// 線区コード
        /// </summary>
        public string InLineCode { get; set; }
        /// <summary>
        /// 駅順コード
        /// </summary>
        public string InStationCode { get; set; }
        /// <summary>
        /// 線区コード
        /// </summary>
        public string OutLineCode { get; set; }
        /// <summary>
        /// 駅順コード
        /// </summary>
        public string OutStationCode { get; set; }
        /// <summary>
        /// 残高
        /// </summary>
        public int Balance { get; set; }
        /// <summary>
        /// 取引通番
        /// </summary>
        public virtual int SequenceNumber { get; set; }
        /// <summary>
        /// リージョンコード
        /// </summary>
        public byte Region { get; set; }



        /// <summary>
        /// 何もしないインストラクタ
        /// </summary>
        public SuicaRecode() { }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="h"></param>
        protected SuicaRecode(SuicaRecode h)
        {
            TerminalId = h.TerminalId;
            ProssesId = h.ProssesId;
            Date = h.Date;
            InLineCode = h.InLineCode;
            InStationCode = h.InStationCode;
            OutLineCode = h.OutLineCode;
            OutStationCode = h.OutStationCode;
            SequenceNumber = h.SequenceNumber;
            Region = h.Region;

        }


        /// <summary>
        /// 物販かどうか
        /// </summary>
        /// <param name="procId"></param>
        /// <returns></returns>
        protected bool IsProductSales()
        {
            switch (ProssesId)
            {
                case 70:
                case 73:
                case 74:
                case 75:
                case 198:
                case 203:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// バスかどうか
        /// </summary>
        /// <param name="procId"></param>
        /// <returns></returns>
        protected bool IsBus()
        {
            switch (ProssesId)
            {
                case 13:
                case 15:
                case 31:
                case 35:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// このクラスをSuicaで使われるbyte配列の16進表記へパックします。
        /// </summary>
        /// <returns></returns>
        public string GeneratePackedBytes()
        {
            var result = new byte[16];
            result[0] = (byte)TerminalId;
            result[1] = (byte)ProssesId;
            //2-3: ??
            var mixDateTime = (Date.Year - 2000) << 9 | Date.Month << 5 | Date.Day;
            result[4] = (byte)((mixDateTime & 0xFF00) >> 8);
            result[5] = (byte)(mixDateTime & 0x00FF);
            result[6] = Convert.ToByte(InLineCode, 16);
            result[7] = Convert.ToByte(InStationCode, 16);
            result[8] = Convert.ToByte(OutLineCode, 16);
            result[9] = Convert.ToByte(OutStationCode, 16);

            result[10] = (byte)(Balance & 0x00FF);//(little endian)
            result[11] = (byte)((Balance & 0xFF00) >> 8);//(little endian)

            result[12] = (byte)((SequenceNumber & 0xFF0000) >> 8 * 2);
            result[13] = (byte)((SequenceNumber & 0x00FF00) >> 8 * 1);
            result[14] = (byte)((SequenceNumber & 0x0000FF));

            result[15] = (byte)Region;
            return BitConverter.ToString(result).Replace("-", "");
        }

        public override string ToString()
        {
            var db = StationStringDb.Instance;
            var inStation = db.SearchInStation(this).SingleOrDefault();
            var outStation = db.SearchOutStation(this).SingleOrDefault();

            return string.Join(", ",
                SequenceNumber,
                db.GetOrDefaultTerminalIdString(TerminalId),
                db.GetOrDefaultProsessIdString(ProssesId),
                UsageCategory,
                string.Format("{0}駅→{1}駅",
                    inStation == null ? StationStringDb.DefaultString : inStation.StaionName,
                    outStation == null ? StationStringDb.DefaultString : outStation.StaionName),
                Date.ToString("d"),
                $"残{Balance}円",
                "region: " + Region);
        }

        public static bool EqualsRawData(string item1, string item2)
        {
            return EqualsRawData(FelicaCardUtil.ToBytes(item1), FelicaCardUtil.ToBytes(item2));
        }

        public static bool EqualsRawData(byte[] item1, byte[] item2)
        {
            if (item1 == null || item2 == null)
                return false;
            if (item1.Length != 16 || item2.Length != 16)
                return false;

            //不明なため無視
            var item1Copy = new byte[16];
            var item2Copy = new byte[16];
            Array.Copy(item1, item1Copy, item1.Length);
            Array.Copy(item2, item2Copy, item2.Length);
            item1Copy[2] = item2Copy[2] = 0;
            item1Copy[3] = item2Copy[3] = 0;

            return item1Copy.SequenceEqual(item2Copy);
        }
    }
}
