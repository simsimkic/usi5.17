using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain;
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Repository.Serializer;

namespace ZdravoCorp.Repository
{
    public class DoctorRepository
    {
        public const string DoctorsFilePath = "..\\..\\..\\Data\\doctors.csv";
        public static List<Doctor> Doctors = new();
        public static Serializer<Doctor> DoctorSerializer = new();

        public DoctorRepository()
        {
            Doctors = DoctorSerializer.fromCSV(DoctorsFilePath);
        }

        public static List<Doctor> GetAllSpecializedDoctors(DoctorSpecialization specialization)
        {
            return DoctorRepository.Doctors.Where(doctor => doctor.Specialization == specialization).ToList();
        }

        public static void Save()
        {
            DoctorSerializer.toCSV(DoctorsFilePath, Doctors);
        }
    }
}
