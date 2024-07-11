using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

public enum RememberID
{
    Remember,
    Nope
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
    public int CharacterIndex;
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