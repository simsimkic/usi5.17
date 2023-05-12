using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Repository;
using ZdravoCorp.Services;

namespace ZdravoCorp.Presentation.Common
{
    /// <summary>
    /// Interaction logic for MedicalRecordWindow.xaml
    /// </summary>
    public partial class MedicalRecordWindow : Window
    {
        private MedicalRecord _medicalRecord;
        private Dictionary<int, string> _reportHistory = new();
        DateTime _currentReportTimestamp;
        public MedicalRecordWindow(MedicalRecord medicalRecord)
        {
            InitializeComponent();
            this._medicalRecord = medicalRecord;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitDiseasesDataGrid();
            FillMedicalRecordWithValues();
            FillReportHistory();
        }

        private void FillMedicalRecordWithValues()
        {
            Domain.Users.Patient patient = PatientService.GetPatient(_medicalRecord.PatientUsername);
            firstNameLabel.Content = patient.FirstName;
            lastNameLabel.Content = patient.LastName;
            usernameLabel.Content = patient.Username;
            dateOfBirthLabel.Content = _medicalRecord.DateOfBirth;
            heightLabel.Content = _medicalRecord.Height;
            weightLabel.Content = _medicalRecord.Weight;
            FillDataGridWithDiseases();
        }

        private void InitDiseasesDataGrid()
        {
            var column = new DataGridTextColumn
            {
                Header = "Disease Name",
                Binding = new Binding()
            };

            diseasesDataGrid.Columns.Add(column);
        }

        private void FillDataGridWithDiseases()
        {
            diseasesDataGrid.Items.Clear();

            if (_medicalRecord.DiseaseHistory[0].Trim().Equals(""))
            {
                return;
            }

            foreach (var disease in _medicalRecord.DiseaseHistory)
            {
                diseasesDataGrid.Items.Add(disease);
            }
            diseasesDataGrid.Items.Refresh();
        }

        private void UpdateReportFields(DateTime previousReportTime, string previousReport)
        {
            dateOfReportLabel.Content = "Report from: " + previousReportTime.ToString("M/d/yyyy h:mm:ss tt");
            reportTextBox.Text = previousReport;
            _currentReportTimestamp = previousReportTime;
        }

        private void FillReportHistory()
        {
            _reportHistory = ReportHistoryService.GetReportHistory(_medicalRecord.PatientUsername);

            if (!_reportHistory.Any())
            {
                reportTextBox.IsReadOnly = true;
                return;
            }

            KeyValuePair<int, string> mostReacentReport = _reportHistory.MaxBy(x => x.Key);
            UpdateReportFields(AppointmentService.GetAppointment(mostReacentReport.Key).TimeSlot.Start, mostReacentReport.Value);

        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<int, string> report in _reportHistory.OrderBy(x => x.Key))
            {
                if (AppointmentService.GetAppointment(report.Key).TimeSlot.Start <= _currentReportTimestamp) continue;

                UpdateReportFields(AppointmentService.GetAppointment(report.Key).TimeSlot.Start, report.Value);
                break;
            }
        }

        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime previousReportTime = default;
            string previousReport = "";
            foreach (KeyValuePair<int, string> report in _reportHistory.OrderBy(x => x.Key))
            {
                if (AppointmentService.GetAppointment(report.Key).TimeSlot.Start < _currentReportTimestamp)
                {
                    previousReport = report.Value;
                    previousReportTime = AppointmentService.GetAppointment(report.Key).TimeSlot.Start;
                    continue;
                }

                if (previousReportTime == default) break;

                UpdateReportFields(previousReportTime, previousReport);
                break;
            }
        }
    }
}
