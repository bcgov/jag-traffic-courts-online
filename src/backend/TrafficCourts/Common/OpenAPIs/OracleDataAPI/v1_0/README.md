The client classes are generated using nswag msbuild task. Inside TrafficCourts.Common.csproj, 
there is this section which will generate the OracleDataApiClient.cs file.

To edit the oracle-data-api.nswag,  install the the NSwagStudio tool. https://github.com/RicoSuter/NSwag/wiki/NSwagStudio

```
	<Target Name="NSwag" BeforeTargets="BeforeBuild">
		<Exec Command="$(NSwagExe_Net60) run OpenAPIs\OracleDataAPI\v1_0\oracle-data-api.nswag" />
	</Target>
```
