using System.Xml.Linq;

namespace UdeCDocsMVC.Models.SysModels
{
    public class HomeModel
    {
        public HomeModel()
        {
            Documents = new HashSet<Document>();
            DocumentsOrder = new HashSet<Document>();
            DocumentsIndex = new HashSet<DocumentRecIndex>();
        }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<DocumentRecIndex> DocumentsIndex { get; set; }
        public virtual ICollection<Document> DocumentsOrder { get; set; }

        public void orderDocuments() {
            int cantDocumentsToShow = 0;
            float aux = 0;
            float recommendationIndex = 0;
            foreach (var document in Documents) {
                recommendationIndex = document.calcRecommendationIndex();
                DocumentsIndex.Add(new DocumentRecIndex { Iddocument = document.Iddocument, RecoIndex = recommendationIndex});
            }
            DocumentsIndex = DocumentsIndex.OrderByDescending(d => d.RecoIndex).ToList();
            while (cantDocumentsToShow != DocumentsIndex.Count && cantDocumentsToShow < 10) {
                var document = Documents.Where(d => d.Iddocument == DocumentsIndex.ElementAt(cantDocumentsToShow).Iddocument).Single();
                DocumentsOrder.Add(document);
                cantDocumentsToShow++;
            }
        }

    }
}
