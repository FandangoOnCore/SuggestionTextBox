using System;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            suggestionTextBox1.AutoCompleteCustomSource = new AutoCompleteStringCollection()
            {
               "Suggestion Box test",
               "Neque porro quisquam est",
               "qui dolorem ipsum quia",
               "dolor sit amet",
               "consectetur",
               "adipisci velit",
               "R.I.P. Chris Cornell",
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
