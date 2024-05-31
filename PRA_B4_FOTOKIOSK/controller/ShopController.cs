using PRA_B4_FOTOKIOSK.magie;
using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PRA_B4_FOTOKIOSK.controller
{
    public class ShopController
    {

        public static Home Window { get; set; }
        private double? runningTotal = 0;
        private List<OrderedProduct> orderedProducts = new List<OrderedProduct>();

        public void Start()
        {
            // Vul de productlijst met producten
            ShopManager.Products.Add(new KioskProduct()
            {
                Name = "Foto 10x15",
                Description = "Afdrukformaat 10x15 cm",
                Price = 2.55
            });
            ShopManager.Products.Add(new KioskProduct()
            {
                Name = "Foto 20x30",
                Description = "Afdrukformaat 20x30 cm",
                Price = 5.00
            });
            ShopManager.Products.Add(new KioskProduct()
            {
                Name = "T-Shirt met foto",
                Description = "Maten XS/S/M/L/XL/XXL",
                Price = 25.00
            });


            // Maak een nieuwe prijslijst string
            StringBuilder priceList = new StringBuilder("Prijzen:\n");

            // Voeg de koppen van de tabel toe
            priceList.AppendLine($"{"Naam",-20} {"Beschrijving",-40} {"Prijs",10}");
            priceList.AppendLine(new string('-', 70)); // scheidingslijn

            // Loop door de producten en voeg ze toe aan de prijslijst
            foreach (KioskProduct product in ShopManager.Products)
            {
                priceList.AppendLine($"{product.Name,-20} {product.Description,-40} {product.Price,10:C}");
            }

            // Stel de prijslijst in aan de rechter kant.
            ShopManager.SetShopPriceList(priceList.ToString());

            // Stel de bon in onderaan het scherm
            ShopManager.SetShopReceipt("Eindbedrag\n€");

            // Update dropdown met producten
            ShopManager.UpdateDropDownProducts();
        }

        // Wordt uitgevoerd wanneer er op de Toevoegen knop is geklikt
        public void AddButtonClick()
        {
            var selectedProduct = ShopManager.GetSelectedProduct();
            int? fotoId = ShopManager.GetFotoId();
            int? amount = ShopManager.GetAmount();

            if (selectedProduct != null && fotoId.HasValue && amount.HasValue)
            {
                double? totalPrice = selectedProduct.Price * amount;
                double? price = selectedProduct.Price;

                runningTotal += totalPrice;

                orderedProducts.Add(new OrderedProduct
                {
                    FotoId = fotoId.Value,
                    ProductName = selectedProduct.Name,
                    Amount = amount.Value,
                    TotalPrice = totalPrice.Value,
                    Price = price.Value,
                });

                updateReceipt();
            }
            else
            {
                ShopManager.SetShopReceipt("Error: Ongeldige Invoer");
            }
        }

        private void updateReceipt()
        {
            StringBuilder receipt = new StringBuilder();

            foreach (var orderedProduct in orderedProducts)
            {
                receipt.AppendLine($"{orderedProduct.ProductName} (Foto-ID: {orderedProduct.FotoId})            {orderedProduct.Amount} * {orderedProduct.Price:C}              {orderedProduct.TotalPrice:C}");
            }

            receipt.AppendLine($"Totaal: {runningTotal:C}");

            // Stel de bijgewerkte bon in
            ShopManager.SetShopReceipt(receipt.ToString());
        }

        // Wordt uitgevoerd wanneer er op de Resetten knop is geklikt
        public void ResetButtonClick()
        {
            runningTotal = 0;
            orderedProducts.Clear();
            ShopManager.SetShopReceipt("Eindbedrag\n€");
        }

        // Wordt uitgevoerd wanneer er op de Save knop is geklikt
        public void SaveButtonClick()
        {
            // Controleer of er items op de bon staan om op te slaan
            if (orderedProducts.Count > 0)
            {
                // Genereer een unieke bestandsnaam voor de bon
                string fileName = $@"C:\Users\jordy\Links\Kassabon_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                try
                {
                    // Schrijf de inhoud van de bon naar een tekstbestand
                    File.WriteAllText(fileName, ShopManager.GetShopReceipt());

                    // Geef een melding dat de bon succesvol is opgeslagen
                    ShopManager.ShowMessage($"De kassabon is opgeslagen als Kassabon_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                }
                catch (Exception ex)
                {
                    // Geef een melding als er een fout optreedt bij het opslaan van de bon
                    ShopManager.ShowMessage($"Er is een fout opgetreden bij het opslaan van de kassabon: {ex.Message}");
                }
            }
            else
            {
                // Geef een melding als er geen items op de bon staan om op te slaan
                ShopManager.ShowMessage("Er zijn geen items op de bon om op te slaan.");
            }
        }
    }
}

