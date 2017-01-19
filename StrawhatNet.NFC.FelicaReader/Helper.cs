using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrawhatNet.NFC.FelicaReader
{
    /// <summary>
    /// Dictionary 型の拡張メソッドを管理するクラス
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 指定したキーに関連付けられている値を取得します。
        /// キーが存在しない場合は既定値を返します
        /// </summary>
        public static TValue GetOrDefault<TKey, TValue>(
            this Dictionary<TKey, TValue> self,
            TKey key,
            TValue defaultValue = default(TValue))
        {
            TValue value;
            return self.TryGetValue(key, out value) ? value : defaultValue;
        }
    }

    public static class FelicaCardUtil
    {
        /// <summary>
        /// 16進数形式の文字列を受け取り、byte配列に変換します。例：<c>030000F000</c>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] ToBytes(string s)
        {
            var result = new List<byte>();
            if (s.Length % 2 != 0)
                throw new ArgumentOutOfRangeException(nameof(s));

            while (s.Length != 0)
            {
                result.Add(Convert.ToByte(string.Concat(s.Take(2)), 16));
                s = string.Concat(s.Skip(2));
            }
            return result.ToArray();
        }

        public static ArraySegment<byte> ToArraySegmentBytes(string s)
        {
            var result = ToBytes(s);
            return new ArraySegment<byte>(result.ToArray(), 0, result.Length);
        }
    }
}
