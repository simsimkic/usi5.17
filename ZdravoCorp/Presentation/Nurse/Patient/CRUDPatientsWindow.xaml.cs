using System;
using System.Collections.Generic;
using System.Globalization;
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
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Repository;
using ZdravoCorp.Services;

namespace ZdravoCorp.Presentation.Nurse
{
    /// <summary>
    /// Interaction logic for CRUDPatientsWindow.xaml
    /// </summary>
    public partial class CRUDPatientsWindow : Window
    {
        public CRUDPatientsWindow()
        {
            InitializeComponent();
            loggedUsernameLabel.Content = Globals.LoggedUser.Username;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NurseWindow nurseWindow = new();
            nurseWindow.Show();
            this.Close();
        }

        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window == mainWindow) continue;

                window.Close();
            }
            Globals.LoggedUser = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            patientsDataGrid.ItemsSource = PatientService.GetAllPatients();

            var column = new DataGridTextColumn
            {
                Header = "Disease Name",
                Binding = new Binding()
            };

            diseaseDataGrid.Columns.Add(column);
        }

        private void PatientsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (patientsDataGrid.SelectedItem == null) return;

            Patient selectedPatient = (Patient)patientsDataGrid.SelectedItem;

            string username = selectedPatient.Username;
            FillFieldForAppropriatePatient(username);

        }

        private void FillFieldForAppropriatePatient(string username)
        {
            FillFieldsWithUserInfo(username);
            FillFieldsWithPatientInfo(username);
            FillFieldsWithMedicalRecordInfo(username);
        }

        private void FillFieldsWithUserInfo(string username)
        {
            var selectedUser = UserService.GetUser(username);

            if (selectedUser == null) return;

            usernameTextBox.Text = selectedUser.Username;
            passwordBox.Password = selectedUser.Password;
        }
        private void FillFieldsWithPatientInfo(string username)
        {
            var selectedPatient = PatientService.GetPatient(username);

            if (selectedPatient == null) return;
            

            firstNameTextBox.Text = selectedPatient.FirstName;
            lastNameTextBox.Text = selectedPatient.LastName;

            isBlockedCheckBox.IsChecked = selectedPatient.Status == Patient.BlockStatus.Blocked;

        }
        private void FillFieldsWithMedicalRecordInfo(string username)
        {
            var medicalRecord = MedicalRecordService.GetMedicalRecord(username);

            if (medicalRecord == null) return;
            

            weightTextBox.Text = medicalRecord.Weight.ToString(CultureInfo.InvariantCulture);
            heightTextBox.Text = medicalRecord.Height.ToString(CultureInfo.InvariantCulture);

            var dateOnlyToDateTime = medicalRecord.DateOfBirth.ToDateTime(new TimeOnly());
            dateOfBirthPicker.SelectedDate = DateTime.SpecifyKind(dateOnlyToDateTime, DateTimeKind.Unspecified);

            FillDiseaseDataGrid(medicalRecord);
        }

        private void FillDiseaseDataGrid(MedicalRecord medicalRecord)
        {
            diseaseDataGrid.Items.Clear();

            if (medicalRecord.DiseaseHistory[0].Trim().Equals("")) return;
            
            foreach (var disease in medicalRecord.DiseaseHistory)
            {
                diseaseDataGrid.Items.Add(disease);
            }
        }

        private void AddDisease_Click(object sender, RoutedEventArgs e)
        {
            if (diseaseTextBox.Text.Trim() == "") return;
            

            diseaseDataGrid.Items.Add(diseaseTextBox.Text.Trim());
        }

        private void RemoveDiseaseBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = diseaseDataGrid.SelectedItem;

            if (selectedRow != null)
            {
                diseaseDataGrid.Items.Remove(selectedRow);
            }
        }

        private void CreatePatientBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!AreFieldsValid()) return;
            
            CreateUser();
            CreateMedicalRecord();
            CreatePatient();
            CreatePatientReportHistory();

            Notification.ShowSuccessDialog("Patient created successfully!");
        }

        private void CreatePatientReportHistory() => ReportHistoryService.InitializeReportHistory(usernameTextBox.Text);

        private void CreatePatient()
        {
            Patient newPatient = new Patient(firstNameTextBox.Text, lastNameTextBox.Text, usernameTextBox.Text,  isBlockedCheckBox.IsChecked ?? false);
            PatientRepository.Patients.Add(newPatient);

            patientsDataGrid.Items.Refresh();

            PatientService.SaveRepository();
        }

        private void CreateUser()
        {
            UserRepository.Users.Add(new User(usernameTextBox.Text, passwordBox.Password, User.UserRole.Patient));
            UserService.SaveRepository();
        }

        private void CreateMedicalRecord()
        {
            DateOnly dateOnly = DateOnly.FromDateTime(dateOfBirthPicker.SelectedDate!.Value.Date);

            double weight = Convert.ToDouble(weightTextBox.Text);
            double height = Convert.ToDouble(heightTextBox.Text);

            string username = usernameTextBox.Text;

            var diseases = RowsToDiseases();

            MedicalRecord newMedicalRecord = new MedicalRecord(weight, height, username, dateOnly, diseases);

            MedicalRecordRepository.MedicalRecords.Add(newMedicalRecord);
            MedicalRecordService.SaveRepository();
        }

        private List<string> RowsToDiseases()
        {
            return (from object? row in diseaseDataGrid.Items select row as string ?? string.Empty).ToList();
        }

        private bool AreFieldsValid(bool isForModifying = false)
        {
            if (IsPatientNameValid() && IsUsernameValid(isForModifying) && IsPasswordValid() && AreHeightAndWeightValid() && IsDateOfBirthValid())
            {
                validationLabel.Foreground = new SolidColorBrush(Colors.White); 
                return true;
            }

            validationLabel.Foreground = new SolidColorBrush(Colors.Red);
            return false;

        }

        private bool IsDateOfBirthValid()
        { 
            if (dateOfBirthPicker.SelectedDate == null) return false;

            var dateOfBirth = dateOfBirthPicker.SelectedDate.Value.Date;

            return DateTime.Compare(dateOfBirth, DateTime.Today) <= 0;
        }

        private bool IsPatientNameValid()
        {
            if (firstNameTextBox.Text.Length >= 3 && lastNameTextBox.Text.Length >= 3) return true;

            validationLabel.Content = "First and last name have to have at least 3 characters!";
            return false;

        }

        private bool IsUsernameValid(bool isForModifying = false)
        {
            string username = usernameTextBox.Text;

            if (!isForModifying)
            {
                if (!UserService.DoesUsernameAlreadyExist(username) && username.Length >= 3) return true;
            }
            else
            {
                string oldUsername = ((Patient)patientsDataGrid.SelectedItem).Username;
                if (UserService.IsUsernameValidForModification(oldUsername, username) && username.Length >= 3) return true;
            }

            validationLabel.Content = "That username is not available!";
            return false;

        }

        private bool IsPasswordValid()
        {
            if (passwordBox.Password.Length >= 3) return true;

            validationLabel.Content = "Password must be at least 3 characters long!";
            return false;

        }

        private bool AreHeightAndWeightValid()
        {
            if (double.TryParse(heightTextBox.Text, out _) && double.TryParse(weightTextBox.Text, out _)) return true;

            validationLabel.Content = "Height and weight have to be numbers!";
            return false;
        }

        private void ModifyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (patientsDataGrid.SelectedItem == null)
            {
                Notification.ShowErrorDialog("Please select patient from the table first!");
                return;
            }

            if (!AreFieldsValid(true)) return;

            string oldUsername = ((Patient)patientsDataGrid.SelectedItem).Username;

            ModifyUser(oldUsername);
            ModifyMedicalRecord(oldUsername);
            ModifyPatient(oldUsername);

            patientsDataGrid.Items.Refresh();

            Notification.ShowSuccessDialog("Patient modified successfully!");
        }

        

        private void ModifyPatient(string oldUsername)
        {
            string newUsername = usernameTextBox.Text;
            string newFirstName = firstNameTextBox.Text;
            string newLastName = lastNameTextBox.Text;
            bool isBlocked = isBlockedCheckBox.IsChecked ?? false;

            PatientService.EditPatient(oldUsername, newUsername, newFirstName, newLastName, isBlocked);
        }

        private void ModifyMedicalRecord(string oldUsername)
        {
            DateOnly dateOfBirth = DateOnly.FromDateTime(dateOfBirthPicker.SelectedDate!.Value.Date);

            double weight = Convert.ToDouble(weightTextBox.Text);
            double height = Convert.ToDouble(heightTextBox.Text);

            string username = usernameTextBox.Text;

            var diseases = RowsToDiseases();

            MedicalRecordService.EditMedicalRecord(oldUsername, username, height, weight, dateOfBirth, diseases);
        }

        private void ModifyUser(string oldUsername)
        {
            UserService.EditUser(oldUsername, usernameTextBox.Text, passwordBox.Password, User.UserRole.Patient);
            patientsDataGrid.Items.Refresh();

        }

        private void DeletePatientBtn_Click(object sender, RoutedEventArgs e)
        {
            if (patientsDataGrid.SelectedItem == null)
            {
                Notification.ShowErrorDialog("Please select patient from the table first!");
                return;
            }

            string selectedUsername = (((Patient)patientsDataGrid.SelectedItem!)).Username;

            UserService.DeleteUser(selectedUsername);
            PatientService.DeletePatient(selectedUsername);
            MedicalRecordService.DeleteMedicalRecord(selectedUsername);

            patientsDataGrid.Items.Refresh();

            Notification.ShowSuccessDialog("Patient deleted successfully!");
        }

    }
}
