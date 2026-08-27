using UglyToad.PdfPig;

namespace SIGSTO.Services
{
    public class CVScoringService
    {
        public string ExtraireTexte(string cheminPDF)
        {
            try
            {
                using var document = PdfDocument.Open(cheminPDF);
                var texte = "";
                foreach (var page in document.GetPages())
                {
                    texte += page.Text + " ";
                }
                return texte.ToLower().Trim();
            }
            catch
            {
                return "";
            }
        }

        public float CalculerScore(string texte, string motsCles)
        {
            if (string.IsNullOrWhiteSpace(texte) || string.IsNullOrWhiteSpace(motsCles))
                return 0;

            var keywords = motsCles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (keywords.Length == 0) return 0;

            var texteLower = texte.ToLower();
            int found = 0;
            foreach (var kw in keywords)
            {
                if (texteLower.Contains(kw.ToLower().Trim()))
                    found++;
            }

            return (float)found / keywords.Length * 100;
        }
    }
}
