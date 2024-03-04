FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PatientsAPI/PatientsAPI.csproj", "PatientsAPI/"]
RUN dotnet restore "PatientsAPI/PatientsAPI.csproj"
COPY . .
WORKDIR "/src/PatientsAPI"
RUN dotnet build "PatientsAPI.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PatientsAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PatientsAPI.dll"]