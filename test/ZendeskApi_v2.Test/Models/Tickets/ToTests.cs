using Newtonsoft.Json;
using Xunit;
using ZendeskApi_v2.Models.Tickets;

namespace Tests.Models.Tickets
{
    public class ToTests
    {
        private const string AllFieldsJson = "{\"address\":\"Test\",\"name\":\"Caller +49 89 555 666777\",\"formatted_phone\":\"+49 89 555 666777\",\"phone\":\"+4989555666777\"}";

        [Fact]
        public void DeserializeAllFieldsTest()
        {
            var to = JsonConvert.DeserializeObject<To>(AllFieldsJson);

            Assert.NotNull(to);
            Assert.Equal("+49 89 555 666777", to.FormattedPhone);
            Assert.Equal("+4989555666777", to.Phone);
            Assert.Equal("Caller +49 89 555 666777", to.Name);
            Assert.Equal("Test", to.Address);
        }

        [Fact]
        public void SerializeAllFieldsTest()
        {
            var to = new To
            {
                Address = "Test",
                Name = "Caller +49 89 555 666777",
                Phone = "+4989555666777",
                FormattedPhone = "+49 89 555 666777"
            };

            var toJson = JsonConvert.SerializeObject(to);

            Assert.NotNull(toJson);
            Assert.Equal(AllFieldsJson, toJson);
        }
    }
}
