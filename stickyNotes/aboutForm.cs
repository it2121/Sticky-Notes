using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stickyNotes
{
    public partial class aboutForm : Form
    {
        public aboutForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.linkedin.com/in/it2121/");

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(" https://github.com/it2121");

       
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        public void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        public void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
    }
}
