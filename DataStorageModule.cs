using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5laba
{
    public class DataStorageModule
    {
        public Dictionary<Bitmap, DataRecord> _storage = new Dictionary<Bitmap, DataRecord>();

        public void AddRecord(string perceptualHash, string semanticInformation, Bitmap img)
        {
            var record = new DataRecord
            {
                Img = img,
                Hash = perceptualHash,
                SemanticInformation = semanticInformation,
            };

            _storage.Add(img, record);
        }

        public DataRecord[] GetRecord(string perceptualHash)
        {
            return _storage.Where(x => x.Value.Hash == perceptualHash).Select(x =>x.Value).ToArray();
        }

    
        public Dictionary<Bitmap, DataRecord> GetRecords()
        { 
            return _storage;
        }

        //public bool UpdateRecord(string perceptualHash, string newSemanticInformation)
        //{
        //    if (_storage.TryGetValue(perceptualHash, out var record))
        //    {
        //        record.SemanticInformation = newSemanticInformation;
        //        return true;
        //    }

        //    return false;
        //}

        //public bool DeleteRecord(string perceptualHash)
        //{
        //    return _storage.Remove(perceptualHash);
        //}

        public class DataRecord
        {
           
            public string SemanticInformation { get; set; }
            public string Hash { get; set; }
            public Bitmap Img { get; set; }
        }



    }

}
