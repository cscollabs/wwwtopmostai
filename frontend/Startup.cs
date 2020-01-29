using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bamtools.frontend.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using bamtools.domain.Services;
using bamtools.domain.Managers;
using bamtools.domain.Models;
using Microsoft.Extensions.FileProviders;

namespace frontend
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            /*** DO THIS EXACTLY FOR IDENTITY REDIRECT TO WORK(1) **/
            services.AddDbContext<BamtoolsDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("dbConnection")));
            services.AddIdentity<BamtoolsUser, IdentityRole>()
                .AddEntityFrameworkStores<BamtoolsDbContext>()
                .AddDefaultTokenProviders();



            // Add application services.
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddScoped<CategoryManager>();
            services.AddScoped<DepartmentManager>();
            services.AddScoped<DocumentManager>();
            services.AddScoped<GalleryManager>();
            services.AddScoped<EditorialManager>();
            services.AddScoped<EditorialCategoryManager>();
            services.AddScoped<LocationService>();
            services.AddScoped<MemberAccountManager>();
            services.AddScoped<MemberManager>();
            services.AddScoped<MemberCodeManager>();
            services.AddScoped<MultiMediaService>();
            services.AddScoped<UploadService>();
            services.AddScoped<ProductManager>();
            services.AddScoped<ProjectManager>();
            services.AddScoped<PermissionManager>();
            services.AddScoped<RecaptchaService>();
            services.AddScoped<StaticPageManager>();
            services.AddScoped<SiteSettingManager>();
            services.AddSingleton<CacheKeys>();

            /*** DO THIS EXACTLY FOR IDENTITY REDIRECT TO WORK(2) **/
            //Enable Authorization Redirect
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.LoginPath = "/signin";

                options.ReturnUrlParameter = "rurl";
            });


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                //   options.CheckConsentNeeded = context => false; // Default is true, make it false
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //Add Lower Case Routes
            services.AddRouting(options => options.LowercaseUrls = true);

            //Add caching
            services.AddMemoryCache();

            services.AddDistributedMemoryCache();

            services
                .AddMvc()
                 .AddRazorPagesOptions(options =>
                 {
                     options.Conventions.AuthorizePage("/account/logout");
                     options.Conventions.AuthorizeFolder("/checkout");
                     options.Conventions.AuthorizeFolder("/member");
                 })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }


            if (env.IsDevelopment())
            {
                if (!string.IsNullOrEmpty(Configuration["VirtualPath_css"]))
                {
                    app.UseFileServer(new FileServerOptions
                    {
                        FileProvider = new PhysicalFileProvider(Configuration[$"VirtualPath_css:bamtools_css"]),
                        RequestPath = new PathString("/css"),
                        EnableDirectoryBrowsing = false
                    });
                }
                if (!string.IsNullOrEmpty(Configuration["VirtualPath_js"]))
                {
                    app.UseFileServer(new FileServerOptions
                    {
                        FileProvider = new PhysicalFileProvider(Configuration[$"VirtualPath_js:bamtools_js"]),
                        RequestPath = new PathString("/js"),
                        EnableDirectoryBrowsing = false
                    });
                }
                if (!string.IsNullOrEmpty(Configuration["VirtualPath_lib"]))
                {
                    app.UseFileServer(new FileServerOptions
                    {
                        FileProvider = new PhysicalFileProvider(Configuration[$"VirtualPath_lib:bamtools_lib"]),
                        RequestPath = new PathString("/lib"),
                        EnableDirectoryBrowsing = false
                    });
                }

                if (!string.IsNullOrEmpty(Configuration["VirtualPath_media"]))
                {
                    app.UseFileServer(new FileServerOptions
                    {
                        FileProvider = new PhysicalFileProvider(Configuration[$"VirtualPath_media:bamtools_media"]),
                        RequestPath = new PathString("/media"),
                        EnableDirectoryBrowsing = false
                    });
                }
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
