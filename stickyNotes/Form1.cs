
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using VisioForge.Shared.MediaFoundation.OPM;

namespace stickyNotes
{
    public partial class Form1 : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        static bool textHumanChange = false;

        static Color bgcolor;
        static double op;
        static string NoteIndex = "0";
        Note note = new Note();

        public Form1()
        {

            textHumanChange = false;
            InitializeComponent();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.FormBorderStyle = FormBorderStyle.None;
            noteTextBox.ContextMenuStrip = contextMenuStrip1;
            headerPanel.ContextMenuStrip = contextMenuStrip1;
            headerPanel.BackColor = Color.FromArgb(7, headerPanel.BackColor);
            bgcolor = this.BackColor;
            op = this.Opacity;


            string line = System.IO.File.ReadAllText("Pointer.txt");
            NoteIndex = line;
            string line2 = System.IO.File.ReadAllText("Holder.txt");

            if (Convert.ToInt32(line) < Convert.ToInt32(line2))
            {

                note.readNoteFromJson(line);
                System.IO.File.WriteAllText("Pointer.txt", (Convert.ToInt32(line) + 1).ToString());
                titalLabel.Text = note.title;
                noteTextBox.Text = note.noteText;
                if (note.starred)
                {
                    starLabel.Visible = true;
                    starredToolStripMenuItem.Checked = true;
                }
                if (note.locked)
                {
                    noteTextBox.ReadOnly = true;
                    label1.Visible = true;
                    lockNoteToolStripMenuItem.Checked = true;

                }

                this.BackColor = note.bgcolor;
                noteTextBox.BackColor = note.bgcolor;
                this.Opacity = (note.op) / 100;
                if (note.ontop)
                {
                    this.TopMost = true;
                    alwaysOnTopToolStripMenuItem.Checked = true;
                }
                textHumanChange = true;

            } 
            else
            {
                note.createNewNote();
                note.readNoteFromJson(line);
                System.IO.File.WriteAllText("Pointer.txt", (Convert.ToInt32(line) + 1).ToString());
                titalLabel.Text = note.title;
                noteTextBox.Text = note.noteText;
                if (note.starred)
                {
                    starLabel.Visible = true;
                    starredToolStripMenuItem.Checked = true;
                }
                if (note.locked)
                {
                    noteTextBox.ReadOnly = true;
                    label1.Visible = true;
                    lockNoteToolStripMenuItem.Checked = true;
                }

                this.BackColor = note.bgcolor;
                noteTextBox.BackColor = note.bgcolor;
                this.Opacity = (note.op) / 100;
                if (note.ontop)
                {
                    this.TopMost = true;

                    alwaysOnTopToolStripMenuItem.Checked = true;
                }
                textHumanChange = true;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (!this.Name.Equals("first"))
            {
                Program.iList.ElementAt(Convert.ToInt32(this.Name)).Hide();


            }
            else
            {

                Program.iList.ElementAt(0).Hide();


            }


        }
        public void deleteNote()
        {


            int holder = 0;
            if (!this.Name.Equals("first"))
            {


                File.Delete(this.Name + ".json");
                holder = Convert.ToInt32(this.Name);
            }
            else
            {


                File.Delete("0" + ".json");
                holder = 0;

            }


            holder++;
            string l = System.IO.File.ReadAllText("Holder.txt");
            string ll = System.IO.File.ReadAllText("Pointer.txt");

            int counter = (Convert.ToInt32(l) - 1) - Convert.ToInt32(ll);
            for (int i = holder; i <= ((Convert.ToInt32(l) - 1)); i++)
            {
                string sourceFile = i + ".json";

                System.IO.FileInfo fi = new System.IO.FileInfo(sourceFile);


                if (fi.Exists)
                {

                    fi.MoveTo(i - 1 + ".json");
                }
            }

            if (!this.Name.Equals("first"))
            {
                Program.iList.ElementAt(Convert.ToInt32(this.Name)).Hide();


            }
            else
            {

                Program.iList.ElementAt(0).Hide();



            }

            System.IO.File.WriteAllText("Holder.txt", (Convert.ToInt32(l) - 1).ToString());
            System.IO.File.WriteAllText("Pointer.txt", (Convert.ToInt32(l) - 1).ToString());



        }
        private void dToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.LightBlue;
            noteTextBox.BackColor = Color.LightBlue;

        }

