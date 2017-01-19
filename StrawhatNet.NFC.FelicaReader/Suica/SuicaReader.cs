using System;
using System.Threading.Tasks;
using Windows.Devices.SmartCards;
using System.Collections.Generic;
using System.Linq;

namespace StrawhatNet.NFC.FelicaReader.Suica
{
    /// <summary>
    /// Suicaなどのサイバネ規格ICカード乗車券の読み取りクラス
    /// </summary>
    public class SuicaReader : FelicaReader
    {
        private const ushort SystemCode = 0x0003;


        public SuicaReader(SmartCardConnection connection)
            : base(connection)
        {
            
        }

        /// <summary>
        /// SmartCardConnectionで指定されたカードからsuicaの履歴を読み取ります。
        /// </summary>
        /// <param name="connection">max: 20</param>
        /// <returns></returns>
        public async Task<SuicaCard> ReadSuicaAsync(SmartCardConnection connection)
        {
            var idm = await GetIdmAsync();
            var usageBytes = await GetUsageHistoryAsync(idm);
            if (usageBytes.Count == 0)
                return null;
            return ParseHistory(idm, usageBytes);
        }

        /// <summary>
        /// IDmと利用履歴のbyte配列を受け取り、解析します。
        /// </summary>
        /// <param name="idm"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        public SuicaCard ParseHistory(byte[] idm, IEnumerable<ArraySegment<byte>> usage)
        {
            var suica = new SuicaCard();
            suica.Idm = idm;

            suica.Recodes.AddRange(usage.Select(u => new SuicaCardPaser(u)));
            return suica;
        }

        private Task<byte[]> GetIdmAsync()
        {
            return base.Polling(SystemCode);
        }

        /// <summary>
        /// idmで指定されたカードから利用履歴を読み取ります。
        /// (service code: 0x090f, block: max 20)
        /// </summary>
        /// <param name="idm">idm</param>
        /// <param name="blockSize">ブロックサイズ＝読み取る履歴の数</param>
        /// <returns>
        /// item1: idm
        /// item2: 読み取った利用履歴（byte配列）の羅列。最大20件。読み取りに失敗した場合、0件のリスト
        /// </returns>
        private async Task<List<ArraySegment<byte>>> GetUsageHistoryAsync(byte[] idm)
        {
            var rawRecode = new List<byte>();
            var readRecodeCount = 0;

            //一度に読み取るレコードの数
            const int numberOfReadingAtOnce = 4;
            //4レコード*5回回す。機器によって取得できるレコードの数が違うため
            for (int i = 0; i < 5; i++)
            {
                var blockList = Enumerable.Range(i * numberOfReadingAtOnce, numberOfReadingAtOnce)
                                    .Select(j => new byte[] { 0x80, (byte)j })
                                    .SelectMany(x => x)
                                    .ToArray();
                ReadWithoutEncryptionResponse rawResponse;
                try
                {
                    rawResponse = await ReadWithoutEncryption(idm, 0x090f, numberOfReadingAtOnce, blockList);
                }
                catch (Exception)
                {
                    //20件取得できなかったので取得できた数だけ返す
                    return new List<ArraySegment<byte>>(Enumerable.Range(0, readRecodeCount)
                        .Select(j => new ArraySegment<byte>(rawRecode.ToArray(), j * 16, 16)));
                }
                rawRecode.AddRange(rawResponse.BlockData);
                readRecodeCount += numberOfReadingAtOnce;
            }
            // response.BlockData[n*16] = 履歴データ。16byte/ブロックの繰り返し。
            return new List<ArraySegment<byte>>(Enumerable.Range(0, readRecodeCount)
                .Select(i => new ArraySegment<byte>(rawRecode.ToArray(), i * 16, 16)));
        }

        /// <summary>
        /// 改札入出場履歴 (service code: 0x108f, block: max 3)
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> GetTicketGateEnterLeaveHistory(byte[] idm)
        {
            ReadWithoutEncryptionResponse result = await base.ReadWithoutEncryption(idm, 0x108f, 0x01, new byte[] { 0x80, 0x00, });
            return result.BlockData;
        }

        /// <summary>
        /// SF入場駅記録 (service code: 0x10cb, block: 2)
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> GetSFEnteredStationInfo(byte[] idm)
        {
            ReadWithoutEncryptionResponse result = await base.ReadWithoutEncryption(idm, 0x10cb, 0x01, new byte[] { 0x80, 0x00, });
            return result.BlockData;
        }

        /// <summary>
        /// 料金 発券/改札記録 (service code: 0x184b, block: 1)
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> GetTicketIssueInspectRecord(byte[] idm)
        {
            ReadWithoutEncryptionResponse result = await base.ReadWithoutEncryption(idm, 0x184b, 0x01, new byte[] { 0x80, 0x00, });
            return result.BlockData;
        }
    }
}
