using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Homework
{
    public partial class MainForm : Form
    {
        private OpenFileDialog oDialog;
        public MainForm()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            this.oDialog = new OpenFileDialog();
            oDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            oDialog.ShowDialog();
            string path = oDialog.FileName;
            string extension = Path.GetExtension(path);


            if (extension == ".json")
            {
                using (var reader = new StreamReader(path))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var root = JToken.Load(jsonReader);
                    TreeViewDisplayer(root, Path.GetFileNameWithoutExtension(path));
                }
            }
        }

        private void TreeViewDisplayer(JToken root, string rootName)
        {
            JSONTreeView.BeginUpdate();
            try
            {
                JSONTreeView.Nodes.Clear();
                var tNode = JSONTreeView.Nodes[JSONTreeView.Nodes.Add(new TreeNode(rootName))];
                tNode.Tag = root;

                AddNode(root, tNode);
            }
            finally
            {
                JSONTreeView.EndUpdate();
            }

        }
        private void AddNode(JToken token, TreeNode inTreeNode)
        {
            if (token == null)
                return;
            if (token is JValue)
            {
                var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(token.ToString() + "<" + token.Type.ToString() + ">"))];
                childNode.Tag = token;
            }
            else if (token is JObject)
            {
                var obj = (JObject)token;
                foreach (var property in obj.Properties())
                {
                    var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(property.Name))];
                    childNode.Tag = property;
                    AddNode(property.Value, childNode);
                }
            }
            else if (token is JArray)
            {
                var array = (JArray)token;
                for (int i = 0; i < array.Count; i++)
                {
                    var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(i.ToString() + "<" + token.Type.ToString() + ">"))];
                    childNode.Tag = array[i];
                    AddNode(array[i], childNode);
                }
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
