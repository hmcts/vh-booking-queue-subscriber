FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN mkdir -p /home/site/wwwroot
COPY BookingQueueSubscriber .
RUN dotnet restore --configfile nuget.config BookingQueueSubscriber && dotnet build BookingQueueSubscriber -c Release

FROM build AS publish
RUN dotnet publish BookingQueueSubscriber -c Release -o /home/site/wwwroot

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /home/site/wwwroot .

ENTRYPOINT ["dotnet", "BookingQueueSubscriber.dll"]