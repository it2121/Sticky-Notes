using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stickyNotes
{
    static class Program
    {
        public static List<Form1> iList = new List<Form1>();
        public static int formCounter = -1;
        public static int X = 0;
        public static int Y = 0;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.IO.File.WriteAllText("Pointer.txt", "0");


            Form1 form = new Form1();


            formCounter++;
            form.Name = "first";


            iList.Add(form);

            var myLoginForm = form;
            myLoginForm.Show();
            X = form.Location.X;
            Y = form.Location.Y;
            int l = Convert.ToInt32(System.IO.File.ReadAllText("Holder.txt"));

            l--;

            if (l > 0)
            {
                for (int i = 0; i < l; i++)
                {
                    Form1 a = new Form1();
                    formCounter++;
                    a.Name = formCounter + "";


                    a.StartPosition = FormStartPosition.Manual;
                    a.Left = form.Location.X - (13 * (i + 1));
                    a.Top = form.Location.Y;


                    iList.Add(a);
                    a.Show();
                }
            }

            form.ntf.Visible = true;
            Application.Run();
        }
    }
}
