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
        private Scenario _scenario = Defaults.Scenario();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var scenarioFile = Application.StartupPath + @"\scenario.json";

            // check for a building file
            if (!File.Exists(scenarioFile))
            {
                // if no building file, then attempt to create one with the default building
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var sw = new StreamWriter(scenarioFile))
                        {
                            var serializer = new DataContractJsonSerializer(_scenario.GetType());
                            serializer.WriteObject(ms, _scenario);
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
                // a building file exists, so attempt to load it
                using (var sr = new StreamReader(scenarioFile))
                {
                    using (var ms = sr.BaseStream)
                    {
                        var serializer = new DataContractJsonSerializer(_scenario.GetType());
                        _scenario = serializer.ReadObject(ms) as Scenario;

                        if (_scenario is null)
                        {
                            _scenario = Defaults.Scenario();
                            MessageBox.Show("Could not load scenario file.",
                                "Scenario File Issue", MessageBoxButtons.OK);
                        }
                    }
                }
            }

            _scenario.RunScenario();
            var logs = _scenario.Logs;
            var outputFile = Application.StartupPath + @"\output.csv";
            if (File.Exists(outputFile)) File.Delete(outputFile);
            using (var sw = new StreamWriter(outputFile))
            {
                sw.WriteLine("Log Date,Employee Id,First Name,Last Name," +
                        "Building Number,Floor Number,Room Number,Room Type,Infection Status,Exposure List");
                foreach (var log in logs)
                {
                    string contacts;
                    using (var ms = new MemoryStream())
                    {
                        var serializer = new DataContractJsonSerializer(log.Contacts.GetType());
                        serializer.WriteObject(ms, log.Contacts);
                        contacts = Encoding.ASCII.GetString(ms.ToArray());
                    }
                    sw.WriteLine(log.Created.ToString("MM-dd-yyyy") + "," + log.Id + "," + log.FirstName + ","+ log.LastName + "," +
                        log.Building + "," + log.Floor + "," + log.Room + "," + log.RoomType + "," + log.Status + "," + contacts);
                }
            }
        }
    }
}
