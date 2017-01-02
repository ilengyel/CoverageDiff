setlocal enableDelayedExpansion
set ROOT=%~dp0..
set XUNIT=%ROOT%\packages\xunit.runner.console.2.1.0\tools\xunit.console.exe 
set OPENCOVER=%ROOT%\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe 
set REPORT_GEN="%ROOT%\packages\ReportGenerator.2.5.1\tools\ReportGenerator.exe"
set RUN_CONTAINERS=%ROOT%\CoverageDiff.Tests\bin\Debug\CoverageDiff.Tests.dll 
set PDBS=%ROOT%\Build
set RESULTSDIR=%ROOT%\Build_Temp\Reports\UnitTest\
set COVERAGE_REPORT=%RESULTSDIR%\OpenCover.xml
set HTML_REPORT=%RESULTSDIR%\OpenCover_Html
set HISTORYDIR=%RESULTSDIR%\History
set COVERAGE_FILTER=+[*]CoverageDiff.* -[*.Tests]* -[*.Specs]* -[*xunit*]* -[*fake*]*
set COVERAGE_TESTS=*

if not exist %RESULTSDIR% mkdir %RESULTSDIR%

::call %XUNIT% %CONTAINERS%
pushd %RESULTSDIR%
%OPENCOVER% ^
    -register:user ^
    -target:"%XUNIT%" ^
    -targetargs:"%RUN_CONTAINERS%" ^
    -searchdirs:%PDBS% ^
    -returntargetcode ^
    -output:%COVERAGE_REPORT% ^
    -skipautoprops ^
    -mergebyhash ^
    -filter:"%COVERAGE_FILTER%" ^
    -excludebyfile:"%COVERAGE_FILEFILTER%" ^
    -coverbytest:"%COVERAGE_TESTS%"

:: Full diff report
%REPORT_GEN% ^
    -reports:"%COVERAGE_REPORT%" ^
    -targetdir:"%HTML_REPORT%" ^
    -reporttypes:Html;XmlSummary ^
    -historydir:"%HISTORYDIR%"
    
popd
endlocal