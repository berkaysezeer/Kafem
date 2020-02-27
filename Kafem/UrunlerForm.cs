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
    public partial class UrunlerForm : Form
    {
        KafeVeri db;
        BindingList<Urun> blUrunler;

        public UrunlerForm(KafeVeri kafeVeri)
        {
            db = kafeVeri;
            InitializeComponent();
            dgvUrunler.AutoGenerateColumns = false; //sutunlar uzerinde degisiklikler yapmamizi saglar
            blUrunler = new BindingList<Urun>(db.Urunler); //formdan gelen veriyi bindinliste kaydetme
            dgvUrunler.DataSource = blUrunler;// binding list sayesinde datagrid view'i dolduruyoruz
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string urunAd = txtUrunAd.Text.Trim();

            if (urunAd == "") MessageBox.Show("Lütfen bir ürün adı giriniz");
            else
            {
                if (nudBirimFiyat.Value > 0)
                {
                    blUrunler.Add(new Urun //kontrollerden girilen degerleri bindingliste urun olarak ekliyoruz
                    {
                        UrunAd = urunAd,
                        BirimFiyat = nudBirimFiyat.Value
                    });

                    db.Urunler.Sort(); //a-z siralama
                    txtUrunAd.Clear();
                }
                else MessageBox.Show("lütfen geçerli bir fiyat giriniz");
            }

            nudBirimFiyat.Value = 0;
        }

        private void dgvUrunler_DataError(object sender, DataGridViewDataErrorEventArgs e) //hucrelere yanlis deger girdiysek
        {
            MessageBox.Show("Geçersiz Değer Girdiniz");
        }

        private void dgvUrunler_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (((string)e.FormattedValue).Trim() == "") //eger degistirilen veri bossa
                {
                    dgvUrunler.Rows[e.RowIndex].ErrorText = "Ürün adı boş girilemez";
                    e.Cancel = true;
                }
                else dgvUrunler.Rows[e.RowIndex].ErrorText = ""; //bos degilse hatayi kaldir/hata varme
            }
        }
    }
}
