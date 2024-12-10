FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ./Application/Application.csproj Application/
RUN ls -l /app/Application

COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY PHMS/PHMS.csproj WebAPI/PHMS/

RUN dotnet restore WebAPI/PHMS/PHMS.csproj

COPY . .

WORKDIR /app/PHMS
RUN dotnet build PHMS.csproj -c Release -o /app/build

RUN dotnet publish PHMS.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PHMS.dll"]