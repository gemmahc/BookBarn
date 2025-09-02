using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BookBarn.Model
{
    [DataContract]
    public class MediaStorageToken
    {
        public MediaStorageToken()
        {
            Headers = new Dictionary<string, string>();
        }

        [DataMember]
        public string? Id { get; set; }

        [DataMember]
        public Uri? StorageEndpoint { get; set; }

        [DataMember]
        public Dictionary<string, string>? Headers { get; set; }
    }
}
