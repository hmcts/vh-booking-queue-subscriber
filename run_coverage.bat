rmdir /q /s Artifacts

SET exclude=\"[*]BookingQueueSubscriber.Common.*,[BookingQueueSubscriber.Services]BookingQueueSubscriber.Services.VideoApiService,[*]BookingQueueSubscriber.UnitTests.*,[BookingQueueSubscriber]BookingQueueSubscriber.Program"
dotnet test --no-build BookingQueueSubscriber/BookingQueueSubscriber.UnitTests/BookingQueueSubscriber.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="${exclude}"

reportgenerator -reports:Artifacts/Coverage/coverage.opencover.xml -targetDir:Artifacts/Coverage/Report -reporttypes:HtmlInline_AzurePipelines

"Artifacts/Coverage/Report/index.htm"