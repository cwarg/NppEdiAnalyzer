using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;


namespace Kbg.Demo.Namespace
{
    partial class frmDatabaseEditor : Form
    {
        private readonly IScintillaGateway editor;

        public frmDatabaseEditor(IScintillaGateway editor)
        {
            this.editor = editor;
            InitializeComponent();
        }

        private void cbDatabaseSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedDatabase = cbDatabaseSelector.SelectedItem.ToString();
            string curAssemblyFolder = new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            curAssemblyFolder = curAssemblyFolder.Replace(".dll", ".db");

            if (!string.IsNullOrEmpty(curAssemblyFolder))
            {
                try
                {
                    using (SQLiteConnection db = new SQLiteConnection(@"Data Source=" + curAssemblyFolder + ";Pooling=true;FailIfMissing=false;Version=3"))
                    {
                        db.Open();
                        var selectedDatabaseQuery = new SQLiteCommand($"select distinct * from {selectedDatabase}", db);
                        SQLiteDataReader finalQuery = selectedDatabaseQuery.ExecuteReader();
                        DataTable dataTable1 = new DataTable();
                        foreach (DataColumn column in dataTable1.Columns) 
                        {
                            column.MaxLength = 255;
                            column.AllowDBNull = true;
                        }
                        dataTable1.Load(finalQuery);
                        dataGridView1.DataSource = dataTable1;
                        
                        //TODO: CHANGE MAXLENGTH FROM NULL to -1 IN ALL TABLES
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }

        }

        void FrmDatabaseEditorVisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK,
                                  PluginBase._funcItems.Items[Main.idFrmDatabaseEditor]._cmdID, 0);
            }
        }

        private void button5_MouseClick(object sender, MouseEventArgs e)
        {
            /*
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string xmlFileName = Path.Combine(assemblyFolder,"AggregatorItems.xml");

            string curAssemblyFolder = new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
             */

            string curAssemblyFolder = new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            MessageBox.Show(string.Format("DLL path is: {0}", curAssemblyFolder));


        }

        //private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{

        //}
    }
}
