using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InfectedLibrary;
using InfectedLibrary.Models;

namespace Infected
{
    public partial class Form1 : Form
    {
        private string _scenarioFile;
        private Scenario _scenario;
        private BindingSource _bindingSource;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _scenarioFile = Application.StartupPath + @"\scenario.json";
            _scenario = new Scenario();
            _bindingSource = new BindingSource();

            btnExport.Enabled = false;
            
            // check for a scenario file
            if (!File.Exists(_scenarioFile))
            {
                // if no scenario file, then attempt to create one with default floors
                _scenario.Floors = Defaults.Floors();
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var sw = new StreamWriter(_scenarioFile))
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
                using (var sr = new StreamReader(_scenarioFile))
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

            // use data grid to show floors
            dgvScenario.AutoGenerateColumns = false;
            dgvScenario.AllowUserToAddRows = false;
            dgvScenario.AllowUserToDeleteRows = false;
            dgvScenario.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

            dgvScenario.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "EmployeesAssigned",
                Name = "Employees"
            });
            dgvScenario.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "OfficeRooms",
                Name = "Offices"
            });
            dgvScenario.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Breakrooms",
                Name = "Breakrooms"
            });
            dgvScenario.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "MeetingRooms",
                Name = "Meeting Rooms"
            });

            _scenario.Floors.ForEach(floor => _bindingSource.Add(floor));
            dgvScenario.DataSource = _bindingSource;
            dgvScenario.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // infection starting floor
            numGroundZero.Maximum = _bindingSource.Count;
            if (numGroundZero.Maximum > 0) numGroundZero.Value = 1;
            numGroundZero.Minimum = 1;

            // save file dialog settings
            saveFileDialog1.Filter = "Comma Separated File|CSV";
            saveFileDialog1.FileName = "output.csv";
            saveFileDialog1.Title = "Save Logs";
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog1.OverwritePrompt = true;

            // date picker defaults
            dtStart.Value = DateTime.Today;
            dtEnd.Value = DateTime.Today.AddMonths(4);
        }

        private void btnAddFloor_Click(object sender, EventArgs e)
        {
            _bindingSource.Add(new Floor());
            numGroundZero.Maximum = _bindingSource.Count;
        }

        private void btnDeleteFloor_Click(object sender, EventArgs e)
        {
            if (dgvScenario.CurrentCell is null) return;
            var selected = dgvScenario.CurrentCell.RowIndex;
            if (selected == -1) return;
            _bindingSource.RemoveAt(selected);
            numGroundZero.Maximum = _bindingSource.Count;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // save any changes made to the scenario in the datagrid
            var floors = _bindingSource.List;
            _scenario.Floors.Clear();
            foreach (Floor floor in floors)
            {
                _scenario.Floors.Add(floor);
            }

            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var sw = new StreamWriter(_scenarioFile))
                    {
                        var serializer = new DataContractJsonSerializer(_scenario.Floors.GetType());
                        serializer.WriteObject(ms, _scenario.Floors);
                        sw.Write(Encoding.ASCII.GetString(ms.ToArray()));
                    }
                }
                MessageBox.Show("Scenario was saved.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save scenario file." + Environment.NewLine + Environment.NewLine +
                    "Error: " + ex.HResult + " - " + ex.Message, "Scenario File Issue", MessageBoxButtons.OK);
            }
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            // grab any changes made to the scenario in the datagrid and then run the scenario
            var floors = _bindingSource.List;
            _scenario.Floors.Clear();
            foreach (Floor floor in floors)
            {
                _scenario.Floors.Add(floor);
            }

            _scenario.StartDate = dtStart.Value;
            _scenario.EndDate = dtEnd.Value;
            _scenario.VariableInfectionRate = chkVariableRate.Checked;
            _scenario.InfectionStartingFloor = Convert.ToInt32(numGroundZero.Value);

            btnAddFloor.Enabled = false;
            btnDefaults.Enabled = false;
            btnDeleteFloor.Enabled = false;
            btnExport.Enabled = false;
            btnRun.Enabled = false;
            btnSave.Enabled = false;

            lblStatus.Text = "processing";

            try
            {
                await Task.Run(() => _scenario.RunScenario());
            }
            catch (Exception) { }
            finally
            {
                lblStatus.Text = "ready";

                btnAddFloor.Enabled = true;
                btnDefaults.Enabled = true;
                btnDeleteFloor.Enabled = true;
                btnExport.Enabled = true;
                btnRun.Enabled = true;
                btnSave.Enabled = true;

                MessageBox.Show("Scenario processing complete.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // export logs
            var result = saveFileDialog1.ShowDialog();
            if (result != DialogResult.OK) return;
            
            var outputFile = saveFileDialog1.FileName;
            if (File.Exists(outputFile)) File.Delete(outputFile);

            var logs = _scenario.Logs;

            try
            {
                using (var sw = new StreamWriter(outputFile))
                {
                    sw.WriteLine("Log Date,Log Time,Employee Id,First Name,Last Name,Sex,Infection Chance,Infected With,Room Number,Room Type,Status,Exposure List");
                    foreach (var log in logs)
                    {
                        var contacts = new StringBuilder();
                        foreach (var contact in log.Contacts)
                        {
                            if (contacts.Length != 0) contacts.Append(",");
                            contacts.Append(contact.Id + " " + contact.FirstName + " " + contact.LastName);
                        }
                        sw.WriteLine(log.Created.ToString("MM-dd-yyyy") + "," + log.Created.ToString("hh:mm tt") + "," +
                            log.Id + "," + log.FirstName + "," + log.LastName + "," + log.Sex + "," +
                            log.ChanceOfInfection + "," + (log.InfectedPercent == 0 ? string.Empty : log.InfectedPercent.ToString()) + "," +
                            log.CurrentLocation + "," + log.CurrentLocationType + "," + log.Status + "," + contacts);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save log data to file." + Environment.NewLine + Environment.NewLine +
                    "Error: " + ex.HResult + " - " + ex.Message, "Export Issue", MessageBoxButtons.OK);
            }

            try
            {
                System.Diagnostics.Process.Start(saveFileDialog1.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open log file." + Environment.NewLine + Environment.NewLine +
                    "Error: " + ex.HResult + " - " + ex.Message, "File Opening Issue", MessageBoxButtons.OK);
            }
        }

        private void btnDefaults_Click(object sender, EventArgs e)
        {
            _bindingSource.Clear();
            Defaults.Floors().ForEach(floor => _bindingSource.Add(floor));
        }
    }
}
