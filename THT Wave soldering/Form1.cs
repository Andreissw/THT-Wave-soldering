using SMTReport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using THT_Wave_soldering.Base;

namespace THT_Wave_soldering
{
    public partial class Form1 : Form
    {
        int X { get; set; } = 12;
        int Y { get; set; } = 12;
        int X_m { get; set; } = 411;
        int Y_m { get; set; } = 12;
        int RowIndex { get; set; }
        string NameUser { get; set; }
        int UserID { get; set; }
        int SpecID { get; set; }

        List<string> listDay = new List<string> { "09:00:00", "11:00:00", "13:00:00", "15:00:00", "17:00:00", "19:30:00" };
        List<string> listNight = new List<string> { "22:00:00", "00:00:00", "02:00:00", "04:00:00", "06:00:00", "07:30:00" };


        delegate void Row();


        public Form1()
        {
            InitializeComponent();
            BTlogin.Click += (a, e) => { if (!Log(TBLogin.Text)) { TBLogin.Clear(); return; } OnMenu(); };
            TBLogin.KeyDown += (a, e) => { if (e.KeyCode == Keys.Enter) { if (!Log(TBLogin.Text)) { TBLogin.Clear(); return; } OnMenu(); } };
            ExBT.Click += (a, e) => { Application.Exit(); };
            MenuGrid.CellClick += (a, e) =>
            {
                RowIndex = MenuGrid.CurrentCell.RowIndex;
                List<Row> _RowMethod = new List<Row>() { AddData, AddModel, SendEmail, Report, GetTable, Back }; Refresh();
                _RowMethod[RowIndex]();

            };

            CBModels.TextChanged += (a, e) => { CBProduct.DataSource = listProduct(CBModels.Text); DefectPos.DataSource = listposition(); DefectPos.Text = ""; CBProduct.Text = ""; };

            CBDayNight.TextChanged += (a, e) => { GetTime(CBDayNight.Text); };

            CBProduct.TextChanged += (a, e) => { CBLot.DataSource = listlot(); CBLot.Text = ""; };

            Close1.Click += (a, e) => { Refresh(); };

            RB1.Click += (a, e) =>
            {
                addmodelSpec();
            };

            SpecRB2.Click += (a, e) => { addmodelSpec2(); };

            SpecCB.SelectedIndexChanged += (a, e) => { specGrid(); };

            AddModelBT.Click += (a, e) =>
            {
                if (_CheckControls(GRAddmodel, "Spec")) return;
                //if (!RB1.Checked & !SpecRB2.Checked) {MessageBox.Show("Надо добавить спецификацию к модели"); return; }
                //if (Spec.Rows.Count == 1) { MessageBox.Show("Спецификации не обнаружено"); return; ; }

                SaveModel();

                _Clear(GRAddmodel);
                Spec.Visible = false;
                SpecCB.Visible = false;
                LBspec.Visible = false; RB1.Checked = false; SpecRB2.Checked = false;
            };

            SaveBT.Click += (a, e) =>
            {
                switch (CHDef.Checked)
                {
                    case true:
                        if (_CheckControls(GRAddData, "Defect"))
                            return;
                        Save();
                        break;
                    case false:
                        if (_CheckControls(GRAddData))
                            return;
                        SaveDef();
                        break;
                }
                DefectCount.Value = 0;
                DefectsCB.Text = "";
                DefectPos.Text = "";
                //_Clear(GRAddData);
            };

            ClearBT.Click += (a, e) => _Clear(GRAddData);

            TableGrid.CellClick += (a, e) => { /*GetUpdateForm();*/ };

            CHDef.Click += (a, e) => { if (CHDef.Checked) { NoDefect(); return; } Defect(); };

            MenuGrid.Location = new Point(X, Y);
            loginGR.Location = new Point(X, Y);

            foreach (Control item in this.Controls.OfType<GroupBox>())
                if (item.Name.StartsWith("GR"))
                    item.Location = new Point(X_m, Y_m);
        }

