using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Milestone1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tokenView.Columns.Add("string", "lexeme");
            tokenView.Columns.Add("token", "Token");

            errorView.Columns.Add("string", "lexeme");
        }

        private void scanBtn_Click(object sender, EventArgs e)
        {
            errorView.Rows.Clear();
            tokenView.Rows.Clear();
            scanBtn.Text = "Scanning...";
            Scanner scanner = new Scanner();
            List<Token> tokens = scanner.startScan(sourceTextBox.Text);
            
            foreach(Token token in tokens)
            {
                if(token.type != Token_class.Invalid)
                {
                    tokenView.Rows.Add(token.lex, token.type);
                }
                else
                {
                    errorView.Rows.Add(token.lex);
                }
            }

            scanBtn.Text = "Scan Code";
        }

        private void tokenView_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
