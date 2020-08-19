using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using THT_Wave_soldering.Base;

namespace THT_Wave_soldering
{
    public partial class FormUpdate : Form
    {
        int idrow { get; set; }

        public List<string> List = new List<string>();

        public FormUpdate(string idrow)
        {
            InitializeComponent();
            this.idrow = int.Parse(idrow);
            Key.Text = this.idrow.ToString();

            CHDef.Click += (a, e) => { if (CHDef.Checked) { NoDefect(); return; } Defect(); };

            Close1.Click += (a, e) => this.Close();

            this.Load += (a, e) => { getdata(); getmodel(); _insertOfList(GRAddData, List); };

            UpdateBT.Click += (a, e) => { CheckControl(); };

            BTdelete.Click += (a, e) => { if(message()) delete(); };

            CBModels.TextChanged += (a, e) => {
                var Form = new Form1();
                CBProduct.DataSource = Form.listProduct(CBModels.Text);
            };

        }

        void getbool()
        {
            if (DefectCount.Value == 0) CHDef.Checked = true;
        }

        void getmodel()
        {
            var Form = new Form1();
            CBModels.DataSource = Form.listModels();
        }

        bool message()
        {
            var MSG =  MessageBox.Show("Уверены, что хотите удалить","Предупреждение", MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (MSG == DialogResult.No) return false;
            return true;
        }

        void delete()
        {
            using (var con = new Connect())
            {                
                var line = con.Logs.Where(c => c.ID == idrow).FirstOrDefault();
                line.Remove = true;
                con.SaveChanges();

            }
        }

        void updateDef()
        {

        }

        void update()
        {

        }

        void NoDefect()
        {
            DefectsCB.Text = "";
            DefectsCB.Enabled = false;
            DefectCount.Value = 0;
            DefectCount.Enabled = false;
            DefectPos.Text = "";
            DefectPos.Enabled = false;

        }

        void Defect()
        {
            DefectsCB.Text = "";
            DefectsCB.Enabled = true;
            DefectCount.Value = 0;
            DefectCount.Enabled = true;
            DefectPos.Text = "";
            DefectPos.Enabled = true;
        }


        void getdata()
        {
            using (var con = new Connect())
            {
                //var list = con.Logs.Where(c => c.ID == idrow).sel;
                
                
            }
        }

         void CheckControl()
        {
            var Form = new Form1();

            switch (CHDef.Checked)
            {
                case true:
                    if (Form._CheckControls(GRAddData, "Defect"))
                        return;
                    update();
                    break;
                case false:
                    if (Form._CheckControls(GRAddData))
                        return;
                    updateDef();
                    break;
            }
         
        }

        void _insertOfList(Control control, List<string> list)
        {
            foreach (Control item in control.Controls)
            {
                int i = control.Controls.IndexOf(item);
                foreach (Control c in control.Controls)
                    if (c.TabIndex == i)
                    {
                        c.Text = list[i];
                        continue; }
            }
        }




    }
}
