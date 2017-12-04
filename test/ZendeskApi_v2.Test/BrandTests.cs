using Xunit;
using System.Linq;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Brands;
using System;

namespace Tests
{
    public class BrandTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);
        public BrandTests()
        {
            var brands = api.Brands.GetBrands();
            if (brands != null)
            {
                foreach (var brand in brands.Brands.Where(o => o.Name.Contains("Test Brand")))
                {
                    api.Brands.DeleteBrand(brand.Id.Value);
                }
            }
        }

        [Fact]
        public void CanGetBrands()
        {
            var res = api.Brands.GetBrands();
            Assert.True(res.Count > 0);

            var ind = api.Brands.GetBrand(res.Brands[0].Id.Value);
            Assert.Equal(ind.Brand.Id, res.Brands[0].Id);            
        }

        [Fact]
        public void CanCreateUpdateAndDeleteTriggers()
        {
            var brand = new Brand()
            {
                Name = "Test Brand",
                Active = true,
                Subdomain = string.Format("test-{0}", Guid.NewGuid())
            };

            var res = api.Brands.CreateBrand(brand);

            Assert.True(res.Brand.Id > 0);

            res.Brand.Name = "Test Brand Updated";
            var update        = api.Brands.UpdateBrand(res.Brand);
            Assert.Equal(update.Brand.Name, res.Brand.Name);

            Assert.True(api.Brands.DeleteBrand(res.Brand.Id.Value));
        }
      
    }
}