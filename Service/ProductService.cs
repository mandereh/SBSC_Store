using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class ProductService : IProductService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public ProductService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync(Guid categoryId, bool trackChanges)
    {
        var category = await _repository.CategoryRepository.GetCategoryAsync(categoryId, trackChanges);
        if(category == null)
            throw new CategoryNotFoundException(categoryId);
        var products = await _repository.ProductRepository.GetProductsAsync(category.Id, trackChanges);
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        return productDtos;
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

    public async Task<ProductDto> CreateProductForCategoryAsync(Guid categoryId, ProductForCreationDto productForCreationDto,
        bool trackChanges)
    {
        var category = await _repository.CategoryRepository.GetCategoryAsync(categoryId, trackChanges);
        if (category == null)
            throw new CategoryNotFoundException(categoryId);
        var product = _mapper.Map<Product>(productForCreationDto);
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
        _repository.SaveAsync();
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