        private void menuB_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            contextMenuStrip1.Show(ptLowerLeft);
        }



        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 a = new Form1();
            Program.formCounter++;
            a.Name = Program.formCounter + "";

            Program.iList.Add(a);
            a.Show();
        }





        private void c1_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem btnSender = (ToolStripMenuItem)sender;
            this.BackColor = btnSender.BackColor;
            noteTextBox.BackColor = btnSender.BackColor;

            bgcolor = btnSender.BackColor;
            note.bgcolor = bgcolor;
            if (!this.Name.Equals("first"))
            {


                note.updateBgColor(this.Name, bgcolor.R, bgcolor.G, bgcolor.B);
            }
            else
            {


                note.updateBgColor("0", bgcolor.R, bgcolor.G, bgcolor.B);

            }


        }
        private void c1_e(object sender, EventArgs e)
        {
            ToolStripMenuItem btnSender = (ToolStripMenuItem)sender;
            this.BackColor = btnSender.BackColor;
            noteTextBox.BackColor = btnSender.BackColor;

        }
        private void c1_l(object sender, EventArgs e)
        {
            this.BackColor = bgcolor;
            noteTextBox.BackColor = bgcolor;

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

        private void o10_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem btnSender = (ToolStripMenuItem)sender;
            this.Opacity = (Convert.ToDouble(btnSender.Text) / 100);

            op = (Convert.ToDouble(btnSender.Text) / 100);
            note.op = op * 100;
            if (!this.Name.Equals("first"))
            {


                note.updateValues(this.Name);
            }
            else
            {


                note.updateValues("0");

            }


        }
        private void o10_e(object sender, EventArgs e)
        {
            ToolStripMenuItem btnSender = (ToolStripMenuItem)sender;
            this.Opacity = (Convert.ToDouble(btnSender.Text)) / 100;

        }
        private void o10_l(object sender, EventArgs e)
        {

            this.Opacity = op;
        }

        private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!alwaysOnTopToolStripMenuItem.Checked)
            {
                this.TopMost = true;
                alwaysOnTopToolStripMenuItem.Checked = true;
                note.ontop = true; ;
                if (!this.Name.Equals("first"))
                {


                    note.updateValues(this.Name);

                }
                else
                {


                    note.updateValues("0");

                }

            }
            else
            {
                this.TopMost = false;
                alwaysOnTopToolStripMenuItem.Checked = false;
                note.ontop = false;

                if (!this.Name.Equals("first"))
                {


                    note.updateValues(this.Name);

                }
                else
                {


                    note.updateValues("0");

                }

            }
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.Name.Equals("first"))
            {
                Program.iList.ElementAt(Convert.ToInt32(this.Name)).Hide();


            }
            else
            {

                Program.iList.ElementAt(0).Hide();


            }
        }

        private void starredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!starredToolStripMenuItem.Checked)
            {
                starLabel.Visible = true;
                starredToolStripMenuItem.Checked = true;
                note.starred = true;
                if (!this.Name.Equals("first"))
                {


                    note.updateValues(this.Name);

                }
                else
                {


                    note.updateValues("0");

                }
            }
            else
            {
                starLabel.Visible = false;

                starredToolStripMenuItem.Checked = false;
                note.starred = false;
                if (!this.Name.Equals("first"))
                {


                    note.updateValues(this.Name);

                }
                else
                {


                    note.updateValues("0");

                }

            }
        }





        private void deleteNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteNote();
        }

        private void newNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 a = new Form1();
            Program.formCounter++;
            a.Name = Program.formCounter + "";

            Program.iList.Add(a);
            a.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
         ntf.Visible = false;

            Application.Exit();
        }

        private void openAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int l = Convert.ToInt32(System.IO.File.ReadAllText("Holder.txt"));
            System.IO.File.WriteAllText("Pointer.txt", "0");


            if (l > 0)
            {
                for (int i = 0; i < l; i++)
                {


                    Program.iList.ElementAt(i).StartPosition = FormStartPosition.Manual;
                    Program.iList.ElementAt(i).Left = Program.X - (13 * (i + 1));
                    Program.iList.ElementAt(i).Top = Program.Y;


                    Program.iList.ElementAt(i).Show();

                }
            }


        }

        private void ntf_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int l = Convert.ToInt32(System.IO.File.ReadAllText("Holder.txt"));
            System.IO.File.WriteAllText("Pointer.txt", "0");


            if (l > 0)
            {
                for (int i = 0; i < l; i++)
                {


                    Program.iList.ElementAt(i).StartPosition = FormStartPosition.Manual;
                    Program.iList.ElementAt(i).Left = Program.X - (13 * (i + 1));
                    Program.iList.ElementAt(i).Top = Program.Y;


                    Program.iList.ElementAt(i).Show();

                }
            }

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutForm aboutform = new aboutForm();
            aboutform.Show();
        }

        private void lockNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {


            if (!lockNoteToolStripMenuItem.Checked)
            {
                label1.Visible = true;
                this.noteTextBox.ReadOnly = true;
                this.lockNoteToolStripMenuItem.Checked = true;

                note.locked = true;
                if (!this.Name.Equals("first"))
                {


                    note.updateValues(this.Name);

                }
                else
                {


                    note.updateValues("0");

                }

            }
            else
            {
                label1.Visible = false;

                this.noteTextBox.ReadOnly = false;

                this.lockNoteToolStripMenuItem.Checked = false;
                note.locked = false;
                if (!this.Name.Equals("first"))
                {


                    note.updateValues(this.Name);

                }
                else
                {


                    note.updateValues("0");

                }

            }
        }
        private void noteTextBox_TextChanged(object sender, EventArgs e)
        {
            if (textHumanChange)
            {
                TextBox noteText = (TextBox)sender;



                note.noteText = noteText.Text;

                if (!this.Name.Equals("first"))
                {


                    note.updateValues(this.Name);

                }
                else
                {


                    note.updateValues("0");

                }



            }

        }

        private void tToolStripMenuItem_Click(object sender, EventArgs e)
        {
            titleChangePanel.Visible = true;
            titleChangePanel.BringToFront();
            titleTextBox.Text = titalLabel.Text;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            note.title = titleTextBox.Text;
            titalLabel.Text = titleTextBox.Text;
            if (!this.Name.Equals("first"))
            {


                note.updateValues(this.Name);

            }
            else
            {


                note.updateValues("0");

            }
            titleChangePanel.Visible = false;
            titleChangePanel.SendToBack();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            titleChangePanel.Visible = false;
            titleChangePanel.SendToBack();
        }



        private void button5_Click(object sender, EventArgs e)
        {
            alarmPanel.Visible = false;
            alarmPanel.SendToBack();
        }

        private void eToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dateNow = DateTime.Now;
            HTextBox.Value = dateNow.Hour;
            MTextBox.Value = dateNow.Minute;

            alarmPanel.Visible = true;
            alarmPanel.BringToFront();
        }
        public static bool withSound = false;
        private void button4_Click(object sender, EventArgs e)
        {
            if (soundCH.Checked) { withSound = true; }
            var dateNow = DateTime.Now;
            var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, (int)HTextBox.Value, (int)MTextBox.Value, 0);
            AlarmClock clock = new AlarmClock(date);
            clock.Alarm += (senderr, ee) => alarm();
            alarmPanel.Visible = false;
            alarmPanel.SendToBack();
        }
        public void alarm()
        {

            if (withSound)
            {

                System.Media.SoundPlayer sp = new System.Media.SoundPlayer(@"..\..\Resources\sound.wav");
                sp.Play();


            }

            MessageBox.Show("The Alert of note ( " + titalLabel.Text + " )", "alert");
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.N))
            {
                aToolStripMenuItem.PerformClick();

                return true;
            }
            else if (keyData == (Keys.Alt | Keys.A))
            {
                eToolStripMenuItem.PerformClick();

                return true;
            }
            else if (keyData == (Keys.Alt | Keys.E))
            {
                tToolStripMenuItem.PerformClick();

                return true;
            }
            else if (keyData == (Keys.Alt | Keys.S))
            {
                starredToolStripMenuItem.PerformClick();

                return true;
            }
            else if (keyData == (Keys.Alt | Keys.T))
            {
                alwaysOnTopToolStripMenuItem.PerformClick();

                return true;
            }
            else if (keyData == (Keys.Alt | Keys.L))
            {
                lockNoteToolStripMenuItem.PerformClick();

                return true;
            }
            else if (keyData == (Keys.Alt | Keys.H))
            {
                hideToolStripMenuItem.PerformClick();

                return true;
            }
            else if (keyData == (Keys.Alt | Keys.D))
            {
                deleteNoteToolStripMenuItem.PerformClick();

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }




    }
}
