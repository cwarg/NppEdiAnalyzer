using Kbg.NppPluginNET.PluginInfrastructure;
using System;
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
            //this.myEDIAnalizer.setEDIAnalizer(editor);
        }

        /*
        private void button1_Click(object sender, EventArgs e)
        {
            int line;
            if (!int.TryParse(textBox1.Text, out line))
                return;
            editor.EnsureVisible(line - 1);
            editor.GotoLine(line - 1);
            editor.GrabFocus();
        }
        */

        /*
        private void frmGoToLine_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyData == Keys.Return) || (e.Alt && (e.KeyCode == Keys.G)))
            {
                button1.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyData == Keys.Escape)
            {
                editor.GrabFocus();
            }
            else if (e.KeyCode == Keys.Tab)
            {
                Control next = GetNextControl((Control)sender, !e.Shift);
                while ((next == null) || (!next.TabStop)) next = GetNextControl(next, !e.Shift);
                next.Focus();
                e.Handled = true;
            }
        }
        */

        void FrmDatabaseEditorVisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK,
                                  PluginBase._funcItems.Items[Main.idFrmDatabaseEditor]._cmdID, 0);
            }
        }

        /*
        private void button2_Click(object sender, EventArgs e)
        {

            //Util.FindNext(textBox2.Text,editor);
            //Util.ReplaceNext(textBox2.Text, textBox3.Text, editor);
            Util.ReplaceAll(textBox2.Text, textBox3.Text, editor);

        }
        */

        /*
        private void button4_Click(object sender, EventArgs e)
        {
            //
            // This is the first node in the view.
            //
            TreeNode treeNode = new TreeNode("Windows");
            treeView1.Nodes.Add(treeNode);
            //
            // Another node following the first node.
            //
            treeNode = new TreeNode("Linux");
            treeView1.Nodes.Add(treeNode);
            //
            // Create two child nodes and put them in an array.
            // ... Add the third node, and specify these as its children.
            //
            TreeNode node2 = new TreeNode("C#");
            TreeNode node3 = new TreeNode("VB.NET");
            TreeNode[] array = new TreeNode[] { node2, node3 };
            //
            // Final node.
            //
            treeNode = new TreeNode("Dot Net Perls", array);
            treeView1.Nodes.Add(treeNode);
        }
        */

        /*
        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //
            // Get the selected node.
            //
            TreeNode node = treeView1.SelectedNode;
            //
            // Render message box.
            //
            MessageBox.Show(string.Format("You selected: {0}", node.Text));
        }
        */

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


        /*
private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
{
//
// Get the selected node.
//
TreeNode node = treeView1.SelectedNode;
//
// Render message box.
//
MessageBox.Show(string.Format("You selected: {0}", node.Text));
}
*/

    }
}
