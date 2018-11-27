using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IConfiguration _configs;

        public ValuesController(IConfiguration configs)
        {
            _configs = configs;
        }
        [HttpGet("{customerId}")]
        public async Task<string> GetCustomerData(string customerId)
        {
            string responseText = "";

            try
            {
                Binding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

                var relayUrl = _configs["ServiceBusRelayUrl"];
                var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(_configs["RelayKeyName"], _configs["RelayKey"]);
                SecurityToken token = await tokenProvider.GetTokenAsync(relayUrl, new TimeSpan(1, 0, 0));

                //Setup HttpClient to communicate with WCF Service 
                using (var httpClient = new HttpClient())
                {
                    //This header is required for Service Bus Relay
                    httpClient.DefaultRequestHeaders.Add("ServiceBusAuthorization", token.TokenValue);

                    //Here we setup the SOAP Action - schema: http://tempuri.org/<Interface>/<OperationName>
                    httpClient.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/ICustomerService/GetCustomerData");
                    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));

                    //Setup soap message body
                    string soapMessageBody = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/""><soapenv:Header /><soapenv:Body><tem:GetCustomerData><tem:customerId>" + customerId + "</tem:customerId></tem:GetCustomerData></soapenv:Body></soapenv:Envelope>";

                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(relayUrl),
                        Method = HttpMethod.Post,
                        Content = new StringContent(soapMessageBody, Encoding.UTF8)
                    };

                    var response = await httpClient.SendAsync(request);
                    responseText = await response.Content.ReadAsStringAsync();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception" + ex.InnerException);
            }
            //you can parse SOAP message to your dto here
            return responseText;

        }
    }
}