        void GetTime(string name)
        {
            CBTime.Items.Clear();
            if (name == "День")
                foreach (var item in listDay)
                    CBTime.Items.Add(item);
            else if (name == "Ночь")
                foreach (var item in listNight)
                    CBTime.Items.Add(item);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var student = new[] { new { Name = "Добавить данные" }, new { Name = "Добавить Модель" }, new { Name = "Отправить Карту" }, new { Name = "Отчёт" }, new { Name = "Исходная таблица" }, new { Name = "Назад" } };
            MenuGrid.DataSource = student;
            if (Start_Form.timesOK) { Report(); timer1.Enabled = true; }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(@"C:\Скриншот");
            FileInfo[] fi = di.GetFiles();
            foreach (FileInfo f in fi)
            {
                f.Delete();
            }

            CallRobot();
            timer1.Enabled = false;
            MessageBox.Show("Сообщение отправлено");
            Refresh();

        }



        bool Log(string RFID)
        {
            using (var con = new Connect())
            {
                UserID = con.Users.Where(c => c.RFID == RFID).Select(c => c.Id).FirstOrDefault();
                NameUser = con.Users.Where(c => c.RFID == RFID).Select(c => c.UserName).FirstOrDefault();
                if (String.IsNullOrWhiteSpace(NameUser)) return false;
                if (UserID == 0) return false;
                return true;
            }
        }

        void OnMenu()//Вход в главное меню
        {
            loginGR.Visible = false;
            MenuGrid.Visible = true;
        }

        void AddData()
        {
            GRAddData.Visible = true;
            GRAddData.Size = new Size(420, 500);
            Controller_TB.Text = NameUser;
            CB_Controlles.DataSource = liStUser();
            CB_Controlles.Text = "";
            CBModels.DataSource = listModels();
            CBModels.Text = "";
            DefectsCB.DataSource = listDefect();
            DefectsCB.Text = " ";
        }

        void AddModel()
        {

            GRAddmodel.Visible = true;
            GRAddmodel.Size = new Size(253, 184);

        }

