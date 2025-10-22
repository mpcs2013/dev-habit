using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Database.Configurations;

public class HabitTagConfiguration : IEntityTypeConfiguration<HabitTag>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<HabitTag> builder)
    {
        builder.HasKey(ht => new { ht.HabitId, ht.TagId });

        builder.HasOne<Habit>()
            .WithMany(h => h.HabitTags)
            .HasForeignKey(ht => ht.HabitId);

        builder.HasOne<Tag>()
            .WithMany()
            .HasForeignKey(ht => ht.TagId);
    }
}
