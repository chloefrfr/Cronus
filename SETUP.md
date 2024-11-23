# Setup Guide

This will be a guide on how to setup the project.

## **NOTE**

Not all fortnite versions are currently supported.

## **Prerequisites**

1. **[Install Dotnet 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0):**

   - Downlaod and install the latest version of the .NET 7.0 SDK.
   - Verify the installation:

   ```bash
   dotnet --version
   ```

2. **[Visual Studio 2022](https://visualstudio.microsoft.com/vs/):**

   - Install the following packages:
     - `.NET desktop development`
     - `ASP.NET and web development`

3. **[PostgreSQL](https://www.postgresql.org/download/):**
   - Download and install the latest version of PostgreSQL.
   - During the installation, remember your username and password for the database.

## **Setup**

# **Step 1: Creating the `Config.json` File**

1. Navigate to the `Larry` folder and create a new file called `Config.json`.

- Must be in the same directory as the `Larry.csproj` file.

2. Open the `Config.json` file in a ide or text editor.
3. Add the following data:

```json
{
  "ConnectionUrl": "Host=localhost;Port=5432;Database=dbtest;Username=postgres;Password=password;Pooling=true;MinPoolSize=1;MaxPoolSize=20;Timeout=600;",
  "Token": "<Your_Discord_Bot_Token>",
  "GuildId": "<Your_Discord_Server_ID>",
  "ClientId": "<Your_Secure_Client_ID>",
  "ClientSecret": "<Your_Secure_Client_Secret>",
  "GameDirectory": "<Path_To_FortniteGame_Directory>",
  "CurrentVersion": "<Fortnite_Build_Version>"
}
```

3. Replace the values with your own.
   - **Token**: Your Discord bot token.
   - **GuildId**: Your Discord server ID.
   - **ClientId**: Can be anything, but it must be unique and secure.
   - **ClientSecret**:Can be anything, but it must be unique and secure.
   - **GameDirectory**: Path to the Fortnite game directory (must include `FortniteGame` and `Engine` folders).
   - **CurrentVersion**: The Fortnite build version (e.g., `11.31`).

## **Step 2: Running the Project**

1. Open the project in **Visual Studio 2022**.
2. Press `F5` to run the project.

## End

If you have any issues or questions, just create a new issue [here](https://github.com/chloefrfr/Larry/issues).