        void SendEmail()
        {
            Refresh();
            Report();
            var result = MessageBox.Show($"Отправить Карту контроля ПВП за {DateTime.Now.ToString("dd.MM.yy")} на {DateTime.Now.ToString("HH:mm")}", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;
            timer1.Enabled = true;
        }

        void GetGridTime(ref int Starthour, ref int EndHour , ref int day)
        {
            var time = DateTime.Now.Hour;
            if  (time >= 21 || time <= 7)
            { ListTime(listNight); Starthour = 20; EndHour = 8; day = 1; } //Ночь

            else if (time >= 8 || time <= 22)
            { ListTime(listDay); Starthour = 8; EndHour = 20; day = 0; } //День

        
        }

        void ListTime(List<string> list)
        {
            for (int i = 3; i < list.Count + 3; i++)           
                GridReport.Columns[i].HeaderText = list[i - 3];           
        }

      

        void Report()
        {
            int Starthour = 8;
            int Endhour = 20;
            int day = 1;
            GetGridTime(ref Starthour, ref Endhour, ref day);
            GRReport.Visible = true;
            GRReport.Size = new Size(1424, 500);
            GRReport.Location = new Point(12, 353);        

            LBDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            LBHour.Text = DateTime.Now.ToString("HH:mm:ss");
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
                
            if (day == 0)
            {
                 start = DateTime.Now.AddHours(-(DateTime.Now.Hour - Starthour)).AddMinutes(-(DateTime.Now.Minute)).AddSeconds(-(DateTime.Now.Second));
                 end = DateTime.Now.AddHours(-(DateTime.Now.Hour - Endhour)).AddMinutes(-(DateTime.Now.Minute)).AddSeconds(-(DateTime.Now.Second)).AddDays(day);
            }
            else if (DateTime.Now.Hour >= 22)
            {
                start = DateTime.Now.AddHours(-(DateTime.Now.Hour - Starthour)).AddMinutes(-(DateTime.Now.Minute)).AddSeconds(-(DateTime.Now.Second));
                end = DateTime.Now.AddHours(-(DateTime.Now.Hour - Endhour)).AddMinutes(-(DateTime.Now.Minute)).AddSeconds(-(DateTime.Now.Second)).AddDays(day);
            }
            else if (DateTime.Now.Hour >= 0)
            {
                start = DateTime.Now.AddHours(-(DateTime.Now.Hour - Starthour)).AddMinutes(-(DateTime.Now.Minute)).AddSeconds(-(DateTime.Now.Second)).AddDays(-day);
                end = DateTime.Now.AddHours(-(DateTime.Now.Hour - Endhour)).AddMinutes(-(DateTime.Now.Minute)).AddSeconds(-(DateTime.Now.Second));
            }

           
            //var start = DateTime.Parse("18.10.2020 20:00:00");
            //var end = DateTime.Parse("19.10.2020 08:00:00");

            if (GetReport(start,end)) return;

            var list = (from DataGridViewRow a in _grid.Rows
                        select new {Name = a.Cells["Name"].Value.ToString(), Product = a.Cells["Product"].Value.ToString(), Line = a.Cells["Line"].Value.ToString(), Objective = a.Cells["Objective"].Value.ToString() }).Distinct().ToList();

            GridReport.RowCount = list.Count();            
            foreach (var item in list) //Получаем инфо в ОтчётГрид по модели изделию и линии 
            {
                var i = list.IndexOf(item);
                GridReport[0,i].Value = item.Name;
                GridReport[1, i].Value = item.Product;
                GridReport[2, i].Value = item.Line;
                GridReport[10, i].Value = item.Objective;
            }

                //Сложный цикл в котором запутается даже автор этого говна || получаем данные в ОтчётГрид по волновой пайке по времени
                for (int k = 0; k < GridReport.RowCount; k++)
                {
                    var Сцепить = GridReport[0, k].Value.ToString() + GridReport[1, k].Value.ToString() + GridReport[2, k].Value.ToString();                  
                    for (int i = 3; i < GridReport.ColumnCount; i++)
                    {

                    var СцепитьВремя = Сцепить + GridReport.Columns[i].HeaderText;

                        for (int l = 0; l < _grid.RowCount; l++)
                        {
                            var Сцепить_grid = _grid[0, l].Value.ToString() + _grid[1, l].Value.ToString() + _grid[2, l].Value.ToString() + _grid[3, l].Value.ToString();
                            if (Сцепить_grid == СцепитьВремя)
                            GridReport[i, k].Value = _grid[5, l].Value;
                          
                        }               
                    }
                }

                for (int i = 0; i < GridReport.RowCount; i++) // Цикл который считает за смену
                {
                    var sumlist = new List<int>() { 0,0};
                    for (int k = 3; k < GridReport.ColumnCount - 2; k++)
                    {
                        if (GridReport[k, i].Value == null) continue;
                        sumlist[0] = sumlist[0] + int.Parse(GridReport[k, i].Value.ToString());
                        sumlist[1] = sumlist[1] + 1;
                        

                    }
                    GridReport[9, i].Value = sumlist[0] / sumlist[1];
                }

                for (int i = 0; i < GridReport.RowCount; i++)
                {
                if (GridReport[10, i].Value.ToString() == "") continue;
                if (int.Parse(GridReport[9, i].Value.ToString()) > int.Parse(GridReport[10, i].Value.ToString())) { GridReport[9, i].Style.BackColor = Color.Red; GridReport[10, i].Style.BackColor = Color.Red; }
                else { GridReport[9, i].Style.BackColor = Color.Green; GridReport[10, i].Style.BackColor = Color.Green; }

                }

                using (var con = new Connect())
                {
                    //var d = DateTime.Now.AddHours(-(DateTime.Now.Hour - 8)).AddMinutes(-(DateTime.Now.Minute)).AddSeconds(-(DateTime.Now.Second));
                    LBName1.Text = (from a in con.Logs  join b in con.Users on a.ControllerId equals b.Id  where a.Date >= start && a.Date <= end orderby a.Date descending  select b.UserName).FirstOrDefault();
                    LBName2.Text = (from a in con.Logs  join b in con.Users on a.Mate_ControllerId equals b.Id  where a.Date >= start && a.Date <= end orderby a.Date descending select b.UserName).FirstOrDefault();
                }

            GridReport.ClearSelection();
        }

        void GetTable()
        {
            GRTable.Visible = true;
            GRTable.Size = new Size(1478, 469);
            GRTable.Location = new Point(12, 353);
            _getTable();         
            TableGrid.FirstDisplayedScrollingRowIndex = TableGrid.RowCount-1;
            TableGrid.ClearSelection();
        }

        void Back()
        {
            loginGR.Visible = true;
            MenuGrid.Visible = false;
        }


        void addmodelSpec()
        {
            LBspec.Visible = true;
            SpecCB.Visible = true;
            Spec.Visible = false;
            Spec.DataSource = null;
            Spec.ColumnCount = 0;
            SpecCB.DataSource = listProductDistinct();
            SpecCB.Text = "";
        }

        void addmodelSpec2()
        {
            LBspec.Visible = false;
            SpecCB.Visible = false;
            Spec.DataSource = null;
            Spec.Visible = true;
            Spec.ColumnCount = 0;
            Spec.ColumnCount = 2;
            Spec.Columns[0].HeaderText = "Cust";          
            Spec.Columns[1].HeaderText = "Позиция";     
        }

        void specGrid()
        {
            if (String.IsNullOrWhiteSpace(SpecCB.Text))
                return;
            Spec.Visible = true;
            Spec.DataSource = SpecGrid();
        }

        void _getTable()
        {
            using (var con = new Connect())
            {
                var date = DateTime.Now.AddDays(-30);
                var L = (from a in con.Logs
                        join b in con.Users on a.ControllerId equals b.Id
                        join c in con.Users on a.Mate_ControllerId equals c.Id
                        join d in con.Models on a.ModelsId equals d.ID
                        orderby a.Date 
                        where a.Date > date && a.Remove != true
                        select new
                        {
                            Дата = a.Date,
                            Контролёр_качества = b.UserName,
                            Сменный_мастер = c.UserName,
                            Модель = d.Name,
                            Изделие = d.Product
                        ,
                            Заказ = a.Lot,
                            Время = a.Time,
                            линия = a.Line,
                            Позиционный_номер = a.Position,
                            Дефект = a.Defects,
                            Дефект_шт = a.count,
                            Выборка = a.Selection
                        ,
                            opportunity = d.Opportunity,
                            Цель = d.Objective,
                            Ключ = a.ID
                        }).ToList();
                TableGrid.DataSource = L;

            }
        }

        void SaveModel() //Сохранение новой модели с готовой спекой
        {
            using (var con = new Connect())
            {
                var numid = con.Models.Where(c => c.Product == SpecCB.Text).Select(c => c.SpecId).FirstOrDefault();

                var model = new Wave_Models()
                {
                    Name = NameModel.Text,
                    Product = Product.Text,
                    Objective = Objective.Text,
                    SpecId = 0,
                    Opportunity = Convert.ToInt32(opport.Text),

                };
                con.Models.Add(model);
                con.SaveChanges();
                MessageBox.Show("Готово");
            }
        }

        #region Добавление модели со спецификацией

        //void SaveModelSpec() //Сохранение новой модели с готовой спекой
        //{
        //    using (var con = new Connect())
        //    {
        //        var numid = con.Models.Where(c => c.Product == SpecCB.Text).Select(c => c.SpecId).FirstOrDefault();

        //        var model = new Wave_Models()
        //        {
        //            Name = NameModel.Text,
        //            Product = Product.Text,                
        //            Objective = Convert.ToInt16(Objective.Text),
        //            SpecId = numid,
        //            Opportunity = Convert.ToInt32(opport.Text),

        //        };
        //        con.Models.Add(model);
        //        con.SaveChanges();
        //        MessageBox.Show("Готово");
        //    }
        //}

        //bool SaveModelNewSpec() //Сохранение модели с ново
        //{
        //    using (var con = new Connect())
        //    {
        //        var numid = con.Specs.OrderByDescending(c => c.NumSpec).Select(c => c.NumSpec).FirstOrDefault();

        //        for (int i = 0; i < Spec.RowCount - 1; i++)
        //        {
        //            if (Spec[0, i].Value == null || Spec[1, i].Value == null) {MessageBox.Show("Не все поля в спецификации заполнены"); return true; }

        //            var spec = new Wave_Spec()
        //            {
        //                Cust = Spec[0, i].Value.ToString(),                        
        //                Position = Spec[1, i].Value.ToString(),
        //                NumSpec = numid + 1
        //            };
        //            con.Specs.Add(spec);
        //            con.SaveChanges();
        //        }

        //        var spec1 = new Wave_Spec()
        //        {
        //            Cust = "",
        //            Position = "",
        //            NumSpec = numid + 1
        //        };

        //        con.Specs.Add(spec1);
        //        con.SaveChanges();

        //        var model = new Wave_Models()
        //        {
        //            Name = NameModel.Text,
        //            Product = Product.Text,                   
        //            Objective = Convert.ToInt16(Objective.Text),                    
        //            SpecId = numid + 1,
        //            Opportunity = Convert.ToInt32(opport.Text),


        //        };

        //        con.Models.Add(model);
        //        con.SaveChanges();
        //        MessageBox.Show("Готово");
        //        return false;

        //    }

        //}
        #endregion

        public void Refresh(string nameObj = "") //Обновление
        {
            foreach (GroupBox T in this.Controls.OfType<GroupBox>())
            {
                if (T.Name == nameObj) { continue; }
              
                T.Visible = false;
                foreach (Control I in T.Controls)
                {
                    if (I.GetType() == typeof(Label)) { }
                    else if (I.GetType() == typeof(Button)) { }
                    else if (I.GetType() == typeof(CheckBox)) { }
                    else if (I.GetType() == typeof(RadioButton)) { }
                    else if (I.GetType() == typeof(ComboBox))
                    {
                        ((ComboBox)I).DataSource = null; ((ComboBox)I).Text = "";
                    }
                    else I.Text = "";
                }
            }
        }

        object liStUser()
        {
            using (var con = new Connect())
            {
                return con.Users.OrderBy(c=>c.UserName).Where(c => c.UserName != NameUser && c.RFID != "12345").Select(c => c.UserName).ToList();
            }
        }

       public object listModels()
        {
            using (var con = new Connect())
            {
                return con.Models.OrderBy(c =>c.Name).Select(c=>c.Name).ToList();
            }
        }

        public object listProduct(string model)
        {
            using (var con = new Connect())
            {
                return con.Models.OrderBy(c => c.Name).Where(c=> new[] {model,"" }.Contains(c.Name)).Select(c => c.Product).ToList();
            }
        }

        object listProductDistinct()
        {
            using (var con = new Connect())
            {
                return con.Models.OrderBy(c => c.Name).Select(c => c.Product).Distinct().ToList();
            }
        }



        object listlot()
        {
            using (var con = new Connect())
            {
                return con.Logs.Select(c => c.Lot).Distinct().ToList();
            }
        }

        object listDefect()
        {
            using (var con = new Connect())
            {
                //var list = con.Defects.Select(c => c.Description).ToList();
                return con.Defects.Select(c=>c.Defect + " " + c.Description).ToList();
            }
        }

        object listposition()
        {
            using (var con = new Connect())
            {
                var modelid = con.Models.Where(c => c.Name == CBModels.Text).Select(c => c.ID).FirstOrDefault();
                //return con.Specs.OrderBy(c => c.Name).Where(c => c.ModelId == modelid).Select(c => c.Position).ToList();
                return (from a in con.Logs                      
                        where a.ModelsId == modelid
                        select a.Position).Distinct().ToList();
            }
        }

        object SpecGrid()
        {
            using (var con = new Connect())
            {
                SpecID = con.Models.Where(c => c.Product == SpecCB.Text).Select(c => c.SpecId).FirstOrDefault();
                return (from a in con.Models
                        join b in con.Specs on a.SpecId equals b.NumSpec
                        where a.Product == SpecCB.Text
                        select new { b.Cust, b.Position }).ToList();    
            }
        }

        public bool _CheckControls(Control _control,string Def = "12345")
        {
        
            foreach (Control I in _control.Controls)
                if (String.IsNullOrEmpty(I.Text) & I.GetType() != typeof(Button))
                {
                    if (I.Name.StartsWith(Def))
                        continue;
                    if (I.Name == "Objective")
                        continue;

                    MessageBox.Show("Не все поля заполнены");
                    I.Select();
                    return true;
                }
            return false;
        }

        void _Clear(Control _control)
        {
            foreach (Control I in _control.Controls)
            {
                if (I.GetType() == typeof(Button))
                    continue;
                if (I.GetType() == typeof(CheckBox))
                     continue;
                if (I.GetType() == typeof(Label))
                    continue;
                if (I.GetType() == typeof(RadioButton))
                    continue;
                if (I.Name == "Controller_TB")
                    continue;
                I.Text = "";
            }
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

        void SaveDef()
        {
            using (var con = new Connect())
            {
                var mateid = con.Users.Where(c => c.UserName == CB_Controlles.Text).Select(c => c.Id).FirstOrDefault();  
                var modelid = con.Models.Where(c => c.Name == CBModels.Text).Select(c => c.ID).FirstOrDefault();    

                var log = new Wave_Log
                {
                    Date = ADDDateProject.Value,
                    DateFact = DateTime.Now,
                    Line = Convert.ToByte(CBLine.Text),
                    Time = CBTime.Text,
                    count = DefectCount.Value.ToString(),
                    ControllerId = UserID,
                    Mate_ControllerId = mateid,
                    Defects = DefectsCB.Text,
                    ModelsId = modelid,
                    Position = DefectPos.Text ,
                    Lot = CBLot.Text,
                    Selection = Convert.ToInt32(SelectionCount.Value)
                };

                con.Logs.Add(log);
                con.SaveChanges();
            }
        }

        void Save()
        {
            using (var con = new Connect())
            {
                var mateid = con.Users.Where(c => c.UserName == CB_Controlles.Text).Select(c => c.Id).FirstOrDefault();
                var defectid = con.Defects.Where(c => c.Defect == DefectsCB.Text).Select(c => c.ID).FirstOrDefault();
                var modelid = con.Models.Where(c => c.Name == CBModels.Text).Select(c => c.ID).FirstOrDefault();
                var positionid = con.Specs.Where(c => c.Position == DefectPos.Text).Select(c => c.ID).FirstOrDefault();

                var log = new Wave_Log
                {
                    Date = ADDDateProject.Value,
                    DateFact = DateTime.Now,
                    Line = Convert.ToByte(CBLine.Text),
                    Time = CBTime.Text,                  
                    ControllerId = UserID,
                    Mate_ControllerId = mateid,                  
                    ModelsId = modelid,
                    Lot = CBLot.Text,
                    count = "0",
                    Selection = Convert.ToInt32(SelectionCount.Value)
                };
               
                con.Logs.Add(log);
                con.SaveChanges();


            }
        }


       bool GetReport(DateTime start, DateTime end) //Получение таблица с лога
        {
            using (var con = new Connect())
            {
               
                var list = (from a in con.Logs
                            join b in con.Models on a.ModelsId equals b.ID

                            where a.Date >= start &&  a.Date <= end && a.Remove != true

                            group a by new { b.Name, b.Product, a.Line, a.Time, a.Selection, b.Opportunity, b.Objective } into g
                            orderby g.Key.Time
                            select new
                            {
                                g.Key.Name,
                                g.Key.Product,
                                g.Key.Line,
                                g.Key.Time,
                                g.Key.Objective,
                                DPMO = Math.Round((g.Select(c => c.count).Cast<double>().Sum() / (g.Key.Selection * g.Key.Opportunity) * 1000000)),
                                date = g.Select(c=>c.Date).FirstOrDefault()
                            }).ToList();
                if (list.Count == 0) return true;
                _grid.DataSource = list; return false;
            }
        }

        private void Objective_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 59) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void opport_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 59) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void Selection_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 59) && e.KeyChar != 8)
                e.Handled = true;
        }

        void GetUpdateForm()
        {
            var idrow = TableGrid[14, TableGrid.CurrentCell.RowIndex].Value.ToString();
            var Form = new FormUpdate(idrow);

            for (int i = 0; i < TableGrid.ColumnCount - 1; i++)
            {
                if (TableGrid[i, TableGrid.CurrentCell.RowIndex].Value == null) { Form.List.Add(""); continue; }
                Form.List.Add(TableGrid[i, TableGrid.CurrentCell.RowIndex].Value.ToString());
            }

            Form.ShowDialog();
            GetTable();

        }

        private void CallRobot()
        {
            string line = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".jpg";
            string obj = $"Карта контроля ПВП за {DateTime.Now.ToString("dd.MM.yy")} на {DateTime.Now.ToString("HH:mm")}";
            GetScreen(1700, 495, line);
            SendEmail(@"C:\Скриншот\" + line, obj);
        }

        void GetScreen(int width, int height, string screen)
        {     
            Bitmap BM = new Bitmap(width, height);
            Size size = new Size(width, height);
            Point point = new Point(12,375);
            Point point2 = new Point(10, 10);
            Graphics GH = Graphics.FromImage(BM as Image);          
            GH.CopyFromScreen(point, point2, BM.Size);     
         
            BM.Save(@"C:\Скриншот\" + screen + "");
        }



        void SendEmail(string image, string card)
        {
            MailAddress fromMailAdress = new MailAddress("controlerotk@dtvs.ru", "КонтроллерОТК");       
            MailAddress to = new MailAddress("a.volodin@dtvs.ru", "ME");
            AlternateView html_view = AlternateView.CreateAlternateViewFromString(fromMailAdress.Address, null, "text/html");
            using (MailMessage MailMessage = new MailMessage(fromMailAdress, to))
            using (SmtpClient SmtpClient = new SmtpClient("mail.technopolis.gs", 25))
            {
                MailMessage.CC.Add("Ломакина Светлана Ивановна <s.lomakina@dtvs.ru>");
                MailMessage.CC.Add("Мелехин Константин Данилович <melekhin@dtvs.ru>");
                MailMessage.CC.Add("Костина Ксения Викторовна <kostina@dtvs.ru>");
                MailMessage.CC.Add("Лишик Станислав Александрович <lishik@dtvs.ru>");
                MailMessage.CC.Add("Контролер ОТК <controlerotk@dtvs.ru>");
                MailMessage.CC.Add("Слабицкая Татьяна Михайловна <slabitskaya@dtvs.ru>");
                MailMessage.CC.Add("Набатов Валерий Юрьевич <v.nabatov@dtvs.ru>");
                MailMessage.CC.Add("Гусаров Валерий Вячеславович <gusarov@dtvs.ru>");
                MailMessage.CC.Add("Ященко Петр Владимирович <yashenko@dtvs.ru>");
                MailMessage.CC.Add("Рыжков Иван Васильевич <i.ryjkov@dtvs.ru>");
                MailMessage.CC.Add("Климчук Андрей Михайлович <klimchuk@dtvs.ru>");
                MailMessage.CC.Add("Каспирович Дмитрий Иванович <kaspirovich@dtvs.ru>");
                MailMessage.CC.Add("Лобанов Олег Юрьевич <lobanov@dtvs.ru>");

                //MailMessage.CC.Add("Володин Андрей Александрович <a.volodin@dtvs.ru>");

            MailMessage.Subject = card;
                MailMessage.AlternateViews.Add(getEmbeddedImage(image, "1"));

                //SmtpClient.EnableSsl = true;
                //SmtpClient.Host = "mail.technopolis.gs";
                //SmtpClient.Port = 587;
                //SmtpClient.EnableSsl = false;
                SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpClient.UseDefaultCredentials = false;
                SmtpClient.Credentials = new NetworkCredential("controlerotk@dtvs.ru", "qwertyuio");
                SmtpClient.Send(MailMessage);
            }

        }

        AlternateView getEmbeddedImage(string filePath, string content)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = content;
            string htmlBody = @"<img src='cid:" + res.ContentId + @"'/>";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

    
    }
}
