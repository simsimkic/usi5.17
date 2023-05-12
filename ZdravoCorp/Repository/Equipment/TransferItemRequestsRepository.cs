using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Equipment;
using ZdravoCorp.Repository.Serializer;

namespace ZdravoCorp.Repository.Equipment
{
    public class TransferItemRequestsRepository
    {
        public const string TransferRequestsFilePath = "..\\..\\..\\Data\\Equipment\\transferItemRequests.csv";
        public static List<TransferItemRequest> TransferRequests = new();
        public static Serializer<TransferItemRequest> TransferRequestsSerializer = new();
        private static readonly object SaveLock = new object();

        public TransferItemRequestsRepository()
        {
            TransferRequests = TransferRequestsSerializer.fromCSV(TransferRequestsFilePath);
        }

        public static void Save()
        {
            lock (SaveLock)
            {
                TransferRequestsSerializer.toCSV(TransferRequestsFilePath, TransferRequests);
            }
        }
    }
}
