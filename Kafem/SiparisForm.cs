using Kafem.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kafem
{
    public partial class SiparisForm : Form
    {
        public event EventHandler<MasaTasimaEventArg> MasaTasiniyor; //siniftaki bilgileri forma, formdakileri sinifa

        KafeVeri db;
        Siparis siparis;
        BindingList<SiparisDetay> blSiparisDetay;

        public SiparisForm(KafeVeri kafeVeri, Siparis siparis)
        {
            db = kafeVeri;
            //db = new KafeVeri(); //veriyi cekemiyor
            this.siparis = siparis;
            blSiparisDetay = new BindingList<SiparisDetay>(siparis.SiparisDetaylar); //siparis detaylarini bindinglisteye atar
            InitializeComponent();
            MasaNoYukle();
            MasaNoGuncelle();
            TutarGuncelle();
            cboUrun.DataSource = db.Urunler; //db'ye eklenmis urunleri combo box'a atar
            dgvSiparisDetay.DataSource = blSiparisDetay; //eklenmis urunşerin detaylarini datagrid viewe ekler
        }

        private void TutarGuncelle()
        {
            lblTutar.Text = siparis.ToplamTutarTl; //toplam tutari Siparis icindeki fieldden alir. field icinde tutar metodu var
        }

        private void MasaNoYukle()
        {
            cboMasaNo.Items.Clear(); //??
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                //masa doluysa combobox'a masano yazdirma
                if (!db.AktifSiparisler.Any(x => x.MasaNo == i)) //aktif siparislerde masano'su i olan yoksa
                    cboMasaNo.Items.Add(i);
            }

            cboMasaNo.SelectedIndex = 0;
        }

        private void MasaNoGuncelle()
        {
            Text = "Masa " + siparis.MasaNo; //Form'un ismine Masa no yazma
            lblMasaNo.Text = siparis.MasaNo.ToString("00"); //labele masa nı aktarma
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (cboUrun.SelectedItem == null)
            {
                MessageBox.Show("Lütfen Bir Ürün Seçiniz");
                return;
            }

            Urun seciliUrun = (Urun)cboUrun.SelectedItem; //cbo'daki urunu boxing yaparak seceriz

            var sd = new SiparisDetay //yeni siparis detaylarini olusturuyoruz
            {
                UrunAd = seciliUrun.UrunAd,
                BirimFiyat = seciliUrun.BirimFiyat,
                Adet = (int)nudAdet.Value
            };

            blSiparisDetay.Add(sd); //binding list icine -- form1' den kafeVeri olarak db gonderilmisti
            nudAdet.Value = 1;
            cboUrun.SelectedIndex = 0;
            TutarGuncelle(); //yeni urunler eklendigi icin tutari guncelliyoruz
        }

        private void btnAnasayfa_Click(object sender, EventArgs e) => Close();

        private void btnSpairsIptal_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Sipariş İptal Edilecektir", //butona bastigimizda uyari cikar
                "Sipariş İptal Onayı",
                MessageBoxButtons.YesNo, //yes ve no adli iki buton olacak diyoruz
                MessageBoxIcon.Warning, //ikon secimi
                MessageBoxDefaultButton.Button2 //secili butonu ikinci buton(no-hayir) yapma
                );

            if (dr == DialogResult.Yes) //eger kapatmaya emin ise
            {
                Close();
                siparis.KapanisZamani = DateTime.Now; //masanin kapanis zamanini kaydet
                siparis.Durum = SiparisDurum.Iptal; //siparis durmunu iptale cekiyoruz
            }
        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Masa kapatılıyor. Onaylıyor musun?",
                "Ödeme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                siparis.KapanisZamani = DateTime.Now;
                siparis.Durum = SiparisDurum.Odendi;
                siparis.OdenenTutar = siparis.ToplamTutar(); // urunlerin toplam tutarini kaydetme
                Close();
            }
        }

        private void dgvSiparisDetay_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) //eger mouse ile sağ tiklandiysa
            {
                int rowIndex = dgvSiparisDetay.HitTest(e.X, e.Y).RowIndex; //tiklanan satirin indeksini alir

                if (rowIndex > -1)
                {
                    dgvSiparisDetay.ClearSelection(); //birden fazla secili oge kalmamasi icin her tiklanmada temizliyoruz
                    dgvSiparisDetay.Rows[rowIndex].Selected = true; //sag tiklanan satiri seciyoruz
                    cmsSiparisDetay.Show(Cursor.Position); //tikladigimiz yerde context menu strip acilir
                    //cmsSiparisDetay.Show(MousePosition);
                }
            }
        }

        private void tsmiSiparisSil_Click(object sender, EventArgs e)
        {
            if (dgvSiparisDetay.SelectedRows.Count > 0) //secili satir varsa
            {
                var seciliSatir = dgvSiparisDetay.SelectedRows[0];
                var siparisDetay = (SiparisDetay)seciliSatir.DataBoundItem; //secili satirdaki nesneyi alir (siparisdetaylari)
                blSiparisDetay.Remove(siparisDetay); //secili siparisin bindinglistten cikarir
            }

            TutarGuncelle(); //urun hesaptan cikarildigi icin guncellenmeli
        }

        private void btnMasTasi_Click(object sender, EventArgs e)
        {
            if (cboMasaNo.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir masa no seçiniz");
                return;
            }

            int eskiMasaNo = siparis.MasaNo; //eski masa noyu kaybetmemek icin
            int hedefMasaNo = (int)cboMasaNo.SelectedItem; //bilgilerin tasinacagi masa no

            if (MasaTasiniyor != null) //butona atanmis bir event varsa
            {
                var args = new MasaTasimaEventArg //eventin icini dolduruyoruz
                {
                    TasinanSiparis = siparis, //suan mevcut masadaki siparisleri tasiyoruz
                    EskiMasaNo = eskiMasaNo, //suanki masa nosunu eski masaya at
                    YeniMasaNo = hedefMasaNo //tasinacak masa no
                };

                MasaTasiniyor(this, args);
            }

            //formu guncelleme kismi
            siparis.MasaNo = hedefMasaNo; //artik tasinmis masa nosu siparis icine atiyoruz
            MasaNoGuncelle(); //label kismina yeni masa noyu yaziyoruz
            MasaNoYukle(); //masa tasindigi icin cbo'yu guncelliyoruz

        }

        public class MasaTasimaEventArg : EventArgs
        {
            public Siparis TasinanSiparis { get; set; }
            public int YeniMasaNo { get; set; }
            public int EskiMasaNo { get; set; }
        }
    }
}
