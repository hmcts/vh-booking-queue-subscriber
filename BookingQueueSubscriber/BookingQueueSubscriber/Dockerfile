FROM gittools/gitversion:5.3.5-linux-alpine.3.10-x64-netcoreapp3.1 AS gitversion
WORKDIR /version 
COPY .git .git
RUN /tools/dotnet-gitversion /ensureassemblyinfo /updateassemblyinfo AssemblyInfo.cs /nofetch /config /GitVersion.yml

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
RUN mkdir -p /home/site/wwwroot
COPY BookingQueueSubscriber .
RUN dotnet restore --configfile nuget.config BookingQueueSubscriber
COPY --from=gitversion /version/AssemblyInfo.cs .
RUN dotnet build BookingQueueSubscriber -c Release

FROM build AS publish
RUN dotnet publish BookingQueueSubscriber -c Release -o /home/site/wwwroot

FROM mcr.microsoft.com/azure-functions/dotnet:3.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=publish ["/home/site/wwwroot", "/home/site/wwwroot"]
