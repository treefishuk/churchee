namespace Churchee.Module.X.Tests.Features.Tweets.Commands.EnableTweetsSync
{
    using Churchee.Module.X.Features.Tweets.Commands.EnableTweetsSync;
    using System;
    using System.Text.Json;
    using Xunit;

    public class GetTweetsApiResponseTests
    {
        [Fact]
        public void Tweet_DefaultConstructor_InitializesPropertiesToEmpty()
        {
            var tweet = new Tweet();

            Assert.Equal(string.Empty, tweet.Text);
            Assert.Equal(string.Empty, tweet.Id);
            Assert.Equal(default, tweet.CreatedAt);
        }

        [Fact]
        public void GetTweetsApiResponse_DefaultConstructor_InitializesTweetsList()
        {
            var response = new GetTweetsApiResponse();

            Assert.NotNull(response.Tweets);
            Assert.Empty(response.Tweets);
        }

        [Fact]
        public void CanSerializeAndDeserializeTweet()
        {
            var tweet = new Tweet
            {
                Text = "Hello",
                Id = "abc123",
                CreatedAt = new DateTime(2024, 1, 1, 12, 0, 0)
            };

            var json = JsonSerializer.Serialize(tweet);
            var deserialized = JsonSerializer.Deserialize<Tweet>(json);

            Assert.Equal("Hello", deserialized?.Text);
            Assert.Equal("abc123", deserialized?.Id);
            Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0), deserialized?.CreatedAt);
        }

        [Fact]
        public void CanSerializeAndDeserializeGetTweetsApiResponse()
        {
            var response = new GetTweetsApiResponse
            {
                Tweets =
                [
                    new Tweet { Text = "Test", Id = "1", CreatedAt = DateTime.UtcNow }
                ]
            };

            var json = JsonSerializer.Serialize(response);
            var deserialized = JsonSerializer.Deserialize<GetTweetsApiResponse>(json);

            Assert.NotNull(deserialized?.Tweets);
            Assert.Single(deserialized.Tweets);
            Assert.Equal("Test", deserialized.Tweets[0].Text);
            Assert.Equal("1", deserialized.Tweets[0].Id);
        }
    }

}
