# Începerea analizei SonarQube
dotnet sonarscanner begin /k:"PHMS" /d:sonar.host.url="http://localhost:9000" /d:sonar.token="sqp_831f2d6f94f1db83b97cb878823793019b24ba3f" /d:sonar.coverageReportPaths="TestResults/SonarQubeReport/sonarqube.xml"

# Construirea proiectului
dotnet build

# Executarea testelor unitare și colectarea acoperirii de cod
dotnet test PHMS.UnitTests --collect:"XPlat Code Coverage" --results-directory ".\TestResults\UnitTests"

# Mutarea fișierului de acoperire unități în locația dorită
Move-Item -Path (Get-ChildItem -Path ".\TestResults\UnitTests" -Recurse -Filter "coverage.cobertura.xml").FullName -Destination ".\TestResults\coverage.unit.cobertura.xml" -Force

# Executarea testelor de integrare și colectarea acoperirii de cod
dotnet test PHMS.IntegrationTests --collect:"XPlat Code Coverage" --results-directory ".\TestResults\IntegrationTests"

# Mutarea fișierului de acoperire integrare în locația dorită
Move-Item -Path (Get-ChildItem -Path ".\TestResults\IntegrationTests" -Recurse -Filter "coverage.cobertura.xml").FullName -Destination ".\TestResults\coverage.integration.cobertura.xml" -Force

# Generarea raportului pentru SonarQube
reportgenerator -reports:".\TestResults\coverage.unit.cobertura.xml;.\TestResults\coverage.integration.cobertura.xml" -targetdir:".\TestResults\SonarQubeReport" -reporttypes:"SonarQube"

# Finalizarea analizei SonarQube
dotnet sonarscanner end /d:sonar.token="sqp_831f2d6f94f1db83b97cb878823793019b24ba3f"