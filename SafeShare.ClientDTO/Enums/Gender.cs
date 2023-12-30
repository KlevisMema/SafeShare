﻿using System.Text.Json.Serialization;

namespace SafeShare.ClientDTO.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Gender
{
    SelectGender = 0,
    Male = 1,
    Female = 2
}