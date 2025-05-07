namespace Churchee.Module.X.Features.Tweets.Commands.EnableTweetsSync
{
    internal static class ViewTemplateData
    {

        internal static string TweetListing = @"

@model List<MediaItemModel>

 @if(Model.Any()){
    <div class=""tweet-slider"">
        <div class=""swiper-wrapper"">

                @for (int i = 0; i < Model.Count(); i++)
                {
                    <div class=""swiper-slide"">
                        <div class=""card"">
                            <div class=""card-header"">
                                <img class=""rounded-circle"" src=""https://pbs.twimg.com/profile_images/1277012778235150337/j-Ye_Uy2_normal.jpg"" width=""45"">
                                <div class=""handle""><a href=""https://x.com/bethelbedwas"">@@bethelbedwas</a></div>
                                <div class=""date"">@Model[i].CreatedDate.Value.ToString(""dd/MM/yyyy"")</div>
                            </div>
                            <div class=""card-body"">
                                <blockquote>
                                    <p>@Model[i].Html</p>
                                </blockquote>

                            </div>
                        </div>
                    </div>
                }
        </div>

        <div class=""swiper-pagination""></div>
    </div>
 }

";






    }
}
