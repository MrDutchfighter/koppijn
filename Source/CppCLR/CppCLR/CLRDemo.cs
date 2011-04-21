using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CppLib;

namespace CppCLR
{
    public partial class CLRDemo : Form
    {
        public CLRDemo()
        {
            InitializeComponent();
        }

        private void BtnCalcSquareClick(object sender, EventArgs e)
        {
            try
            {
                NativeClassWrapper wrapper = new NativeClassWrapper();

                int input = Convert.ToInt32(tbIntNumber.Text);

                lblOutput.Text = wrapper.SquareWrapper(input).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGetListClick(object sender, EventArgs e)
        {
            NativeClassWrapper wrapper = new NativeClassWrapper();
            List<int> list = wrapper.GetNumbersWrapper();
            foreach(int i in list)
            {
                lvIntList.Items.Add(new ListViewItem(i.ToString()));
            }
        }

        private void BtnPersonClick(object sender, EventArgs e)
        {
            try
            {
                string name = tbPersonName.Text;
                int age = Convert.ToInt32(tbPersonAge.Text);
                PersonWrapper person = new PersonWrapper(name, age);

                //Properties work
                person.Name = tbPersonName.Text;

                tbPersonOutput.Text = person.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
