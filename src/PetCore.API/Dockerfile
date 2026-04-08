FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["src/PetCore.API/PetCore.API.csproj", "src/PetCore.API/"]
COPY ["src/PetCore.Domain/PetCore.Domain.csproj", "src/PetCore.Domain/"]
COPY ["src/PetCore.Infrastructure/PetCore.Infrastructure.csproj", "src/PetCore.Infrastructure/"]
RUN dotnet restore "src/PetCore.API/PetCore.API.csproj"
COPY . .
WORKDIR "/src/src/PetCore.API"
RUN dotnet build "PetCore.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PetCore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PetCore.API.dll"]
