using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrawhatNet.NFC.FelicaReader.Suica
{
    /// <summary>
    /// Suicaカード本体を表します。
    /// </summary>
    public class SuicaCard
    {
        public byte[] Idm { get; set; }
        public List<SuicaCardPaser> Recodes { get; set; }
        public SuicaCard()
        {
            Idm = new byte[16];
            Recodes = new List<SuicaCardPaser>();
        }
    }

    /// <summary>
    /// Suicaカードの利用履歴の1レコードを表します。パースできます。
    /// </summary>
    public class SuicaCardPaser : SuicaRecode
    {
        /// <summary>
        /// 何もしないコンストラクタ。テストなどに。後で初期化してください。
        /// </summary>
        public SuicaCardPaser() { }

        //デバッグ用
        private byte[] rawData = new byte[16];
        /// <summary>
        /// 生データ。生データが与えられなかった場合null
        /// </summary>
        public string RawData => rawData == null ? null : BitConverter.ToString(rawData).Replace("-", "");

        /// <summary>
        /// <paramref name="data"/>で与えられたバイナリデータを解釈してこのクラスを初期化します。
        /// </summary>
        /// <param name="data">suicaから取得したバイナリデータ。16bytes</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/>がnullなとき</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="data"/>が16byte以外の場合。</exception>
        public SuicaCardPaser(ArraySegment<byte> data) : base()
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Count != 16)
                throw new ArgumentOutOfRangeException(nameof(data));
#if DEBUG
            Array.Copy(data.Array, data.Offset, rawData, 0, data.Count);
#endif
            Parse(data);
        }

        private void Parse(ArraySegment<byte> data)
        {
            TerminalId = data.Array[data.Offset + 0]; //0: 端末種
            ProssesId = data.Array[data.Offset + 1]; //1: 処理
                                                     //2-3: ??
            int mixDate = toInt(data.Array, data.Offset, 4, 5);
            var yearShort = (mixDate >> 9) & 0x07f;
            var month = (mixDate >> 5) & 0x00f;
            var day = mixDate & 0x01f;
            Date = new DateTime(2000 + yearShort, month, day);

            if (IsBus())
            {
                OutLineCode = toInt(data.Array, data.Offset, 6, 7).ToString("x");
                OutStationCode = toInt(data.Array, data.Offset, 8, 9).ToString("x");
            }
            if (!(IsBus() && IsProductSales()))
            {
                //6 : 入線区
                InLineCode = (data.Array[data.Offset + 6] & 0xFF).ToString("x");
                //7 : 入駅順
                InStationCode = (data.Array[data.Offset + 7] & 0xFF).ToString("x");
                //8 : 出線区
                OutLineCode = (data.Array[data.Offset + 8] & 0xFF).ToString("x");
                //9 : 出駅順
                OutStationCode = (data.Array[data.Offset + 9] & 0xFF).ToString("x");
            }


            Balance = toInt(data.Array, data.Offset, 11, 10); //10-11: 残高 (little endian)
            SequenceNumber = toInt(data.Array, data.Offset, 12, 13, 14); //12-14: 連番
            Region = (byte)(data.Array[data.Offset + 15] & 0xFF); ; //15: リージョン 
        }

        /// <summary>
        /// リトルエンディアンbyte配列をビッグエンディアンintへでアンパック
        /// </summary>
        /// <param name="res"></param>
        /// <param name="off"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        private int toInt(byte[] res, int off, params int[] idx)
        {
            int num = 0;
            for (int i = 0; i < idx.Length; i++)
            {
                num = num << 8;
                num += ((int)res[off + idx[i]]) & 0x0ff;
            }
            return num;
        }
    }
}
