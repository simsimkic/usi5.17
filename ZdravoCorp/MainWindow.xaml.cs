using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FluentScheduler;
using ZdravoCorp.Presentation;
using ZdravoCorp.Presentation.Nurse;
using ZdravoCorp.Repository;
using ZdravoCorp.Presentation.ManagerWindow;
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Repository.Equipment;
using ZdravoCorp.Repository.Orders;
using ZdravoCorp.Repository.Rooms;
using ZdravoCorp.Repository.Serializer;
using ZdravoCorp.Services;
using ZdravoCorp.Services.Orders;
using PatientWindow = ZdravoCorp.Presentation.PatientsWindow.PatientWindow;
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Domain.Calendar;
using ZdravoCorp.Domain.Equipment;
using ZdravoCorp.Services.Equipment;
using ZdravoCorp.Services.Equipment.InventoryTimer;

namespace ZdravoCorp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            User user = UserService.GetLoginUser(this.usernameTextbox.Text, this.passwordTextbox.Password);
            if (user == null)
            {
                errorLabel.Visibility = Visibility.Visible;
                return;
            }

            Globals.LoggedUser = user;
            try
            {
                openRoleWindow(user);
            }
            catch (Exception error)
            {
                Notification.ShowErrorDialog(error.Message);
                
                return;
            }
            this.Close();
        }

        private void openRoleWindow(User user)
        {
            switch (user.Role)
            {
                case User.UserRole.Doctor:
                    DoctorWindow dw = new DoctorWindow();
                    dw.Show();
                    break;
                case User.UserRole.Nurse:
                    NurseWindow nw = new NurseWindow();
                    nw.Show();
                    break;
                case User.UserRole.Patient:
                    if (PatientService.IsPatientBlocked(user.Username))
                    {
                        throw new InvalidOperationException("This account has been permanently disabled.");
                    }
                    else
                    {
                        PatientWindow pd = new PatientWindow();
                        pd.Show();
                    }
                    break;
                case User.UserRole.Manager:
                    ManagerWindow wm = new ManagerWindow();
                    wm.Show();
                    break;
            }
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _ = new EquipmentRepository();
            _ = new RoomRepository();
            _ = new InventoryRepository();
            _ = new UserRepository();
            _ = new PatientRepository();
            _ = new MedicalRecordRepository();
            _ = new DoctorRepository();
            _ = new AppointmentRepository();
            _ = new ReportHistoryRepository();
            _ = new TransferItemRequestsRepository();
            _ = new OrdersRepository();
            JobManager.Initialize(new InventoryTimerRegistry());
            _ = new NoticeRepository();
            UpdateAppointmentsStatus();
        }

        private void UpdateAppointmentsStatus()
        {
            foreach (Appointment appointment in AppointmentRepository.Appointments.Values)
            {
                if (appointment.TimeSlot.End < DateTime.Now && appointment.Status != Appointment.AppointmentStatus.Canceled)
                {
                    appointment.Status = Appointment.AppointmentStatus.Finished;
                }
            }
            AppointmentRepository.Save();
        }

        private void loginWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                loginBtn_Click(sender,e);
        }
    }

}
