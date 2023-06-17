using System;
using System.Windows.Forms;

namespace Work3
{

    public partial class InputForm : System.Windows.Forms.Form
    {
        public double doubleHeight { get; set; }
        public double doubleWidth { get; set; }

        public InputForm(double DoubleHeight, double DoubleWidth)
        {
            InitializeComponent();

            doubleHeight = DoubleHeight;
            doubleWidth = DoubleWidth;

            txtHeight.Text = doubleHeight.ToString();
            txtWidth.Text = doubleWidth.ToString();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtHeight.Text, out double height) && double.TryParse(txtWidth.Text, out double width))
            {
                doubleHeight = height;
                doubleWidth = width;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректные числовые значения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
