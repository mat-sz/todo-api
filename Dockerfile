FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build

WORKDIR /app
ENV PATH /root/.dotnet/tools:$PATH
COPY . /app

RUN dotnet restore
RUN dotnet tool install --global dotnet-ef
RUN dotnet ef database update

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime
WORKDIR /app
COPY --from=build /app/out ./
COPY --from=build /app/app.db ./app.db
COPY --from=build /app/appsettings.json ./appsettings.json

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "todo-api.dll"]