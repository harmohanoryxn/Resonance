@echo off

MSBuild /target:Build ../../../GalwayClinic.sln /p:Platform="Any CPU"
if ERRORLEVEL 1 goto :showerror 

MSBuild /target:Deploy /property:TargetDatabase=WCS;TargetConnectionString="Data Source=.;Integrated Security=True;Pooling=False" ../WcsDB.dbproj
if ERRORLEVEL 1 goto :showerror 

"C:\Program Files (x86)\Microsoft SQL Server\100\DTS\Binn\dtexec.exe" /FILE "..\..\WcsSsis\InsertMasterData.dtsx" /CONNECTION MasterDataSpreadsheet;"\"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\..\WcsSsis\MasterData_Test.xls;Extended Properties=\"\"EXCEL 8.0;HDR=YES\"\"\"" /CONNECTION WCS;"\"Data Source=.;Initial Catalog=WCS;Provider=SQLNCLI10.1;Integrated Security=SSPI;Auto Translate=False;\""  /CHECKPOINTING OFF  /REPORTING E /X86
if ERRORLEVEL 1 goto :showerror 

"C:\Program Files (x86)\Microsoft SQL Server\100\DTS\Binn\dtexec.exe" /FILE "..\..\WcsSsis\InsertDeviceConfiguration.dtsx" /CONNECTION DeviceConfigurationSpreadsheet;"\"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\..\WcsSsis\DeviceConfiguration_Test.xls;Extended Properties=\"\"EXCEL 8.0;HDR=YES\"\"\"" /CONNECTION WCS;"\"Data Source=.;Initial Catalog=WCS;Provider=SQLNCLI10.1;Integrated Security=SSPI;Auto Translate=False;\""  /CHECKPOINTING OFF  /REPORTING E  /X86
if ERRORLEVEL 1 goto :showerror 

"C:\Program Files (x86)\Microsoft SQL Server\100\DTS\Binn\dtexec.exe" /FILE "..\..\WcsSsis\InsertStagingData.dtsx" /CONNECTION StagingDataSpreadsheet;"\"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\..\WcsSsis\StagingData_Test.xls;Extended Properties=\"\"EXCEL 8.0;HDR=YES\"\"\"" /CONNECTION WCS;"\"Data Source=.;Initial Catalog=WCS;Provider=SQLNCLI10.1;Integrated Security=SSPI;Auto Translate=False;\""  /CHECKPOINTING OFF  /REPORTING E  /X86
if ERRORLEVEL 1 goto :showerror 

"C:\Program Files (x86)\Microsoft SQL Server\100\DTS\Binn\dtexec.exe" /FILE "..\..\WcsSsis\MergeStagingData.dtsx" /CONNECTION WCS;"\"Data Source=.;Initial Catalog=WCS;Provider=SQLNCLI10.1;Integrated Security=SSPI;Auto Translate=False;\""  /CHECKPOINTING OFF  /REPORTING EW 
if ERRORLEVEL 1 goto :showerror 

mstest /testcontainer:"../../../Server/WCS.Services.Test/bin/Debug/WCS.Services.Test.dll"
if ERRORLEVEL 1 goto :showerror 

echo Build was successful

goto :EOF 
 
:showerror 

echo Build error occurred
