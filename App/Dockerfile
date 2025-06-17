FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy only the project files first to leverage Docker cache
COPY ["RM.Presentation/RM.Presentation.csproj", "RM.Presentation/"]
COPY ["RM.Application/RM.Application.csproj", "RM.Application/"]
COPY ["RM.Domain/RM.Domain.csproj", "RM.Domain/"]
COPY ["RM.Infrastructure/RM.Infrastructure.csproj", "RM.Infrastructure/"]

RUN dotnet restore "RM.Presentation/RM.Presentation.csproj"

COPY . .

WORKDIR "/src/RM.Presentation"
RUN dotnet build "RM.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RM.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "RM.Presentation.dll"]