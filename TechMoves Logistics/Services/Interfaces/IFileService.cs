namespace TechMoves_Logistics.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> SavePdfAsync(IFormFile file);
        bool IsValidPdf(IFormFile file);
        void DeleteFile(string filePath);
    }
}
