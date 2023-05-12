using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Repository;

namespace ZdravoCorp.Services
{
    public static class DoctorService
    {
        public static Doctor? GetDoctor(string username)
        {
            return DoctorRepository.Doctors.FirstOrDefault(doctor => doctor.Username == username);
        }

        public static List<Doctor> GetAllDoctors()
        {
            return DoctorRepository.Doctors.ToList();
        }

        public static List<Doctor> GetAllSpecializedDoctors(DoctorSpecialization specialization)
        {
            return DoctorRepository.GetAllSpecializedDoctors(specialization);
        }
    }
}
