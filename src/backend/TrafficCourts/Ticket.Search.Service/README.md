
# Configuration

| Config Path | Environment Variable | Description |
|:---| --- |--- |
| TicketSearch:SearchType | | RoadSafety or Mock. Defaults to RoadSafety |
| TicketSearch:AuthenticationUrl | OAUTH_OAUTHURL | |
| TicketSearch:ResourceUrl | OAUTH_RESOURCEURL | |
| TicketSearch:ClientId | OAUTH_CLIENTID | |
| TicketSearch:ClientSecret | OAUTH_SECRET | |


## User Secrets 
%APPDATA%\Microsoft\UserSecrets\435ec3aa-a9e2-4529-945c-14755637cb75

## Local Logging Using Seq

dotnet user-secrets set Serilog:WriteTo:1:Name Seq
dotnet user-secrets set Serilog:WriteTo:1:Args:serverUrl http://localhost:5341
