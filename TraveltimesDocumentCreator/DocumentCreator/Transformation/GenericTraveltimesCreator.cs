using Database_Access_Object;
using DocumentModels.Generic;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
/// <summary>
/// Creates generic models for traveltimes independent from source type T
/// 
/// </summary>
namespace TraveltimesDocumentCreator
{
    public abstract class GenericTraveltimesCreator<T> where T: IDocumentPOCO {

        protected static QueryManager<GenericTraveltimeSegment> PersistDocuments;

        protected GenericTraveltimesCreator(string outputCollection)
        {
            PersistDocuments = new QueryManager<GenericTraveltimeSegment>
            {
                CollectionName = outputCollection,
                DatabaseKey = ConfigManager.GetProperty("database:key") as string,
                DatabaseName = ConfigManager.GetProperty("database:name") as string,
                DatabaseUri = ConfigManager.GetProperty("database:endpoint") as string
            };
            PersistDocuments.Init();
        }

        protected GenericTraveltimesCreator()
        {
                
        }


        protected static  void PersistResultDocuments(List<GenericTraveltimeSegment> docs)
        {
            foreach(GenericTraveltimeSegment doc in docs)
            {
                PersistResultDocuments(doc);
            }
        }

        protected static void PersistResultDocuments(GenericTraveltimeSegment doc, int tryCount = 0)
        {
            try
            {
                PersistDocuments.Create(doc).Wait();
            }catch(DocumentClientException ex)
            {
                Console.WriteLine("Error occured:" + ex.Error);
                Console.WriteLine($"Retrycount: {tryCount}, {(tryCount <= 10 ? String.Format("retrying in 1 second") : String.Format("skipping document"))}");
                if( tryCount < 10)
                {
                    System.Threading.Thread.Sleep(1000);
                    PersistResultDocuments(doc, tryCount++);
                }
            }


        }

        protected static void ReplaceResultDocuments(GenericTraveltimeSegment doc, int tryCount = 0)
        {
            try
            {
                PersistDocuments.Update(doc.Id,doc).Wait();
            }
            catch (DocumentClientException ex)
            {
                Console.WriteLine("Error occured:" + ex.Error);
                Console.WriteLine($"Retrycount: {tryCount}, {(tryCount <= 10 ? String.Format("retrying in 1 second") : String.Format("skipping document"))}");
                if (tryCount < 10)
                {
                    System.Threading.Thread.Sleep(1000);
                    PersistResultDocuments(doc, tryCount++);
                }
            }


        }
    }
}
