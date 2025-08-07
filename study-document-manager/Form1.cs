using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace study_document_manager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy HH:mm:ss";
        }

        private void tongsotailieu_Paint(object sender, PaintEventArgs e)
        {
            // S?a: Gán giá tr? cho Text property c?a label thay v? gán cho event handler
            if (sender is Label label)
            {
                label.Text = GetTotalDocumentCount().ToString();
            }
        }

        private void daduyet_Paint(object sender, PaintEventArgs e)
        {

        }

        private void saphethan_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tongtailieu_Click(object sender, EventArgs e)
        {
            // S?a: Gán giá tr? cho Text property c?a label thay v? gán cho event handler
            if (sender is Label label)
            {
                label.Text = GetTotalDocumentCount().ToString();
            }
        }

        // Thêm method GetTotalDocumentCount
        private int GetTotalDocumentCount()
        {
            // Placeholder - b?n có th? thay th? b?ng logic th?c t? đ? đ?m tài li?u
            return 42; // Ví d?: tr? v? 42 tài li?u
        }
    }
}