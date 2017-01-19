using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassyi.NFC.Kelmis.Services
{
    public partial class SettingsService
    {
        public static SettingsService Current => new SettingsService();

        private SettingsService() { }
    }
}
