# Use the official ASP.NET Core runtime image as a base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy just the project files and restore dependencies
COPY ClassifyNumber.csproj .
COPY Properties/ Properties/
COPY Controller/ Controller/
COPY appsettings.Development.json .
COPY appsettings.json .
COPY Program.cs .

# Restore packages
RUN dotnet restore "ClassifyNumber.csproj"

# Copy all remaining files, if necessary
COPY . .

# Build the application
RUN dotnet build "ClassifyNumber.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ClassifyNumber.csproj" -c Release -o /app/publish

# Final stage to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClassifyNumber.dll"]
