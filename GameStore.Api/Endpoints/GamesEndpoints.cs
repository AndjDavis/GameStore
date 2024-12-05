namespace GameStore.Api.Endpoints;

using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

public static class GamesEndpoints
{
    const string GetGameEndPoint = "GetGame";

    // minimal api (path, handler)
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        // GET /games
        group.MapGet(
            "/",
            (GameStoreContext dbContext) =>
                dbContext
                    .Games.Include(game => game.Genre)
                    .Select(game => game.ToGameSummaryDto())
                    .AsNoTracking()
        );

        // GET /games/1
        group
            .MapGet(
                "/{id}",
                (int id, GameStoreContext dbContext) =>
                {
                    Game? game = dbContext.Games.Find(id);
                    return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
                }
            )
            .WithName(GetGameEndPoint);

        // POST /games
        group.MapPost(
            "/",
            (CreateGameDto newGame, GameStoreContext dbContext) =>
            {
                Game game = newGame.ToEntity();

                dbContext.Games.Add(game);
                dbContext.SaveChanges();

                return Results.CreatedAtRoute(
                    GetGameEndPoint,
                    new { id = game.Id },
                    game.ToGameDetailsDto()
                );
            }
        );

        // PUT /games
        group.MapPut(
            "/{id}",
            (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
            {
                var existingGame = dbContext.Games.Find(id);

                if (existingGame is null)
                {
                    return Results.NotFound();
                }

                dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGame.ToEntity(id));
                dbContext.SaveChanges();

                return Results.NoContent();
            }
        );

        // DELETE /games/{id}
        group.MapDelete(
            "/{id}",
            (int id, GameStoreContext dbContext) =>
            {
                var game = dbContext.Games.Where(game => game.Id == id).ExecuteDelete();
                return Results.NoContent();
            }
        );

        return group;
    }
}
