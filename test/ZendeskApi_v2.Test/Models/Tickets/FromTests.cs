using Newtonsoft.Json;
using Xunit;
using ZendeskApi_v2.Models.Tickets;

namespace Tests.Models.Tickets
{
    public class FromTests
    {
        private const string AllFieldsJson = "{\"address\":\"Test\",\"name\":\"Caller +49 89 555 666777\",\"formatted_phone\":\"+49 89 555 666777\",\"phone\":\"+4989555666777\"}";

        [Fact]
        public void DeserializeAllFieldsTest()
        {
            var from = JsonConvert.DeserializeObject<From>(AllFieldsJson);

            Assert.NotNull(from);
            Assert.Equal("+49 89 555 666777", from.FormattedPhone);
            Assert.Equal("+4989555666777", from.Phone);
            Assert.Equal("Caller +49 89 555 666777", from.Name);
            Assert.Equal("Test", from.Address);
        }

        [Fact]
        public void SerializeAllFieldsTest()
        {
            var from = new From
            {
                Address = "Test",
                Name = "Caller +49 89 555 666777",
                Phone = "+4989555666777",
                FormattedPhone = "+49 89 555 666777"
            };

            var jsonFrom = JsonConvert.SerializeObject(from);

            Assert.NotNull(jsonFrom);
            Assert.Equal(AllFieldsJson, jsonFrom);
        }
    }
}
