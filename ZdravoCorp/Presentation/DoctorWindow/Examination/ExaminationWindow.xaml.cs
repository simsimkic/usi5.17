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
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Presentation.Common;
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Services;

namespace ZdravoCorp.Presentation.DoctorWindow.Examination
{
    /// <summary>
    /// Interaction logic for ExaminationWindow.xaml
    /// </summary>
    public partial class ExaminationWindow : Window
    {
        private readonly Appointment _appointment;
        private readonly ZdravoCorp.DoctorWindow _doctorWindow;
        public ExaminationWindow(Appointment appointment, ZdravoCorp.DoctorWindow dw)
        {
            InitializeComponent();
            this._appointment = appointment;
            this._doctorWindow = dw;
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            ReportHistoryService.AddOrEditReport(_appointment.PatientUsername, _appointment.Id, reportTextBox.Text);
            _appointment.Status = Appointment.AppointmentStatus.Finished;
            AppointmentService.AddOrEditAppointment(_appointment);

            _doctorWindow.UpdateAppointmentsTable();

            this.Close();

            DynamicEquipmentForm dynamicEquipmentForm = new(_appointment.RoomName);
            dynamicEquipmentForm.ShowDialog();
        }

        private void EditMedicalRecordButton_Click(object sender, RoutedEventArgs e)
        {
            EditMedicalRecordWindow editMedicalRecordWindow =
                new(MedicalRecordService.GetMedicalRecord(_appointment.PatientUsername));
            editMedicalRecordWindow.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MedicalRecordWindow medicalRecordWindow =
                new(MedicalRecordService.GetMedicalRecord(_appointment.PatientUsername));
            medicalRecordWindow.Show();
        }
    }
}
