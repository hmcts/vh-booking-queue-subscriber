#bash 
rm -rf Artifacts

exclude=\"[*]BookingQueueSubscriber.Common.*,[BookingQueueSubscriber.Services]BookingQueueSubscriber.Services.VideoApiService,[*]BookingQueueSubscriber.UnitTests.*,[BookingQueueSubscriber]BookingQueueSubscriber.Startup\"
dotnet test --no-build BookingQueueSubscriber/BookingQueueSubscriber.UnitTests/BookingQueueSubscriber.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="${exclude}"

~/.dotnet/tools/reportgenerator -reports:Artifacts/Coverage/coverage.opencover.xml -targetDir:./Artifacts/Coverage/Report -reporttypes:HtmlInline_AzurePipelines

open ./Artifacts/Coverage/Report/index.htm