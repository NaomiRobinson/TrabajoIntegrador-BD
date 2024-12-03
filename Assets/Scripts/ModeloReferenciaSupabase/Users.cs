using Postgrest.Models;
using Postgrest.Attributes;

public class users : BaseModel
{
    [Column("id"), PrimaryKey]
    public int id { get; set; }

    [Column("username")]
    public string username { get; set; }


    [Column("password")]
    public string password { get; set; }

    [Column("age")]
    public int age { get; set; }

}
