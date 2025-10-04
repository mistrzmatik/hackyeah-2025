FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.sln ./
COPY backend-src/*.csproj ./backend-src/
RUN dotnet restore
COPY . .
RUN dotnet publish backend-src/backend-src.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# ASPNETCORE_URLS pozwala nasłuchiwać na porcie 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "backend-src.dll"]