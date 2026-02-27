using AnimalShelterAPI.Infrastructure.Repositories;
using AnimalShelterAPI.Models;
using AnimalShelterAPI.Services.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<Animal> _repository;
        private readonly IReportRepository _reportRepository;

        private readonly Dictionary<int, string> TypeToSpelling = new Dictionary<int, string>()
        {
            { 0, "pas"},
            { 1, "mačke"},
            { 2, "drugih" }
        };

        private readonly Dictionary<int, string> FurToSpelling = new Dictionary<int, string>()
        {
            { 0, "Kratkodlaki" },
            { 1, "Grubodlaki" },
            { 2, "Srednje dužine" },
            { 3, "Dugodlaki" }
        };

        public ReportService(IRepository<Animal> repository, IReportRepository reportRepository)
        {
            _repository = repository;
            _reportRepository = reportRepository;
        }

        public async Task<MemoryStream> GenerateAdmissionAct(int id)
        {
            var templateFile = Path.Combine(Directory.GetCurrentDirectory(), "Report_templates", "zapisnikOprijemu.docx");
            var animal = await _repository.GetById(id);

            var stream = new MemoryStream();
            var fileBytesArray = await File.ReadAllBytesAsync(templateFile);
            await stream.WriteAsync(fileBytesArray, 0, fileBytesArray.Length);
            stream.Position = 0;

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
            {
                // Zamena u tabelama
                foreach (var table in wordDoc.MainDocumentPart.Document.Body.Elements<Table>())
                {
                    foreach (var row in table.Elements<TableRow>())
                    {
                        foreach (var cell in row.Elements<TableCell>())
                        {
                            string cellText = string.Join("", cell.Descendants<Text>().Select(t => t.Text));

                            cellText = cellText
                                .Replace("{datum_prijema}", animal.AdmissionDate.ToString("yyyy-MM-dd"))
                                .Replace("{grad}", animal.AdmissionCity ?? "-")
                                .Replace("{region}", animal.AdmissionRegion ?? "-")
                                .Replace("{vrsta}", animal.AnimalType.ToString())
                                .Replace("{pol}", animal.Gender.ToString())
                                .Replace("{starost}", animal.Birthday == null ? "-" : FormatAnimalAge((DateTime.Today - animal.Birthday.Value).TotalDays))
                                .Replace("{dlaka}", FurToSpelling[(int)animal.FurType])
                                .Replace("{oznaka}", animal.SpecialTags ?? "-")
                                .Replace("{zdravstveno_stanja}", animal.HealthCondition ?? "-")
                                .Replace("{specijalni_ID}", animal.SpecialID ?? "-")
                                .Replace("{datum_ugradnje_cipa}", animal.MicrochipIntegrationDate?.ToString("yyyy-MM-dd") ?? "-")
                                .Replace("{datum_vakcinacije}", animal.VaccinationDate?.ToString("yyyy-MM-dd") ?? "-")
                                .Replace("{kontakti_prijemne_organizacije}", animal.AdmissionOrganisationContacts ?? "-")
                                .Replace("{kontakti_preduzne_organizacije}", animal.TransferOrganisationContacts ?? "-");

                            foreach (var text in cell.Descendants<Text>().ToList())
                                text.Remove();

                            cell.Append(new Paragraph(new Run(new Text(cellText))));
                        }
                    }
                }

                // Zamena u paragrafima van tabela
                foreach (var paragraph in wordDoc.MainDocumentPart.Document.Body.Elements<Paragraph>())
                {
                    string paragraphText = string.Join("", paragraph.Descendants<Text>().Select(t => t.Text));

                    paragraphText = paragraphText
                        .Replace("{datum_prijema}", animal.AdmissionDate.ToString("yyyy-MM-dd"))
                        .Replace("{grad}", animal.AdmissionCity ?? "-")
                        .Replace("{region}", animal.AdmissionRegion ?? "-")
                        .Replace("{vrsta}", animal.AnimalType.ToString())
                        .Replace("{pol}", animal.Gender.ToString())
                        .Replace("{starost}", animal.Birthday == null ? "-" : FormatAnimalAge((DateTime.Today - animal.Birthday.Value).TotalDays))
                        .Replace("{dlaka}", FurToSpelling[(int)animal.FurType])
                        .Replace("{oznaka}", animal.SpecialTags ?? "-")
                        .Replace("{zdravstveno_stanja}", animal.HealthCondition ?? "-")
                        .Replace("{specijalni_ID}", animal.SpecialID ?? "-")
                        .Replace("{datum_ugradnje_cipa}", animal.MicrochipIntegrationDate?.ToString("yyyy-MM-dd") ?? "-")
                        .Replace("{datum_vakcinacije}", animal.VaccinationDate?.ToString("yyyy-MM-dd") ?? "-")
                        .Replace("{kontakti_prijemne_organizacije}", animal.AdmissionOrganisationContacts ?? "-")
                        .Replace("{kontakti_preduzne_organizacije}", animal.TransferOrganisationContacts ?? "-");

                    foreach (var text in paragraph.Descendants<Text>().ToList())
                        text.Remove();

                    paragraph.Append(new Run(new Text(paragraphText)));
                }

                wordDoc.MainDocumentPart.Document.Save();
            }

            stream.Position = 0;
            return stream;
        }

        public async Task<MemoryStream> GenerateYearReport(int AnimalType, int Year)
        {
            var templateFile = Path.Combine(Directory.GetCurrentDirectory(), "Report_templates", "godisnjiIzvestaj.docx");
            var report = await _reportRepository.GetAnimalReport(AnimalType, Year);

            var stream = new MemoryStream();
            var fileBytesArray = await File.ReadAllBytesAsync(templateFile);
            await stream.WriteAsync(fileBytesArray, 0, fileBytesArray.Length);
            stream.Position = 0;

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
            {
                // Zamena u tabelama
                foreach (var table in wordDoc.MainDocumentPart.Document.Body.Elements<Table>())
                {
                    foreach (var row in table.Elements<TableRow>())
                    {
                        foreach (var cell in row.Elements<TableCell>())
                        {
                            string cellText = string.Join("", cell.Descendants<Text>().Select(t => t.Text));
                            cellText = cellText
                                .Replace("{primljeno}", report.AdmittedCount.ToString())
                                .Replace("{poklonjeno}", report.GiftedCount.ToString())
                                .Replace("{smrti}", report.DeadCount.ToString())
                                .Replace("{trenutno_zivi}", report.LivingNowCount.ToString());
                            foreach (var text in cell.Descendants<Text>().ToList())
                                text.Remove();
                            cell.Append(new Paragraph(new Run(new Text(cellText))));
                        }
                    }
                }

                // Zamena u paragrafima van tabela
                foreach (var paragraph in wordDoc.MainDocumentPart.Document.Body.Elements<Paragraph>())
                {
                    string paragraphText = string.Join("", paragraph.Descendants<Text>().Select(t => t.Text));
                    paragraphText = paragraphText
                        .Replace("{godina}", Year.ToString())
                        .Replace("{datum}", DateTime.Now.ToString("yyyy-MM-dd"))
                        .Replace("{vrsta_zivotinje}", TypeToSpelling[AnimalType]);
                    foreach (var text in paragraph.Descendants<Text>().ToList())
                        text.Remove();
                    paragraph.Append(new Run(new Text(paragraphText)));
                }

                wordDoc.MainDocumentPart.Document.Save();
            }

            stream.Position = 0;
            return stream;
        }

        private string FormatAnimalAge(double DayCount)
        {
            double years = Math.Truncate(DayCount / 365);
            double months = Math.Truncate((DayCount % 365) / 30);
            double days = Math.Truncate((DayCount % 365) % 30);
            return string.Format("{0} godina {1} mesec {2} dana", years, months, days);
        }
    }
}
