using Postgrest.Models;
using Postgrest.Attributes;

public class attempt : BaseModel
{
    [Column("id"), PrimaryKey]
    public int id { get; set; }

    [Column("score")]
    public string score { get; set; }

     [Column("time")]
    public string time { get; set; }


    [Column("correct_answercount")]
    public int correct_answercount { get; set; }

    [Column("trivia_id")]
    public int trivia_id { get; set; }

    [Column("users_id")]
    public int users_id { get; set; }
}