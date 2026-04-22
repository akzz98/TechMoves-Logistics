using TechMoves_Logistics.Services.Interfaces;

namespace TechMoves_Logistics.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public bool IsValidPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".pdf")
                return false;

            // Check the MIME type
            if (file.ContentType.ToLowerInvariant() != "application/pdf")
                return false;

            return true;
        }
        public async Task<string> SavePdfAsync(IFormFile file)
        {
            if (!IsValidPdf(file))
                throw new InvalidOperationException("Only PDF files are allowed.");

            // Build upload folder path
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            // Create folder if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // UUID filename to prevent overwrites
            var uniqueFileName = $"{Guid.NewGuid()}.pdf";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            // Return relative path for storage in DB
            return $"/uploads/{uniqueFileName}";
        }
        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}