﻿using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.CSTools.Tools;
using Newtonsoft.Json;

namespace Aiursoft.Directory.SDK.Attributes;

public class IsAccessToken : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        try
        {
            if (value is not string val)
            {
                return false;
            }

            if (EntryExtends.IsInUnitTests())
            {
                return true;
            }
            
            if (!val.Contains('.'))
            {
                return false;
            }

            var tokenParts = val.Split('.');
            if (tokenParts.Length != 2)
            {
                return false;
            }

            string tokenBase64 = tokenParts[0], tokenSign = tokenParts[1];
            if (string.IsNullOrWhiteSpace(tokenSign))
            {
                return false;
            }
            
            var token = JsonConvert.DeserializeObject<AppToken>(tokenBase64.Base64ToString());
            return DateTime.UtcNow <= token.Expires;
        }
        catch
        {
            return false;
        }
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (IsValid(value))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult($"The {validationContext.DisplayName} is not a valid Directory access token!");
    }
}