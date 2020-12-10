### DbContext FluentApi - Useful snippets
___
###### Uniqueness (No NULLs)

```csharp
 modelBuilder.Entity<MyTable>()
             .HasAlternateKey(r => r.Name);
```
___
###### Uniqueness (Many NULLs)
```csharp
modelBuilder.Entity<MyTable>()
            .HasIndex(r => r.Path)
            .IsUnique();
```
___
###### Uniqueness (Single NULL)
```csharp
// Add this to migration Up()
migrationBuilder.AddUniqueConstraint(
   name: "UQ_MyTable_Column1_Column2_Column3",
   table: "MyTable",
   columns: new[] { "Column1", "Column2", "Column3" });
```
___
###### Many-To-Many
```csharp
modelBuilder.Entity<JoinTable>()
            .HasAlternateKey(r => new { r.MyTableId, r.OtherTableId });
modelBuilder.Entity<JoinTable>()
            .HasOne(j => j.MyTable)
            .WithMany(r => r.JoinTable)
            .HasForeignKey(j => j.MyId);
modelBuilder.Entity<JoinTable>()
            .HasOne(j => j.OtherTable)
            .WithMany(r => r.JoinTable)
            .HasForeignKey(j => j.OtherTableId);
```
___
###### Default Values
```csharp
modelBuilder.Entity<MyTable>()
            .Property(r => r.MaxTokenMinutes)
            .HasDefaultValue(DEF_MAX_TOKEN_MINS);
```
___
###### Computed Columns
```csharp
modelBuilder.Entity<MyTable>()
            .Property(r => r.Score)
            .HasComputedColumnSql("8*Col1 + 4*Col2 + 2*Col3 + CASE WHEN PreRelease IS NULL THEN 1 ELSE 0 END");
```
___
###### Cross-Environment Seeding (Design-time)
```csharp
// Seed enum lookup table
modelBuilder.SeedEnum<LookupEntry<MyEnum>, MyEnum>();

// Other 'static' entries 
modelBuilder.Entity<MyTable>().HasData(new[]
{
    new MyTable { Id = -1, Name = "Music" },
    new MyTable { Id = -2, Name = "Pictures" },
    new MyTable { Id = -3, Name = "Videos" },
});
```