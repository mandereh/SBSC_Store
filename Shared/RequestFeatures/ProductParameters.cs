namespace Shared.RequestFeatures;

public class ProductParameters : RequestParameters
{
    public decimal MinPrice { get; set; } = 0;
    public decimal MaxPrice { get; set; } = decimal.MaxValue;
    public string SearchTerm { get; set; }

    public bool ValidPriceRange => MaxPrice > MinPrice;
}