using BotDataAccess;
using BotDataAccess.repositories;
using BotShared.models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Data;
using Xunit;

namespace BotUnitTests
{
    public class ChallengeRepoTests
    {
        private readonly ChallengeRepo repository;
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        public ChallengeRepoTests()
        {
            configuration = DbConfiguration.BuildConfiguration();
            connectionString = DbConfiguration.GetDatabaseConnectionString(configuration);
            repository = new ChallengeRepo(connectionString);
        }

        [Fact]
        public async Task AddAsync_InsertsChallengeSuccessfully()
        {
            /*// Arrange
            var newChallenge = new ChallengeModel
            {
                ServerId = 123,
                ChannelId = 456,
                CreationDate = DateTime.UtcNow,
                LeetcodeName = "Sample Challenge",
                LeetcodeId = 009
            };

            // Act
            await repository.AddAsync(newChallenge);

            // Assert
            using (var connection = new SqlConnection(connectionString))
            {
                var insertedChallenge = await connection.QuerySingleOrDefaultAsync<ChallengeModel>(
                "SELECT * FROM Challenges WHERE LeetcodeId = @LeetcodeId;", new { LeetcodeId = newChallenge.LeetcodeId });


                Assert.NotNull(insertedChallenge);
                Assert.Equal(newChallenge.LeetcodeName, insertedChallenge.LeetcodeName);
            }

            // Cleanup - remove the inserted test data to maintain isolation
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(
                    "DELETE FROM Challenges WHERE LeetcodeId = @LeetcodeId;",
                     new { LeetcodeId = newChallenge.LeetcodeId });
            }*/
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectChallenge_WhenIdExists()
        {
            /*/ Arrange
            var expectedChallenge = new ChallengeModel
            {
                ServerId = 123,
                ChannelId = 456,
                CreationDate = DateTime.UtcNow,
                LeetcodeName = "Sample Challenge",
                LeetcodeId = 6 // Ensure this ID exists in your test setup
            };


            var repository = new ChallengeRepo(connectionString);

            // Act
            var actualChallenge = await repository.GetChallengeByIdAsync(expectedChallenge.LeetcodeId);

            // Assert
            Assert.NotNull(actualChallenge);
            Assert.Equal(expectedChallenge.LeetcodeName, actualChallenge.LeetcodeName);*/

        }
    }
}