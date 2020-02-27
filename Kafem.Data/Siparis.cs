using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafem.Data
{
    public enum SiparisDurum
    {
        Aktif, Odendi, Iptal
    }

    public class Siparis
    {
        public Siparis()
        {
            SiparisDetaylar = new List<SiparisDetay>();
        }

        public int MasaNo { get; set; }
        public DateTime? AcilisZamani { get; set; } // DateTime normalde null olamaz. Fakat ? koydugumuzda artik null değer alabilir
        public DateTime? KapanisZamani { get; set; } // DateTime normalde null olamaz. Fakat ? koydugumuzda artik null değer alabilir
        public SiparisDurum Durum { get; set; }
        public decimal OdenenTutar { get; set; }

        public List<SiparisDetay> SiparisDetaylar { get; set; }

        public string ToplamTutarTl => string.Format("{0:0.00}₺", ToplamTutar()); //field oldugunu unutma

        public decimal ToplamTutar() => SiparisDetaylar.Sum(x => x.Tutar()); //siparisleri toplam fiyatini doner
    }
}
