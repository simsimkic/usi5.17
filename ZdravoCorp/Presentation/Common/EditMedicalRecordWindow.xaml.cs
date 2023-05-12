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
using ZdravoCorp.Domain.Calendar;
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Presentation.Nurse;
using ZdravoCorp.Repository;
using ZdravoCorp.Services;

namespace ZdravoCorp.Presentation.Common
{
    /// <summary>
    /// Interaction logic for MedicalRecordWindow.xaml
    /// </summary>
    public partial class EditMedicalRecordWindow : Window
    {
        private readonly MedicalRecord _medicalRecord;
        private Dictionary<int, string> _reportHistory = new();
        private DateTime _currentReportTimestamp;
        private int _currentReportId;
        private int _appointmentId;
        public EditMedicalRecordWindow(MedicalRecord medicalRecord, int appointmentId = -1)
        {
            InitializeComponent();
            this._medicalRecord = medicalRecord;
            this._appointmentId = appointmentId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitDiseasesDataGrid();
            FillMedicalRecordWithValues();
            FillReportHistory();
            ChangeAppointmentStatus();
        }

        private void ChangeAppointmentStatus()
        {
            if (_appointmentId == -1) return;

            Appointment checkedInAppointment = AppointmentService.GetAppointment(_appointmentId);
            checkedInAppointment.Status = Appointment.AppointmentStatus.CheckedIn;
            checkedInAppointment.PatientUsername = _medicalRecord.PatientUsername!;
            AppointmentService.AddOrEditAppointment(checkedInAppointment);

            RefreshCheckInWindow();
        }

        private void RefreshCheckInWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is PatientCheckInWindow)
                {
                    window.Close();
                }
            }

            _ = new PatientCheckInWindow();
            this.Focus();
        }


        private void FillMedicalRecordWithValues()
        {
            Patient patient = PatientService.GetPatient(_medicalRecord.PatientUsername)!;
            firstNameTextBox.Text = patient.FirstName;
            lastNameTextBox.Text = patient.LastName;
            usernameTextBox.Text = patient.Username;

            var dateOnlyToDateTime = _medicalRecord.DateOfBirth.ToDateTime(new TimeOnly());
            dateOfBirthPicker.SelectedDate = DateTime.SpecifyKind(dateOnlyToDateTime, DateTimeKind.Unspecified);
            
            heightTextBox.Text = _medicalRecord.Height.ToString();
            weightTextBox.Text = _medicalRecord.Weight.ToString();
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

            if (_medicalRecord.DiseaseHistory.Count == 0) return;

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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (diseaseTextBox.Text.Trim() == "")
            {
                return;
            }

            diseasesDataGrid.Items.Add(diseaseTextBox.Text.Trim());
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = diseasesDataGrid.SelectedItem;

            if (selectedRow != null)
            {
                diseasesDataGrid.Items.Remove(selectedRow);
            }
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<int, string> report in _reportHistory.OrderBy(x => x.Key))
            {
                if (AppointmentService.GetAppointment(report.Key).TimeSlot.Start <= _currentReportTimestamp) continue;

                SaveAnyReportChanges();
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

                SaveAnyReportChanges();
                UpdateReportFields(previousReportTime, previousReport);
                break;
            }
        }

        private void SaveAnyReportChanges()
        {
            List<Appointment> appointments =
                AppointmentService.GetAllAppointmentsForPatient(_medicalRecord.PatientUsername);

            foreach (Appointment appointment in appointments)
            {
                if (appointment.TimeSlot.Start == _currentReportTimestamp)
                {
                    _reportHistory[appointment.Id] = reportTextBox.Text;
                }
            }
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AreFieldsValid()) return;
            
            SaveChanges();
            Notification.ShowSuccessDialog("Successfully modified patient's medical record!");
        }

        private void SaveChanges()
        {
            SaveMedicalRecord();
            SaveReportHistory();
        }

        private void SaveReportHistory()
        {
            SaveAnyReportChanges();
            ReportHistoryService.AddOrEditReportHistory(_medicalRecord.PatientUsername, _reportHistory);
        }

        private void SaveMedicalRecord()
        {
            DateOnly dateOfBirth = DateOnly.FromDateTime(dateOfBirthPicker.SelectedDate!.Value.Date);

            double weight = Convert.ToDouble(weightTextBox.Text);
            double height = Convert.ToDouble(heightTextBox.Text);

            string username = usernameTextBox.Text;

            var diseases = RowsToDiseases();

            MedicalRecordService.EditMedicalRecord(username, username, height, weight, dateOfBirth, diseases);

            UpdateAppointment();
        }

        private void UpdateAppointment()
        {
            if (_appointmentId == -1) return;

            string username = usernameTextBox.Text.Trim();
            Patient newPatient = PatientService.GetPatient(username)!;
            PatientService.EditPatient(username, username, firstNameTextBox.Text, lastNameTextBox.Text, false);

            Appointment newAppointment = AppointmentService.GetAppointment(_appointmentId);
            newAppointment.PatientUsername = usernameTextBox.Text.Trim();
            AppointmentService.AddOrEditAppointment(newAppointment);
        }

        private bool AreFieldsValid(bool isForModifying = false)
        {
            try
            {
                if (IsPatientNameValid() && AreHeightAndWeightValid() && IsDateOfBirthValid())
                {
                    return true;
                }
            }
            catch (Exception exception)
            {
                Notification.ShowErrorDialog(exception.Message);
            }

            return false;

        }

        private bool IsDateOfBirthValid()
        {
            if (dateOfBirthPicker.SelectedDate == null) 
                throw new Exception("Date of birth must be selected!");

            var dateOfBirth = dateOfBirthPicker.SelectedDate.Value.Date;

            if (DateTime.Compare(dateOfBirth, DateTime.Today) <= 0)
            {
                return true;
            }

            throw new Exception("Date of birth cannot be after today's date!");
        }

        private bool IsPatientNameValid()
        {
            if (firstNameTextBox.Text.Length >= 3 && lastNameTextBox.Text.Length >= 3) return true;

            throw new Exception("First and last name must have at least 3 characters!");

        }

        private bool AreHeightAndWeightValid()
        {
            if (double.TryParse(heightTextBox.Text, out _) && double.TryParse(weightTextBox.Text, out _)) return true;

            throw new Exception("Height and weight must be numbers!");

        }
        private List<string> RowsToDiseases()
        {
            List<string> diseases = new List<string>();

            foreach (var row in diseasesDataGrid.Items)
            {
                string diseaseName = row as string ?? string.Empty;
                diseases.Add(diseaseName);
            }

            return diseases;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (AreFieldsValid()) return;
            e.Cancel = true;
        }
    }
}
