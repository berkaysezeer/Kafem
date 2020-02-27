using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafem.Data
{
    public class Urun : IComparable
    {
        public string UrunAd { get; set; }
        public decimal BirimFiyat { get; set; }

        public int CompareTo(object obj) //urun adina gore siralama icin metot
        {
            return UrunAd.CompareTo(((Urun)obj).UrunAd);
        }

        public override string ToString() // string metodunu kendimize gore duzenledik -- override
        {
            return string.Format("{0} {1:0.00}₺", UrunAd, BirimFiyat);
        }
    }
}
