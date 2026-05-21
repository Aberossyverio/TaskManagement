using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskApi.Features.TaskModule;

public partial class TaskEntityConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("mt_task");

        builder.HasKey(e => e.Id)
            .HasName("pk_mt_task");

        builder.HasIndex(e => e.Title, "uq_mt_task_title")
            .IsUnique();

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Ignore(e => e.CreateByStr);
        builder.Ignore(e => e.UpdateByStr);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasColumnName("title");

        builder.Property(e => e.Description)
            .IsUnicode(false)
            .HasColumnName("description");

        builder.Property(e => e.TaskStatus)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false)
            .HasColumnName("task_status");

        builder.Property(e => e.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(e => e.Status).HasColumnName("status");

        builder.Property(e => e.CreateBy).HasColumnName("create_by");
        builder.Property(e => e.CreateTime)
            .HasDefaultValueSql("(now())")
            .HasColumnType("timestamp with time zone")
            .HasColumnName("create_time");

        builder.Property(e => e.UpdateBy).HasColumnName("update_by");
        builder.Property(e => e.UpdateTime)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("update_time");

        OnConfigurePartial(builder);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TaskItem> builder);
}
