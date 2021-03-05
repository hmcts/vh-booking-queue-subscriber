ARG SDK_VERSION=latest
ARG RUNTIME_VERSION=latest

FROM gittools/gitversion:5.3.5-linux-alpine.3.10-x64-netcoreapp3.1 AS gitversion
WORKDIR /version 
COPY .git .git
RUN /tools/dotnet-gitversion /ensureassemblyinfo /updateassemblyinfo AssemblyInfo.cs /nofetch /config /GitVersion.yml

FROM mcr.microsoft.com/dotnet/core/sdk:${SDK_VERSION} AS build
RUN mkdir -p /home/site/wwwroot
COPY BookingQueueSubscriber .
RUN dotnet restore --configfile nuget.config BookingQueueSubscriber
COPY --from=gitversion /version/AssemblyInfo.cs .
RUN dotnet build BookingQueueSubscriber -c Release

FROM build AS publish
RUN dotnet publish BookingQueueSubscriber -c Release -o /home/site/wwwroot

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet:3.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet:${RUNTIME_VERSION}
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=publish ["/home/site/wwwroot", "/home/site/wwwroot"]
