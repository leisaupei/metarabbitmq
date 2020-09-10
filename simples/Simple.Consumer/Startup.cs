using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meta.RabbitMQ.Consumer;
using Meta.RabbitMQ.Extension;
using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Simple.Consumer
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions();
			services.AddLogging();
			services.Configure<RabbitMQOptionCollection>(a =>
			{
				a.Add(new RabbitMQOption
				{
					HostName = "localhost",
					Port = 5672,
					UserName = "guest",
					Password = "guest",
					VirtualHost = "/",
					MaxChannelPoolSize = 5000,
					Name = "v1"
				}) ;
			});
			services.AddSingleton<IConsumerSubscriber, SimpleSubscriber>();
			//services.AddSingleton<IConsumerSubscriber, SimpleSubscriber2>();
			services.AddRabbitMQConsumerService(a =>
			{
				a.SubscribeThreadCount = 20;
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConsumerRegister consumerRegister, IHostApplicationLifetime applicationLifetime)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			applicationLifetime.ApplicationStarted.Register(consumerRegister.Start);
			applicationLifetime.ApplicationStopped.Register(consumerRegister.Dispose);
			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{

			});
		}
	}
}
