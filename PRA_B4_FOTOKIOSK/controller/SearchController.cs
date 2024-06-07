using PRA_B4_FOTOKIOSK.magie;
using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRA_B4_FOTOKIOSK.controller
{
    public class SearchController
    {
        // De window die we laten zien op het scherm
        public static Home Window { get; set; }
        

        // Start methode die wordt aangeroepen wanneer de zoek pagina opent.
        public void Start()
        {
            SearchManager.Instance = Window;
        }

        // Wordt uitgevoerd wanneer er op de Zoeken knop is geklikt
        public void SearchButtonClick()
        {
            SearchPhotos();
        }

        // Nieuwe methode voor het zoeken naar foto's op dag en tijd
        public void SearchPhotos()
        {
            // Vraag de zoekinput op van de gebruiker
            string searchInput = SearchManager.GetSearchInput();

            if (!string.IsNullOrEmpty(searchInput))
            {
                string[] parts = searchInput.Split('_');
                if (parts.Length == 2 && int.TryParse(parts[0], out int dayFromInput) && TimeSpan.TryParse(parts[1] + ":00", out TimeSpan timeFromInput))
                {
                    var now = DateTime.Now;
                    var searchDate = now.Date.AddDays(dayFromInput - (int)now.DayOfWeek);
                    var searchDateTime = searchDate.Add(timeFromInput);

                    foreach (string dir in Directory.GetDirectories(@"../../../fotos"))
                    {
                        string folderName = Path.GetFileName(dir);
                        string[] folderParts = folderName.Split('_');
                        if (folderParts.Length > 0 && int.TryParse(folderParts[0], out int dayFromFolder) && dayFromFolder == (int)searchDateTime.DayOfWeek)
                        {
                            foreach (string file in Directory.GetFiles(dir))
                            {
                                string fileName = Path.GetFileNameWithoutExtension(file);
                                string[] timeParts = fileName.Split('_');

                                if (timeParts.Length >= 3 &&
                                    int.TryParse(timeParts[0], out int hour) &&
                                    int.TryParse(timeParts[1], out int minute) &&
                                    int.TryParse(timeParts[2], out int second))
                                {
                                    var photoTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, second);

                                    if (photoTime.Hour == searchDateTime.Hour && photoTime.Minute == searchDateTime.Minute)
                                    {
                                        SearchManager.SetPicture(file);

                                        // De afbeelding informatie
                                        string imageInfo = $"ID: {fileName}, Time: {photoTime:HH:mm:ss}, Date: {photoTime:yyyy-MM-dd}";
                                        SearchManager.SetSearchImageInfo(imageInfo);

                                        return; // Stop na het vinden van de eerste match
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    SearchManager.ShowMessage("Ongeldige zoekopdracht. Gebruik het formaat 'dag_tijd' (bijv. '3_14:30').");
                }
            }
            else
            {
                SearchManager.ShowMessage("Zoekopdracht mag niet leeg zijn.");
            }
        }
    }
}
