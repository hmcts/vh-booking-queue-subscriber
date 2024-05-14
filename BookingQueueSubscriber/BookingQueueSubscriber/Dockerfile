FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN mkdir -p /home/site/wwwroot
COPY BookingQueueSubscriber .
RUN dotnet restore --configfile nuget.config BookingQueueSubscriber
RUN dotnet build BookingQueueSubscriber -c Release

FROM build AS publish
RUN dotnet publish BookingQueueSubscriber -c Release -o /home/site/wwwroot

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=publish ["/home/site/wwwroot", "/home/site/wwwroot"]
