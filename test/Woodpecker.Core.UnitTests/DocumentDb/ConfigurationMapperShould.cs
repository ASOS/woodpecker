using System;

using FluentAssertions;
using Woodpecker.Core.Metrics.Configuration;
using Xunit;

namespace Woodpecker.Core.UnitTests.DocumentDb
{
    public class ConfigurationMapperShould
    {
        private ConfigurationMapper Sut;

        public ConfigurationMapperShould()
        {
            Sut = new ConfigurationMapper();
        }

        [Fact]
        public void Map_Configuration_String_To_Configuration()
        {
            //Arrange
            var configurationString = "TenantId=MyTestTenant1.onmicrosoft.com;ClientId=B6BC8DAF-C802-457B-B906-4E9A9483A36C;ClientSecret=cfrWa1+UAHSdas23hsdlkjf8hdiashdasd=;ResourceId=subscriptions/mySub/resourceGroups/myRG/providers/Microsoft.DocumentDB/databaseAccounts/myDBAcc/databases/myDB12==/collections/myColl234=";
            var expectedResult = new Configuration() { ResourceId = "subscriptions/mySub/resourceGroups/myRG/providers/Microsoft.DocumentDB/databaseAccounts/myDBAcc/databases/myDB12==/collections/myColl234=", TenantId = "MyTestTenant1.onmicrosoft.com", ClientSecret = "cfrWa1+UAHSdas23hsdlkjf8hdiashdasd=", ClientId = "B6BC8DAF-C802-457B-B906-4E9A9483A36C" };

            //Act
            var actualResult = Sut.Map(configurationString);

            //Assert
            expectedResult.Should().BeEquivalentTo(actualResult);
        }

        [Fact]
        public void Map_Configuration_With_Spaces_In_Connection_String_To_Configuration()
        {
            //Arrange
            var configurationString = "TenantId=MyTestTenant1.onmicrosoft.com; ClientId    =  B6BC8DAF-C802-457B-B906-4E9A9483A36C ; ClientSecret = cfrWa1+UAHSdas23hsdlkjf8hdiashdasd=;ResourceId =subscriptions/mySub/resourceGroups/myRG/providers/Microsoft.DocumentDB/databaseAccounts/myDBAcc/databases/myDB12==/collections/myColl234= ";
            var expectedResult = new Configuration() { ResourceId = "subscriptions/mySub/resourceGroups/myRG/providers/Microsoft.DocumentDB/databaseAccounts/myDBAcc/databases/myDB12==/collections/myColl234=", TenantId = "MyTestTenant1.onmicrosoft.com", ClientSecret = "cfrWa1+UAHSdas23hsdlkjf8hdiashdasd=", ClientId = "B6BC8DAF-C802-457B-B906-4E9A9483A36C" };

            //Act
            var actualResult = Sut.Map(configurationString);

            //Assert
            expectedResult.Should().BeEquivalentTo(actualResult);
        }

        [Fact]
        public void Throw_When_Number_Of_Configuration_Pairs_Is_5()
        {
            //Arrange
            var configurationString = "TenantId=id587;ClientId=resourceGroup12;ClientSecret=documentAccount45;ResourceId=dbId45;Id=5";

            //Act
            var resultException = Assert.Throws<ArgumentException>(() => Sut.Map(configurationString));

            //Assert
            Assert.Equal(resultException.Message, "The argument count must be 4 in the connection string: 'TenantId=id587;ClientId=resourceGroup12;ClientSecret=documentAccount45;ResourceId=dbId45;Id=5'.");
        }

        [Fact]
        public void Throw_When_The_Configuration_String_Ends_With_Semicolon()
        {
            //Arrange
            var configurationString = "TenantId=id587;ClientId=resourceGroup12;ClientSecret=documentAccount45;ResourceId=dbId45;";

            //Act
            var resultException = Assert.Throws<ArgumentException>(() => Sut.Map(configurationString));

            //Assert
            Assert.Equal(resultException.Message, "The name value pair '' must be separated by '=' in the connection string.");
        }

        [Fact]
        public void Throw_When_Number_Of_Configuration_Pairs_Is_3()
        {
            //Arrange
            var configurationString = "TenantId=id587;ClientId=resourceGroup12;ClientSecret=documentAccount45";

            //Act
            var resultException = Assert.Throws<ArgumentException>(() => Sut.Map(configurationString));

            //Assert
            Assert.Equal(resultException.Message, "The argument count must be 4 in the connection string: 'TenantId=id587;ClientId=resourceGroup12;ClientSecret=documentAccount45'.");
        }

        [Fact]
        public void Throw_When_Pair_Separator_Is_Invalid()
        {
            //Arrange
            var configurationString = "TenantId-id587;ClientId=resourceGroup12;ClientSecret=documentAccount45;ResourceId=dbId45;CollectionId=collId95";

            //Act
            var resultException = Assert.Throws<ArgumentException>(() => Sut.Map(configurationString));

            //Assert
            Assert.Equal(resultException.Message, "The name value pair 'TenantId-id587' must be separated by '=' in the connection string.");
        }

        [Theory()]
        [InlineData("TenantIdx=id587;ClientId=resourceGroup12;ClientSecret=documentAccount45;ResourceId=dbId45", "The name 'TenantId' is not present in the connection string.")]
        [InlineData("TenantId=id587;ClientIdx=resourceGroup12;ClientSecret=documentAccount45;ResourceId=dbId45", "The name 'ClientId' is not present in the connection string.")]
        [InlineData("TenantId=id587;ClientId=resourceGroup12;ClientSecretx=documentAccount45;ResourceId=dbId45", "The name 'ClientSecret' is not present in the connection string.")]
        [InlineData("TenantId=id587;ClientId=resourceGroup12;ClientSecret=documentAccount45;ResourceIdx=dbId45", "The name 'ResourceId' is not present in the connection string.")]
        public void Throw_When_Configuration_Name_Is_Invalid(string connectionString, string errorMessage)
        {
            //Act
            var resultException = Assert.Throws<ArgumentException>(() => Sut.Map(connectionString));

            //Assert
            Assert.Equal(resultException.Message, errorMessage);
        }

        [Theory()]
        [InlineData("TenantId= ;ClientId=resourceGroup12;ClientSecret=documentAccount45;ResourceId=dbId45", "The value of 'TenantId' should not be empty or whitespace value in the connection string.")]
        [InlineData("TenantId=id587;ClientId=;ClientSecret=documentAccount45;ResourceId=dbId45", "The value of 'ClientId' should not be empty or whitespace value in the connection string.")]
        [InlineData("TenantId=id587;ClientId=resourceGroup12;ClientSecret=  ;ResourceId=dbId45", "The value of 'ClientSecret' should not be empty or whitespace value in the connection string.")]
        [InlineData("TenantId=id587;ClientId=resourceGroup12;ClientSecret=documentAccount45;ResourceId=", "The value of 'ResourceId' should not be empty or whitespace value in the connection string.")]
        public void Throw_When_Configuration_Value_Is_Empty_Or_Whitespace(string connectionString, string errorMessage)
        {
            //Act
            var resultException = Assert.Throws<ArgumentException>(() => Sut.Map(connectionString));

            //Assert
            Assert.Equal(resultException.Message, errorMessage);
        }
    }
}