# Use the official ASP.NET Core runtime image as a base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Correct the path for the COPY command
COPY ["HngBackendNumberClassifier.csproj", "./"]
RUN dotnet restore "HngBackendNumberClassifier.csproj"

# Copy the entire project and build it
COPY . .
WORKDIR "/src"
RUN dotnet build "HngBackendNumberClassifier.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "HngBackendNumberClassifier.csproj" -c Release -o /app/publish

# Final stage to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HngBackendNumberClassifier.dll"]
