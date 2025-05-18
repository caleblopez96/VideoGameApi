using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using VideoGameApi.Data;
using VideoGameApi.Models;

namespace VideoGameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoGameController(VideoGameDbContext context) : ControllerBase
    {
        private readonly VideoGameDbContext _context = context; // this makes it so that you can reference the database 

        // get all video games
        [HttpGet]
        public async Task<ActionResult<List<VideoGame>>> GetVideoGame()
        {
            return Ok(await _context.VideoGames.ToListAsync()); // returns 200 (Ok) if found
        }

        // get video game by id 
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VideoGame>> GetVideoGameById(int id)
        {
            var game = await _context.VideoGames.FindAsync(id);
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
        public async Task<ActionResult<List<string>>> GetVideoGameTitle()
        {
            var titles = await _context.VideoGames.Select(vg => vg.Title).ToListAsync();
            return Ok(titles); // returns 200 (Ok) if found
        }

        // get all developers
        [HttpGet("developer")]
        public ActionResult<List<string>> GetVideoGameDeveloper()
        {
            var developer = _context.VideoGames.Select(vg => vg.Developer).ToList();
            return Ok(developer); // returns 200 (Ok) if found
        }

        // get game by developer name
        [HttpGet("developers/{developerName}")]
        public ActionResult<List<VideoGame>> GetVideoGameByDevelopers(string developerName)
        {
            var games = _context.VideoGames.Where(game =>
                game.Developer != null &&
                EF.Functions.Like(game.Developer.ToLower(), developerName.ToLower())
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
        public async Task<ActionResult<List<string>>> GetVideoGamePublishers()
        {
            var publisher = await _context.VideoGames.Select(vg => vg.Publisher).ToListAsync();
            return Ok(publisher); // returns 200 (Ok) if found
        }

        // get games by platform
        [HttpGet("platform/{platformName}")]
        public async Task<ActionResult<List<VideoGame>>> GetVideoGamesByPlatform(string platformName)
        {
            var games = await _context.VideoGames.Where(game =>
                game.Platform != null &&
                EF.Functions.Like(game.Platform.ToLower(), platformName.ToLower())
            ).ToListAsync();

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
        public async Task<ActionResult<VideoGame>> AddVideoGame(VideoGame newGame)
        {
            if (newGame is null)
            {
                return BadRequest(); // returns 400 (bad request) if null 
            }
            else
            {
                _context.VideoGames.Add(newGame); // adds the game to the database
                await _context.SaveChangesAsync(); // saves the changes in the database
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
        // use IActionResult instead 
        public async Task<IActionResult> UpdateVideoGame(int id, VideoGame updatedGame)
        {
            var game = await _context.VideoGames.FindAsync(id);
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

                await _context.SaveChangesAsync();

                return NoContent();
            }
        }


        // delete game
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<VideoGame>> DeleteVideoGame(int id)
        {
            var game = await _context.VideoGames.FindAsync(id);
            if (game is null)
            {
                return NotFound();
            }
            else
            {
                _context.VideoGames.Remove(game);
                await _context.SaveChangesAsync();
                return Ok(game); // return NoContent() if you don't want to return the deleted game
            }
        }

    }
}