using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;

namespace IBOTracker.Google
{
    internal static class Services
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        private static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private static UserCredential SheetCredential;

        private static Dictionary<string, SheetsService> sheetServices = new Dictionary<string, SheetsService>(StringComparer.InvariantCultureIgnoreCase);

        public static SheetsService GetSheetsService(string appName)
        {
            SheetsService service;
            if (sheetServices.TryGetValue(appName, out service)) return service;

            InitSheetCredential();
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = SheetCredential,
                ApplicationName = appName,
            });

            sheetServices.Add(appName, service);
            return service;
        }

        public static void Clear()
        {
            sheetServices.Clear();
            Directory.Delete("./Google/token.$$$", true);
        }

        private static void InitSheetCredential()
        {
            if (SheetCredential != null) return;

            using (var stream = new FileStream("./Google/sheets.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "./Google/token.$$$";
                SheetCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

                Logger.Debug($"Credential file saved to: {credPath}");
            }
        }
    }
}
