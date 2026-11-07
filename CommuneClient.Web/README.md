# CommuneClient.Web

ASP.NET Core MVC replacement for the existing WinForms CommuneClient.

## Architecture

- Browser clients connect to this application through IIS over the commune LAN.
- This application calls the existing CommuneServer HTTP API on the same Windows PC.
- PostgreSQL and the existing CommuneServer application remain unchanged.
- The JWT returned by CommuneServer is kept in the protected ASP.NET Core authentication ticket. It is not returned to browser JavaScript or stored in local storage.

## Development

1. Install the .NET 10 SDK.
2. Open `CommuneClient.Web.csproj` in Visual Studio.
3. Confirm `CommuneApi:BaseUrl` points to the local CommuneServer API.
4. Start CommuneServer and make sure its API is listening.
5. Run the web project and open its local development URL.

## IIS deployment

1. Install the .NET 10 ASP.NET Core Hosting Bundle on the commune server PC.
2. Publish this project to a folder, for example `C:\CommuneSystem\CommuneClient.Web\`.
3. In IIS, create a website pointing to the publish folder.
4. Use an application pool with **.NET CLR Version: No Managed Code** and enable **Load User Profile**.
5. Add an HTTP binding on port 80. Leave the host name empty if the site must be reachable by both IP address and a LAN DNS name.
6. Grant the application-pool identity Modify permission on the configured Data Protection key directory.
7. Restrict inbound TCP port 80 in Windows Firewall to the commune's local subnet only.

Example browser addresses:

- `http://192.168.1.35/`
- `http://commune-server/` (after the LAN DNS name resolves to the server PC)

## CommuneServer API

The default API URL is `http://127.0.0.1:5050/`.

For this loopback-only design, configure `LocalApiIp` in the deployed CommuneServer `App.config` as `127.0.0.1`, keep `LocalApiPort` as `5050`, restart CommuneServer, and verify:

`http://127.0.0.1:5050/api/server/ping`

Do this only after confirming that no remaining desktop client on another PC needs direct access to the old API. If such clients still need it, keep the API LAN binding temporarily and block its port from ordinary client PCs using Windows Firewall.

## HTTP warning

This deployment is intended for a trusted, isolated local network only. HTTP does not encrypt passwords, browser cookies, or application data. Do not publish this site or API to the internet. Use HTTPS or a VPN if access outside the isolated LAN is ever required.
