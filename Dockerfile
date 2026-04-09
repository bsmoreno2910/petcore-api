# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first for layer caching
COPY PetCore.slnx ./
COPY src/PetCore.Domain/PetCore.Domain.csproj src/PetCore.Domain/
COPY src/PetCore.Infrastructure/PetCore.Infrastructure.csproj src/PetCore.Infrastructure/
COPY src/PetCore.API/PetCore.API.csproj src/PetCore.API/

# Restore dependencies
RUN dotnet restore PetCore.slnx

# Copy everything else and publish
COPY . .
RUN dotnet publish src/PetCore.API/PetCore.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user
RUN adduser --disabled-password --gecos "" appuser

COPY --from=build /app/publish .

# Switch to non-root user
USER appuser

EXPOSE 8080

ENTRYPOINT ["dotnet", "PetCore.API.dll"]
