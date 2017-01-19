using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrawhatNet.NFC.FelicaReader.Suica
{
    public class SuicaReadWithoutEncryptionResponse : ReadWithoutEncryptionResponse
    {
        public bool IsCommandSuccess
        {
            get
            {
                return BlockData[10] == 0 && BlockData[11] == 0;
            }
        }
    }
}
