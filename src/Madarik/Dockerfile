FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Madarik/Madarik.csproj", "Madarik/"]
RUN dotnet restore "Madarik/Madarik.csproj"
COPY /src .
WORKDIR "/src/Madarik"
RUN dotnet build "Madarik.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Madarik.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Madarik.dll"]
