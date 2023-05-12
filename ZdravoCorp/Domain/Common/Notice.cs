using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using ZdravoCorp.Repository.Serializer;

namespace ZdravoCorp.Domain.Common
{
    public class Notice : Serializable
    {
        public DateTime TimeOfNotice { get; set; }
        public string Content { get; set; }
        public string NotifiedUserUsername { get; set; }

        public Notice(DateTime timeOfNotice, string content, string notifiedUsername)
        {
            TimeOfNotice = timeOfNotice;
            Content = content;
            NotifiedUserUsername = notifiedUsername;
        }

        public Notice() { }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                TimeOfNotice.ToString(),
                Content,
                NotifiedUserUsername
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            TimeOfNotice = DateTime.Parse(values[0]);
            Content = values[1];
            NotifiedUserUsername = values[2];
            
        }
    }
}
