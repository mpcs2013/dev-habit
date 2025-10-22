using DevHabit.Api.Database;
using DevHabit.Api.DTOs.HabitTags;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;


[ApiController]
[Route("habits/{habitId}/tags")]
public sealed class HabitTagsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult> UpsertHabitTags(string habitId, UpsertHabitTagsDto upsertHabitTagsDto)
    {
        Habit? habit = await dbContext.Habits
            .Include(h => h.HabitTags)
            .FirstOrDefaultAsync(h => h.Id == habitId);

        if (habit is null)
        {
            return NotFound();
        }

        var currentTagIds = habit.HabitTags
            .Select(ht => ht.TagId).ToHashSet();
        if(currentTagIds.SetEquals(upsertHabitTagsDto.TagIds))
        {
            return NoContent();
        }

        List<string> existingTagsIds = await dbContext
            .Tags
            .Where(t => upsertHabitTagsDto.TagIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync();
        if(existingTagsIds.Count != upsertHabitTagsDto.TagIds.Count)
        {
            return BadRequest("One or more tag IDs is invalid");
        }

        habit.HabitTags.RemoveAll(ht => !upsertHabitTagsDto.TagIds.Contains(ht.TagId));

        string[] tagIdsToAdd = upsertHabitTagsDto.TagIds.Except(currentTagIds).ToArray();
        habit.HabitTags.AddRange(tagIdsToAdd.Select(tagId => new HabitTag
        {
            HabitId = habitId,
            TagId = tagId,
            CreatedAtUtc = DateTime.UtcNow

        }));

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{tagId}")]
    public async Task<ActionResult> AddTagToHabit(string habitId, string tagId)
    {
        HabitTag? habitTag = await dbContext.HabitTags
            .SingleOrDefaultAsync(ht => ht.HabitId == habitId && ht.TagId == tagId);

        if(habitTag is not null)
        {
            return NoContent();
        }

        string? existTagId = await dbContext
            .Tags
            .Where(t => t.Id == tagId)
            .Select(t => t.Id)
            .SingleOrDefaultAsync();
        
        if(string.IsNullOrEmpty(existTagId))
        {
            return BadRequest($"Tag ID {tagId} is invalid");
        }

        await dbContext.HabitTags.AddAsync(new HabitTag
                {
                    HabitId = habitId,
                    TagId = tagId,
                    CreatedAtUtc =  DateTime.UtcNow
                });

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    // habits/:id/tags/:tagId
    [HttpDelete("{tagId}")]
    public async Task<ActionResult> RemoveTagFromHabit(string habitId, string tagId)
    {
        HabitTag? habitTag = await dbContext.HabitTags
            .SingleOrDefaultAsync(ht => ht.HabitId == habitId && ht.TagId == tagId);

        if (habitTag is null)
        {
            return NotFound();
        }
        
        dbContext.HabitTags.Remove(habitTag);
        
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
