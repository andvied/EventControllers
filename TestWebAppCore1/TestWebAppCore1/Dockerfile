FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TestWebAppCore1.dll"]

# cd C:\Users\andvie\Documents\Visual Studio 2017\Projects\TestApps\TestWebAppCore1\TestWebAppCore1
# docker build -t aspnetapp .
# docker run -d -p 8080:80 --name myapp aspnetapp