using bamtools.backend.Data;
using bamtools.backend.Models;
using bamtools.domain.Managers;
using bamtools.domain.Models;
using bamtools.domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using bamtools.backend.UI;

namespace backend
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

            //INJECT HTTP CONTEXT TO DETERMINE DEFAULT DB
            services.AddDbContext<BamtoolsDbContext>((serviceProvider, options) =>
            {

                options.UseSqlServer(Configuration.GetConnectionString("dbConnection"));
            });

            services.AddIdentity<BamtoolsUser, IdentityRole>()
                .AddEntityFrameworkStores<BamtoolsDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<CategoryManager>();
            services.AddScoped<DepartmentManager>();
            services.AddScoped<DocumentManager>();
            services.AddScoped<EditorialManager>();
            services.AddScoped<EditorialCategoryManager>();
            services.AddScoped<GalleryManager>();
            services.AddScoped<LocationService>();
            services.AddScoped<MemberManager>();
            services.AddScoped<MultiMediaService>();
            services.AddScoped<ProjectManager>();
            services.AddScoped<PermissionManager>();
            services.AddScoped<SiteSettingManager>();
            services.AddScoped<StaticPageManager>();
            services.AddScoped<UploadService>();

            services.AddSingleton<CacheKeys>();

            services.ConfigureOptions(typeof(UIConfigureOptions));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                app.UseFileServer(new FileServerOptions
                {
                    FileProvider = new PhysicalFileProvider(Configuration[$"VirtualPath_media:bamtools_media"]),
                    RequestPath = new PathString("/media"),
                    EnableDirectoryBrowsing = false
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}