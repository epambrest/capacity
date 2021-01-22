using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Teams.Business.Services;
using Teams.Security;
using Teams.Data;
using Teams.Data.Models;
using Teams.Data.Repository;
using Teams.Web.Resources.ViewModels;

namespace Teams.Web
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddTransient<IManageTeamsMembersService, ManageTeamsMembersService>();
            services.AddTransient<IManageTeamsService, ManageTeamsService>();
            services.AddTransient<IManageSprintsService, ManageSprintsService>();
            services.AddTransient<IManageTasksService, ManageTasksService>();
            services.AddTransient<IManageMemberWorkingDaysService, ManageMemberWorkingDaysService>();
            services.AddTransient<IRepository<Team, int>, TeamRepository>();
            services.AddTransient<IRepository<TeamMember, int>, TeamMemberRepository>();
            services.AddTransient<IRepository<Sprint, int>, SprintRepository>();
            services.AddTransient<IRepository<Task, int>, TaskRepository>();
            services.AddTransient<IRepository<MemberWorkingDays, int>, MemberWorkingDaysRepository>();
            services.AddHttpContextAccessor();
            services.AddTransient<ICurrentUser, CurrentUser>();
            services.AddTransient<IManageTeamsMembersService, ManageTeamsMembersService>();
            services.AddTransient<IAccessCheckService, AccessCheckService>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                    options.DataAnnotationLocalizerProvider = (type, factory) => 
                        factory.Create(typeof(ValidationResource)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("ru")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
