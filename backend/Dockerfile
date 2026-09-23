# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os arquivos de projeto (csproj) e restaura dependências
COPY ["src/Catarinense.API/Catarinense.API.csproj", "Catarinense.API/"]
COPY ["src/Catarinense.Application/Catarinense.Application.csproj", "Catarinense.Application/"]
COPY ["src/Catarinense.Domain/Catarinense.Domain.csproj", "Catarinense.Domain/"]
COPY ["src/Catarinense.Infrastructure/Catarinense.Infrastructure.csproj", "Catarinense.Infrastructure/"]
RUN dotnet restore "Catarinense.API/Catarinense.API.csproj"

# Copia todo o código restante e faz a publicação (publish)
COPY src/ .
WORKDIR "/src/Catarinense.API"
RUN dotnet publish "Catarinense.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final de execução (imagem menor, apenas runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Variáveis de ambiente pro Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Catarinense.API.dll"]
