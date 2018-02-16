using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PRLoginRedes.Data;
using PRLoginRedes.Models;
using PRLoginRedes.Services;
using Microsoft.AspNetCore.Http;

namespace PRLoginRedes
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
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

                // services.AddDbContext<ApplicationDbContext>(options =>
                // options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Chamando o serviço de Autenticação do Facebook
                services.AddAuthentication().AddFacebook(facebookOptions =>{
                    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];//Authentication é a referencia para inserir o App Id e App Secret no .jason
                    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });

            //Chamando o serviço de Autenticação do Google
            services.AddAuthentication().AddGoogle(googleOptions =>{
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });
            
            //Chamando o serviço de Autenticação da Microsoft
            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>{
                microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ApplicationId"];
                microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:Password"];
            });

            //Chamando o serviço de Autenticação do Twitter
            services.AddAuthentication().AddTwitter(twitterOptions =>{
                twitterOptions.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                twitterOptions.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
            });

            //Chamando o serviço de Autenticação do Twitter
             services.AddAuthentication().AddOAuth("Github",githubOptions =>{
                 githubOptions.ClientId = Configuration["Authentication:GitHub:ClientId"];
                 githubOptions.ClientSecret = Configuration["Authentication:GitHub:ClientSecret"];
                 githubOptions.CallbackPath = new PathString("/signin-github");

                githubOptions.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                githubOptions.TokenEndpoint = "https://github.com/login/oauth/access_token";
                githubOptions.UserInformationEndpoint = "https://api.github.com/user";
             });
            

          // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
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
