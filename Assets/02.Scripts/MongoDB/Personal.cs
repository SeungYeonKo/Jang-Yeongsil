using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

public enum RememberID
{
    Remember,
    Nope
}

public enum CharacterGender
{
    Male = 1,
    Female = 2
}

[Serializable]
public class Personal
{
    [BsonId]
    public ObjectId Id;
    public RememberID Remember;
    [BsonElement("Name")]
    public string Name;
    public string Password;
    public int Coins;
    public CharacterGender SelectCharacter;
}
[Serializable]
public class PersonalData
{
    public List<Personal> Data;

    public PersonalData(List<Personal> data)
    {
        Data = data;
    }
}