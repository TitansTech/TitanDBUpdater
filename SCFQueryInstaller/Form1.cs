using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SCFQueryInstaller
{
    public partial class Form1 : Form
    {
        Database db;
        Database db2;
        Database db3;

        string tablemu;
        string tableme;
        string tablerank;

        public string FilePath = System.Windows.Forms.Application.StartupPath + @"\TitanDBUpdater.ini";

        public Form1()
        {
            InitializeComponent();
        }
        private void LogAdd(string txt)
        {
            File.WriteLine(Application.StartupPath + @"/Log/Normal_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_Info.log", txt, true);
            listBox1.Items.Add(txt);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }
        private void LogAddUp(string txt)
        {
            File.WriteLine(Application.StartupPath + @"/Log/Upgrader_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_Info.log", txt, true);
            listBox1.Items.Add(txt);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }
        private void LogAddDn(string txt)
        {
            File.WriteLine(Application.StartupPath + @"/Log/Downgrader_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_Info.log", txt, true);
            listBox1.Items.Add(txt);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        public string ReadFile(string FilePath)
        {
            try
            {
                StreamReader r = new StreamReader(FilePath);
                string str = r.ReadToEnd();
                r.Close();
                return str;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int QueryExecuted = 0;
            int QuerySuccess = 0;

            string Query = "";
            bool Success = false;

            LogAdd("=============================================================");
            LogAdd("[" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "](" + DateTime.Now.ToShortTimeString() + ") " + Application.ProductName + " VERSION: " + Application.ProductVersion);
            LogAdd("=============================================================");
            LogAdd("                        NORMAL UPDATE                        ");
            LogAdd("-------------------------------------------------------------");
            LogAdd("");

            string[] files = Directory.GetFiles(Application.StartupPath + @"\Scripts\MuOnline\");
            foreach (string file in files)
            {
                if (Path.GetExtension(file) == ".tdu")
                {
                    QueryExecuted++;
                    Query = ReadFile(file);
                    Query = Query.Replace("%s", tablemu);
                    Success = db.Exec(Query);
                    if (Success == false)
                    {
                        LogAdd("[Error](" + tablemu + ") Fail on query: " + Path.GetFileNameWithoutExtension(file) + " - " + db.ExError.Message);
                    }
                    else
                    {
                        QuerySuccess++;
                        LogAdd("[Done](" + tablemu + ")  Success executing query: " + Path.GetFileNameWithoutExtension(file));
                    }
                }
            }

            if (checkBox1.Checked == true)
            {
                QueryExecuted++;
                Query = "UPDATE " + tablemu + ".dbo.Character set scfmasterskill=0x00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
                Success = db.Exec(Query);
                if (Success == false)
                {
                    QuerySuccess++;
                    LogAdd("[Error] Cleanning Master SkillTree: " + db.ExError.Message);
                }
                else
                {
                    LogAdd("[Done] Success Master SkillTree");
                }
            }

            files = Directory.GetFiles(Application.StartupPath + @"\Scripts\Me_MuOnline\");
            foreach (string file in files)
            {
                if (Path.GetExtension(file) == ".tdu")
                {
                    QueryExecuted++;
                    Query = ReadFile(file);
                    Query = Query.Replace("%s", tableme);
                    Success = db2.Exec(Query);
                    if (Success == false)
                    {
                        LogAdd("[Error](" + tableme + ")  Fail on query: " + Path.GetFileNameWithoutExtension(file) + " - " + db2.ExError.Message);
                    }
                    else
                    {
                        QuerySuccess++;
                        LogAdd("[Done](" + tableme + ")  Success executing query: " + Path.GetFileNameWithoutExtension(file));
                    }
                }
            }

            files = Directory.GetFiles(Application.StartupPath + @"\Scripts\Ranking\");
            foreach (string file in files)
            {
                if (Path.GetExtension(file) == ".tdu")
                {
                    QueryExecuted++;
                    Query = ReadFile(file);
                    Query = Query.Replace("%s", tablerank);
                    Success = db3.Exec(Query);
                    if (Success == false)
                    {
                        LogAdd("[Error](" + tablerank + ")  Fail on query: " + Path.GetFileNameWithoutExtension(file) + " - " + db3.ExError.Message);
                    }
                    else
                    {
                        QuerySuccess++;
                        LogAdd("[Done](" + tablerank + ")  Success executing query: " + Path.GetFileNameWithoutExtension(file));
                    }
                }
            }

            LogAdd("");
            LogAdd("[" + QuerySuccess  + "/" + QueryExecuted + "] Query's executed fine.");
            LogAdd("TASK COMPLETED");
            LogAdd("");
            MessageBox.Show("Titan DB Update Completed!");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(Application.StartupPath + @"\Log\") == false)
                System.IO.Directory.CreateDirectory(Application.StartupPath + @"\Log\");


            tablemu = File.GetString("DataBase", "MuOnline", "MuOnline", FilePath);
            tableme = File.GetString("DataBase", "MeMuOnline", "Me_MuOnline", FilePath);
            tablerank = File.GetString("DataBase", "Ranking", "Ranking", FilePath);

            string DNS = File.GetString("SQLConfig", "DNS1", "MuOnline", FilePath);
            string DNS2 = File.GetString("SQLConfig", "DNS2", "MuOnlineJoinDB", FilePath);
            string DNS3 = File.GetString("SQLConfig", "DNS3", "Ranking", FilePath);

            string User = File.GetString("SQLConfig", "User", "sa", FilePath);
            string Pass = File.GetString("SQLConfig", "Password", "", FilePath);
            string IpAddress = File.GetString("SQLConfig", "IpAdress", "(local)", FilePath);

            db = new Database(IpAddress);
            bool con = db.Connect(DNS, User, Pass);

            if (con == false)
            {
                MessageBox.Show("Connection failed(1)!");
                Application.Exit();
            }

            db2 = new Database(IpAddress);
            con = db2.Connect(DNS2, User, Pass);

            if (con == false)
            {
                MessageBox.Show("Connection failed(2)!");
                Application.Exit();
            }

            db3 = new Database(IpAddress);
            con = db3.Connect(DNS3, User, Pass);

            if (con == false)
            {
                MessageBox.Show("Connection failed(3)!");
                Application.Exit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string Query = "";
            bool Success = false;

            LogAddUp("=============================================================");
            LogAddUp("[" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "](" + DateTime.Now.ToShortTimeString() + ") " + Application.ProductName + " VERSION: " + Application.ProductVersion);
            LogAddUp("=============================================================");
            LogAddUp("                 INVENTORY AND VAULT UPGRADE                 ");
            LogAddUp("-------------------------------------------------------------");
            LogAddUp("");

            Query = "ALTER TABLE " + tablemu + ".dbo.Character ALTER COLUMN Inventory varbinary(3776)";
            Success = db.Exec(Query);
            if (Success == false)
            {
                LogAddUp("[Error] Failed Modify Inventory Column: " + db.ExError.Message);
            }
            else
            {
                LogAddUp("[Done] Success Modify Inventory Column");
            }

            Query = "ALTER TABLE " + tablemu + ".dbo.warehouse ALTER COLUMN Items varbinary(3840)";
            Success = db.Exec(Query);
            if (Success == false)
            {
                LogAddUp("[Error] Failed Modify Warehouse_Items Column: " + db.ExError.Message);
            }
            else
            {
                LogAddUp("[Done] Success Modify Warehouse_Items Column");
            }

            Query = "ALTER TABLE " + tablemu + ".dbo.ExtendedWarehouse ALTER COLUMN Items varbinary(3840)";
            Success = db.Exec(Query);
            if (Success == false)
            {
                LogAddUp("[Error] Failed Modify ExtendedWarehouse_Items Column: " + db.ExError.Message);
            }
            else
            {
                LogAddUp("[Done] Success Modify ExtendedWarehouse_Items Column");
            }

            Query = "ALTER TABLE " + tablemu + ".dbo.GuildWarehouse ALTER COLUMN Items varbinary(3840)";
            Success = db.Exec(Query);
            if (Success == false)
            {
                LogAddUp("[Error] Failed Modify GuildWarehouse_Items Column: " + db.ExError.Message);
            }
            else
            {
                LogAddUp("[Done] Success Modify GuildWarehouse_Items Column");
            }
            LogAddUp("TASK COMPLETED");
            LogAddUp("");
            MessageBox.Show("Titan DB Update Completed!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string Query = "";
            bool Success = false;

            LogAddDn("=============================================================");
            LogAddDn("[" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "](" + DateTime.Now.ToShortTimeString() + ") " + Application.ProductName + " VERSION: " + Application.ProductVersion);
            LogAddDn("=============================================================");
            LogAddDn("                INVENTORY AND VAULT DOWNGRADE                ");
            LogAddDn("-------------------------------------------------------------");
            LogAddDn("");

            Query = "ALTER TABLE " + tablemu + ".dbo.Character ALTER COLUMN Inventory varbinary(1728)";
            Success = db.Exec(Query);
            if (Success == false)
            {
                LogAddDn("[Error] Failed Modify Inventory Column: " + db.ExError.Message);
            }
            else
            {
                LogAddDn("[Done] Success Modify Inventory Column");
            }

            Query = "ALTER TABLE " + tablemu + ".dbo.warehouse ALTER COLUMN Items varbinary(1200)";
            Success = db.Exec(Query);
            if (Success == false)
            {
                LogAddDn("[Error] Failed Modify Warehouse_Items Column: " + db.ExError.Message);
            }
            else
            {
                LogAddDn("[Done] Success Modify Warehouse_Items Column");
            }

            Query = "ALTER TABLE " + tablemu + ".dbo.ExtendedWarehouse ALTER COLUMN Items varbinary(1200)";
            Success = db.Exec(Query);
            if (Success == false)
            {
                LogAddDn("[Error] Failed Modify ExtendedWarehouse_Items Column: " + db.ExError.Message);
            }
            else
            {
                LogAddDn("[Done] Success Modify ExtendedWarehouse_Items Column");
            }

            Query = "ALTER TABLE " + tablemu + ".dbo.GuildWarehouse ALTER COLUMN Items varbinary(1200)";
            Success = db.Exec(Query);
            if (Success == false)
            {
                LogAddDn("[Error] Failed Modify GuildWarehouse_Items Column: " + db.ExError.Message);
            }
            else
            {
                LogAddDn("[Done] Success Modify GuildWarehouse_Items Column");
            }

            LogAddDn("TASK COMPLETED");

            LogAddDn("");
            MessageBox.Show("Titan DB Update Completed!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string Query = "";
            bool Success = false;

            LogAddUp("=============================================================");
            LogAddUp("[" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "](" + DateTime.Now.ToShortTimeString() + ") " + Application.ProductName + " VERSION: " + Application.ProductVersion);
            LogAddUp("=============================================================");
            LogAddUp("            INVENTORY AND VAULT UPGRADE (eX700)              ");
            LogAddUp("-------------------------------------------------------------");
            LogAddUp("");

            Query = "ALTER TABLE " + tablemu + ".dbo.Character ALTER COLUMN Inventory varbinary(3792)";
            Success = db.Exec(Query);
            if (Success == false)
            {
                LogAddUp("[Error] Failed Modify Inventory Column: " + db.ExError.Message);
            }
            else
            {
                LogAddUp("[Done] Success Modify Inventory Column");
            }

            Query = "ALTER TABLE " + tablemu + ".dbo.Character ALTER COLUMN SCFMasterSkill varbinary(300)";
            Success = db.Exec(Query);
            if (Success == false)
            {
                LogAddUp("[Error] Failed Modify SCFMasterSkill Column: " + db.ExError.Message);
            }
            else
            {
                LogAddUp("[Done] Success Modify SCFMasterSkill Column");
            }
        }
    }
}
