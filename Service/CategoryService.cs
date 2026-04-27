using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class CategoryService : ICategoryService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public CategoryService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(bool trackChanges)
    {
        var categories = await _repository.CategoryRepository.GetAllCategoriesAsync(trackChanges);
        // var categoriesDto = categories.Select(c => new CategoryDto(c.Id, c.Name, c.Description)).ToList();
        var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);
        return categoriesDto;
    }
    public async Task<CategoryDto> GetCategoryAsync(Guid categoryId, bool trackChanges)
    {
        var category = await _repository.CategoryRepository.GetCategoryAsync(categoryId, trackChanges);
        if (category == null)
            throw new CategoryNotFoundException(categoryId);
        var categoryDto = _mapper.Map<CategoryDto>(category);
        return categoryDto;
    }
    public async Task<CategoryDto> CreateCategoryAsync(CategoryForCreationDto categoryForCreationDto)
    {
        var category = _mapper.Map<Category>(categoryForCreationDto);
        _repository.CategoryRepository.CreateCategory(category);
       await _repository.SaveAsync();
        var categoryDto = _mapper.Map<CategoryDto>(category);
        return categoryDto;
    }

    public async Task<IEnumerable<CategoryDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        if (ids == null)
            throw new IdParametersBadRequestException("ids cannot be null");
        var categories = await _repository.CategoryRepository.GetByIdsAsync(ids, trackChanges);
        if (ids.Count() != categories.Count())
            throw new CollectionByIdsBadRequestException();
        var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
        return categoryDtos;
    }

    public async Task<(IEnumerable<CategoryDto> categories, string ids)> CreateCategoryCollectionAsync(
        IEnumerable<CategoryForCreationDto> categoryCollection)
    {
        if (categoryCollection == null)
            throw new CategoryCollectionBadRequest();
        var categoryEntities = _mapper.Map<IEnumerable<Category>>(categoryCollection);
        foreach (var category in categoryEntities)
        {
            _repository.CategoryRepository.CreateCategory(category);
        }
        await _repository.SaveAsync();
        var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categoryEntities);
        var ids = string.Join(",", categoryDtos.Select(x => x.Id));
        return (categories:  categoryDtos, ids: ids);
    }

    public async Task DeleteCategoryAsync(Guid categoryId, bool trackChanges)
    {
        var category = await _repository.CategoryRepository.GetCategoryAsync(categoryId, trackChanges);
        if (category == null)
            throw new CategoryNotFoundException(categoryId);
        _repository.CategoryRepository.DeleteCategory(category);
        await _repository.SaveAsync();
    }

    public async Task UpdateCategoryAsync(Guid categoryId, CategoryForUpdateDto categoryForUpdateDto, bool trackChanges)
    {
        var category = await _repository.CategoryRepository.GetCategoryAsync(categoryId, trackChanges);
        if (category == null)
            throw new CategoryNotFoundException(categoryId);
        _mapper.Map(categoryForUpdateDto, category);
        await _repository.SaveAsync();
    }
}