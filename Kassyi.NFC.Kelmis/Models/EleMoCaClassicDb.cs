using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassyi.NFC.Kelmis.Models
{
    public class CardInfo
    {
        public string CardID { get; set; }
    }

    public class Rireki
    {
        [PrimaryKey]
        public int TorihikiTsuuban { get; set; }
        public int RiyouKubun { get; set; }
        public int RiyouYmd { get; set; }
        public int RiyouHms { get; set; }
        public int Kingaku { get; set; }
        public int Zangaku { get; set; }
        public int Charge { get; set; }
        public int Kiki { get; set; }
        public int Shiharai { get; set; }
        public int Nyuusyutujou { get; set; }
        public int NyuujouCD { get; set; }
        public int SyutujouCD { get; set; }
        public int Chiiki { get; set; }
    }
}
