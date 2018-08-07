using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrafficDataImportGUI.BeMobile;
using TrafficDataImportGUI.BeMobile.HostedServices;
using TrafficDataImportGUI.BeMobile.Import;
using TraveltimesDocumentCreator;
using TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile;
using TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile.LocalModels;
using TraveltimesDocumentCreator.HostedServices;

namespace TrafficDataImportGUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IBlockingQueue<BeMobileTaskModel, BEMobileSegmentTaskModel>, BlobCopyQueue>();
            services.AddSingleton<TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile.IBlockingQueue<GenericQueueTask<TrajectTaskModel>>, TrajectQueue>();
            services.AddSingleton<TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile.IBlockingQueue<GenericQueueTask<SegmentTaskModel>>, SegmentQueue>();
            services.AddSingleton<IDataNormalizer, BeMobileDataNormalization>();
            services.AddHostedService<BeMobileSegmentService>();
            services.AddHostedService<BeMobileTrajectService>();
            services.AddHostedService<SegmentUploadService>();
            services.AddHostedService<BlobCopyService>();
            services.AddSingleton<IThreadPool, ThreadPool>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
