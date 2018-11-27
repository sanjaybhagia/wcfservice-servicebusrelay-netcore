# wcfservice-servicebusrelay-netcore

This is a sample application for accessing WCF Service (host in IIS on on-premises envrionment) from a ASP.NET Core Web application via Azure Service Bus Relay


- **WcfService** project is simple WCF Service with basicHttpRelayBinding 
- **WebApplication** is a standing .NET Core (2.1) web api application calling WCF Service

You can find more detail in my blogpost here: https://www.sanjaybhagia.com/2018/11/27/accessing-wcf-service-via-azure-service-bus-relay-with-net-core/
