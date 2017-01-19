using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrawhatNet.NFC.FelicaReader.Suica
{
    /// <summary>
    /// DB用の駅の情報。これを使ってDBのレコードが表現されるい
    /// </summary>
    public class Station
    {
        [PrimaryKey]
        public int Id { get; set; }
        /// <summary>
        /// 0-4
        /// </summary>
        public byte AreaCode { get; set; }
        /// <summary>
        /// 16進表記かつ、小文字
        /// </summary>
        public string LineCode { get; set; }
        /// <summary>
        /// 16進表記かつ、小文字
        /// </summary>
        public string StationCode { get; set; }
        public string CompanyName { get; set; }
        public string LineName { get; set; }
        public string StaionName { get; set; }
    }

}
