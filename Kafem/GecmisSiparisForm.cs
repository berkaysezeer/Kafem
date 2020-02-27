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
    public partial class GecmisSiparisForm : Form
    {
        KafeVeri db;

        public GecmisSiparisForm(KafeVeri kafeVeri)
        {
            db = kafeVeri;
            InitializeComponent();

            dgvSiparisler.DataSource = db.GecmisSiparisler; //gecmis siparisler datagridview'e
        }

        private void dgvSiparisler_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSiparisler.SelectedRows.Count > 0) //secili bir satir varsa
            {
                DataGridViewRow satir = dgvSiparisler.SelectedRows[0]; //verileri satir olarak sakla
                Siparis siparis = (Siparis)satir.DataBoundItem; //satirdaki nesneyi alir
                dgvSiparisDetay.DataSource = siparis.SiparisDetaylar;
            }
        }
    }
}
