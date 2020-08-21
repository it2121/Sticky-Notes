using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stickyNotes
{
    class Note
    {

        public string title { get; set; }
        public string noteText { get; set; }
        public bool starred { get; set; }
        public bool locked { get; set; }
        public Color bgcolor { get; set; }
        public double op { get; set; }
        public bool ontop { get; set; }

        public void createNewNote()
        {
            if (Convert.ToInt32(getNoteCount()) != 0)
            {
                title = "New Note (" + getNoteCount() + ")";
            }
            else
            {
                title = "New Note";
            }
            noteText = "";
            starred = false;
            locked = false;
            bgcolor = Color.FromArgb(255, 255, 153);

            op = 100;
            ontop = false;
            writeNewNoteAsJson(getFileName());
        }

        private string getNoteCount()
        {

            string line = System.IO.File.ReadAllText("Holder.txt");


            return line;


        }

        private string getFileName()
        {

            string line = System.IO.File.ReadAllText("Holder.txt");

            System.IO.File.WriteAllText("Holder.txt", (Convert.ToInt32(line) + 1).ToString());

            return line;


        }

        public void writeNewNoteAsJson(string filename)
        {


            using (StreamWriter file = File.CreateText(filename + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, this);
            }


        }
        public void readNoteFromJson(string filename)
        {
            string json = File.ReadAllText(filename + ".json");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            this.title = jsonObj["title"];
            this.noteText = jsonObj["noteText"];
            this.starred = jsonObj["starred"];
            this.locked = jsonObj["locked"];
            this.bgcolor = (jsonObj["bgcolor"]);
            this.op = jsonObj["op"];
            this.ontop = jsonObj["ontop"];
        }
        public void updateValues(string filename)
        {

            string json = File.ReadAllText(filename + ".json");

            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            jsonObj["title"] = this.title;
            jsonObj["noteText"] = this.noteText;
            jsonObj["starred"] = this.starred;
            jsonObj["locked"] = this.locked;

            jsonObj["op"] = this.op;
            jsonObj["ontop"] = this.ontop;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filename + ".json", output);


        }

        public void updateBgColor(string filename, int R = 255, int G = 255, int B = 192)
        {

            string json = File.ReadAllText(filename + ".json");

            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            jsonObj["bgcolor"] = R + "," + G + "," + B;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filename + ".json", output);

        }

    }
}
