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
        private SQLiteCommandBuilder cmdBlder;
        private SQLiteConnection conn;
        private SQLiteDataAdapter adap;
        private System.Data.DataSet ds;
        private string database;

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
                    // using (SQLiteConnection db = new SQLiteConnection(@"Data Source=" + curAssemblyFolder + ";Pooling=true;FailIfMissing=false;Version=3"))
                    // {
                    //     db.Open();
                    //     var selectedDatabaseQuery = new SQLiteCommand($"select * from {selectedDatabase}", db);
                    //     SQLiteDataReader finalQuery = selectedDatabaseQuery.ExecuteReader();
                    //     DataTable dataTable1 = new DataTable();
                    //     foreach (DataColumn column in dataTable1.Columns) 
                    //     {
                    //         column.MaxLength = 255;
                    //         column.AllowDBNull = true;
                    //     }
                    //     dataTable1.Load(finalQuery);
                    //     dataGridView1.DataSource = dataTable1;
                    // }

                    conn = new SQLiteConnection();
                    conn.ConnectionString = @"Data Source=" + curAssemblyFolder +
                                            ";Pooling=true;FailIfMissing=false;Version=3";
                    conn.Open();
                    string sqlQuery = $"select * from {selectedDatabase}";
                    adap = new SQLiteDataAdapter(sqlQuery, conn);
                    
                    adap.TableMappings.Add("Table", "LNK_Elements_Components");
                    adap.TableMappings.Add("Table1", "LNK_Segments_Elements");
                    adap.TableMappings.Add("Table2", "MST_Components");
                    adap.TableMappings.Add("Table3", "MST_Elements");
                    adap.TableMappings.Add("Table4", "MST_RefTables");
                    adap.TableMappings.Add("Table5", "MST_Segments");

                    ds = new System.Data.DataSet();
                    adap.Fill(ds, selectedDatabase);
                    dataGridView1.DataSource = ds.Tables[selectedDatabase];
                    database = selectedDatabase;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        //private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{

        //}
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                cmdBlder = new SQLiteCommandBuilder(adap);
                adap.Update(ds, database);
                ds.AcceptChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                cmdBlder = new SQLiteCommandBuilder(adap);
                ds.AcceptChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
