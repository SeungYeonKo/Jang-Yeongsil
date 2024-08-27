using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

[Serializable]
public class Rank
{
    [BsonId]
    public ObjectId Id;
    [BsonElement("Name")]
    public string Name;
    public int Score;
    public DateTime DateTime;
    public CharacterGender SelectCharacter;
    public int RankPosition;
}
