using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrawhatNet.NFC.FelicaReader;
using StrawhatNet.NFC.FelicaReader.Suica;
using SQLite.Net.Attributes;
using System.Globalization;

namespace Kassyi.NFC.Kelmis.Models
{
    /// <summary>
    /// アプリのログの1レコードを表します
    /// </summary>
    public class KelmisLogRecode : SuicaRecode
    {
        /// <summary>
        /// 取引通番
        /// </summary>
        [PrimaryKey]
        public override int SequenceNumber { get; set; }

        /// <summary>
        /// カードのIdm
        /// </summary>
        public string Idm { get; set; }
        public byte[] IdmBytes => FelicaCardUtil.ToBytes(Idm);

        public KelmisLogRecode() : base() { }

        public KelmisLogRecode(SuicaRecode history, byte[] idm) : base(history)
        {
            Idm = BitConverter.ToString(idm).Replace("-", "");
        }

        /// <summary>
        /// EleMoca ViewerのDBレコードを使用してこのクラスを初期化します
        /// </summary>
        /// <param name="r"></param>
        /// <param name="idm"></param>
        public KelmisLogRecode(Rireki r, byte[] idm)
        {
            Idm = BitConverter.ToString(idm).Replace("-", "");
            TerminalId = r.Kiki;
            ProssesId = r.RiyouKubun;
            Date = DateTime.ParseExact(r.RiyouYmd.ToString(), "yyyyMMdd", DateTimeFormatInfo.InvariantInfo);

            //バス等の場合、レコードが違うため以下4つは設定しない
            if (!(IsBus() && IsProductSales()))
            {
                //Nyuujou・Syutuzyou CD
                //上位8bit = StationCode 下位8bit = LineCode

                InLineCode = (r.NyuujouCD >> 8).ToString("x");
                InStationCode = (r.NyuujouCD & 0xff).ToString("x");
                OutLineCode = (r.SyutujouCD >> 8).ToString("x");
                OutStationCode = (r.SyutujouCD & 0xff).ToString("x");
            }

            Balance = r.Zangaku;
            SequenceNumber = r.TorihikiTsuuban;
            Region = (byte)r.Chiiki;

        }
    }
}
