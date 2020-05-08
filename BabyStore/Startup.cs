using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BabyStore.Models;
using Microsoft.EntityFrameworkCore;
using BabyStore.Infrastrcture;
using Microsoft.AspNetCore.Identity;
using Stripe;

namespace BabyStore
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
            services.AddSession();

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            services.AddDbContextPool<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(Configuration["IdentityCS:IdentityConnection"]);

            });

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();


            services.AddDbContextPool<StoreContext>(option => {
                option.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });



         
            services.AddTransient<IEntityRepository<Category>, EntityRepository<Category>>();
            services.AddTransient<IEntityRepository<BabyStore.Models.Product>, EntityRepository<BabyStore.Models.Product>>();
            services.AddTransient<IEntityRepository<ProductImage>, EntityRepository<ProductImage>>();
            services.AddTransient<IEntityRepository<ProductImageMapping>, EntityRepository<ProductImageMapping>>();
            services.AddTransient<IEntityRepository<BasketLine>, EntityRepository<BasketLine>>();
            services.AddTransient<IEntityRepository<OrderLine>, EntityRepository<OrderLine>>();
            services.AddTransient<IEntityRepository<BabyStore.Models.Order>, EntityRepository<BabyStore.Models.Order>>();

            services.AddTransient<FileValidation>();
            services.AddTransient<TotalAccount>();

            services.AddScoped<Basket>(sp => Basket.GetBasket(sp));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMemoryCache();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            app.UseSession();
           StripeConfiguration
              .SetApiKey(Configuration.GetSection("Stripe")["SecretKey"]);
            //app.UseHttpsRedirection();
            //app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
