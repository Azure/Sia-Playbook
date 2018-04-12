FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

COPY . . 

WORKDIR /app/Sia.Playbook
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/Sia.Playbook/Sia.Playbook/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "Sia.Playbook.dll"]
