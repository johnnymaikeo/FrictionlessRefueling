using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FrictionlessRefueling.Biz.DataModel;
using FrictionlessRefueling.Biz.Repository;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Mobile.Server;
using System.Threading.Tasks;

namespace FrictionlessRefueling.Web.Controllers
{
    public class CarEntryController : ApiController
    {
        private CarEntryRepository _repository;

        public CarEntryController()
        {
            _repository = new CarEntryRepository();
        }

        // GET api/carentry
        public async Task<HttpResponseMessage> Get()
        {
            try
            {
                var list = _repository.ListAll();
                await CarEntryNotificationHub("Hello, your friendly car just told us you're running low in fuel.");
                return Request.CreateResponse(HttpStatusCode.OK, list);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        // POST api/carentry
        public HttpResponseMessage Post([FromBody]CarEntry entry)
        {
            try
            {
                _repository.Create(entry);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        public async Task CarEntryNotificationHub(string notification)
        {
            // Get the settings for the server project.
            HttpConfiguration config = this.Configuration;

            MobileAppSettingsDictionary settings = this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            // Get the Notification Hubs credentials for the Mobile App.
            string notificationHubName = settings.NotificationHubName;
            string notificationHubConnection = settings.Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

            // Create a new Notification Hub client.
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            // iOS payload
            var appleNotificationPayload = "{\"aps\":{\"alert\":\"" + notification + "\"}}";

            try
            {
                // Send the push notification and log the results.
                var result = await hub.SendAppleNativeNotificationAsync(appleNotificationPayload);

                // Write the success result to the logs.
                config.Services.GetTraceWriter().Info(result.State.ToString());
            }
            catch (System.Exception ex)
            {
                // Write the failure result to the logs.
                config.Services.GetTraceWriter().Error(ex.Message, null, "Push.SendAsync Error");
            }
            //return CreatedAtRoute("Tables", new { id = current.Id }, current);
            return;
        }
    }
}
