@echo off

MSBuild /target:Build ../WcsDB.dbproj
if ERRORLEVEL 1 goto :showerror 

MSBuild /target:Deploy /property:TargetDatabase=WCS;TargetConnectionString="Data Source=.;Integrated Security=True;Pooling=False" ../WcsDB.dbproj
if ERRORLEVEL 1 goto :showerror 

"C:\Program Files (x86)\Microsoft SQL Server\100\DTS\Binn\dtexec.exe" /FILE "..\..\WcsSsis\InsertMasterData.dtsx" /CONNECTION MasterDataSpreadsheet;"\"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\..\WcsSsis\MasterData_GalwayClinic.xls;Extended Properties=\"\"EXCEL 8.0;HDR=YES\"\"\"" /CONNECTION WCS;"\"Data Source=.;Initial Catalog=WCS;Provider=SQLNCLI10.1;Integrated Security=SSPI;Auto Translate=False;\""  /CHECKPOINTING OFF  /REPORTING E
if ERRORLEVEL 1 goto :showerror 

"C:\Program Files (x86)\Microsoft SQL Server\100\DTS\Binn\dtexec.exe" /FILE "..\..\WcsSsis\InsertDeviceConfiguration.dtsx" /CONNECTION DeviceConfigurationSpreadsheet;"\"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\..\WcsSsis\DeviceConfiguration_GalwayClinic.xls;Extended Properties=\"\"EXCEL 8.0;HDR=YES\"\"\"" /CONNECTION WCS;"\"Data Source=.;Initial Catalog=WCS;Provider=SQLNCLI10.1;Integrated Security=SSPI;Auto Translate=False;\""  /CHECKPOINTING OFF  /REPORTING E
if ERRORLEVEL 1 goto :showerror 

echo Build was successful

goto :EOF 
 
:showerror 

echo Build error occurred
