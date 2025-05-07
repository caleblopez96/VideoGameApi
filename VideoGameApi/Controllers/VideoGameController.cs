using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace VideoGameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoGameController : ControllerBase
    {
        // this has to be static. if not, when you go to add a video game,
        // you'll only ever have the 3 listed below
        private static List<VideoGame> videoGames = [
            new VideoGame
            {
                Id = 1,
                Title = "Spider-Man 2",
                Platform = "PS5",
                Developer = "Insomniac Games",
                Publisher = "Sony Interactive Entertainment"
            },
            new VideoGame
            {
                Id = 2,
                Title = "The Legend of Zelda: Breath of the Wild",
                Platform = "Nintendo Switch",
                Developer = "Nintendo EPD",
                Publisher = "Nintendo"
            },
            new VideoGame
            {
                Id = 3,
                Title = "CyberPunk 2077",
                Platform = "PC",
                Developer = "CD Projekt Red",
                Publisher = "CD Projekt"
            },
        ];

        [HttpGet]
        // When an HTTP GET request comes in for this controller, call this method.

        // ActionResult<List<VideoGame>> 
        // -> This method returns an ActionResult containing a List of VideoGame objects.
        // The ActionResult lets us return either the list OR an HTTP status code if needed.

        public ActionResult<List<VideoGame>> GetVideoGame()
        {
            return videoGames;
        }

    }
}
