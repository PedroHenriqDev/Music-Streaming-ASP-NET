using Google.Cloud.Firestore.V1;
using MusicWeave.Cloud.Classes;
using MusicWeave.Cloud.Services;

namespace MusicWeave.Services
{
    public class MusicService
    {
        private readonly FirestoreService _firestoreService;

        public MusicService(FirestoreService firestoreService)
        {
            _firestoreService = firestoreService;
        }
    }
}
