using Postgrest.Models;
using Postgrest.Attributes;
using System.Collections.Generic;



public class trivia : BaseModel
{
    [Column("id"), PrimaryKey]
    public int id { get; set; }

    [Column("category")]
    public string category { get; set; }

    public List<question> questions { get; set; }
}

