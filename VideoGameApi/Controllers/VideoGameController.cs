using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace VideoGameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoGameController : ControllerBase
    {
        private static List<VideoGame> VideoGames = [
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

        // get all video games
        [HttpGet]
        public ActionResult<List<VideoGame>> GetVideoGame()
        {
            return Ok(VideoGames); // returns 200 (Ok) if found
        }

        // get video game by id 
        [HttpGet("{id:int}")]
        public ActionResult<VideoGame> GetVideoGameById(int id)
        {
            var game = VideoGames.FirstOrDefault(g => g.Id == id);
            if (game is null)
            {
                return NotFound(); // returns 404 (not found) if not found
            }
            else
            {
                return Ok(game); // returns 200 (Ok) and the value of 'game'
            }
        }

        // get all titles:
        [HttpGet("titles")] // this creates route: /api/VideoGame/titles
        public ActionResult<List<string>> GetVideoGameTitle()
        {
            var titles = VideoGames.Select(vg => vg.Title).ToList();
            return Ok(titles); // returns 200 (Ok) if found
        }

        // get all developers
        [HttpGet("developer")]
        public ActionResult<List<string>> GetVideoGameDeveloper()
        {
            var developer = VideoGames.Select(vg => vg.Developer).ToList();
            return Ok(developer); // returns 200 (Ok) if found
        }

        // get game by developer name
        [HttpGet("developers/{developerName}")]
        public ActionResult<List<VideoGame>> GetVideoGameByDevelopers(string developerName)
        {
            // StringComparison for better matching
            var games = VideoGames.Where(game =>
                game.Developer != null &&
                game.Developer.Equals(developerName, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            if (games.Count == 0)
            {
                return NotFound("No games by that developer"); // returns 404 if not found
            }
            return Ok(games); // returns 200 (Ok) if found
        }
        // TEST:
        // https://localhost:7227/api/VideoGame/Developers/CD%20Projekt%20Red

        // get all publishers:
        [HttpGet("publisher")]
        public ActionResult<List<string>> GetVideoGamePublishers()
        {
            var publisher = VideoGames.Select(vg => vg.Publisher).ToList();
            return Ok(publisher); // returns 200 (Ok) if found
        }

        // get games by platform
        [HttpGet("platform/{platformName}")]
        public ActionResult<List<VideoGame>> GetVideoGamesByPlatform(string platformName)
        {
            var games = VideoGames
                .Where(vg => vg.Platform != null && vg.Platform.Equals(platformName, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (games.Count == 0)
            {
                return NotFound(); // returns 404 (not found) if not found
            }
            else
            {
                return Ok(games); // returns 200 (Ok) and the value of 'game'
            }
        }
        // TEST: 
        // https://localhost:7227/api/VideoGame/Platform/PS5



        // create a brand new video game
        [HttpPost]
        public ActionResult<VideoGame> AddVideoGame(VideoGame newGame)
        {
            if (newGame is null)
            {
                return BadRequest(); // returns 400 (bad request) if null 
            }
            else
            {
                newGame.Id = VideoGames.Max(g => g.Id) + 1; // set the new game to get a new id at creation
                VideoGames.Add(newGame); // add the video game to the VideoGames list
                return CreatedAtAction(nameof(GetVideoGameById), new { id = newGame.Id }, newGame); // returns 201 (created response)
                // you have to pass CreatedAtAction 3 arguments:
                //  - arg1: the action name - The name of the action method that will handle a GET request for the newly created resource. Usually, it's a GetById style method.
                //  - arg2: An anonymous object containing route parameters needed to generate the URL for that action. This is how it knows which resource URL to point to.
                //  - arg3: The actual object to return in the response body.
            }
        }

        // update a video game
        [HttpPut("{id:int}")]
        // since this is updating an exisitng object it doesnt return anything
        // use IActionResult instad 
        public IActionResult UpdateVideoGame(int id, VideoGame updatedGame)
        {
            var game = VideoGames.FirstOrDefault(vg => vg.Id == id);
            if (game is null)
            {
                return NotFound();
            }
            else
            {
                game.Title = updatedGame.Title;
                game.Platform = updatedGame.Platform;
                game.Developer = updatedGame.Developer;
                game.Publisher = updatedGame.Publisher;
                return NoContent();
            }
        }


        // delete game
        [HttpDelete("{id:int}")]
        public ActionResult<VideoGame> DeleteVideoGame(int id)
        {
            var game = VideoGames.FirstOrDefault(g => g.Id == id);
            if (game is null)
            {
                return NotFound();
            }
            else
            {
                VideoGames.Remove(game);
                return Ok(game); // return NoContent() if you don't want to return the deleted game
            }
        }

    }
}