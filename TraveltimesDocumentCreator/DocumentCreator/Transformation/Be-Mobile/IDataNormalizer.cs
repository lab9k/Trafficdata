using System;
using System.Collections.Generic;
using System.Text;

namespace TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile
{
    public interface IDataNormalizer
    {
        void StartNormalization(string inputEndpoint, string inputKey, string inputDb, string inputCollStatic, string inputCollDynamic, string[] segments = null,
            string outputColl = null, string outputDb = null, string outputEndpoint = null, string outputKey = null);
    }
}
