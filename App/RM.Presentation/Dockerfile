# Use multi-stage builds to keep the final image lightweight
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only the project files first to leverage Docker cache
COPY ["RM.Presentation/RM.Presentation.csproj", "RM.Presentation/"]
COPY ["RM.Application/RM.Application.csproj", "RM.Application/"]
COPY ["RM.Domain/RM.Domain.csproj", "RM.Domain/"]
COPY ["RM.Infrastructure/RM.Infrastructure.csproj", "RM.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "RM.Presentation/RM.Presentation.csproj"

# Copy the rest of the application code
COPY . .

# Build the application
WORKDIR "/src/RM.Presentation"
RUN dotnet build "RM.Presentation.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "RM.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Copy the published output
COPY --from=publish /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "RM.Presentation.dll"]