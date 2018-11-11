using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Channels;

// link: https://stackoverflow.com/questions/33166679/get-client-ip-address-using-wcf-4-5-remoteendpointmessageproperty-in-load-balanc

namespace ServiceLibrary.Tools
{
    internal class ClientAddress
    {
        public static string DeducedClientAddress()
        {
            string address = String.Empty;

            try
            {
                OperationContext context = OperationContext.Current;
                MessageProperties properties = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

                if (properties.Keys.Contains(HttpRequestMessageProperty.Name))
                {
                    HttpRequestMessageProperty endpointLoadBalancer = properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
                    if (endpointLoadBalancer != null && endpointLoadBalancer.Headers["X-Forwarded-For"] != null)
                    {
                        address = endpointLoadBalancer.Headers["X-Forwarded-For"];
                    }
                }
                if (String.IsNullOrEmpty(address))
                {
                    address = endpoint.Address;
                }
            }
            catch (Exception)
            {
                address = String.Empty;
            }

            return address;
        }
    }
}
