namespace ProjektLABDetailing.Models.ViewModels
{
    public class CarImagesViewModel
    {
        public int CarId { get; set; }
        public List<ImageViewModel> ExistingImages { get; set; }
    }

    public class ImageViewModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

}
