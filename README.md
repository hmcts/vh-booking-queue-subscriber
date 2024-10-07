# vh-booking-queue-subscriber

Subscriber for booking queue

## Download the local.settings.json

Install the Azure Functions Core Tools 
```npm install -g azure-functions-core-tools```

Then install the Azure PowerShell module
```Install-Module -Name Az -AllowClobber -Scope CurrentUser```

Login to Azure using your HMCTS account
```Connect-AzAccount```

Finally, run the following command into a terminal in the same directory as the BookingQueueSubscriber.csproj file

```bash
cd BookingQueueSubscriber/BookingQueueSubscriber
func azure functionapp fetch-app-settings vh-booking-queue-subscriber-dev
```

## Running the app as a container

Visit the VH-Setup repository for
[Instructions to run as a container locally.](https://github.com/hmcts/vh-setup/tree/main/docs/local-container-setup).
