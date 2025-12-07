# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["Roboto.Service/Roboto.Service.csproj", "Roboto.Service/"]
COPY ["Roboto.Repository/Roboto.Repository.csproj", "Roboto.Repository/"]
COPY ["Roboto.Models/Roboto.Models.csproj", "Roboto.Models/"]

# Restore dependencies
RUN dotnet restore "Roboto.Service/Roboto.Service.csproj"

# Copy everything else
COPY . .

# Build the application
WORKDIR "/src/Roboto.Service"
RUN dotnet build "Roboto.Service.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Roboto.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published files
COPY --from=publish /app/publish .

# Set entry point
ENTRYPOINT ["dotnet", "Roboto.Service.dll"]