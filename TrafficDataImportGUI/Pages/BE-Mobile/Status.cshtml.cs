using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrafficDataImportGUI.BeMobile;
using TrafficDataImportGUI.BeMobile.Import;
using TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile;
using TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile.LocalModels;

namespace TrafficDataImportGUI.Pages.BE_Mobile
{
    public class StatusModel : PageModel
    {
        private IBlockingQueue<BeMobileTaskModel, BEMobileSegmentTaskModel> _importQueue;
        private IBlockingQueue<GenericQueueTask<TrajectTaskModel>> _normalizeTaskQueue;
        private IBlockingQueue<GenericQueueTask<SegmentTaskModel>> _normalizeDocumentQueue;

        public StatusModel(IBlockingQueue<BeMobileTaskModel, BEMobileSegmentTaskModel> importQueue, TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile.IBlockingQueue<GenericQueueTask<TrajectTaskModel>> normalizeTaskQueue, TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile.IBlockingQueue<GenericQueueTask<SegmentTaskModel>> normalizeDocumentQueue)
        {
            _importQueue = importQueue;
            _normalizeTaskQueue = normalizeTaskQueue;
            _normalizeDocumentQueue = normalizeDocumentQueue;
        }

        public void OnGet()
        {
            ViewData["import_job_queued"] = _importQueue.JobQueue.Count;
            ViewData["import_doc_queued"] = _importQueue.DocumentQueue.Count;
            ViewData["transform_job_queued"] = _normalizeTaskQueue.Queue.Count;
            ViewData["transform_doc_queued"] = _normalizeDocumentQueue.Queue.Count;

            ViewData["import_job_finished"] = _importQueue.JobsFinished;
            ViewData["import_doc_finished"] = _importQueue.DocumentsFinished;
            ViewData["transform_job_finished"] = _normalizeTaskQueue.Finished;
            ViewData["transform_doc_finished"] = _normalizeDocumentQueue.Finished;


            ViewData["import_job_failed"] = _importQueue.JobsFailed;
            ViewData["import_doc_failed"] = _importQueue.DocumentsFailed;
            ViewData["transform_job_failed"] = _normalizeTaskQueue.Failed;
            ViewData["transform_doc_failed"] = _normalizeDocumentQueue.Failed;
        }
    }
}