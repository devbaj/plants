using System;
using System.ComponentModel.DataAnnotations;

namespace belt
{
  public class User
  {
    [Key]
    public int UserID {get;set;}
    public string FirstName {get;set;}
    public string LastName {get;set;}
    public string Email {get;set;}
    public string HashedPassword {get;set;}
    public DateTime CreatedAt {get;set;}
    public DateTime UpdatedAt {get;set;}
  }
}