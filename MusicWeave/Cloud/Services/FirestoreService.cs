using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using System.IO;

namespace MusicWeave.Cloud.Services
{
    public class FirestoreService
    {
        private FirestoreDb _db;

        public FirestoreService(IConfiguration configuration)       
        {
            var firebaseCredentialPath = configuration["FireBaseCredentialPath"];

            GoogleCredential credential;

            using (var jsonStream = new FileStream(firebaseCredentialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(jsonStream);
            }

            _db = FirestoreDb.Create("music-weave", new FirestoreClientBuilder
            {
                CredentialsPath = firebaseCredentialPath
            }.Build());
        }
    }
}
