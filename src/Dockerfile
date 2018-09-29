FROM microsoft/aspnetcore-build:2.0 AS build-env

WORKDIR /src
# copy csproj and restore as distinct layers
COPY *.sln ./
COPY Projets.Bean/*.csproj Projets.Bean/
COPY Projets.WS/*.csproj Projets.WS/
RUN dotnet restore

# copy everything else and build
COPY . ./
WORKDIR /src/Projets.WS
RUN dotnet publish -c Release -o out

# build runtime image
FROM microsoft/aspnetcore-build:2.0
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Docker
COPY --from=build-env /src/Projets.WS/out .
ENTRYPOINT ["dotnet", "Projets.WS.dll"]
