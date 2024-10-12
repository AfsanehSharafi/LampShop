using _0_Framework.Application;
using ShopManagement.Application.Contracts.Product;
using ShopManagement.Domain.ProductAgg;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ShopManagement.Application
{
    public class ProductApplication : IProductApplication
    {
        private readonly IProductRepository _productRepository;
        public ProductApplication(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


        public OperationResult Create(CreateProduct command)
        {
            var operation = new OperationResult();
            if (_productRepository.Exists(x => x.Name == command.Name))
                return operation.Failed(ApplicationMessages.DuplicatedRecord);

            var slug = command.Slug.GenerateSlug();
            //var categorySlug = _productRepository.GetSlugById(command.CategoryId);
            //var path = $"{categorySlug}//{slug}";
            //var picturePath = _fileUploader.Upload(command.Picture, path);
            var product = new Product(command.Name, command.Code,
                command.ShortDescription, command.Description,/* picturePath,*/
                command.PictureAlt, command.PictureTitle, command.CategoryId, slug,
                command.Keywords, command.MetaDescription,command.Picture);
            _productRepository.Create(product);
            _productRepository.SaveChanges();
            return operation.Succeded();
        }

        public OperationResult Edit(EditProduct command)
        {
            var operation = new OperationResult();
            var product = _productRepository.Get(command.Id);
            if (product == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            if (_productRepository.Exists(x => x.Name == command.Name && x.Id != command.Id))
                return operation.Failed(ApplicationMessages.DuplicatedRecord);

            var slug = command.Slug.GenerateSlug();
            //var path = $"{product.Category.Slug}/{slug}";

            //var picturePath = _fileUploader.Upload(command.Picture, path);
            product.Edit(command.Name, command.Code,
                command.UnitPrice,command.Picture,
                command.ShortDescription, command.Description,/* picturePath,*/
                command.PictureAlt, command.PictureTitle, command.CategoryId, slug,
                command.Keywords, command.MetaDescription);

            _productRepository.SaveChanges();
            return operation.Succeded();
        }

        public EditProduct GetDetails(long id)
        {
            return _productRepository.GetDetails(id);
        }

        public List<ProductViewModel> GetProducts()
        {
            return _productRepository.GetProducts();
        }

        public OperationResult IsStock(long id)
        {
            var operation = new OperationResult();
            var product = _productRepository.Get(id);
            if (product == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);
            product.InStock();
            _productRepository.SaveChanges();
            return operation.Succeded();
        }

        public OperationResult NotIsStock(long id)
        {
            var operation = new OperationResult();
            var product = _productRepository.Get(id);
            if (product == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);
            product.NotIdStock();
            _productRepository.SaveChanges();
            return operation.Succeded();
        }

        public List<ProductViewModel> Search(ProductSearchModel searchModel)
        {
            return _productRepository.Search(searchModel);
        }
    }
}
