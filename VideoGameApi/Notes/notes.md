# C# ASP.NET Core Web API — HTTP Endpoint Blueprints
---

## GET Endpoint

```csharp
// Route: GET /api/[controller]
[HttpGet]
public async Task<ActionResult<IEnumerable<VideoGame>>> GetVideoGames()
{
    return Ok(await _context.VideoGames.ToListAsync());
}
```

### Explanation

- `[HttpGet]` — Attribute marking this method to handle HTTP GET requests.
- `public async Task<ActionResult<IEnumerable<VideoGame>>>` — Method signature:
  - `async` — Enables asynchronous execution.
  - `Task<T>` — Represents the ongoing operation. In this case, it wraps
    `ActionResult<IEnumerable<VideoGame>>`.
  - `ActionResult<IEnumerable<VideoGame>>` — Represents an HTTP response that
    can return a list of `VideoGame` objects with appropriate status codes.
- `_context.VideoGames.ToListAsync()` — Fetches all records asynchronously from
  the `VideoGames` table.
- `return Ok(...)` — Returns HTTP 200 OK with the list of video games.

---

## POST Endpoint

```csharp
[HttpPost]
public async Task<ActionResult<VideoGame>> CreateVideoGame(VideoGame newGame)
{
    _context.VideoGames.Add(newGame);
    await _context.SaveChangesAsync();
    return CreatedAtAction(nameof(GetVideoGames), new { id = newGame.Id }, newGame);
}
```

### Explanation

- `[HttpPost]` — Marks the method for HTTP POST requests.
- `CreateVideoGame(VideoGame newGame)` — Accepts a `VideoGame` object from the
  request body.
- `Add(newGame)` — Adds the new game to the context.
- `await SaveChangesAsync()` — Commits the new record asynchronously.
- `CreatedAtAction(...)` — Returns HTTP 201 Created, includes the URI of the new
  resource and the created object.

---

## PUT Endpoint

```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateVideoGame(int id, VideoGame updatedGame)
{
    if (id != updatedGame.Id)
    {
        return BadRequest();
    }

    _context.Entry(updatedGame).State = EntityState.Modified;
    await _context.SaveChangesAsync();

    return NoContent();
}
```

### Explanation

- `[HttpPut("{id}")]` — Marks for HTTP PUT requests and expects an `id`
  parameter in the URL.
- `UpdateVideoGame(int id, VideoGame updatedGame)` — Accepts an `id` and the
  updated object from the request body.
- `if (id != updatedGame.Id)` — Validates ID consistency.
- `Entry(updatedGame).State = EntityState.Modified` — Flags the object as
  modified.
- `await SaveChangesAsync()` — Persists changes.
- `return NoContent()` — Returns HTTP 204 No Content, indicating success without
  returning data.

---

## DELETE Endpoint

```csharp
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteVideoGame(int id)
{
    var game = await _context.VideoGames.FindAsync(id);
    if (game == null)
    {
        return NotFound();
    }

    _context.VideoGames.Remove(game);
    await _context.SaveChangesAsync();

    return NoContent();
}
```

### Explanation

- `[HttpDelete("{id}")]` — Marks for HTTP DELETE requests with `id` as a URL
  parameter.
- `DeleteVideoGame(int id)` — Accepts an `id` for the resource to delete.
- `FindAsync(id)` — Retrieves the resource by ID.
- `if (game == null)` — Checks if resource exists.
- `Remove(game)` — Deletes the resource from the context.
- `await SaveChangesAsync()` — Saves the deletion.
- `return NoContent()` — Returns HTTP 204 No Content.

---

## Notes on `Task<T>` and `ActionResult<T>`

- `Task<T>`: Represents an asynchronous operation that returns a value of type
  `T`. Commonly used for async database or network calls.
- `ActionResult<T>`: Allows returning either a `T` object (like a `VideoGame`)
  or an HTTP response status (like `NotFound()`, `Ok()`, `NoContent()`, etc.).

---


#### Why might people use IEnumerable<T>?
Interface-based abstraction: if you don't care about list-specific methods (like .Add(), .Remove()), and just want to loop through the collection.

Future flexibility: if later you change from .ToListAsync() to something else (like .Where() + .AsEnumerable() or .ToArrayAsync()), you won’t have to update your method signature.

Cleaner contracts: This exposes less to consumers of your API. They don’t need to know it’s a list — just that it’s a collection you can iterate through.

When working with LINQ queries, IEnumerable<T> supports deferred execution — meaning the query isn't evaluated until you actually iterate over it. This can be useful for performance optimization, especially with large datasets or chained queries.