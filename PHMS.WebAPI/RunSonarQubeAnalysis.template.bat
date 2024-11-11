@echo off
SETLOCAL

dotnet sonarscanner begin /k:"PHMS.WebAPI" /d:sonar.host.url="http://localhost:9000" /d:sonar.token="YOUR_TOKEN" /d:sonar.coverageReportPaths="TestResults/SonarQubeReport/sonarqube.xml"
IF ERRORLEVEL 1 (
    echo Error: SonarScanner begin failed. Exiting.
    EXIT /B 1
)

dotnet build
IF ERRORLEVEL 1 (
    echo Error: Build failed. Exiting.
    EXIT /B 1
)

dotnet test PHMS.UnitTests --collect:"XPlat Code Coverage" --results-directory ".\TestResults\UnitTests"
IF ERRORLEVEL 1 (
    echo Error: Unit tests failed. Exiting.
    EXIT /B 1
)

for /r ".\TestResults\UnitTests" %%f in (coverage.cobertura.xml) do move "%%f" ".\TestResults\coverage.unit.cobertura.xml"
IF ERRORLEVEL 1 (
    echo Error: Failed to move unit test coverage file. Exiting.
    EXIT /B 1
)

dotnet test PHMS.IntegrationTests --collect:"XPlat Code Coverage" --results-directory ".\TestResults\IntegrationTests"
IF ERRORLEVEL 1 (
    echo Error: Integration tests failed. Exiting.
    EXIT /B 1
)

for /r ".\TestResults\IntegrationTests" %%f in (coverage.cobertura.xml) do move "%%f" ".\TestResults\coverage.integration.cobertura.xml"
IF ERRORLEVEL 1 (
    echo Error: Failed to move integration test coverage file. Exiting.
    EXIT /B 1
)

reportgenerator -reports:".\TestResults\coverage.unit.cobertura.xml;.\TestResults\coverage.integration.cobertura.xml" -targetdir:".\TestResults\SonarQubeReport" -reporttypes:"SonarQube"
IF ERRORLEVEL 1 (
    echo Error: Report generation failed. Exiting.
    EXIT /B 1
)

dotnet sonarscanner end /d:sonar.token="YOUR_TOKEN"
IF ERRORLEVEL 1 (
    echo Error: SonarScanner end failed. Exiting.
    EXIT /B 1
)

ENDLOCAL
