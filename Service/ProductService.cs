using AutoMapper;
using Contracts;
using Entities.Enums;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service;

internal sealed class ProductService : IProductService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly IFileServiceFactory _fileServiceFactory;

    public ProductService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IFileServiceFactory fileServiceFactory)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _fileServiceFactory = fileServiceFactory;
    }

    public async Task<(IEnumerable<ProductDto> products, MetaData metaData)> GetProductsAsync(Guid categoryId, ProductParameters productParameters,
        bool trackChanges)
    {
        if (!productParameters.ValidPriceRange)
            throw new MaxPriceRangeBadRequestException();
        var category = await _repository.CategoryRepository.GetCategoryAsync(categoryId, trackChanges);
        if(category == null)
            throw new CategoryNotFoundException(categoryId);
        var productsWithMetaData = await _repository.ProductRepository.GetProductsAsync(categoryId, productParameters, trackChanges);
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(productsWithMetaData);
        return (products:  productDtos, metaData: productsWithMetaData.MetaData);
    }

    public async Task<ProductDto?> GetProductAsync(Guid categoryId, Guid productId, bool trackChanges)
    {
        var category = await _repository.CategoryRepository.GetCategoryAsync(categoryId, trackChanges);
        if (category == null)
            throw new CategoryNotFoundException(categoryId);
        var product = await _repository.ProductRepository.GetProductAsync(category.Id, productId, trackChanges);
        if (product == null)
            throw new ProductNotFoundException(productId);
        var productDto = _mapper.Map<ProductDto>(product);
        return productDto;
    }

    public async Task<ProductDto> CreateProductForCategoryAsync(Guid categoryId, ProductForCreationDto productForCreationDto, IFormFile? image,
        bool trackChanges)
    {
        _logger.LogInfo($"CreateProductForCategoryAsync called for category {categoryId}. Image present: {image != null}");
        var category = await _repository.CategoryRepository.GetCategoryAsync(categoryId, trackChanges);
        if (category == null)
            throw new CategoryNotFoundException(categoryId);
        var product = _mapper.Map<Product>(productForCreationDto);
        
        // Handle image upload if provided
        if (image != null && image.Length > 0)
        {
            // Validate content type and size
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(image.ContentType?.ToLowerInvariant()))
                throw new ImageBadRequestException("Invalid image format. Allowed: jpeg, png, gif.");

            if (image.Length > 5 * 1024 * 1024) // 5 MB limit (example)
                throw new ImageBadRequestException("Image too large. Max 5 MB.");

            var fileExt = Path.GetExtension(image.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExt}";

            try
            {
                var fileService = _fileServiceFactory.Create(FileServiceType.Local);
                var publicUrl = await fileService.UploadFile(image, fileName);
                _logger.LogInfo($"Image uploaded to Local Storage. URL: {publicUrl}");
                product.ImageUrl = publicUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Image upload failed: {ex.Message}");
                throw; // rethrow so caller gets failure
            }
        }
        
        _repository.ProductRepository.CreateProductForCategory(categoryId, product);
        await _repository.SaveAsync();
        var productDto = _mapper.Map<ProductDto>(product);
        return productDto;
    }

    public async Task DeleteProductForCategoryAsync(Guid categoryId, Guid productId, bool trackChanges)
    {
        var category = await _repository.CategoryRepository.GetCategoryAsync(categoryId, trackChanges);
        if (category == null)
            throw new CategoryNotFoundException(categoryId);
        var product = await _repository.ProductRepository.GetProductAsync(category.Id, productId, trackChanges);
        if (product == null)
            throw new ProductNotFoundException(productId);
        _repository.ProductRepository.DeleteProduct(product);
        await _repository.SaveAsync();
    }

    public async Task UpdateProductForCategoryAsync(Guid categoryId, Guid productId, ProductForUpdateDto productForUpdateDto,
        bool catTrackChanges, bool proTrackChanges)
    {
        var category = await _repository.CategoryRepository.GetCategoryAsync(categoryId, catTrackChanges);
        if (category == null)
            throw new CategoryNotFoundException(categoryId);
        var product = await _repository.ProductRepository.GetProductAsync(category.Id, productId, proTrackChanges);
        if (product == null)
            throw new ProductNotFoundException(productId);
        _mapper.Map(productForUpdateDto, product);
        await _repository.SaveAsync();
    }

    public async Task<(ProductForUpdateDto productForUpdateDto, Product product)> GetProductForPatchAsync(Guid categoryId, Guid productId,
        bool catTrackChanges, bool proTrackChanges)
    {
        var category = await _repository.CategoryRepository.GetCategoryAsync(categoryId, catTrackChanges);
        if (category == null)
            throw new CategoryNotFoundException(categoryId);
        var product = await _repository.ProductRepository.GetProductAsync(category.Id, productId, proTrackChanges);
        if (product == null)
            throw new ProductNotFoundException(productId);
        var productForUpdateDto = _mapper.Map<ProductForUpdateDto>(product);
        return (productForUpdateDto, product);
    }

    public async Task SaveChangesForPatchAsync(ProductForUpdateDto productForUpdateDto, Product product)
    {
        _mapper.Map(productForUpdateDto, product);
        await _repository.SaveAsync();
    }
}