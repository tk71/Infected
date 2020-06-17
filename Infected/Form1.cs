using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infected;
using InfectedLibrary;
using InfectedLibrary.Models;

namespace Infected
{
    public partial class Form1 : Form
    {
        private Scenario _scenario;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var scenarioFile = Application.StartupPath + @"\scenario.json";

            _scenario = new Scenario();

            // check for a scenario file
            if (!File.Exists(scenarioFile))
            {
                // if no scenario file, then attempt to create one with default floors
                _scenario.Floors = Defaults.Floors();
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var sw = new StreamWriter(scenarioFile))
                        {
                            var serializer = new DataContractJsonSerializer(_scenario.Floors.GetType());
                            serializer.WriteObject(ms, _scenario.Floors);
                            sw.Write(Encoding.ASCII.GetString(ms.ToArray()));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not save scenario file." + Environment.NewLine + Environment.NewLine +
                        "Error: " + ex.HResult + " - " + ex.Message, "Scenario File Issue", MessageBoxButtons.OK);
                }
            }
            else
            {
                // a scenario file exists, so attempt to load it
                using (var sr = new StreamReader(scenarioFile))
                {
                    using (var ms = sr.BaseStream)
                    {
                        var serializer = new DataContractJsonSerializer(_scenario.Floors.GetType());
                        _scenario.Floors = serializer.ReadObject(ms) as List<Floor>;

                        if (_scenario is null)
                        {
                            MessageBox.Show("Could not load scenario file.",
                                "Scenario File Issue", MessageBoxButtons.OK);
                            _scenario.Floors = Defaults.Floors();
                        }
                    }
                }
            }

            _scenario.RunScenario();
            
            // export logs
            var logs = _scenario.Logs;
            var outputFile = Application.StartupPath + @"\output.csv";
            if (File.Exists(outputFile)) File.Delete(outputFile);
            using (var sw = new StreamWriter(outputFile))
            {
                sw.WriteLine("Log Date,Log Time,Employee Id,First Name,Last Name,Sex,Room Number,Room Type,Status,Exposure List");
                foreach (var log in logs)
                {
                    var contacts = new StringBuilder();
                    foreach (var contact in log.Contacts)
                    {
                        if (contacts.Length != 0) contacts.Append(",");
                        contacts.Append(contact.Id + " " + contact.FirstName + " " + contact.LastName);
                    }
                    sw.WriteLine(log.Created.ToString("MM-dd-yyyy") + "," + log.Created.ToString("hh:mm tt") + "," + 
                        log.Id + "," + log.FirstName + ","+ log.LastName + "," + log.Sex + "," +
                        log.CurrentRoom + "," + log.CurrentRoomType + "," + log.Status + "," + contacts);
                }
            }
        }
    }
}
