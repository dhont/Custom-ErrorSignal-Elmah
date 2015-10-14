@Echo OFF
SET SolutionOrProjectPath="CustomErrorSignal.sln"
SET NugetCsproj="CustomErrorSignal\CustomErrorSignal.csproj"
SET NugetPath=".nuget"

SET MSBuildPath="%windir%\Microsoft.NET\Framework\v4.0.30319"

Echo Rebuilding Solution
%MSBuildPath%\MSBuild.exe %SolutionOrProjectPath% /p:VisualStudioVersion=12.0 /t:rebuild

Echo Creating Package
%NugetPath%\NuGet.exe pack %NugetCsproj% -Prop Configuration=Release -Exclude **\*.dll -Exclude web.config -Exclude Web.config.transform -Exclude Global.asax -Exclude Scripts/*
pause
