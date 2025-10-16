using Google.Apis.AndroidPublisher.v3;
using Google.Apis.Auth.OAuth2;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Connector
{
    public class GoogleCredentialConnector
    {
        private static GoogleCredential? instance;
        private static readonly object _padlock = new object();

        private GoogleCredentialConnector() { }

        public static GoogleCredential Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_padlock)
                    {
                        if (instance == null)
                        {
                            var jsonKeyFilePath = ConstInfo.GoogleCredentialKeyJson;
                            instance = GoogleCredential.FromFile(jsonKeyFilePath)
                                .CreateScoped(new[] { AndroidPublisherService.Scope.Androidpublisher });
                        }                        
                    }
                }
                return instance;
            }
        }
    }
}
