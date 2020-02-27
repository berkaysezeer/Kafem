using Kafem.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Kafem
{
    public partial class Form1 : Form
    {
        KafeVeri db;

        public Form1()
        {
            db = new KafeVeri();
            //OrnekVerileriYukle();
            VerileriOku();
            InitializeComponent();
            MasalarOlustur(); //masa gorselleri ve masa bilgileri icin metot
        }

        private void VerileriOku() //json olarak verileri okuma
        {
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonConvert.DeserializeObject<KafeVeri>(json); //verileri deserilazson yapma
            }
            catch (Exception)
            {
                db = new KafeVeri(); //okunacak veri yoksa veritabanini sifirdan baslat
            }
        }

        private void OrnekVerileriYukle()
        {
            db.Urunler = new List<Urun>()
            {
                new Urun{UrunAd="Kola", BirimFiyat=6.99m},
                new Urun{UrunAd="Su", BirimFiyat=2.00m},
            };
        } //silinecek

        private void MasalarOlustur()
        {
            #region Resim Hazırlama
            ImageList img = new ImageList();
            img.Images.Add("bos", Properties.Resources.bos); //kaynak dosyadan adi bos olan gorseli ceker
            img.Images.Add("dolu", Properties.Resources.dolu);
            img.ImageSize = new Size(64, 64);
            lvwMasalar.LargeImageList = img; //gorselleri listview icine liste olarak atar
            #endregion

            ListViewItem lvi;
            for (int masaNo = 1; masaNo <= db.MasaAdet; masaNo++)
            {
                lvi = new ListViewItem("Masa" + masaNo);

                Siparis sip = db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == masaNo); //masaNo ait aktif siparis var mi? varsa sip'e kaydet

                if (sip == null) //aktif siparis yoksa masa bostur
                {
                    lvi.Tag = masaNo;
                    lvi.ImageKey = "bos";
                }
                else //doluysa Tag'a siparisi at
                {
                    lvi.Tag = sip;
                    lvi.ImageKey = "dolu";
                }

                lvwMasalar.Items.Add(lvi); //listview icine itemler eklenir
            }

        }

        private void lvwMasalar_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var lvi = lvwMasalar.SelectedItems[0]; // secili masayi lvi'ye gonderir
                lvi.ImageKey = "dolu"; //cift tiklandigi an masa dolu olur

                Siparis sip;

                if (lvi.Tag is Siparis) sip = (Siparis)lvi.Tag; //eger masa doluysa Tag icindeki siparis bilgilerini sip'e at
                else //masa bossa
                {
                    sip = new Siparis();
                    sip.MasaNo = (int)lvi.Tag; //masa bos oldugu icin iceride tag'de masa no tutulur. Bilgiyi sip nesnesine yolla
                    sip.AcilisZamani = DateTime.Now;
                    lvi.Tag = sip; //masa artik dolu old icin tag'a siparisi atariz
                    db.AktifSiparisler.Add(sip); //aktif siparislere ekle
                }

                SiparisForm frmSiparis = new SiparisForm(db, sip); //kafeveri'yi ve sip'arisi diger forma aktarma
                frmSiparis.MasaTasiniyor += FrmSiparis_MasaTasiniyor;
                frmSiparis.ShowDialog();

                if (sip.Durum != SiparisDurum.Aktif) //siparis odenmis ya da iptal edilmisse
                {
                    lvi.Tag = sip.MasaNo; //masa noyu tekrar tag'e at
                    lvi.ImageKey = "bos"; //gorseli degis
                    db.AktifSiparisler.Remove(sip); //aktif siparisleri bosalt
                    db.GecmisSiparisler.Add(sip); //verilmis siparisleri gecmissiparis liste ekle
                }

            }
        }

        private void FrmSiparis_MasaTasiniyor(object sender, SiparisForm.MasaTasimaEventArg e)
        {
            //adim1 eski masayi bosalt
            ListViewItem lviEskiMasa = MasaBul(e.EskiMasaNo); //eski masayi bulup lvieskimasaya at
            lviEskiMasa.Tag = e.EskiMasaNo;
            lviEskiMasa.ImageKey = "bos";

            //adim2 yeni masaya siparis koy
            ListViewItem lviYeniMasa = MasaBul(e.YeniMasaNo);
            lviYeniMasa.Tag = e.TasinanSiparis;
            lviYeniMasa.ImageKey = "dolu";
        }

        private ListViewItem MasaBul(int masaNo)
        {
            foreach (ListViewItem item in lvwMasalar.Items)
            {
                if (item.Tag is int && (int)item.Tag == masaNo) return item; //tag'da siparis olmadigini garantiye aliriz
                else if (item.Tag is Siparis && ((Siparis)item.Tag).MasaNo == masaNo) return item;
            }
            return null;
        }

        private void tsmiGecmisSiparis_Click(object sender, EventArgs e)
        {
            GecmisSiparisForm frmGecmisSiparis = new GecmisSiparisForm(db);
            frmGecmisSiparis.ShowDialog();
        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            UrunlerForm frmUrunler = new UrunlerForm(db); //verileri yolluyoruz
            frmUrunler.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string json = JsonConvert.SerializeObject(db); //veritabanina serilizasyon yapar
            File.WriteAllText("veri.json", json); //hedef dosyaya kaydeder
        }
    }
}
