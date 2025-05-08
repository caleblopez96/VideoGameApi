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
            new VideoGame
            {
                Id = 4,
                Title = "God of War Ragnarök",
                Platform = "PS5",
                Developer = "Santa Monica Studio",
                Publisher = "Sony Interactive Entertainment"
            },
            new VideoGame
            {
                Id = 5,
                Title = "Elden Ring",
                Platform = "PC",
                Developer = "FromSoftware",
                Publisher = "Bandai Namco Entertainment"
            },
            new VideoGame
            {
                Id = 6,
                Title = "Halo Infinite",
                Platform = "Xbox Series X",
                Developer = "343 Industries",
                Publisher = "Xbox Game Studios"
            },
            new VideoGame
            {
                Id = 7,
                Title = "Forza Horizon 5",
                Platform = "Xbox Series X",
                Developer = "Playground Games",
                Publisher = "Xbox Game Studios"
            },
            new VideoGame
            {
                Id = 8,
                Title = "Resident Evil 4 Remake",
                Platform = "PS5",
                Developer = "Capcom",
                Publisher = "Capcom"
            },
            new VideoGame
            {
                Id = 9,
                Title = "Hogwarts Legacy",
                Platform = "PS5",
                Developer = "Avalanche Software",
                Publisher = "Warner Bros. Games"
            },
            new VideoGame
            {
                Id = 10,
                Title = "Street Fighter 6",
                Platform = "PC",
                Developer = "Capcom",
                Publisher = "Capcom"
            },
            new VideoGame
            {
                Id = 11,
                Title = "Final Fantasy XVI",
                Platform = "PS5",
                Developer = "Square Enix",
                Publisher = "Square Enix"
            },
            new VideoGame
            {
                Id = 12,
                Title = "The Last of Us Part I",
                Platform = "PC",
                Developer = "Naughty Dog",
                Publisher = "Sony Interactive Entertainment"
            },
            new VideoGame
            {
                Id = 13,
                Title = "Sea of Thieves",
                Platform = "Xbox Series X",
                Developer = "Rare",
                Publisher = "Xbox Game Studios"
            },
            new VideoGame
            {
                Id = 14,
                Title = "Super Smash Bros. Ultimate",
                Platform = "Nintendo Switch",
                Developer = "Bandai Namco Studios",
                Publisher = "Nintendo"
            },
            new VideoGame
            {
                Id = 15,
                Title = "Mario Kart 8 Deluxe",
                Platform = "Nintendo Switch",
                Developer = "Nintendo EPD",
                Publisher = "Nintendo"
            },
            new VideoGame
            {
                Id = 16,
                Title = "Call of Duty: Modern Warfare III",
                Platform = "PS5",
                Developer = "Sledgehammer Games",
                Publisher = "Activision"
            },
            new VideoGame
            {
                Id = 17,
                Title = "Assassin's Creed Mirage",
                Platform = "PC",
                Developer = "Ubisoft Bordeaux",
                Publisher = "Ubisoft"
            },
            new VideoGame
            {
                Id = 18,
                Title = "Baldur's Gate 3",
                Platform = "PC",
                Developer = "Larian Studios",
                Publisher = "Larian Studios"
            },
            new VideoGame
            {
                Id = 19,
                Title = "Diablo IV",
                Platform = "PC",
                Developer = "Blizzard Entertainment",
                Publisher = "Blizzard Entertainment"
            },
            new VideoGame
            {
                Id = 20,
                Title = "Alan Wake 2",
                Platform = "PS5",
                Developer = "Remedy Entertainment",
                Publisher = "Epic Games Publishing"
            },

        ];

        // get all video games
        [HttpGet]
        public ActionResult<List<VideoGame>> GetVideoGame()
        {
            return Ok(VideoGames); // returns 200 (Ok) if found
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






        //* THIS DOES NOT WORK
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




        // get video game by id - make sure it only matches numbers
        [HttpGet("{id:int}")] // Add constraint to only match integers
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
    }
}