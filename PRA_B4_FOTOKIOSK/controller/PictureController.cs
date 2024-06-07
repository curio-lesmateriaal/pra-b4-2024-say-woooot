using PRA_B4_FOTOKIOSK.magie;
using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PRA_B4_FOTOKIOSK.controller
{
    public class PictureController
    {
        // De window die we laten zien op het scherm
        public static Home Window { get; set; }

        // De lijst met fotos die we laten zien
        public List<KioskPhoto> PicturesToDisplay = new List<KioskPhoto>();

        // Start methode die wordt aangeroepen wanneer de foto pagina opent.
        public void Start()
        {
            // Initializeer de lijst met fotos
            var now = DateTime.Now;
            int today = (int)now.DayOfWeek;
            var minTime = now.AddMinutes(-30); // 30 minuten geleden
            var maxTime = now.AddMinutes(-2);  // 2 minuten geleden

            // Dictionary om de fotos met tijd van maken op te slaan
            Dictionary<DateTime, string> photoDictionary = new Dictionary<DateTime, string>();

            foreach (string dir in Directory.GetDirectories(@"../../../fotos"))
            {
                string folderName = Path.GetFileName(dir); //pakt de hele map 0_Zondag
                string[] parts = folderName.Split('_'); //split op de underscore
                if (parts.Length > 0 && int.TryParse(parts[0], out int dayFromFolder) && dayFromFolder == today)
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
                            if (photoTime >= minTime && photoTime <= maxTime)
                            {
                                // Voeg de foto toe aan de dictionary
                                photoDictionary[photoTime] = file;
                            }
                        }
                    }
                }
            }

            // Maak paren van fotos 60 seconden na elkaar
            foreach (var photoTime in photoDictionary.Keys.ToList())
            {
                var pairedTime = photoTime.AddSeconds(60);
                if (photoDictionary.ContainsKey(pairedTime))
                {
                    // Voeg ze beide toe aan de display
                    PicturesToDisplay.Add(new KioskPhoto() { Id = 0, Source = photoDictionary[photoTime] });
                    PicturesToDisplay.Add(new KioskPhoto() { Id = 0, Source = photoDictionary[pairedTime] });

                    // Verwijder de gepaarde foto ivm duplicaten
                    photoDictionary.Remove(pairedTime);
                }
            }

            // Update de fotos
            PictureManager.UpdatePictures(PicturesToDisplay);
        }

        // Wordt uitgevoerd wanneer er op de Refresh knop is geklikt
        public void RefreshButtonClick()
        {
            
        }
    }
}
