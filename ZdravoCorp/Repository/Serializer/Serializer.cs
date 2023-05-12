using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Repository.Serializer
{
    public class Serializer<T> where T : Serializable, new()
    {
        private static char DELIMITER = '|';
        private static readonly object toCsvLock = new object();
        private static readonly object fromCsvLock = new object();

        public void toCSV(string fileName, List<T> objects)
        {
            lock (toCsvLock)
            {
                using StreamWriter streamWriter = new StreamWriter(fileName);

                foreach (Serializable obj in objects)
                {
                    string line = string.Join(DELIMITER.ToString(), obj.ToCSV());
                    streamWriter.WriteLine(line);
                }
            }
        }

        public List<T> fromCSV(string fileName)
        {
            lock (fromCsvLock)
            {
                List<T> objects = new List<T>();

                foreach (string line in File.ReadLines(fileName))
                {
                    string[] csvValues = line.Split(DELIMITER);
                    T obj = new T();
                    obj.FromCSV(csvValues);
                    objects.Add(obj);
                }

                return objects;
            }
        }
    }

}
