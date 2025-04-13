## Install Dependencies

- [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)
- [nodejs](https://nodejs.org/)
- [yarn](https://yarnpkg.com/): `$ npm install --global yarn`
- [docker](https://www.docker.com/products/docker-desktop/)

## Configuration

You have to create a `appsettings.Local.json` with the following entries

```
{
  "AzureAdB2C": {
    "TenantName": "vreedaid",
    "ClientId": "<ask for client id at vreeda>",
    "ClientSecret": "<ask for client secret at vreeda>",
    "PrimaryUserFlow": "B2C_1A_VREELI_SERVICE_SIGNIN_PROD"
  }
}
```

## Getting Started With Development

First, start the backend development server:

```bash
$ dotnet run
```

If you get a warnging about https certificates run

```bash
$ dotnet dev-certs https --trust
```

Open [https://localhost:3000](https://localhost:3000) with your browser to see the result.

This project uses [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet) for the backend and [React](https://react.dev/) in combination with [React Material UI](https://mui.com/material-ui/getting-started/) for the frontend.

You can start editing the frontend by modifying `Frontend/src/pages/Home.tsx`. The page auto-updates as you edit the file.

You can extend the backend logic in the `Api` folder and subfolders. The backend auto-updates as you edit files. 

The backend contains the basic functionality to implement a vreeda service:

- authentication and authorization based on vreeda id management in `/Api/Auth`
- user configuration and device access token management in `/Api/User`
- basic VREEDA api client in `/Api/Vreeda`
- externally triggered background jobs in `/Api/Jobs` 

## Learn More

To learn more about the VREEDA platform, take a look at the following resources:

- [VREEDA API Documentation](https://api.vreeda.com/)
